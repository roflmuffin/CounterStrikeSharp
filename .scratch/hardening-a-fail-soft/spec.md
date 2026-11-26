# Spec A: Fail soft per Engine Feature (I1, I2, I3)

Status: ready-for-agent

Part of the incremental hardening effort (`.scratch/incremental-hardening/`). Governed by ADR-0001 (incremental, upstream-shaped, fork PRs for now) and ADR-0002 (fail soft per Engine Feature). Vocabulary: `GLOSSARY.md`.

## Problem Statement

When a Game update moves or removes something a Binding points at, CounterStrikeSharp today fails in the worst available way, and nobody can tell what is broken:

- Signature scanning takes the first match. A signature that matches several places silently binds to the wrong function (ClientPrint bound to an unrelated constructor at 1.41.8.2; the follow-up fix lengthened it onto the same wrong function).
- One failed Binding cascades. Native init returns early on the first miss and silently skips unrelated Bindings after it; a missing CheckTransmit offset or CGameEventManager vtable aborts the whole CounterStrikeSharp load; a missing game-system signature segfaults the server (an offset is added before the null check); some resolved-or-null functions are later called without a check.
- On the managed side, one missing Gamedata key kills every helper in the static virtual-function table for the rest of the process; a memory function whose signature was not found is silently left null and crashes or throws a vague error on first call; a missing per-OS offset silently becomes 0.
- A missing schema class or field yields offset 0, so reads and writes hit the start of the object instead of failing.
- Failures are logged inconsistently (error, critical, engine warning, debug, or nothing), with no single place that says what is broken. Plugins have no way to ask whether something works.

For a server operator this means a routine Game update crashes the server or quietly breaks unrelated functionality for hours or days. For a tournament operator it means a match can run with a rule-enforcing plugin silently half-working.

## Solution

CounterStrikeSharp resolves every core Binding once at boot into a single immutable Resolution Report for the running build. Each Engine Feature declares the Bindings it needs; it is available exactly when all of them resolved. A failed Binding disables only the Engine Features that depend on it; the server stays up and everything else keeps working. Only loader-level failures stay fatal.

- A core signature must match exactly once; zero or several matches fail the Binding, with the match count in the report.
- One startup report lists available and disabled Engine Features, each disabled one with the failed Binding and the reason.
- Plugins can ask whether an Engine Feature is available and why not. Calling into a disabled Engine Feature throws a typed exception that names the feature and the failed Binding. Registering a hook or listener on a disabled Engine Feature succeeds, logs once, and never fires.
- A plugin can declare Required Engine Features; if any is disabled, that plugin refuses to load, loudly. Plugins that declare nothing behave as before.
- A missing schema field throws a typed exception on first access instead of reading at offset 0.
- Raw signatures passed in by plugins keep first-match behaviour (no breaking existing plugins) but warn when ambiguous, with an opt-in strict mode.

## User Stories

### Server operator

1. As a server operator, I want the server to keep running after a Game update breaks one Binding, so that one broken function does not take my whole server down.
2. As a server operator, I want a Game update that breaks the game-system signature to disable the affected Engine Feature instead of segfaulting, so that my server boots.
3. As a server operator, I want a missing CheckTransmit offset to disable only transmit control, so that CounterStrikeSharp and all other plugins still load.
4. As a server operator, I want a missing game-event-manager vtable to disable game events only, so that commands, chat and everything not event-based keep working.
5. As a server operator, I want one startup report listing every disabled Engine Feature with the Binding that failed and why, so that I can tell at a glance what a Game update broke.
6. As a server operator, I want the startup report to show the build it was resolved against, so that I can match it to the Game update that caused it.
7. As a server operator, I want an early native log line listing failed Bindings, so that I still see what is broken if .NET never starts.
8. As a server operator, I want ambiguous core signatures to be reported with their match count, so that I know a signature became unreliable rather than silently wrong.
9. As a server operator, I want consistent wording and severity for every Binding failure, so that I can grep and alert on one pattern.
10. As a server operator, I want plugins that cannot run safely to refuse to load with a clear message naming the missing Engine Feature, so that I am not running a half-working plugin unknowingly.
11. As a server operator, I want plugins that did not declare requirements to keep loading as before, so that this change does not break my existing setup.
12. As a server operator, I want invalid hex in Gamedata to fail that Binding with a clear reason, so that a typo does not produce a garbage pattern.

### Tournament / match operator

