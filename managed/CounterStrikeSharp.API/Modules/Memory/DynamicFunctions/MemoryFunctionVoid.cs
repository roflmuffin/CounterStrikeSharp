using System;

namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

public class MemoryFunctionVoid : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID, Array.Empty<DataType>())
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID, Array.Empty<DataType>())
    {
    }

    public void Invoke()
    {
        InvokeInternalVoid();
    }
}

public class MemoryFunctionVoid<T1> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[] { typeof(T1).ToValidDataType() })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[] { typeof(T1).ToValidDataType() })
    {
    }

    public void Invoke(T1 arg1)
    {
        InvokeInternalVoid(arg1);
    }
}

public class MemoryFunctionVoid<T1, T2> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[] { typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType() })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[] { typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType() })
    {
    }

    public void Invoke(T1 arg1, T2 arg2)
    {
        InvokeInternalVoid(arg1, arg2);
    }
}

public class MemoryFunctionVoid<T1, T2, T3> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[] { typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType() })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[] { typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(), typeof(T3).ToValidDataType() })
    {
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        InvokeInternalVoid(arg1, arg2, arg3);
    }
}

public class MemoryFunctionVoid<T1, T2, T3, T4> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType()
        })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType()
        })
    {
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        InvokeInternalVoid(arg1, arg2, arg3, arg4);
    }
}

public class MemoryFunctionVoid<T1, T2, T3, T4, T5> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType()
        })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType()
        })
    {
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        InvokeInternalVoid(arg1, arg2, arg3, arg4, arg5);
    }
}

public class MemoryFunctionVoid<T1, T2, T3, T4, T5, T6> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType()
        })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType()
        })
    {
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        InvokeInternalVoid(arg1, arg2, arg3, arg4, arg5, arg6);
    }
}

public class MemoryFunctionVoid<T1, T2, T3, T4, T5, T6, T7> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType()
        })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType()
        })
    {
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        InvokeInternalVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
}

public class MemoryFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType()
        })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType()
        })
    {
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    {
        InvokeInternalVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }
}

public class MemoryFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType(),
            typeof(T9).ToValidDataType()
        })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType(),
            typeof(T9).ToValidDataType()
        })
    {
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
    {
        InvokeInternalVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }
}

public class MemoryFunctionVoid<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : BaseMemoryFunction
{
    public MemoryFunctionVoid(string signature) : base(signature, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType(),
            typeof(T9).ToValidDataType(), typeof(T10).ToValidDataType()
        })
    {
    }

    public MemoryFunctionVoid(string signature, string binarypath) : base(signature, binarypath, DataType.DATA_TYPE_VOID,
        new[]
        {
            typeof(T1).ToValidDataType(), typeof(T2).ToValidDataType(),
            typeof(T3).ToValidDataType(), typeof(T4).ToValidDataType(),
            typeof(T5).ToValidDataType(), typeof(T6).ToValidDataType(),
            typeof(T7).ToValidDataType(), typeof(T8).ToValidDataType(),
            typeof(T9).ToValidDataType(), typeof(T10).ToValidDataType()
        })
    {
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
    {
        InvokeInternalVoid(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }
}