# 12: Packaging and install docs

Spec: `.scratch/hardening-d-own-loader/spec.md` (Packaging and install, Rollout).

**What to build:** The release package ships the Bootstrap and `.vdf` (same locations), the core, the Proxy and the managed runtime. Extracting it over an existing install changes nothing about how the server loads. The install docs gain a Proxy Mode section:
- where to put the line, with and without Metamod
- rollback
- what happens after a game update overwrites `gameinfo.gi` (automatic fallback)
- `-css_fail_closed`
- guidance for hosting panels
- a short "which mode am I in" pointer to the boot checker

**Blocked by:** 05

**Status:** ready-for-agent

**Tier:** 2 add-on · must

**Size / risk:** S · low

- [ ] Release artifact contains all four parts for Linux; the Windows artifact is unchanged apart from the Bootstrap/core split
- [ ] Upgrade in place on a Plugin Mode server: identical boot
- [ ] INSTALL.md / docfx Proxy Mode section reviewed against a fresh install by following it literally
