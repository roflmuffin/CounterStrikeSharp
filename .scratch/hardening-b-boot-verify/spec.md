# Spec B: Vtable Checks, Slot Bindings and Heal (I4, I5)

Status: ready-for-agent

Part of the incremental hardening effort (`.scratch/incremental-hardening/`). Governed by ADR-0001 (incremental, upstream-shaped, additive), ADR-0002 (fail soft per Engine Feature, Resolution Report), ADR-0003 (Slot Bindings: fingerprint windows, healed at boot) and ADR-0004 (binding resolution behind a swappable seam). Vocabulary: `GLOSSARY.md`. Builds on spec A's base (`.scratch/hardening-a-fail-soft/`, tickets 03–05).

## Problem Statement

CounterStrikeSharp calls and hooks engine interfaces through vtable slot indices compiled in from community SDK headers, and calls game-class virtual functions through hand-maintained Gamedata offsets. When Valve inserts or removes a virtual, those indices silently point at the wrong function:

- At 1.42.0.1 Valve inserted one virtual into ISource2Server. All 10 hooked ISource2Server slots shifted by one; the GameFrame hook landed on an unrelated function and the server segfaulted. The fix had to wait for an SDK update; servers ran on a community binary patch meanwhile.
- IVEngineServer2 shifted slots CounterStrikeSharp uses in 7 incidents and ICvar in 3, each time breaking a hook and many direct calls at once (around 20 engine calls and 14 cvar calls).
- Game-class virtuals used through Gamedata offsets move constantly: Respawn and CommitSuicide moved 15 times each, ChangeTeam and Teleport 9 times. Each move is a manual Gamedata fix while the feature calls the wrong function.
- Nothing checks at boot that a slot still holds the expected function. A shifted slot is discovered by crashes or by users.

For server operators this means crashes and wrong behaviour on routine Game updates; for maintainers it means an SDK bump or a hand Gamedata fix for every shift.

## Solution

Every vtable slot CounterStrikeSharp hooks or calls on an interface that moves, and every game-class virtual it calls through Gamedata, becomes a Slot Binding: data in Gamedata carrying its slot index and a fingerprint window (the masked instruction fingerprint of the slot and its two neighbours). At boot, a Vtable Check verifies each Slot Binding against the live vtable found by RTTI:

