---
title: Defining Admin Groups
description: A guide on how to define admin groups for CounterStrikeSharp.
---

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

:::note
All group names MUST start with a hashtag # character, otherwise CounterStrikeSharp will recognize the group.
:::

Admins can be assigned to multiple groups and they will inherit their flags. You can manually assign groups to players in code with `AdminManager.AddPlayerToGroup` and `AdminManager.RemovePlayerFromGroup`.