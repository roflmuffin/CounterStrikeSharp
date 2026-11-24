# 05: Remaining ISource2Server hooks and calls

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Hooks and calls through Slot Bindings, New Engine Features).

**What to build:** The other six ISource2Server hooks (hibernation update, Steam API activated/deactivated, hostname changed, pre-fatal shutdown, update when not in game, pre-world update) and its two direct calls become verified Slot Bindings, hooked and called by verified index, each backing its Engine Feature (`cssharp.server/hibernation-events`, `steam-api-events`, `hostname-changed`, `fatal-shutdown`, `update-when-not-in-game`, `pre-world-update`, plus the Engine Features of the direct calls). After this ticket a 1.42.0.1-style insert heals every ISource2Server slot we use.

**Blocked by:** 03, 04

**Status:** ready-for-agent

**Tier:** 2 add-on · must

- [ ] Slot Bindings bootstrapped on 1.41.9.7 and carried forward by the generator
- [ ] Real-build test (local): all 10 ISource2Server Slot Bindings heal at +1 from 1.41.9.7 to 1.42.0.1, zero wrong
- [ ] Each new Engine Feature appears in the startup report and fails independently
- [ ] End-to-end: all ISource2Server Slot Bindings one index off → all heal, their listeners fire
- [ ] Windows behaves as today
