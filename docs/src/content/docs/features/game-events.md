---
title: Game Events
description: How to listen to Source 1 style game events.
---

## Adding an Event Listener

### Automatic registration

CounterStrikeSharp will automatically register event listeners marked with a `GameEventHandler` attribute on the `BasePlugin` class. These listeners are automatically registered/deregistered for you on hot reload.

:::note
The first parameter type must be a subclass of the `GameEvent` class. The names are automatically generated from the [game event list](https://cs2.poggu.me/dumped-data/game-events).
:::

```csharp
[GameEventHandler]
public void OnPlayerConnect(EventPlayerConnect @event)
{
    // Userid will give you a reference to a CCSPlayerController class
    Log($"Player {@event.Userid.PlayerName} has connected!");
}
```

### On Load

It is also possible to bind event listeners in the `OnLoad` (or anywhere you have access to the plugin instance).

```csharp
public override void Load(bool hotReload)
{
    RegisterEventHandler<EventRoundStart>(@event =>
    {
        Console.WriteLine($"Round has started with time limit of {@event.Timelimit}");
    });
}
```

## Accessing Event Parameters

The specific subclass of `GameEvent` will provide strongly typed parameters from the event definition. e.g. `event.Timelimit` will be a `long` value, `event.UserId` will be a `CCSPlayerController` and so-on.

These event properties are mutable so you can update them as normal and they will update in the event instance.
