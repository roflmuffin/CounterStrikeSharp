# 10: `-css_fail_closed` launch parameter

Spec: `.scratch/hardening-d-own-loader/spec.md` (Proxy, Failure).

**What to build:** When the server is launched with `-css_fail_closed` and the core is missing, fails to load, has the wrong ABI version or fails to start, the Proxy aborts server start with the reason instead of failing open. The Proxy reads the parameter from the command line and parses no config files. Plugin Mode ignores it.

**Blocked by:** 05

**Status:** ready-for-agent

**Tier:** 2 add-on · must

**Size / risk:** S · low

- [ ] Live: wrong-ABI core without the flag → server runs vanilla; with the flag → server exits with the reason
- [ ] Healthy core with the flag → normal boot
- [ ] Documented in the install docs (12)
