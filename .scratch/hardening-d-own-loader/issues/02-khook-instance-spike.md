# 02: Spike: one KHook API, two instance sources

Spec: `.scratch/hardening-d-own-loader/spec.md` (KHook). Type: prototype.

**What to build:** Prove that the core can compile against KHook's forwarding header, with every call through one instance pointer set at runtime, and that a statically linked standalone KHook can supply that instance:
- with hidden symbols (`-fvisibility=hidden`, `--exclude-libs,ALL`)
- never shut down
- able to stack on a vtable slot and on an inline-detoured function that Metamod's KHook already hooks

If the standalone build can't supply an instance for the pointer, build and validate a thin adapter with the same API instead. Record the answer in this file.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

**Tier:** 0 standalone · must

**Size / risk:** M · med. Unknown whether standalone KHook exposes an instance object; two-instance stacking is only inferred from source.

- [ ] Minimal test binary: the same hook code runs against instance A or B, chosen at runtime
- [ ] Two instances stack on the same vtable slot and the same function; both pre/post hooks fire; outer/inner order is as predicted
- [ ] `nm -D` on the build shows no exported KHook symbols
- [ ] Findings (works / adapter needed, plus the size of the change to the ~150 call sites) appended under ## Answer
