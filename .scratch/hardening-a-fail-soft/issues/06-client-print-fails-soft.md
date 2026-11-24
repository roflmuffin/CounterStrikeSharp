# 06: ClientPrint fails soft (tracer 2)

Spec: `.scratch/hardening-a-fail-soft/spec.md` (managed core helpers, typed exception). ADR-0002.

**What to build:** The first managed-consumed Engine Feature, `cssharp.chat/client-print` (Bindings: `ClientPrint`, `UTIL_ClientPrintAll`). Its core helpers take addresses from the Resolution Report lazily, per member, instead of resolving signatures in a static initializer. Calling them while the Engine Feature is disabled throws the typed exception naming the Engine Feature, the failed Binding and the reason. Other core helpers keep working. This sets the pattern for ticket 12.

**Blocked by:** 05

**Status:** resolved

**Tier:** 2 add-on · must

- [ ] Managed unit tests: with a report where client-print failed, calling ClientPrint throws the typed exception carrying name, Binding and reason; other helpers in the same table still work
- [ ] A missing Gamedata key for one helper no longer breaks every helper in the table
- [ ] End-to-end: with `ClientPrint` broken, the server stays up, ClientPrint throws the typed exception, and an unrelated helper (e.g. entity creation) still works
- [ ] Registering a hook on a disabled Engine Feature succeeds, logs once, and never fires

## Comments

- Resolved on `hardening/spec-a` (merge of `spec-a/06-client-print-fails-soft`). Native `CREATE_VIRTUAL_FUNCTION_FROM_ADDRESS`; internal `CoreFunction.FromEngineFeature` / stopgap `FromGamedata`; all `VirtualFunctions` members lazy (public fields unchanged); `cssharp.chat/client-print` managed row. Managed 88/88. E2E `eng/e2e/cases/client-print.sh`: valid 192/0/4; ClientPrint ambiguous: server up, typed exception, hook warns once. Pattern for 12 in orchestrator notes `patterns-managed.md` ("Lazy core helper (t06)").
