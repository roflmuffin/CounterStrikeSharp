# 14: CI jobs for Proxy Mode

Spec: `.scratch/hardening-d-own-loader/spec.md` (Testing Decisions). Extends spec C's I7 per-build CI.

**What to build:** Add two jobs to the per-build CI: "Proxy Mode, no Metamod: boot + native test suite" and "Proxy Mode under Metamod + CS2Fixes + MultiAddonManager: boot, map change, `MetaFactory`, shutdown". Both run on every new CS2 build next to the Plugin Mode job. Until this exists, 11's checklist runs by hand per release.

**Blocked by:** 06, spec C I7 (per-build CI, not yet specced)

**Status:** ready-for-agent

**Tier:** 3 dependent add-on · later

**Size / risk:** M · med. Third-party plugin builds in CI may lag game updates; their failure must not fail CounterStrikeSharp's job.

- [ ] Both jobs run on a new build and report pass/fail separately from the Plugin Mode job
- [ ] A broken third-party plugin marks the coexistence job "inconclusive", not "failed"
