# With Entity Output Hooks
This example shows how to implement hooks for entity output, such as StartTouch, OnPickup etc.

For one entity, use `HookSingleEntityOutput(entity, "OnPressed", handler)`.
Pass `HookMode.Post` as the fourth argument to run after the original output
method. The three-argument overload still uses `HookMode.Pre`.
`UnhookSingleEntityOutput(entity, "OnPressed", handler)` remembers the registered
mode; hooks are also removed on map end and plugin disposal.

A post hook does not wait for delayed entity inputs. If your handler needs state
changed by queued inputs, read it after those inputs have been processed.
