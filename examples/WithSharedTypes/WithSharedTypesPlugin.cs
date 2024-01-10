using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using MySharedTypes.Contracts;

namespace WithSharedTypes;

[MinimumApiVersion(143)]
public class WithSharedTypesPlugin : BasePlugin
{
    public override string ModuleName => "Example: Shared Types";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that shares types between multiple plugins";

    // Declares a player capability, that stores some sort of functionality for a player.
    // In this case, it's a balance handler, which is used to store a player's balance.
    // Note that we use the same name for the capability as the one in the other plugin.
    // IBalanceHandler is defined in MySharedTypes.Contracts, which is a shared library and placed in the `shared/` subfolder.
    public static PlayerCapability<IBalanceHandler> BalanceCapability { get; } = new("mymod:balance");
    
    // Plugin capabilities are similar to player capabilities, but they are not tied to a player, and are just generic APIs
    // that are exposed by a plugin. In this case, we expose a balance service, which is used to clear all balances.
    public static PluginCapability<IBalanceService> BalanceServiceCapability { get; } = new("mymod:balance_service");

    public override void Load(bool hotReload)
    {
        // Register the capability implementations here. Note that plugins don't need to register an implementation if it is already implemented in another plugin.
        Capabilities.RegisterPlayerCapability(BalanceCapability, player => new BalanceHandler(player));
        Capabilities.RegisterPluginCapability(BalanceServiceCapability, () => new BalanceService());

        AddCommand("css_balance", "Gets your current balance", (player, info) =>
        {
            if (player == null) return;
            player.PrintToChat($"Your balance is {BalanceCapability.Get(player)?.Balance}");
        });

        AddCommand("css_give", "Gives you money", (player, info) =>
        {
            if (player == null) return;

            var balance = BalanceCapability.Get(player);
            if (balance == null) return;

            balance.Add(100);
            player.PrintToChat($"Your balance is now {balance.Balance}");
        });
    }

    public override void Unload(bool hotReload)
    {
    }
}