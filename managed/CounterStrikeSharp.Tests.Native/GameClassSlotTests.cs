using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using Xunit;

namespace NativeTestsPlugin;

// Game-class virtuals called through Gamedata offsets are Slot Bindings, verified (or healed) at boot. Plugins reading
// them by key get the index the Vtable Check found, not the Gamedata one; a failed one throws instead of a wrong index.
// Passes whatever E2E does to their Gamedata; logs each index so E2E can check a Heal reached plugins.
public class GameClassSlotTests
{
    private static readonly string[] Keys =
    [
        "ISource2GameEntities::CheckTransmit",
        "CCSPlayerController_ChangeTeam",
        "CCSPlayerController_Respawn",
        "CCSPlayer_ItemServices_GiveNamedItem",
        "CCSPlayer_ItemServices_DropActivePlayerWeapon",
        "CCSPlayer_ItemServices_RemoveWeapons",
        "CGameSceneNode_GetSkeletonInstance",
        "CCSGameRules_FindPickerEntity",
        "CBaseEntity_IsPlayerPawn",
        "CBaseEntity_Teleport",
        "CBasePlayerPawn_CommitSuicide",
    ];

    [Fact]
    public void GameClassVirtuals_ReadByKey_FollowTheVtableCheck()
    {
        foreach (var key in Keys)
        {
            var binding = EngineFeatures.Current.Report.Find(key);
            Assert.NotNull(binding);
            Assert.Equal(BindingKind.Slot, binding.Kind);

            if (!binding.Resolved)
            {
                // Failed Vtable Check: InvalidOperationException; key or OS entry missing: GetOffset's usual exceptions.
                var exception = Assert.ThrowsAny<Exception>(() => GameData.GetOffset(key));
                Console.WriteLine($"[GameClassSlotTests] {key} read by key: {exception.Message}");
                continue;
            }

            Assert.Equal(binding.Offset, GameData.GetOffset(key));
            var how = binding.Healed ? $"healed from {binding.Slot!.StatedIndex}" : binding.Unverified ? "unverified" : "verified";
            Console.WriteLine($"[GameClassSlotTests] {key} read by key: {binding.Offset} ({how})");
        }
    }
}
