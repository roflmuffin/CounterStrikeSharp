# 09: Game-class virtuals verified and healed

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Slot Bindings and the Gamedata shape). Respawn and CommitSuicide moved 15 times each.

**What to build:** Gamedata offsets that are game-class virtuals (respawn, commit suicide, change team, teleport, item services drop/remove/give, skeleton instance, find picker entity, is-player-pawn, check-transmit) gain class, module and fingerprint-window fields and become verified Slot Bindings with Heal. Native and managed callers use the verified or healed index from the Resolution Report, including plugins that read these offsets by key. Struct field offsets are untouched.

**Blocked by:** 03, 04; spec A 12 (managed helpers read from the report)

**Status:** ready-for-agent

**Tier:** 2 add-on · must

- [ ] Every game-class virtual offset carries its window; struct field offsets are unchanged
- [ ] Real-build test (local): over the 7 archived pairs, the known moves (e.g. ChangeTeam 102→104) heal or fail, never silently wrong
- [ ] Generator replay regenerates the hand-fixed offset history correctly or flags it
- [ ] Reading one of these offsets by key returns the verified or healed index
- [ ] End-to-end: CommitSuicide one off → heals and works; corrupted → only commit-suicide disabled
- [ ] Windows behaves as today
