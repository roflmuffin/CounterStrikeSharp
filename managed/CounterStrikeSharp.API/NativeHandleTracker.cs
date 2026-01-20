using System.Threading;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API
{
    internal static class NativeHandleTracker
    {
        private sealed class Entry
        {
            public WeakReference<NativeObject> Target { get; }
            public IntPtr Handle { get; }

            public Entry(NativeObject target, IntPtr handle)
            {
                Target = new WeakReference<NativeObject>(target);
                Handle = handle;
            }
        }

        private static readonly List<Entry> _entries = new();
        private static readonly object _lockObj = new();
        private static int _nextCleanupIndex;
        private static Timer? _timer;

        public static void Track(NativeObject target, IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            lock (_lockObj)
            {
                _entries.Add(new Entry(target, handle));
                EnsureTimerStartedLocked();
                CleanupSomeLocked(1);
            }
        }

        private static void EnsureTimerStartedLocked()
        {
            if (_timer != null)
            {
                return;
            }

            _timer = new Timer(_ => CleanupSome(2048), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private static void CleanupSome(int budget)
        {
            lock (_lockObj)
            {
                CleanupSomeLocked(budget);
            }
        }

        private static void CleanupSomeLocked(int budget)
        {
            if (_entries.Count == 0)
            {
                return;
            }

            var processedCount = 0;

            while (processedCount < budget && _entries.Count > 0)
            {
                if (_nextCleanupIndex >= _entries.Count)
                {
                    _nextCleanupIndex = 0;
                }

                var entry = _entries[_nextCleanupIndex];

                if (!entry.Target.TryGetTarget(out _))
                {
                    Marshal.FreeHGlobal(entry.Handle);

                    var lastIndex = _entries.Count - 1;
                    _entries[_nextCleanupIndex] = _entries[lastIndex];
                    _entries.RemoveAt(lastIndex);
                    processedCount++;
                    continue;
                }

                _nextCleanupIndex++;
                processedCount++;
            }

            if (_entries.Count == 0)
            {
                _nextCleanupIndex = 0;
            }
        }
    }
}
