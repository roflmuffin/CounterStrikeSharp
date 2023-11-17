---
title: Defining Command Overrides
description: A guide on how to define command overrides for CounterStrikeSharp.
---

## Defining Admin and Group specific overrides

Command permissions can be overriden so specific admins or groups can execute the command, regardless of any permissions they may or may not have. You can define command overrides by adding the `command_overrides` key to each admin in `configs/admins.json` or group in `configs/admin_groups.json`. Command overrides can either be set to `true`, meaning the admin/group can execute the command, or `false`, meaning the admin/group cannot execute the command at all.

```json
{
  "ZoNiCaL": {
    "identity": "76561198808392634",
    "flags": ["@css/changemap", "@css/generic"],
    "immunity": 100,
    "command_overrides": {
			"example_command": true
		}
  }
}
```

```json
"#css/simple-admin": {
  "flags": [
    "@css/generic",
    "@css/reservation",
    "@css/ban",
    "@css/slay",
  ],
  "command_overrides": {
    "example_command_2": false
  }
}
```

You can set a command override for a player in code using `AdminManager.SetPlayerCommandOverride`.

## Replacing Command permissions

Command permissions can be entirely replaced. These are defined in `configs/admin_overrides.json`. The important things you need to declare are what commands are being changed and what their new flags are. Command overrides can be set to be enabled or disabled, and you can toggle them in code with `AdminManager.SetCommandOverrideState`. You can also specify whether the command override requires the caller to have all of the new permissions (similar to a `RequiresPermissions` attribute check) or only one or more permissions (similar to a `RequirePermissionsOr` attribute check). You cannot stack permission checks. 

```json
"css": {
  "flags": [
    "@css/custom-permission"
  ],
  "check_type": "all",
  "enabled": true
}
```

You can check if a command has been overriden in code using `AdminManager.CommandIsOverriden`, and you can manipulate the command override permissions using `AdminManager.AddPermissionOverride` and `AdminManager.RemovePermissionOverride`.