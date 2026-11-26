# 01: Investigate tier0 imports and hard-coded struct layouts

Effort: possible spec E (tier0 resolved at runtime + struct layouts checked at boot). Not specced; this ticket decides whether to spec it. Origin: upstream-note alignment, Q2 (2026-11-25). In the note it is step D, badged "consider", with both parts (tier0, struct layouts) marked "investigate".

Type: research

**Status:** ready-for-agent

**Tier:** 0 standalone · must (decides spec E)

**Blocked by:** None

## Why

After A–C, 329 outage hours remain over 7 incidents. All of them come from tier0 link breaks (L, 4 incidents) and hard-coded struct layouts (T, 6 incidents). Hand estimate from `G1/model.py` + `own-loader/findings.md`:

| On top of A–C | Outage left |
|---|---|
| tier0 at runtime only | ~288 h |
| struct checks only | ~265 h |
| both | ~147 h (Metamod's own outage is the floor while loaded through Metamod) |
| both + D (Proxy Mode) | near 0 |

This is the biggest lever left, and the only thing that makes D move hours. It is unmodelled and unsized.

## Questions to answer (Linux first)

1. **tier0 import inventory.** List every undefined symbol `libcounterstrikesharp.so` imports from `libtier0.so` (`nm -D`), at the current release and at the releases before each L incident (Feb 2024, Armory Oct 2024, Jul 2025, Apr 2026, Jul 2026). Attribute each one to its origin:
   - our own code;
   - a static SDK library (tier1, interfaces, mathlib, …);
   - inline SDK header code (templates and constructors that reach tier0 globals, e.g. `CUtlStringToken` → `g_bUpdateStringTokenDatabase`).
2. **Per symbol, the cheapest way out:**
   - `dlsym` a C export with a fallback;
   - replace it with a minimal own type (ModSharp's approach);
   - drop the static library;
   - unavoidable.

   Count how many remain unavoidable.
3. **Replay.** Across the 11 SDK `libtier0.so` snapshots (~1,360 exports removed or re-mangled), how many of the documented CounterStrikeSharp load failures would the result of question 2 have avoided?
4. **Struct layout inventory.** Enumerate every struct layout CounterStrikeSharp relies on outside the schema system:
   - `CEntityInstance`;
   - `ConVarData` and ConVar;
   - Utl containers;
   - `CGlobalVarsBase`;
   - KeyValues3;
   - server-side client classes and `INetworkGameServer`;
   - any others the code uses.

   For each one:
   - how often it changed in the ~12 T incidents;
   - which plugin-visible APIs or core features depend on it;
   - whether it can be checked at boot: a size via RTTI or allocator, a field via schema or a known accessor's disassembly, or a fingerprint of a function that reads it.
5. **Re-run the estimate.** Add "tier0 at runtime", "struct checks" and "both" scenarios (with and without D) to the backtest (`specsplit/split.py` or `G1/model.py`), replacing the hand estimate above. Keep the assumption about M/G kill-switch softening explicit.
6. **Windows.** Note what the same imports look like on Windows (`server.dll` imports, Metamod #279: a static tier0 import fails `LoadLibrary` for the whole DLL). Linux-only analysis is fine; say what is unknown.

## Deliverable

- A findings file next to this ticket: the inventories, the per-symbol answers, the replay result and the re-run backtest numbers.
- A recommendation: spec E yes or no. If yes, a ticket list with size (S/M/L/XL) and risk (low/med/high, naming what makes it risky).
- One line for the upstream note's E card to replace "estimated, not modelled".

## Pointers

- Backtest: `.scratch/incremental-hardening/research/G1/model.py`, `specsplit/split.py`.
- Metamod overlap per incident: `.scratch/incremental-hardening/research/own-loader/findings.md` §1 (S2 column = own loader + `dlsym` tier0 + slots as data).
- Archived builds: `~/dev/cs2-builds/<build-id>/`; full research archive: `~/dev/other/cssharp-hardening-research/scratchpad/`.
- SDK history: the hl2sdk-cs2 / s2sdk commit history used for "42 forced SDK changes" in the internal report.
- Non-goals here: implementing anything; changing D's scope.
