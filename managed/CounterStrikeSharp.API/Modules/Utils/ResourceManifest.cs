using CounterStrikeSharp.API.Modules.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public class ResourceManifest : NativeObject
    {
        private Action<nint, string> AddResourceInternal;
        public ResourceManifest(nint pointer) : base(pointer)
        {
            AddResourceInternal = VirtualFunction.CreateVoid<nint, string>(Handle, GameData.GetOffset("CEntityResourceManifest_AddResourceInternal"));
        }

        public void PrecacheResource(string resourcePath) => AddResourceInternal(Handle, resourcePath);
    }
}
