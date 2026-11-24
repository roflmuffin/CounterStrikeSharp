# 08: Entity IO, sound and spawn fail soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Engine Feature catalog). Migration batch.

**What to build:** Move these Engine Features onto the resolver and report, each disabled independently: `cssharp.entity/output-hooks`, `cssharp.entity/accept-input`, `cssharp.entity/add-io-event`, `cssharp.entity/emit-sound`, `cssharp.entity/dispatch-spawn`. A failed Binding no longer stops the Bindings after it from resolving.

**Blocked by:** 02, 05

**Status:** ready-for-agent

- [ ] Each listed Engine Feature is declared with its Bindings and appears in the startup report
- [ ] Breaking any one of their Bindings disables only that Engine Feature; the others keep working
- [ ] Calling a disabled one throws the typed exception; hooks on a disabled one never fire and log once
- [ ] End-to-end harness gains a case breaking the output-hook Binding: server up, entity output hooks disabled, accept-input still works
