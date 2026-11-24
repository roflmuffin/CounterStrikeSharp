# 03: GameFrame heals a slot insert

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Vtable Check and Heal, Resolver seam). ADR-0003.

**What to build:** When GameFrame's fingerprint window does not match at its stated index but matches exactly once within ±16 slots, the Binding resolves at the new index as a Heal and GameFrame is hooked there. The Resolution Report carries the healed status with its evidence (old index, new index, matched window); healed Engine Features count as available; the startup report lists every Heal; plugins see the Heal through the query API. Ambiguous windows and shifts across identical bodies fail instead of healing.

**Blocked by:** 02; spec A 05 (report reaches plugins)

**Status:** ready-for-agent

**Tier:** 1 base · must

- [ ] Resolver unit tests (synthetic): shift by k within range heals with evidence; window matching twice fails as ambiguous; shift across identical bodies fails; shift beyond ±16 fails
- [ ] Real-build test (local, skipped without archive): GameFrame's Slot Binding from 1.41.9.7 resolved against 1.42.0.1 heals at +1
- [ ] Managed unit tests: a hand-built report with a healed Binding shows the feature available, the Heal in its status, and the Heal in the startup report
- [ ] End-to-end: Gamedata with GameFrame one index off → server heals it, timers fire, startup report shows the Heal
- [ ] Healed indices are never written back to Gamedata at runtime
- [ ] Windows behaves as today
