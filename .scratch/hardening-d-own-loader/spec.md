# Spec D: own loader as an opt-in Proxy Mode

Status: ready-for-agent

Part of the incremental hardening effort (`.scratch/incremental-hardening/`).
- **Governed by:** ADR-0005 (own loader as opt-in Proxy Mode under Metamod, which amends ADR-0001) and ADR-0001 (upstream-shaped, plugin API additive).
- **Vocabulary:** `GLOSSARY.md` (Host, Plugin Mode, Proxy Mode, Proxy, Bootstrap).
- **Evidence:** `.scratch/incremental-hardening/research/own-loader/findings.md`.
- **Dependencies:** independent of specs A and B, except for one shared piece (pristine-copy signature scanning, ticket 01), which spec A's resolver also uses.

## Problem Statement

CounterStrikeSharp can only load as a Metamod plugin. That makes Metamod a hard runtime and build dependency, although it uses only a small part of it: loading, KHook, interface factories, level events, console print and `MetaFactory`.

- **A broken Metamod core takes CounterStrikeSharp down, even when CounterStrikeSharp itself is fine.**
  - Metamod/KHook bugs #278 (Jun 2026) and #288 (Oct 2026) stopped some Linux servers for about 14–17 days each.
  - While Metamod doesn't load, nobody can see whether CounterStrikeSharp is broken too.
- **Metamod's API churn forces CounterStrikeSharp work.** The SourceHook→KHook move (Metamod API 17→18) forced #1418 and made dev Metamod builds mandatory.
- **CounterStrikeSharp pulls in metamod-source and its whole release cadence** for services it could provide itself. Two peers already load themselves: ModSharp, with 0 loader fixes in about 13 months, and SwiftlyS2.
- **What this does *not* fix: most CS2-update outages.** In 7 of 9 Metamod load failures, CounterStrikeSharp shared the root cause (tier0 imports, SDK header/struct batches). A CounterStrikeSharp with its own loader would have saved ≤8% of Metamod's 162 h down time. That work is the tier0/struct decoupling, which is out of scope here.

Server operators, especially those running CS2Fixes, MultiAddonManager, sql_mm or cs2kz next to CounterStrikeSharp, must not be forced into any setup change.

## Solution

CounterStrikeSharp gets a second way to load, **Proxy Mode**, next to today's **Plugin Mode**:

- **Plugin Mode (default, unchanged):** Metamod loads the **Bootstrap** plugin (same `.vdf` as today). The Bootstrap loads the core on the Metamod host with Metamod's KHook. Behaviour, hook order and install are as today.
- **Proxy Mode (opt-in):** the operator adds `Game csgo/addons/counterstrikesharp` to `gameinfo.gi`, after the Metamod line if Metamod is installed.
  - CounterStrikeSharp's **Proxy** `libserver.so` then becomes the game DLL: Metamod's loader picks it as its game DLL, or the engine loads it directly when Metamod is absent.
  - The Proxy loads the real server and the core on the own host with CounterStrikeSharp's own KHook.
  - If Metamod is present, the Bootstrap notices the running core and only hands over Metamod's API, so `MetaFactory` keeps working.
  - A broken Metamod core no longer takes CounterStrikeSharp down.
- **Falls back on its own:** if a game update overwrites `gameinfo.gi` and the operator only restores the Metamod line, CounterStrikeSharp runs in Plugin Mode again. Rollback means removing the line.
- **Status at boot:** a boot checker logs which mode is active and why.

Linux first. Windows keeps Plugin Mode until the Windows Proxy follows, so it's never worse off than today.

## User Stories

### Server operator

