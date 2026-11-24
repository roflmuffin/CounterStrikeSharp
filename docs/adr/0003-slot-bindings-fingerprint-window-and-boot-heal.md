# Slot Bindings: fingerprint windows, healed at boot

Engine interface slots we hook or call on interfaces that actually move (ISource2Server hooks, IServerGameClients hooks, IVEngineServer2 and ICvar hooks and calls, plus existing entity vfunc offsets) are Slot Bindings in Gamedata, not compiled header indices. Each carries a fingerprint window: the masked instruction fingerprint of its slot and one neighbour each side (immediates and RIP-relative operands masked). At boot the Vtable Check compares the live vtable: a matching window resolves the Binding; a window that matches exactly once within a bounded search range elsewhere resolves it at the new index as a Heal, reported with its evidence; anything else fails the Binding and disables only its Engine Features (ADR-0002). A vtable size change alone only warns.

Measured on 8 archived builds: 0.86% of unmoved slots changed fingerprint (2.6% of our 34 slots, mostly in one large update), 6 of 6 real moves of our slots detected, and at 1.42.0.1 every one of the 10 shifted ISource2Server slots had its old window exactly one slot later.

## Considered Options

- **Size-only, disable the whole interface on any size change** (s2script): simple, but sizes changed in ~8% of class-pairs with none of our slots moving, so it disables working features about as often as it catches real shifts, and never heals.
- **Detour GameFrame by reference resolution** (ModSharp): survives a slot insert for one function only, and detouring virtuals is unsafe in general because ~30% of interface slots share a body with other slots. Healing protects every Slot Binding with one mechanism (same rule as the PoC's ADR-0007).
- **No heal, fail and wait for regenerated Gamedata**: safe but turns every slot insert into a feature outage until a human runs the generator.

## Consequences

- A Heal is only accepted on an exactly-unique window match and never across slots with identical bodies (those fail instead). Every Heal appears in the startup report; the Resolution Report gains a "healed" status alongside resolved and failed.
- The runtime never writes healed indices back to Gamedata; the generator does that offline.
- Windows Slot Bindings come from the hl2sdk headers, marked unverified; the Vtable Check and Heal are Linux-only for now.
- Wrong-but-unique signatures are not caught by fingerprints (they would be generated from the wrong match); they need a second independent anchor and belong to T2.
