using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace WithCheckTransmit;

[MinimumApiVersion(276)]
public class WithCheckTransmitPlugin : BasePlugin
{
    public override string ModuleName => "Example: With CheckTransmit";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that uses the CheckTransmit listener!";

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.CheckTransmit>((CCheckTransmitInfoList infoList, int infoCount) =>
        {
            // Get the list of the current players, we only work with this value later on
            List<CCSPlayerController> players = Utilities.GetPlayers();

            for (int i = 0; i < infoCount; i++)
            {
                // The 'Get' function returns a tuple with the info for the given index, also the slot the info belongs to.
                // You should never pass an integer outside of the boundaries! (0 <= index < infoCount)
                (CCheckTransmitInfo info, int slot) = infoList.Get(i);

                // We can use the slot to get the player the info belongs to
                CCSPlayerController? infoPlayer = Utilities.GetPlayerFromSlot(slot);

                // If no player is found, we can return
                if (infoPlayer == null)
                {
                    return;
                }

                // Otherwise, lets do the work:

                // as an example, lets hide everyone for this player who is in the same team.
                IEnumerable<CCSPlayerController> targetPlayers = players.Where(p =>
                    // is the player and its pawn valid
                    p.IsValid && p.Pawn.IsValid &&

                    // we shouldn't hide ourselves
                    p.Slot != infoPlayer.Slot &&

                    // is the player is in the same team
                    p.Team == infoPlayer.Team &&

                    // is alive
                    p.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE
                );

                foreach (CCSPlayerController targetPlayer in targetPlayers)
                {
                    // Calling 'Clear' will remove the entity index of the target player pawn from the transmission list
                    // so it won't be transmitted for the 'infoPlayer' player.
                    info.TransmitEntity.Clear(targetPlayer.Pawn.Index);
                }
            }
        });
    }
}
