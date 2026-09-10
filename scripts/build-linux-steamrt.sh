#!/usr/bin/env bash
set -euo pipefail

# Build against the same older Linux runtime family as upstream CI. Building on
# a recent host distro can silently introduce GLIBC/GLIBCXX requirements that
# prevent dlopen on hosted CS2 servers.
repo=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)
build_dir=${1:-"$repo/build-steamrt"}
mkdir -p "$build_dir"
build_dir=$(cd "$build_dir" && pwd)
sysroot=${SNIPER_SYSROOT:?Set SNIPER_SYSROOT to an exported Steam Runtime sniper SDK root}
sysroot=$(cd "$sysroot" && pwd)
# Exported images contain absolute symlinks. Relocate those within the sysroot
# before using it from another container, otherwise they resolve to host files.
python3 - "$sysroot" <<'PY'
import os, sys
from pathlib import Path
root = Path(sys.argv[1])
for directory, dirs, files in os.walk(root, followlinks=False):
    for name in dirs + files:
        p = Path(directory) / name
        if p.is_symlink() and os.readlink(p).startswith('/'):
            target = os.path.relpath(root / os.readlink(p).lstrip('/'), p.parent)
            p.unlink()
            p.symlink_to(target)
PY
image=${STEAMRT_IMAGE:-registry.gitlab.steamos.cloud/steamrt/steamrt4/sdk@sha256:2ff6220464894964266c7dbda1b91ca24e433a58ad3edff26571a2a9e5163e00}
export SEMVER=${SEMVER:-1.0.374-khs-khook-steamrt}
export GITHUB_SHA_SHORT=${GITHUB_SHA_SHORT:-$(git -C "$repo" rev-parse --short=7 HEAD)}

docker run --rm -i --network host --user "$(id -u):$(id -g)" \
    -e HOME=/tmp -e SEMVER -e GITHUB_SHA_SHORT \
    -v "$repo:/src" -v "$build_dir:/build" -v "$sysroot:/opt/sniper:ro" -w /src "$image" \
    bash -s -- "${JOBS:-6}" <<'BUILD'
set -euo pipefail
cmake --fresh -S /src -B /build -G Ninja -DCMAKE_BUILD_TYPE=Release -DCMAKE_SKIP_RPATH=ON \
    -DCMAKE_C_COMPILER=/opt/sniper/usr/bin/gcc \
    -DCMAKE_CXX_COMPILER=/opt/sniper/usr/bin/g++ \
    -DCMAKE_ASM_COMPILER=/opt/sniper/usr/bin/gcc -DCMAKE_SYSROOT=/opt/sniper \
    -DCMAKE_AR=/usr/bin/ar -DCMAKE_RANLIB=/usr/bin/ranlib \
    -DCMAKE_FIND_ROOT_PATH_MODE_PROGRAM=NEVER
cmake --build /build -j "$1"

plugin=/build/addons/counterstrikesharp/bin/linuxsteamrt64/counterstrikesharp.so
versions=$(readelf --version-info "$plugin")
glibc=$(printf '%s\n' "$versions" | grep -oE 'GLIBC_[0-9.]+' | sort -Vu | tail -1)
glibcxx=$(printf '%s\n' "$versions" | grep -oE 'GLIBCXX_[0-9.]+' | sort -Vu | tail -1)
printf 'Required runtime: %s, %s\n' "$glibc" "$glibcxx"
test "$(printf '%s\n' "$glibc" GLIBC_2.31 | sort -V | tail -1)" = GLIBC_2.31
test "$(printf '%s\n' "$glibcxx" GLIBCXX_3.4.28 | sort -V | tail -1)" = GLIBCXX_3.4.28
if readelf -d "$plugin" | grep -Eq 'RPATH|RUNPATH'; then
    echo 'Unexpected build-machine runtime search path' >&2
    exit 1
fi

# Actual loading requires the game's libtier0 allocator, not the SDK link stub.
BUILD
