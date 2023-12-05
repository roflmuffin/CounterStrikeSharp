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

namespace CounterStrikeSharp.API.Modules.Memory;

public partial class VirtualFunctionVoid
{
    private Action Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid(signature, binarypath);
    }

    public void Invoke()
    {
        this.Function();
    }
}

public partial class VirtualFunctionVoid<TArg1>
{
    private Action<TArg1> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1)
    {
        this.Function(arg1);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2>
{
    private Action<TArg1, TArg2> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2)
    {
        this.Function(arg1, arg2);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3>
{
    private Action<TArg1, TArg2, TArg3> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3>(signature);
    }
    
    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        this.Function(arg1, arg2, arg3);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4>
{
    private Action<TArg1, TArg2, TArg3, TArg4> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
    {
        this.Function(arg1, arg2, arg3, arg4);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5>
{
    private Action<TArg1, TArg2, TArg3, TArg4, TArg5> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
    {
        this.Function(arg1, arg2, arg3, arg4, arg5);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
{
    private Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
    {
        this.Function(arg1, arg2, arg3, arg4, arg5, arg6);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>
{
    private Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
    {
        this.Function(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>
{
    private Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
    {
        this.Function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>
{
    private Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
    {
        this.Function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }
}

public partial class VirtualFunctionVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>
{
    private Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> Function;

    public VirtualFunctionVoid(string signature)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(signature);
    }

    public VirtualFunctionVoid(string signature, string binarypath)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(signature, binarypath);
    }

    public VirtualFunctionVoid(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(objectPtr, offset);
    }

    public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10)
    {
        this.Function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }
}