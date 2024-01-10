using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Core.Plugin;
using MySharedTypes.Contracts;

namespace WithSharedTypesConsumer;

[MinimumApiVersion(142)]
public class WithSharedTypesPlugin : BasePlugin
{
    public override string ModuleName => "Example: Shared Types";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that shares types between multiple plugins";

    public static PlayerCapability<IBalanceHandler> BalanceCapability { get; } = new("mymod:balance");

    public override void Load(bool hotReload)
    {
        AddCommand("css_subtract", "Subtracts 50 from your balance", (player, info) =>
        {
            if (player == null) return;
            var balance = BalanceCapability.Get(player);
            if (balance == null) return;
            balance.Balance -= 50;
            player.PrintToChat($"Your balance is now {balance.Balance}");
        });
    }

    public override void Unload(bool hotReload)
    {
    }
}