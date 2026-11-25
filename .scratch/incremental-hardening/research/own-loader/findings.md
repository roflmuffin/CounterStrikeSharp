# Own loader: research findings (spec D grilling, 2026-11-25)

Three research passes run during the spec D grilling. M = measured (read in source, git or binaries); I = inferred, not tested live. All Linux unless stated. `AR` = `~/dev/cs2-0downtime-mod-stack-poc/research-archive`; Metamod refs are `AR/B/metamod-source @c5cad5a`.

Inputs copied here: `handoff-cssharp-own-loader.md` (the session brief) and `no-metamod-findings.md` (earlier research; its §3 "0 h measured" argument is void, see below, but its §1 surface table still holds).

## 1. Would a standalone CounterStrikeSharp have survived Metamod's 9 load failures?

**Method:** the undefined imports (`nm -D`) of pre-incident and fix release binaries of both projects, intersected with the exports removed between SDK `libtier0.so` snapshots. Both projects are loaded with `dlopen(RTLD_NOW)`, so a missing import fails the load.

**Why fix ordering proves nothing:** while Metamod won't load, nobody can see whether CounterStrikeSharp is broken too. So "CounterStrikeSharp's fix landed after Metamod's" is not evidence that Metamod was off the critical path.

**Columns:** S1 = own loader, tier0 linked as today. S2 = S1 plus `dlsym` tier0 plus slots as data.

| Incident (Metamod down) | Same root cause for standalone CounterStrikeSharp? | S1 h saved | S2 h saved (working / "loads degraded") |
|---|---|---|---|
| Feb 2024 (12.5 h) | **No (M).** Metamod 1279 imported the re-mangled `KeyValues::GetString`; CounterStrikeSharp v161 imports none of the 27 removed exports | ≤12.5 (only if work was serial) | 12.5 / 12.5 |
| Oct 2024 Armory (22.0 h) | **Yes (M+I).** Both import `_AssertValidWritePtr` | ~0. PR #584 opened 8.5 h before Metamod's fix (work ran in parallel) | ~0 / 22 |
| Jul 2025 (68.2 h) | **Yes (M).** v1.0.325 imports `UtlMemory_*`, `CUtlBuffer`, `CRawAllocator` and others, all removed | ~0.7 | ~0 (ICvar struct changes) / 68.2 |
| Aug 2025 (3.3 h) | **Yes (M).** Fix `f7c69a60` is only the SDK bump | 0 | ~1 / 3.3 |
| Sep 2025 (1.6 h) | **Yes (I).** Same SDK range, plus IGameSystem | 0 | ~1.6 / 1.6 |
| Oct 2025 (14.4 h) | **Yes, for the code part (I).** Metamod 1367 is a rebuild on the IGameSystem/ICvar SDK. The "libserver_valve / loader path" log came from setups launching without `LD_LIBRARY_PATH`, not from Metamod's fix | ~0 | ~0 / 14.4 |
| Jan 2026 (18.6 h) | **Yes (I), but not tier0 (M).** SDK headers (ConVarValueInfo_t, CSchemaSystem, IGameSystem); the brief's `UtilMemory_*` label was wrong | 0 | ~0 / 18.6 |
| Apr 2026 AG2 (10 h) | **Yes, and worse (M+I).** CounterStrikeSharp alone also imported `ThreadAtomicNotifyOne(Pj)`, which became `PKj` | 0 | ~0 / 10 |
| Jul 2026 (11 h) | **Yes (M).** Both import `g_bUpdateStringTokenDatabase` | ~0.2 | ~0 / 11 |

**Totals:**
- **S1 ≈ 0–13 h of 162 h (≤8%).** S2 ≈ 15 h to a fully working core, up to ≈160 h if "loads, some features off" counts and struct mismatches don't crash.
- **The SDK merge was the long pole** in most incidents; no loader changes that. In Apr and Jul 2026 the SDK fixes were authored weeks earlier and merged on update day.
- **Parallel work while Metamod was down is measured** in 4 incidents: Armory PR #584, Jul 2025 `b7abd1d5`, Oct 2025 #1064, Jan 2026 `f9617b9a`.
- **Not counted:** Metamod-only bugs #278 (Jun 2026) and #288 (Oct 2026) stopped some Linux servers for about 14–17 days each. A proxy loaded by Metamod (ordering A, §2) survives a broken Metamod core.

