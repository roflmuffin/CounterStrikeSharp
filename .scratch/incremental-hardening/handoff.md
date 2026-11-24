# Handoff: plan the incremental CounterStrikeSharp hardening changes

Repo: `/home/markus/dev/other/CounterStrikeSharp` (main @ 0485c767, clean; nothing was changed in the repo).
Date: 2026-11-24. The previous session was research only.

## Goal of the next session
Write implementation plans for the "incremental CSSharp" package (T1 below), one plan per change, ordered by value and effort. Research is done. Planning only; don't implement unless the user asks.

## Source of truth (read first)
- **Report (live artifact):** https://claude.ai/artifact/CkhPT5n5hbWJyW3SZcy7y4. Local file: `research/report.html` (full archive: `~/dev/other/cssharp-hardening-research/scratchpad/`). Most relevant sections:
  - "A more resilient toolkit": collapsed "Design details…", including "Without starting over".
  - "Backtest": churn chart, scenario details, empirical check, degraded-time breakdown, "What T1 still misses".
  - "Every stack compared": collapsed "Seven gaps in the best stacks", the code audit, and patterns to borrow.
  - "Adversarial review": corrected claims.
- **Raw evidence:** subdirs `A`–`H`, `R` : scripts and text in `research/` (this dir); everything incl. binaries, build images, cloned repos (6.2G) in `~/dev/other/cssharp-hardening-research/scratchpad/` (outside git). Key files:
  - `A/gamedata_history.json`: every gamedata change.
  - `A/clusters.txt`
  - `G1/model.py` + `G1/report.txt`: incident backtest model.
  - `G2/scripts/` + `G2/cache/final.txt`: empirical anchor-resolver backtest, plus the i64 → binary extractor.
  - `H/degrade.py`: feature-group degraded hours.
  - `C/scripts/`: RTTI/vtable/sig tooling.
  - `R/elfvt.py`, `R/fpcmp.py`: independent RTTI parser and slot fingerprinting.
- **Memory:** `~/.claude/projects/-home-markus-dev-other-CounterStrikeSharp/memory/` holds a report-style preference (high-level first, details collapsed).

