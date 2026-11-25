# 07: Level lifecycle parity in Proxy Mode

Spec: `.scratch/hardening-d-own-loader/spec.md` (Level lifecycle in Proxy Mode).

**What to build:** The own host hooks loop-mode `LoopInit`/`LoopShutdown` at the points Metamod's provider uses, and drives level start/end, `OnMapStart` and `OnMapEnd` from them. All-plugins-loaded fires at the later of "core start done" and "Bootstrap's all-plugins-loaded", or at core start without Metamod. The OnTakeDamage hook attach moves with it. The managed event sequence for a scripted map change is identical in Plugin Mode and Proxy Mode.

**Blocked by:** 05

**Status:** ready-for-agent

**Tier:** 2 add-on · must

**Size / risk:** S–M · med. Needs live validation on map change and hibernation.

- [ ] Event-order parity test: same map-change script in both modes produces the same managed event sequence
- [ ] OnTakeDamage hooks work in Proxy Mode with and without Metamod
- [ ] Hibernation enter/leave behaves the same in both modes
