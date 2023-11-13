using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Admin
{
    public class BaseRequiresPermissions : Attribute
    {
        public string[] Permissions { get; }

        public BaseRequiresPermissions(params string[] permissions)
        {
            Permissions = permissions;
        }

        public virtual bool CanExecuteCommand(CCSPlayerController? caller)
        {
            return false;
        }
    }
}
