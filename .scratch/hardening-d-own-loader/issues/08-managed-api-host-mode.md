# 08: Managed API for Host mode and Metamod reachability

Spec: `.scratch/hardening-d-own-loader/spec.md` (Managed API).

**What to build:** Additive managed API:
- `Server.HostMode` (`Plugin`/`Proxy`) and `Server.IsMetamodLoaded`. Proposed names; final names are set in this ticket.
- `Utilities.MetaFactory` returns `IntPtr.Zero` when the Host has no Metamod API.
- `OnMetamodAllPluginsLoaded` fires once in every mode with 07's timing, and a new `OnAllPluginsLoaded` alias fires at the same moment. The old name is not obsoleted.

**Blocked by:** 05

**Status:** ready-for-agent

**Tier:** 2 add-on · must

**Size / risk:** S · low

- [ ] API.Tests through a fake Host: `MetaFactory` null without a Metamod API; both all-plugins-loaded listeners fire exactly once; `HostMode`/`IsMetamodLoaded` mirror the Host
- [ ] Existing plugins using `OnMetamodAllPluginsLoaded` and `MetaFactory` compile and run unchanged in Plugin Mode
- [ ] Docfx: "Plugin Mode and Proxy Mode" page covering detection, `MetaFactory` null, hook order next to Metamod plugins, and no unload in Proxy Mode
