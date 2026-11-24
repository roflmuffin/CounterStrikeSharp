# Fail soft per Engine Feature

When a Binding fails to resolve (missing, not found, or ambiguous), CounterStrikeSharp disables only the Engine Features that depend on it, keeps loading, and reports it. Only loader-level failures stay fatal: Gamedata missing or unparseable, a required engine interface missing, or .NET failing to start. We chose this over fail-fast because a Game update usually breaks one or two Bindings, and today that crashes the server or kills unrelated functionality for hours or days.

- Which Bindings an Engine Feature needs is declared in code, not in Gamedata, so Gamedata stays pure address data (and regenerable later).
- Plugins can query availability (`IsAvailable` plus the reason); calling into a disabled Engine Feature throws a typed exception naming the feature and the failed Binding; registering a hook or listener on one succeeds, logs once, and never fires.
- One aggregated startup report lists available and disabled Engine Features.
- A signature in core Gamedata must match exactly once, or its Binding fails. Raw signatures passed in by plugins keep first-match behaviour with a warning, plus an opt-in strict mode (ADR-0001: no breaking existing plugins).

## Considered Options

- **Fail fast** (refuse to load on any miss): honest, but turns every minor Game update into a full outage.
- **Silent no-op** for disabled Engine Features: hides broken behaviour (e.g. a round that never terminates).
