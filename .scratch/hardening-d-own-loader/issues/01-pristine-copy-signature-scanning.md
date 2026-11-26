# 01: Pristine-copy signature scanning

Spec: `.scratch/hardening-d-own-loader/spec.md` (Pristine-copy signature scanning). Shared with spec A's resolver.

**What to build:** Signature scanning, for both core Gamedata and plugin-supplied signatures, reads a read-only mapping of the module file on disk instead of live memory, and translates matches back to live addresses through the section layout. Signatures then keep matching when a Metamod plugin, another KHook instance or anything else has inline-detoured the target. It works in both modes and is a prerequisite for Proxy Mode next to Metamod plugins.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

**Tier:** 0 standalone · must

**Size / risk:** S–M · low-med. The address translation must be right on ELF and PE; the PE file layout differs from the mapped layout.

- [ ] A signature on bytes that are detoured in memory but pristine on disk resolves, to the live address
- [ ] Address translation is unit-tested on synthetic ELF and PE layouts
- [ ] Every core Gamedata signature resolves on the current build to the same address as before
- [ ] Plugin-supplied signatures (managed `FindSignature` path) use the same scanner
- [ ] Windows builds and resolves as today
