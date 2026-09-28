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

using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace CounterStrikeSharp.API.Modules.Memory;

public partial class VirtualFunctionWithReturn<TArg1, TResult> : MemoryFunctionWithReturn<TArg1, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TResult> : MemoryFunctionWithReturn<TArg1, TArg2, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TResult> : MemoryFunctionWithReturn<TArg1, TArg2, TArg3, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TResult> : MemoryFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> : MemoryFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5,
    TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> : MemoryFunctionWithReturn<TArg1, TArg2, TArg3, TArg4,
    TArg5, TArg6, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> : MemoryFunctionWithReturn<TArg1, TArg2, TArg3,
    TArg4, TArg5, TArg6, TArg7, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> : MemoryFunctionWithReturn<TArg1, TArg2,
    TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> : MemoryFunctionWithReturn<TArg1,
    TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> : MemoryFunctionWithReturn<
    TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>
{
    public VirtualFunctionWithReturn(string signature) : base(signature)
    {
    }

    public VirtualFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionWithReturn(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionWithReturn(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionWithReturn(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionWithReturn(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}