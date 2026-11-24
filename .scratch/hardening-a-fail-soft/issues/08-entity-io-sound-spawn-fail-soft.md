# 08: Entity IO, sound and spawn fail soft

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Engine Feature catalog). Migration batch.

**What to build:** Move these Engine Features onto the resolver and report, each disabled independently: `cssharp.entity/output-hooks`, `cssharp.entity/accept-input`, `cssharp.entity/add-io-event`, `cssharp.entity/emit-sound`, `cssharp.entity/dispatch-spawn`. A failed Binding no longer stops the Bindings after it from resolving.

**Blocked by:** 02, 05

**Status:** resolved

**Tier:** 2 add-on · later

- [ ] Each listed Engine Feature is declared with its Bindings and appears in the startup report
- [ ] Breaking any one of their Bindings disables only that Engine Feature; the others keep working
- [ ] Calling a disabled one throws the typed exception; hooks on a disabled one never fire and log once
- [ ] End-to-end harness gains a case breaking the output-hook Binding: server up, entity output hooks disabled, accept-input still works

## Comments

- Resolved on `hardening/spec-a` (merge of `spec-a/08-entity-io-sound-spawn-fail-soft`). 5 native rows; EntityManager init no longer returns early for these keys; managed calls guard with `ThrowIfUnavailable`; `HookEntityOutput` uses `CheckRegistration`. doctest 258/258, managed 74/74. E2E `eng/e2e/cases/entity-io.sh`: valid + FireOutputInternal not found (server up, only output-hooks disabled, accept-input works). Only output-hooks broken in E2E; others unit-tested. Follow-up for review: `HookEntityOutput` passes plugin `Logger`, null if called from a ctor.
