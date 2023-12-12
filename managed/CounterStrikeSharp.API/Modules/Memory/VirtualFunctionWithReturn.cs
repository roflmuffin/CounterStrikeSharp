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

public partial class VirtualFunctionWithReturn<TResult>
{
    private Func<TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TResult>(signature);
    }
    
    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TResult>(objectPtr, offset);
    }

    public TResult Invoke()
    {
        return this.Function();
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TResult>
{
    private Func<TArg1, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1)
    {
        return this.Function(arg1);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TResult>
{
    private Func<TArg1, TArg2, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2)
    {
        return this.Function(arg1, arg2);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TResult>
{
    private Func<TArg1, TArg2, TArg3, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        return this.Function(arg1, arg2, arg3);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TResult>
{
    private Func<TArg1, TArg2, TArg3, TArg4, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
    {
        return this.Function(arg1, arg2, arg3, arg4);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>
{
    private Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
    {
        return this.Function(arg1, arg2, arg3, arg4, arg5);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>
{
    private Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
    {
        return this.Function(arg1, arg2, arg3, arg4, arg5, arg6);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>
{
    private Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
    {
        return this.Function(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>
{
    private Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
    {
        return this.Function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>
{
    private Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
    {
        return this.Function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }
}

public partial class VirtualFunctionWithReturn<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>
{
    private Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> Function;

    public VirtualFunctionWithReturn(string signature)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(signature);
    }

    public VirtualFunctionWithReturn(string signature, string binarypath)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(signature, binarypath);
    }

    public VirtualFunctionWithReturn(IntPtr objectPtr, int offset)
    {
        this.Function = VirtualFunction.Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(objectPtr, offset);
    }

    public TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10)
    {
        return this.Function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }
}