## 2. Metamod loader mechanics and coexistence options

**How Metamod's loader picks the game DLL (M):**
- It walks every `Game`/`GameBin` search path in `gameinfo.gi` and builds `<entry>/bin/linuxsteamrt64/libserver.so` (Windows: `/win64/server.dll`) for each (`loader/gamedll.cpp:82-216`).
- It skips only its own path and dlopens the candidates in order, taking the first whose `CreateInterface` returns non-NULL (`:676-695`).
- So a CounterStrikeSharp `libserver.so` at `csgo/addons/counterstrikesharp/bin/linuxsteamrt64/`, in a Game path listed **after** Metamod's, becomes Metamod's game DLL.

**Graceful fallback (M):** if the Metamod core fails, the loader logs fatal, sets its bridge to NULL and still calls the original Init (`:322-402`). The server then runs without Metamod plugins and forwards everything to its game DLL. So in ordering A, a broken Metamod core leaves the CounterStrikeSharp proxy running.

**Metamod's own hooks and assumptions:**
- It vtable-swaps `ISource2ServerConfig::Connect` (slot 0) and `ISource2Server::Init`, both self-restoring, and `Disconnect` permanently (`:517-647`).
- It asserts that its saved originals are NULL/non-NULL. Layers that each restore their own saved original stack LIFO (I).
- `GetGameBinaryPath()` returns the proxy's path in ordering A. CS2Fixes and cs2kz don't call it (M).

**`server_valve` (M disasm + reports):**
- libengine2 tries `server_valve` before `server` (gate flags undecoded).
- Renaming Metamod's loader to `libserver_valve.so` worked around #278, and Plugify's "autoload" mode uses the same name.
- It is undocumented Valve behaviour.

**Precedents (M):**
- **ModSharp:** no Metamod coexistence. Hard-coded `../../csgo/bin/...` relative to the working directory, and vtable hooks on slots 0/3/5. 0 loader fixes in about 13 months.
- **SwiftlyS2:** dual-mode until `e5855628` "NUKE METAMOD", with the entry point chosen by which export was called and no double-load guard. Now loader-only with SafetyHook inline detours. Two byte-signature breaks, then a string anchor (`7e111c8`), 0 breaks since. tier0 is resolved dynamically.
- **Plugify:** two separate host builds (a Metamod plugin, and a launcher / `server_valve`). Module enumeration filters `/metamod/` paths.

**Coexistence options:**

| Option | Feasibility | Effort / risk | Main risk |
|---|---|---|---|
| Standalone only | High | M, low-med | Users lose their Metamod plugins |
| Dual-mode (Metamod plugin when present, else proxy) | High | L, med | Still dies when Metamod dies |
| **A. Metamod first, CounterStrikeSharp proxy as Metamod's game DLL** | High (I) | S-M, med | gameinfo order; slot-restore order |
| B. CounterStrikeSharp proxy chain-loads Metamod | Med-high (I) | M-L, med-high | Re-entry from Metamod's walk; `server_valve` |
| Host Metamod plugins (ISmmAPI shim) | Possible | XL, high | Reimplements the Metamod core |

## 3. KHook with two instances

- **State is per binary (M):** `static GlobalState* g_globals`, one worker thread, own ranges, no cross-instance registry (`third_party/khook/src/detour.cpp:1546-1603`).
- **Virtual hooks are a plain vtable-entry swap (M):** the current entry becomes "original" (`detour.hpp:96-103`). Function hooks are SafetyHook inline detours. Capsules never restore slots and are only freed by `KHook::Shutdown`.
- **Two instances on one target (I):**
  - They chain, and the later-installed one is outer.
  - `FindOriginal*` in the outer instance returns the inner instance's JIT.
  - Each instance's signature scan sees the other's inline patches.
  - Freeing either instance's JIT memory while the other chains through it crashes.
