# 14: Plugin signatures warn when ambiguous

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Plugin-facing API). ADR-0001 (no breaking existing plugins).

**What to build:** Raw signatures passed in by plugins (and Gamedata files plugins ship) keep first-match behaviour, but when a signature matches more than once, a warning names it with the match count. An opt-in strict mode makes an ambiguous plugin signature fail instead. A plugin memory function whose signature was not found reports that clearly instead of silently holding a null handle.

**Blocked by:** 04

**Status:** resolved

**Tier:** 2 add-on · later

- [ ] An ambiguous plugin signature still resolves to the first match and logs one warning with the count
- [ ] With strict mode on, the same signature fails with an "ambiguous" reason
- [ ] A plugin signature that is not found logs a clear message at creation, and calling it throws a clear error
- [ ] Existing plugins with unique signatures see no change

## Comments

- Resolved on `hardening/spec-a` (merge of `spec-a/14-plugin-signatures-warn-when-ambiguous`). Native `COUNT_SIGNATURE_MATCHES` reuses the resolver counter; managed `SignatureFunctions` warns once on ambiguity; strict opt-in via new `bool strict` ctors on memory functions; unbound functions throw `NativeException` naming the reason. Managed 38/38, doctest pass. Not exercised on a live server. Not covered: raw `NativeAPI.FindSignature`.