1. As a server operator who changes nothing, I want CounterStrikeSharp to load and behave exactly as today, so that updating CounterStrikeSharp never forces a setup change.
2. As a server operator, I want to opt into Proxy Mode by adding one `gameinfo.gi` line, so that switching is one documented step.
3. As a server operator in Proxy Mode, I want CounterStrikeSharp to keep running when the Metamod core fails to load, so that a Metamod-only bug doesn't take my plugins down.
4. As a server operator in Proxy Mode, I want my Metamod plugins (CS2Fixes, MultiAddonManager, sql_mm, cs2kz) to keep working whenever Metamod itself loads, so that I don't have to choose between them and CounterStrikeSharp.
5. As a server operator, I want a game update that overwrites `gameinfo.gi` to fall back to Plugin Mode on its own if I only restore the Metamod line, so that a lost line never causes an outage.
6. As a server operator, I want to roll back by removing the line, so that trying Proxy Mode is risk-free.
7. As a server operator, I want the boot log to state the active mode and why (line missing, Metamod absent, wrong line order, core mismatch), so that I can tell what I'm running.
8. As a server operator, I want a loud warning when the CounterStrikeSharp line is above Metamod's, so that I don't silently lose all Metamod plugins.
9. As a server operator without Metamod, I want CounterStrikeSharp to run with only its own line, so that I can drop Metamod if I don't need it.
10. As a server operator, I want a core that fails to load or start to leave the server running without CounterStrikeSharp, with a clear log line, so that a CounterStrikeSharp break doesn't stop the game server.
11. As a server operator on Windows, I want CounterStrikeSharp to behave exactly as today, so that Linux-first work never regresses Windows.

### Tournament operator

12. As a tournament operator, I want a launch parameter that refuses to start the server when the core fails, so that a match never runs without its admin and match plugins.
13. As a tournament operator, I want to see the active mode at boot, so that I know which setup each match server runs.

### Plugin author

14. As a plugin author, I want `Utilities.MetaFactory` to keep working whenever Metamod is loaded, in either mode, so that my RayTrace or audio interop keeps working.
15. As a plugin author, I want to query whether Metamod is loaded and which mode is active, so that I can degrade gracefully when `MetaFactory` returns null.
16. As a plugin author, I want `OnMetamodAllPluginsLoaded` to fire in every mode, even without Metamod, so that my late-init code still runs.
17. As a plugin author, I want a neutral `OnAllPluginsLoaded` alias, so that new code doesn't name Metamod.
18. As a plugin author, I want map start and map end to fire in the same order in both modes, so that my plugin doesn't need to know the mode.
19. As a plugin author, I want the hook-order difference in Proxy Mode documented, so that I understand why my pre-hook runs before a Metamod plugin's.
20. As a plugin author, I want gamedata signatures to match even when a Metamod plugin has detoured the same function, so that my plugin doesn't break only on servers that also run CS2Fixes.

### CounterStrikeSharp maintainer

21. As a maintainer, I want every Metamod-facing call behind one Host seam, so that the core doesn't care how it was loaded.
22. As a maintainer, I want the core to compile without Metamod headers, so that Metamod API churn touches only the Bootstrap and the Metamod host.
23. As a maintainer, I want the Proxy to import nothing from tier0 or the SDK, so that tier0 export changes can never stop it loading.
24. As a maintainer, I want the Proxy to avoid byte signatures and hook only slots that never moved, so that it needs no game-update fixes.
25. As a maintainer, I want the Proxy to load only a core with the matching ABI version, so that mixed installs fail clearly instead of crashing.
26. As a maintainer, I want a live coexistence checklist with CS2Fixes and MultiAddonManager, so that each release proves both modes work next to Metamod plugins.
27. As a maintainer, I want the Proxy-mode boots added to the per-build CI once it exists, so that both modes are checked on every game update.

## Implementation Decisions

### Host seam

