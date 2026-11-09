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
        private readonly Timer? _timer;
        private BatchList _pending;
        private BatchList _active;
        private BatchList _survivors;
        private Batch? _current;
        private int _cursor;
        private int _count;
        private int _cleaning;
        private bool _sweeping;
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
                    if (_pending.Tail is null || _pending.Tail.Count == BatchSize)
                    {
                        _pending.Add(RentBatch());
                    }

                    _pending.Tail!.Entries[_pending.Tail.Count++] = entry;
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
                // A failed batch allocation leaves the current record intact; retry without killing the timer thread.
                _timer!.Change(1000, 10);
                Console.Error.WriteLine($"NativeHandleTracker cleanup failed: {exception}");
            }
        }

        // A single consumer owns detached batches. New allocations never extend or interrupt the current sweep.
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
                if (!_sweeping)
                {
                    var gc = GC.CollectionCount(0);
                    // GC counts are only a hint: periodically rescan even if a background GC cleared weak handles later.
                    if (gc == _completedGc && Stopwatch.GetElapsedTime(_completedAt) < TimeSpan.FromSeconds(1))
                    {
                        return 0;
                    }

                    _sweepGc = gc;
                    _active = _survivors;
                    _survivors = default;
                    lock (_gate)
                    {
                        _active.Append(_pending);
                        _pending = default;
                    }
                    _sweeping = true;
                }

                for (var visited = 0; visited < budget; visited++)
                {
                    _current ??= _active.Take();
                    if (_current is null)
                    {
                        break;
                    }

                    ref var entry = ref _current.Entries[_cursor];
                    if (entry.Target.TryGetTarget(out var target))
                    {
                        if (_survivors.Tail is null || _survivors.Tail.Count == BatchSize)
                        {
                            _survivors.Add(RentBatch());
                        }

                        _survivors.Tail!.Entries[_survivors.Tail.Count++] = entry;
                        GC.KeepAlive(target);
                    }
                    else
                    {
                        Marshal.FreeHGlobal(entry.Handle);
                        entry.Target.Dispose();
                        Interlocked.Decrement(ref _count);
                        released++;
                    }

                    // Moving a weak handle transfers ownership: only its destination may subsequently dispose it.
                    entry = default;
                    if (++_cursor == _current.Count)
                    {
                        _current.Count = 0;
                        lock (_gate)
                        {
                            if (_pool.Count < PoolLimit)
                            {
                                _pool.Push(_current);
                            }
                        }
                        _current = null;
                        _cursor = 0;
                        if (_active.Head is null)
                        {
                            break;
                        }
                    }

                    // Soft time limit: a native free, scheduling or GC can exceed it.
                    if ((visited & 31) == 31 && Stopwatch.GetElapsedTime(started) >= duration)
                    {
                        break;
                    }
                }

                if (_current is null && _active.Head is null)
                {
                    _sweeping = false;
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

        private Batch RentBatch()
        {
            lock (_gate)
            {
                return _pool.Count > 0 ? _pool.Pop() : new Batch();
            }
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
            public Batch? Next;
        }

        private struct BatchList
        {
            public Batch? Head;
            public Batch? Tail;

            public void Add(Batch batch)
            {
                if (Tail is null)
                {
                    Head = batch;
                }
                else
                {
                    Tail.Next = batch;
                }

                Tail = batch;
            }

            public void Append(BatchList batches)
            {
                if (batches.Head is null)
                {
                    return;
                }

                if (Tail is null)
                {
                    Head = batches.Head;
                }
                else
                {
                    Tail.Next = batches.Head;
                }

                Tail = batches.Tail;
            }

            public Batch? Take()
            {
                var batch = Head;
                if (batch is null)
                {
                    return null;
                }

                Head = batch.Next;
                if (Head is null)
                {
                    Tail = null;
                }

                batch.Next = null;
                return batch;
            }
        }
    }
}
