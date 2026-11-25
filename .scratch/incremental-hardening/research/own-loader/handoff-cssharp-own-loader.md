# Handoff: grill the "own loader" idea for CounterStrikeSharp, then write its spec

Repo: `/home/markus/dev/other/CounterStrikeSharp` (main @ 0485c767, clean; nothing was changed in the repo).
Date: 2026-11-25. The previous sessions were research only.

## Goal of the next session
1. Run a **grilling session** (`/grilling`) with the user on CounterStrikeSharp loading itself instead of through Metamod:Source.
2. Turn the outcome into a **spec**: scope, design, compatibility, rollout and tickets.

Research exists, but its downtime conclusion was wrong (see "Correction"), so the case is open. Spec only; don't implement unless asked.

## Correction: why "0 h measured" is void
The background research said dropping Metamod would have saved 0 h of outage, because CSSharp's fix always landed after Metamod's in all 9 Metamod load failures. **The user rejected that, and is right:**
- While Metamod won't load, nobody can tell whether CSSharp is broken. Maintainers, users and any CI only start finding CSSharp's own breakage once Metamod loads again.
- "CSSharp fixed after Metamod" is therefore exactly what serial blocking would produce. It is not evidence that Metamod was off the critical path.
- With its own loader, CSSharp could boot on the new build immediately. Its own breakage would be visible (to CI, maintainers and users) from the moment the build ships.

Per-incident numbers (measured; build → first fix):

| Update | Metamod down | CSSharp to release | CSSharp after Metamod loaded | Metamod root cause |
|---|---|---|---|---|
| Feb 2024 | 12.5 h | 29.5 h | 17.0 h | tier0 `KeyValues::GetString` re-mangled |
| Oct 2024 Armory | 22.0 h | 38.7 h | 16.7 h | tier0 `_AssertValidWritePtr` |
| Jul 2025 | 68.2 h | 146 h | 77.8 h | SDK/loader; CSSharp gamedata commit landed before Metamod's fix, so some work ran in parallel |
| Aug 2025 | 3.3 h | 4 h | 0.7 h | SDK header bump |
| Sep 2025 | 1.6 h | 31.5 h | 29.9 h | SDK header bump |
| Oct 2025 | 14.4 h | 32 h | 17.6 h | loader path (`libserver_valve`, Source2ServerConfig001) |
| Jan 2026 | 18.6 h | 35 h | 16.4 h | tier0 `UtilMemory_CalcNewAllocationCount` |
| Apr 2026 AnimGraph 2 | 10 h | 35 h | 25 h | SDK header bump |
| Jul 2026 | 11 h | 35 h | 24 h | tier0 `g_bUpdateStringTokenDatabase` |
| **Total** | **162 h** | **387 h** (of 606 h) | **225 h** | |

How much an own loader saves depends on two things:
- **Upper bound, about 162 h (27% of the 606 h outage):** applies if CSSharp's work was purely serial behind Metamod and a standalone CSSharp would not have hit the same break.
- **Shared root cause cuts into it:** 4 of the 9 were tier0 link symbols, and several were SDK header bumps. CSSharp links the same tier0 stub and compiles the same headers, so a standalone CSSharp would likely have failed to load on some of these too, unless tier0 imports go and slots become data.
- **For the grill:** settle this per incident. For each of the 9, would standalone CSSharp still have broken, and how much earlier could the fix have started?

