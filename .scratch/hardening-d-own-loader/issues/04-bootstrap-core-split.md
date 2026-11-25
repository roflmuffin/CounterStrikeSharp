# 04: Bootstrap and core split

Spec: `.scratch/hardening-d-own-loader/spec.md` (Bootstrap and core split, KHook).

**What to build:** The binary the existing `.vdf` points at becomes the Bootstrap, the only code that includes Metamod headers. Its `Load()` loads the core library from the same `bin/<platform>/` directory and starts it on the Metamod host. The core build drops the Metamod include dirs. The core compiles against the KHook forwarding header with the instance pointer set by the Host, using the approach from 02. The Bootstrap's unload behaves as today's plugin unload. Plugin Mode is behaviour-identical, including hook order.

**Blocked by:** 02, 03

**Status:** ready-for-agent

**Tier:** 1 base · must

**Size / risk:** M · med. Library path and loading on both OSes; the Windows build must keep working.

- [ ] The core target compiles without Metamod include dirs; only the Bootstrap includes `ISmmPlugin.h`
- [ ] The existing `.vdf` and install layout are unchanged; an untouched server upgraded in place loads as before
- [ ] Live: native test suite passes in Plugin Mode; `meta unload`/`meta load` of CounterStrikeSharp behaves as today
- [ ] Windows builds, loads and passes the smoke test in Plugin Mode
