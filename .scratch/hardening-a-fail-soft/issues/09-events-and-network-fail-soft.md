# 09: Events and network fail soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Fatal vs fail soft). Migration batch.

**What to build:** Move `cssharp.events/game-events`, `cssharp.events/fire-event-to-client`, `cssharp.network/state-changed` and `cssharp.network/check-transmit` onto the resolver and report. The two cases that abort the whole CounterStrikeSharp load today (CheckTransmit offset missing; the game event manager's RTTI vtable not found) now disable only their Engine Feature. A missing event-manager init Binding no longer silently stops all managed game-event handlers without a report.

**Blocked by:** 02, 05

**Status:** resolved

**Tier:** 2 add-on · must

- [ ] With the CheckTransmit offset removed, CounterStrikeSharp loads; check-transmit is disabled and reported
- [ ] With the event manager RTTI vtable unresolvable, CounterStrikeSharp loads; game-events is disabled and reported
- [ ] With the event-manager init Binding broken, game-events is reported disabled (not silently dead)
- [ ] Registering a game-event handler while game-events is disabled succeeds, logs once, never fires
- [ ] End-to-end harness gains the CheckTransmit and event-manager cases

## Comments

- Resolved on `hardening/spec-a` (merge of `spec-a/09-events-and-network-fail-soft`). 4 native rows (game-events incl. RTTI vtable, fire-event-to-client, state-changed, check-transmit); new `globals::InitializeHooks()` after `ResolveCoreBindings()`; CheckTransmit offset and RTTI vtable misses no longer abort Load. Managed typed exceptions for events / SetStateChanged; `RegisterEventHandler` gated. E2E `eng/e2e/cases/game-events.sh`, `check-transmit.sh` (RTTI case via harness-only `--server-patch`, restored before the lock is released). Integration tip abe297f4 valid run: all 51 Bindings resolved, all 36 Engine Features available, smoke 195/1/4. Leftovers for 15: `CCheckTransmitInfoList` still reads CheckTransmitPlayerSlot via `GameData.GetOffset`; real-binary doctest fixture lacks a schema lookup for ticket 10 rows.
