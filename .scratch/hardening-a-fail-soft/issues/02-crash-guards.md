# 02: Crash guards for unresolved Bindings

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Prefactor).

**What to build:** Remove the known crash paths when a Binding fails to resolve, before the fail-soft machinery exists:
- Game-system init null-checks the resolved signature before applying its instruction offset, logs the miss, and skips game-system registration instead of segfaulting.
- The natives that call dispatch-spawn and the legacy game-event listener check that the function resolved and raise a clear error instead of calling null.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

**Tier:** 0 standalone · must

- [ ] With the game-system signature removed from Gamedata, the server boots (no segfault) and logs the miss
- [ ] With dispatch-spawn unresolved, calling it from a plugin gives a clear error, no crash
- [ ] With the legacy game-event listener unresolved, firing an event to a client gives a clear error, no crash
- [ ] Behaviour with valid Gamedata is unchanged
