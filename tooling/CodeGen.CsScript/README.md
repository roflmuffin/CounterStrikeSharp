# cs_script generator

Plugins use the core-owned context on the game thread:

```csharp
var script = CsScriptRuntime.Shared;
bool warmup = script.GameRules.IsWarmupPeriod();
script.GameRules.AddTeamMoney(3d, CSTeamMoneyReason.NONE, 100d);
```

Entity methods are generated on partial schema classes in
`managed/CounterStrikeSharp.API/Modules/CsScript/Generated/Entities.g.cs`:

```csharp
var bomb = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4").First();
bool plantedAtA = bomb.IsBombsiteA();
double plantTime = bomb.GetPlantTime();
bomb.SetEntityName("tracked_bomb"); // Inherited from CBaseEntity.
```

Each entity call acquires `CsScriptRuntime.Shared` and passes the current entity's
`EntityHandle.Raw`. There is no separate `CsScriptCSPlantedC4` or `runtime.Entity(...)`
facade. Shared-context access checks the game thread before reading the entity
handle; native invocation validates the handle and serial. Entity methods acquire
the current shared host automatically after host recreation, while deleted entities
remain invalid.

Schema names default to `C` plus the TypeScript entity name (for example,
`CSPlayerPawn` becomes `CCSPlayerPawn`). A small override table handles exceptions
such as `CSPlantedC4` to `CPlantedC4`, and `Entity` to `CBaseEntity`. The resolved
schema class must exist or generation fails. Methods are emitted on their
declaring class and inherited through the schema hierarchy. Existing schema
members retain their behavior: conflicting script methods use the suffix `Script` (`IsValidScript`,
`RemoveScript`, `SetModelScript`, `IsRescuedScript`, `IsBotScript`, `IsScopedScript`,
and `IsBuyMenuOpenScript`). Module exports such as `GameRules`, `Console`, and
`Instance` remain runtime facades. Inherited native descriptors are retained;
existing scalar method identities remain stable.

The enabled native adapter schedules the empty host at map startup. It waits for
a completed simulated frame and creates the host from a later `GameFrame` post
hook once the engine isolate is available, rather than creating entities during
`StartupServer` or `PreWorldUpdate`. `Shared` can report not ready in `OnMapStart`.
The host is shared by all plugins. There is no plugin owner, `Create`, or `Dispose`
on the runtime. Core map shutdown releases V8 handles; core unload also removes
the host. Unexpected host deletion schedules recreation after simulation resumes.
Cached runtimes and module facades continue to use the current shared host after
a map change or host replacement. There are no session IDs or plugin-owned scopes.
Access before initialization or off the game thread throws. `CCSGameRules.Script()` also uses the shared context.

