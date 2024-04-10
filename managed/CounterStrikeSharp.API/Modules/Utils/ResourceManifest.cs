using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Modules.Utils;

public class ResourceManifest : NativeObject
{
    private Action<nint, string> _AddResource;
    public ResourceManifest(nint pointer) : base(pointer)
    {
        _AddResource = VirtualFunction.CreateVoid<nint, string>(Handle, GameData.GetOffset("CEntityResourceManifest_AddResource"));
    }

    public void AddResource(string resourcePath) => _AddResource(Handle, resourcePath);
}
