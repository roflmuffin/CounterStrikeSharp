# 05: Resolution Report reaches plugins (tracer 1b)

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Native → managed boundary, Plugin-facing API, Startup report). ADR-0002.

**What to build:** The Resolution Report crosses to managed through one native call and becomes an immutable managed value. Engine Feature availability is a pure function of it. Plugins can ask whether an Engine Feature is available, why not, and list all with status, by stable string name, with a generated enum as convenience. One startup report after managed load shows the build id, the count available, and each disabled Engine Feature with its failed Binding and reason.

**Blocked by:** 04

**Status:** ready-for-agent

- [ ] Managed unit tests build Resolution Reports by hand and assert availability, the query API (available, reason, list) and the startup report text
- [ ] Engine Feature identity is the namespaced string; the enum is generated from the names
- [ ] End-to-end: with `Host_Say` broken, a test plugin sees `cssharp.chat/say-hooks` as unavailable with reason "not found" (or "ambiguous" with count), and the startup report lists it
- [ ] End-to-end: with valid Gamedata the startup report shows all declared Engine Features available