- The window matches at the expected index: the Binding resolves.
- The window matches exactly once nearby: the Binding resolves at the new index as a Heal, and the startup report says so with the evidence. A single inserted virtual (as at 1.42.0.1) becomes a non-event.
- Otherwise: the Binding fails and only its Engine Features are disabled (spec A's fail-soft machinery).

A generator carries Slot Bindings forward to a new build by aligning the previous build's vtables against the new one, so a maintainer regenerates indices and fingerprints with one command instead of hand-editing. Resolution sits behind a swappable seam, so an external resolver can later replace the built-in one without changes elsewhere.

## User Stories

### Server operator

1. As a server operator, I want a Game update that inserts one virtual into an engine interface to heal at boot, so that my server keeps running with no update from anyone.
2. As a server operator, I want GameFrame (ticks, timers, next-frame callbacks) to keep working across slot inserts, so that the most critical hook is never the reason my server is down.
3. As a server operator, I want a slot that cannot be verified or healed to disable only the Engine Features that use it, so that one shifted method does not crash the server.
4. As a server operator, I want the startup report to list every Heal (Binding, old index, new index, evidence), so that I know the server is running on healed locations.
5. As a server operator, I want the startup report to warn when a vtable's size changed, so that I know a Game update touched an interface even if nothing we use moved.
6. As a server operator, I want direct engine calls (player kick, client listening, command execution, cvar access) to be verified, so that a shifted engine interface never calls the wrong function.
7. As a server operator, I want game-class virtuals like respawn, suicide, team change and teleport to heal when they move, so that those features keep working without a Gamedata release.
8. As a server operator, I want a Heal to be refused when the evidence is ambiguous, so that the server never silently calls the wrong function.
9. As a server operator on Windows, I want the startup report to say that Slot Bindings are unverified on my platform, so that I know the protection level I have.
10. As a server operator, I want verification to add no noticeable boot time, so that restarts stay fast.

### Tournament operator

11. As a tournament operator, I want a plugin that requires GameFrame or connection events to refuse to load if those are disabled, so that a match never runs on a half-hooked server.
12. As a tournament operator, I want to see before a match whether anything was healed or disabled on this build, so that I can decide whether to play on it.

### Plugin author

13. As a plugin author, I want Engine Features backed by hooks and engine calls (game frame, connection events, client commands, voice, console dispatch, server lifecycle events) to be queryable like spec A's, so that I can degrade gracefully.
14. As a plugin author, I want healed Engine Features to count as available, with the Heal visible in their status, so that I can choose to warn users without breaking.
15. As a plugin author, I want my plugin's existing reads of Gamedata offsets by key to keep working, so that the new Slot Binding fields do not break me.
16. As a plugin author, I want the offsets I read from Gamedata for core game-class virtuals to reflect the verified or healed index, so that my calls follow a Heal too.
17. As a plugin author, I want listeners on a disabled hook-backed Engine Feature to register, log once and never fire, so that my plugin still loads.

### Gamedata maintainer

18. As a Gamedata maintainer, I want one command that takes the previous Gamedata and a new build's binaries and emits updated Slot Bindings, so that slot shifts stop being manual work.
19. As a Gamedata maintainer, I want the generator to flag every Slot Binding it cannot carry forward unambiguously, so that I only look at the hard cases.
20. As a Gamedata maintainer, I want the generator never to guess, so that a regenerated Gamedata file is never silently wrong.
21. As a Gamedata maintainer, I want the generator to also regenerate fingerprint windows, so that the next boot verifies against the new build.
22. As a Gamedata maintainer, I want the generator to read builds from the shared build archive, so that I can regenerate for any archived build.
23. As a Gamedata maintainer, I want the generator's output to be ordinary Gamedata in the agreed shape, so that I review it as a normal diff and commit it.
24. As a Gamedata maintainer, I want an initial set of Slot Bindings bootstrapped from the last build where the SDK headers were known correct, so that we start from verified ground truth.
25. As a Gamedata maintainer, I want signatures to stay out of the generator, so that its output is limited to what alignment can prove.

### CounterStrikeSharp maintainer

26. As a maintainer, I want the boot check and the generator to share one fingerprint and alignment implementation, so that they can never disagree and wrongly disable features.
27. As a maintainer, I want Binding resolution behind one resolver interface whose only output is the Resolution Report, so that an external resolver can replace the built-in one without touching callers.
28. As a maintainer, I want RTTI vtable lookup to match the exact class name and report ambiguity, so that a class whose decorated name is a substring of another is not found by luck of ordering.
29. As a maintainer, I want hooks to be registered by verified slot index, so that hooks follow Heals instead of compiled header positions.
30. As a maintainer, I want direct calls on IVEngineServer2 and ICvar to go through verified slot indices, so that the two most frequently shifting interfaces stop breaking us.
31. As a maintainer, I want direct calls on interfaces that never moved to stay compiled but be verified, so that I do not migrate stable code needlessly yet still catch a future shift.
32. As a maintainer, I want Heal tested against real archived build pairs, including 1.41.9.7 → 1.42.0.1, so that I know it works on actual breaking updates, not just synthetic ones.
33. As a maintainer, I want shifts across identical function bodies to fail rather than heal, so that the known blind spot of fingerprinting never produces a silent wrong.
34. As a maintainer, I want the Gamedata shape change to be additive, so that older tooling and plugins reading Gamedata keep working.
35. As a maintainer, I want Windows to build and behave exactly as today (header indices), so that Linux-only verification does not regress Windows.
36. As a maintainer, I want the Python research scripts usable as a test oracle for the C++ fingerprint, so that the port is checked against the numbers the design was based on.

## Implementation Decisions

### Slot Bindings and the Gamedata shape (additive)

- A Slot Binding names: the RTTI class whose primary vtable holds it, the module that class lives in (server, engine2 or tier0), the slot index, and the fingerprint window (three fingerprints: slot − 1, slot, slot + 1), per OS.
- Existing Gamedata offset entries that are game-class virtuals gain optional class, module and fingerprint-window fields; their keys and the existing per-OS index stay, so anything reading them by key keeps working. Offsets that are struct field offsets (e.g. take-damage hit group, check-transmit player slot) are not Slot Bindings and are untouched.
- Interface Slot Bindings are new Gamedata keys with the same shape.
- Windows entries carry today's header-derived indices and no fingerprints; they are reported as unverified.

### Fingerprints, Vtable Check and Heal (ADR-0003)

- Fingerprint: the first instructions of the slot's target function (up to 24, stopping at the first return or unconditional jump), with immediates and all memory displacements masked (RIP-relative and base + displacement; amended, ADR-0003: masking only RIP-relative operands gave 26 false failures of our slots over the archived pairs, all displacements 7, both with zero wrong). The whole-function level measured 0.86% false changes over all slots and 2.6% on our slots across 8 archived builds.
- Vtable lookup by RTTI uses exact decorated-name matching (length prefix, not a substring of a longer name) and fails as ambiguous if more than one primary vtable matches.
- Vtable Check per Slot Binding, on Linux: window matches at the stated index → resolved; otherwise the slot's own fingerprint matches at the stated index and is unique within ±16 slots both on the generation build (recorded in Gamedata) and in the live vtable → resolved (amended, ADR-0003: a changed neighbour alone no longer fails it; false failures 7 → 4, zero wrong); otherwise search within ±16 slots for a position where the whole window matches; exactly one such position → healed at that index; zero or several → failed (reason: fingerprint mismatch or ambiguous heal). A slot whose own fingerprint is shared by an adjacent slot (identical bodies) never heals; a mismatch there fails.
- A Slot Binding without a fingerprint window in Gamedata (e.g. Gamedata from before Slot Bindings) is unverified: used at its Gamedata index, reported "unverified", never "failed".
- The search range ±16 covers every shift of our slots observed in three years (largest: +12 at the tail of the engine service manager; +10 for game-class virtuals). It is a constant, not configuration.
- A vtable whose size differs from the size recorded at generation time is a warning in the startup report, never a failure by itself.
- The runtime never writes healed indices back to Gamedata.
- An x86-64 instruction decoder is required at boot. Reuse one already linked into the plugin by its hooking libraries if available; otherwise add a small permissively licensed decoder. (None is linked: KHook's Zydis lives inside Metamod's binary. Zydis 4, MIT, is vendored as its amalgamation in `libraries/zydis`.)

### Resolver seam (ADR-0004)

- The Binding resolver from spec A becomes an interface: input is Gamedata plus the loaded modules; output is the Resolution Report. The built-in adapter does signature scanning (spec A) and Vtable Checks with Heal (this spec). No code outside an adapter locates Bindings in binaries.
- The Resolution Report gains the "healed" status with evidence (old index, new index, matched window) alongside resolved and failed. Healed Bindings count as resolved for Engine Feature availability; the startup report and the plugin query API show the Heal.
- One C++ fingerprint and alignment core is shared by the built-in adapter and the generator.

### Hooks and calls through Slot Bindings (I5)

- All 22 compiled-in vtable hooks are registered by verified slot index on the live vtable instance, not by compiled member-function position. GameFrame is hooked like the others (no detour).
- Direct calls on IVEngineServer2 and ICvar go through verified slot indices, and so do the two direct calls on ISource2Server (one shifted at the Armory update), since that interface's hooks migrate anyway.
- Direct calls on interfaces that never shifted a slot we use (game event manager, game event system, network server service, engine service manager, game entities) stay compiled method calls. Each called slot is still declared as a Slot Binding and verified; if its verified index differs from the compiled one (a Heal or a move), its Engine Features are disabled, because a compiled call cannot follow a Heal.
- Network messages (shifted once, on a direct call) and the schema system live in modules the build archive does not hold yet, so they cannot be fingerprinted; their calls stay compiled and unverified in this spec, reported as such. Adding those modules to the archive and verifying them is a later ticket.
- Game-class virtuals already called through Gamedata offsets (respawn, suicide, team change, teleport, item services, skeleton instance, picker entity, is-player-pawn, check-transmit) use the verified or healed index, on both the native and managed sides.

### New Engine Features (stable names, additive)

| Engine Feature | Slot Bindings |
|---|---|
| `cssharp.server/game-frame` | ISource2Server GameFrame |
| `cssharp.server/hibernation-events` | ISource2Server ServerHibernationUpdate |
| `cssharp.server/steam-api-events` | ISource2Server GameServerSteamAPIActivated, GameServerSteamAPIDeactivated |
| `cssharp.server/hostname-changed` | ISource2Server OnHostNameChanged |
| `cssharp.server/fatal-shutdown` | ISource2Server PreFatalShutdown |
| `cssharp.server/update-when-not-in-game` | ISource2Server UpdateWhenNotInGame |
| `cssharp.server/pre-world-update` | ISource2Server PreWorldUpdate |
| `cssharp.server/startup` | INetworkServerService StartupServer |
| `cssharp.server/loop-mode` | IEngineServiceMgr RegisterLoopMode, FindService |
| `cssharp.player/connection-events` | IServerGameClients ClientConnect, ClientPutInServer, ClientDisconnect |
| `cssharp.player/client-commands` | IServerGameClients ClientCommand |
| `cssharp.player/voice-events` | IServerGameClients ClientVoice |
| `cssharp.player/set-listening` | IVEngineServer2 SetClientListening |
| `cssharp.ui/custom-hud-layout` | IServerGameClients ClientSvcUserMessage |
| `cssharp.commands/console-dispatch` | ICvar DispatchConCommand |
| `cssharp.commands/add-command` | ICvar FindConCommand, RegisterConCommand, GetConCommandData, UnregisterConCommandCallbacks, GetCharacterSet |
| `cssharp.player/execute-client-command-from-server` | ICvar GetCharacterSet, FindConCommand, DispatchConCommand |
| `cssharp.convars/find` | ICvar FindConVar, GetConVarData |
| `cssharp.convars/read` | ICvar GetConVarData |
| `cssharp.convars/write` | ICvar GetConVarData, CallFilterCallback, CallChangeCallback, CallGlobalChangeCallbacks, QueueThreadSetValue |
| `cssharp.convars/create` | ICvar FindConVar, GetConVarData, RegisterConVar, UnregisterConVarCallbacks |
| `cssharp.convars/change-events` | ICvar InstallGlobalChangeCallback, RemoveGlobalChangeCallback, GetConVarData |
| `cssharp.network/user-messages` | IGameEventSystem PostEventAbstract |

- `cssharp.events/game-events` (spec A) additionally depends on the game event manager's LoadEventsFromFile and FireEvent Slot Bindings.
- Each direct call on IVEngineServer2 and ICvar is a Slot Binding of the Engine Feature(s) whose plugin-visible API uses it, named under the same rule (one Engine Feature per plugin-visible API, `cssharp.<area>/<name>`). The ticket that migrates those calls enumerates them against the code and adds them to the catalog; names are permanent once added.
- Existing spec A Engine Features backed by game-class virtual offsets keep their names; their Bindings simply become verified Slot Bindings.

### Generator (offline, ADR-0004)

- A command-line tool built alongside the native test target. Input: the previous Gamedata, the previous build's binaries and the new build's binaries (server, engine2, tier0), read from the shared build archive `~/dev/cs2-builds/<build-id>/`. Output: Gamedata with updated Slot Binding indices, fingerprint windows and recorded vtable sizes, plus a list of Slot Bindings it could not carry forward unambiguously.
- Carry-forward rule: locate the class by RTTI in both builds; align old and new vtables by fingerprint; accept a new index only if the old window matches exactly once in the new vtable; otherwise leave the entry unchanged and list it for a human. Never guess.
- Signatures are out of scope for the generator.
- Bootstrap: the first Slot Bindings and fingerprints are generated on build 1.41.9.7, where the SDK header indices are known correct, then carried forward to the current build by the generator itself (which must reproduce the 1.42.0.1 shift).
- The generator never runs at server boot and is not shipped with the plugin.

### Windows

- Builds and behaves as today: hooks and calls use header-derived indices from Gamedata; no Vtable Check or Heal; the startup report states "unverified". Windows verification is a later effort.

## Testing Decisions

- Good tests assert external behaviour at the seams: given these Gamedata entries and these binaries, this Resolution Report; given these two builds and this Gamedata, this regenerated Gamedata; given this boot, this server behaviour and startup report. Never assert internal structure of the fingerprint or alignment code.
- **Seam 1, resolver (native, doctest):**
  - Synthetic vtables, in CI: window matches → resolved; window shifted by k within range → healed with evidence; window matches twice → failed as ambiguous; shift across identical bodies → failed, never healed; shift beyond ±16 → failed; size change alone → resolved with a warning; RTTI name that is a substring of another class name → exact match only; Windows → unverified.
  - Real archived builds, local and laptop only, skipped when the build archive is absent (Valve binaries are never committed): Gamedata bootstrapped on 1.41.9.7 resolved against 1.42.0.1 heals all 10 ISource2Server Slot Bindings at +1 with zero wrong; the 7 consecutive pairs of the 8 archived builds produce zero wrong resolutions and the known real moves (e.g. ChangeTeam) are healed or failed, never silently wrong.
- **Seam 2, generator (native CLI):** replay archived pairs; compare output with the hand-fixed Gamedata history: every moved Slot Binding regenerated correctly or flagged, none wrong; unchanged builds produce no diff except fingerprints that legitimately changed.
- **Seam 3, end-to-end (laptop server, spec A's harness):** boot with all ISource2Server Slot Bindings set one index too low (simulating 1.42.0.1 from the other side): the server heals them, GameFrame ticks (timers fire), and the startup report lists the Heals. Boot with one corrupted fingerprint: only its Engine Feature is disabled. Boot with valid Gamedata: no Heals, no failures.
- **Managed side:** spec A's tests that build Resolution Reports by hand gain the healed status: healed counts as available; the query API and startup report show the Heal.
- The Python research scripts (`.scratch/incremental-hardening/research/specB/`, `R/`, `G2/scripts/`) are a test oracle: fixture expectations for the real-build tests come from them.
- Prior art: spec A's doctest target and end-to-end harness; the native test plugin's smoke-test command.

## Out of Scope

- Wrong-but-unique signatures (need a second independent anchor; T2).
- Regenerating signatures (T2).
- The IGameSystem vtable CounterStrikeSharp implements; hard-coded struct layouts; tier0/loader link breaks.
- Windows Vtable Checks and Heal.
- Migrating direct calls on interfaces that never shifted (they are verified, not migrated).
- Verifying network messages and schema system calls (their modules are not archived yet).
- Running the generator automatically per build (spec C's CI), runtime Gamedata reload, and the signed updater (spec C).
- The build archiver (owned by the PoC repo; this spec only reads its output).
- Replacing the built-in resolver with the PoC's resolver library (enabled by the seam, not done here).
- Upstream PRs (fork PRs only for now).

## Further Notes

- Depends on spec A's base (tickets 03–05: doctest target, resolver and Resolution Report, report reaches plugins). Independent of spec A's add-ons.
- Evidence: the spec B grilling numbers (vtable-shift history per interface, fingerprint false and true rates, identical-body counts, RTTI class map) and scripts in `.scratch/incremental-hardening/research/specB/`; the full archive at `~/dev/other/cssharp-hardening-research/scratchpad/`.
- Sliced vertically into 11 tickets under `.scratch/hardening-b-boot-verify/issues/` (blockers first, each with a `Tier:` line):
  - Tier 0 standalone: 01 exact-name RTTI lookup.
  - Tier 1 base: 02 GameFrame verified at boot (tracer; fingerprint core, generator bootstrap mode, resolver interface) → 03 GameFrame heals a slot insert, and 04 generator carry-forward (03 and 04 in parallel).
  - Tier 2 add-ons (each needs 03 + 04): 05 ISource2Server rest, 06 IServerGameClients, 07 IVEngineServer2, 08 ICvar, 09 game-class virtuals (also needs spec A 12), 10 never-moved interfaces.
  - Tier 3 cleanup: 11 contract.
  - **Must set:** 01–05, 07, 08, 09. **Later:** 06, 10, 11.
  - External blockers: spec A 03 and 04 (for 02), spec A 05 (for 03), spec A 12 (for 09).
- The PoC resolver (`~/dev/cs2-0downtime-mod-stack-poc`) plans interface slot bindings too (its ticket 03); if it matures, it plugs in as a second resolver adapter and as an alternative generator.
