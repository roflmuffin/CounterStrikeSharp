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

public partial class VirtualFunctionVoid<TArg1> : MemoryFunctionVoid<TArg1>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2> : MemoryFunctionVoid<TArg1, TArg2>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3> : MemoryFunctionVoid<TArg1, TArg2, TArg3>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4> : MemoryFunctionVoid<TArg1, TArg2, TArg3, TArg4>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5> : MemoryFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> : MemoryFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> : MemoryFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6,
    TArg7>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> : MemoryFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5,
    TArg6, TArg7, TArg8>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> : MemoryFunctionVoid<TArg1, TArg2, TArg3, TArg4,
    TArg5, TArg6, TArg7, TArg8, TArg9>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}

public partial class
    VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> : MemoryFunctionVoid<TArg1, TArg2, TArg3,
    TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>
{
    public VirtualFunctionVoid(string signature) : base(signature)
    {
    }

    public VirtualFunctionVoid(string signature, string binarypath) : base(signature, binarypath)
    {
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset) : base(objectPtr, offset)
    {
    }

    public VirtualFunctionVoid(NativeObject instance, int offset) : base(instance.Handle, offset)
    {
    }

    public VirtualFunctionVoid(int offset) : base(typeof(TArg1).Name, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, int offset) : base(symbolName, offset)
    {
    }

    public VirtualFunctionVoid(string symbolName, string binaryPath, int offset) : base(symbolName, binaryPath, offset)
    {
    }

    public VirtualFunctionVoid(VTableBase vtable, int offset) : base(vtable, offset)
    {
    }
}