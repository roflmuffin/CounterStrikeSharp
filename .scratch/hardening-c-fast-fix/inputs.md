# Spec C inputs (not a spec yet)

Collected before `/to-spec` so nothing is lost. Spec C = I6 (signed, build-keyed gamedata updater on both OSes) + I7 (CI that boots each new build). See `.scratch/incremental-hardening/README.md`.

## Must cover

- **Kill switch.** An empty or blank binding in build-keyed gamedata disables that binding's Engine Feature on that build, through spec A's fail-soft path. The backtest assumes it: it is what turns `IGameSystem` (G) and semantic-change (M) crashes into "feature off" in the A–C numbers. The upstream note states it under C2 and under "What it does not fix" (`IGameSystem`). Source: upstream-note alignment, row 8, 2026-11-25.
- **Linux updater is new behaviour.** The updater has been Windows-only since 1f30e561 (Aug 2025); `src/mm_plugin.cpp` puts it behind `#ifdef _WIN32`. Turning it on for Linux needs the maintainer's OK. Decide whether it is on by default or opt-in on Linux, and who holds the signing key. The upstream note lists this as an open question.
- **Gamedata per build id.** Store gamedata per build id and never overwrite in place (README, grilling round 4).
- **Runtime re-resolve.** Decide whether a gamedata push re-resolves Bindings at runtime (swap in a whole new Resolution Report, ADR-0002) or only at the next boot (spec A, out of scope).
- **Upstream gamedata without the new fields.** The updater (`AutoUpdateEnabled`, default true) can overwrite gamedata.json with upstream's, which lacks the Slot Binding fields. Slot Bindings without a window must degrade to "unverified, gamedata index", not "failed" (spec B grilling).
- **C4 scope.** CI runs spec B's generator (slot window match) on each new build and opens a gamedata PR. A human merges it. Signature re-derivation is not part of C; it goes with auto-publish (T2, the note's C5).
- **CI host.** This laptop as a stopgap (README); the upstream note says C3 needs a named owner.
