---
title: Getting Started
description: How to get started installing & using CounterStrikeSharp.
---

# Installation

### Installing Metamod

`CounterStrikeSharp` uses `Metamod:Source` as its main way of communicating with the game server. To install it, you can follow the detailed instructions found <a href="https://cs2.poggu.me/metamod/installation/" target="_blank">here</a>.

### Installing CounterStrikeSharp

Download the latest release of CounterStrikeSharp from <a href="https://github.com/roflmuffin/CounterStrikeSharp/actions/workflows/cmake-single-platform.yml" target="_blank">GitHub actions build pages</a> (just choose the latest development snapshot). **You may need to be logged into GitHub to gain access to the downloads**.

:::caution[.NET Runtime]
If this is your first time installing, you will need to download the `with-runtime` version. This includes a copy of the .NET runtime, which is required to run the plugin.
Depending on the os you might also either need to install `libicu` / `icu-libs` / `libicu-dev` using your package manager for .NET to run or setting `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true` in your servers environment variables. You can find more infos about that <a href="https://github.com/dotnet/runtime/blob/main/docs/design/features/globalization-invariant-mode.md#enabling-the-invariant-mode" target="_blank">here</a> 

Subsequent upgrades will not require the runtime, unless a version bump of the .NET runtime is required (i.e. from 7.0.x to 8.0.x). We will inform you when this occurs.
:::

Extract the `addons` folder to the `/csgo/` directory of the dedicated server. The contents of your addons folder should contain both the `counterstrikesharp` folder and the `metamod` folder as seen below.

```shell
<server_path>/game/csgo/addons > tree -L 2
addons
├── counterstrikesharp
│   ├── api
│   ├── bin
│   ├── dotnet
│   ├── plugins
│   └── gamedata
│
├── metamod
│   ├── bin
│   ├── counterstrikesharp.vdf
│   ├── metaplugins.ini
│   └── README.txt
├── metamod.vdf
└── metamod_x64.vdf
```

### Start the Server

Launch your CS2 dedicated server as normal. If everything is working correctly, you should see a message in the console that says `CSSharp: CounterStrikeSharp.API Loaded Successfully.`.

Running the command `meta list` in the console should show 1 plugin loaded 🎉

```shell
meta list
Listing 1 plugin:
  [01] CounterStrikeSharp (0.1.0) by Roflmuffin
```
