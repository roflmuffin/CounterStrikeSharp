using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Core
{
    public static class SharedPluginApi
    {
        private static readonly List<ApiFuncRegister> allFuncs = new();
        
        internal static void RegisterFunction(IPlugin plugin, string name, Delegate func)
        {
            var v = new ApiFuncRegister(plugin, name, func);
            allFuncs.Add(v);
        }

        internal static void UnregisterPlugins(IPlugin plugin)
        {
            allFuncs.RemoveAll(x => x.Plugin.Equals(plugin));
        }

        public static object? Call(string moduleName, string funcName, params object?[] values)
        {
            var pluginFound = false;
            var v = allFuncs.LastOrDefault(x =>
            {
                if (!moduleName.Equals(x.Plugin.ModuleName)) { return false; }
                pluginFound = true;
                return funcName.Equals(x.Name);
            });
            if (v == null)
            {
                if (pluginFound)
                {
                    throw new Exception($"cannot find '{funcName}' shared api in '{moduleName}'");
                }
                throw new Exception($"cannot find plugin called '{moduleName}'");
            }
            return v.Function.DynamicInvoke(values);
        }

        private class ApiFuncRegister
        {
            public ApiFuncRegister(IPlugin plugin, string name, Delegate func)
            {
                this.Plugin = plugin;
                this.Name = name;
                this.Function = func;
            }

            public IPlugin Plugin { get; }
            public string Name { get; }
            public Delegate Function { get; }
        }

    }

}
