using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Modules.Utils;

public class ResourceManifest : NativeObject
{
    private Action<nint, string> _addResource;
    public ResourceManifest(nint pointer) : base(pointer)
    {
        _addResource = VirtualFunction.CreateVoid<nint, string>(Handle, GameData.GetOffset("CEntityResourceManifest_AddResource"));
    }

    public void AddResource(string resourcePath) => _addResource(Handle, resourcePath);
}
