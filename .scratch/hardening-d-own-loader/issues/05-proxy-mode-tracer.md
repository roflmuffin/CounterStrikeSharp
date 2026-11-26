# 05: Proxy Mode without Metamod (tracer, Linux)

Spec: `.scratch/hardening-d-own-loader/spec.md` (Proxy, Host seam, KHook).

**What to build:** End-to-end Proxy Mode on a server with no Metamod.
- **Proxy:** `addons/counterstrikesharp/bin/linuxsteamrt64/libserver.so`, importing only libc/libdl.
  - Exports `CreateInterface` as pure pass-through, and loads the real server by an absolute path derived from its own location.
  - Hooks `ISource2ServerConfig` slot 0 and `ISource2Server` slots 3/5 before returning the interface, with compare-before-restore.
- **Core start in the Init thunk:** load the core by absolute path, check the ABI version, start it on the own host (factories from the Proxy, console print via dynamic tier0 `Msg`, own statically linked hidden KHook that is never freed), then call the real Init.
- **Failure:** fail open with one loud log line.
- **Double-load guard:** the Proxy exports a symbol; the core refuses a second start.

**Blocked by:** 01, 04

**Status:** ready-for-agent

**Tier:** 1 base · must

**Size / risk:** M–L · med. Needs live validation; the first time CounterStrikeSharp owns process loading.

- [ ] `nm -D --undefined-only` on the Proxy shows only libc/libdl symbols
- [ ] Live, gameinfo with only `Game csgo/addons/counterstrikesharp` (+ `Game csgo`): server boots, GameFrame ticks (timers fire), a managed plugin loads and runs a command
- [ ] Proxy slot restore unit-tested with two stacked fake hookers, restored in either order
- [ ] Core with a wrong ABI version or missing: server runs vanilla, one clear log line
- [ ] Plugin Mode unaffected; Windows build unaffected
