using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Core.Plugin;
using MySharedTypes.Contracts;

namespace WithSharedTypesConsumer;

[MinimumApiVersion(142)]
public class WithSharedTypesConsumerPlugin : BasePlugin
{
    public override string ModuleName => "Example: Shared Types (Consumer)";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that utilises the balance api from another plugin";

    public static PlayerCapability<IBalanceHandler> BalanceCapability { get; } = new("myplugin:balance");
    public static PlayerCapability<Decimal> BalanceCapabilityDecimal { get; } = new("myplugin:balance_decimal");

    public static PluginCapability<IBalanceService> BalanceServiceCapability { get; } = new("myplugin:balance_service");
    
    public override void Load(bool hotReload)
    {
        AddCommand("css_subtract", "Subtracts 50 from your balance", (player, info) =>
        {
            if (player == null) return;
            var balance = BalanceCapability.Get(player);
            if (balance == null) return;
            balance.Subtract(50);
            player.PrintToChat($"Your balance is now {balance.Balance}");
        });
        
        AddCommand("css_clearbalances", "Clears all balances", (player, info) =>
        {
            if (player == null) return;
            var service = BalanceServiceCapability.Get();
            if (service == null) return;

            service.ClearAllBalances();

            var balance = BalanceCapability.Get(player);
            if (balance == null) return;
            player.PrintToChat($"Your balance is now {balance.Balance}");
        });
        
        AddCommand("css_decimalbalance", "Gets your current balance", (player, info) =>
        {
            if (player == null) return;
            player.PrintToChat($"Your balance is {BalanceCapabilityDecimal.Get(player)}");
        });
    }

    public override void Unload(bool hotReload)
    {
    }
}