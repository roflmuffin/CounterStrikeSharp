/*
 * Copyright (c) 2014 Bas Timmer/NTAuthority et al.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * This file has been modified from its original form for use in this program
 * under GNU Lesser General Public License, version 2.
 */

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using CounterStrikeSharp.API.Modules.Utils;
using FastGenericNew;

namespace CounterStrikeSharp.API.Core
{
    public class NativeException : Exception
    {
        public NativeException(string message) : base(message)
        {
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public unsafe struct fxScriptContext
    {
        public int numArguments;
        public int numResults;
        public int hasError;

        public ulong nativeIdentifier;
        public fixed byte functionData[8 * 32];
        public fixed byte result[8];
    }

    public class ScriptContext
    {
        [ThreadStatic] private static ScriptContext _globalScriptContext;

        public static ScriptContext GlobalScriptContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _globalScriptContext ?? InitGlobalScriptContext(); }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ScriptContext InitGlobalScriptContext()
        {
            _globalScriptContext = new ScriptContext();
            return _globalScriptContext;
        }

        public unsafe ScriptContext()
        {
        }

        public unsafe ScriptContext(fxScriptContext* context)
        {
            m_extContext = *context;
        }

        private readonly ConcurrentQueue<IntPtr> ms_finalizers = new ConcurrentQueue<IntPtr>();

        private readonly object ms_lock = new object();

        internal object Lock => ms_lock;

        internal fxScriptContext m_extContext = new fxScriptContext();

        internal bool isCleanupLocked = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecuritySafeCritical]
        public void Reset()
        {
            InternalReset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityCritical]
        private void InternalReset()
        {
            m_extContext.numArguments = 0;
            m_extContext.numResults = 0;
            m_extContext.hasError = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecuritySafeCritical]
        public void Invoke()
        {
            if (!isCleanupLocked)
            {
                isCleanupLocked = true;
                InvokeNativeInternal();
                GlobalCleanUp();
                isCleanupLocked = false;
                return;
            }

            InvokeNativeInternal();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityCritical]
        private void InvokeNativeInternal()
        {
            unsafe
            {
                fixed (fxScriptContext* cxt = &m_extContext)
                {
                    Helpers.InvokeNative(new IntPtr(cxt));
                }
            }
        }

        public unsafe byte[] GetBytes()
        {
            fixed (fxScriptContext* context = &m_extContext)
            {
                byte[] arr = new byte[8 * 32];
                Marshal.Copy((IntPtr)context->functionData, arr, 0, 8 * 32);

                return arr;
            }
        }

        public unsafe IntPtr GetContextUnderlyingAddress()
        {
            fixed (fxScriptContext* context = &m_extContext)
            {
                return (IntPtr)context;
            }
        }

        [SecuritySafeCritical]
        public void Push(object arg)
        {
            PushInternal(arg);
        }

        [SecuritySafeCritical]
        public unsafe void SetResult(object arg, fxScriptContext* cxt)
        {
            SetResultInternal(cxt, arg);
        }

