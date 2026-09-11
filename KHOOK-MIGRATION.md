# PlayCup CounterStrikeSharp (`playcup` branch)

`main` tracks [upstream](https://github.com/roflmuffin/CounterStrikeSharp). `playcup` is the patched tree used on PlayCup match servers.

## Why this branch exists

Metamod:Source 2.0-dev **1461+** raised the plugin API from 17 to 18 (KHook). Upstream v1.0.374 still reports API 17, so Metamod refuses to load it:

```
Plugin uses old SourceHook Metamod build, probably 1.12.x or an early 2.0 version (17 < 18).
```

Current CS2 also **segfaults on Metamod 1460**. Do not downgrade the loader. This branch ports native hooks to KHook (from [PR #1417](https://github.com/roflmuffin/CounterStrikeSharp/pull/1417)) and pins `libraries/metamod-source` at `a8c72ea` (API 18).

## Build

Linux binaries are produced by GitHub Actions (Steam Runtime sniper SDK, same as upstream). Do not compile the native plugin on the host.

- Workflow: `.github/workflows/build-playcup.yml`
- Rolling release tag: `playcup`
- Asset: `counterstrikesharp-with-runtime-linux-playcup.zip`

Install on a game server with `make cs2addon-css-playcup-refresh` from `game-servers/cs2mm`.
