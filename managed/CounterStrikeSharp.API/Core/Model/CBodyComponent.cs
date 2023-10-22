using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CBodyComponent
{
    public CGameSceneNode? SceneNode => Schema.GetPointer<CGameSceneNode>(Handle + 0x8);
}