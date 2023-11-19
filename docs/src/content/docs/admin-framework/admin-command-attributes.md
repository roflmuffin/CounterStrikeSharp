---
title: Admin Command Attributes
description: A guide on using the Admin Command Attributes in plugins.
---

## Assigning permissions to a Command

Assigning permissions to a Command is as easy as tagging the Command method (function callback) with either a `RequiresPermissions` or `RequiresPermissionsOr` attribute. The difference between the two attributes is that `RequiresPermissionsOr` needs only one permission to be present on the caller to be passed, while `RequiresPermissions` needs the caller to have all permissions listed. CounterStrikeSharp handles all of the permission checks behind the scenes for you.

```csharp
[RequiresPermissions("@css/slay", "@custom/permission")]
public void OnMyCommand(CCSPlayerController? caller, CommandInfo info)
{
    ...
}
```

```csharp
[RequiresPermissionsOr("@css/ban", "@custom/permission-2")]
public void OnMyOtherCommand(CCSPlayerController? caller, CommandInfo info)
{
    ...
}
```

You can even stack the attributes on top of each other, and all of them will be checked.

```csharp
// Requires (@css/cvar AND @custom/permission-1) AND either (@custom/permission-1 OR @custom/permission-2).
[RequiresPermissions("@css/cvar", "@custom/permission-1")]
[RequiresPermissionsOr("@css/ban", "@custom/permission-2")]
public void OnMyComplexCommand(CCSPlayerController? caller, CommandInfo info)
{
    ...
}
```

You can also check for groups using the same attributes.

```csharp
[RequiresPermissions("#css/simple-admin")]
public void OnMyGroupCommand(CCSPlayerController? caller, CommandInfo info)
{
    ...
}
```

