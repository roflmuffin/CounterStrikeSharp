# With ConVars

This example shows how to create real engine console variables with `ConVar<T>`,
read and write typed values, set numeric bounds, find existing ConVars, and
listen for changes.

Unlike `FakeConVar<T>`, these variables are registered with the engine as
ConVars. After loading the plugin, try the following in the server console:

```text
example_real_enabled
example_real_enabled false
example_real_message "Hello from the console!"
example_real_speed 25
example_real_speed 150
```

Changes to `example_real_speed` are logged by the plugin. Its value is clamped
to the range 0–100, so assigning 150 results in 100. The example also reads the
existing `mp_warmuptime` ConVar without changing it.

The type parameter must match the ConVar's engine type, such as `bool`, `int`,
`float`, or `string`.

ConVars created as public fields on the plugin are automatically tracked and
deleted on unload or hot reload. For public fields on another object or type,
call `RegisterConVars(instance)` or `RegisterConVars(typeof(YourConVars))`.
ConVars created elsewhere need explicit cleanup with `Delete()`, as demonstrated
by the temporary ConVar. It is not advised to delete existing engine ConVars
found with `Find`, but it is allowed.
