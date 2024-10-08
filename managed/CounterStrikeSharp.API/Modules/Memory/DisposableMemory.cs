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

using System;

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Memory
{
    public abstract class DisposableMemory : NativeObject, IDisposableMemory
    {
        internal static Type DisposableType = typeof(DisposableMemory);

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

        /// <summary>
        /// Recursively checks if the given type is (or contains) a <see cref="DisposableMemory"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsDisposableType(Type type)
        {
            /* This part will be only needed if we have a correct implementation for `NetworkedVector<>` or any class that has any `DisposableMemory` as generic.
             * Until that, this would be overkill
            if (type == DisposableType || DisposableType.IsAssignableFrom(type))
                return true;

            if (type.IsGenericType)
            {
                foreach (Type arg in type.GetGenericArguments())
                {
                    if (DisposableType.IsAssignableFrom(arg))
                    {
                        return true;
                    }

                    if (IsDisposableType(arg))
                    {
                        return true;
                    }
                }
            }

            return type.BaseType != null && type.BaseType.Namespace!.StartsWith("CounterStrikeSharp") && IsDisposableType(type.BaseType);
            */

            return type == DisposableType || DisposableType.IsAssignableFrom(type);
        }

        internal static void MarkAsPure(DisposableMemory disposable)
        {
            disposable.PurePointer = true;

            // we should not count these as they are not handled by us.
            Instances--;
        }

        /*
        internal static void MarkCollectionAsPure<T>(IEnumerable<T> collection) where T: DisposableMemory
        {
            foreach (DisposableMemory disposable in collection)
            {
                MarkAsPure(disposable);
            }
        }
        */

        /* Span<T> where T has pointer or reference: Only value types without pointers or references are supported.
        internal static void MarkSpanAsPure<T>(Span<T> collection)
        {
            foreach (T instance in collection)
            {
                if (instance is DisposableMemory disposable)
                {
                    MarkAsPure(disposable);
                }
            }
        }
        */

        internal static void MarkAsPure(object? instance)
        {
            if (instance == null)
                return;

            switch (instance)
            {
                case DisposableMemory disposable:
                    MarkAsPure(disposable);
                    break;

                /* since 'Networked vectors currently only support CHandle<T>' lets stab ourselves in the back in the future
                case NetworkedVector<Vector> vectors:
                    MarkCollectionAsPure<Vector>(vectors);
                    break;

                case NetworkedVector<Vector2D> vector2ds:
                    MarkCollectionAsPure<Vector2D>(vector2ds);
                    break;

                case NetworkedVector<Vector4D> vector4ds:
                    MarkCollectionAsPure<Vector4D>(vector4ds);
                    break;

                // 'Angle' is only referenced inside the 'Vector' class
                case NetworkedVector<QAngle> angles:
                    MarkCollectionAsPure<QAngle>(angles);
                    break;

                case NetworkedVector<Quaternion> quaternions:
                    MarkCollectionAsPure<Quaternion>(quaternions);
                    break;

                case NetworkedVector<matrix3x4_t> matrixes:
                    MarkCollectionAsPure<matrix3x4_t>(matrixes);
                    break;

                default: throw new NotSupportedException($"'MarkAsPure': type '{instance.GetType().Name}' is not supported.");
                */
                default: return;
            }
        }
    }
}
