# 04: Generator carries Slot Bindings forward

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Generator). ADR-0004.

**What to build:** The generator's carry-forward mode: given the previous Gamedata, the previous build's binaries and a new build's binaries from the shared archive, it aligns each Slot Binding's class vtable between the builds and emits Gamedata with updated indices, fingerprint windows and recorded vtable sizes. A Slot Binding whose old window does not match exactly once in the new vtable is left unchanged and listed for a human. It never guesses. Output is ordinary Gamedata, reviewed as a diff.

**Blocked by:** 02

**Status:** resolved

**Tier:** 1 base · must

- [x] Replaying 1.41.9.7 → 1.42.0.1 moves GameFrame from 19 to 20 and refreshes its window
- [x] A Slot Binding with an ambiguous or missing window is listed, and its entry is left unchanged
- [x] An unchanged build produces no index changes
- [x] Signatures and non-Slot-Binding entries pass through untouched
- [x] Runs against `~/dev/cs2-builds/<build-id>/` and fails clearly when a build is missing from the archive
- [x] Uses the same fingerprint core as the boot-time Vtable Check

## Comments

- Resolved on `spec-b/04-generator-carries-slot-bindings-forward`. Offline tooling only (no runtime code touched, so no e2e run). Native doctest: 84 cases / 1069 assertions pass locally with the archive (3 skipped: the oracle and `CSSHARP_SERVER_BINARY` tests, which need env vars; with both set, 87 / 87 pass); CI mode (`CSSHARP_BUILD_ARCHIVE=/nonexistent`) 77 pass, 10 skipped. New: 7 synthetic cases in `generator_test.cpp`, 4 real-build cases in `archive_test.cpp`.
- **What landed:**
  - `generator::CarryForward(previousGamedata, previousBuild, nextBuild)` in `tooling/gamedata-generator/generator.{h,cpp}`. Builds are `VtableFingerprints` callbacks (library, class → fingerprints), so tests use fake vtables and the CLI uses the archive. Returns the new Gamedata plus `carried` (key, from, to), `held` (key, reason) and bootstrap-style `warnings`. Entries are read through `bindings::ParseGamedata`, the same reader the resolver uses. A carried entry is re-recorded through `BootstrapSlotBinding` on the next build, so its shape and warnings are the bootstrap's.
  - Carry rule (never guesses), per Slot Binding (a key with `vtable`):
    1. It needs class, library, `offsets.linux` and a linux window. Otherwise it is held ("no fingerprint window" etc.).
    2. It must verify on the **previous** build at its Gamedata index under `vtable::CheckSlot` (Window or OwnFingerprint). Otherwise it is held ("Gamedata is not for that build").
    3. The window aligned is the previous build's window at that index (`WindowAt`), not the recorded one. This refreshes a stale recorded window that still verified by its own fingerprint.
    4. That window must occur exactly once in the whole previous vtable, or it is held as ambiguous on the previous build (runs of identical bodies).
    5. It must match exactly once in the whole **next** vtable (not only ±16; the generator can afford the whole vtable). If it matches 0 times or 2+ times, it is held.
    6. Then it moves: `offsets.linux` is the new index, and `vtable.linux` (window, uniqueNearby, size) is recorded on the next build. All other fields and keys are kept, in order.
  - `SlotBindingLibraries(gamedata)`: the libraries a CLI must load. `ArchivedBuild` in `build_archive.{h,cpp}` loads modules once each. Its constructor throws "build X is not in the build archive R", and `Require` throws "build X in R has no libengine2.so or .img".
  - CLI: `gamedata-generator carry-forward --from <id> --to <id> --gamedata <file> [--archive <dir>] [--out <file>]`. Both builds and every needed module are loaded up front, so a missing one exits 2 with that message. Output is the whole Gamedata (ordered_json, dump(2)). stderr lists `moved K: a -> b`, warnings, `held K: reason` and a summary line. Exit codes: 0 when all were carried, 4 when some were held (output still written).
- **Replays (real builds, local):**
  - Committed Gamedata with GameFrame bootstrapped on 1.41.9.7 at 19, carried to 1.42.0.1: **byte-identical to the committed gamedata.json** (19 → 20, window and size 105 refreshed). Checked both in the test and with the CLI (`diff` empty).
  - 1.42.0.1 → 1.42.0.1: byte-identical, 1 unmoved.
  - The 10 ISource2Server slots in use (EXPLORE §8 table), bootstrapped on 1.41.9.7: all carry +1 (16/20/23/32/42–47), none held. Ticket 05 can bootstrap at 1.41.9.7 and carry them forward the same way.
  - Every slot of CSource2Server, CSource2GameClients, CSource2GameEntities, CGameEventManager, CEngineServer, CGameEventSystem and CCvar, 1.41.9.7 → 1.42.0.1: **0 wrong**. Every carried slot went to its own index, or +1 at and after CSource2Server's insert at 14. Held: only 8 of 104 CSource2Server slots, none of ours. Slots 13 and 14 are held because the insert changed their neighbour. Slots 59/60, 66/67 and 94/95 are held because they sit in runs of identical bodies. All other classes carried 100%.
  - CLI on 1.41.9.6 → 1.42.0.1 with the 1.42.0.1 Gamedata: GameFrame is held ("does not match the previous build at index 20"), exit 4. 1.43.0.0 (absent): exit 2, "build 1.43.0.0 is not in the build archive …".
- **Deviations / decisions:**
  - Own-fingerprint rule (decision 2) is **not** used to move or refresh a Slot Binding on the next build. If only a neighbour changed, the slot is held, although the boot check would still resolve it there. The ticket says "window matches exactly once" and "never guesses". A maintainer can re-bootstrap such an entry at its index. Rule 2 above does use the own-fingerprint rule to accept the previous build. This matches what the boot check accepted on that build, so it is not a guess about the new build.
  - Window search is over the whole vtable, both for ambiguity on the previous build and for the match on the next. Ticket 03's boot-time Heal searches ±16.
- **For the later review (unify with ticket 03):**
  - Search helpers: `WindowMatches(window, vtable[, first, last])` in `generator.cpp` (anonymous namespace) is the generator's window search, also used by the bootstrap's "also matches nearby" warning. It is kept out of `vtable_fingerprint.{h,cpp}` on purpose so it does not collide with 03's heal search. If 03 adds a core function returning all window-match indices in a range, replace `WindowMatches` with it. Keep the generator's rule "exactly once in the whole vtable, and once in the previous one".
  - CheckSlot results: `AlignSlot` treats `CheckSlot(...) == SlotMatch::None` as "does not verify on the previous build". If 03 makes `CheckSlot` return a heal result (a new `SlotMatch` value or an index), the generator must accept only Window/OwnFingerprint at the stated index there. A Gamedata entry that only heals on the previous build is not for that build and should stay held. Check this comparison when merging 03.
- Formatting: hand-formatted in the existing style. The local clang-format differs from the files' existing formatting (ticket 02's code isn't clang-format-clean either), so I did not reformat.
