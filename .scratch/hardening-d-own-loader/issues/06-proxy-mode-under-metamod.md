# 06: Proxy Mode under Metamod (ordering A)

Spec: `.scratch/hardening-d-own-loader/spec.md` (Bootstrap and core split, Proxy, KHook).

**What to build:** With `Game csgo/addons/metamod` above `Game csgo/addons/counterstrikesharp`, Metamod's loader picks the Proxy as its game DLL. The Bootstrap's `Load()` detects the running core through the Proxy's exported symbol (`dlopen` with `RTLD_NOLOAD`) and only hands over `ISmmAPI` and Metamod's all-plugins-loaded. It never loads a second core. `meta unload` of the Bootstrap withdraws the API. A missing or broken Metamod core leaves CounterStrikeSharp running.

**Blocked by:** 05

**Status:** ready-for-agent

**Tier:** 2 add-on · must

**Size / risk:** M · med. Init patch order relative to Metamod's loader thunk is inferred from source and needs live confirmation.

- [ ] Live: Metamod + CS2Fixes + MultiAddonManager + CounterStrikeSharp in Proxy Mode all load; `meta list` shows the Bootstrap
- [ ] `Utilities.MetaFactory("MultiAddonManager003")` returns non-null, already during CounterStrikeSharp plugin load
- [ ] With `metamod.2.cs2.so` removed: server and CounterStrikeSharp run; `MetaFactory` returns null
- [ ] gameinfo with only the Metamod line: Plugin Mode, unchanged behaviour (automatic fallback)
- [ ] `meta unload` of the Bootstrap in Proxy Mode: no crash, `MetaFactory` returns null afterwards
