# 13: Windows Proxy

Spec: `.scratch/hardening-d-own-loader/spec.md` (Proxy, Rollout). Should follow the Linux release; not in the must set.

**What to build:** `addons/counterstrikesharp/bin/win64/server.dll`, with the same rules as the Linux Proxy:
- kernel32 imports only; everything else via `GetProcAddress`
- absolute paths from its own module path
- pure pass-through, the same slots with compare-before-restore, no byte signatures
- ABI check, fail open / `-css_fail_closed`

It also handles `AllowDedicatedServers` as Metamod's loader does. 06–10 then apply on Windows too. Windows Plugin Mode stays the default and unchanged.

**Blocked by:** 05, 06

**Status:** ready-for-agent

**Tier:** 3 dependent add-on · later (should-follow)

**Size / risk:** M–L · med-high. No Windows binary analysis done yet; needs a live Windows server.

- [ ] `dumpbin /imports` shows kernel32 only
- [ ] Live Windows: Proxy Mode without Metamod and under Metamod (+ CS2Fixes) pass 11's matrix rows
- [ ] Windows Plugin Mode unchanged
