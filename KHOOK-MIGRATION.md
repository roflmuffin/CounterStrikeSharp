# CounterStrikeSharp v1.0.374: Metamod API 18 migration

This branch (`khs-khook-fix`) ports the v1.0.374 / `6c02f47` native hooks
to KHook. It targets Linux x86-64 and Metamod 2.0-dev+1466 (API 18).
It is a server-test candidate, not a claim of successful live CS2 validation.

## References and dependency pins

- [Reference migration](https://github.com/mrc4tt/CounterStrikeSharp/commit/d42e3f1c3b97a76ca62d3ce10426fead84cd5073), used only for hook conversion.
- [Official Metamod sample](https://github.com/alliedmodders/metamod-source/tree/a8c72eaf29d4bee30989cddbebfe5bd31e76db07/samples/s2_sample_mm).
- Metamod: `a8c72eaf29d4bee30989cddbebfe5bd31e76db07`.
- Its KHook submodule: `876c66949a1fd8b990687773dcef4ddaea2b4970`.
- Its SafetyHook submodule: `ec3f698a1d9936d72c57c639536fbbedab6d7c8a`.
- All other dependency pins remain at the v1.0.374 baseline.

No custom branding, logging, managed APIs, game behavior, configuration or
gamedata from the reference fork was imported. Existing CustomHudLayout code
was already present in v1.0.374; only its hook registration changed.

## Changed files and hook inventory

| Files | Migration |
| --- | --- |
| `src/mm_plugin.cpp`, `src/mm_plugin.h` | GameFrame, StartupServer, RegisterLoopMode, FindService; global LoadEventsFromFile hook on the resolved CGameEventManager vtable |
| `src/core/globals.cpp`, `src/core/globals.h` | Remove plugin-owned SourceHook globals and includes; use Metamod's KHook interface |
| `src/core/customhudlayout.cpp`, `src/core/customhudlayout.h` | ClientSvcUserMessage post hook |
| `src/core/managers/player_manager.cpp`, `src/core/managers/player_manager.h` | ClientConnect pre/post, ClientPutInServer post, ClientDisconnect pre/post, ClientCommand pre, ClientVoice post; remove unused declarations |
| `src/core/managers/server_manager.cpp`, `src/core/managers/server_manager.h` | Seven server lifecycle callbacks, preserving pre/post placement and const receiver |
| `src/core/managers/entity_manager.cpp`, `src/core/managers/entity_manager.h` | CheckTransmit manual vtable offset becomes Configure/AddContext/AddGlobal; offset still comes from existing gamedata |
| `src/core/managers/event_manager.cpp`, `src/core/managers/event_manager.h` | FireEvent pre/post, Supersede and Recall for changed broadcast arguments |
| `src/core/managers/usermessage_manager.cpp`, `src/core/managers/usermessage_manager.h` | Eight-argument PostEventAbstract virtual pre hook, preserving filtering/blocking |
| `src/core/managers/voice_manager.cpp`, `src/core/managers/voice_manager.h` | SetClientListening pre hook; Recall preserves argument replacement |
| `src/core/managers/con_command_manager.cpp`, `src/core/managers/con_command_manager.h` | DispatchConCommand pre/post; plain command callback no longer invokes hook-only macros |
| `src/scripting/natives/natives_usermessages.cpp` | Existing six-argument original-call path uses KHook::CallOriginal |
| `CMakeLists.txt`, `makefiles/shared.cmake`, `libraries/metamod-source` | Remove five compiled SourceHook sources, switch include directory, pin API 18 dependency |
| `tests/native/CMakeLists.txt`, `tests/native/migration_tests.cpp`, `tests/native/probe_plugin.cpp` | Linux test harness, five migration integration cases, compiled-plugin API probe |
| `KHOOK-MIGRATION.md` | This report and reproduction/server-test instructions |

All migrated hooks are virtual methods, so they use `KHook::Virtual` rather
than unnecessarily replacing them with `KHook::Member` or `KHook::Function`.
Instance hooks use Add/Remove; the two former DVP hooks use AddGlobal/RemoveGlobal.
Callbacks now receive the interface pointer and return `KHook::Return` with
Ignore/Supersede. Existing funchook/DynoHook dynamic native APIs remain intact.

## Behavior differences and limits

- Requires Metamod API 18; it is not compatible with the old API 17 loader.
- A missing CGameEventManager vtable now fails loading with an explicit error.
- ClientConnect post processing checks for absent original-return storage after
  another hook supersedes a call, and uses the override result in that case.
  FindService also handles absent original storage without dereferencing null.
- Unload explicitly removes RegisterLoopMode and FindService as well as the
  other plugin-owned registrations. Existing global-manager lifecycle dispatch
  was not broadened as part of this migration.
- Event ownership, callbacks, managed signatures, offsets, native IDs and HUD
  behavior otherwise follow v1.0.374. Cross-plugin ordering and engine behavior
  still require live validation with the installed API 18 native plugins.

## Local validation (2026-09-09)

| Check | Result |
| --- | --- |
| Linux native Release, Ubuntu 24.04 / GCC 13.3 / CMake 3.28 / WSL2 | PASS; counterstrikesharp.so built |
| dlopen/CreateInterface metadata probe | PASS; CounterStrikeSharp reports API 18; does not call Load |
| Pinned KHook tests plus five migration cases | PASS: 84/84 |
| Entire managed solution Release build | PASS: 0 errors; existing warnings remain |
| Managed API unit tests | PASS: 24/24 |
| API and NativeTestsPlugin publish | PASS |
| Bundled DynoHook generated test executable | FAIL: SIGSEGV; reproduced with the unchanged pinned DynoHook in the separate baseline build |
| NativeTestsPlugin inside CS2 | NOT RUN: no CS2 server/game binaries available locally |

The initial static Debug KHook test harness crashed in nine inline-hook cases:
SafetyHook temporarily removed execute permission from a target page that also
contained shared C++ helpers. Linking the test engine as a separate DSO with
hidden internal symbols, like Metamod's engine separation, resolved all nine.
Neither KHook nor SafetyHook source was patched to achieve this.

The separate DynoHook failure reports trampoline allocation/relocation errors;
GDB ends in asmjit::CodeHolder::copyFlattenedData via createPostCallback during
the virtual-hook test. That dependency and its generated tests were not modified.
This is an outstanding baseline/environment issue, not a passing test result.

Running `dotnet test` against the entire solution also attempts to run the
in-server test plugin as a normal test assembly and fails with missing testhost
assets. Its supported runner is `css_run_tests` inside CS2; run the API unit
test project directly outside the game, as upstream CI does.

The local binary requires at least GLIBC 2.38 and GLIBCXX 3.4.32, as measured
from ELF version requirements. It was built on Ubuntu 24.04, not inside an
official SteamRT SDK. Validate those libraries in the actual server runtime;
SteamRT4 deployment has not been tested. The existing libtier0 dependency is
provided by CS2. The packaged binary has no build-machine RPATH.

## Reproduce on Linux

Use GCC 13+, CMake 3.28+, Ninja, Git, .NET SDK 10 and recursive submodules.
Run from the repository root on `khs-khook-fix`:

```sh
git submodule update --init --recursive
export SEMVER=1.0.374-khs-khook
export GITHUB_SHA_SHORT="$(git rev-parse --short=7 HEAD)"
cmake -S . -B build-khook -G Ninja -DCMAKE_BUILD_TYPE=Release -DCMAKE_SKIP_RPATH=ON
cmake --build build-khook -j6
cmake -S tests/native -B build-khook-tests -G Ninja -DCMAKE_BUILD_TYPE=Debug
cmake --build build-khook-tests -j6
ctest --test-dir build-khook-tests --output-on-failure
LD_LIBRARY_PATH="$PWD/libraries/hl2sdk-cs2/lib/linux64${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}" \
  build-khook-tests/probe_plugin build-khook/addons/counterstrikesharp/bin/linuxsteamrt64/counterstrikesharp.so
dotnet build managed/CounterStrikeSharp.sln -c Release
dotnet test managed/CounterStrikeSharp.API.Tests/CounterStrikeSharp.API.Tests.csproj -c Release
dotnet publish managed/CounterStrikeSharp.API -c Release -o package/addons/counterstrikesharp/api
mkdir -p package/addons/counterstrikesharp/bin/linuxsteamrt64
cp build-khook/addons/counterstrikesharp/bin/linuxsteamrt64/counterstrikesharp.so package/addons/counterstrikesharp/bin/linuxsteamrt64/
tar -C package -czf counterstrikesharp-khs-khook-linux-overlay.tar.gz addons
```

For the additional unchanged dependency test, run
`build-khook/libraries/DynoHook/dynohook_test`; see its known local failure above.

## SourceHook audit

The initial audit covered the complete tracked repository, including the
additional native usermessage-send path outside the listed managers.
The final code/build-input search has no active SourceHook macros, globals,
includes or implementation files. One explanatory comment remains in
`src/mm_plugin.cpp`. This report mentions the old API for documentation.
Upstream dependency source/docs retain legacy references and are not migrated
wholesale; they are not CounterStrikeSharp's active SourceHook implementation.

```sh
git grep -n -E 'SourceHook|sourcehook|\bSH_[A-Z_]+|META_RESULT_ORIG_RET|META_IFACEPTR|RETURN_META|\bMRES_' -- src makefiles CMakeLists.txt
git grep --recurse-submodules -n -E 'SourceHook|sourcehook|\bSH_[A-Z_]+|META_RESULT_ORIG_RET|META_IFACEPTR|RETURN_META|\bMRES_'
```

## Artifact and server test

`counterstrikesharp-khs-khook-linux-overlay.tar.gz` contains only the native
binary and published managed API. Apply over an existing complete v1.0.374
installation. It intentionally has no configs, gamedata, other plugins or
bundled .NET runtime; keep the existing compatible .NET 10 runtime.

On a staging server, stop CS2, back up the existing `bin` and `api` directories,
extract the overlay under `game/csgo`, then start CS2 with Metamod 1466.
Check `meta version`, `meta list`, `meta info <CSS plugin ID>` and `css_plugins list`.
Record the complete startup log if loading fails. Test join/disconnect, map
change, events, command blocking, voice routing, usermessages, HUD and transmit
filtering with representative managed plugins.

The separately published `NativeTestsPlugin` directory can be copied into
`addons/counterstrikesharp/plugins/` on that test server. Use `css_run_tests`
or `css_itest <filter>` after the map has loaded. These tests exercise the game
and are not included in the normal overlay.

Other native plugins showing API 17 errors need their own API 18 migrations;
updating CounterStrikeSharp cannot repair those independent binaries.
