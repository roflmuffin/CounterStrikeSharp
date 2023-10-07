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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Listeners;

namespace CounterStrikeSharp.API.Core
{
    public class MethodAttribute<T> where T : Attribute
    {
        public MethodAttribute(T attribute, MethodInfo method)
        {
            Attribute = attribute;
            Method = method;
        }

        public T Attribute;
        public MethodInfo Method;
    }

    public static class Helpers
    {
        private static MethodAttribute<T>[] FindMethodAttributes<T>(BasePlugin plugin) where T: Attribute
        {
            return plugin
                .GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(T), false).Length > 0)
                .Select(x => new MethodAttribute<T>(x.GetCustomAttribute<T>(), x))
                .ToArray();   
        }

        private const string dllPath = "counterstrikesharp";

        [SecurityCritical]
        [DllImport(dllPath, EntryPoint = "InvokeNative")]
        public static extern void InvokeNative(IntPtr ptr);

        [UnmanagedCallersOnly]
        // Used by .NET Host in C++ to initiate loading
        public static int LoadAllPlugins()
        {
            try
            {
                var globalContext = new GlobalContext();
                globalContext.LoadAll();
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return 0;
            }
        }

        public delegate void Callback();
    }
}