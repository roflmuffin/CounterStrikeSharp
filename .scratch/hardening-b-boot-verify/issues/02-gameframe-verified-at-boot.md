# 02: GameFrame verified at boot (tracer)

Spec: `.scratch/hardening-b-boot-verify/spec.md`. ADR-0003, ADR-0004.

**What to build:** The first Slot Binding end to end: `cssharp.server/game-frame` (ISource2Server GameFrame). GameFrame's slot index and fingerprint window live in Gamedata in the additive Slot Binding shape. At boot the resolver, now behind its resolver interface, runs a Vtable Check: it finds the class vtable by RTTI, fingerprints the window and resolves the Binding when it matches, or fails it (disabling game-frame) when it does not. GameFrame is hooked by the verified slot index instead of its compiled header position. The fingerprint core is shared with a first slice of the generator: a bootstrap mode that emits the Slot Binding entry (index, window, recorded vtable size) for a named class and slot on a given build. No Heal yet.

**Blocked by:** 01; spec A 03 (C++ test target); spec A 04 (resolver and Resolution Report)

**Status:** ready-for-agent

**Tier:** 1 base · must

- [ ] An x86-64 decoder is available natively (reused from an already linked hooking library if present, otherwise a small permissive one)
- [ ] The C++ fingerprint matches the Python research oracle for every slot of CSource2Server on archived builds (local, skipped when the archive is absent) and on synthetic fixtures (CI)
- [ ] Generator bootstrap mode emits GameFrame's Slot Binding for build 1.41.9.7 from the archive, and that entry is committed to Gamedata
- [ ] Resolver unit tests (synthetic vtables): matching window resolves; mismatching window fails with reason "fingerprint mismatch"; size change alone resolves with a warning
- [ ] All Binding resolution goes through the resolver interface; the built-in adapter is the only code locating Bindings
- [ ] End-to-end: valid Gamedata → GameFrame hooked by index, ticks and timers work; corrupted GameFrame fingerprint → game-frame disabled, server stays up, startup report lists it
- [ ] Existing readers of Gamedata by key are unaffected by the new fields
- [ ] Windows builds and behaves as today (header index, reported unverified)
