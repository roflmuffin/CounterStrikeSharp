# 09: Game-class virtuals verified and healed

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Slot Bindings and the Gamedata shape). Respawn and CommitSuicide moved 15 times each.

**What to build:** Gamedata offsets that are game-class virtuals (respawn, commit suicide, change team, teleport, item services drop/remove/give, skeleton instance, find picker entity, is-player-pawn, check-transmit) gain class, module and fingerprint-window fields and become verified Slot Bindings with Heal. Native and managed callers use the verified or healed index from the Resolution Report, including plugins that read these offsets by key. Struct field offsets are untouched.

**Blocked by:** 03, 04; spec A 12 (managed helpers read from the report)

**Status:** resolved

**Tier:** 2 add-on · must

- [x] Every game-class virtual offset carries its window; struct field offsets are unchanged
- [x] Real-build test (local): over the 7 archived pairs, the known moves (e.g. ChangeTeam 102→104) heal or fail, never silently wrong
- [x] Generator replay regenerates the hand-fixed offset history correctly or flags it
- [x] Reading one of these offsets by key returns the verified or healed index
- [x] End-to-end: CommitSuicide one off → heals and works; corrupted → only commit-suicide disabled
- [x] Windows behaves as today

## Comments

- Resolved on `spec-b/09-game-class-virtuals-verified-and-healed`. Native doctest: locally with the archive, `CSSHARP_SERVER_BINARY` and `CSSHARP_FINGERPRINT_ORACLE`, 100 / 100 cases (2102 assertions); CI mode (`CSSHARP_BUILD_ARCHIVE=/nonexistent`) 86 pass, 14 skipped. Managed unit tests 177 pass. E2E `eng/e2e/cases/game-class-virtuals.sh` (new): valid, one off and corrupted, all green (see below); smoke failures only the known flaky ones.
- **What landed:**
  - Gamedata: the 11 keys (`ISource2GameEntities::CheckTransmit`, ChangeTeam, Respawn, the three item services, GetSkeletonInstance, FindPickerEntity, IsPlayerPawn, Teleport, CommitSuicide) gained `vtable` (class, `server`, linux window/uniqueNearby/size). Produced with the generator: bootstrapped on 1.41.9.7 at the known indices (plus GameFrame at 19), then `carry-forward` to 1.42.0.1; the output is byte-identical to the committed file (none of these moved at 1.42.0.1, only GameFrame 19 -> 20). Linux/Windows indices unchanged. Struct field and other plain offsets (`CTakeDamageInfo_HitGroup`, `CheckTransmitPlayerSlot`, `GameEntitySystem`, `GameEventManager`, `SetStateChanged`) untouched; a test pins this.
  - Classes as in the research map, except **IsPlayerPawn is verified on `CCSPlayerPawn`**, not `CBaseEntity`: in CBaseEntity it is the shared `xor eax,eax; ret` stub next to an identical one (never heals); in CCSPlayerPawn its body is unique, so the 1.41.8.2 move 168 -> 170 heals. The index is the same in every class of that primary chain.
  - Native: `BindingDeclaration::headerIndex` (default true: interface methods, Windows falls back to the compiled index as before). `ClassSlot(key)` declares a game-class virtual (false). `CoreBindingDeclarations` always declares the 11 keys as `ClassSlot`, even when Gamedata has no `vtable` (auto-updated upstream file): then they are unverified at their Gamedata index (decision 3). Gamedata-only keys with `vtable` are auto-declared `ClassSlot`. Catalog rows check-transmit and take-damage-hooks now use `ClassSlot`. Native `IsPawn()` and the CheckTransmit hook read `GetBindingSlot` (verified/healed index).
  - Windows (decision 9): a Slot Binding without header index is unverified at its Gamedata windows index; without one it fails `MissingForThisOs`. Interface Slot Bindings unchanged. All 11 have Windows indices, so Windows behaves as today (they now show in the "unverified" startup line).
  - Managed: `BindingDeclaration.Slot(key)`; the managed features using these keys declare `Slot`, so helpers get the healed index through `EngineFeatures.Current.GetOffset`. **`GameData.GetOffset(key)`** for an entry with `vtable` that native resolved as a Slot Binding returns the report's index (verified/healed/unverified); if the Slot Binding failed it throws `InvalidOperationException("Offset K failed its Vtable Check on this build (<reason>)")` instead of returning the stale index; other keys read JSON as before. `LoadedGameData.Vtable` is internal (`[JsonInclude]`), no public API change. `GameDataTests` joined the `EngineFeaturesApiTests` collection (installs a report).
  - Smoke test `GameClassSlotTests.GameClassVirtuals_ReadByKey_FollowTheVtableCheck` (Tests.Native) compares `GameData.GetOffset` with the report for all 11 keys and logs each index. (Do not put "Healed"/"unverified" in smoke test names: `game-frame.sh`'s valid scenario greps logs for them.)
- **Real-build replays** (`tests/native/game_class_slot_archive_test.cpp`; truth = hand-fixed Gamedata history: 1.41.7.7 fix `faa97820` for 1.41.8.1, the 1.41.8.2 fix `2f984988` for every later build):
  - Resolver, 8 consecutive server pairs 1.41.8.1 .. 1.42.0.1 (the ticket's 7 + 1.41.9.7 -> 1.42.0.1), Gamedata bootstrapped on the older build at its hand index: 80 resolved, 5 healed, 3 failed, **0 wrong**. Heals (all at 1.41.8.2): Respawn 272 -> 276, FindPickerEntity 27 -> 26, IsPlayerPawn 168 -> 170, Teleport 162 -> 164, CommitSuicide 384 -> 387. Failures (fingerprint mismatch): ChangeTeam 102 -> 104 (its own body changed too), CheckTransmit at 1.41.8.2 and at 1.41.9.7 (recompiled in place; false fails, never wrong).
  - Generator, carry-forward chained 1.41.8.1 -> ... -> 1.42.0.1: the same five moves regenerated correctly, ChangeTeam and CheckTransmit (x2) held for a human (the test re-bootstraps them at the hand index, as a maintainer would), nothing wrong; the chain ends byte-identical to the committed entries.
  - Committed entries re-bootstrap identically on 1.42.0.1 and resolve there by window, no size warnings.
  - ~80 s locally (fingerprinting the big game-class vtables of 9 builds); skipped without the archive.
- **E2E** `game-class-virtuals.sh`: valid (all resolved, no Heal/unverified, GetOffset 387/104 "verified", ChangeTeam/Respawn/CommitSuicide/Teleport/GiveNamedItem/RemoveWeapons smoke pass); one off (`offsets.linux = 386`: "all N resolved, 1 healed", native and managed Heal lines 386 -> 387, CommitSuicide and Respawn smoke pass, plugin read by key 387 "healed from 386", gamedata.json unchanged after the run); corrupted (`window[1]` zeroed: "1 of N failed: CBasePlayerPawn_CommitSuicide (fingerprint mismatch)", only commit-suicide disabled, typed exception, respawn/teleport pass, read by key throws). Regression: `game-frame.sh`, `commit-suicide.sh`, `check-transmit.sh`, `take-damage.sh` re-run with `--no-build`.
- **Deviations / notes for others:**
  - CheckTransmit (an ISource2GameEntities interface slot) is done here because this ticket lists it; ticket 10 ("game entities") should not re-add it. It is a `ClassSlot` (its hook has no compiled fallback).
  - `archive_test.cpp`'s 1.41.9.7 -> 1.42.0.1 carry test no longer asserts exactly one carried Binding: GameFrame moves 19 -> 20, every other carried Slot Binding must stay. Tickets 05–08 adding Slot Bindings fit that.
  - Behaviour change for plugins: `GameData.GetOffset` on one of these keys now throws when its Vtable Check failed (previously it returned the Gamedata number, which would call the wrong function).
