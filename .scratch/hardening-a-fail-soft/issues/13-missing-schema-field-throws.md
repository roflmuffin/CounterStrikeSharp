# 13: Missing schema field throws

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Schema fields). I3.

**What to build:** Native schema lookup reports "class missing" and "field missing" explicitly instead of offset 0, and never dereferences a missing class. Managed schema access caches the explicit miss and throws a typed exception naming class and field on access, logged once per field; it never reads or writes at offset 0 for a missing field. Native core code that relies on specific schema fields declares them as Bindings of its Engine Feature, checked at boot.

**Blocked by:** 04

**Status:** resolved

**Tier:** 2 add-on · later

- [ ] Accessing a field that does not exist throws the typed exception with class and field; a second access throws again but does not log again
- [ ] An unknown class does not crash native lookups
- [ ] Existing fields behave exactly as before
- [ ] End-to-end harness gains a case accessing a non-existent field from a test plugin: exception, server up

## Comments

- Merged to `hardening/spec-a` (`spec-a/13-missing-schema-field-throws`): native `schema::LookupField` + `LOOKUP_SCHEMA_FIELD`, resolver `BindingKind::SchemaField`, managed `SchemaFieldNotFoundException` (logged once per field), in-server tests in `NativeTestsPlugin/SchemaTests.cs`. Unit/builds pass. E2E pending harness; ticket stays open. Schema Bindings for native core fields to be declared by tickets 10 (RunThink fields) etc.
- E2E verified (orchestrator, harness run on tip dcd95571, valid Gamedata): all 5 `SchemaTests` missing-field/class cases pass in the smoke suite, `Schema field CBaseEntity::m_cssharpDoesNotExist not found` logged exactly once per log sink, server stayed up. Resolved.
