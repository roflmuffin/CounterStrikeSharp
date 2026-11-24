# 07: IVEngineServer2 hook and direct calls

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Hooks and calls through Slot Bindings). IVEngineServer2 shifted slots we use in 7 incidents.

**What to build:** The SetClientListening hook and every direct call on IVEngineServer2 (about 20) go through verified Slot Bindings in the engine2 module. Enumerate the direct calls against the code, group them by plugin-visible API, and add one Engine Feature per API to the catalog (`cssharp.<area>/<name>`, permanent once added), alongside `cssharp.player/set-listening`.

**Blocked by:** 03, 04

**Status:** ready-for-agent

**Tier:** 2 add-on · must

- [ ] The catalog lists every IVEngineServer2-backed Engine Feature with its Slot Bindings
- [ ] Vtable Check works on the engine2 module (class CEngineServer, exact-name lookup)
- [ ] Real-build test (local): zero wrong resolutions on the archived engine2 pair
- [ ] A failed engine Slot Binding disables only the Engine Features using it; calls throw the typed exception
- [ ] End-to-end: one engine call's Slot Binding one off → heals; one corrupted → only its Engine Feature disabled, server up
- [ ] Windows behaves as today
