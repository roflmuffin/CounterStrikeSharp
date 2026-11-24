# 09: Events and network fail soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Fatal vs fail soft). Migration batch.

**What to build:** Move `cssharp.events/game-events`, `cssharp.events/fire-event-to-client`, `cssharp.network/state-changed` and `cssharp.network/check-transmit` onto the resolver and report. The two cases that abort the whole CounterStrikeSharp load today (CheckTransmit offset missing; the game event manager's RTTI vtable not found) now disable only their Engine Feature. A missing event-manager init Binding no longer silently stops all managed game-event handlers without a report.

**Blocked by:** 02, 05

**Status:** ready-for-agent

- [ ] With the CheckTransmit offset removed, CounterStrikeSharp loads; check-transmit is disabled and reported
- [ ] With the event manager RTTI vtable unresolvable, CounterStrikeSharp loads; game-events is disabled and reported
- [ ] With the event-manager init Binding broken, game-events is reported disabled (not silently dead)
- [ ] Registering a game-event handler while game-events is disabled succeeds, logs once, never fires
- [ ] End-to-end harness gains the CheckTransmit and event-manager cases
