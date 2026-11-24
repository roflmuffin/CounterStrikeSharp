# 10: Game-system events fail soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Fatal vs fail soft). Migration batch.

**What to build:** Move `cssharp.server/game-system-events` (Binding: `IGameSystem_InitAllSystems_pFirst`) onto the resolver and report. When it fails, only precache, pre/post entity think and the button-change listener are disabled; CounterStrikeSharp loads (today a false return here aborts the load).

**Blocked by:** 02, 05

**Status:** ready-for-agent

- [ ] With the game-system signature broken, CounterStrikeSharp loads and reports game-system-events disabled
- [ ] Listeners for precache and entity think register, log once, and never fire while disabled
- [ ] End-to-end harness gains this case
