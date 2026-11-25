# Could CounterStrikeSharp drop Metamod:Source?

Archive root used below: `AR=/home/markus/dev/cs2-0downtime-mod-stack-poc/research-archive`. CSSharp = `/home/markus/dev/other/CounterStrikeSharp`.

## Verdict

- **Plausible:** yes. Two active stacks already run without Metamod: ModSharp and SwiftlyS2, which removed Metamod in April 2026 (`AR/D/swiftlys2` commit `e5855628` "NUKE METAMOD"). CSSharp's Metamod surface is small and concentrated in `src/mm_plugin.cpp`.
- **Downtime lever:** not on its own. In all 9 Metamod load failures in the backtest window, CSSharp's own fix landed after Metamod's. So a standalone loader removes **0 h measured** of the 606 h outage (upper bound 162 h if Metamod had been purely serial blocking, which the timestamps contradict).
- **Real win:** that comes from removing CSSharp's own tier0 imports and header-compiled slots (the "ideal" bundle) and from auto-publishing gamedata. A loader is one small piece of the ideal bundle, not a lever by itself.
- **Recommendation:** stay on Metamod for now (option c+). Add a thin host-abstraction seam, and the option to link KHook statically, so that dual-mode (option b) becomes cheap later. Spend the effort on incremental hardening (T1) plus auto-publish.

## 1. What Metamod provides CounterStrikeSharp today

| Service | Where | Replacement needed standalone |
|---|---|---|
| Loading into the server process (gameinfo.gi `Game csgo/addons/metamod`, loader poses as `libserver.so`) | MM `loader/gamedll.cpp:72,107-117` | Own proxy `libserver.so`/`server.dll` (ModSharp/SwiftlyS2 pattern) |
| Plugin entry, `ISmmAPI*` | CSSharp `src/mm_plugin.cpp:69` (`PLUGIN_EXPOSE`), `src/core/globals.cpp:70` | Own exported `StartCore`-style entry |
| **Hooking engine (KHook)**: all CSSharp hooks go through the KHook instance exported by Metamod; refuses to load without it | `src/mm_plugin.cpp:78`; MM `core/ISmmPlugin.h:494,516` (`GetDetourInterface`); `src/core/hooks.h` wraps `KHook::Virtual/Function` | Statically link KHook (standalone lib, zlib licence, `AR/B/metamod-source/third_party/khook`) |
| Engine/server interface factories | `src/mm_plugin.cpp:107-120` (`GET_V_IFACE_*` via `GetEngineFactory/GetServerFactory`); `src/scripting/natives/natives_engine.cpp:128-135` | Factories captured by the loader (`Source2ServerConfig::Connect` slot 0 gives the engine factory; `dlopen` of the real server gives `CreateInterface`) |
| Level lifecycle (`IMetamodListener::OnLevelInit/OnLevelShutdown`) | `src/mm_plugin.cpp:97` (`AddListener`), OnMapEnd built on it | Own hooks (ModSharp hooks `Source2Server` init slot 3: `AR/D/modsharp-public/Loader/loader.cpp:250`) |
| Console print | `natives_engine.cpp:171` (`META_CONPRINT`) | tier0 `Msg`/`ConMsg` (C export) |
| Inter-plugin interop exposed to the managed API: `Utilities.MetaFactory`, `OnMetamodAllPluginsLoaded` | `src/scripting/natives/natives_metamod.cpp:25`, `managed/CounterStrikeSharp.API/Utilities.cs:288-294`, `Listeners.g.cs:248` | Only meaningful with Metamod present; must keep it in Metamod mode |
| ConVar/ConCommand registration | `src/mm_plugin.cpp:206` uses the SDK's `ConVar_Register`, not Metamod | Nothing |

Build ties: `makefiles/shared.cmake:25,63` (Metamod include dirs, KHook headers), `makefiles/metamod/configure_metamod.cmake` (metaplugin `.vdf`), submodule `libraries/metamod-source`.

Hook surface on KHook: about 150 call sites across `player_manager`, `server_manager`, `dynamic_hook`, `entity_manager`, `voice_manager` and others. These are API-compatible if KHook is linked statically, so there is no rewrite.

