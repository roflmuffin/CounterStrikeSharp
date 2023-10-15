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
using System.Linq;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory
{
    public class VirtualFunctionVoid<TArg1> : VirtualFunction
    {
        public VirtualFunctionVoid(IntPtr objPtr, string signature, DataType?[] arguments, DataType returnType) : base(
            objPtr, signature, arguments, returnType)
        {
        }

        public VirtualFunctionVoid(IntPtr objPtr, int offset, DataType?[] arguments, DataType returnType) : base(objPtr,
            offset, arguments, returnType)
        {
        }

        public VirtualFunctionVoid(IntPtr pointer) : base(pointer)
        {
        }

        public void Invoke(TArg1 arg1)
        {
            this.InvokeInternal(new object[] { arg1 });
        }
    }

    public class VirtualFunctionVoid<TArg1, TArg2> : VirtualFunction
    {
        public VirtualFunctionVoid(IntPtr objPtr, int offset, DataType?[] arguments, DataType returnType) : base(objPtr,
            offset, arguments, returnType)
        {
        }

        public VirtualFunctionVoid(IntPtr objPtr, string signature, DataType?[] arguments, DataType returnType) : base(
            objPtr, signature, arguments, returnType)
        {
        }

        public VirtualFunctionVoid(IntPtr pointer) : base(pointer)
        {
        }

        public void Invoke(TArg1 arg1, TArg2 arg2)
        {
            this.InvokeInternal(new object[] { arg1, arg2 });
        }
    }

    public class VirtualFunctionVoid<TArg1, TArg2, TArg3> : VirtualFunction
    {
        public VirtualFunctionVoid(IntPtr objPtr, int offset, DataType?[] arguments, DataType returnType) : base(objPtr,
            offset, arguments, returnType)
        {
        }

        public VirtualFunctionVoid(IntPtr pointer) : base(pointer)
        {
        }

        public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            this.InvokeInternal(new object[] { arg1, arg2, arg3 });
        }
    }

    public class VirtualFunction<TArg1, TReturnType> : VirtualFunction
    {
        public VirtualFunction(IntPtr objPtr, int offset, DataType?[] arguments, DataType returnType) : base(objPtr,
            offset, arguments, returnType)
        {
        }

        public VirtualFunction(IntPtr pointer) : base(pointer)
        {
        }

        public TReturnType Invoke(TArg1 arg)
        {
            return this.InvokeInternal<TReturnType>(new object[] { arg });
        }
    }


    public class VirtualFunction : NativeObject
    {
        private DataType?[] _arguments;

        private object[] _convertedArguments =>
            _arguments.Where(x => x.HasValue).Select(x => x.Value).Cast<object>().ToArray();

        private DataType _returnType;
        private IntPtr _entityPointer;
        private int _offset;
        private string _signature;

        public VirtualFunction(IntPtr objPtr, int offset, DataType?[] arguments, DataType returnType) : base(
            IntPtr.Zero)
        {
            _entityPointer = objPtr;
            _offset = offset;
            _arguments = arguments;
            _returnType = returnType;

            Handle = NativeAPI.CreateVirtualFunction(_entityPointer, _offset, _arguments.Length, (int)_returnType,
                _convertedArguments);
        }

        public VirtualFunction(IntPtr objPtr, string signature, DataType?[] arguments, DataType returnType) : base(
            IntPtr.Zero)
        {
            _entityPointer = objPtr;
            _signature = signature;
            _arguments = arguments;
            _returnType = returnType;

            Handle = NativeAPI.CreateVirtualFunctionBySignature(_entityPointer, "server", signature, _arguments.Length,
                (int)_returnType, _convertedArguments);
        }

        public T InvokeInternal<T>(object[] arguments)
        {
            return NativeAPI.ExecuteVirtualFunction<T>(Handle, arguments);
        }

        protected void InvokeInternal(object[] arguments)
        {
            NativeAPI.ExecuteVirtualFunction<object>(Handle, arguments);
        }

        private static void ExecuteFunction(IntPtr objPtr, int offset, DataType?[] argumentTypes, DataType returnType,
            object[] arguments)
        {
            var convertedArguments = argumentTypes.Where(x => x.HasValue).Select(x => x.Value).Cast<object>().ToArray();

            if (convertedArguments.Length != arguments.Length)
            {
                throw new Exception("Invalid arguments provided.");
            }

            var ptr = NativeAPI.CreateVirtualFunction(objPtr, offset, convertedArguments.Length, (int)returnType,
                convertedArguments);

            NativeAPI.ExecuteVirtualFunction<object>(ptr, arguments);
        }

        private static void ExecuteFunction(IntPtr objPtr, string signature, DataType?[] argumentTypes,
            DataType returnType, object[] arguments)
        {
            var convertedArguments = argumentTypes.Where(x => x.HasValue).Select(x => x.Value).Cast<object>().ToArray();

            if (convertedArguments.Length != arguments.Length)
            {
                throw new Exception("Invalid arguments provided.");
            }

            var ptr = NativeAPI.CreateVirtualFunctionBySignature(objPtr, Addresses.ServerPath, signature,
                convertedArguments.Length, (int)returnType, convertedArguments);

            NativeAPI.ExecuteVirtualFunction<object>(ptr, arguments);
        }

        public static VirtualFunctionVoid<TArg1, TArg2, TArg3> CreateObject<TArg1, TArg2, TArg3>(IntPtr objPtr,
            int offset)
        {
            var arguments = new[]
            {
                typeof(TArg1).ToDataType(),
                typeof(TArg2).ToDataType(),
                typeof(TArg3).ToDataType()
            };

            return new VirtualFunctionVoid<TArg1, TArg2, TArg3>(objPtr, offset, arguments, DataType.DATA_TYPE_VOID);
        }

        public static VirtualFunctionVoid<TArg1, TArg2> CreateObject<TArg1, TArg2>(IntPtr objPtr, int offset)
        {
            var arguments = new[]
            {
                typeof(TArg1).ToDataType(),
                typeof(TArg2).ToDataType(),
            };

            return new VirtualFunctionVoid<TArg1, TArg2>(objPtr, offset, arguments, DataType.DATA_TYPE_VOID);
        }

        public static VirtualFunctionVoid<TArg1> CreateObject<TArg1>(IntPtr objPtr, string signature)
        {
            var arguments = new[]
            {
                typeof(TArg1).ToDataType()
            };

            return new VirtualFunctionVoid<TArg1>(objPtr, signature, arguments, DataType.DATA_TYPE_VOID);
        }

        public static VirtualFunctionVoid<TArg1, TArg2> CreateObject<TArg1, TArg2>(IntPtr objPtr, string signature)
        {
            var arguments = new[]
            {
                typeof(TArg1).ToDataType(),
                typeof(TArg2).ToDataType(),
            };

            return new VirtualFunctionVoid<TArg1, TArg2>(objPtr, signature, arguments, DataType.DATA_TYPE_VOID);
        }

        public static Action<TArg1> Create<TArg1>(IntPtr objPtr, int offset)
        {
            return new((arg1) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, DataType.DATA_TYPE_VOID, new object[] { arg1 });
            });
        }

        public static Action<TArg1, TArg2> Create<TArg1, TArg2>(IntPtr objPtr, int offset)
        {
            return new((arg1, arg2) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, DataType.DATA_TYPE_VOID, new object[] { arg1, arg2 });
            });
        }

        public static Action<TArg1> Create<TArg1>(IntPtr objPtr, string signature)
        {
            return new((arg1) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType()
                };

                ExecuteFunction(objPtr, signature, arguments, DataType.DATA_TYPE_VOID, new object[] { arg1 });
            });
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
            TArg6, TArg7>(IntPtr objPtr, string signature)
        {
            return new((arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
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
                };

                ExecuteFunction(objPtr, signature, arguments, DataType.DATA_TYPE_VOID,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
            });
        }

        public static Action<TArg1, TArg2> Create<TArg1, TArg2>(IntPtr objPtr, string signature)
        {
            return new((arg1, arg2) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType()
                };

                ExecuteFunction(objPtr, signature, arguments, DataType.DATA_TYPE_VOID, new object[] { arg1, arg2 });
            });
        }

        public static Action<TArg1, TArg2, TArg3> Create<TArg1, TArg2, TArg3>(IntPtr objPtr, int offset)
        {
            return new((arg1, arg2, arg3) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType(),
                    typeof(TArg3).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, DataType.DATA_TYPE_VOID, new object[] { arg1, arg2, arg3 });
            });
        }

        public static Func<TArg1, TArg2, TResult> CreateFunc<TArg1, TArg2, TResult>(IntPtr objPtr, string signature)
        {
            return new((arg1, arg2) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType()
                };

                ExecuteFunction(objPtr, signature, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TResult> CreateFunc<TResult>(IntPtr objPtr, int offset)
        {
            return new(() =>
            {
                var arguments = Array.Empty<DataType?>();

                ExecuteFunction(objPtr, offset, arguments, typeof(TResult).ToDataType().Value, new object[] { });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TArg1, TArg2, TResult> CreateFunc<TArg1, TArg2, TResult>(IntPtr objPtr, int offset)
        {
            return new((arg1, arg2) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TArg1, TArg2, TArg3, TResult> CreateFunc<TArg1, TArg2, TArg3, TResult>(IntPtr objPtr,
            string signature)
        {
            return new((arg1, arg2, arg3) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType(),
                    typeof(TArg3).ToDataType()
                };

                ExecuteFunction(objPtr, signature, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2, arg3 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TArg1, TArg2, TArg3, TResult> CreateFunc<TArg1, TArg2, TArg3, TResult>(IntPtr objPtr,
            int offset)
        {
            return new((arg1, arg2, arg3) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType(),
                    typeof(TArg3).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2, arg3 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TResult> CreateFunc<TArg1, TArg2, TArg3, TArg4, TResult>(
            IntPtr objPtr, int offset)
        {
            return new((arg1, arg2, arg3, arg4) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType(),
                    typeof(TArg3).ToDataType(),
                    typeof(TArg4).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2, arg3, arg4 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> CreateFunc<TArg1, TArg2, TArg3, TArg4, TArg5,
            TResult>(IntPtr objPtr, int offset)
        {
            return new((arg1, arg2, arg3, arg4, arg5) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType(),
                    typeof(TArg3).ToDataType(),
                    typeof(TArg4).ToDataType(),
                    typeof(TArg5).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2, arg3, arg4, arg5 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> CreateFunc<TArg1, TArg2, TArg3, TArg4, TArg5,
            TResult>(IntPtr objPtr, string signature)
        {
            return new((arg1, arg2, arg3, arg4, arg5) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType(),
                    typeof(TArg3).ToDataType(),
                    typeof(TArg4).ToDataType(),
                    typeof(TArg5).ToDataType()
                };

                ExecuteFunction(objPtr, signature, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2, arg3, arg4, arg5 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> CreateFunc<TArg1, TArg2, TArg3, TArg4,
            TArg5, TArg6, TResult>(IntPtr objPtr, string signature)
        {
            return new((arg1, arg2, arg3, arg4, arg5, arg6) =>
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

                ExecuteFunction(objPtr, signature, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> CreateFunc<TArg1, TArg2, TArg3, TArg4,
            TArg5, TArg6, TResult>(IntPtr objPtr, int offset)
        {
            return new((arg1, arg2, arg3, arg4, arg5, arg6) =>
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

                ExecuteFunction(objPtr, offset, arguments, typeof(TResult).ToDataType().Value,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });

                return ScriptContext.GlobalScriptContext.GetResult<TResult>();
            });
        }

        public static Action<TArg1, TArg2, TArg3, TArg4> Create<TArg1, TArg2, TArg3, TArg4>(IntPtr objPtr, int offset)
        {
            return new((arg1, arg2, arg3, arg4) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType(),
                    typeof(TArg3).ToDataType(),
                    typeof(TArg4).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, DataType.DATA_TYPE_VOID,
                    new object[] { arg1, arg2, arg3, arg4 });
            });
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5> Create<TArg1, TArg2, TArg3, TArg4, TArg5>(IntPtr objPtr,
            int offset)
        {
            return new((arg1, arg2, arg3, arg4, arg5) =>
            {
                var arguments = new[]
                {
                    typeof(TArg1).ToDataType(),
                    typeof(TArg2).ToDataType(),
                    typeof(TArg3).ToDataType(),
                    typeof(TArg4).ToDataType(),
                    typeof(TArg5).ToDataType()
                };

                ExecuteFunction(objPtr, offset, arguments, DataType.DATA_TYPE_VOID,
                    new object[] { arg1, arg2, arg3, arg4, arg5 });
            });
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
            IntPtr objPtr, int offset)
        {
            return new((arg1, arg2, arg3, arg4, arg5, arg6) =>
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

                ExecuteFunction(objPtr, offset, arguments, DataType.DATA_TYPE_VOID,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
            });
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
            TArg6, TArg7>(IntPtr objPtr, int offset)
        {
            return new((arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
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

                ExecuteFunction(objPtr, offset, arguments, DataType.DATA_TYPE_VOID,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
            });
        }

        public VirtualFunction(IntPtr pointer) : base(pointer)
        {
        }
    }
}