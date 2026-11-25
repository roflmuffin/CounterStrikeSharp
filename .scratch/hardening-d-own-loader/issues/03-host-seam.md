# 03: Host seam with the Metamod host

Spec: `.scratch/hardening-d-own-loader/spec.md` (Host seam). Prefactor; no behaviour change.

**What to build:** Every Metamod-facing call in the core goes through one Host interface: factories, level start/end, console print, the KHook instance, the optional Metamod API and all-plugins-loaded. The Metamod host implements it by wrapping today's `ISmmAPI` usage:
- `mm_plugin.cpp` `GET_V_IFACE_*`, `AddListener`, level callbacks
- `globals::ismm`
- `natives_metamod.cpp` `META_FACTORY`
- `natives_engine.cpp` `GetServerFactory`/`GetEngineFactory`/`META_CONPRINT`

Afterwards no core file outside the Metamod host touches `ismm` or `META_*`.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

**Tier:** 1 base · must

**Size / risk:** M · low

- [ ] `grep` for `ismm`, `META_`, `ISmmAPI` outside the Metamod host and the plugin entry finds nothing
- [ ] Natives `META_FACTORY` and `GetValveInterface` and console print behave identically
- [ ] Live: Plugin Mode boots, native test suite passes, map change fires OnMapEnd/OnMapStart as before
- [ ] Windows builds and behaves as today
