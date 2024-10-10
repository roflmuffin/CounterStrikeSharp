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

    private Dictionary<int, bool> ShouldSeeDoors = new Dictionary<int, bool>();

    public override void Load(bool hotReload)
    {
        // This command is related to the following example.
        AddCommand("nodoors", "Toggle door transmit", (player, info) =>
        {
            if (player == null)
                return;

            if (ShouldSeeDoors.ContainsKey(player.Slot))
            {
                ShouldSeeDoors[player.Slot] = !ShouldSeeDoors[player.Slot];
            } else
            {
                ShouldSeeDoors.Add(player.Slot, false);
            }

            info.ReplyToCommand($"You should {(ShouldSeeDoors[player.Slot] ? "see" : "not see")} doors");
        });

        // In this example, we will hide every door for players that have enabled the option with the command 'nodoors'
        RegisterListener<Listeners.CheckTransmit>((CCheckTransmitInfoList infoList) =>
        {
            // Get the list of the currently available doors (prop_door_rotating)
            IEnumerable<CPropDoorRotating> doors = Utilities.FindAllEntitiesByDesignerName<CPropDoorRotating>("prop_door_rotating");

            // Do nothing if there is none.
            if (!doors.Any())
                return;

            // Go through every received info
            foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in infoList)
            {
                // If no player is found, we can continue
                if (player == null)
                    continue;

                // Otherwise, lets do the work:

                // Check if we should clear or not:

                // If we have no data saved for this player, then we should not continue
                if (!ShouldSeeDoors.ContainsKey(player.Slot))
                    continue;

                // If this value is true, then this player should see doors
                if (ShouldSeeDoors[player.Slot])
                    continue;

                // Otherwise, lets remove the door entity indexes from the info list so they won't be transmitted
                foreach (CPropDoorRotating door in doors)
                {
                    info.TransmitEntities.Remove(door);
                }

                // NOTE: this is a barebone example, saving data and doing sanity checks is up to you.
            }
        });

        // In this example, we will hide other players in the same team as the player.
        // NOTE: 'Hiding' players requires extra work to do, killing non-transmitted players results in crash.
        RegisterListener<Listeners.CheckTransmit>((CCheckTransmitInfoList infoList) =>
        {
            // Get the list of the current players, we only work with this value later on
            List<CCSPlayerController> players = Utilities.GetPlayers();

            // Go through every received info
            foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in infoList)
            {
                // If no player is found, we can continue
                if (player == null)
                    continue;

                // Otherwise, lets do the work:

                // as an example, lets hide everyone for this player who is in the same team.
                IEnumerable<CCSPlayerController> targetPlayers = players.Where(p =>
                    // is the player and its pawn valid
                    p.IsValid && p.Pawn.IsValid &&

                    // we shouldn't hide ourselves
                    p.Slot != player.Slot &&

                    // is the player is in the same team
                    p.Team == player.Team &&

                    // is alive
                    p.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE
                );

                foreach (CCSPlayerController targetPlayer in targetPlayers)
                {
                    // Calling 'Remove' will clear the entity index of the target player pawn from the transmission list
                    // so it won't be transmitted for the 'player'.
                    info.TransmitEntities.Remove(targetPlayer.Pawn);
                }
            }
        });
    }
}
