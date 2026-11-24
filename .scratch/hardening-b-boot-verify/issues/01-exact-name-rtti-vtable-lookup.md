# 01: Exact-name RTTI vtable lookup

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Fingerprints, Vtable Check and Heal). Prefactor.

**What to build:** Finding a class's vtable by RTTI matches the exact decorated class name only (not a substring of a longer name) and reports ambiguity when more than one primary vtable matches, instead of returning the first hit. Today CCSGameRules is found correctly only because of ordering (its decorated name also occurs inside a pointer-type name).

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

**Tier:** 0 standalone · must

- [ ] A class whose decorated name is a substring of another type's name resolves to its own vtable
- [ ] Two genuine matches are reported as ambiguous, not resolved to the first
- [ ] Every class lookup used today (event manager, HUD dialog variable, nav physics interface, plugin lookups by symbol) still resolves on the current build
- [ ] Windows behaves as today
