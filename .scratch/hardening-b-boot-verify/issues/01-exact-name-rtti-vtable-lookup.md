# 01: Exact-name RTTI vtable lookup

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Fingerprints, Vtable Check and Heal). Prefactor.

**What to build:** Finding a class's vtable by RTTI matches the exact decorated class name only (not a substring of a longer name) and reports ambiguity when more than one primary vtable matches, instead of returning the first hit. Today CCSGameRules is found correctly only because of ordering (its decorated name also occurs inside a pointer-type name).

**Blocked by:** None (can start immediately)

**Status:** resolved

**Tier:** 0 standalone · must

- [x] A class whose decorated name is a substring of another type's name resolves to its own vtable
- [x] Two genuine matches are reported as ambiguous, not resolved to the first
- [x] Every class lookup used today (event manager, HUD dialog variable, nav physics interface, plugin lookups by symbol) still resolves on the current build
- [x] Windows behaves as today

## Comments

- Resolved on `spec-b/01-exact-name-rtti-vtable-lookup`. New pure module `src/core/rtti_vtable.{h,cpp}`: `rtti::FindPrimaryVtable(className, names, data)` returns `VtableLookup { address, matchCount }`. Exactness is "a typeinfo's name pointer points at the occurrence of `<len><name>\0`" (every occurrence is tried, so a linker tail-merged name still works); primary = offset-to-top 0; distinct vtables counted, >1 is ambiguous (address 0). Pointer scans are 8-byte aligned and bounded (the old `SignatureIterator` loop could read past a section end).
- Adapter: `CModule::LookupVirtualTable` (Linux: `.rodata` names, `.data.rel.ro(.local)` data; Windows: wraps the unchanged old lookup, matchCount 0/1). `CModule::FindVirtualTable` keeps its signature and returns nullptr with a warning on ambiguity. `ModuleImage::findVirtualTable` now returns `rtti::VtableLookup`; an ambiguous RTTI vtable Binding fails with the existing `FailureReason::Ambiguous` and its match count (no new enum value, managed unchanged).
- Tests: native doctest 45 cases / 400 assertions pass (6 new synthetic cases: substring of pointer-type name, tail-merged name, longer name containing it, two primary vtables ambiguous, secondary vtable ignored, not found; resolver maps ambiguous). Real binary (`CSSHARP_SERVER_BINARY`, 1.42.0.1 libserver.so): the fixture now loads the ELF as the loader would (PT_LOAD + RELATIVE/R_X86_64_64 relocations) and runs the real lookup; CGameEventManager, HUDPanelDialogVariableString_t, CNavPhysicsInterface, CCSGameRules, CCSPlayerPawn, CCSPlayerController, CBaseEntity, CCSGameRulesProxy each resolve to exactly one primary vtable whose own typeinfo name is the class; the core catalog (now through the real lookup instead of the old name-exists stand-in) still resolves all 46. Release build: the whole fixture (read, relocate, 9 lookups) takes 0.15 s. Managed unit tests 160 pass.
- Live smoke test added: `NativeTestsPlugin.RttiVtableTests` (same classes resolve; CCSGameRules equals the live game rules object's vptr; unknown class is not found). **Not run on the e2e server**: this worktree has no submodules (cloning them is refused by the sandbox: file transport), so `eng/e2e/run.sh` cannot build the plugin here. Plugin sources were syntax-checked with the e2e build's flags against spec-a's libraries; the test plugin builds. Run `eng/e2e/cases/game-events.sh` (and check the RttiVtableTests outcomes) from a worktree with submodules before merging.
- Windows: code path unchanged apart from the `LookupVirtualTable` wrapper; not compiled on Windows.
