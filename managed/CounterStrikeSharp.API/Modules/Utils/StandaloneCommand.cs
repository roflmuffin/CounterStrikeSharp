using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Utils;

public class Commands
{
    public static void AddStandaloneCommand(string name, string description, CommandInfo.CommandCallback handler, bool serverOnly)
    {
        var wrappedHandler = new Action<int, IntPtr>((i, ptr) =>
        {
            if (i == -1)
            {
                handler?.Invoke(null, new CommandInfo(ptr, null));
                return;
            }

            if (serverOnly) return;

            var entity = new CCSPlayerController(NativeAPI.GetEntityFromIndex(i + 1));
            var command = new CommandInfo(ptr, entity);
            handler?.Invoke(entity.IsValid ? entity : null, command);
        });

        var subscriber = new BasePlugin.CallbackSubscriber(handler, wrappedHandler, () => { });
        NativeAPI.AddCommand(name, description, serverOnly, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND,
            subscriber.GetInputArgument());
    }
}
