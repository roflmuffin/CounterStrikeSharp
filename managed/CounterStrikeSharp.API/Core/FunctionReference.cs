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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core
{
    /// <summary>
    /// Describes the lifetime of a function reference.
    /// </summary>
    public enum FunctionLifetime
    {
        /// <summary>Delegate will be removed after the first invocation.</summary> 
        SingleUse,
        /// <summary>Delegate will remain in memory for the lifetime of the application.</summary>
        Permanent
    }
    
    public class FunctionReference
    {
        private readonly Delegate m_method;
        
        public FunctionLifetime Lifetime { get; set; } = FunctionLifetime.Permanent;

        public unsafe delegate void CallbackDelegate(fxScriptContext* context);
        private CallbackDelegate s_callback;

        private FunctionReference(Delegate method)
        {
            m_method = method;

            unsafe
            {
                var dg = new CallbackDelegate((fxScriptContext* context) =>
                {
                    try
                    {
                        var scriptContext = new ScriptContext(context);

                        if (method.Method.GetParameters().FirstOrDefault()?.ParameterType == typeof(ScriptContext))
                        {
                            var returnO = m_method.DynamicInvoke(scriptContext);
                            if (returnO != null)
                            {
                                scriptContext.SetResult(returnO, context);
                            }

                            return;
                        }

                        var paramsList = method.Method.GetParameters().Select((x, i) =>
                        {
                            var param = method.Method.GetParameters()[i];
                            object obj = null;
                            if (typeof(NativeObject).IsAssignableFrom(param.ParameterType))
                            {
                                obj = Activator.CreateInstance(param.ParameterType,
                                    new[] { scriptContext.GetArgument(typeof(IntPtr), i) });
                            }
                            else
                            {
                                obj = scriptContext.GetArgument(param.ParameterType, i);
                            }

                            return obj;
                        }).ToArray();

                        var returnObj = m_method.DynamicInvoke(paramsList);

                        if (returnObj != null)
                        {
                            scriptContext.SetResult(returnObj, context);
                        }
                    }
                    catch (Exception e)
                    {
                        Application.Instance.Logger.LogError(e, "Error invoking callback");
                    }
                    finally
                    {
                        if (Lifetime == FunctionLifetime.SingleUse)
                        {
                            Remove(Identifier);
                            if (references.ContainsKey(m_method))
                                references.Remove(m_method, out _);
                        }
                    }
                });
                s_callback = dg;
            }

        }

        public int Identifier { get; private set; }

        public static FunctionReference Create(Delegate method)
        {
            if (references.TryGetValue(method, out var existingReference))
            {
                return existingReference;
            }

            var reference = new FunctionReference(method);
            var referenceId = Register(reference);

            reference.Identifier = referenceId;
            
            return reference;
        }

        private static ConcurrentDictionary<int, FunctionReference> ms_references = new ConcurrentDictionary<int, FunctionReference>();
        private static int ms_referenceId;

        private static ConcurrentDictionary<Delegate, FunctionReference> references =
            new ConcurrentDictionary<Delegate, FunctionReference>();

        private static int Register(FunctionReference reference)
        {
            var thisRefId = ms_referenceId;
            ms_references[thisRefId] = reference;
            references[reference.m_method] = reference;

            unchecked { ms_referenceId++; }

            return thisRefId;
        }

        public static FunctionReference Get(int reference)
        {
            if (ms_references.ContainsKey(reference))
            {
                return ms_references[reference];
            }

            return null;
        }
        
        public IntPtr GetFunctionPointer()
        {
            IntPtr cb = Marshal.GetFunctionPointerForDelegate(s_callback);
            return cb;
        }

        public static void Remove(int reference)
        {
            if (ms_references.TryGetValue(reference, out var funcRef))
            {
                ms_references.Remove(reference, out _);

                Application.Instance.Logger.LogDebug("Removing function/callback reference: {Reference}", reference);
            }
        }
    }
}