        [SecurityCritical]
        private unsafe void PushInternal(object arg)
        {
            fixed (fxScriptContext* context = &m_extContext)
            {
                Push(context, arg);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SecurityCritical]
        public unsafe void SetIdentifier(ulong arg)
        {
            fixed (fxScriptContext* context = &m_extContext)
            {
                context->nativeIdentifier = arg;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void CheckErrors()
        {
            if (m_extContext.hasError != 0)
            {
                string error = GetResult<string>();
                Reset();
                throw new NativeException(error);
            }
        }

        [SecurityCritical]
        internal unsafe void Push(fxScriptContext* context, object arg)
        {
            if (arg == null)
            {
                arg = 0;
            }

            if (arg.GetType().IsEnum)
            {
                arg = Convert.ChangeType(arg, arg.GetType().GetEnumUnderlyingType());
            }

            if (arg is string)
            {
                var str = (string)Convert.ChangeType(arg, typeof(string));
                PushString(context, str);

                return;
            }
            else if (arg is InputArgument ia)
            {
                Push(context, ia.Value);

                return;
            }
            else if (arg is IMarshalToNative marshalToNative)
            {
                foreach (var value in marshalToNative.GetNativeObject())
                {
                    Push(context, value);
                }

                return;
            }
            else if (arg is NativeObject nativeObject)
            {
                Push(context, (InputArgument)nativeObject);
                return;
            }
            else if (arg is NativeEntity nativeValue)
            {
                Push(context, (InputArgument)nativeValue);
                return;
            }

            if (Marshal.SizeOf(arg.GetType()) <= 8)
            {
                PushUnsafe(context, arg);
            }

            context->numArguments++;
        }

        [SecurityCritical]
        internal unsafe void SetResultInternal(fxScriptContext* context, object arg)
        {
            if (arg == null)
            {
                arg = 0;
            }

            if (arg.GetType().IsEnum)
            {
                arg = Convert.ChangeType(arg, arg.GetType().GetEnumUnderlyingType());
            }

            if (arg is string)
            {
                var str = (string)Convert.ChangeType(arg, typeof(string));
                SetResultString(context, str);

                return;
            }
            else if (arg is InputArgument ia)
            {
                SetResultInternal(context, ia.Value);

                return;
            }

            if (Marshal.SizeOf(arg.GetType()) <= 8)
            {
                SetResultUnsafe(context, arg);
            }
        }

        [SecurityCritical]
        internal unsafe void PushUnsafe(fxScriptContext* cxt, object arg)
        {
            *(long*)(&cxt->functionData[8 * cxt->numArguments]) = 0;
            Marshal.StructureToPtr(arg, new IntPtr(cxt->functionData + (8 * cxt->numArguments)), true);
        }

        /// <summary>
        /// Pushes a primitive/unmanaged value directly into the context's function
        /// data buffer without boxing or Marshal.StructureToPtr overhead.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushPrimitive<T>(T value) where T : unmanaged
        {
            fixed (fxScriptContext* cxt = &m_extContext)
            {
                *(long*)(&cxt->functionData[8 * cxt->numArguments]) = 0;
                *(T*)(&cxt->functionData[8 * cxt->numArguments]) = value;
                cxt->numArguments++;
            }
        }

        [SecurityCritical]
        internal unsafe void SetResultUnsafe(fxScriptContext* cxt, object arg)
        {
            *(long*)(&cxt->result[0]) = 0;
            Marshal.StructureToPtr(arg, new IntPtr(cxt->result), true);
        }

        [SecurityCritical]
        internal unsafe void PushString(string str)
        {
            fixed (fxScriptContext* cxt = &m_extContext)
            {
                PushString(cxt, str);
            }
        }

        [SecurityCritical]
        internal unsafe void PushString(fxScriptContext* cxt, string str)
        {
            if (str == null)
            {
                *(IntPtr*)(&cxt->functionData[8 * cxt->numArguments]) = IntPtr.Zero;
                cxt->numArguments++;
                return;
            }

            int maxBytes = Encoding.UTF8.GetMaxByteCount(str.Length);
            var ptr = Marshal.AllocHGlobal(maxBytes + 1);

            var dest = new Span<byte>((void*)ptr, maxBytes + 1);
            int written = Encoding.UTF8.GetBytes(str, dest);
            dest[written] = 0;

            ms_finalizers.Enqueue(ptr);

            *(IntPtr*)(&cxt->functionData[8 * cxt->numArguments]) = ptr;
            cxt->numArguments++;
        }

        [SecurityCritical]
        internal unsafe void SetResultString(fxScriptContext* cxt, string str)
        {
            if (str == null)
            {
                *(IntPtr*)(&cxt->result[0]) = IntPtr.Zero;
                return;
            }

            int maxBytes = Encoding.UTF8.GetMaxByteCount(str.Length);
            var ptr = Marshal.AllocHGlobal(maxBytes + 1);

            var dest = new Span<byte>((void*)ptr, maxBytes + 1);
            int written = Encoding.UTF8.GetBytes(str, dest);
            dest[written] = 0;

            ms_finalizers.Enqueue(ptr);
            *(IntPtr*)(&cxt->result[8]) = ptr;
        }

        [SecuritySafeCritical]
        public T GetArgument<T>(int index)
        {
            return (T)GetArgument(typeof(T), index);
        }

        [SecuritySafeCritical]
        public object GetArgument(Type type, int index)
        {
            return GetArgumentHelper(type, index);
        }

        [SecurityCritical]
        internal unsafe object GetArgument(fxScriptContext* cxt, Type type, int index)
        {
            return GetArgumentHelper(cxt, type, index);
        }

        [SecurityCritical]
        private unsafe object GetArgumentHelper(Type type, int index)
        {
            fixed (fxScriptContext* cxt = &m_extContext)
            {
                return GetArgumentHelper(cxt, type, index);
            }
        }

        [SecurityCritical]
        private unsafe object GetArgumentHelper(fxScriptContext* context, Type type, int index)
        {
            return GetResult(type, &context->functionData[index * 8]);
        }

        [SecuritySafeCritical]
        public T GetResult<T>()
        {
            return (T)GetResult(typeof(T));
        }

        [SecuritySafeCritical]
        public object GetResult(Type type)
        {
            return GetResultHelper(type);
        }

        [SecurityCritical]
        internal unsafe object GetResult(fxScriptContext* cxt, Type type)
        {
            return GetResultHelper(cxt, type);
        }

        [SecurityCritical]
        private unsafe object GetResultHelper(Type type)
        {
            fixed (fxScriptContext* cxt = &m_extContext)
            {
                return GetResultHelper(cxt, type);
            }
        }

        [SecurityCritical]
        private unsafe object GetResultHelper(fxScriptContext* context, Type type)
        {
            return GetResult(type, &context->result[0]);
        }

        [SecurityCritical]
        internal unsafe object GetResult(Type type, byte* ptr)
        {
            if (type == typeof(string))
            {
                var nativeUtf8 = *(byte**)ptr;

                if (nativeUtf8 == null)
                {
                    return null;
                }

                return Marshal.PtrToStringUTF8((IntPtr)nativeUtf8);
            }

            if (typeof(NativeObject).IsAssignableFrom(type))
            {
                var pointer = (IntPtr)GetResult(typeof(IntPtr), ptr);
                return Activator.CreateInstance(type, pointer);
            }

            if (type == typeof(Color))
            {
                var pointer = (IntPtr)GetResult(typeof(IntPtr), ptr);
                return Marshaling.ColorMarshaler.NativeToManaged(pointer);
            }

            // this one only works if the 'Raw'/uint is passed
            // maybe do this with a marshaler?!
            if (type == typeof(CEntityHandle))
            {
                return new CEntityHandle((uint)GetResult(typeof(uint), ptr));
            }

            if (type == typeof(object))
            {
                return null;
            }

            if (type.IsEnum)
            {
                return Enum.ToObject(type, GetResult(type.GetEnumUnderlyingType(), ptr));
            }

            if (Marshal.SizeOf(type) <= 8)
            {
                return GetResultInternal(type, ptr);
            }

            return null;
        }

        [SecurityCritical]
        private unsafe object GetResultInternal(Type type, byte* ptr)
        {
            var obj = Marshal.PtrToStructure(new IntPtr(ptr), type);
            return obj;
        }

        [SecurityCritical]
        internal unsafe string GetResultString()
        {
            fixed (fxScriptContext* cxt = &m_extContext)
            {
                var nativeUtf8 = *(byte**)(&cxt->result[0]);

                if (nativeUtf8 == null)
                {
                    return null;
                }

                return Marshal.PtrToStringUTF8((IntPtr)nativeUtf8);
            }
        }

        /// <summary>
        /// Reads a primitive/unmanaged result directly from the context's result
        /// buffer without Marshal.PtrToStructure or boxing overhead.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T GetResultPrimitive<T>() where T : unmanaged
        {
            fixed (fxScriptContext* cxt = &m_extContext)
            {
                return *(T*)(&cxt->result[0]);
            }
        }

        /// <summary>
        /// Reads a pointer result from the context and creates a NativeObject-derived
        /// instance using FastGenericNew. Avoids Activator.CreateInstance overhead.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T GetResultNativeObject<T>()
        {
            fixed (fxScriptContext* cxt = &m_extContext)
            {
                var pointer = *(IntPtr*)(&cxt->result[0]);
                return FastNew.CreateInstance<T, IntPtr>(pointer);
            }
        }


        [SecurityCritical]
        internal unsafe string ErrorHandler(byte* error)
        {
            if (error != null)
            {
                var errorStart = error;
                int length = 0;

                for (var p = errorStart; *p != 0; p++)
                {
                    length++;
                }

                return Encoding.UTF8.GetString(errorStart, length);
            }

            return "Native invocation failed.";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void GlobalCleanUp()
        {
            if (!ms_finalizers.IsEmpty)
            {
                GlobalCleanUpSlow();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GlobalCleanUpSlow()
        {
            lock (ms_lock)
            {
                while (ms_finalizers.TryDequeue(out var ptr))
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        public override string ToString()
        {
            return $"ScriptContext{{numArgs={m_extContext.numArguments}}}";
        }
    }
}
