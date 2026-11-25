# RFC: optional Proxy Mode, so CounterStrikeSharp can load without depending on Metamod

_Draft for a roflmuffin/CounterStrikeSharp discussion. Not posted (see ticket 16)._

## Summary

This adds an **opt-in** way for CounterStrikeSharp to load itself: a small proxy `libserver.so` that the engine loads, or that Metamod loads as its game DLL. **Nothing changes for anyone who doesn't opt in.** The Metamod plugin install stays the default, and it keeps the same `.vdf`, the same behaviour and the same hook order.

## Why

- **A broken Metamod core takes CounterStrikeSharp down, even when CounterStrikeSharp itself is fine.** Example: the Metamod/KHook issues #278 and #288 stopped some Linux servers for weeks.
- **Metamod API changes force work on CounterStrikeSharp.** The KHook migration (#1418) is an example.
- **CounterStrikeSharp needs only a few things from Metamod:** loading, KHook, interface factories, level events, console print and `MetaFactory`.
- **Precedent:** ModSharp and SwiftlyS2 load themselves. ModSharp's loader needed no game-update fixes in about 13 months.

**What this does *not* fix:** most game-update breakages. Those come from tier0 imports and SDK header changes, which CounterStrikeSharp shares with Metamod. This RFC is about dependencies, not outage hours.

## How it works

```
gameinfo.gi (opt-in):           Game csgo/addons/metamod             <- optional
                                Game csgo/addons/counterstrikesharp  <- the one new line
                                Game csgo

engine → Metamod loader → CSSharp proxy libserver.so → real csgo libserver.so
                              └─ loads CSSharp core (own KHook)
Metamod plugins load as usual; the CSSharp .vdf plugin only hands ISmmAPI to the core
```

- **Metamod present:** Metamod's loader picks the first other `libserver.so` on the search paths, which is the proxy. If the Metamod core fails, Metamod's loader keeps forwarding, so CounterStrikeSharp still runs.
- **Metamod absent:** the engine loads the proxy directly.
- **Line missing** (for example after a game update overwrites `gameinfo.gi`): CounterStrikeSharp loads as a Metamod plugin, exactly as today.
- **The proxy itself:**
  - imports only libc/libdl
  - passes every interface through unchanged
  - hooks only `ISource2ServerConfig::Connect` and `ISource2Server::Init`/`Shutdown`, and restores a slot only if it still holds the proxy's own pointer
  - uses no byte signatures
  - fails open (the server runs without CounterStrikeSharp), or fails closed with `-css_fail_closed`

## Plugin-facing changes (additive)

- `Utilities.MetaFactory` keeps working whenever Metamod is loaded, and returns null otherwise.
- `OnMetamodAllPluginsLoaded` fires in every mode. A neutral alias, `OnAllPluginsLoaded`, is added.
- New: `Server.HostMode`, `Server.IsMetamodLoaded`.
- In Proxy Mode, CounterStrikeSharp's hooks sit outside Metamod plugins' hooks on shared functions. This is documented.
- Signature scanning reads the on-disk module, so detours installed by other plugins can't break signatures. This applies in both modes.

## Code changes, in reviewable steps

1. Pristine-copy signature scanning (standalone).
2. A host seam behind `mm_plugin.cpp`, with no behaviour change.
3. A Metamod bootstrap/core split: only the bootstrap includes Metamod headers.
4. The proxy, plus its own KHook instance (Linux).
5. Proxy Mode under Metamod, level-event parity, and the managed API.
6. A boot status log, the fail-closed switch, and docs. Windows follows.

## Not proposed

- Dropping Metamod support.
- Hosting Metamod plugins inside CounterStrikeSharp.
- Auto-editing `gameinfo.gi`.
- Changing the default install.

## Questions for maintainers

- Is an opt-in second load path acceptable to maintain?
- Is a host seam in core acceptable, in the step-2 shape?
- Naming for `Server.HostMode` / `Server.IsMetamodLoaded`.
- Should Proxy Mode be documented as experimental for its first releases?