The generator is a .NET 10 console application using the
[TypeScriptParser](https://www.nuget.org/packages/TypeScriptParser/0.4.2.39)
Tree-sitter AST library. Node.js, npm, and a TypeScript compiler installation are
not required. Run from the repository root:

```sh
dotnet run --project tooling/CodeGen.CsScript
dotnet run --project tooling/CodeGen.CsScript -- --check
```

Both commands always download the current declarations from:

<https://raw.githubusercontent.com/SteamTracking/GameTracking-CS2/refs/heads/master/content/csgo_addons/cs_script_demo/maps/scripts/point_script.d.ts>

Generation records the fetched declarations in `generated/point_script.d.ts` and
the source URL and normalized Git blob SHA in `generated/manifest.json`. These
files record which source produced the outputs; they are never an offline fallback.
`--check` downloads the source again, compares all outputs, and exits nonzero if
any are stale without writing files. Download failures, syntax errors, missing
schema classes, unsupported enum initializers, and hash collisions fail before
any output is written. `docs/references/cs_script/master` remains a historical
reference, not the generator input.

Before emitting enums, the generator checks `managed/CounterStrikeSharp.API/Generated/Schema/Enums`.
It reuses a schema enum when its type name matches (ignoring a leading `CS`,
a trailing `_t`, underscores and case) and its complete member set has identical
numeric values. Member names may differ by case and a uniform schema prefix
such as `WEAPONTYPE_`. Extra members, different values, ambiguous matches and
unrecognized schema syntax retain a separate script enum; unsigned sentinels
are not treated as negative values. Empty enums are not reused.

Reused enums are omitted from `Api.g.cs`, and method parameters, return types
and result casts reference the schema type directly. For example, `CSWeaponType`
uses `CounterStrikeSharp.API.Core.CSWeaponType` (with `WEAPONTYPE_*` members),
and `CustomCameraMode` uses `CustomCameraMode_t`. TypeScript names remain in
the manifest and native method identities, so reuse does not change method IDs
or the numeric transport ABI.

The AST visitor resolves named types and inheritance, groups overloads, and
extracts scalar and entity signatures. It does not run the TypeScript semantic
checker. Supported methods tagged `@deprecated` are emitted with `[Obsolete]`,
including each generated optional-argument overload. Deprecation is tracked per
TypeScript overload, not across all overloads of the same name. Every emitted
method's XML summary begins with `This is Valve generated cs_script method`.
Complex and unrecognized type shapes remain unsupported and are recorded as
diagnostics, rather than guessed into callable signatures. Enum initializers are
evaluated from AST nodes with JavaScript bitwise semantics and must produce safe
integer values. Optional boolean arguments retain the previous generator's
unsupported union behavior during this migration.

The parser package includes native Tree-sitter binaries for Linux x64, Windows
x64, and macOS ARM64; only the generator needs them. API/runtime dependencies are
unchanged. There is no separate generator test project.

Supported methods receive a 32-bit DJB2-XOR hash using the same algorithm
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

Runtime controls use separate registered natives, declared in
`src/scripting/natives/natives_cs_script.yaml` and generated into `NativeAPI`:
`CsScriptGetHost` and `CsScriptIsReady`.
Both take no arguments and validate the current shared host.

Generated methods push arguments directly into an invocation-owned `ScriptContext`
and call the separate `NativeAPI.CsScriptInvoke` native. The native generator's
`scriptContext` parameter means the caller supplies prepared slots and reads the
result from that context; the binding sets its native identifier, invokes it, and
checks errors without resetting or boxing the arguments. Regenerate these native
bindings with `dotnet run --project tooling/CodeGen.Natives -- --natives`.

There are no object argument arrays or JSON requests/results. Invocation slots
0–1 contain the method hash and entity handle. Up to 24 scalar or entity-handle
arguments follow.
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

Entity arguments and results use zero-extended 32-bit handles, including their
serial numbers. Native code validates each handle before asking the engine to
wrap it in the active V8 context. Entity results must have the engine's kind-2
binding marker and match the wrapper cached for that context. The entity-handle
offset comes from gamedata without a hardcoded layout check. No V8 or entity pointers
cross the managed transport boundary.

The generator uses the mapped schema types, for example
`CCSPlayerPawn.DropWeapon(CCSWeaponBase target)` and
`CHostage? CCSPlayerPawn.GetHostage()`. Entity unions with `undefined` or `null`
become nullable C# types. `uint.MaxValue` represents absence only for those
contracts: C# null maps to JS undefined when allowed, otherwise JS null; when both
are allowed it maps to undefined. An absent JS result maps to C# null. Required
entity arguments reject null, and deleted entity objects are never treated as
absence. Omitted optional arguments continue to use shorter overloads and fewer
slots. Arrays of entities and arbitrary object shapes remain unsupported.

Entity contracts include their declared type and absence semantics in method
identities.

The manifest remains JSON as a generation artifact. Neither the native descriptor
table nor the managed/native invocation path parses JSON. Complex objects,
callbacks and promises still require separate marshalling and lifetime support;
unsupported signatures are recorded as diagnostics rather than emitted as stubs.