## The T1 package to plan (I1–I7)
Line refs come from research agents; re-verify them before planning.
1. **I1 uniqueness check in signature scanning.**
   - Today `FindSignature` returns the first match without a uniqueness check.
   - Evidence: the ClientPrint sig silently re-bound to a `CNavBFSLattice` ctor at 1.41.8.2. The 1.41.9.7 "fix" lengthened it onto the same wrong function (#1494). TerminateRound was ambiguous one update before it broke.
   - I1 alone doesn't catch a wrong-but-unique match. Consider a caller-set or similarity sanity check per sig.
   - Cost: sigs that work today only by luck will fail loudly.
2. **I2 per-feature failure instead of cascades.**
   - Native init returns on the first missing sig: `src/core/globals.cpp:129-157`, `src/core/managers/entity_manager.cpp:63-110`. `game_system.cpp:41` computes `ResolveSignature()+3` before the null check.
   - Managed side: `BaseMemoryFunction` already catches sig failures, but it leaves a null function that crashes on first call. A missing gamedata key throws in the `VirtualFunctions` static initializer.
   - Goal: each binding backs a feature; a miss disables that feature and logs it. Expose availability to plugins (capability flags), since no stack has them.
3. **I3 missing schema field → explicit error, never offset 0** (`schema.cpp:174,185`, `managed/.../Schema.cs:61-76`).
4. **I4 boot-time RTTI vtable checks.**
   - Compare the vtable size of each class or interface used against the expected size, and disable the affected hooks or calls on mismatch.
   - Model: s2script `s2script_mm.cpp:3671-3830`, whose `<Iface>_vtableSize` pins gate hooks to -1.
   - Also verify that gamedata vfunc offsets fall inside the RTTI vtable.
   - CSSharp already has FindVirtualTable (0eb73eb3).
   - Weakness: an insert plus a removal in the same vtable passes a count-only check. Consider per-slot fingerprints for the hooked slots.
5. **I5 engine-interface slots out of compiled headers, into gamedata.**
   - Today there are 22 KHook vtable hooks and 46 direct interface vcalls compiled from s2sdk headers. The ISource2Server +1 virtual in build 26879581 / 1.42.0.1 causes #1502 / #1500, still open upstream on s2sdk PR #128. A community 8-byte binary patch works around it.
   - Plan: slot indices as data, verified by I4; GameFrame via a detour of a function resolved by refs (ModSharp pattern: cvar `sv_early_network_message_processing` ∩ game-system class ∩ `CSource2Server` membership).
   - Caveat: 25–30% of interface slots have identical bodies (`xor eax,eax; ret` etc.). Detour only unique bodies, and use neighbour alignment that ignores line-number strings.
   - **Limitation:** IGameSystem, which CSSharp *implements*, isn't covered by I5. It caused 8 incidents.
6. **I6 signed, build-keyed gamedata updater on both OSes.**
   - Today `src/core/gameconfig_updater.cpp` downloads over plain HTTP (TLS removed in ffb274d6), has no signature, isn't keyed to the build, and is Windows-only (`mm_plugin.cpp:140-150`, since 1f30e561).
   - With I2 this works as a kill switch: blanking a binding disables only its feature.
7. **I7 CI on every new build.**
   - Watch Steam (push/PICS preferred; s2script's 5-min cron missed a build), boot a headless DS, run the ~180 native xUnit tests (currently manual only).
   - Anonymous Steam serves only the *current* manifest, so archive every build as it ships. Old builds are recoverable only via third-party IDA dumps (`G2/scripts/i64extract.py`).

## Expected payoff (model, 39 incidents Nov 2023–Nov 2026; numbers are in the report)
- Releases −62%, fix commits −73%, follow-ups −84%.
- Hard outage 606 h → 329 h.
- Effective outage (crash or core feature down) −27%; the ideal stack reaches −35%.
- 1.42.0.1 becomes a non-event under T1.
- T1 delivers about 77–91% of the ideal stack's release reduction and about 46% of its outage reduction.
- Biggest remaining lever: most degraded hours come from slow human gamedata fixes (7 incidents over 100 h each = 60%). Pair T1 with auto-regeneration (T2) or out-of-timezone maintainers.
- T2 evidence (n=16, Sep–Nov 2026): a two-anchor rule auto-healed 6/10 broken sigs and 6/6 moved offsets with 0 wrong. A single anchor was 20% silently wrong.

## Not covered by T1 (out of scope unless the user widens it)
- tier0 / loader link breaks (Armory, Jul 2025, Jul 2026).
- Hard-coded struct layouts: native `CCheckTransmitInfoHack`, `CNetworkStateChangedInfo`, `EmitSound_t`; managed `EntitySystem.cs`, `CEntityIOOutput.cs` (13 raw offsets), `CPlayerPawnComponent.cs`, `CBaseEntity.cs:91`, `CTakeDamageInfo.cs`.
- IGameSystem vtable.
- SDK refactors.
- Schema renames.
- Semantic changes: 8 incidents, irreducible.
- Windows/MSVC behaviour is untested; all binary analysis was Linux-only.

## Open questions for the user
- Scope: T1 only, or T1 + T2 (anchor-based auto-regen tooling)?
- Upstream to roflmuffin/CounterStrikeSharp, or a fork?
- Plugin-facing API changes: are capability flags acceptable? Does I1 strictness need a migration period?
- CI infra: where would a headless CS2 DS run? It needs Steam anonymous access and roughly 40 GB.

## Suggested skills
- `codebase-design`: seams for a binding/resolver module and the capability API.
- `domain-modeling`: glossary for binding, anchor, capability; ADRs per decision (e.g. slots-as-data).
- `explain`: walk the current native init and managed memory-function paths before planning.
- `tdd`: I1–I4 are unit-testable with fixture binaries or images.
- `grilling`: stress-test the plan ordering and the scope questions above.
- Later: `pr`, `commit`, `code-review`.
