# Game Events

How to listen to Source 1 style game events.

## Adding an Event Listener

### Automatic registration

CounterStrikeSharp will automatically register event listeners marked with a `GameEventHandler` attribute on the `BasePlugin` class. These listeners are automatically registered/deregistered for you on hot reload.

> [!NOTE]
> The first parameter type must be a subclass of the `GameEvent` class. The names are automatically generated from the [game event list](https://cs2.poggu.me/dumped-data/game-events).

```csharp
[GameEventHandler]
public HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
{
    // Userid will give you a reference to a CCSPlayerController class.
    // Before accessing any of its fields, you must first check if the Userid
    // handle is actually valid, otherwise you may run into runtime exceptions.
    // See the documentation section on Referencing Players for details.
    if (@event.Userid.IsValid) {
        Logger.LogInformation("Player {Name} has connected!", @event.Userid.PlayerName);
    }

    return HookResult.Continue;
}
```

### On Load

It is also possible to bind event listeners in the `OnLoad` (or anywhere you have access to the plugin instance).

```csharp
public override void Load(bool hotReload)
{
    RegisterEventHandler<EventRoundStart>((@event, info) =>
    {
        Logger.LogInformation("Round has started with time limit of {Timelimit}", @event.Timelimit);

        return HookResult.Continue;
    });
}
```

## Accessing Event Parameters

The specific subclass of `GameEvent` will provide strongly typed parameters from the event definition. e.g. `event.Timelimit` will be a `long` value, `event.UserId` will be a `CCSPlayerController` and so-on.

These event properties are mutable so you can update them as normal and they will update in the event instance.

## Preventing Broadcast

You can modify a game event so that it does not get broadcast to clients by modifying the `bool info.DontBroadcast` property. e.g.

## Cancelling an Event

In a pre-event hook, you can prevent the event from continuing to other plugins by returning `HookResult.Handled` or `HookResult.Stop`.
