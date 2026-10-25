# cs_script generator

Plugins use the core-owned context on the game thread:

```csharp
var script = CsScriptRuntime.Shared;
bool warmup = script.GameRules.IsWarmupPeriod();
script.GameRules.AddTeamMoney(3d, CSTeamMoneyReason.NONE, 100d);
```

The enabled native adapter schedules the empty host at map startup. It waits for
a completed simulated frame and creates the host from a later `GameFrame` post
hook once the engine isolate is available, rather than creating entities during
`StartupServer` or `PreWorldUpdate`. `Shared` can report not ready in `OnMapStart`.
The host is shared by all plugins. There is no plugin owner, `Create`, or `Dispose`
on the runtime. Core map shutdown releases V8 handles; core unload also removes
the host. Unexpected host deletion schedules recreation after simulation resumes.
Reacquire `Shared` after a map change or host replacement: previous session IDs
are never rebound to the new context. Access before initialization or off the
game thread throws. `CCSGameRules.Script()` also uses the shared context.

Run `npm ci`, then `npm run generate` from this directory. `npm test` checks the
generator, and `npm run check` verifies checked-in outputs against the pinned
TypeScript declarations in `docs/references/cs_script/master`.

Supported scalar methods receive a 32-bit DJB2-XOR hash using the same algorithm
as `hash_string` in `src/scripting/script_engine.h`. The canonical identity includes
the module, receiver kind and export name, member name, parameter types and
optionality, and return type. For example:

```text
cs_script/point_script:export:GameRules.AddTeamMoney(number,enum:CSTeamMoneyReason,number?)->void
```

Declaration order, documentation and parameter names do not affect the ID.
Overloads with different signatures get different hashes; the two C# overloads
for an optional argument share a hash and send different argument counts.
Generation fails on duplicate IDs, including collisions between distinct names.

Generated wrappers push arguments directly into an invocation-owned `ScriptContext`.
There are no object argument arrays or JSON requests/results. Slots 0–3 contain
the four 64-bit words of a SHA-256 schema digest; slots 4–7 contain the operation
hash, session ID, method hash, and entity handle. Up to 24 scalar arguments follow.
The digest covers the transport version, method contracts and enum values.
Native code validates the header and argument count, finds the compiled descriptor
by hash, creates V8 values, and calls the named method on its engine-owned receiver.
The method name stays in the native descriptor and does not cross the boundary.

Numbers use `double`, booleans use a zero-filled slot, and enum arguments use a
JavaScript number. Strings use the existing NUL-terminated UTF-8 native ABI and
are limited to 1 MiB; embedded NULs are rejected rather than truncated. Native
string results are copied out of V8 before the handle scope closes, and then
copied into a managed string before another native call. Scalars return through
the context's result slot and exceptions through its existing native-error path.
Per-call disposal releases argument strings even if argument preparation fails.

The manifest remains JSON as a generation artifact. Neither the native descriptor
table nor the managed/native invocation path parses JSON. Complex objects,
callbacks and promises still require separate marshalling and lifetime support;
unsupported signatures are recorded as diagnostics rather than emitted as stubs.
