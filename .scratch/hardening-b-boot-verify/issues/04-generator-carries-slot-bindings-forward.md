# 04: Generator carries Slot Bindings forward

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Generator). ADR-0004.

**What to build:** The generator's carry-forward mode: given the previous Gamedata, the previous build's binaries and a new build's binaries from the shared archive, it aligns each Slot Binding's class vtable between the builds and emits Gamedata with updated indices, fingerprint windows and recorded vtable sizes. A Slot Binding whose old window does not match exactly once in the new vtable is left unchanged and listed for a human. It never guesses. Output is ordinary Gamedata, reviewed as a diff.

**Blocked by:** 02

**Status:** ready-for-agent

**Tier:** 1 base · must

- [ ] Replaying 1.41.9.7 → 1.42.0.1 moves GameFrame from 19 to 20 and refreshes its window
- [ ] A Slot Binding with an ambiguous or missing window is listed, and its entry is left unchanged
- [ ] An unchanged build produces no index changes
- [ ] Signatures and non-Slot-Binding entries pass through untouched
- [ ] Runs against `~/dev/cs2-builds/<build-id>/` and fails clearly when a build is missing from the archive
- [ ] Uses the same fingerprint core as the boot-time Vtable Check
