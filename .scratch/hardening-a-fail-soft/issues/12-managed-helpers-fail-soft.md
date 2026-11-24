# 12: Remaining managed helpers fail soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Engine Feature catalog). Migration batch, following the pattern from ticket 06.

**What to build:** Move every remaining managed-consumed Engine Feature onto lazy per-member helpers backed by the report: player (`set-pawn`, `switch-team`, `change-team`, `respawn`, `commit-suicide`, `post-think-hooks`), game rules (`terminate-round`, `find-picker-entity`), entity (`create`, `remove`, `set-model`, `teleport`, `trigger-touch-hooks`, `skeleton-instance`), items (`give-named-item`, `can-acquire-hooks`, `weapon-can-use-hooks`, `remove-player-item`, `drop-active-weapon`, `remove-weapons`, `weapon-data`) and `nav/nav-mesh`.

**Blocked by:** 01, 06

**Status:** ready-for-agent

**Tier:** 2 add-on · must

- [ ] Every listed Engine Feature is declared with its Bindings (as in the spec's catalog) and appears in the startup report
- [ ] Each fails independently: breaking one Binding disables only its Engine Features; calls throw the typed exception
- [ ] Offsets missing for the current OS disable their Engine Feature instead of calling slot 0
- [ ] Managed unit tests cover at least one signature-backed and one offset-backed helper in each group
- [ ] End-to-end harness gains a case breaking `CBasePlayerPawn_CommitSuicide`: server up, respawn and teleport still work
