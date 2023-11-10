using System;

namespace CounterStrikeSharp.API.Modules.Admin
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionHelperAttribute : Attribute
    {
        public string[] RequiredPermissions { get; }

        public PermissionHelperAttribute(params string[] permissions)
        {
            RequiredPermissions = permissions;
        }
    }
}
