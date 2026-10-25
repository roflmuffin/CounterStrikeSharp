# Native Tests
This plugin is intended to be ran inside a running CS2 server running a version of CS# and runs tests against the exposed `NativeAPI` methods.

## Live cs_script tests

Build the native plugin with `CSS_ENABLE_CS_SCRIPT=ON`, then deploy the matching
native plugin, managed API, `gamedata/cs_script.linux.json`, and this test plugin.
The experimental adapter currently targets Linux CS2 1.41.9.2 and V8 11.9.169.6;
it checks the server binary build ID before creating a host.

With a map loaded, run this in the server console or RCON:

```text
css_test_cs_script
```

CS# automatically creates one empty `point_script` context per map. Plugins use
`CsScriptRuntime.Shared` on the game thread without creating or disposing a runtime.
Map startup only schedules initialization. The core waits for a completed simulated
frame, then initializes from a later `GameFrame` post hook when V8 is available.
Access during `OnMapStart` can therefore report not ready. Initialization stages
and failures are logged by the core and reported on access.

The ten tests compare GameRules reads with the native schema, round-trip a Unicode
entity name, check singleton reuse across plugin consumers and disposal, verify
host recreation and stale-handle rejection, reject worker-thread access, verify a
$100 team-money award, and reject unknown method hashes and malformed headers.
The host-recreation test deliberately removes the shared host and waits for CS#
to replace it. No script file, compiled asset, or addon loader is needed.

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

Calls use hashed method identifiers and typed `ScriptContext` slots. Deploy the
native plugin and managed API from the same build when changing this transport.
After a map change, reacquire `CsScriptRuntime.Shared`; cached runtimes and entity
facades from the previous context are rejected. The host belongs to CS#, so plugin
unload cannot destroy it. The test suite can be rerun after changing maps to check
automatic initialization on the new map.
