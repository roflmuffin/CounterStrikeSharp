---
title: Referencing Players
description: Difference between player slots, indexes, userids, controllers & pawns.
---

# Referencing Players

Difference between player slots, indexes, userids, controllers & pawns.

## Controllers & Pawns

All players in CS2 are split between a player controller & a player pawn. The player controller represents the player on the server, and the player pawn represents the players physical character in the game world. This means to edit a players health for example, you would need to edit their `PlayerPawn`'s health; but to check for a player's SteamID, you would check the `PlayerController`.

Every player controller has access to a `PlayerPawn` property which is a `CHandle` to a players pawn. Likewise, a reverse lookup is possible as each `PlayerPawn` has a `Controller` property which provides access to a pawns controller.

```csharp
CCSPlayerController player = ...;
CCSPlayerPawn playerPawn = player.PlayerPawn.Value; // as `PlayerPawn` is a `CHandle`, to fetch its underlying value we must get the `.Value` property
CCSPlayerController samePlayer = playerPawn.Controller.Value; // same as above.
```

## Identifying Players

Players can be identified in a number of ways, by __slot__, by __index__, by __userid__ and also by __pointer__. Player slot represents a "slot" in the server, and is basically an entity index minus one. So a player controller with an index of `10` is equivalent to player slot 9. Game events with `userid` as a field expose the pointer to the player controller directly from the engine, e.g.

```csharp
RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
{
    CCSPlayerController player = @event.Userid;
}
```

### CPlayerSlot/Slot
Represents a "slot" in the server, and is basically an entity index minus one. So a player controller with an index of `10` is equivalent to player slot 9.

### User IDs
Userids are similar to a slot, and they are what show in the console when you type the `status` command. A Userid can be converted to a slot (and then ultimately an index by adding +1) by doing a bitshift `userid & 0xFF`.

### Entity Index
All entity instances have an entity index (similar to CSGO), which means both the player controller and the player pawn both have different indexes. The Player Controller has a reserved entity index (because of the slot system 0-MAXPLAYERS(64)), but a player pawn does not, so it is common to retrieve a player pawn with an index in the hundreds.

### Entity Pointers & Handles
All "entity objects" you interact with in CounterStrikeSharp are actually wrappers around a __pointer__ on the server, which can be accessed by retrieving the `.Handle` property. Which means to go from a CPlayerSlot, UserID or Index value, you must use the matching utility method to fetch the entity pointer. There are three utility methods to get an instance of a player using these identifiers:

```csharp
var player = Utilities.GetPlayerFromUserid(userid);
var player = Utilities.GetPlayerFromIndex(index);
var player = Utilities.GetPlayerFromSlot(slot);
```

> [!NOTE]
> Wherever possible, you should check the validity of any handle you are accessing before assuming it is safe to use. 
> ```csharp
> RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
> {
>     if (!@event.Userid.IsValid) return 0; // Checks that the PlayerController is valid
>     if (!@event.Userid.PlayerPawn.IsValid) return 0; // Checks that the value of the CHandle is pointing to a valid PlayerPawn.
> }
> ```
