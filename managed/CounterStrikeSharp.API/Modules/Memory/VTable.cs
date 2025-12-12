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
    public abstract class VTableBase : NativeObject
    {
        internal VTableBase(IntPtr ptr) : base(ptr)
            { }

        internal VTableBase(string symbolName) : this(NativeAPI.FindVirtualTable(Addresses.ServerPath, symbolName))
            { }

        internal VTableBase(string symbolName, string binaryPath) : this(NativeAPI.FindVirtualTable(binaryPath, symbolName))
            { }
    }

    /// <summary>
    /// Represents a low-level virtual table.
    /// </summary>
    public sealed class VTable : VTableBase
    {
        public VTable(IntPtr ptr) : base(ptr)
            { }

        public VTable(string symbolName) : base(symbolName)
            { }

        public VTable(string symbolName, string binaryPath) : base(binaryPath, symbolName)
            { }

        public VirtualFunctionVoid<TArg1> GetFunctionVoid<TArg1>(int index)
            => new VirtualFunctionVoid<TArg1>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2> GetFunctionVoid<TArg1, TArg2>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2, TArg3> GetFunctionVoid<TArg1, TArg2, TArg3>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2, TArg3>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4> GetFunctionVoid<TArg1, TArg2, TArg3, TArg4>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5> GetFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> GetFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> GetFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> GetFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> GetFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(this, index);

        public VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> GetFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(int index)
            => new VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(this, index);

        public VirtualFunctionWithReturn<TArg1, TResult> GetFunctionWithReturn<TArg1, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TResult> GetFunctionWithReturn<TArg1, TArg2, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TResult> GetFunctionWithReturn<TArg1, TArg2, TArg3, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TResult> GetFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> GetFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> GetFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> GetFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> GetFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> GetFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(this, index);

        public VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> GetFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(int index)
            => new VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(this, index);
    }

    /// <summary>
    /// Represents a low-level virtual table.
    /// This version is meant to be used with explicit class type. TClass will be passed as TArg1 on invocation.
    /// </summary>
    public sealed class VTable<TClass> : VTableBase
    {
        public VTable(IntPtr ptr) : base(ptr)
            { }

        public VTable() : base(typeof(TClass).Name)
            { }

        public VTable(string binaryPath) : base(typeof(TClass).Name, binaryPath)
            { }

        public VirtualFunctionVoid<TClass> GetFunctionVoid(int index)
            => new VirtualFunctionVoid<TClass>(this, index);

        public VirtualFunctionVoid<TClass, TArg2> GetFunctionVoid<TArg2>(int index)
            => new VirtualFunctionVoid<TClass, TArg2>(this, index);

        public VirtualFunctionVoid<TClass, TArg2, TArg3> GetFunctionVoid<TArg2, TArg3>(int index)
            => new VirtualFunctionVoid<TClass, TArg2, TArg3>(this, index);

        public VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4> GetFunctionVoid<TArg2, TArg3, TArg4>(int index)
            => new VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4>(this, index);

        public VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5> GetFunctionVoid<TArg2, TArg3, TArg4, TArg5>(int index)
            => new VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5>(this, index);

        public VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6> GetFunctionVoid<TArg2, TArg3, TArg4, TArg5, TArg6>(int index)
            => new VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6>(this, index);

        public VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> GetFunctionVoid<TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(int index)
            => new VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(this, index);

        public VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> GetFunctionVoid<TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(int index)
            => new VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(this, index);

        public VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> GetFunctionVoid<TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(int index)
            => new VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(this, index);

        public VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> GetFunctionVoid<TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(int index)
            => new VirtualFunctionVoid<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(this, index);

        public VirtualFunctionWithReturn<TClass, TResult> GetFunctionWithReturn<TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TResult> GetFunctionWithReturn<TArg2, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TArg3, TResult> GetFunctionWithReturn<TArg2, TArg3, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TArg3, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TResult> GetFunctionWithReturn<TArg2, TArg3, TArg4, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TResult> GetFunctionWithReturn<TArg2, TArg3, TArg4, TArg5, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> GetFunctionWithReturn<TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> GetFunctionWithReturn<TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> GetFunctionWithReturn<TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> GetFunctionWithReturn<TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(this, index);

        public VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> GetFunctionWithReturn<TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(int index)
            => new VirtualFunctionWithReturn<TClass, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(this, index);
    }
}