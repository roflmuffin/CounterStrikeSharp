# Incremental hardening, shaped for upstream

To make CounterStrikeSharp survive Game updates better, we do the incremental package (T1: uniqueness checks, per-Engine-Feature failure, schema errors, boot-time vtable checks, engine slots as Gamedata, signed build-keyed updater, CI per build) and not the "ideal stack" rewrite (own loader, no compiled engine headers, extracted struct layouts). Anchor-based auto-regeneration (T2) is a separate later effort.

Work happens in the markus-wa fork, but every change is shaped as an upstreamable PR to roflmuffin/CounterStrikeSharp: one item per PR, no dependency on fork-only code, and plugin-facing API changes are additive only (existing plugins keep working).

For now no upstream PRs are opened; PRs target the fork. The upstream-shaped constraint stays so that upstreaming remains an option.

## Considered Options

- **Ideal stack rewrite**: modelled as turning ~110 more outage hours/year into degraded time, but it is a rewrite that cannot be upstreamed incrementally. T1 gets ~75–90% of its release reduction and about half its outage reduction (see `.scratch/incremental-hardening/research/report.html`).
- **Fork-only**: free to break the plugin API, but upstream churn (fork was 153 commits behind at the start) makes a divergent fork expensive.

## Consequences

- Strictness that would break existing plugins (e.g. rejecting ambiguous plugin-supplied signatures) must be opt-in or warn-first.
- T1 leaves loader/tier0 link breaks, hard-coded struct layouts, and the IGameSystem vtable uncovered.
