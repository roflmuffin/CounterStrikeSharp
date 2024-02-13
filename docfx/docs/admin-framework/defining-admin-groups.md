---
title: Defining Admin Groups
description: A guide on how to define admin groups for CounterStrikeSharp.
---

# Defining Admin Groups

A guide on how to define admin groups for CounterStrikeSharp.

## Adding Groups

Groups can be created to group a series of permissions together under one tag. They are defined in `configs/admin_groups.json`. The important things you need to declare is the name of the group and the permissions they have.

```json
"#css/simple-admin": {
  "flags": [
    "@css/generic",
    "@css/reservation",
    "@css/ban",
    "@css/slay",
  ]
}
```

You can add admins to groups using the `groups` array in `configs/admins.json`
```json
{
  "erikj": {
    "identity": "76561198808392634",
    "flags": ["@mycustomplugin/admin"],
    "groups": ["#css/simple-admin"]
  },
  "Another erikj": {
    "identity": "STEAM_0:1:1",
    "flags": ["@mycustomplugin/admin"],
    "groups": ["#css/simple-admin"]
  }
}
```

> [!NOTE]
> All group names MUST start with a hashtag # character, otherwise CounterStrikeSharp won't recognize the group.


Admins can be assigned to multiple groups and they will inherit their flags. You can manually assign groups to players in code with `AdminManager.AddPlayerToGroup` and `AdminManager.RemovePlayerFromGroup`.

