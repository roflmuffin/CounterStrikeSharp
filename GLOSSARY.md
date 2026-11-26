# CounterStrikeSharp

A .NET plugin framework for Counter-Strike 2 dedicated servers. A native core, loaded as a Metamod plugin (Plugin Mode) or through its own Proxy (Proxy Mode), binds to the game binaries; managed plugins use the API it exposes.

## Game binding

**Gamedata**:
The per-OS table of signatures and offsets that tells CounterStrikeSharp where game functions and vtable slots are in the current CS2 build.
_Avoid_: config, sigs file

**Binding**:
One Gamedata entry resolved against the game binary: a signature, an offset, a vtable slot, or a schema field. It either resolves or fails.
_Avoid_: sig (for the concept), gamedata entry, address

**Slot Binding**:
A Binding whose value is a vtable slot index of a method we hook or call, on an engine interface or a game class.
_Avoid_: vfunc offset, hook index (not every Gamedata "offset" is a Slot Binding; some are struct field offsets)

**Vtable Check**:
The boot-time verification of a class's live vtable against the expected fingerprints of the slots we use.
_Avoid_: layout check, size check

**Heal**:
A Slot Binding resolved at a different index than Gamedata states, because its expected fingerprint window matched exactly once nearby. Reported with its evidence; never written back to Gamedata at runtime.
_Avoid_: auto-fix, fallback

**Engine Feature**:
A named piece of CounterStrikeSharp functionality that depends on one or more Bindings, and is either available or disabled as a whole. Examples: ClientPrint, entity output hooks, take-damage hooks.
_Avoid_: capability (that means plugin-to-plugin services), feature (bare)

**Resolution Report**:
The immutable record of how every Binding resolved against one CS2 build: which resolved, where, and why the others failed. Engine Feature availability is read from it.
_Avoid_: gamedata status, binding state

**Required Engine Feature**:
An Engine Feature a plugin declares it cannot run without; if it is disabled, that plugin does not load.
_Avoid_: dependency (ambiguous with plugin dependencies)

**Fail Mode**:
The operator's server-wide choice of what a disabled core Engine Feature does: `disable` (fail soft, the default) keeps the server running without it; `crash` logs the startup report and stops the server. The server-wide counterpart of a plugin's Required Engine Features.
_Avoid_: strict mode (that is the per-signature opt-in for plugin signatures)

**Plugin Signature**:
A signature a plugin resolves itself, raw or from a Gamedata file it ships. Reported per plugin with its match count and outcome, apart from the Resolution Report; it never disables an Engine Feature or triggers the Fail Mode.
_Avoid_: Binding (Bindings are CounterStrikeSharp's own)

**Game update**:
A new CS2 build shipped by Valve. It may move or remove things Bindings point at.
_Avoid_: patch (ambiguous with binary patches)

## Loading

**Host**:
What CounterStrikeSharp's core runs on: the provider of engine and server factories, level start/end, console output and the KHook instance. One of two kinds per process: the Metamod host or the own host.
_Avoid_: platform, environment

**Plugin Mode**:
CounterStrikeSharp loaded by Metamod as a Metamod plugin, using Metamod's KHook. The default install, and how CounterStrikeSharp has always loaded.
_Avoid_: Metamod mode, legacy mode

**Proxy Mode**:
CounterStrikeSharp loaded through its own Proxy as the game DLL, using its own KHook. Opted into with one `gameinfo.gi` line; works with or without Metamod installed.
_Avoid_: standalone (ambiguous: Proxy Mode may run alongside Metamod), own-loader mode

**Proxy**:
CounterStrikeSharp's `libserver.so`/`server.dll` that sits where the engine (or Metamod's loader) expects the game DLL, loads the real server and the core, and passes every interface through unchanged.
_Avoid_: loader (Metamod's loader is also a loader), shim

**Bootstrap**:
The thin Metamod plugin shipped with CounterStrikeSharp. In Plugin Mode it loads the core; in Proxy Mode it only hands Metamod's API to the already-running core so `MetaFactory` works. The only code that includes Metamod headers.
_Avoid_: shim, stub
