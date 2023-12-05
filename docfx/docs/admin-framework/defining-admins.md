---
title: Defining Admins
description: A guide on how to define admins for CounterStrikeSharp.
---

# Defining Admins

A guide on how to define admins for CounterStrikeSharp.

## Admin Framework

CounterStrikeSharp has a basic framework which allows plugin developers to assign permissions to commands. When CSS is initialized, a list of admins are loaded from `configs/admins.json`.

## Adding Admins

Adding an Admin is as simple as creating a new entry in the `configs/admins.json` file. The important things you need to declare are the SteamID identifier and the permissions they have. CounterStrikeSharp will do all the heavy-lifting to decipher your SteamID. If you're familiar with SourceMod, permission definitions are slightly different as they're defined by an array of strings instead of a string of characters.

```json
{
  "ZoNiCaL": {
    "identity": "76561198808392634",
    "flags": ["@css/changemap", "@css/generic"]
  },
  "another ZoNiCaL": {
    "identity": "STEAM_0:1:1",
    "flags": ["@css/generic"]
  }
}
```

You can also manually assign permissions to players in code with `AdminManager.AddPlayerPermissions` and `AdminManager.RemovePlayerPermissions`. These changes are not saved to `configs/admins.json`.

> [!NOTE]
> All user permissions MUST start with an at-symbol @ character, otherwise CounterStrikeSharp will not recognize the permission.

### Standard Permissions

Because the flag system is just a list of strings associated with a user, there is no real list of letter based flags like there was previously in something like SourceMod. This means as a plugin author you can declare your own flags, scoped with an `@` symbol, like `@roflmuffin/guns`, which might be the permission to allow spawning of guns in a given command.

However there is a somewhat standardized list of flags that it is advised you use if you are adding functionality that aligns with their purpose, and these are based on the original SourceMod flags:

```shell
@css/reservation # Reserved slot access.
@css/generic # Generic admin.
@css/kick # Kick other players.
@css/ban # Ban other players.
@css/unban # Remove bans.
@css/vip # General vip status.
@css/slay # Slay/harm other players.
@css/changemap # Change the map or major gameplay features.
@css/cvar # Change most cvars.
@css/config # Execute config files.
@css/chat # Special chat privileges.
@css/vote # Start or create votes.
@css/password # Set a password on the server.
@css/rcon # Use RCON commands.
@css/cheats # Change sv_cheats or use cheating commands.
@css/root # Magically enables all flags and ignores immunity values.
```

> [!NOTE]
> CounterStrikeSharp does not implement traditional admin command such as `!slay`, `!kick`, and `!ban`. It is up to individual plugins to implement these commands.