13. As a tournament operator, I want a match-integrity plugin that requires take-damage hooks to refuse to load when they are disabled, so that a match never runs without its rules enforced.
14. As a tournament operator, I want to see before a match which Engine Features are disabled on this build, so that I can decide whether to play on it.

### Plugin author

15. As a plugin author, I want to ask whether an Engine Feature is available, so that I can degrade gracefully or tell my users what is missing.
16. As a plugin author, I want the reason an Engine Feature is disabled (failed Binding, not found vs ambiguous vs missing from Gamedata), so that my error messages are actionable.
17. As a plugin author, I want calling a disabled Engine Feature to throw a typed exception naming the feature and Binding, so that I can catch it specifically instead of parsing "Invalid function pointer".
18. As a plugin author, I want registering a hook or listener on a disabled Engine Feature not to throw, so that my plugin still loads and the rest of it works.
19. As a plugin author, I want a single warning when I register on a disabled Engine Feature, so that I know my handler will never fire.
20. As a plugin author, I want to declare Required Engine Features on my plugin class, the same way I declare a minimum API version, so that the framework enforces it for me.
21. As a plugin author, I want Engine Feature names to be stable strings that never change meaning, so that my declarations and checks keep working across CounterStrikeSharp versions.
22. As a plugin author, I want a generated enum over the Engine Feature names, so that I get autocomplete and typo safety.
23. As a plugin author, I want one missing Gamedata key to affect only the helpers that use it, so that ClientPrint still works when TerminateRound broke.
24. As a plugin author, I want accessing a schema field that no longer exists to throw a typed exception naming the class and field, so that I never read or write garbage at offset 0.
25. As a plugin author, I want a missing schema field to be logged once, not on every access, so that logs stay readable.
26. As a plugin author, I want my own raw signatures to keep working exactly as today, so that this change does not break my plugin.
27. As a plugin author, I want a warning when my own raw signature matches more than once, with the match count, so that I learn it is fragile before it binds to the wrong function.
28. As a plugin author, I want an opt-in strict mode for my own signatures, so that an ambiguous match fails instead of picking the first one.
29. As a plugin author, I want a missing per-OS offset to be reported as missing, not silently treated as 0, so that I never call vtable slot 0 by accident.

### CounterStrikeSharp maintainer (fork)

30. As a maintainer, I want every core Binding declared once alongside the Engine Feature that needs it, so that I can see which functionality a Gamedata key backs.
31. As a maintainer, I want Gamedata to stay pure address data, so that it can be pushed and later regenerated without code knowledge.
32. As a maintainer, I want the Resolution Report to be a plain immutable value, so that availability, the startup report and plugin checks are all derived from one source of truth.
33. As a maintainer, I want Binding resolution to be testable against synthetic binary images without a server, so that uniqueness and failure handling are covered in CI.
34. As a maintainer, I want availability, the typed exception, the startup report and the Required Engine Feature gate to be testable from hand-built Resolution Reports, so that I don't need a server to test the policy.
35. As a maintainer, I want an end-to-end smoke run with deliberately broken Gamedata, so that I know the server really stays up and the report is right.
36. As a maintainer, I want the scanners to test every candidate position, including the last, so that a signature at the end of a segment is found.
37. As a maintainer, I want managed code to take core addresses from the Resolution Report instead of resolving signatures per call, so that core resolution happens once and consistently.
38. As a maintainer, I want adding an Engine Feature to be additive only (new name, never renaming or reusing one), so that plugins built against older versions keep working.
39. As a maintainer, I want the change shaped so it could go upstream as small PRs, so that we can upstream later without a rewrite.
40. As a maintainer, I want Windows to build and fail soft the same way as Linux for these changes, so that Windows operators get the same protection even though Linux is the tested platform.

## Implementation Decisions

### Prefactor (lands first, each a small PR to the fork)

- Hex parsing rejects invalid hex characters (today the error sentinel is compared against the wrong value, so invalid characters pass).
- Every scanner tests the final candidate position (today the last position is skipped).
- Managed Gamedata reports a missing per-OS offset as missing instead of defaulting to 0.
- Game-system init null-checks the resolved address before applying the instruction offset (today it adds 3 to null and dereferences).
- Natives that call the dispatch-spawn and legacy-game-event-listener functions null-check them.
- Separate, whenever: the managed vtable constructor that passes its arguments in swapped order.

### Bindings and the Resolution Report

