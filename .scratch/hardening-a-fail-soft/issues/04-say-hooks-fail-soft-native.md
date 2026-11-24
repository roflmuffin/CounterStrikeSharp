# 04: Say-hooks fail soft natively (tracer 1a)

Spec: `.scratch/hardening-a-fail-soft/spec.md`. ADR-0002.

**What to build:** The first end-to-end path for one native-owned Engine Feature, `cssharp.chat/say-hooks` (Binding: `Host_Say`). A Binding resolver resolves its declared Bindings strictly (exactly one match) into an immutable Resolution Report for the running build. The chat hook is registered only when the Engine Feature is available; otherwise it is skipped and the server keeps running. An early native log line lists failed Bindings before .NET starts. This ticket also builds the end-to-end harness on the laptop server that later tickets extend with their own broken-key cases.

**Blocked by:** 01, 03

**Status:** ready-for-agent

**Tier:** 1 base · must

- [ ] Native unit tests (doctest) on synthetic images: unique match resolves; zero matches fails as not found; two matches fails as ambiguous with the count; match at the final position resolves; invalid hex fails as invalid pattern; missing key and missing per-OS entry fail with their reasons
- [ ] The Resolution Report records the build id and, per Binding, status, reason, match count and address
- [ ] Signatures are scanned in the module named by the Gamedata `library` field
- [ ] End-to-end: with `Host_Say` broken in Gamedata the server boots and stays up, the chat hook is not registered, and the early native log names `Host_Say` and its reason
- [ ] End-to-end: with valid Gamedata, chat hooks work as before
- [ ] The end-to-end harness can boot the laptop server with a modified Gamedata file and collect the native test plugin's results file

## Comments

- Unit side merged to `hardening/spec-a` (`spec-a/04-say-hooks-fail-soft-native`): pure resolver `src/core/binding_resolver.*`, catalog `src/core/engine_features.*`, adapter `src/core/core_bindings.*`, doctest 27 cases. E2E criteria pending the harness (`spec-a/04a-e2e-harness`); ticket stays open until then. Native patterns: see orchestrator notes `patterns-native.md`.
