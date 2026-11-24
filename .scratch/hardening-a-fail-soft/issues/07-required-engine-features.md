# 07: Required Engine Features

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Plugin-facing API). ADR-0002.

**What to build:** A plugin can declare the Engine Features it requires, by name, on its plugin class, the same way it declares a minimum API version. At plugin load, if any Required Engine Feature is disabled, that plugin refuses to load with a clear message naming the Engine Feature and the failed Binding. Plugins that declare nothing load as before.

**Blocked by:** 05

**Status:** ready-for-agent

- [ ] Managed unit tests: a plugin requiring an available Engine Feature loads; one requiring a disabled Engine Feature is refused with a message naming feature, Binding and reason; an unknown name is reported clearly
- [ ] The check sits beside the existing minimum-API-version check and produces the same style of refusal
- [ ] End-to-end: with `Host_Say` broken, a test plugin requiring `cssharp.chat/say-hooks` is refused at load while other plugins load