- A **Binding declaration** names a Gamedata key, its kind (signature, offset, vtable via RTTI name, native schema field), and for signatures the module it is scanned in (honouring the Gamedata `library` field, which most consumers ignore today).
- A **Binding resolver** is one native module with a deep, narrow interface: given parsed Gamedata, the loaded module images and the Binding declarations, it returns a Resolution Report. Nothing else in the codebase scans for core signatures or reads core offsets directly.
- The **Resolution Report** is immutable and holds: the build id it was resolved against, and per Binding: resolved or failed; for resolved, the address or offset; for failed, a reason from a closed set (missing from Gamedata, missing for this OS, invalid pattern, not found, ambiguous with match count, RTTI vtable not found, schema class missing, schema field missing).
- Core signatures resolve strictly: exactly one match across the scanned segments, or the Binding fails as ambiguous. Uniqueness counts matches across all scanned segments of the module, using the on-disk segment bytes (so other plugins' patches do not create or hide matches).
- All core Gamedata keys are resolved at boot, including keys that only managed code consumes today. Bindings resolve once, at boot; there is no re-resolution in this spec.
- Native core code that depends on specific schema fields declares them as Bindings of its Engine Feature, so they are checked at boot (resolved after the schema system is available).

### Engine Features

- An **Engine Feature** is declared in code (not in Gamedata) as: a stable namespaced string name, and the Binding keys it needs. It is available exactly when every one of its Bindings resolved. Availability is a pure function of the Resolution Report and the Engine Feature catalog.
- Engine Feature names are permanent: never renamed or reused. Adding one is additive. A managed enum is generated over the names as a convenience; the string is the identity.
- Initial catalog (one per plugin-visible API; names and their Gamedata keys):

| Engine Feature | Bindings (Gamedata keys) |
|---|---|
| `cssharp.events/game-events` | CGameEventManager_Init, CGameEventManager RTTI vtable |
| `cssharp.events/fire-event-to-client` | LegacyGameEventListener |
| `cssharp.network/state-changed` | NetworkStateChanged, SetStateChanged |
| `cssharp.network/check-transmit` | ISource2GameEntities::CheckTransmit, CheckTransmitPlayerSlot |
| `cssharp.server/game-system-events` | IGameSystem_InitAllSystems_pFirst |
| `cssharp.chat/say-hooks` | Host_Say |
| `cssharp.chat/client-print` | ClientPrint, UTIL_ClientPrintAll |
| `cssharp.entity/output-hooks` | CEntityIOOutput_FireOutputInternal |
| `cssharp.entity/accept-input` | CEntityInstance_AcceptInput |
| `cssharp.entity/add-io-event` | CEntitySystem_AddEntityIOEvent |
| `cssharp.entity/emit-sound` | CBaseEntity_EmitSoundFilter |
| `cssharp.entity/dispatch-spawn` | CBaseEntity_DispatchSpawn |
| `cssharp.entity/create` | UTIL_CreateEntityByName |
| `cssharp.entity/remove` | UTIL_Remove |
| `cssharp.entity/set-model` | CBaseModelEntity_SetModel |
| `cssharp.entity/teleport` | CBaseEntity_Teleport |
| `cssharp.entity/take-damage-hooks` | CBaseEntity_TakeDamageOld, CBaseEntity_IsPlayerPawn |
| `cssharp.entity/take-damage-hit-group` | CTakeDamageInfo_HitGroup |
| `cssharp.entity/trigger-touch-hooks` | CBaseTrigger_StartTouch, CBaseTrigger_EndTouch |
| `cssharp.entity/skeleton-instance` | CGameSceneNode_GetSkeletonInstance |
| `cssharp.game-rules/terminate-round` | CCSGameRules_TerminateRound |
| `cssharp.game-rules/find-picker-entity` | CCSGameRules_FindPickerEntity |
| `cssharp.player/set-pawn` | CBasePlayerController_SetPawn |
| `cssharp.player/switch-team` | CCSPlayerController_SwitchTeam |
| `cssharp.player/change-team` | CCSPlayerController_ChangeTeam |
| `cssharp.player/respawn` | CCSPlayerController_Respawn |
| `cssharp.player/commit-suicide` | CBasePlayerPawn_CommitSuicide |
| `cssharp.player/post-think-hooks` | CCSPlayerPawnBase_PostThink |
| `cssharp.items/give-named-item` | GiveNamedItem, CCSPlayer_ItemServices_GiveNamedItem |
| `cssharp.items/can-acquire-hooks` | CCSPlayer_ItemServices_CanAcquire |
| `cssharp.items/weapon-can-use-hooks` | CCSPlayer_WeaponServices_CanUse |
| `cssharp.items/remove-player-item` | CBasePlayerPawn_RemovePlayerItem |
| `cssharp.items/drop-active-weapon` | CCSPlayer_ItemServices_DropActivePlayerWeapon |
| `cssharp.items/remove-weapons` | CCSPlayer_ItemServices_RemoveWeapons |
| `cssharp.items/weapon-data` | GetCSWeaponDataFromKey |
| `cssharp.nav/nav-mesh` | CCSNavArea_IsValidNavMesh |

  Keys are verified against current Gamedata (2026-11-24). "CGameEventManager RTTI vtable" is an RTTI-name Binding, not a Gamedata key. Gamedata keys are never renamed to fit this table.
- Native hook registration (vtable hooks by Gamedata index, function detours, the game-system registration) checks its Engine Feature first. A disabled Engine Feature skips registration instead of throwing out of plugin load.

### Fatal vs fail soft

- Fatal (CounterStrikeSharp refuses to load): Gamedata file missing or unparseable; a required engine interface missing; .NET failing to start.
- Everything else fails soft per ADR-0002, including the three cases that are fatal or crash today (CheckTransmit offset, CGameEventManager vtable, game-system signature).

### Native → managed boundary

- Native exposes the Resolution Report through one native call that returns it as data (per Binding: key, status, reason, match count, address/offset; plus build id). The managed side builds its immutable view from that once at startup.
- Native logs one compact line of failed Bindings at boot, before .NET starts.
- Managed owns: the Engine Feature catalog for managed-consumed Bindings, availability, the public query API, the typed exceptions, the startup report, and the Required Engine Feature gate. Native-owned Engine Features are reported by name in the same report; names match by string.
- Managed core helpers (the static virtual-function table and the per-type offset users) take addresses and offsets from the Resolution Report per member, lazily, so one failed Binding affects only its own helpers. Calling one whose Engine Feature is disabled throws the typed exception.

### Plugin-facing API (additive only)

- A query surface: is an Engine Feature available; why not (failed Binding and reason); list all with status.
- A typed exception for calling into a disabled Engine Feature, carrying the Engine Feature name, the failed Binding key and the reason.
- A plugin class attribute declaring Required Engine Features by name, checked at plugin load next to the existing minimum-API-version check, with the same style of clear refusal message.
- A typed exception for a missing schema field, carrying class and field name; logged once per field.
- Raw signatures passed by plugins: first match retained; a warning with the match count when ambiguous; an opt-in strict flag that fails on ambiguity instead.
- (Ticket 16) Plugin Signatures as a separate list next to the Engine Features (`EngineFeatures.GetPluginSignatures()`): plugin, signature or Gamedata key, module, match count, outcome.

### Schema fields (I3)

- Native schema lookup reports class-missing and field-missing explicitly instead of returning offset 0, and never dereferences a missing class.
- Managed schema access caches the explicit "missing" result and throws the typed exception on access; it never reads or writes at offset 0 for a missing field.
- (Ticket 16) Warn first: core.json `SchemaMissMode` is `warn` by default for now. A missing field logs one warning per field saying it will throw in a later release; the accessors return the type's default value (an empty string for strings) and drop writes. `throw` is the behaviour above. `Schema.GetSchemaOffset` throws in both modes, since its caller adds the offset to a pointer itself. A later ticket flips the default to `throw` after at least one release.

### Startup report

- One block after managed load: the build id, the count of available Engine Features, and for each disabled one its name, the failed Binding and the reason. Consistent wording and severity, replacing the scattered error/critical/warning/silent messages for core Binding failures.
- (Ticket 16) It says when Heal is off (`HealEnabled: false`; never on Windows, which has no Heal). With `FailMode: crash` and any core Engine Feature disabled, a fatal error naming them follows the block and the server stops (exit code 1) before plugins load. The stop native refuses unless FailMode is crash and a core Binding failed. Both config readers (native and managed) take the modes case-insensitively, ignore surrounding whitespace, and fall back to the default for that key only (with a warning) on an unknown value or wrong type.
- (Ticket 16) A separate "plugin signatures" block lists the Plugin Signatures not resolved to exactly one match (ambiguous, not found, invalid, or rejected under strict mode), with plugin and match count. It is logged once after all plugins have loaded and again after each plugin (re)load. A plugin's entries are replaced when it loads again and dropped when it is unloaded for good.

## Testing Decisions

- Good tests here assert external behaviour at the two seams, never internal structure: given these inputs, this Resolution Report; given this Resolution Report, this availability, this exception, this report text, this plugin load outcome.
- **Seam 1, producer side (native):** a new C++ test target (doctest, header-only; its own PR first) exercising the Binding resolver as a function: synthetic module images and Gamedata in, Resolution Report out. Cases: unique match; zero matches; two matches (ambiguous, with count); match at the final position; invalid hex; key missing; per-OS entry missing; offsets; module selection by library. An optional local fixture runs the resolver against an archived real server binary when present (never committed).
- **Seam 1, consumer side (managed):** existing unit test project (already in CI; can see internals; has Moq). Tests construct Resolution Report values by hand and assert availability, the query API, the typed exceptions, the startup report content, the Required Engine Feature load gate, lazy per-member core helpers, and plugin raw-signature warn/strict behaviour against a faked scan result. Prior art: the existing Admin, SteamID and Translation tests in that project.
- **Seam 2, end-to-end (laptop server):** boot a real server with Gamedata deliberately broken for a handful of keys (one missing, one ambiguous, one per-OS missing, the game-system signature, the CheckTransmit offset) and one missing schema field. Assert: server stays up; startup report lists exactly the expected disabled Engine Features with the right reasons; a test plugin requiring a disabled Engine Feature refuses to load; the native test suite passes for unaffected Engine Features. Prior art: the native test plugin and its smoke-test command that writes a results file.
- Schema-field misses are covered end-to-end only; no unit seam is added to managed schema access.

## Out of Scope

- Wrong-but-unique signature matches (per-Binding fingerprints or sanity checks): spec B.
- Boot-time RTTI vtable size checks and engine interface slots as Gamedata (I4, I5): spec B.
- Re-resolving Bindings at runtime after a Gamedata push; the signed, build-keyed updater; storing Gamedata per build id; CI per new build (I6, I7): spec C.
- The build archiver: its own small ticket, first in order, not part of this spec.
- Anchor-based auto-regeneration of Gamedata (T2).
- Hard-coded struct layouts, tier0/loader link breaks, the IGameSystem vtable we implement, schema renames.
- Windows-specific verification beyond "builds and fails soft the same way".
- Upstream PRs (fork PRs only for now).

## Further Notes

- Evidence for the problem statement and the expected payoff: `.scratch/incremental-hardening/research/report.html` and `handoff.md`.
- Sliced vertically into 15 tickets under `.scratch/hardening-a-fail-soft/issues/` (blockers first; each file lists its blockers). Shape: prefactors 01–03 → tracer 04 (native, builds the end-to-end harness) → tracer 05 (report reaches plugins) → 06 ClientPrint tracer, 07 Required Engine Features, migration batches 08–12, independent 13 (schema) and 14 (plugin signatures) → 15 contract. Drive 04–07 with `/implement` per ticket (they set the patterns); 08–15 suit `/implement-spec`. The build archiver is separate: `.scratch/build-archiver/issues/`.
- **Tiers** (each ticket carries a `Tier:` line):
  - Tier 0 standalone (no base needed): 01, 02, plus the build archiver.
  - Tier 1 base (required by every add-on and by specs B and C): 03, 04, 05.
  - Tier 2 add-ons (each needs only the base; any subset, any order; 12 follows 06's pattern): 06, 07, 08, 09, 10, 11, 12, 13, 14.
  - Tier 3 cleanup (after 08–12; deferrable, leaves two resolution paths until done): 15.
  - **Must set** (~80% of spec A's value): 01, 02, archiver, 03, 04, 05, 06, 12, 07, 09. **Later**: 08, 11, 13, 10, 14, then 15. Spec B needs only the base.
- **Follow-up (ticket 16, after the upstream-note alignment):**
  - operator `FailMode: disable | crash` (default `disable`) and `HealEnabled` (spec B Heal; default `true`);
  - schema misses warn first by default (`SchemaMissMode: warn | throw`) instead of throwing straight away;
  - plugin signatures that are ambiguous or not found get their own block in the report.

  This supersedes the "throws" wording under Schema fields until the default flips to `throw`.
- Unused Gamedata keys found during the survey (two offsets and one signature with no consumer) are left alone; the registry simply does not declare them.
