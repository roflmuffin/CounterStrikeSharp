# 15: Spike: loading through `libserver_valve`

Spec: `.scratch/hardening-d-own-loader/spec.md` (Out of Scope → spike). Type: research.

**What to build:** Find out whether a Proxy named `libserver_valve.so` (and `server_valve.dll`) loads ahead of Metamod and without any gameinfo edit on a stock dedicated server:
- decode libengine2's gate flags for the `server_valve` probe
- test it live
- describe what it would mean for ordering under Metamod (CounterStrikeSharp would be outer, Metamod loaded as its game DLL?)

Answer only; nothing ships.

**Blocked by:** 05

**Status:** ready-for-agent

**Tier:** 2 add-on · later

**Size / risk:** S · med. Undocumented Valve behaviour that may disappear.

- [ ] Gate conditions documented with disasm refs (Linux; Windows if available)
- [ ] Live result: loads or doesn't, with and without Metamod
- [ ] Recommendation appended under ## Answer
