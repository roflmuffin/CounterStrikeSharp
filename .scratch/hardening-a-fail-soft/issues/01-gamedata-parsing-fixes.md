# 01: Gamedata parsing fixes

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Prefactor).

**What to build:** Three small correctness fixes on the path every Binding goes through, so later tickets build on sound parsing and scanning:
- A signature containing invalid hex characters is rejected as an invalid pattern instead of silently becoming a garbage byte.
- Every signature scanner tests the final candidate position in a segment, so a match at the very end is found.
- Managed Gamedata reports an offset that is missing for the current OS as missing instead of silently returning 0.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

**Tier:** 0 standalone · must

- [ ] A signature with an invalid hex token fails to parse, with a log naming the Gamedata key
- [ ] A pattern placed at the last possible position of a segment is found by every scanner
- [ ] Requesting an offset whose per-OS entry is absent raises a clear "missing for this OS" error, never 0
- [ ] Existing Gamedata still resolves exactly as before (no behaviour change for valid entries)
- [ ] Managed unit tests cover the missing per-OS offset case
