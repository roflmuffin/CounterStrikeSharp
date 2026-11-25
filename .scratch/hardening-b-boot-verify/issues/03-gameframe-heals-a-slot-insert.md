# 03: GameFrame heals a slot insert

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Vtable Check and Heal, Resolver seam). ADR-0003.

**What to build:** When GameFrame's fingerprint window does not match at its stated index but matches exactly once within ±16 slots, the Binding resolves at the new index as a Heal and GameFrame is hooked there. The Resolution Report carries the healed status with its evidence (old index, new index, matched window); healed Engine Features count as available; the startup report lists every Heal; plugins see the Heal through the query API. Ambiguous windows and shifts across identical bodies fail instead of healing.

**Blocked by:** 02; spec A 05 (report reaches plugins)

**Status:** resolved

**Tier:** 1 base · must

- [x] Resolver unit tests (synthetic): shift by k within range heals with evidence; window matching twice fails as ambiguous; shift across identical bodies fails; shift beyond ±16 fails
- [x] Real-build test (local, skipped without archive): GameFrame's Slot Binding from 1.41.9.7 resolved against 1.42.0.1 heals at +1
- [x] Managed unit tests: a hand-built report with a healed Binding shows the feature available, the Heal in its status, and the Heal in the startup report
- [x] End-to-end: Gamedata with GameFrame one index off → server heals it, timers fire, startup report shows the Heal
- [x] Healed indices are never written back to Gamedata at runtime
- [x] Windows behaves as today

## Comments

- Resolved on `spec-b/03-gameframe-heals-a-slot-insert`. Native doctest 80 cases pass in CI mode (3 skipped: local fixtures); locally with the archive, `CSSHARP_SERVER_BINARY` and `CSSHARP_FINGERPRINT_ORACLE`, 83 / 83 (916 assertions). Managed unit tests 172 pass. E2E `eng/e2e/cases/game-frame.sh`, all three scenarios green: valid (no Heal, no unverified, no size warning), healed (below), corrupted (unchanged). Smoke 203 passed / 3 failed / 4 skipped in both the valid and the healed run; the failures are known flaky ones (`RealConVarTests.CreatedConVarIsAccessibleFromServerConsole`, `GameEventTests.CanRegisterAndDeregisterEventHandlers`, `Listener_ConVarChanged`, `NativeObjectsTests.EnsureNativeHandle_IsFreed_Vector3`, once `ConVarTests.BoolConVar`), none related to game-frame.
- **What landed:**
  - Fingerprint core (`src/core/vtable_fingerprint.{h,cpp}`): `SlotMatch::Heal`, `WindowMatchesNear(window, live, index)` (whole-window matches within ±16, index excluded) and `LocateSlot(recorded, live)` → `SlotLocation { match, index, windowMatches }`. `CheckSlot` is unchanged (still the at-index check) so ticket 04 can keep using it; `LocateSlot` runs it first, then the heal search. A recorded window whose own fingerprint equals a recorded neighbour's (identical bodies) never heals.
  - Resolver: `BindingStatus::Healed` (counts as resolved; `offset` is the new index, `slot->statedIndex` the old one), `FailureReason::AmbiguousHeal` ("ambiguous heal (N matches)", `matchCount` = window positions), `SlotEvidence::window` (the recorded window; JSON `slot.window`, null entries outside the vtable). Identical bodies and zero matches fail as `FingerprintMismatch` (the spec's closed set: mismatch or ambiguous heal). `FormatBindingSummary` adds ", N healed"; new `FormatHeals` lines ("Healed KEY: CLASS (LIB) slot OLD -> NEW, window [fp, fp, fp] matched once within 16 slots"), logged as warnings at boot next to the size warnings.
  - Managed: `BindingFailureReason.AmbiguousHeal = 10`; public `SlotHeal` (Binding, ClassName, Library, OldIndex, NewIndex, Window) and `EngineFeatureStatus.Heals`; an available healed status describes itself as "available, healed (KEY: slot 19 -> 20)", so `ToString()` and `css_engine_features` show it. The startup report lists every Heal in the native wording, before the size warnings.
  - Tests: synthetic heal tests in `tests/native/slot_binding_test.cpp` (shift +1/+3/+16/−1/−16 heals with evidence, ambiguous, identical bodies, beyond ±16, summary/Heal lines/JSON). `archive_test.cpp`: the 1.41.9.7 bootstrap (index 19, size 104) heals at +1 on 1.42.0.1, landing on the committed index 20; the committed 1.42.0.1 Binding now heals at −1 on 1.41.9.7 (it failed before). E2E scenario "healed": `offsets.linux = 19`, `size = 104` (the real 1.41.9.7 entry); checks the native and managed Heal lines, the plugin-visible status via `css_engine_features`, the GameFrame/timer/next-frame smoke tests, and that `gamedata.json` after the run is byte-identical to the installed one (`run.sh` now saves `gamedata.after-run.json`).
- **Never written back:** the resolver takes Gamedata by const reference and the Heal lives only in the report; the e2e `cmp` covers the runtime end to end.
- **Windows:** unchanged; the Windows branch returns Unverified before any check or heal (existing synthetic test).
- **For later tickets:** hooks (`HookSlot`) and `GetBindingSlot` read `offset`, so every Slot Binding hooked that way follows a Heal with no extra code. Ticket 10 (compiled calls on never-moved interfaces) must compare the verified index with the compiled one and disable on a Heal. The spec's "all 10 ISource2Server Slot Bindings heal at +1" and the 7-pair zero-wrong replay need ticket 05's Bindings; the archive test pattern here extends to them.