## 2. Options

| Option | What it is | Precedent | Main blocker |
|---|---|---|---|
| a. Standalone only | Own proxy loader; Metamod dropped | ModSharp (`Loader/loader.cpp:168,183-185,279-283`: proxies `CreateInterface`, hooks Source2ServerConfig slot 0, loads real `libserver.so` by explicit path); SwiftlyS2 loader (`AR/D/swiftlys2-loader/src/iface.cpp:30`, `hooks.cpp:58,68,73`) | Breaks interop with Metamod plugins users run alongside (CS2Fixes, MultiAddonManager, sql_mm, cs2kz, …) and with the CSSharp plugins that call `MetaFactory` |
| b. Dual-mode | Load as a Metamod plugin when Metamod is present, otherwise through its own proxy | Plugify (own launcher or Metamod), per earlier census | Two proxies cannot both be "the" `libserver.so`. Metamod and ModSharp load the real server by explicit path (`gamedll.cpp:72`; ModSharp `loader.cpp:281`), so the second proxy in gameinfo.gi is skipped. Chain-loading is unproven |
| c. Stay on Metamod, decouple | Keep Metamod; add a host seam; allow static KHook; ship/pin a known-good Metamod build | — | None technical |

**gameinfo.gi is still needed by every own loader observed.**
- ModSharp's docs say "same as installing Metamod, except the path…" (`AR/D/modsharp-public/docfx/docs/en-us/guides/getting-started.md:53,89`).
- SwiftlyS2's loader is also a `libserver.so` placed under addons.
- So the "every update overwrites gameinfo.gi" problem does not go away. Only a launch wrapper (LD_PRELOAD, or a launcher exe) avoids it, and many hosting panels won't allow that.

## 3. Downtime effect (backtest, Nov 2023–Nov 2026)

### Metamod failures against CounterStrikeSharp's own outage

Measured from `AR/B/metamod-source` git, `AR/G1/model.py`, `AR/G1/fix_tags.txt`:

| Update | Metamod down (build → fix) | CSSharp incident, hours to release | CSSharp fix landed vs Metamod fix |
|---|---|---|---|
| Feb 2024 | 12.5 h (`f950db7`) | #6, 29.5 h | after (`f2b8044b` Feb 8 06:03Z) |
| Oct 2024 Armory | 22.0 h (`557121c`) | #14, 38.7 h | after (`2c7f8961`) |
| Jul 2025 | 68.2 h (`de48d45`) | #20, 146 h | gamedata before, code fix `1f30e561` 3 days after |
| Aug 2025 | 3.3 h (`c1cc0a6`) | #21, 4 h | about 40 min after (`f7c69a60`; plausibly gated by the shared SDK fix) |
| Sep 2025 | 1.6 h (`3f3136d`) | #24, 31.5 h | after |
| Oct 2025 | 14.4 h (`4399ff0`) | #25, 32 h | after (bumped the Metamod submodule `7572722d`) |
| Jan 2026 | 18.6 h (`11f7b87`) | #29, 35 h | after |
| Apr 2026 AG2 | 10 h (`ec7ff82`) | #32, 35 h | after |
| Jul 2026 | 11 h (`26c03fa`) | #33, 35 h | after |
| **Total** | **162 h** | 387 h of the 606 h | Metamod never the long pole |

