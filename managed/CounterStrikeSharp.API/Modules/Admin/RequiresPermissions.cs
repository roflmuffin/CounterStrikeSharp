using System;

namespace CounterStrikeSharp.API.Modules.Admin
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresPermissions : Attribute
    {
        public string[] RequiredPermissions { get; }

        public RequiresPermissions(params string[] permissions)
        {
            RequiredPermissions = permissions;
        }
    }
}
