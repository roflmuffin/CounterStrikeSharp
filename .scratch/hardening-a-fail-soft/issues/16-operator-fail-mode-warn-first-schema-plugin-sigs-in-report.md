# 16: Operator fail mode, warn-first schema misses, plugin signatures in the report

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Plugin-facing API, Schema fields, Startup report). ADR-0001 (warn-first or opt-in for stricter behaviour), ADR-0002 (amended: operator fail mode). Follow-up from aligning the plan with the upstream note (steps A2, A3, A4 and B3 there).

**What to build:** Three changes to spec A as implemented, plus one switch for spec B's Heal. All of them are additive config or report changes. Defaults keep today's fork behaviour, except the schema miss, which warns in its first release.

1. **Operator fail mode (A2).** A new core config setting `FailMode`: `disable` (default, today's fail soft) or `crash`. With `crash`, if any core Engine Feature is disabled after boot resolution, CounterStrikeSharp logs the startup report and then stops the server with a fatal error naming the disabled Engine Features and their failed Bindings. This is for competitive and anti-cheat servers that would rather not run than run with a hook missing. Healed Bindings count as available. Plugin signatures and runtime schema misses never trigger it. The per-plugin Required Engine Features gate (07) is unchanged; `FailMode` is the server-wide counterpart.
2. **Heal switch (B3).** A new core config setting `HealEnabled` (default `true`). With `false`, a Slot Binding whose window does not match at its stated index fails as a fingerprint mismatch instead of healing, and `FailMode` decides what happens next. The startup report says that healing is off. This covers the upstream note's promise that the operator setting can turn healing off.
3. **Warn-first schema misses (A4).** A new core config setting `SchemaMissMode`: `warn` (default for now) or `throw` (today's ticket-13 behaviour). In `warn` mode, a missing field is logged once per field as a warning that says it will throw in a later release. Reads return the type's default value and writes are dropped. A missing field still never reads or writes at offset 0. A later ticket flips the default to `throw` after at least one release.
4. **Plugin signatures in the report (A3).** Signatures that plugins resolve (memory functions, plugin-shipped Gamedata, the strict and non-strict paths from 14) record one entry each: plugin, signature or key, module, match count and outcome (resolved, ambiguous with first match taken, ambiguous and failed under strict mode, not found). A "plugin signatures" block, listing only those that are ambiguous or not found, is logged once after all plugins have loaded and again on each plugin (re)load. Plugins can read the same entries through the existing Engine Feature query API, as a separate list next to the Engine Features. Plugin signatures never disable core Engine Features.

**Blocked by:** 05, 07, 13, 14 (all resolved); spec B 03 for the heal switch (resolved on `hardening/impl`)

**Status:** ready-for-agent

**Tier:** 2 add-on · must

- [ ] `FailMode` defaults to `disable`; existing configs without the key behave exactly as today
- [ ] With `FailMode: crash` and one broken core Binding, the startup report is logged, then the server stops with a fatal error naming the Engine Feature and Binding (end-to-end, broken `Host_Say`)
- [ ] With `FailMode: crash` and only healed Bindings, the server keeps running (end-to-end, ISource2Server one slot off)
- [ ] With `HealEnabled: false` and ISource2Server one slot off, the affected Engine Features fail as fingerprint mismatch; the startup report says that healing is off
- [ ] With `SchemaMissMode: warn`, accessing a missing field logs one warning per field, reads return the default value, writes are dropped, and offset 0 is never touched; with `throw`, ticket 13's tests pass unchanged
- [ ] An ambiguous plugin signature and a missing one both appear in the plugin-signatures block with plugin name and match count; unique ones do not; the block reappears after a hot reload
- [ ] Plugins can list plugin-signature entries through the query API
- [ ] Managed unit tests for config parsing and defaults, the warn-mode accessors, and plugin-signature report formatting
- [ ] Windows behaves the same, except that `HealEnabled` has no effect there (no Heal on Windows)
