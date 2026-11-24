# 07: Required Engine Features

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Plugin-facing API). ADR-0002.

**What to build:** A plugin can declare the Engine Features it requires, by name, on its plugin class, the same way it declares a minimum API version. At plugin load, if any Required Engine Feature is disabled, that plugin refuses to load with a clear message naming the Engine Feature and the failed Binding. Plugins that declare nothing load as before.

**Blocked by:** 05

**Status:** resolved

**Tier:** 2 add-on · must

- [ ] Managed unit tests: a plugin requiring an available Engine Feature loads; one requiring a disabled Engine Feature is refused with a message naming feature, Binding and reason; an unknown name is reported clearly
- [ ] The check sits beside the existing minimum-API-version check and produces the same style of refusal
- [ ] End-to-end: with `Host_Say` broken, a test plugin requiring `cssharp.chat/say-hooks` is refused at load while other plugins load

## Comments

- Resolved on `hardening/spec-a` (merge of `spec-a/07-required-engine-features`). `[RequiredEngineFeatures(...)]` (string or enum), pure `RequiredEngineFeatureCheck.GetRefusal`, wired after the min-API check in `PluginContext.Load`. Managed 82/82. E2E `eng/e2e/cases/required-engine-features.sh` (fixture plugin `managed/CounterStrikeSharp.Tests.RequiresSayHooks`): valid loads; Host_Say not found refuses it while NativeTestsPlugin loads.
- Fixture folder renamed to `managed/CounterStrikeSharp.Tests.RequiresSayHooks` during code review fixes.
