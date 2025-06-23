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
    /// <summary>
    /// Provides mechanism to release managed and unmanaged memory.
    /// Used for garbage collecting.
    /// </summary>
    public interface IDisposableMemory : IDisposable
    {
        /// <summary>
        /// Is the resource disposed?
        /// The class should implement a disposed field.
        /// </summary>
        public bool IsDisposed { get; set; }

        /// <summary>
        /// Clean up unmanaged resources inside this.
        /// This is always called on the main thread, its safe to use natives.
        /// <para><b>USED INTERNALLY BY <see cref="Core.Memory.MemoryManager"/></b></para>
        /// </summary>
        public void ReleaseUnmanaged();

        /// <summary>
        /// Acts as the normal managed 'Dispose'.
        /// <para><b>USED INTERNALLY BY <see cref="Core.Memory.MemoryManager"/></b></para>
        /// </summary>
        public void ReleaseManaged();

        /// <summary>
        /// Dispose internally
        /// </summary>
        internal void DisposeInternal()
        {
            if (!IsDisposed)
            {
                // Free managed resources
                ReleaseManaged();

                Server.NextWorldUpdate(() =>
                {
                    // Free unmanaged resources on the main thread
                    ReleaseUnmanaged();
                });

                IsDisposed = true;
            }
        }
    }
}