- The core reaches Metamod only through a Host interface. It covers:
  - the engine and server interface factories
  - level start/end notifications
  - console print
  - the KHook instance
  - an optional Metamod API (null when Metamod isn't reachable)
  - all-plugins-loaded
- Two implementations:
  - **Metamod host:** wraps today's `ISmmAPI` calls: `GET_V_IFACE_*`, `AddListener`, `META_CONPRINT`, `MetaFactory`, `GetServerFactory`/`GetEngineFactory`.
  - **Own host:** factories come from the Proxy (engine factory from `ISource2ServerConfig::Connect`, server factory from the real server's `CreateInterface`). Console print uses tier0's C `Msg` export, resolved dynamically. Level events come from its own loop-mode hooks. The KHook instance is its own.
- Natives that use Metamod today (`META_FACTORY`, `GetValveInterface`, console print) go through the Host. Their behaviour in Plugin Mode is unchanged.

### Bootstrap and core split

- The Metamod plugin binary the existing `.vdf` points at becomes the Bootstrap. The core becomes a separate library in the same `bin/<platform>/` directory.
- The Bootstrap is the only code that includes Metamod headers. The core build drops the Metamod include dirs. The metamod-source submodule stays, for the Bootstrap.
- **Bootstrap `Load()`:**
  1. If a core is already running (the Proxy loaded it, detected through an exported C symbol on the already-loaded Proxy, e.g. `dlopen` with `RTLD_NOLOAD`), hand over `ISmmAPI` and Metamod's all-plugins-loaded and level events. Don't load a second core.
  2. Otherwise load the core and start it on the Metamod host.
- **Bootstrap unload:**
  - In Proxy Mode, `meta unload` of the Bootstrap only withdraws the Metamod API: `MetaFactory` returns null again.
  - In Plugin Mode, unload behaves as today.

### KHook

- The core compiles against KHook's forwarding header. Every call goes through one instance pointer that the Host sets.
  - Metamod host: Metamod's per-plugin instance, as today.
  - Own host: a statically linked standalone KHook instance.
- Ticket 02 verifies that a standalone KHook build can supply an instance for that pointer. If it can't, the fallback is a thin adapter with the same API.
- The own KHook is built with hidden symbol visibility, and with `--exclude-libs,ALL` on Linux, so Metamod's KHook calls can never bind to it.
- The own KHook instance is never shut down and its JIT memory is never freed, because Metamod's KHook may chain through it. Leaking at process exit is fine.
- In Proxy Mode with Metamod plugins, both instances hook shared slots (GameFrame, ClientCommand, RegisterLoopMode). Metamod plugins hook in their `Load()` before the core starts, so CounterStrikeSharp's hooks are the outer ones. This is documented, and today's interleaving isn't reproduced.

### Proxy (Linux first)

- **Location:** `csgo/addons/counterstrikesharp/bin/linuxsteamrt64/libserver.so`, next to the Bootstrap and the core. Metamod's loader then finds it as `<Game path>/bin/linuxsteamrt64/libserver.so`.
- **Imports:** libc and libdl only, with no SDK and no tier0. Anything else is resolved via `dlsym`. (Windows: kernel32 only.)
- **Paths:**
  - The real server is loaded by absolute path, derived from the Proxy's own location (`csgo/bin/linuxsteamrt64/libserver.so`), never from the working directory.
  - The core is loaded by absolute path from the Proxy's directory.
- **`CreateInterface`:** exported, and passes every request through to the real server's `CreateInterface` unchanged. Interface pointers are never wrapped.
- **Hooks:** `ISource2ServerConfig` slot 0 (Connect, which yields the engine factory) and `ISource2Server` slots 3 and 5 (Init, Shutdown). Patched by vtable swap *before* the interface pointer is returned, so that Metamod's loader thunk is outer and loads Metamod plugins before the core starts. A slot is restored only if it still holds the Proxy's own pointer. No byte signatures. A string anchor only if a needed entry point is unreachable through these slots.
- **Core start:** in the Init thunk. Load the core, check its ABI version (a constant exported by both, bumped on any Proxy↔core contract change), start it on the own host, then call the real Init.
- **Double-load guard:** the Proxy exports a C symbol the Bootstrap can find, and the core refuses a second start.
- **Failure:**
  - **Fail open (default):** if the core is missing, fails to load (missing import), has the wrong ABI version, or its start fails, the Proxy logs one loud line (stderr plus the server log via tier0 `Msg` if available), writes the reason to the status file, and keeps forwarding. The server runs without CounterStrikeSharp.
  - **`-css_fail_closed`:** the Proxy aborts server start with that reason instead. The parameter is read from the command line by the Proxy, which never parses config files. Plugin Mode ignores it.
- **Not handled:** `libserver_valve` (spike ticket only) and chain-loading Metamod.

### Level lifecycle in Proxy Mode

- The own host hooks loop-mode `LoopInit`/`LoopShutdown` at the same points Metamod's provider does (`provider_source2.cpp:437-474`). From those it drives the core's level start/end, and from them `OnMapStart`/`OnMapEnd`.
- **Event order** must match Plugin Mode, verified by an event-order comparison test:
  - map start, map end and map change
  - all-plugins-loaded relative to map start
  - hibernation
- **All-plugins-loaded** fires at the later of "core start done" and "Bootstrap's all-plugins-loaded". If Metamod isn't loaded, it fires at core start.
- **Things that hang off it today** move with it: the OnTakeDamage hook attach, and `OnMetamodAllPluginsLoaded`.

### Managed API (additive)

- `Utilities.MetaFactory`: unchanged signature. It returns null (`IntPtr.Zero`) when the Host has no Metamod API.
- New query for the active Host (proposed: `Server.HostMode` with values `Plugin` and `Proxy`) and for whether Metamod's API is reachable (proposed: `Server.IsMetamodLoaded`). These are not Engine Features (they don't depend on Bindings) and not capabilities (which are plugin-to-plugin services).
- `OnMetamodAllPluginsLoaded` fires in every mode, with the timing above. New alias `OnAllPluginsLoaded` fires at the same moment. The old name is not marked `[Obsolete]` in this spec.
- Plugin docs gain a "Plugin Mode and Proxy Mode" page covering:
  - how to detect the mode
  - `MetaFactory` returning null
  - hook order next to Metamod plugins
  - no unload in Proxy Mode

### Pristine-copy signature scanning (both modes)

- Signature scanning (core gamedata and plugin-supplied signatures) scans a read-only mapping of the module file on disk, not live memory. Matches are translated back to live addresses through the section layout (ELF program headers; PE section headers on Windows).
- This makes signatures immune to inline detours installed by any KHook instance, Metamod plugin or anti-cheat.
- Shared with spec A: its resolver scans through the same scanner, so the uniqueness check (I1) also runs on pristine bytes.

### Boot checker and status

- At core start, and in the Proxy on failure, one log block states:
  - the active mode (Plugin or Proxy)
  - whether Metamod is loaded
  - whether `gameinfo.gi` contains the CounterStrikeSharp line and where it sits relative to Metamod's
  - why this mode was chosen (e.g. "proxy line missing, running as Metamod plugin" or "Metamod line below CounterStrikeSharp line: Metamod plugins are not loaded")
  - the Proxy and core ABI versions
- A status file (`addons/counterstrikesharp/status/loader.json` or similar) holds the same facts for panels and scripts.
- Read-only: the checker never edits `gameinfo.gi`.

### Packaging and install

- One release package contains:
  - the Bootstrap and the `.vdf` (unchanged location)
  - the core
  - the Proxy
  - the managed runtime, as today
- Extracting the package changes nothing on an existing server. Proxy Mode needs the one gameinfo line.
- Install docs gain a Proxy Mode section covering:
  - line placement with and without Metamod
  - rollback
  - what happens after a game update overwrites `gameinfo.gi`
  - `-css_fail_closed`
  - guidance for hosting panels

### Rollout

1. Proxy Mode ships opt-in on Linux.
2. Windows keeps Plugin Mode until the Windows Proxy ticket lands. That ticket should follow, but isn't must.
3. After 3 game updates per OS during which the Proxy needed no fix, the docs switch the *recommended* install to Proxy Mode. Code defaults never change, and `gameinfo.gi` is never auto-edited.
4. Removing the Metamod host or the metamod-source submodule is out of scope.

## Testing Decisions

- Good tests assert behaviour at seams: given this install and these files, this mode and this boot status; given this event sequence, these managed callbacks. They don't assert Host internals.
- **Native unit tests (spec A's doctest target):**
  - Pristine-copy scanning: a pattern on a buffer that is detoured in memory but pristine on disk still resolves, and to the live address. Address translation on synthetic ELF and PE layouts.
  - Proxy slot restore: compare-before-restore with two stacked fake hookers, restored in either order.
  - The ABI version check.
  - Boot checker gameinfo parsing: line present, absent, above Metamod, below Metamod, Metamod absent.
- **Managed tests (API.Tests):**
  - `MetaFactory` returns null without a Metamod API.
  - `OnAllPluginsLoaded` and `OnMetamodAllPluginsLoaded` fire once, in both modes, through a fake Host.
  - `Server.HostMode` and `Server.IsMetamodLoaded` report what the Host says.
- **Live matrix (laptop CS2 server, Linux; manual checklist per release until spec C's CI exists, then two CI jobs):**
  1. **Plugin Mode, unchanged install:** native test suite passes, behaviour identical to the last release.
  2. **Proxy Mode, no Metamod:** server boots, GameFrame ticks, a managed plugin loads, and map change fires `OnMapEnd` → `OnMapStart`.
  3. **Proxy Mode under Metamod with CS2Fixes and MultiAddonManager:**
     - GameFrame, ClientCommand and map change work in all three.
     - `MetaFactory("MultiAddonManager003")` returns non-null.
     - `meta unload` of CS2Fixes, then `meta unload` of the Bootstrap, then server shutdown: no crash.
  4. **Proxy Mode with the Metamod core removed** (`metamod.2.cs2.so` missing): server runs, CounterStrikeSharp runs, `MetaFactory` returns null, and the boot status says so.
  5. **Line lost:** gameinfo with the Metamod line only gives Plugin Mode, and the boot status explains it.
  6. **Wrong order:** CounterStrikeSharp line above Metamod gives a loud warning.
  7. **Core failure:** a core with a wrong ABI version or a missing import fails open; with `-css_fail_closed` the server refuses to start.
- **Event-order parity:** the same map-change script in Plugin Mode and Proxy Mode produces the same managed event sequence.
- Prior art: spec A's doctest target and live harness, and the native test plugin's smoke-test command.

## Out of Scope

- tier0 `dlsym` imports, slots as data and struct decoupling. These would move outage hours; they belong to specs B and C and a later decoupling effort.
- Removing the Metamod host, the Bootstrap or the metamod-source submodule.
- Hosting Metamod plugins inside CounterStrikeSharp, or chain-loading Metamod from the Proxy.
- Loading through `libserver_valve` (spike ticket only).
- Auto-repairing `gameinfo.gi`.
- Pinning or shipping a Metamod build.
- Any outage-hours target.
- Runtime unload or reload of the core in Proxy Mode.
- The Windows Proxy in the must set (it's a should-follow ticket).
- Opening upstream PRs (ADR-0001: fork PRs for now; the RFC draft is ready for when that changes).

## Further Notes

- Upstream RFC draft: `.scratch/hardening-d-own-loader/rfc.md`.
- Sliced into 17 tickets under `.scratch/hardening-d-own-loader/issues/`, each with a `Tier:` line:
  - **Tier 0, standalone:**
    - 01 pristine-copy signature scanning
    - 02 KHook instance spike
    - 16 post the RFC (human)
  - **Tier 1, base:**
    - 03 Host seam (Metamod host only, no behaviour change)
    - 04 Bootstrap/core split
    - 05 Proxy Mode without Metamod (tracer)
  - **Tier 2, add-ons** (each needs 05):
    - 06 Proxy Mode under Metamod
    - 07 level lifecycle parity
    - 08 managed API
    - 09 boot checker and status
    - 10 `-css_fail_closed`
    - 12 packaging and docs
    - 15 `server_valve` spike
  - **Tier 3, dependent add-ons:**
    - 11 coexistence validation (needs 06, 07)
    - 13 Windows Proxy (needs 06)
    - 14 CI jobs (needs 06 and spec C's I7)
    - 17 promote Proxy Mode to recommended (human, needs 11 and 3 updates in the field)
  - **Must set:** 01–12. **Later:** 13 (should-follow), 14, 15, 16, 17.
- Evidence: `.scratch/incremental-hardening/research/own-loader/` (findings, the session brief, earlier research).
