---
title: Shared Plugin API (Capabilities)
description: How to add inter-plugin communication to CounterStrikeSharp plugins.
---

# Shared Plugin API

> [!NOTE]
> **New (experimental)**: You can now resolve plugin dependencies directly from your local **NuGet packages cache** instead of copying every DLL into the `shared/` folder. See **Dependency Resolution** below. This feature **disabled by default.**

How to expose and use shared plugin APIs between multiple plugins.

## Creating a Contract Library

Inter-plugin communication requires a contract/shared library that simply exposes the shape of the API, using simple interfaces. e.g.

```csharp
public interface IBalanceHandler
{
    decimal Balance { get; }

    // These are just here to show that you can have methods on your shared types.
    // You could also add a Setter to the Balance property.
    public decimal Add(decimal amount);
    public decimal Subtract(decimal amount);
}
```

This library ideally should not contain any business logic, and simply define the schema for callers.

This library should be placed in the `shared` subfolder, in the same folder layout as the plugins folder. So if a contract DLL is named `MySharedApi.dll` its file path should be: `shared/MySharedApi/MySharedApi.dll`.

## Creating a Capability

A capability can be declared with a simple static variable in your plugin class. A `PlayerCapability` is a player specific capability, and a `PluginCapability` is generic functionality that a plugin can expose.

```csharp
public static PlayerCapability<IBalanceHandler> BalanceCapability { get; } = new("myplugin:balance");

public static PluginCapability<IBalanceService> BalanceServiceCapability { get; } = new("myplugin:balance_service");
```

For every plugin that wishes to use this new "Balance API", they must ensure they create a capability using the shared API interface (`IBalanceHandler`), as well as use the same name (`myplugin:balance`).

## Registering a Capability

If you are the plugin that is expected to provide the basis of the API (i.e. you are providing a currency/balance plugin which does nothing except store users balances), then you will need to provide the implementation that other callers will use. This is done through the use of static members on the `Capabilities` class:

```csharp
// Player capabilities are given the calling player context
Capabilities.RegisterPlayerCapability(BalanceCapability, player => new BalanceHandler(player));

// Plugin capabilities can simply return an instance of the interface
Capabilities.RegisterPluginCapability(BalanceServiceCapability, () => new BalanceService());
```

### Using Capabilities

To utilise a capability, simply call the `.Get()` method provided on the static capability you declared earlier, i.e.

```csharp
var balance = BalanceCapability.Get(player);
var balanceService = BalanceServiceCapability.Get();

if (balance == null) return;

balance.Add(500);
```

This value _MUST_ be checked for null, as if there are no plugins providing implementations for a given capability, this method will return null, and you must handle this flow in your plugin.


## Dependency Resolution

CounterStrikeSharp supports two complementary ways to resolve **external** assemblies used by your plugins and shared contracts:

1. **Shared Folder Resolution (manual)**: copy dependency DLLs into `shared/<PackageName>/<Assembly>.dll`.
2. **NuGet Dependency Resolver (auto)**: when enabled, resolves missing assemblies from the local **NuGet packages root**

### Enabling the NuGet Resolver

Add the following property to your core config (disabled by default):

```json
{
    ...
    "PluginResolveNugetPackages": true
    ...
}
```

> [!NOTE]
> The engine looks for assemblies in the NuGet cache defined by the `NUGET_PACKAGES` environment variable, or falls back to the default user cache (e.g., `~/.nuget/packages` on Linux/macOS, `%UserProfile%\.nuget\packages` on Windows).

### Dependencies Resolution Order

When the NuGet resolver is **enabled**, resolution proceeds in this general order:

1. **Plugins directory** (in-place assemblies)
2. `shared/` **folder** (existing shared assemblies mechanism)
3. **NuGet cache** (auto-resolver)

This lets you keep proven `shared/` workflows while reducing manual copying for common NuGet dependencies.