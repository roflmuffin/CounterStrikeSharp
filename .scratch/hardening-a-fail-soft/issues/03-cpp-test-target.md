# 03: C++ test target

Spec: `.scratch/hardening-a-fail-soft/spec.md` (Testing Decisions, seam 1 producer side).

**What to build:** A native unit test target using doctest (header-only, no new CI dependencies) that builds and runs in CI on Linux, with one sanity test. This is the home for Binding resolver tests from ticket 04 onward. Kept as its own change so it is reviewable in isolation and upstreamable later.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [ ] The test target builds alongside the plugin without changing the plugin build output
- [ ] CI runs the native tests on Linux and fails the build on a failing test
- [ ] One sanity test exercising existing pure code (e.g. hex parsing) passes
- [ ] Windows build still succeeds (running tests on Windows is optional)
