# 11: Take-damage fails soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Engine Feature catalog). Migration batch.

**What to build:** Move `cssharp.entity/take-damage-hooks` (Bindings: `CBaseEntity_TakeDamageOld`, `CBaseEntity_IsPlayerPawn`) and `cssharp.entity/take-damage-hit-group` (`CTakeDamageInfo_HitGroup`) onto the resolver and report. The custom-signature path no longer reads the take-damage Gamedata entry in a way that breaks when it is missing.

**Blocked by:** 05

**Status:** resolved

**Tier:** 2 add-on · later

- [ ] Breaking either take-damage Binding disables take-damage-hooks; entity and player take-damage listeners never fire and log once
- [ ] Breaking the hit-group offset disables only take-damage-hit-group; reading it throws the typed exception
- [ ] Creating a plugin memory function from a raw signature works even when the take-damage Gamedata key is absent
- [ ] End-to-end harness gains a take-damage case, including a plugin requiring take-damage-hooks being refused

## Comments

- Resolved on `hardening/spec-a` (merge of `spec-a/11-take-damage-fail-soft`). Native row take-damage-hooks (TakeDamageOld sig + IsPlayerPawn offset), detour gated; `natives_memory.cpp` TakeDamageOld special case null-safe; managed row take-damage-hit-group; `ListenerEngineFeatures` listener gate (shared with 10). doctest 36, managed 105/105. E2E `eng/e2e/cases/take-damage.sh`: valid + both keys broken (192/0/4; listeners never fire; hit-group typed exception; raw plugin sig binds; plugin requiring take-damage-hooks refused). Decision to revisit in review: `CBaseEntity.IsPlayerPawn()` throws only when its own offset failed, not when the whole feature is disabled.
