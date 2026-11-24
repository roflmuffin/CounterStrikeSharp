# V8 build inputs

V8 11.9.169.6 headers and CS2 Windows/Linux x86-64 libraries for the cs_script bridge.

- `headers/`: [V8](https://github.com/v8/v8) submodule pinned to tag `11.9.169.6`;
  CMake uses its `include/` directory.
- `lib/linuxsteamrt64/`: the server libraries previously used by the local cs_script build.
  `libv8.so` links the bridge; the other five libraries are its V8 dependencies.
- `lib/win64/`: the `v8.lib` import library and `v8.def` export definition used to
  generate it. Windows DLLs are supplied by CS2 and are not stored here.
- `LICENSE*`: upstream V8 and dependency notices for the 11.9.169.6 release.
- `SHA256SUMS`: checksums of the vendored libraries.

Build from the repository root:

```sh
git submodule update --init --recursive
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build
```

The cs_script bridge is always built on Windows and Linux. CMake uses these files by default.
`CSS_V8_INCLUDE_DIR` and `CSS_V8_LIBRARY` can override them when updating to another
server build. To clear previously cached paths, configure with `-U 'CSS_V8_*'`.

The libraries are link inputs and are not copied into the addon. At runtime,
CS2 supplies `v8.dll` or `libv8.so` and their dependencies. Update the headers and libraries
together when changing V8 versions.

Regenerate the Windows import library from the checked-in export definition:

```sh
llvm-dlltool -m i386:x86-64 -D v8.dll -d libraries/v8/lib/win64/v8.def -l libraries/v8/lib/win64/v8.lib
```

On a Visual Studio developer prompt, `lib /machine:x64 /def:v8.def /out:v8.lib`
provides the equivalent import library. Update `v8.def` from the replacement
DLL's export table when updating V8.