- **Symbols:** standalone KHook has default visibility (`khook.hpp:21-32`). Build it hidden with `--exclude-libs,ALL` (SwiftlyS2 `bd4c47a` "Fix elf hijacking").
- **Plugin builds today:** they use the forwarding header, so every call goes through `KHook::__exported__khook`, which Metamod sets per plugin (`ISmmPlugin.h:514-518`). Whether a static standalone build can supply an instance for that same pointer is **unverified** (spec D ticket 02).

## 4. MetaFactory and Metamod interop in ordering A

- **How `MetaFactory` resolves (M):** it asks only each plugin listener's `OnMetamodQuery`, plus `IPluginManager` (`core/metamod.cpp:802-852`). Metamod's exported `CreateInterface` reaches only `OnGameDLLQuery`, which no real plugin implements. The Metamod core exports no `ISmmAPI` symbol (`-fvisibility=hidden`).
- **The proxy cannot reach Metamod plugin interfaces on its own (M):**
  - MultiAddonManager003
  - sql_mm `SQLInterface`
  - CS2Fixes001
  - the cs2kz interfaces

  A small Metamod plugin must receive `ISmmAPI*` in `Load()` and hand it over.
- **Timing (M, assuming the proxy patches Init before returning the interface):** Metamod's Init thunk runs `DLLInit_Pre`, which loads all Metamod plugins. It then calls the proxy's Init thunk (CounterStrikeSharp init), then the real Init, then `AllPluginsLoaded`. So `MetaFactory` works already at CounterStrikeSharp init, which is earlier than today.
- **Level events (M):** Metamod raises `OnLevelInit`/`OnLevelShutdown` from its own loop-mode `LoopInit`/`LoopShutdown` post hooks (`provider_source2.cpp:437-474`), and only for its own plugins.
- **Hook order:** Metamod plugins hook in `Load()` before CounterStrikeSharp init, so in proxy mode CounterStrikeSharp is the outer hook on shared slots (GameFrame, RegisterLoopMode, ClientCommand).
- **Pass-through requirement:** the proxy must pass interface pointers through unwrapped. Otherwise the engine and Metamod hook a wrapper while CounterStrikeSharp hooks the real object, and hook order splits.
- **Managed users (M, gh code search):**
  - `Utilities.MetaFactory`: CRayTraceInterface wrappers (cs2-zones, SonarBomb, SLAYER_*, CS2BotImprover), CS2-Music `Audio002`, bot plugins.
  - `OnMetamodAllPluginsLoaded` used as a generic late-init signal: EntWatchSharp, cs2-challenges, cs2-zones, ZEAssist.

## 5. CounterStrikeSharp's Metamod surface (HEAD 0485c767)

| Area | Location |
|---|---|
| Plugin entry and lifecycle | `src/mm_plugin.cpp`: `:69` `PLUGIN_EXPOSE`, `:77-82` KHook check, `:97` `AddListener`, `:99` `globals::ismm`, `:107-119` `GET_V_IFACE_*`, `:174` / `:249` all-plugins-loaded, `:290` / `:324` level init/shutdown, `:342-360` metadata |
| Globals | `src/core/globals.h:14,114`, `src/core/globals.cpp:70,101-110` |
| Natives | `src/scripting/natives/natives_metamod.cpp:22-29`; `natives_engine.cpp:128,132,135,171` |
| oslink and KHook scanning | `src/core/memory_module.cpp:26,299,558`; `con_command_manager.cpp:48` |
| KHook usage | 28 files, about 150 references |
| Managed | `Utilities.cs:288-294`, `Generated/Natives/API.cs:1699`, `Core/Listeners.g.cs:248-249` |
| Build | `makefiles/shared.cmake:25,62-63,72`, `makefiles/metamod/`, `CMakeLists.txt:22-23,86`, submodule `libraries/metamod-source` |

## 6. Windows

- **Metamod:** same search-path walk with `server.dll`, plus an `AllowDedicatedServers` vtable patch. #279 (Jul 2026): a static tier0 import fails `LoadLibrary` for the whole DLL.
- **ModSharp:** `LoadLibraryA` relative to the working directory, no `AllowDedicatedServers` patch, no Windows loader bugs found.
- **SwiftlyS2:** absolute path, tier0 resolved dynamically. One Windows break (`e1c226e`, an init byte signature).
- **`server_valve.dll`:** not checked for CS2 on Windows.
