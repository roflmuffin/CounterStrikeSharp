using System;

namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference argument.

public class MemoryFunctionWithReturn<TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        Array.Empty<DataType>())
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        Array.Empty<DataType>())
    {
    }

    public TResult Invoke()
    {
        return InvokeInternal<TResult>();
    }
}

public class MemoryFunctionWithReturn<T1, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[] { typeof(T1).ToValidDataType() })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[] { typeof(T1).ToValidDataType() })
    {
    }

    public TResult Invoke(T1 arg1)
    {
        return InvokeInternal<TResult>(arg1);
    }
}

public class MemoryFunctionWithReturn<T1, T2, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[] { typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType() })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[] { typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType() })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2)
    {
        return InvokeInternal<TResult>(arg1, arg2);
    }
}

public class MemoryFunctionWithReturn<T1, T2, T3, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[] { typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType() })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[] { typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType() })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        return InvokeInternal<TResult>(arg1, arg2, arg3);
    }
}

public class MemoryFunctionWithReturn<T1, T2, T3, T4, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType()
        })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType()
        })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return InvokeInternal<TResult>(arg1, arg2, arg3, arg4);
    }
}

public class MemoryFunctionWithReturn<T1, T2, T3, T4, T5, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType()
        })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType()
        })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        return InvokeInternal<TResult>(arg1, arg2, arg3, arg4, arg5);
    }
}

public class MemoryFunctionWithReturn<T1, T2, T3, T4, T5, T6, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType()
        })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType()
        })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        return InvokeInternal<TResult>(arg1, arg2, arg3, arg4, arg5, arg6);
    }
}

public class MemoryFunctionWithReturn<T1, T2, T3, T4, T5, T6, T7, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType()
        })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType()
        })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        return InvokeInternal<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
}

public class MemoryFunctionWithReturn<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType()
        })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType()
        })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    {
        return InvokeInternal<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }
}

public class MemoryFunctionWithReturn<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType(), typeof(T9).ToValidDataType()
        })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType(), typeof(T9).ToValidDataType()
        })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
    {
        return InvokeInternal<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }
}

public class MemoryFunctionWithReturn<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> : BaseMemoryFunction
{
    public MemoryFunctionWithReturn(string signature) : base(signature, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType(), typeof(T9).ToValidDataType(),
            typeof(T10).ToValidDataType()
        })
    {
    }

    public MemoryFunctionWithReturn(string signature, string binarypath) : base(signature, binarypath, typeof(TResult).ToValidDataType(),
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType(),
            typeof(T4).ToValidDataType(), typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType(), typeof(T9).ToValidDataType(),
            typeof(T10).ToValidDataType()
        })
    {
    }

    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
    {
        return InvokeInternal<TResult>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }
}

#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.