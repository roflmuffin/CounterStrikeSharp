using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Commands
{
    public enum CommandUsage
    {
        CLIENT_AND_SERVER = 0,
        CLIENT_ONLY,
        SERVER_ONLY
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandHelperAttribute : Attribute
    {
        public int MinArgs { get; }
        public string Usage { get; }
        public CommandUsage WhoCanExcecute { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minArgs">The minimum amount of arguments required to execute this command.</param>
        /// <param name="usage">If the command fails, this string is printed to the caller to show the CommandUtils intended usage.</param>
        /// <param name="whoCanExecute">Restricts the command so it can only be executed by players, the server console, or both (see CommandUsage).</param>
        public CommandHelperAttribute(int minArgs = 0, string usage = "", CommandUsage whoCanExecute = CommandUsage.CLIENT_AND_SERVER)
        {
            MinArgs = minArgs;
            Usage = usage;
            WhoCanExcecute = whoCanExecute;
        }
    }
}
