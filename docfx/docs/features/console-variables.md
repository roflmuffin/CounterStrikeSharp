---
title: Console Variables
description: How to read & write console variables (ConVars).
---

## Finding a ConVar

Use the `ConVar.Find` static method to find a reference to an existing ConVar (or `null`).

```csharp
var cheatsCvar = ConVar.Find("sv_cheats");
```

## Manipulating Primitive Values

Reading the value of a ConVar will depend on the type; for basic value Cvars (like float, int, bool) you can use the `GetPrimitiveValue` method to get a `ref` to the value.

```csharp
cheatsCvar.GetPrimitiveValue<bool>(); // false
```

Because this is passed as a ref, you can simply set this value and it will change the underlying value, e.g.

```csharp
cheatsCvar.GetPrimitiveValue<bool>() = true; // false

// You can also use the simplified helper:
cheatsCvar.SetValue(true);
```

## Manipulating Strings

Because we have to do extra work to marshal strings between C# & the server, accessing strings is done differently, through the `StringValue` property which has getters & setters provided for you:

```csharp
var stringCvar = ConVar.Find("sv_skyname");
Console.WriteLine($"sv_skyname = {stringCvar.StringValue}");
stringCvar.StringValue = "foobar";
```

## Manipulating Native Objects

Native objects must be handled differently as well, since these are not refs or strings, to do so we can use the `GetNativeValue()` method:

```csharp
var fogCvar = ConVar.Find("fog_color");
var fogColor = fogCvar.GetNativeValue<Vector>();
Console.WriteLine($"fog_color = {fogColor}");

// You can then manipulate the vector as normal.
fogColor.X = 0.12345;
```
