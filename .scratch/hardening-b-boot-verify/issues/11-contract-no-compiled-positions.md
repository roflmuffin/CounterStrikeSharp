# 11: Contract: no compiled positions for migrated interfaces

Spec: `.scratch/hardening-b-boot-verify/spec.md`. Contract step.

**What to build:** Remove the old paths: no hook or call on a migrated interface (ISource2Server, IServerGameClients, IVEngineServer2, ICvar) or game-class virtual uses a compiled header slot position any more; every Slot Binding is verified at boot; the full real-build replay and the full end-to-end run pass.

**Blocked by:** 05, 06, 07, 08, 09, 10

**Status:** ready-for-agent

**Tier:** 3 cleanup · later

- [ ] No compiled slot position remains for migrated interfaces or game-class virtuals
- [ ] Real-build replay over all archived pairs: zero wrong resolutions; every known move healed or failed
- [ ] Full end-to-end run with valid Gamedata: no Heals, no failures, native test suite passes
- [ ] Windows behaves as today
