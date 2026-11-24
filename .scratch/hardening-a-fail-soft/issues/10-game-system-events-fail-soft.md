# 10: Game-system events fail soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Fatal vs fail soft). Migration batch.

**What to build:** Move `cssharp.server/game-system-events` (Binding: `IGameSystem_InitAllSystems_pFirst`) onto the resolver and report. When it fails, only precache, pre/post entity think and the button-change listener are disabled; CounterStrikeSharp loads (today a false return here aborts the load).

**Blocked by:** 02, 05

**Status:** resolved

**Tier:** 2 add-on · later

- [ ] With the game-system signature broken, CounterStrikeSharp loads and reports game-system-events disabled
- [ ] Listeners for precache and entity think register, log once, and never fire while disabled
- [ ] End-to-end harness gains this case

## Comments

- Resolved on `hardening/spec-a` (merge of `spec-a/10-game-system-events-fail-soft`). Native row with `IGameSystem_InitAllSystems_pFirst` + 5 RunThink SchemaField Bindings (schema resolves fine at boot: all 50 Bindings resolved on 1.42.0.1). Managed `ListenerEngineFeatures` gates precache / entity think / buttons-changed listeners (log once). Managed 94/94. E2E `eng/e2e/cases/game-system-events.sh`: valid + signature not found (server up, feature disabled, listeners warn once). Button-change listener unit-tested only. Leftover for 15: duplicate ERROR "Failed to initialize GameSystem" after the WARN; `CGameConfig::ResolveSignature` has no callers.
