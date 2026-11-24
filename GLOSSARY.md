# CounterStrikeSharp

A .NET plugin framework for Counter-Strike 2 dedicated servers. A native Metamod plugin binds to the game binaries; managed plugins use the API it exposes.

## Game binding

**Gamedata**:
The per-OS table of signatures and offsets that tells CounterStrikeSharp where game functions and vtable slots are in the current CS2 build.
_Avoid_: config, sigs file

**Binding**:
One Gamedata entry resolved against the game binary: a signature, an offset, a vtable slot, or a schema field. It either resolves or fails.
_Avoid_: sig (for the concept), gamedata entry, address

**Engine Feature**:
A named piece of CounterStrikeSharp functionality that depends on one or more Bindings, and is either available or disabled as a whole. Examples: ClientPrint, entity output hooks, take-damage hooks.
_Avoid_: capability (that means plugin-to-plugin services), feature (bare)

**Game update**:
A new CS2 build shipped by Valve. It may move or remove things Bindings point at.
_Avoid_: patch (ambiguous with binary patches)