Additional benefits not in the hours above:
- CI can boot CSSharp on day zero even when Metamod is broken.
- No dependence on Metamod's release cadence or API churn. The SourceHook→KHook move (Metamod API 17→18) forced CSSharp #1418 (`a55cb8bc`) and made dev Metamod builds mandatory.
- Two single-setup Metamod/KHook bugs (#278 Jun 2026, #288 Oct 2026) stopped some Linux servers for about 14–17 days. They're excluded from the counts.

## Baselines to compare against (model, 39 incidents)
| Scenario | Crashed | Effective outage | Broken or degraded |
|---|---|---|---|
| CSSharp today | 606 h | 635 h | 1,871 h |
| Incremental + auto-publish | 329 h | ~426 h | ~1,396 h |
| Ideal + auto-publish | 0 h | ~317 h | ~1,323 h |

- **The plain "Ideal" bar (1,689 h / 415 h) is not the baseline to use.** It assumed no signature heals itself, which made the ideal stack look worse than "incremental + auto-publish". Use the auto-publish rows: they assume the measured ~60% signature heal rate.
- **Script:** `idealpub.py` in this directory.

## What Metamod provides CSSharp today (re-verify the refs)
Full table with file:line refs: `no-metamod-findings.md` §1 (same directory as this file). In short:
- **Loading.** The `gameinfo.gi` line makes Metamod's loader pose as `libserver.so` (MM `loader/gamedll.cpp:72,107-117`).
- **Plugin entry.** `PLUGIN_EXPOSE` at `src/mm_plugin.cpp:69`, and `ISmmAPI*` at `src/core/globals.cpp:70`.
- **Hooking.** All of it goes through **KHook exported by Metamod**: `src/mm_plugin.cpp:78`, which refuses to load without it; `src/core/hooks.h`; about 150 call sites. KHook is a standalone zlib library (`research-archive/B/metamod-source/third_party/khook`). Linking it statically keeps the hooks API-compatible, so no rewrite is needed.
- **Interface factories.** `src/mm_plugin.cpp:107-120` and `natives_engine.cpp:128-135`.
- **Level lifecycle.** `IMetamodListener` at `mm_plugin.cpp:97`.
- **Console print.** `META_CONPRINT` at `natives_engine.cpp:171`.
- **Managed interop.** `Utilities.MetaFactory`, `OnMetamodAllPluginsLoaded` (`natives_metamod.cpp:25`, `Utilities.cs:288-294`, `Listeners.g.cs:248`).
- **Build.** `makefiles/shared.cmake:25,63`, `makefiles/metamod/configure_metamod.cmake`, submodule `libraries/metamod-source`.

## Precedents (code on disk under `/home/markus/dev/cs2-0downtime-mod-stack-poc/research-archive/D/`)
- **ModSharp** (`modsharp-public/Loader/loader.cpp:168,183-185,250,279-283`):
  - Proxies `CreateInterface`, hooks Source2ServerConfig slot 0 for the engine factory and Source2Server slot 3 for init, and loads the real server by explicit path.
  - Its loader needed **0 game-update fixes in about 13 months.**
- **SwiftlyS2** (`swiftlys2-loader/src/iface.cpp:30`, `hooks.cpp:58,68,73`):
  - Removed Metamod in April 2026 (`e5855628` "NUKE METAMOD").
  - Its loader broke twice, both from a byte signature for engine init (`e1c226e`, `105e87e`). It has had **0 breaks since switching to a string anchor** (`7e111c8`, Aug 2026).
- **Plugify:** dual-mode, with its own launcher or Metamod.
- **Every own loader observed still needs a `gameinfo.gi` edit.** Only a launch wrapper (LD_PRELOAD or a launcher exe) avoids it, and many hosting panels forbid that.

## Questions to grill (suggested order)
1. **Goal and success metric.** Is it outage hours, time to first detection, or independence from Metamod? Is the target the 162 h upper bound, or something else?
2. **Shared root causes.** Which of the 9 incidents would standalone CSSharp still have hit (tier0 imports, compiled slots)? Is "own loader" without "no mangled tier0 imports" worth shipping?
3. **Coexistence.** Users run Metamod plugins alongside CSSharp (CS2Fixes, MultiAddonManager, sql_mm, cs2kz). Two proxies can't both be `libserver.so`, because Metamod and ModSharp load the real server by explicit path. The choices:
   - standalone-only
   - dual-mode (detect Metamod and stay a plugin)
   - chain-load Metamod from the CSSharp proxy (unproven)
   - CSSharp hosts Metamod plugins itself

   What happens to plugins that call `MetaFactory`?
4. **KHook ownership.** Static KHook versus Metamod's instance. Two KHook instances must never patch the same target.
5. **Detection and CI.** With own loading, CI can boot each new build immediately. Is this the real win, and should the spec pair the loader with the T1 CI job (I7)?
6. **Loader robustness design.** Which slots and anchors are used (only IAppSystem-style slots that never moved, string anchors, no byte signatures)? What is the self-test, and how does it fail?
7. **Install and `gameinfo.gi`.** Is there an auto-repair or checker, how are hosting panels handled (they ship Metamod plus CSSharp by default), and is there one package or two?
8. **Windows parity.** All binary analysis so far is Linux-only.
9. **Migration and rollout.** Does it start opt-in, how do users switch, and how do we roll back?
10. **Interaction with T1** (`handoff-cssharp-incremental-hardening.md`). Ordering, and shared pieces: host seam, capability flags, boot self-test.

## Spec outline expected from the session
- Problem and goal, with the success metric and baseline numbers above.
- Non-goals.
- Architecture: proxy loader, host abstraction (`IHost`: factories, level listeners, console, detour provider), KHook linkage.
- Compatibility matrix: standalone, with Metamod, with other Metamod plugins, and managed API surface.
- Failure modes and self-test.
- Install, packaging and `gameinfo.gi`.
- CI and validation plan, including live tests on Linux and Windows.
- Rollout and rollback.
- Tickets.

## Draft tickets (from the research; re-tier after grilling)
Effort uses the user's convention: S/M/L/XL plus risk. Never time or headcount. Tier into base/must and later add-ons.

**Base (must):**
1. Host seam behind `mm_plugin.cpp`. M, low.
2. Static KHook option that prefers Metamod's instance when present. M, med: needs a live test alongside other Metamod plugins.
3. Minimal proxy loader, ModSharp-style: `CreateInterface` passthrough, slot 0 engine factory, explicit-path load, string-anchored init, no byte signatures. M, med: live validation on both OSes.
4. Own level-lifecycle hooks. S, med.
5. `MetaFactory` fallback (null plus a capability flag). S, low.
6. CI that boots standalone CSSharp on each new build. M, med.

**Later add-ons:**
7. Coexistence: chain-load or host Metamod plugins. M–L, high: unproven.
8. Packaging plus a `gameinfo.gi` checker/repair. S, low, but needs a human step with hosting panels.
9. Pinned known-good Metamod for the Metamod-mode package. S, low.
10. Remove mangled tier0 imports (`dlsym` plus fallbacks). L, high. This is what makes standalone survive the tier0-class incidents.

## Sources
- **Report (live artifact):** https://claude.ai/artifact/CkhPT5n5hbWJyW3SZcy7y4. See "Backtest" (now with reference bars: CSSharp today, Metamod, s2script, SwiftlyS2, and "Incremental + auto-publish") and "Every stack compared". Its local source is volatile; use `Artifact read` to fetch it.
- **Research findings:** `no-metamod-findings.md` (this directory). Its §3 "0 h measured" conclusion is superseded by the correction above.
- **Models:** `autopub.py` (auto-publish scenario, run `python3 -I autopub.py <archive> 0.6`) and `coreheal.py` (this directory).
- **Raw evidence (durable):** `/home/markus/dev/cs2-0downtime-mod-stack-poc/research-archive/`:
  - `G1/model.py`, `G1/fix_tags.txt`: incident backtest
  - `H/degrade.py`: degraded hours
  - `B/metamod-source`, `B/mm_issue_details.txt`: Metamod history
  - `D/*`: loaders
- **Related handoff:** `handoff-cssharp-incremental-hardening.md` (T1 package, I1–I7).
- **Memory:** `~/.claude/projects/-home-markus-dev-other-CounterStrikeSharp/memory/`. It holds report-style and ticket-tiering preferences.
