# 13: Missing schema field throws

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Schema fields). I3.

**What to build:** Native schema lookup reports "class missing" and "field missing" explicitly instead of offset 0, and never dereferences a missing class. Managed schema access caches the explicit miss and throws a typed exception naming class and field on access, logged once per field; it never reads or writes at offset 0 for a missing field. Native core code that relies on specific schema fields declares them as Bindings of its Engine Feature, checked at boot.

**Blocked by:** 04

**Status:** ready-for-agent

**Tier:** 2 add-on · later

- [ ] Accessing a field that does not exist throws the typed exception with class and field; a second access throws again but does not log again
- [ ] An unknown class does not crash native lookups
- [ ] Existing fields behave exactly as before
- [ ] End-to-end harness gains a case accessing a non-existent field from a test plugin: exception, server up
