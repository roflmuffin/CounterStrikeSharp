# 15: Contract: one resolver, one report

Spec: `.scratch/hardening-a-fail-soft/spec.md`. Contract step of the migration.

**What to build:** Remove the old paths now that every core Binding goes through the resolver: no core Gamedata key is resolved anywhere else, the scattered per-site failure logs (mixed error/critical/warning/silent) are gone, and the startup report plus the early native line are the only reports of Binding failures. Fatal is limited to Gamedata missing/unparseable, a required engine interface missing, or .NET failing to start.

**Blocked by:** 08, 09, 10, 11, 12

**Status:** resolved

**Tier:** 3 cleanup · later

- [ ] No core Gamedata lookup remains outside the resolver (plugin-supplied signatures excepted)
- [ ] Every Engine Feature in the spec's catalog is declared and reported
- [ ] The full end-to-end harness passes: every broken-key case leaves the server up with exactly the expected Engine Features disabled, and the native test suite passes for everything else
- [ ] With valid Gamedata, the native test suite passes and the startup report shows everything available

## Comments

- Resolved on `hardening/spec-a` (merge of `spec-a/15-contract-one-resolver-one-report`). `CGameConfig` only parses; no core lookup outside the resolver; native gate lines debug-only; one wording `<feature> disabled (<Key>: <reason>)`; .NET start failure fatal (`InitGameSystems` moved after .NET init); real-binary fixture skips SchemaField rows. doctest 39 + managed 156/156. Full harness: all case scripts in `eng/e2e/cases/` incl. new `fatal.sh` pass (20 runs, server up in all, exact disabled counts asserted).
