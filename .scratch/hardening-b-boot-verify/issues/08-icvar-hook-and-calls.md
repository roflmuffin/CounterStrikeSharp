# 08: ICvar hook and direct calls

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Hooks and calls through Slot Bindings). ICvar shifted slots we use in 3 incidents.

**What to build:** The DispatchConCommand hook and every direct call on ICvar (about 14) go through verified Slot Bindings in the tier0 module, backing `cssharp.commands/console-dispatch` and one Engine Feature per plugin-visible cvar/command API (enumerated against the code, named under the catalog rule).

**Blocked by:** 03, 04

**Status:** ready-for-agent

**Tier:** 2 add-on · must

- [ ] The catalog lists every ICvar-backed Engine Feature with its Slot Bindings
- [ ] Vtable Check works on the tier0 module (class CCvar)
- [ ] Real-build test (local): zero wrong resolutions on the archived tier0 pair
- [ ] A failed cvar Slot Binding disables only the Engine Features using it
- [ ] End-to-end: console dispatch one off → heals, commands still dispatch; one corrupted cvar call → only its Engine Feature disabled
- [ ] Windows behaves as today