- **Measured: 0 h** of CSSharp outage would have been removed by dropping Metamod alone. At most about 0.7 h (Aug 2025).
- **Why (inferred):** Metamod's breaks share root causes with CSSharp's own. Four were tier0 link symbols (KeyValues::GetString, _AssertValidWritePtr, UtilMemory_CalcNewAllocationCount, g_bUpdateStringTokenDatabase), and others were loader-path or SDK header changes. CSSharp links the same SDK and tier0, so a standalone CSSharp binary hits the same breaks until it drops mangled tier0 imports and compiled slots.
- **Not counted (inferred):** two single-setup Metamod/KHook bugs that stopped servers for about 14–17 days on some Linux hosts (#278 Jun 2026, #288 Oct 2026 hang with zero plugins). Standalone would avoid these, but so would pinning a known-good Metamod.

### New risk from owning a loader

Measured from git history:
- **ModSharp loader:** 0 game-update fixes in about 13 months (Oct 2025 – Nov 2026, through AG2 and July). It hooks only IAppSystem-style slots 0 and 3, which have never moved.
- **SwiftlyS2 loader:** 2 breaks in about 13 months (`e1c226e` Jan 2026 Windows sig, `105e87e` Jul 2026 Linux sig). Both came from a byte signature for engine init. It switched to a string anchor on Aug 8 2026 (`7e111c8`, now `hooks.cpp:58`) and has had 0 breaks since.
- **Metamod's own loader:** broke in Oct 2025 (Source2ServerConfig001 / `libserver_valve` path) and Jun 2026 (#278).
- **Net (inferred):** a ModSharp-style minimal loader is at least as stable as Metamod's. Risk is roughly neutral or slightly lower, not a new liability.

### Where standalone does help (inferred, unmeasured)

- **Fewer moving parts.** No Metamod API churn. The SourceHook → KHook move (Metamod API 17→18) forced CSSharp's #1418 rewrite (`a55cb8bc`, Sep 2026) and made dev Metamod builds mandatory. That cost is maintenance, not downtime.
- **Control over load timing.** Resolving and self-testing before the first map, and a single place for capability flags. T1 can do most of this as a Metamod plugin too.
- **Prerequisite for the ideal stack's tier0/loader bucket** ("own loader, dlsym, no static SDK libs"). Feb 2024, Armory, Jul 2025 and Jul 2026 in the T1-remaining outage need that whole bundle, not the loader alone.

## 4. Is it a good idea?

Pros:
- It's the path the fastest-recovering peers took.
- It removes an external release dependency and Metamod API churn, and sets up the ideal stack.

Cons:
- It breaks Metamod-plugin interop and the `MetaFactory` API unless dual-mode works.
- Hosting panels ship Metamod plus CSSharp by default.
- Two install paths double the support surface.
- gameinfo.gi editing remains.
- Measured downtime benefit is about 0 h.

**Recommendation:**
1. **Now:** option c+. Add the host seam and statically linkable KHook (both cheap and low-risk), keep shipping as a Metamod plugin, and put the effort into T1 + auto-publish (−25% broken time in the model) and tier0/slot decoupling.
2. **Later:** dual-mode (b), once tier0 imports and compiled slots are gone. At that point a standalone loader becomes the last piece that lets a CSSharp build survive loader- and link-class updates without anyone waiting on SDK or Metamod.

## 5. Tickets

**Base (must) — option c+, keeps the door open:**
1. Host-abstraction seam (`IHost`: factories, level listeners, console print, detour provider) behind `mm_plugin.cpp`. Size M, risk low.
2. Build option to link KHook (+SafetyHook) statically; at runtime prefer Metamod's exported KHook when present. Size M, risk med: two KHook instances must never patch the same target, so it needs a live test with other Metamod plugins.
3. Pin and ship a known-good Metamod build in the CSSharp "with-Metamod" package, plus a boot check that reports the Metamod build. Size S, risk low.

**Later add-ons — dual-mode (b):**
4. Minimal proxy loader, ModSharp-style: `CreateInterface` passthrough, Source2ServerConfig slot 0 for the engine factory, explicit-path load of the real server, string-anchored init. Size M, risk med: needs live validation on Windows and Linux.
5. Own level-lifecycle hooks replacing `IMetamodListener`. Size S, risk med.
6. Coexistence: detect Metamod and stay a Metamod plugin, or chain-load Metamod's loader from the CSSharp proxy. Size M, risk high: unproven, Metamod assumes it is the first proxy, needs live tests with CS2Fixes and MultiAddonManager.
7. `MetaFactory` / `OnMetamodAllPluginsLoaded` in standalone mode: return null plus a capability flag, with docs. Size S, risk low.
8. Packaging and install: two variants, gameinfo.gi checker/repair script for panels. Size S, risk low, but needs a human step with hosting panels.
9. CI: boot both modes on each new build. Size M, risk med.
10. (ideal-stack prerequisite) Remove mangled tier0 imports (`dlsym` + fallbacks). Size L, risk high. This is what actually moves the loader/link outage hours.
