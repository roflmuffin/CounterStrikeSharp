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
        RegisterListener<Listeners.CheckTransmit>((CCheckTransmitInfoList infoList, int infoCount) =>
        {
            // Get the list of the currently available doors (prop_door_rotating)
            IEnumerable<CPropDoorRotating> doors = Utilities.FindAllEntitiesByDesignerName<CPropDoorRotating>("prop_door_rotating");

            // Do nothing if there is none.
            if (!doors.Any())
                return;

            // Go through every received info
            for (int i = 0; i < infoCount; i++)
            {
                // The 'Get' function returns a tuple with the info for the given index, also the slot the info belongs to.
                // You should never pass an integer outside of the boundaries! (0 <= index < infoCount)
                (CCheckTransmitInfo info, int slot) = infoList.Get(i);

                // We can use the slot to get the player the info belongs to
                CCSPlayerController? infoPlayer = Utilities.GetPlayerFromSlot(slot);

                // If no player is found, we can continue
                if (infoPlayer == null)
                    continue;

                // Otherwise, lets do the work:

                // Check if we should clear or not:

                // If we have no data saved for this player, then we should not continue
                if (!ShouldSeeDoors.ContainsKey(infoPlayer.Slot))
                    continue;

                // If this value is true, then this player should see doors
                if (ShouldSeeDoors[infoPlayer.Slot])
                    continue;

                // Otherwise, lets clear the door entity indexes from the info list so they won't be transmitted
                foreach (CPropDoorRotating door in doors)
                {
                    info.TransmitEntities.Remove(door);
                }

                // NOTE: this is a barebone example, saving data and doing sanity checks is up to you.
            }
        });

        // In this example, we will hide other players in the same team as the player.
        // NOTE: 'Hiding' players requires extra work to do, killing non-transmitted players results in crash.
        RegisterListener<Listeners.CheckTransmit>((CCheckTransmitInfoList infoList, int infoCount) =>
        {
            // Get the list of the current players, we only work with this value later on
            List<CCSPlayerController> players = Utilities.GetPlayers();

            // Go through every received info
            for (int i = 0; i < infoCount; i++)
            {
                // The 'Get' function returns a tuple with the info for the given index, also the slot the info belongs to.
                // You should never pass an integer outside of the boundaries! (0 <= index < infoCount)
                (CCheckTransmitInfo info, int slot) = infoList.Get(i);

                // We can use the slot to get the player the info belongs to
                CCSPlayerController? infoPlayer = Utilities.GetPlayerFromSlot(slot);

                // If no player is found, we can continue
                if (infoPlayer == null)
                    continue;

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
                    info.TransmitEntities.Remove(targetPlayer.Pawn);
                }
            }
        });
    }
}
