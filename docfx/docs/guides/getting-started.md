---
title: Getting Started
description: How to get started installing & using CounterStrikeSharp.
---

# Getting Started

In this guide you will learn how to install CounterStrikeSharp onto your vanilla Counter-Strike 2 server. `CounterStrikeSharp` uses `Metamod:Source` as its main way of communicating with the game server, so both frameworks will need to be installed.

If you're more of a visual person, here is a <a href="https://www.youtube.com/watch?v=FlsKzStHJuY" target="_blank">Youtube video</a> that covers everything.

## Prerequisites
- <a href="https://www.metamodsource.net/downloads.php/?branch=master" target="_blank">Metamod: Source 2.X Dev Build</a>
- <a href="https://github.com/roflmuffin/CounterStrikeSharp/releases" target="_blank">CounterStrikeSharp With Runtime</a>

## Installing Metamod

1. Extract Metamod and copy the `/addons/` directory to `/game/csgo/`.
2. Inside `/game/csgo/`, locate `gameinfo.gi`.
3. Create a new line underneath `Game_LowViolence    csgo_lv` and add `Game    csgo/addons/metamod`.
4. Restart your game server.

Your `gameinfo.gi` should look like <a href="../../images/gameinfogi-example.png" target="_blank">this</a>. Type `meta list` in your server console to see if Metamod is loaded.

## Installing CounterStrikeSharp

1. Extract CounterStrikeSharp and copy the `/addons/` directory to `/game/csgo/`.
2. Restart your game server.

Running the command `meta list` in the console should show 1 plugin loaded 🎉

```shell
meta list
Listing 1 plugin:
  [01] CounterStrikeSharp (0.1.0) by Roflmuffin
```

> [!CAUTION]
> For Windows servers, you must have <a href="https://aka.ms/vs/17/release/vc_redist.x64.exe" target="_blank">Visual Studio Redistributables</a> installed otherwise CounterStrikeSharp will not work.

## Upgrading CounterStrikeSharp

To upgrade CounterStrikeSharp you simply need to download the latest release and copy it to your server, the same as the original installation. 

CounterStrikeSharp is designed in a way where your configuration files will not be overwritten if you do this. As CounterStrikeSharp is already installed, you may download the non `with-runtime` build, but you will need to ensure your .NET runtime is up-to-date yourself. 

## Troubleshooting

- If this is your first time installing, you **MUST** download the `with-runtime` version. This includes a copy of the .NET runtime, which is required to run the plugin.
- Depending on your OS you might also either need to install `libicu` / `icu-libs` / `libicu-dev` using your package manager for .NET to run.
- If you get `Unknown Command` when typing `meta list` into your console, double-check the folders are copied over correctly and that your `gameinfo.gi` file is correctly modified.

Your folder structure should look like this:

```shell
<server_path>/game/csgo/addons > tree -L 2
addons
├── counterstrikesharp
│   ├── api
│   ├── bin
│   ├── dotnet
│   ├── plugins
│   └── gamedata
│
├── metamod
│   ├── bin
│   ├── counterstrikesharp.vdf
│   ├── metaplugins.ini
│   └── README.txt
├── metamod.vdf
└── metamod_x64.vdf
```
