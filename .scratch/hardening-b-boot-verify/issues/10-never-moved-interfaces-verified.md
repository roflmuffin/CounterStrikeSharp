# 10: Never-moved interfaces verified

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Hooks and calls through Slot Bindings, New Engine Features).

**What to build:** Hooks and direct calls on interfaces that never shifted a slot we use (game event manager, game event system, network server service, engine service manager, game entities) are declared as Slot Bindings and verified at boot. Hooks move to verified indices; direct calls stay compiled, and if a verified index differs from the compiled one, their Engine Features are disabled. Adds `cssharp.server/startup`, `cssharp.server/loop-mode` and `cssharp.network/user-messages`; `cssharp.events/game-events` additionally depends on the event manager's load-events and fire-event Slot Bindings. Network messages and the schema system are reported as unverified (modules not archived).

**Blocked by:** 03, 04

**Status:** ready-for-agent

**Tier:** 2 add-on · later

- [ ] Each of these interfaces' used slots is a verified Slot Binding
- [ ] A compiled direct call whose verified index differs disables its Engine Features instead of calling the wrong slot
- [ ] Startup report lists network messages and schema system as unverified
- [ ] End-to-end: corrupted fire-event window → game-events disabled, server up
- [ ] Windows behaves as today
