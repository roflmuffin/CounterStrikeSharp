# Incremental hardening (T1: I1–I7)

Planning workspace for making CounterStrikeSharp survive CS2 game updates with fewer and smaller breakages.

- `handoff.md`: research summary, T1 package, open questions. Start here.
- `research/`: scripts and text evidence (report.html, gamedata history, backtest models, RTTI tooling).
- Full archive (binaries, old build images, cloned repos; 6.2G, outside git): `~/dev/other/cssharp-hardening-research/scratchpad/`.
- Live report: https://claude.ai/artifact/CkhPT5n5hbWJyW3SZcy7y4

## Decisions (grilling round 1, 2026-11-24)

- Scope: T1 only (I1–I7); T2 auto-regen later, separately. See ADR-0001.
- Fork-first, every change upstream-shaped, plugin API additive. See ADR-0001.
- Linux first-class + tested; Windows must build and fail soft; Windows I4/I5 are later tickets.
- Order: build archiver first, then spec A, then B, then C.
- CI host: this laptop for now (stopgap).
- Vocabulary: Binding, Engine Feature (see GLOSSARY.md).

## Decisions (grilling round 2)

- Build archiver: laptop as stopgap too (accept gaps while it's off); binaries only (server, engine2, tier0; Linux + Windows), keyed by build id.
- Fail-soft policy, feature registry in code, typed exception, `IsAvailable`, startup report, strict core sigs / warn-only plugin sigs: ADR-0002.
- Wrong-but-unique sig detection (ClientPrint #1494) → spec B, not A.
- Prefactor PRs before spec A: HexToByte invalid-hex, scanner last-position off-by-one, managed missing per-OS offset → 0, InitGameSystems +3-before-null, unchecked DispatchSpawn / LegacyGameEventListener. VTable ctor arg swap: separate, whenever.
- No upstream PRs for now; all PRs target the fork (code stays upstream-shaped).

## Decisions (grilling round 3) — spec A design

- Engine Features fine-grained: one per plugin-visible API (~30). Draft catalog in the round-3 table of the grilling session; spec A carries the final list.
- Schema fields are Bindings but not in the Engine Feature catalog: managed throws a typed exception on first access to a missing field (logs once); native core code declares the fields it relies on as Bindings of its Engine Feature (checked at boot).
- Bindings resolve at boot only in spec A; runtime re-resolve after a Gamedata push is spec C's question.
- Tests: (1) new C++ doctest target (own PR first): uniqueness + registry on synthetic buffers, optional local archived libserver.so fixture; (2) managed API.Tests: IsAvailable, typed exception, startup report via fake resolver; (3) laptop live smoke with deliberately broken Gamedata keys.
- Managed owns the public EngineFeature enum, IsAvailable and the merged startup report; native exposes Binding/feature state via one native call and logs one early line in case .NET never starts; names matched by string.

## Decisions (grilling round 4: values over places)

- Resolution Report: one immutable value per build; availability = pure function of it; reload swaps the whole value. ADR-0002.
- Engine Feature identity = stable namespaced string (`cssharp.entity/take-damage-hooks`); enum generated as convenience. ADR-0002.
- Plugins may declare Required Engine Features; missing → that plugin refuses to load. ADR-0002.
- Spec C: Gamedata stored per build id (accumulate, never overwrite in place); design there.

## Plan of record

1. `/grill-with-docs`: decisions that span all items (T1 vs T1+T2, capability API, I1 migration, CI host, order). Writes `GLOSSARY.md` + `docs/adr/`.
2. Three specs, each `/to-spec` → `/to-tickets`:
   - `.scratch/hardening-a-fail-soft/`: I1 sig uniqueness, I3 schema miss → error, I2 per-feature failure + capability flags.
   - `.scratch/hardening-b-boot-verify/`: I4 RTTI vtable checks, I5 interface slots as gamedata.
   - `.scratch/hardening-c-fast-fix/`: I6 signed build-keyed updater, I7 CI per build.
3. `/implement-spec` or `/implement` per ticket.
