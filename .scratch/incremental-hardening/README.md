# Incremental hardening (T1: I1–I7)

Planning workspace for making CounterStrikeSharp survive CS2 game updates with fewer and smaller breakages.

- `handoff.md`: research summary, T1 package, open questions. Start here.
- `research/`: scripts and text evidence (report.html, gamedata history, backtest models, RTTI tooling).
- Full archive (binaries, old build images, cloned repos; 6.2G, outside git): `~/dev/other/cssharp-hardening-research/scratchpad/`.
- Live report: https://claude.ai/artifact/CkhPT5n5hbWJyW3SZcy7y4

## Plan of record

1. `/grill-with-docs`: decisions that span all items (T1 vs T1+T2, capability API, I1 migration, CI host, order). Writes `GLOSSARY.md` + `docs/adr/`.
2. Three specs, each `/to-spec` → `/to-tickets`:
   - `.scratch/hardening-a-fail-soft/`: I1 sig uniqueness, I3 schema miss → error, I2 per-feature failure + capability flags.
   - `.scratch/hardening-b-boot-verify/`: I4 RTTI vtable checks, I5 interface slots as gamedata.
   - `.scratch/hardening-c-fast-fix/`: I6 signed build-keyed updater, I7 CI per build.
3. `/implement-spec` or `/implement` per ticket.
