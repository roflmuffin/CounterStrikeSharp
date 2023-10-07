# CounterStrikeSharp

CounterStrikeSharp is a server side modding framework for Counter-Strike: Global Offensive. This project attempts to implement a .NET Core scripting layer on top of a Metamod Source Plugin, allowing developers to create plugins that interact with the game server in a modern language (C#) to facilitate the creation of maintainable and testable code.

## History

This project is an ongoing migration of a previous project (titled [VSP.NET](https://github.com/roflmuffin/vspdotnet)) whereby a scripting layer was added to a Valve Server Plugin for CSGO.

Due to the architectural changes of CS2, the plugin is being rebuilt on the ground up, to support Linux 64-bit, something which was previously impossible.

## Philosophy

As a result, there are a few key philosophies and trade-offs that drive the project.
- Only 64 bit is supported.
  - .NET only supports x64 on Linux; CSGO previously only supported 32 bit servers, but CS2 supports 64 bit on Linux.
- Most dedicated servers are hosted on Linux, and there are no real plans to support Windows.
- The scripting runtime will only support C#/.NET at this time, but there is no reason it cannot be extended in the future to support other languages by utilising the native bindings created by this plugin (in JS, Rust or Go for example).

## What works?
_(Note, these were features in the previous VSP.NET project, but have not been implemented yet in this project)_

These features are the core of the platform and work pretty well/have a low risk of causing issues.

- [x] Console Variables, Console Commands, Server Commands (e.g. sv_customvar)
- [x] Game Event Handlers & Custom Events (e.g. player_death)
- [x] Game Tick Based Timers (e.g. repeating map timers)
- [x] Listeners (e.g. client connected, disconnected, map start etc.)
- [x] Server Information (current map, game time, tick rate, model precaching)
- [x] Radio Menus (create menus and respond to selections)

## Use
Development builds are currently available through GitHub actions, you can download the latest build from [there](https://github.com/roflmuffin/CounterStrikeSharp/actions).

## What kind of works?

These features have a rudimentary implementation but have not been thoroughly tested.

- [x] Entity Manipulation
    - Basic manipulation of networked entity props/sendinfo e.g. position, team, ground entity, position, velocity etc.
    - Currently missing entity input/output functionality, for things like func_door "Open" inputs etc.
- [x] Engine Raycasts
    - Can do basic raycasts with predicate filter to match entities
- [x] Poor Memory Functionality
    - It is possible to hook and call virtual functions by supplying the int offset & parameters of the method.
    - Might cause crashes if you use the wrong parameters :(
    - These are things that are traditionally provided by SDK_Hooks or SDK_Tools in SourceMod
- [x] Multi Threading & Game Frames
    - Game Event Listeners & Command Handlers happen synchronously in the game frame
    - If you spawn a new thread/task in .NET you will need to queue your game actions for the next in-game frame or some things might crash.

## Examples

You can view the example Warcraft plugin located in the managed folder under "ClassLibrary2" (until it gets renamed). This plugin shows how you can hook events, create commands & console variables, use third party libraries (SQLite) and do basic entity manipulation.

## Credits

A lot of code has been borrowed from SourceMod as well as Source.Python, two pioneering source engine plugin frameworks which this project lends a lot of its credit to.
I've also used the scripting context & native system that is implemented in FiveM for GTA5.

## How to Build

Building requires CMake on Linux.

Clone the repository

```bash
git clone https://github.com/roflmuffin/counterstrikesharp
```

Init and update submodules

```bash
git submodule update --init --recursive
```

Make build folder

```bash
mkdir build
cd build
```

Generate CMake Build Files

```bash
cmake ..
```

Build

```bash
cmake --build . --config Debug
```
