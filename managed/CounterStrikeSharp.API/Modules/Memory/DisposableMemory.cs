/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

namespace CounterStrikeSharp.API.Modules.Memory
{
    public abstract class DisposableMemory : NativeObject, IDisposableMemory
    {
        internal static int _instances;

        internal static int Instances
        {
            get { return _instances; }
            set
            {
                _instances = value;

                // Should not happen?
                if (_instances < 0)
                {
                    _instances = 0;
                }
            }
        }

        private bool _disposed;

        public bool IsDisposed
        {
            get => _disposed;
            set => _disposed = value;
        }

        /// <summary>
        /// This is only <see langword="true"/> if the <see cref="NativeObject.Handle"/> under the hood is purely from the game.
        /// </summary>
        internal bool PurePointer { get; set; } = false;

        public DisposableMemory(IntPtr ptr) : base(ptr)
        {
            Instances++;
        }

        ~DisposableMemory()
        {
            if (!PurePointer)
            {
                (this as IDisposableMemory).DisposeInternal();
                Instances--;
            }
        }

        public virtual void Dispose()
        {
            if (PurePointer)
                return;

            // Dont call finalizer
            GC.SuppressFinalize(this);

            (this as IDisposableMemory).DisposeInternal();
            Instances--;
        }

        public virtual void ReleaseManaged()
            { }

        public virtual void ReleaseUnmanaged()
        {
            MemAlloc.Free(Handle);
        }
    }
}
