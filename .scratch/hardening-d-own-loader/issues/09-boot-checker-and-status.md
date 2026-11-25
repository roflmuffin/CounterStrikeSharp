# 09: Boot checker and loader status

Spec: `.scratch/hardening-d-own-loader/spec.md` (Boot checker and status).

**What to build:** A read-only checker that logs one block at core start (and from the Proxy on failure) and writes a status file. The block contains:
- the active mode and whether Metamod is loaded
- the position of the CounterStrikeSharp line in `gameinfo.gi` relative to Metamod's
- the reason this mode was chosen
- the Proxy and core ABI versions

It warns loudly when the CounterStrikeSharp line sits above Metamod's while Metamod is installed, because that silently disables all Metamod plugins. It never writes `gameinfo.gi`.

**Blocked by:** 05

**Status:** ready-for-agent

**Tier:** 2 add-on · must

**Size / risk:** S · low

- [ ] Unit tests on gameinfo fixtures: line present, absent, above Metamod, below Metamod, Metamod absent
- [ ] Live: each of those five setups logs the expected mode and reason; the status file matches
- [ ] Plugin Mode servers without the line get one informational line, no warning
