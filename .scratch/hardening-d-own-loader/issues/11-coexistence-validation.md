# 11: Coexistence validation with Metamod plugins

Spec: `.scratch/hardening-d-own-loader/spec.md` (KHook, Testing Decisions → live matrix).

**What to build:** Run and record the full live matrix from the spec on Linux (Plugin Mode unchanged; Proxy Mode without Metamod; under Metamod with CS2Fixes and MultiAddonManager; Metamod core removed; line lost; wrong order; core failure). The run covers:
- GameFrame, ClientCommand and map change in all plugins
- `meta unload` of CS2Fixes, then of the Bootstrap, then server shutdown, all without crashes
- confirmation that CounterStrikeSharp's own KHook is never freed

Turn the matrix into a per-release checklist under `.scratch/hardening-d-own-loader/release-checklist.md`, and document the hook-order difference for plugin authors.

**Blocked by:** 06, 07

**Status:** ready-for-human

**Tier:** 3 dependent add-on · must

**Size / risk:** M · med-high. Live servers with third-party plugins; teardown order across two KHook instances is only inferred.

- [ ] All matrix rows pass, results recorded with build id and plugin versions
- [ ] Release checklist committed
- [ ] Hook-order note in the plugin docs (08's page)
