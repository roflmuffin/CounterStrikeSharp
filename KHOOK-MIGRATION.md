# CounterStrikeSharp-KHS compatibility notes

Maintained by **K0di** on branch `khs-khook-fix`.

## Confirmed server result

K0di's Linux CS2 server successfully loaded CounterStrikeSharp and **29 managed plugins**, as reported by `meta list` and `css_plugins list` on September 10, 2026.

- Base: CounterStrikeSharp v1.0.374.
- Metamod: 2.0.0-dev+1466, plugin interface 18:18, commit `a8c72ea`.
- Compatibility fixes: native SourceHook registrations migrated to KHook, isolated ELF exports and a non-executable stack.
- Linux build: SteamRT4 container with a Steam Runtime sniper sysroot; requires GLIBC 2.29 and GLIBCXX 3.4.26.
- Native exports: `CreateInterface` and `InvokeNative`; no build-machine RPATH.

This confirms loading on the server. It does not establish that every gameplay callback, map transition or unload/reload scenario has been tested. Other native Metamod plugins need their own API 18 updates.

## Build and installation

Use `scripts/build-linux-steamrt.sh` with `SNIPER_SYSROOT` set to a relocated sniper SDK root. The script pins the SteamRT4 build image and checks ELF runtime requirements.

The complete installation archive is `counterstrikesharp-khs-khook-steamrt4-linux-with-runtime.zip`. It includes the native binary, managed API, default supporting files and .NET 10.0.3 runtime. Extract under `game/csgo` with the server stopped, after backing up existing configuration.

The earlier Ubuntu 24.04 overlay required GLIBC 2.38 and failed on this server. Use the SteamRT4 package instead.

## Credits

Original CounterStrikeSharp by Roflmuffin and contributors, under its existing license. KHS maintenance and server validation by **K0di**.

KHook migration reference: [mrc4tt commit d42e3f1](https://github.com/mrc4tt/CounterStrikeSharp/commit/d42e3f1c3b97a76ca62d3ce10426fead84cd5073). Metamod integration follows the [official Source 2 sample](https://github.com/alliedmodders/metamod-source/tree/a8c72eaf29d4bee30989cddbebfe5bd31e76db07/samples/s2_sample_mm).
