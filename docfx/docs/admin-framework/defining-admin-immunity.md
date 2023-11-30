---
title: Defining Admin Immunity
description: A guide on how to define immunity for admins for CounterStrikeSharp.
---

## Player Immunity

Admins can be assigned an immunity value, similar to SourceMod. If an admin or player with a lower immunity value targets another admin or player with a larger immunity value, then the command will fail. You can define an immunity value by adding the `immunity` key to each admin in `configs/admins.json`.

```json
{
  "ZoNiCaL": {
    "identity": "76561198808392634",
    "flags": ["@css/changemap", "@css/generic"],
    "immunity": 100
  }
}
```

You can even assign an immunity value to groups. If an admin has a lower immunity value, the group's value will be used instead.

```json
"#css/simple-admin": {
  "flags": [
    "@css/generic",
    "@css/reservation",
    "@css/ban",
    "@css/slay",
  ],
  "immunity": 100
}
```

> [!NOTE]
> CounterStrikeSharp does not automatically handle immunity checking. It is up to individual plugins to handle immunity checks as they can implement different ways of targeting players. This can be done in code with `AdminManager.CanPlayerTarget`. You can also set a player's immunity in code with `AdminManager.SetPlayerImmunity`.