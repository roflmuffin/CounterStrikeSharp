# 05: Remaining ISource2Server hooks and calls

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Hooks and calls through Slot Bindings, New Engine Features).

**What to build:** The other six ISource2Server hooks (hibernation update, Steam API activated/deactivated, hostname changed, pre-fatal shutdown, update when not in game, pre-world update) and its two direct calls become verified Slot Bindings, hooked and called by verified index, each backing its Engine Feature (`cssharp.server/hibernation-events`, `steam-api-events`, `hostname-changed`, `fatal-shutdown`, `update-when-not-in-game`, `pre-world-update`, plus the Engine Features of the direct calls). After this ticket a 1.42.0.1-style insert heals every ISource2Server slot we use.

**Blocked by:** 03, 04

**Status:** resolved

**Tier:** 2 add-on · must

- [x] Slot Bindings bootstrapped on 1.41.9.7 and carried forward by the generator
- [x] Real-build test (local): all 10 ISource2Server Slot Bindings heal at +1 from 1.41.9.7 to 1.42.0.1, zero wrong
- [x] Each new Engine Feature appears in the startup report and fails independently
- [x] End-to-end: all ISource2Server Slot Bindings one index off → all heal, their listeners fire
- [x] Windows behaves as today

## Comments

- Resolved on `spec-b/05-remaining-isource2server-hooks-and-calls`. Native doctest: 98 cases / 1616 assertions pass locally with the archive (3 skipped: oracle and `CSSHARP_SERVER_BINARY`); CI mode (`CSSHARP_BUILD_ARCHIVE=/nonexistent`) 88 pass, 13 skipped. Managed unit tests 193 pass. E2E `eng/e2e/cases/isource2server.sh`, all three scenarios green (valid, healed, corrupted); smoke failures outside the new tests were the known flaky ones (`RealConVarTests.CreatedConVarIsAccessibleFromServerConsole`, `CommandTests.*ChatTrigger`/`IssueServerCommand`, which also fail in runs from before this branch). Server exit 139 on quit as before.
- **What landed:**
  - Gamedata: 9 new Slot Bindings after `ISource2Server::GameFrame` (one contiguous block): `ISource2Server::{PreWorldUpdate, ServerHibernationUpdate, IsPaused, GameServerSteamAPIActivated, GameServerSteamAPIDeactivated, OnHostNameChanged, PreFatalShutdown, UpdateWhenNotInGame, GetEconItemSystem}`. Produced by the generator: `bootstrap` on 1.41.9.7 at the pinned SDK indices (15/22/31/41–46, GameFrame 19), then `carry-forward 1.41.9.7 -> 1.42.0.1`: all 10 moved +1, none held, GameFrame byte-identical to the committed entry. Linux only (no `offsets.windows`).
  - Catalog (`engine_features.{h,cpp}`, one block): `cssharp.server/{hibernation-events, steam-api-events, hostname-changed, fatal-shutdown, update-when-not-in-game, pre-world-update}` plus, for the two direct calls, `cssharp.server/econ-item-system` (GetEconItemSystem) and `cssharp.server/is-paused` (IsPaused). Managed enum regenerated (45 Engine Features).
  - Hooks: `ServerManager::OnAllInitialized` registers each hook with `HookSlot`, gated by `ShouldRegisterHooks` on its own feature; callbacks are still created unconditionally.
  - Calls: new `src/core/slot_calls.h` `CallSlot(std::optional<int> index, &Iface::Method, instance, args...)` (const and non-const). It calls through the vtable at the given index. On Windows with no index it makes the compiled call; on Linux with no index it returns `R{}`. `ServerManager::GetEconItemSystem/IsPaused` cache `GetBindingSlot` in function statics. The natives `GET_ECON_ITEM_SYSTEM` and `IS_SERVER_PAUSED` throw `DescribeUnavailable` ("Engine Feature cssharp.server/is-paused is disabled (ISource2Server::IsPaused: fingerprint mismatch)") when their feature is disabled.
  - Managed: `ListenerEngineFeatures` maps the 7 ISource2Server listeners. Ticket 02's open item: new `FrameTaskEngineFeatures` (NextFrame/RunOnTick -> game-frame, NextWorldUpdate -> pre-world-update) is called from every `Server.NextFrame*/NextWorldUpdate*/RunOnTick*`. These queue as before and warn once per process ("NextFrame task registered on ... ; it will never fire"). `AddTickTimer` warns like `AddTimer` ("Timer", once per plugin).
- **Tests:** `tests/native/isource2server_test.cpp` covers the catalog rows, a synthetic CSource2Server where all 10 resolve, an insert at 14 that heals all 10 with every feature available, a corrupted key that disables only its own feature (all 10 keys), and `CallSlot` by index. `archive_test.cpp` (real builds): all 10 bootstrapped on 1.41.9.7 heal at +1 on 1.42.0.1 to the committed indices ("all 10 resolved, 10 healed"), zero wrong; the committed 10 resolve on 1.42.0.1 by window. The carry-forward replay now bootstraps all 10 on 1.41.9.7 and still reproduces the committed Gamedata. It asserts per key (ISource2Server +1, others unmoved) instead of `carried.size() == 1`, so tickets 06–10 can add Bindings. Managed `ISource2ServerEngineFeatureTests` cover per-key independence in the startup report, listener warn-once and frame-task warn-once. Smoke `ISource2ServerTests` (12) check all 8 features available, that the PreWorldUpdate and hostname listeners fire (`hostname "<guid>"` does reach `OnHostNameChanged`), that `IsServerPaused` returns false and that `GetEconItemSystem` returns consistently.
- **E2E scenarios:** valid (all resolved, no Heal, 12/12 pass, frames, timers and NextWorldUpdate run). healed (all 10 `offsets.linux -= 1`, `size = 104`: "all N resolved, 10 healed", one Heal line per key, 12/12 pass, Gamedata after run byte-identical). corrupted (OnHostNameChanged and IsPaused own fingerprints: exactly those 2 Bindings fail and exactly hostname-changed and is-paused are disabled; exactly the 4 dependent smoke tests fail, the IsPaused one with the disabled-feature message; frames still run).
- **Deviations / notes:**
  - Hibernation, Steam API (the e2e server runs `sv_lan 1`, Steam never activates), PreFatalShutdown and UpdateWhenNotInGame cannot be triggered on the test server. For these, "listeners fire" is only covered through availability and the Heal. Their hook code path is the same `HookSlot` as the PreWorldUpdate and hostname hooks, which are exercised.
  - `GetEconItemSystem` is `xor eax,eax; ret` on these dedicated builds (it returns null). The smoke test therefore checks only that the call returns consistently. Its fingerprint is shared with slots 21/24, which are outside ±16 of 47, so `uniqueNearby` is true and it heals.
  - Plugin hot reload and file-deletion unload run via `Server.NextWorldUpdate`. On a disabled pre-world-update they never happen; they now warn once instead of failing silently.
  - For the final review: `CallSlot` (`slot_calls.h`) is the generic direct-call helper; tickets 07/08 (IVEngineServer2, ICvar) likely need the same and may have added their own. Unify there.

