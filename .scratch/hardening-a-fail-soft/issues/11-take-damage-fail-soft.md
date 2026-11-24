# 11: Take-damage fails soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Engine Feature catalog). Migration batch.

**What to build:** Move `cssharp.entity/take-damage-hooks` (Bindings: `CBaseEntity_TakeDamageOld`, `CBaseEntity_IsPlayerPawn`) and `cssharp.entity/take-damage-hit-group` (`CTakeDamageInfo_HitGroup`) onto the resolver and report. The custom-signature path no longer reads the take-damage Gamedata entry in a way that breaks when it is missing.

**Blocked by:** 05

**Status:** ready-for-agent

- [ ] Breaking either take-damage Binding disables take-damage-hooks; entity and player take-damage listeners never fire and log once
- [ ] Breaking the hit-group offset disables only take-damage-hit-group; reading it throws the typed exception
- [ ] Creating a plugin memory function from a raw signature works even when the take-damage Gamedata key is absent
- [ ] End-to-end harness gains a take-damage case, including a plugin requiring take-damage-hooks being refused
