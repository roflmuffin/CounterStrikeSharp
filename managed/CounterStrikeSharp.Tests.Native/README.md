# Native Tests
This plugin is intended to be ran inside a running CS2 server running a version of CS# and runs tests against the exposed `NativeAPI` methods.

## Live cs_script tests

The cs_script bridge is built automatically on Windows and Linux. Deploy the matching
native plugin, managed API, `gamedata/gamedata.json`, and this test plugin.
The adapter resolves the required engine functions and reads layout offsets from
gamedata. It has no fixed server version, V8 version, or struct-size checks.
CMake defaults to the headers and link libraries in `libraries/v8`.
See [V8 build inputs](../../libraries/v8/README.md) for overrides and updates.

With a map loaded, run this in the server console or RCON:

```text
css_test_cs_script
```

CS# automatically creates one empty `point_script` context per map. Plugins use
`CsScriptRuntime.Shared` on the game thread without creating or disposing a runtime.
Map startup only schedules initialization. The core waits for a completed simulated
frame, then initializes from a later `GameFrame` post hook when V8 is available.
Access during `OnMapStart` can therefore report not ready. Initialization failures
are logged by the core and reported on access.

The eleven tests compare GameRules reads with the native schema, round-trip a
Unicode entity name, round-trip entity arguments and results, check singleton
reuse across plugin consumers and disposal, verify cached facades survive host recreation and reject stale
entity handles (including entity arguments), reject worker-thread access, verify a $100
team-money award, and reject unknown method hashes and malformed headers.
The host-recreation test deliberately removes the shared host and waits for CS#
to replace it. No script file, compiled asset, or addon loader is needed.

The entity-owner test creates two temporary `info_target` entities. It calls
`SetOwner` and `GetOwner`, compares the returned handle with `OwnerEntity`, uses
the returned entity in another script call, checks entity results from
`Instance.FindEntityByName`, and clears the owner to verify `undefined`/`null`
conversion. It removes both temporary entities in `finally` and needs no players
or special map setup. Run it alone with:

```text
css_itest EntityOwner_RoundTripsEntityArgumentsAndResults
```

The money test requires a normal round outside warmup and a connected T or CT
(a bot is fine) eligible for round money with at least $100 below `mp_maxmoney`.
It fails with a prerequisite message if no such player exists. It awards $100
through `GameRules.AddTeamMoney`, checks eligible teammates and unchanged
opposing-team balances, then restores captured account and next-round earnings
in `finally`. Balance checks happen in one game-thread callback. Run it alone with:

```text
css_itest AddTeamMoney_IncreasesEligiblePlayerBalances
```

Use `css_itest EmptyHost_ReadsCurrentGameRules` for just the initial context and
API smoke test, or `css_itest CsScriptTests` for the full group. The console prints
the shared host handle and the existing xUnit reporter prints pass/fail details.
These tests require a running server; a successful build does not establish that
the reverse-engineered runtime works on the live engine.

Regenerate the script API with `dotnet run --project tooling/CodeGen.CsScript`
from the repository root. The generator downloads the latest upstream declarations
on each run; see [its README](../../tooling/CodeGen.CsScript/README.md).

Runtime controls use separate generated `NativeAPI.CsScript*` natives. Script
method calls use `NativeAPI.CsScriptInvoke` with hashed method identifiers and
typed `ScriptContext` slots. Deploy the
native plugin and managed API from the same build when changing this transport.
Cached runtimes and module facades automatically use the current shared host after
a map change or host replacement. Generated methods on entity schema
classes acquire the shared context per call and pass their own entity handle. The
host belongs to CS#, so plugin unload cannot destroy it. The test suite can be rerun after changing maps to check
automatic initialization on the new map.
