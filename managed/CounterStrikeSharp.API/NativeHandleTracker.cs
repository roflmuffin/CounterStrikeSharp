using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace CounterStrikeSharp.API
{
    // Owns only AllocHGlobal allocations. The shared instance outlives plugins and never frees a live owner's memory.
    internal sealed class NativeHandleTracker
    {
        private const int BatchSize = 256;
        private const int PoolLimit = 16;
        private static readonly IntPtr _initializing = new(-1);
        internal static readonly NativeHandleTracker Shared = new(true);

        private readonly object _gate = new();
        private readonly Stack<Batch> _pool = new(PoolLimit);
        private readonly Queue<Batch> _batches = new();
        private readonly Timer? _timer;
        private Batch? _pending;
        private Batch? _current;
        private Batch? _survivor;
        private int _cursor;
        private int _surviving;
        private int _remaining;
        private int _count;
        private int _cleaning;
        private int _sweepGc;
        private int _completedGc = -1;
        private long _completedAt;

        internal int Count => Volatile.Read(ref _count);

        internal NativeHandleTracker(bool automaticCleanup = false)
        {
            if (automaticCleanup)
            {
                // The process-wide worker must not retain a plugin's ambient execution context.
                using var flow = ExecutionContext.IsFlowSuppressed() ? default : ExecutionContext.SuppressFlow();
                _timer = new Timer(_ => CleanupTick(), null, 10, 10);
            }
        }

        internal static void Track(NativeObject target, IntPtr handle)
        {
            Shared.Register(target, handle);
        }

        // Publish only after registration succeeds. Concurrent first access shares one allocation;
        // failed allocation/registration leaves the object retryable and owns no native memory.
        internal static unsafe IntPtr Allocate(NativeObject target, ref IntPtr ownedHandle, ReadOnlySpan<float> values)
        {
            var spinner = new SpinWait();
            while (true)
            {
                var handle = Volatile.Read(ref ownedHandle);
                if (handle != IntPtr.Zero && handle != _initializing)
                {
                    return handle;
                }

                if (handle == IntPtr.Zero && Interlocked.CompareExchange(ref ownedHandle, _initializing, IntPtr.Zero) == IntPtr.Zero)
                {
                    break;
                }

                spinner.SpinOnce();
            }

            var allocated = IntPtr.Zero;
            try
            {
                allocated = Marshal.AllocHGlobal(checked(values.Length * sizeof(float)));
                values.CopyTo(new Span<float>((void*)allocated, values.Length));
                Track(target, allocated);
                Volatile.Write(ref ownedHandle, allocated);
                return allocated;
            }
            catch
            {
                Marshal.FreeHGlobal(allocated);
                Volatile.Write(ref ownedHandle, IntPtr.Zero);
                throw;
            }
        }

        internal void Register(NativeObject target, IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            ArgumentNullException.ThrowIfNull(target);
            // Also protect derived objects that access their allocation in a finalizer or resurrect themselves.
            var entry = new Entry
            {
                Target = new WeakGCHandle<NativeObject>(target, trackResurrection: true),
                Handle = handle
            };

            try
            {
                lock (_gate)
                {
                    if (_pending?.Count == BatchSize)
                    {
                        _batches.Enqueue(_pending);
                        _pending = null;
                    }

                    _pending ??= _pool.TryPop(out var batch) ? batch : new Batch();
                    _pending.Entries[_pending.Count++] = entry;
                    Interlocked.Increment(ref _count);
                }
            }
            catch
            {
                entry.Target.Dispose();
                throw;
            }
            finally
            {
                GC.KeepAlive(target);
            }
        }

        private void CleanupTick()
        {
            try
            {
                Cleanup(Math.Clamp(Count / 32, 512, 4096), TimeSpan.FromMilliseconds(1));
            }
            catch (Exception exception)
            {
                // Keep an unexpected cleanup failure from terminating the timer thread.
                _timer!.Change(1000, 10);
                Console.Error.WriteLine($"NativeHandleTracker cleanup failed: {exception}");
            }
        }

        // Registration only writes its pending batch. The cleaner scans sealed batches outside the gate.
        internal int Cleanup(int budget, TimeSpan duration)
        {
            if (budget <= 0 || duration <= TimeSpan.Zero || Count == 0 || Interlocked.CompareExchange(ref _cleaning, 1, 0) != 0)
            {
                return 0;
            }

            var released = 0;
            var started = Stopwatch.GetTimestamp();
            try
            {
                if (_remaining == 0)
                {
                    var gc = GC.CollectionCount(0);
                    // GC counts are only a hint: periodically rescan even if a background GC cleared weak handles later.
                    if (gc == _completedGc && Stopwatch.GetElapsedTime(_completedAt) < TimeSpan.FromSeconds(1))
                    {
                        return 0;
                    }

                    _sweepGc = gc;
                    lock (_gate)
                    {
                        if (_pending is not null)
                        {
                            _batches.Enqueue(_pending);
                            _pending = null;
                        }
                        // Appending registrations and rotated survivors cannot extend this sweep.
                        _remaining = _batches.Count;
                    }
                }

                for (var visited = 0; visited < budget && _remaining > 0; visited++)
                {
                    if (_current is null)
                    {
                        lock (_gate)
                        {
                            _current = _batches.Dequeue();
                        }
                    }

                    if (_cursor < _current.Count)
                    {
                        ref var entry = ref _current.Entries[_cursor];
                        if (entry.Target.TryGetTarget(out var target))
                        {
                            // Leave dense live batches untouched; only move records across holes.
                            if (_surviving != _cursor)
                            {
                                _current.Entries[_surviving] = entry;
                                entry = default;
                            }
                            _surviving++;
                            GC.KeepAlive(target);
                        }
                        else
                        {
                            Marshal.FreeHGlobal(entry.Handle);
                            entry.Target.Dispose();
                            entry = default;
                            Interlocked.Decrement(ref _count);
                            released++;
                        }
                        _cursor++;
                    }

                    if (_cursor >= _current.Count)
                    {
                        FinishBatch();
                    }

                    // Soft time limit: a native free, scheduling or GC can exceed it.
                    if ((visited & 31) == 31 && Stopwatch.GetElapsedTime(started) >= duration)
                    {
                        break;
                    }
                }

                if (_remaining == 0)
                {
                    _survivor = null;
                    _completedGc = _sweepGc;
                    _completedAt = Stopwatch.GetTimestamp();
                }
                return released;
            }
            finally
            {
                Volatile.Write(ref _cleaning, 0);
            }
        }

        private void FinishBatch()
        {
            var batch = _current!;
            batch.Count = _surviving;
            // Both batches are already scanned; merging keeps sparse survivors from pinning whole arrays.
            if (batch.Count > 0 && _survivor is not null && _survivor.Count + batch.Count <= BatchSize)
            {
                Array.Copy(batch.Entries, 0, _survivor.Entries, _survivor.Count, batch.Count);
                _survivor.Count += batch.Count;
                Array.Clear(batch.Entries, 0, batch.Count);
                batch.Count = 0;
                _surviving = 0;
            }

            lock (_gate)
            {
                if (batch.Count > 0)
                {
                    _batches.Enqueue(batch);
                    _survivor = batch;
                }
                else if (_pool.Count < PoolLimit)
                {
                    _pool.Push(batch);
                }
            }
            // Retain the current batch until enqueue succeeds, so a failed queue growth is retryable.
            _current = null;
            _cursor = 0;
            _surviving = 0;
            _remaining--;
        }

        private struct Entry
        {
            public WeakGCHandle<NativeObject> Target;
            public IntPtr Handle;
        }

        private sealed class Batch
        {
            public readonly Entry[] Entries = new Entry[BatchSize];
            public int Count;
        }
    }
}
