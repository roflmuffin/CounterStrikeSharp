---
title: Admin Framework
description: A guide on using the Admin Framework in plugins.
---

## Admin Framework

CounterStrikeSharp has a basic framework which allows plugin developers to assign permissions to commands. When CSS is initialized, a list of admins are loaded from `configs/admins.json`.

## Adding Admins

Adding an Admin is as simple as creating a new entry in the `configs/admins.json` file. The important things you need to declare are the SteamID identifier and the permissions they have. CounterStrikeSharp will do all the heavy-lifting to decipher your SteamID. If you're familar with SourceMod, permission definitions are slightly different as they're defined by an array of strings instead of a string of characters.

```json
{
  "ZoNiCaL": {
    "identity": "76561198808392634",
    "flags": ["can_manipulate_players", "admin_messages"]
  }
}
```

You can also manually assign permissions to players in code with `AddPlayerPermissions` and `RemovePlayerPermissions`. These changes are not saved to `configs/admins.json`.

## Assigning permissions to a Command

Assigning permissions to a Command is as easy as tagging the Command method (function callback) with a `PermissionHelper` attribute.

```csharp
[PermissionHelper("can_execute_test_command", "other_permission")]
public void OnMyCommand(CCSPlayerController? caller, CommandInfo info)
{
    ...
}
```

CounterStrikeSharp handles all of the permission checks behind the scenes for you.
