---
title: Console Commands
description: How to add a new console command
---

# Console Commands

How to add a new console command

## Adding a Console Command

### Automatic registration

CounterStrikeSharp will automatically register console commands marked with a `ConsoleCommand` attribute on the `BasePlugin` class. These commands are automatically registered/deregistered for you on hot reload.

```csharp
[ConsoleCommand("custom_command", "This is an example command description")]
public void OnCommand(CCSPlayerController? player, CommandInfo command)
{
    if (player == null) {
        Console.WriteLine("Command has been called by the server.");
        return;
    }

    Console.WriteLine("Custom command called.");
}
```

### On Load

It is also possible to bind commands in the `OnLoad` (or anywhere you have access to the plugin instance).

```csharp
public override void Load(bool hotReload)
{
    AddCommand("on_load_command", "A command is registered during OnLoad", (player, info) =>
    {
        if (player == null) return;
        Console.WriteLine($"Custom command called.");
    });
}
```

## Accessing Command Parameters

`CommandInfo` class provides access to the calling command parameters. The first parameter will always be the command being called. Quoted values are returned unquoted, e.g.

```csharp
[ConsoleCommand("custom_command", "This is an example command description")]
public void OnCommand(CCSPlayerController? player, CommandInfo command)
{
    Console.Write($@"
Arg Count: {command.ArgCount}
Arg String: {command.ArgString}
Command String: {command.GetCommandString}
First Argument: {command.ArgByIndex(0)}
Second Argument: {command.ArgByIndex(1)}");
}
```

```shell
> custom_command "Test Quoted" 5 13

# Output
Arg Count: 4
Arg String: "Test Quoted" 5 13
Command String: custom_command "Test Quoted" 5 13
First Argument: custom_command
Second Argument: Test Quoted
```

## Helper Attribute

CounterStrikeSharp provides the `CommandHelper` attribute for Command methods (function callback) to simplify the process of checking for the correct amount of arguments and to restrict commands to being executed by the server console or by players (or both!).

```csharp
[ConsoleCommand("freeze", "Freezes a client.")]
[CommandHelper(minArgs: 1, usage: "[target]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
public void OnFreezeCommand(CCSPlayerController? caller, CommandInfo command)
{
    ...
}
```

If a client tries to execute the command without the `[target]` argument, it will print a message to them in chat:

```shell
[CSS] Expected usage: "!freeze [target]".
```

If a command is executed by the wrong user, it will print a message to them:

```shell
[CSS] This command can only be executed by clients.
```

Valid `CommandUsage` values:

```csharp
public enum CommandUsage
{
    CLIENT_AND_SERVER = 0,
    CLIENT_ONLY,
    SERVER_ONLY
}
```
