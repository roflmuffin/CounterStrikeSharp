# 06: IServerGameClients hooks

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Hooks and calls through Slot Bindings, New Engine Features).

**What to build:** The IServerGameClients hooks (client connect, put in server, disconnect, client command, client voice, client user message) become verified Slot Bindings hooked by index, backing `cssharp.player/connection-events`, `cssharp.player/client-commands`, `cssharp.player/voice-events` and `cssharp.ui/custom-hud-layout`.

**Blocked by:** 03, 04

**Status:** ready-for-agent

**Tier:** 2 add-on · later

- [ ] Slot Bindings bootstrapped and carried forward by the generator
- [ ] Real-build test (local): zero wrong resolutions over the archived pairs
- [ ] Each Engine Feature fails independently; listeners on a disabled one register, log once and never fire
- [ ] End-to-end: connection hooks one index off → heal; corrupted ClientCommand window → client-commands disabled, connections still work
- [ ] Windows behaves as today
