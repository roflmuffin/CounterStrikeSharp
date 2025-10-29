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

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

#pragma warning disable CS8601 // Possible null reference assignment.

namespace CounterStrikeSharp.API.Modules.Memory
{
    public partial class VirtualFunction
    {
        #region Cache

        private const int PRIME = 397;

        [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly struct Cache(IntPtr address, int hash) : IEquatable<Cache>
        {
            private readonly IntPtr _address = address;
            private readonly int _hash = hash;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(Cache other)
            {
                return _hash == other._hash
                    && _address == other._address;
            }

            public override bool Equals(object? obj)
            {
                return obj is Cache other && Equals(other);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                return _hash;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator ==(Cache left, Cache right)
            {
                return left.Equals(right);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool operator !=(Cache left, Cache right)
            {
                return !left.Equals(right);
            }
        }

        private static readonly ConcurrentDictionary<Cache, IntPtr> _cache = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CreateHash(int offset, DataType[] argumentTypes, DataType returnType)
        {
            unchecked
            {
                var hash = offset * PRIME;
                hash = (hash * PRIME) ^ (int)returnType;

                for (var i = 0; i < argumentTypes.Length; i++)
                {
                    hash = (hash * PRIME) ^ (int)argumentTypes[i];
                }

                return hash;
            }
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static IntPtr CreateVirtualFunction(
            IntPtr objectPtr,
            int offset,
            IEnumerable<DataType> argumentTypes,
            DataType returnType,
            object[] arguments
        )
        {
            var args = argumentTypes as DataType[] ?? [.. argumentTypes];
            var hash = CreateHash(offset, args, returnType);

            unsafe
            {
                IntPtr** tablePtr = *(IntPtr***)(IntPtr*)objectPtr;
                var cache = new Cache(*tablePtr[offset], hash);

                if (_cache.TryGetValue(cache, out var cachePtr))
                {
                    return cachePtr;
                }

                var funcPtr = NativeAPI.CreateVirtualFunction(objectPtr, offset, args.Length, (int)returnType, arguments);
                _cache[cache] = funcPtr;

                return funcPtr;
            }
        }

        #region Void Actions

        public static Action Create(IntPtr objectPtr, int offset)
        {
            var arguments = Enumerable.Empty<DataType>().ToArray();

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments,
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return () => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { }); };
        }

        public static Action Create(IntPtr objectPtr, int offset, bool bypasshook)
        {
            var arguments = Enumerable.Empty<DataType>().ToArray();

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments,
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return () => { NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { }); };
        }

        public static Action<TArg1> CreateVoid<TArg1>(IntPtr objectPtr, int offset)
        {
            var arguments = new[]
            {
                typeof(TArg1).ToDataType()
            };

            if (arguments.Any(x => x == null))
            {
                throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
            }

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1 });
            };
        }

        public static Action<TArg1> CreateVoid<TArg1>(IntPtr objectPtr, int offset, bool bypasshook)
        {
            var arguments = new[]
            {
                typeof(TArg1).ToDataType()
            };

            if (arguments.Any(x => x == null))
            {
                throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
            }

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1 });
            };
        }

        public static Action<TArg1, TArg2> CreateVoid<TArg1, TArg2>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1, arg2 });
            };
        }

        public static Action<TArg1, TArg2> CreateVoid<TArg1, TArg2>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2 });
            };
        }

        public static Action<TArg1, TArg2, TArg3> CreateVoid<TArg1, TArg2, TArg3>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false, new object[] { arg1, arg2, arg3 });
            };
        }

        public static Action<TArg1, TArg2, TArg3> CreateVoid<TArg1, TArg2, TArg3>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2, arg3 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4> CreateVoid<TArg1, TArg2, TArg3, TArg4>(IntPtr objectPtr,
            int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4> CreateVoid<TArg1, TArg2, TArg3, TArg4>(IntPtr objectPtr,
            int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(
            IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5>(
            IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5,
            TArg6>(
            IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> CreateVoid<TArg1, TArg2, TArg3, TArg4, TArg5,
            TArg6>(
            IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> CreateVoid<TArg1, TArg2, TArg3, TArg4,
            TArg5,
            TArg6, TArg7>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> CreateVoid<TArg1, TArg2, TArg3, TArg4,
            TArg5,
            TArg6, TArg7>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> CreateVoid<TArg1, TArg2, TArg3,
            TArg4,
            TArg5, TArg6, TArg7, TArg8>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> CreateVoid<TArg1, TArg2, TArg3,
            TArg4,
            TArg5, TArg6, TArg7, TArg8>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> CreateVoid<TArg1, TArg2,
            TArg3,
            TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> CreateVoid<TArg1, TArg2,
            TArg3,
            TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> CreateVoid<TArg1,
            TArg2,
            TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
            };
        }

        public static Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> CreateVoid<TArg1,
            TArg2,
            TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                DataType.DATA_TYPE_VOID, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
            {
                NativeAPI.ExecuteVirtualFunction<object>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
            };
        }

        #endregion

        #region Func

        public static Func<TResult> Create<TResult>(IntPtr objectPtr, int offset)
        {
            var arguments = Enumerable.Empty<DataType>().ToArray();

            if (typeof(TResult).ToDataType() == null)
            {
                throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
            }

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments,
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return () => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { });
        }

        public static Func<TResult> Create<TResult>(IntPtr objectPtr, int offset, bool bypasshook)
        {
            var arguments = Enumerable.Empty<DataType>().ToArray();

            if (typeof(TResult).ToDataType() == null)
            {
                throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
            }

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments,
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return () => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { });
        }

        public static Func<TArg1, TResult> Create<TArg1, TResult>(IntPtr objectPtr, int offset)
        {
            var arguments = new[]
            {
                typeof(TArg1).ToDataType()
            };

            if (arguments.Any(x => x == null))
            {
                throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
            }

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1 });
        }

        public static Func<TArg1, TResult> Create<TArg1, TResult>(IntPtr objectPtr, int offset, bool bypasshook)
        {
            var arguments = new[]
            {
                typeof(TArg1).ToDataType()
            };

            if (arguments.Any(x => x == null))
            {
                throw new Exception($"Invalid argument type(s) supplied to Virtual Function");
            }

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1) => NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1 });
        }

        public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, TResult>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1, arg2 });
        }

        public static Func<TArg1, TArg2, TResult> Create<TArg1, TArg2, TResult>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2 });
        }

        public static Func<TArg1, TArg2, TArg3, TResult> Create<TArg1, TArg2, TArg3, TResult>(IntPtr objectPtr,
            int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false, new object[] { arg1, arg2, arg3 });
        }

        public static Func<TArg1, TArg2, TArg3, TResult> Create<TArg1, TArg2, TArg3, TResult>(IntPtr objectPtr,
            int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook, new object[] { arg1, arg2, arg3 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Create<TArg1, TArg2, TArg3, TArg4, TResult>(
            IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Create<TArg1, TArg2, TArg3, TArg4, TResult>(
            IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
            TResult>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
            TResult>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
            TArg6, TResult>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Create<TArg1, TArg2, TArg3, TArg4, TArg5,
            TArg6, TResult>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Create<TArg1, TArg2, TArg3, TArg4,
            TArg5, TArg6, TArg7, TResult>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Create<TArg1, TArg2, TArg3, TArg4,
            TArg5, TArg6, TArg7, TResult>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Create<TArg1, TArg2, TArg3,
            TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Create<TArg1, TArg2, TArg3,
            TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> Create<TArg1, TArg2,
            TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> Create<TArg1, TArg2,
            TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(IntPtr objectPtr, int offset, bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> Create<TArg1,
            TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(IntPtr objectPtr, int offset)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, false,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }

        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> Create<TArg1,
            TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult>(IntPtr objectPtr, int offset,
            bool bypasshook)
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

            var virtualFunctionPointer = CreateVirtualFunction(objectPtr, offset, arguments.Cast<DataType>(),
                (DataType)typeof(TResult).ToDataType()!, arguments.Cast<object>().ToArray());

            return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) =>
                NativeAPI.ExecuteVirtualFunction<TResult>(virtualFunctionPointer, bypasshook,
                    new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }

        #endregion
    }
}

#pragma warning restore CS8601 // Possible null reference assignment.
