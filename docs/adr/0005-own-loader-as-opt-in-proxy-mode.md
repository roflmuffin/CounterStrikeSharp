# Own loader as an opt-in Proxy Mode under Metamod

CounterStrikeSharp gains a second way to load: a **Proxy** `libserver.so`/`server.dll` that becomes the game DLL and loads the real server and the CounterStrikeSharp core itself. When Metamod is installed, its loader picks the Proxy as its game DLL (ordering A): the `counterstrikesharp` search path goes after Metamod's. When Metamod is absent, the engine loads the Proxy directly. **Plugin Mode** (CounterStrikeSharp as a Metamod plugin, as today) stays the default on Linux and Windows. **Proxy Mode** is opt-in via one `gameinfo.gi` line. If the line is lost after an update, the server falls back to Plugin Mode on its own.

**Goal:** fewer dependencies CounterStrikeSharp doesn't need, and independence from failures that are Metamod's alone (a broken Metamod core, Metamod/KHook bugs such as #278 and #288, Metamod API churn such as the KHook migration).

**Not a goal:** outage hours. Measured over 9 Metamod load failures, a CounterStrikeSharp with its own loader would have shared the root cause in 7 of them: tier0 imports and SDK header/struct batches. That saves ≤8% of Metamod's 162 h (`.scratch/incremental-hardening/research/own-loader/findings.md`). Outage hours are reported, not targeted.

**Amends ADR-0001.** ADR-0001 left the own loader out of T1 as part of the "ideal stack" rewrite. It returns as a separate spec (D) because it's cheap, ships incrementally and keeps the plugin API additive. The rest of the ideal stack is still out: no compiled engine headers, `dlsym` for tier0, extracted struct layouts. Those are what would actually move outage hours.

## Decisions

- **Code split:** a thin Metamod **Bootstrap** plugin is the only code that links Metamod headers. The core runs behind a **Host** seam with two implementations: `MetamodHost` (Plugin Mode) and `OwnHost` (Proxy Mode).
- **KHook:** in Plugin Mode the core uses Metamod's KHook instance, exactly as today. In Proxy Mode it uses its own statically linked instance with hidden symbols, which is never freed.
- **Bootstrap in Proxy Mode:** when the Proxy is present, the Bootstrap only hands `ISmmAPI` to the core, so `MetaFactory` keeps working. A double-load guard prevents two cores.
- **Proxy rules:**
  - No SDK or tier0 imports.
  - Absolute paths derived from its own location.
  - Pure pass-through of interface pointers.
  - Hooks only `ISource2ServerConfig` slot 0 and `ISource2Server` slots 3/5, restoring a slot only if it still holds the Proxy's own pointer.
  - No byte signatures.
  - Loads only a core with a matching ABI version.
- **Failure handling:** fail open by default, so the server runs without CounterStrikeSharp and logs loudly. Fail closed with `-css_fail_closed`.
- **Rollout:** Proxy Mode becomes the *recommended* install (docs only) after 3 game updates per OS with no loader fix. `gameinfo.gi` is never auto-edited.

## Considered Options

- **Standalone only:** drops users' Metamod plugins (CS2Fixes, MultiAddonManager, sql_mm, cs2kz).
- **Dual-mode** (a Metamod plugin when Metamod is present, otherwise the Proxy): CounterStrikeSharp still dies whenever Metamod dies.
- **Proxy chain-loads Metamod:** needs a re-entry guard against Metamod's search-path walk, more slot juggling and the undocumented `server_valve`. Kept as a fallback only if gameinfo ordering proves fragile.
- **Host Metamod plugins inside CounterStrikeSharp:** reimplements the Metamod core (XL) and follows its ABI churn.
- **`libserver_valve` instead of a gameinfo line:** would need no edit, but relies on undocumented Valve behaviour. Spike only.

## Consequences

- In Proxy Mode alongside Metamod plugins there are two KHook instances. CounterStrikeSharp's hooks run outside theirs on shared slots. This is documented, not hidden.
- In Proxy Mode CounterStrikeSharp can't be unloaded at runtime. Managed plugin hot-reload is unchanged.
- hl2sdk stays a build dependency. The metamod-source submodule stays too, for the Bootstrap and `MetamodHost`.
