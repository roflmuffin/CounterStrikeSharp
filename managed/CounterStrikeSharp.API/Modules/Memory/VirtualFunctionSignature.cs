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
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

#pragma warning disable CS8601 // Possible null reference assignment.

public partial class VirtualFunction
{
    private static Dictionary<string, IntPtr> _createdFunctions = new();

    private static IntPtr CreateVirtualFunctionBySignature(string signature, IEnumerable<DataType> argumentTypes,
        DataType returnType,
        object[] arguments)
    {
        if (!_createdFunctions.TryGetValue(signature, out var function))
        {
            try
            {
                function = NativeAPI.CreateVirtualFunctionBySignature(IntPtr.Zero, Addresses.ServerPath, signature,
                    argumentTypes.Count(), (int)returnType, arguments);
                _createdFunctions[signature] = function;
            }
            catch (Exception)
            {
            }
        }

        return function;
    }

    private static IntPtr CreateVirtualFunctionBySignature(string signature, string binarypath, IEnumerable<DataType> argumentTypes,
        DataType returnType,
        object[] arguments)
    {
        if (!_createdFunctions.TryGetValue(signature, out var function))
        {
            try
            {
                function = NativeAPI.CreateVirtualFunctionBySignature(IntPtr.Zero, binarypath, signature,
                    argumentTypes.Count(), (int)returnType, arguments);
                _createdFunctions[signature] = function;
            }
            catch (Exception)
            {
            }
        }

        return function;
    }

