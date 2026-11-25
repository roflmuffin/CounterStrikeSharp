# 17: Promote Proxy Mode to the recommended install

Spec: `.scratch/hardening-d-own-loader/spec.md` (Rollout).

**What to build:** Once the Proxy has gone through 3 game updates on an OS without needing a fix, switch the docs' *recommended* install to Proxy Mode for that OS. Count each OS separately. Update the panel guidance too. Code defaults and `gameinfo.gi` handling don't change.

**Blocked by:** 11, 12 (and 3 game updates in the field per OS)

**Status:** ready-for-human

**Tier:** 3 dependent add-on · later

**Size / risk:** S · low. Needs field evidence and a human call.

- [ ] Update log: build ids passed per OS with no Proxy fix
- [ ] Docs flipped for each OS that reached 3
