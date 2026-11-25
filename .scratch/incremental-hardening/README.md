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

## Decisions (spec B grilling round 1)

- Scope: I4 Vtable Checks for every class we hook or call; I5 Slot Bindings for the 22 hooks plus direct calls on IVEngineServer2 (~20) and ICvar (~14), the interfaces that actually moved (7 and 3 incidents). Never-moved interfaces stay compiled, guarded by the Vtable Check. Out: IGameSystem impl, struct layouts, Windows.
- Vtable Check policy: per-slot fingerprint with ±1 neighbour window (immediates/RIP masked); size change is a warning only. Evidence: 0.86% false flags over all slots, 2.6% on our 34 slots, 6/6 real moves caught.
- Generator carries Slot Bindings forward by vtable alignment from today's known-good values, including existing entity vfunc offsets (Respawn etc. moved 15×). Signatures stay manual (T2). Slot regeneration thus moves from T2 into T1.
- Vocabulary: Slot Binding, Vtable Check.
- Research for spec B: `research/specB/` scripts; numbers in the grilling transcript (vtable-shift history per interface, fingerprint false/true rates).

## Decisions (spec B grilling round 2)

- Build B's Vtable Check, Heal and generator in CounterStrikeSharp now, not on the PoC resolver; resolution sits behind a swappable seam (runtime resolver interface → Resolution Report; offline generator → Gamedata shape) so the PoC resolver can drop in later. ADR-0004.
- GameFrame: no detour. All Slot Bindings Heal at boot on an exactly-unique fingerprint window match (heals all 10 ISource2Server hooks at 1.42.0.1). ADR-0003. Replaces round-1 Q25.
- One C++ fingerprint/alignment core shared by the boot check and a generator CLI; Python research scripts become a test oracle. ADR-0004.
- Wrong-but-unique signatures → T2 (needs a second anchor). ADR-0001 amended.
- Build archiver: one, owned by the PoC repo, Linux + Windows, shared folder `~/dev/cs2-builds/<build-id>/`; our ticket closed as duplicate.
- Windows Slot Bindings from hl2sdk headers, marked unverified; no Vtable Check or Heal on Windows yet.
- Gamedata shape changes are additive: existing offset entries gain optional class / fingerprint window / module fields; interface Slot Bindings are new keys of the same shape.

## Spec A tiers

- Must: 01, 02 (standalone; build archiver now lives in the PoC repo) → 03, 04, 05 (base) → 06, 12, 07, 09 (add-ons).
- Later: 08, 11, 13, 10, 14 (add-ons) → 15 (cleanup).
- Spec B can start once the base (03–05) is done.

## Spec B tiers

- Must: 01 (standalone) → 02, 03, 04 (base) → 05, 07, 08, 09 (add-ons).
- Later: 06, 10 (add-ons) → 11 (cleanup).
- Needs spec A base (03–05); 09 also needs spec A 12.

## Decisions (spec D grilling, own loader, 2026-11-25)

- Goal: fewer unneeded dependencies + independence from Metamod-only failures. Not outage hours: a standalone CSSharp shared the root cause in 7/9 Metamod load failures (≤8% of 162 h). Evidence: `research/own-loader/findings.md`. ADR-0005 (amends ADR-0001).
- Two modes, one package: Plugin Mode stays default on both OSes (unchanged, Metamod's KHook); Proxy Mode opt-in via one gameinfo line after Metamod's (ordering A: Metamod loads the Proxy as its game DLL). Lost line → automatic fallback to Plugin Mode.
- Thin Metamod Bootstrap is the only code with Metamod headers; in Proxy Mode it only hands ISmmAPI over (MetaFactory). Host seam: Metamod host / own host.
- KHook: Metamod's in Plugin Mode; own static, hidden-symbol, never-freed in Proxy Mode (spike first). CSSharp hooks outer next to Metamod plugins; documented.
- Proxy: no SDK/tier0 imports, absolute paths, pure pass-through, slots 0/3/5 compare-before-restore, no byte sigs, ABI-matched core, fail open; `-css_fail_closed` launch param.
- Pristine-copy sig scanning in both modes (shared with spec A I1). Own level lifecycle with event-order parity. OnMetamodAllPluginsLoaded always fires + OnAllPluginsLoaded alias. Read-only boot checker; no gameinfo auto-repair.
- Linux first; Windows Proxy should follow (not must), Windows never worse than today. Promote Proxy Mode in docs after 3 updates/OS without a Proxy fix.
- Non-goals: tier0/slot/struct decoupling, removing Metamod host or submodule, hosting/chain-loading Metamod, server_valve (spike), pinning Metamod, hours target.
- Upstream RFC drafted (`hardening-d-own-loader/rfc.md`); posting is a human ticket (ADR-0001: fork PRs for now).

## Spec D tiers

- Must: 01, 02 (standalone) → 03, 04, 05 (base) → 06, 07, 08, 09, 10, 12 (add-ons) → 11 (coexistence validation, human).
- Later: 13 Windows Proxy (should-follow), 14 CI jobs (needs spec C I7), 15 server_valve spike, 16 post RFC (human), 17 promote Proxy Mode (human).
- Independent of specs A/B except 01, which spec A's resolver shares.

## Backlog (not ticketed)

- Managed VTable constructor passes symbol name and binary path in swapped order (found during the code survey; unrelated to hardening).

## Plan of record

1. `/grill-with-docs`: decisions that span all items (T1 vs T1+T2, capability API, I1 migration, CI host, order). Writes `GLOSSARY.md` + `docs/adr/`.
2. Three specs, each `/to-spec` → `/to-tickets`:
   - `.scratch/hardening-a-fail-soft/`: I1 sig uniqueness, I3 schema miss → error, I2 per-feature failure + capability flags.
   - `.scratch/hardening-b-boot-verify/`: I4 Vtable Checks with Heal, I5 Slot Bindings (hooks + IVEngineServer2/ICvar calls + entity offsets), generator.
   - `.scratch/hardening-c-fast-fix/`: I6 signed build-keyed updater, I7 CI per build.
   - `.scratch/hardening-d-own-loader/`: opt-in Proxy Mode (own loader) under Metamod (ADR-0005).
3. `/implement-spec` or `/implement` per ticket.