    #region Funcs
    public static Func<TResult> Create<TResult>(string signature)
    {
        var arguments = Enumerable.Empty<DataType>().ToArray();

        if (typeof(TResult).ToDataType() == null)
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments,
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return () => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { });
    }

    public static Func<TResult> Create<TResult>(string signature, bool bypasshook)
    {
        var arguments = Enumerable.Empty<DataType>().ToArray();

        if (typeof(TResult).ToDataType() == null)
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments,
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return () => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { });
    }

    public static Func<TArg1, TResult> Create<TArg1, TResult>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1 });
    }

    public static Func<TArg1, TResult> Create<TArg1, TResult>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1 });
    }

    public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, TResult>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1, arg2 });
    }

    public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, TResult>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2 });
    }

    public static Func<TArg1, TArg2, TArg3, TResult> Create<TArg1, TArg2, TArg3, TResult>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1, arg2, arg3 });
    }

    public static Func<TArg1, TArg2, TArg3, TResult> Create<TArg1, TArg2, TArg3, TResult>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2, arg3 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Create<TArg1, TArg2, TArg3, TArg4, TResult>(
        string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1, arg2, arg3, arg4 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Create<TArg1, TArg2, TArg3, TArg4, TResult>(
        string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2, arg3, arg4 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
        string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
        string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
        TArg6, TResult>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer,
            false, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
        TArg6, TResult>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer,
            bypasshook, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Create<TArg1, TArg2, TArg3, TArg4,
        TArg5, TArg6, TArg7, TResult>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Create<TArg1, TArg2, TArg3, TArg4,
        TArg5, TArg6, TArg7, TResult>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Create<TArg1, TArg2, TArg3,
        TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Create<TArg1, TArg2, TArg3,
        TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> Create<TArg1, TArg2,
        TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> Create<TArg1, TArg2,
        TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> Create<TArg1,
        TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType(),
            typeof(TArg10).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> Create<TArg1,
        TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType(),
            typeof(TArg10).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
    }

    #endregion

    #region FuncsBinary
    public static Func<TResult> Create<TResult>(string signature, string binarypath)
    {
        var arguments = Enumerable.Empty<DataType>().ToArray();

        if (typeof(TResult).ToDataType() == null)
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath,  arguments,
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return () => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { });
    }

    public static Func<TResult> Create<TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = Enumerable.Empty<DataType>().ToArray();

        if (typeof(TResult).ToDataType() == null)
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath,  arguments,
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return () => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { });
    }

    public static Func<TArg1, TResult> Create<TArg1, TResult>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1 });
    }

    public static Func<TArg1, TResult> Create<TArg1, TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1 });
    }

    public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, TResult>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1, arg2 });
    }

    public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2 });
    }

    public static Func<TArg1, TArg2, TArg3, TResult> Create<TArg1, TArg2, TArg3, TResult>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1, arg2, arg3 });
    }

    public static Func<TArg1, TArg2, TArg3, TResult> Create<TArg1, TArg2, TArg3, TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2, arg3 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Create<TArg1, TArg2, TArg3, TArg4, TResult>(
        string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1, arg2, arg3, arg4 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Create<TArg1, TArg2, TArg3, TArg4, TResult>(
        string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2, arg3, arg4 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
        string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
        string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
        TArg6, TResult>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer,
            false, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
        TArg6, TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer,
            bypasshook, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Create<TArg1, TArg2, TArg3, TArg4,
        TArg5, TArg6, TArg7, TResult>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Create<TArg1, TArg2, TArg3, TArg4,
        TArg5, TArg6, TArg7, TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Create<TArg1, TArg2, TArg3,
        TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Create<TArg1, TArg2, TArg3,
        TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> Create<TArg1, TArg2,
        TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> Create<TArg1, TArg2,
        TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> Create<TArg1,
        TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType(),
            typeof(TArg10).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
    }

    public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> Create<TArg1,
        TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType(),
            typeof(TArg10).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
            NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
    }
    #endregion

    #region Void Actions

    public static Action CreateVoid(string signature)
    {
        var arguments = Enumerable.Empty<DataType>().ToArray();

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments,
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return () => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { }); };
    }

    public static Action CreateVoid(string signature, bool bypasshook)
    {
        var arguments = Enumerable.Empty<DataType>().ToArray();

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments,
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return () => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { }); };
    }

    public static Action<TArg1> CreateVoid<TArg1>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1) => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1 }); };
    }

    public static Action<TArg1> CreateVoid<TArg1>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1) => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1 }); };
    }

    public static Action<TArg1, TArg2> CreateVoid<TArg1, TArg2>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1, arg2 });
        };
    }

    public static Action<TArg1, TArg2> CreateVoid<TArg1, TArg2>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2 });
        };
    }

    public static Action<TArg1, TArg2, TArg3> CreateVoid<TArg1, TArg2, TArg3>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1, arg2, arg3 });
        };
    }

    public static Action<TArg1, TArg2, TArg3> CreateVoid<TArg1, TArg2, TArg3>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2, arg3 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4> CreateVoid<TArg1, TArg2, TArg3, TArg4>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4> CreateVoid<TArg1, TArg2, TArg3, TArg4>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(
        string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(
        string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
        string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
        string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5,
        TArg6, TArg7>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5,
        TArg6, TArg7>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> CreateVoid<TArg1, TArg2, TArg3, TArg4,
        TArg5, TArg6, TArg7, TArg8>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> CreateVoid<TArg1, TArg2, TArg3, TArg4,
        TArg5, TArg6, TArg7, TArg8>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> CreateVoid<TArg1, TArg2, TArg3,
        TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> CreateVoid<TArg1, TArg2, TArg3,
        TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> CreateVoid<TArg1, TArg2,
        TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(string signature)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType(),
            typeof(TArg10).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> CreateVoid<TArg1, TArg2,
        TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(string signature, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType(),
            typeof(TArg10).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        };
    }

    #endregion

    #region Void Actions Binary
    public static Action CreateVoid(string signature, string binarypath)
    {
        var arguments = Enumerable.Empty<DataType>().ToArray();

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments,
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return () => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { }); };
    }

    public static Action CreateVoid(string signature, string binarypath, bool bypasshook)
    {
        var arguments = Enumerable.Empty<DataType>().ToArray();

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments,
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return () => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { }); };
    }

    public static Action<TArg1> CreateVoid<TArg1>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1) => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1 }); };
    }

    public static Action<TArg1> CreateVoid<TArg1>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1) => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1 }); };
    }

    public static Action<TArg1, TArg2> CreateVoid<TArg1, TArg2>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1, arg2 });
        };
    }

    public static Action<TArg1, TArg2> CreateVoid<TArg1, TArg2>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2 });
        };
    }

    public static Action<TArg1, TArg2, TArg3> CreateVoid<TArg1, TArg2, TArg3>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1, arg2, arg3 });
        };
    }

    public static Action<TArg1, TArg2, TArg3> CreateVoid<TArg1, TArg2, TArg3>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2, arg3 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4> CreateVoid<TArg1, TArg2, TArg3, TArg4>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4> CreateVoid<TArg1, TArg2, TArg3, TArg4>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(
        string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(
        string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
        string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
        string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5,
        TArg6, TArg7>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5,
        TArg6, TArg7>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> CreateVoid<TArg1, TArg2, TArg3, TArg4,
        TArg5, TArg6, TArg7, TArg8>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> CreateVoid<TArg1, TArg2, TArg3, TArg4,
        TArg5, TArg6, TArg7, TArg8>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> CreateVoid<TArg1, TArg2, TArg3,
        TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> CreateVoid<TArg1, TArg2, TArg3,
        TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> CreateVoid<TArg1, TArg2,
        TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(string signature, string binarypath)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType(),
            typeof(TArg10).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        };
    }

    public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> CreateVoid<TArg1, TArg2,
        TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(string signature, string binarypath, bool bypasshook)
    {
        var arguments = new[]
        {
            typeof(TArg1).ToDataType(),
            typeof(TArg2).ToDataType(),
            typeof(TArg3).ToDataType(),
            typeof(TArg4).ToDataType(),
            typeof(TArg5).ToDataType(),
            typeof(TArg6).ToDataType(),
            typeof(TArg7).ToDataType(),
            typeof(TArg8).ToDataType(),
            typeof(TArg9).ToDataType(),
            typeof(TArg10).ToDataType()
        };

        if (arguments.Any(x => x == null))
        {
            throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
        }

        var virtualFunctionPointer = CreateVirtualFunctionBySignature(signature, binarypath, arguments.Cast<DataType>(),
            DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

        return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
        {
            NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        };
    }
    #endregion
}

#pragma warning restore CS8601 // Possible  null reference assignment.