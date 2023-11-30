using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.Logging;

namespace WithGameEventHandlers;

[MinimumApiVersion(80)]
public class WithGameEventHandlersPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Game Event Handlers";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that subscribes to game events";

    public override void Load(bool hotReload)
    {
        // Subscriptions can be added via the instance method
        RegisterEventHandler<EventPlayerDeath>((@event, info) =>
        {
            // You can use `info.DontBroadcast` to set the dont broadcast flag on the event (in pre handlers)
            // This will prevent the event from being broadcast to other clients.
            // In this example we prevent kill-feed messages from being broadcast if it was not a headshot.
            if (!@event.Headshot)
            {
                @event.Attacker.PrintToChat($"Skipping player_death broadcast");
                info.DontBroadcast = true;
            }

            return HookResult.Continue;
        }, HookMode.Pre);
    }
    
    // Subscriptions can be added via an attribute
    [GameEventHandler]
    public HookResult OnPlayerBlind(EventPlayerBlind @event, GameEventInfo info)
    {
        Logger.LogInformation("Player was just blinded for {Duration}", @event.BlindDuration);

        return HookResult.Continue;
    }

    // The event name is inferred from the event type you pass to the first argument.
    // e.g. EventRoundStart becomes "round_start"
    // Note: You can use the `HookMode` enum to specify the hook mode
    // If you do not specify a hook mode, it will default to `HookMode.Post`
    [GameEventHandler(HookMode.Pre)]
    public HookResult OnEventRoundStartPre(EventRoundStart @event, GameEventInfo info)
    {
        Logger.LogInformation("Round has started with Timelimit: {Timelimit}", @event.Timelimit);

        return HookResult.Continue;
    }
}