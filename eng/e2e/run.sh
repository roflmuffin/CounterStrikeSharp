#!/usr/bin/env bash
#
# End-to-end harness: build CounterStrikeSharp from this checkout, install it
# into an isolated copy of a local CS2 dedicated server, boot the server, run
# the native test plugin's smoke test and collect the results.
#
# Usage: eng/e2e/run.sh [options]
#
# Build:
#   --no-build              Reuse this worktree's last staged package (skip native + managed builds).
#   --build-only            Build and stage the package, then exit.
#
# Install (applied to the server copy only, never to this checkout):
#   --gamedata FILE         Install FILE as gamedata.json instead of the built one.
#   --gamedata-jq FILTER    Apply a jq filter to the installed gamedata.json. Repeatable,
#                           applied in order. Examples:
#                             'del(.Host_Say)'                              key missing
#                             'del(.Host_Say.signatures.linux)'             per-OS entry missing
#                             '.Host_Say.signatures.linux = "55 48 89 E5"'  replace signature
#                             '.CCSPlayerController_ChangeTeam.offsets.linux = 999'
#   --core-jq FILTER        Apply a jq filter to the installed core.json. Repeatable.
#   --plugin DIR            Install a built plugin directory as plugins/<basename DIR>. Repeatable.
#   --plugin-project PROJ   Build PROJ (.csproj, path relative to the repo root or absolute)
#                           and install its output as plugins/<project name>. Repeatable.
#
# Run:
#   --map NAME              Map to boot (default: de_dust2; competitive, warmup held).
#   --server-arg ARG        Extra server command line argument, before +map. Repeatable.
#   --exec CMD              Server console command to issue after load, before the
#                           smoke test. Repeatable.
#   --ready-regex REGEX     Also wait for a console line matching REGEX before the smoke test.
#                           Repeatable. Built in: Native Tests plugin loaded, map activated.
#   --no-smoke              Do not run the smoke test.
#   --boot-timeout SEC      Wait this long for CounterStrikeSharp and the test plugin to load (180).
#   --smoke-timeout SEC     Wait this long for smoke-results.json (900).
#   --settle SEC            Keep the server up this long after the smoke test before
#                           checking it is still alive (10).
#
# Checks (any failure makes the run exit 5):
#   --expect-log REGEX      Extended regex that must match a line in the collected logs.
#   --expect-no-log REGEX   Extended regex that must not match any line in the collected logs.
#   --check CMD             Run CMD with bash in the output dir (env E2E_OUT, E2E_RESULTS).
#   --require-smoke-pass    Fail when the smoke test reports any failed test.
#
# Output:
#   --out DIR               Output dir (default: $CSS_E2E_HOME/runs/<timestamp>).
#
# Exit codes: 0 ok, 1 setup/build error, 2 boot failure (exit or timeout before load),
#             3 server died after load, 4 smoke test timed out, 5 checks failed.
#
# Environment:
#   CSS_E2E_HOME        Cache root (default ~/.cache/cssharp-e2e): build dirs, downloads, server copy.
#   CSS_E2E_SERVER_SRC  Pristine server install to copy once (default ~/.cache/cs2-14201). Never written.
#   CSS_E2E_LOCK        Lock file serialising server runs (default ~/.cache/cssharp-e2e.lock).
#   CSS_E2E_S2SDK_SHA   s2sdk commit used for the native build (default: alliedmodders/s2sdk#128 head).
#                       Harness-only override; the libraries/hl2sdk-cs2 submodule pointer is not used.
#   CSS_E2E_MM_BUILD    Metamod:Source drop to install (default mmsource-2.0.0-git1491-linux).
#   CSS_E2E_STEAMCLIENT steamclient.so for the server's isolated HOME, copied once
#                       (default ~/.cache/cs2-poc-steamcmd/linux64/steamclient.so; never a Steam client dir).
#   CSS_E2E_NATIVE      Native toolchain: "container" (default; the steamrt sniper SDK image CI uses,
#                       binary also loads inside steamrt) or "host" (host compiler; needs host glibc).
#   CSS_E2E_SNIPER_IMAGE  Image for container builds (default registry.gitlab.steamos.cloud/steamrt/sniper/sdk:latest).
#   CC, CXX             Host-mode compilers (cmake default when unset; host clang 23 cannot build spdlog).

set -euo pipefail

HERE=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
REPO=$(cd "$HERE/../.." && pwd)

E2E_HOME=${CSS_E2E_HOME:-$HOME/.cache/cssharp-e2e}
SERVER_SRC=${CSS_E2E_SERVER_SRC:-$HOME/.cache/cs2-14201}
LOCK=${CSS_E2E_LOCK:-$HOME/.cache/cssharp-e2e.lock}
S2SDK_SHA=${CSS_E2E_S2SDK_SHA:-ccd5b3e116864c0e7f6a0044046223cb772d33b7}
MM_BUILD=${CSS_E2E_MM_BUILD:-mmsource-2.0.0-git1491-linux}

DL=$E2E_HOME/dl
MIRROR=$E2E_HOME/src
NATIVE_MODE=${CSS_E2E_NATIVE:-container}
SNIPER_IMAGE=${CSS_E2E_SNIPER_IMAGE:-registry.gitlab.steamos.cloud/steamrt/sniper/sdk:latest}
NATIVE_BUILD=$E2E_HOME/build/native-$NATIVE_MODE
# Per-worktree managed output and staged package, so parallel worktrees never
# install each other's build between the build lock and the server lock.
REPO_KEY=$(printf '%s' "$REPO" | sha1sum | cut -c1-10)
MANAGED_BUILD=$E2E_HOME/build/managed-$REPO_KEY
PKG=$E2E_HOME/pkg/$REPO_KEY
SERVER=$E2E_HOME/server
HOME_DIR=$E2E_HOME/home
STEAMCLIENT=${CSS_E2E_STEAMCLIENT:-$HOME/.cache/cs2-poc-steamcmd/linux64/steamclient.so}

do_build=1 build_only=0 do_smoke=1 require_pass=0
map=de_dust2 boot_timeout=180 smoke_timeout=900 settle=10 out=""
gamedata_file=""
gamedata_jq=() core_jq=() plugins=() plugin_projects=() execs=() server_args=()
# Boot is done when every one of these extended regexes matches a console line.
ready=('Finished loading plugin Native Tests' 'Host activate: Loading \(')
expect=() expect_not=() checks=()

die() { echo "e2e: $*" >&2; exit 1; }
log() { echo "e2e: $*" >&2; }

while [ $# -gt 0 ]; do
    case $1 in
        --no-build) do_build=0 ;;
        --build-only) build_only=1 ;;
        --gamedata) gamedata_file=$(realpath "$2"); shift ;;
        --gamedata-jq) gamedata_jq+=("$2"); shift ;;
        --core-jq) core_jq+=("$2"); shift ;;
        --plugin) plugins+=("$(realpath "$2")"); shift ;;
        --plugin-project) plugin_projects+=("$2"); shift ;;
        --map) map=$2; shift ;;
        --server-arg) server_args+=("$2"); shift ;;
        --exec) execs+=("$2"); shift ;;
        --ready-regex) ready+=("$2"); shift ;;
        --no-smoke) do_smoke=0 ;;
        --boot-timeout) boot_timeout=$2; shift ;;
        --smoke-timeout) smoke_timeout=$2; shift ;;
        --settle) settle=$2; shift ;;
        --expect-log) expect+=("$2"); shift ;;
        --expect-no-log) expect_not+=("$2"); shift ;;
        --check) checks+=("$2"); shift ;;
        --require-smoke-pass) require_pass=1 ;;
        --out) out=$2; shift ;;
        -h|--help) sed -n '2,/^$/p' "${BASH_SOURCE[0]}" | sed 's/^# \{0,1\}//'; exit 0 ;;
        *) die "unknown option: $1 (see --help)" ;;
    esac
    shift
done

for tool in cmake ninja dotnet jq rsync curl flock setsid ss; do
    command -v "$tool" >/dev/null || die "missing tool: $tool"
done
mkdir -p "$DL" "$E2E_HOME/build"

# Download URL to $DL/NAME once.
fetch() {
    local url=$1 name=$2
    if [ ! -s "$DL/$name" ]; then
        log "downloading $url"
        curl -fsSL -o "$DL/$name.part" "$url"
        mv "$DL/$name.part" "$DL/$name"
    fi
}

# Extract a cached tarball into DIR once.
extract_once() {
    local tarball=$1 dir=$2
    shift 2
    if [ ! -e "$dir/.extracted" ]; then
        rm -rf "$dir"
        mkdir -p "$dir"
        tar -xzf "$tarball" -C "$dir" "$@"
        touch "$dir/.extracted"
    fi
}

dotnet_runtime_version() {
    grep -oE 'host/fxr/[0-9]+\.[0-9]+\.[0-9]+' "$REPO/src/scripting/dotnet_host.cpp" | head -1 | cut -d/ -f3
}

build_native() {
    local sdk=$E2E_HOME/s2sdk/$S2SDK_SHA
    fetch "https://codeload.github.com/alliedmodders/s2sdk/tar.gz/$S2SDK_SHA" "s2sdk-$S2SDK_SHA.tar.gz"
    extract_once "$DL/s2sdk-$S2SDK_SHA.tar.gz" "$sdk" --strip-components=1

    # Mirror the native sources by content (no mtimes) so the build dir stays
    # incremental across checkouts; swap in the s2sdk override.
    mkdir -p "$MIRROR/libraries"
    rsync -rlc --delete --exclude=/libraries/hl2sdk-cs2 --exclude=/configs/addons/metamod/ \
        --include='/CMakeLists.txt' --include='/makefiles/***' --include='/src/***' \
        --include='/libraries/***' --include='/configs/***' --exclude='*' \
        "$REPO/" "$MIRROR/"
    ln -sfn "$sdk" "$MIRROR/libraries/hl2sdk-cs2"

    log "native build ($NATIVE_MODE, s2sdk $S2SDK_SHA)"
    local run=()
    if [ "$NATIVE_MODE" = container ]; then
        # Same image as CI build_linux; same absolute paths so the build dir stays incremental.
        command -v docker >/dev/null || die "CSS_E2E_NATIVE=container needs docker (or set CSS_E2E_NATIVE=host)"
        local user=(--user "$(id -u):$(id -g)")
        # Rootless docker already maps container root to the calling user.
        docker info -f '{{.SecurityOptions}}' 2>/dev/null | grep -q rootless && user=()
        run=(docker run --rm "${user[@]}" -v "$E2E_HOME:$E2E_HOME" -w "$E2E_HOME" "$SNIPER_IMAGE")
    fi
    "${run[@]}" cmake -S "$MIRROR" -B "$NATIVE_BUILD" -G Ninja \
        -DCMAKE_BUILD_TYPE=Release >"$E2E_HOME/build/native-configure.log" 2>&1 \
        || { tail -30 "$E2E_HOME/build/native-configure.log"; die "native configure failed"; }
    "${run[@]}" cmake --build "$NATIVE_BUILD" >"$E2E_HOME/build/native-build.log" 2>&1 \
        || { grep -E 'error:|FAILED:' "$E2E_HOME/build/native-build.log" | head -30; die "native build failed (see $E2E_HOME/build/native-build.log)"; }
}

# The API's compat check rewrites CompatibilitySuppressions.xml in the checkout; the harness skips it.
DOTNET_PROPS="-p:ApiCompatValidateAssemblies=false -p:GenerateCompatibilitySuppressionFile=false"

# Build a managed project with bin/obj outside the checkout; prints the output dir.
build_managed_project() {
    local proj=$1 name
    name=$(basename "$proj" .csproj)
    dotnet build -c Release --nologo -v q --artifacts-path "$MANAGED_BUILD" $DOTNET_PROPS "$proj" \
        >"$E2E_HOME/build/managed-$name.log" 2>&1 \
        || { grep -E 'error' "$E2E_HOME/build/managed-$name.log" | head -30; die "managed build of $name failed"; }
    # Artifacts layout: bin/<project>/release
    echo "$MANAGED_BUILD/bin/$name/release"
}

build_all() {
    local lock_fd
    exec {lock_fd}>"$E2E_HOME/build.lock"
    flock "$lock_fd"

    build_native

    log "managed build"
    dotnet publish -c Release --nologo -v q --artifacts-path "$MANAGED_BUILD" $DOTNET_PROPS \
        "$REPO/managed/CounterStrikeSharp.API/CounterStrikeSharp.API.csproj" \
        >"$E2E_HOME/build/managed-api.log" 2>&1 \
        || { grep -E 'error' "$E2E_HOME/build/managed-api.log" | head -30; die "managed API build failed"; }
    local tests
    tests=$(build_managed_project "$REPO/managed/CounterStrikeSharp.Tests.Native/NativeTestsPlugin.csproj")

    # Stage the package: configs, native binary, metamod vdf, API, native test plugin.
    log "staging package in $PKG"
    rm -rf "$PKG.new"
    mkdir -p "$PKG.new"
    cp -a "$MIRROR/configs/addons" "$PKG.new/"
    install -D -m 755 "$NATIVE_BUILD/addons/counterstrikesharp/bin/linuxsteamrt64/counterstrikesharp.so" \
        "$PKG.new/addons/counterstrikesharp/bin/linuxsteamrt64/counterstrikesharp.so"
    mkdir -p "$PKG.new/addons/counterstrikesharp/api" "$PKG.new/addons/counterstrikesharp/plugins/NativeTestsPlugin"
    cp -a "$MANAGED_BUILD/publish/CounterStrikeSharp.API/release/." "$PKG.new/addons/counterstrikesharp/api/"
    cp -a "$tests/." "$PKG.new/addons/counterstrikesharp/plugins/NativeTestsPlugin/"
    local dirty=""
    [ -n "$(git -C "$REPO" status --porcelain --untracked-files=no)" ] && dirty="-dirty"
    echo "$(git -C "$REPO" rev-parse HEAD)$dirty s2sdk=$S2SDK_SHA native=$NATIVE_MODE" >"$PKG.new/BUILD"
    rm -rf "$PKG"
    mv "$PKG.new" "$PKG"

    exec {lock_fd}>&-
}

if [ "$do_build" = 1 ]; then
    build_all
else
    [ -d "$PKG/addons" ] || die "no staged package in $PKG; run without --no-build first"
fi

# Extra plugin projects are built even with --no-build (they are cheap and run-specific).
for proj in "${plugin_projects[@]}"; do
    case $proj in /*) ;; *) proj=$REPO/$proj ;; esac
    plugins+=("$(build_managed_project "$proj")")
done

[ "$build_only" = 1 ] && { log "package staged in $PKG ($(cat "$PKG/BUILD"))"; exit 0; }

# Metamod and the .NET runtime are downloaded once.
fetch "https://mms.alliedmods.net/mmsdrop/2.0/$MM_BUILD.tar.gz" "$MM_BUILD.tar.gz"
extract_once "$DL/$MM_BUILD.tar.gz" "$E2E_HOME/$MM_BUILD"
DOTNET_VERSION=$(dotnet_runtime_version)
[ -n "$DOTNET_VERSION" ] || die "could not read the .NET runtime version from src/scripting/dotnet_host.cpp"
fetch "https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/$DOTNET_VERSION/aspnetcore-runtime-$DOTNET_VERSION-linux-x64.tar.gz" \
    "aspnetcore-runtime-$DOTNET_VERSION-linux-x64.tar.gz"
extract_once "$DL/aspnetcore-runtime-$DOTNET_VERSION-linux-x64.tar.gz" "$E2E_HOME/dotnet-$DOTNET_VERSION"

out=${out:-$E2E_HOME/runs/$(date +%Y%m%d-%H%M%S)}
mkdir -p "$out"
out=$(cd "$out" && pwd)
console=$out/server-console.log

# ---------------------------------------------------------------------------
# Everything below touches the shared server copy: one run at a time.
exec 9>"$LOCK"
if ! flock -n 9; then
    log "waiting for $LOCK"
    flock 9
fi

# The server copy is made once. On copy-on-write filesystems it is a reflink
# copy (no shared data at all). Otherwise it is a hardlink copy whose small
# files (the ones a server may rewrite) are replaced by private copies.
if [ ! -e "$SERVER/.ready" ]; then
    [ -x "$SERVER_SRC/game/bin/linuxsteamrt64/cs2" ] || die "no server at $SERVER_SRC"
    log "creating server copy $SERVER from $SERVER_SRC"
    rm -rf "$SERVER"
    if cp -a --reflink=always "$SERVER_SRC" "$SERVER" 2>/dev/null; then
        log "server copy: reflink"
    else
        rm -rf "$SERVER"
        cp -al "$SERVER_SRC" "$SERVER"
        find "$SERVER" -type f -size -4096k -print0 | while IFS= read -r -d '' f; do
            cp -p "$f" "$f.e2e-tmp" && mv -f "$f.e2e-tmp" "$f"
        done
        log "server copy: hardlinks (small files private)"
    fi
    touch "$SERVER/.ready"
fi

game=$SERVER/game
addons=$game/csgo/addons

# Replace a file in the server copy by writing a new inode (never through a hardlink).
replace_file() {
    local target=$1 src=$2
    cp "$src" "$target.e2e-tmp"
    mv -f "$target.e2e-tmp" "$target"
}

log "installing Metamod ($MM_BUILD), CounterStrikeSharp and plugins"
rm -rf "$addons"
mkdir -p "$addons"
cp -a "$E2E_HOME/$MM_BUILD/addons/." "$addons/"
cp -a "$PKG/addons/." "$addons/"
cp -a --reflink=auto "$E2E_HOME/dotnet-$DOTNET_VERSION" "$addons/counterstrikesharp/dotnet"
rm -f "$addons/counterstrikesharp/dotnet/.extracted"
for p in "${plugins[@]}"; do
    [ -d "$p" ] || die "plugin dir not found: $p"
    name=$(basename "$p")
    [ "$name" = release ] && name=$(basename "$(dirname "$p")")
    rm -rf "$addons/counterstrikesharp/plugins/$name"
    cp -a "$p" "$addons/counterstrikesharp/plugins/$name"
done

css_cfg=$addons/counterstrikesharp/configs
# Auto update would overwrite the gamedata under test.
jq '.AutoUpdateEnabled = false' "$css_cfg/core.example.json" >"$css_cfg/core.json"
for f in "${core_jq[@]}"; do
    jq "$f" "$css_cfg/core.json" >"$css_cfg/core.json.tmp" && mv "$css_cfg/core.json.tmp" "$css_cfg/core.json"
done

gd=$addons/counterstrikesharp/gamedata/gamedata.json
[ -n "$gamedata_file" ] && cp "$gamedata_file" "$gd"
for f in "${gamedata_jq[@]}"; do
    jq "$f" "$gd" >"$gd.tmp" || die "gamedata jq filter failed: $f"
    mv "$gd.tmp" "$gd"
done
cp "$gd" "$out/gamedata.installed.json"
cp "$PKG/BUILD" "$out/BUILD"

# gameinfo.gi: load Metamod first. Regenerated from the pristine copy each run.
gi_tmp=$(mktemp)
awk '!done && /^[ \t]*Game[ \t]+csgo[ \t\r]*$/ { print "\t\t\tGame\tcsgo/addons/metamod\r"; done=1 } { print }' \
    "$SERVER_SRC/game/csgo/gameinfo.gi" >"$gi_tmp"
grep -q 'csgo/addons/metamod' "$gi_tmp" || die "could not patch gameinfo.gi"
replace_file "$game/csgo/gameinfo.gi" "$gi_tmp"
rm -f "$gi_tmp"

# Pick an unused port on 127.0.0.1.
port=""
# Start at a random offset: other local servers (not under the lock) tend to sit on 27015/27215.
base=$((27400 + RANDOM % 400))
for p in $(seq "$base" $((base + 100))); do
    if ! ss -Hlnu "sport = :$p" | grep -q . && ! ss -Hlnt "sport = :$p" | grep -q .; then port=$p; break; fi
done
[ -n "$port" ] || die "no free port"

# Isolated HOME: the server must not see or write the user's Steam dirs. The
# game server still needs a steamclient.so in ~/.steam/sdk64; it is copied once
# from a steamcmd install.
if [ ! -s "$HOME_DIR/.steam/sdk64/steamclient.so" ]; then
    [ -s "$STEAMCLIENT" ] || die "no steamclient.so at $STEAMCLIENT (set CSS_E2E_STEAMCLIENT to a steamcmd linux64/steamclient.so)"
    install -D -m 755 "$STEAMCLIENT" "$HOME_DIR/.steam/sdk64/steamclient.so"
fi
fifo=$out/.console-in
rm -f "$fifo"
mkfifo "$fifo"

stamp=$out/.start
touch "$stamp"
run_id=e2e-$(date +%s)
results_src=$addons/counterstrikesharp/plugins/NativeTestsPlugin/smoke-results.json
native_log=$game/bin/linuxsteamrt64/counterstrikesharp.log # relative to the engine's working dir
rm -f "$results_src" "$native_log"

log "booting server: map $map, 127.0.0.1:$port, output $out"
(
    cd "$game"
    export HOME=$HOME_DIR
    export LD_LIBRARY_PATH="$game/bin/linuxsteamrt64${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
    ulimit -n 65535 2>/dev/null || true
    # No core dumps unless asked: the engine segfaults on every quit (see e2e notes).
    [ -n "${CSS_E2E_CORE:-}" ] || ulimit -c 0
    # Line-buffer stdout: block buffering into the log file hides the ready lines for minutes.
    exec setsid stdbuf -oL -eL ./bin/linuxsteamrt64/cs2 -dedicated -insecure -console -nobreakpad \
        +ip 127.0.0.1 -port "$port" +sv_lan 1 +tv_enable 0 +sv_hibernate_when_empty 0 \
        +game_type 0 +game_mode 1 "${server_args[@]}" +map "$map" <"$fifo" >"$console" 2>&1
) &
server_pid=$!
exec 8>"$fifo" # keep the server's stdin open
start_ts=$(date +%s)

stop_server() {
    { exec 8>&-; } 2>/dev/null || true
    if kill -0 "$server_pid" 2>/dev/null; then
        kill -TERM -- "-$server_pid" 2>/dev/null || kill -TERM "$server_pid" 2>/dev/null || true
        for _ in $(seq 1 20); do kill -0 "$server_pid" 2>/dev/null || break; sleep 0.5; done
        kill -KILL -- "-$server_pid" 2>/dev/null || kill -KILL "$server_pid" 2>/dev/null || true
    fi
    if [ -z "$server_exit" ]; then reap; fi
}
trap stop_server EXIT
trap 'exit 130' INT TERM HUP

send() { log "console> $*"; printf '%s\n' "$*" >&8; }
alive() { kill -0 "$server_pid" 2>/dev/null; }

status=ok rc=0
server_exit=""
reap() { server_exit=0; wait "$server_pid" 2>/dev/null || server_exit=$?; }

# Wait for CounterStrikeSharp, the native test plugin and the map to be up.
all_ready() {
    local re
    for re in "${ready[@]}"; do grep -qE -- "$re" "$console" 2>/dev/null || return 1; done
}
loaded=0
deadline=$((start_ts + boot_timeout))
while [ "$(date +%s)" -lt "$deadline" ]; do
    if ! alive; then reap; break; fi
    if all_ready; then loaded=1; sleep 3; break; fi
    sleep 1
done
boot_secs=$(( $(date +%s) - start_ts ))

if [ "$loaded" = 0 ]; then
    if [ -n "$server_exit" ]; then
        status="boot failure: server exited with code $server_exit after ${boot_secs}s"
    else
        status="boot failure: CounterStrikeSharp/Native Tests not loaded within ${boot_timeout}s"
    fi
    rc=2
else
    log "loaded after ${boot_secs}s"
    # The bot tests raise bot_quota on an empty server: bots must join without a human
    # and spawn alive (held warmup), or every test bot wait times out.
    send "bot_join_after_player 0; mp_warmup_pausetimer 1"
    for c in "${execs[@]}"; do send "$c"; sleep 1; done
    if [ "$do_smoke" = 1 ]; then
        send "css_smoke_test $run_id"
        deadline=$(( $(date +%s) + smoke_timeout ))
        while [ ! -s "$results_src" ] && [ "$(date +%s)" -lt "$deadline" ]; do
            if ! alive; then reap; break; fi
            sleep 2
        done
        if [ -s "$results_src" ]; then
            cp "$results_src" "$out/smoke-results.json"
        elif [ -n "$server_exit" ]; then
            status="server died during the smoke test (exit code $server_exit)"; rc=3
        else
            status="smoke test produced no results within ${smoke_timeout}s"; rc=4
        fi
    fi
    if [ "$rc" = 0 ]; then
        sleep "$settle"
        send "status"
        sleep 2
        if alive; then
            log "server stayed up"
            echo "server stayed up" >"$out/server-stayed-up"
        else
            reap
            status="server died after load (exit code $server_exit)"; rc=3
        fi
    fi
fi
total_secs=$(( $(date +%s) - start_ts ))

if alive; then
    send "quit"
    for _ in $(seq 1 30); do alive || break; sleep 0.5; done
fi
stop_server
trap - EXIT
shutdown_exit=$server_exit

# Collect logs.
cp "$native_log" "$out/counterstrikesharp-native.log" 2>/dev/null || true
if [ -d "$addons/counterstrikesharp/logs" ]; then
    cp -a "$addons/counterstrikesharp/logs" "$out/css-logs"
fi
rm -f "$fifo"

# Guard: nothing may have been written through a hardlink into the shared server files.
leaked=$(find "$SERVER" -xdev -type f -links +1 -newer "$stamp" 2>/dev/null | head -20 || true)
if [ -n "$leaked" ]; then
    echo "$leaked" >"$out/hardlink-writes.txt"
    log "WARNING: files shared by hardlink were modified (see $out/hardlink-writes.txt)"
    [ "$rc" = 0 ] && { status="hardlinked server files were modified"; rc=5; }
fi

# Smoke summary.
if [ -s "$out/smoke-results.json" ]; then
    jq -r '"smoke: total \(.total) passed \(.passed) failed \(.failed) skipped \(.skipped) errors \(.errors | length)"' \
        "$out/smoke-results.json" | tee "$out/smoke-summary.txt" >&2
    jq -r '.tests[] | select(.outcome == "failed") | .name' "$out/smoke-results.json" | sort >"$out/smoke-failed.txt"
    jq -r '.tests[] | select(.outcome == "skipped") | .name' "$out/smoke-results.json" | sort >"$out/smoke-skipped.txt"
    if [ "$require_pass" = 1 ] && [ "$(jq .failed "$out/smoke-results.json")" != 0 ] && [ "$rc" = 0 ]; then
        status="smoke test reported failures"; rc=5
    fi
fi

# Checks.
check_failures=0
logs=("$console" "$out"/counterstrikesharp-native.log)
[ -d "$out/css-logs" ] && while IFS= read -r -d '' f; do logs+=("$f"); done < <(find "$out/css-logs" -type f -print0)
for re in "${expect[@]}"; do
    if grep -hqE -- "$re" "${logs[@]}" 2>/dev/null; then log "check ok: log matches /$re/"
    else log "CHECK FAILED: no log line matches /$re/"; check_failures=$((check_failures + 1)); fi
done
for re in "${expect_not[@]}"; do
    if grep -hqE -- "$re" "${logs[@]}" 2>/dev/null; then log "CHECK FAILED: a log line matches /$re/"; check_failures=$((check_failures + 1))
    else log "check ok: no log line matches /$re/"; fi
done
for c in "${checks[@]}"; do
    if (cd "$out" && E2E_OUT=$out E2E_RESULTS=$out/smoke-results.json bash -c "$c"); then log "check ok: $c"
    else log "CHECK FAILED: $c"; check_failures=$((check_failures + 1)); fi
done
if [ "$check_failures" -gt 0 ] && [ "$rc" = 0 ]; then status="$check_failures check(s) failed"; rc=5; fi

{
    echo "status: $status"
    echo "exit: $rc"
    echo "build: $(cat "$PKG/BUILD")"
    echo "map: $map port: $port"
    echo "boot_seconds: $boot_secs"
    echo "run_seconds: $total_secs"
    echo "server_exit_code: $shutdown_exit"
    [ -f "$out/server-stayed-up" ] && echo "server stayed up: yes" || echo "server stayed up: no"
    [ -f "$out/smoke-summary.txt" ] && cat "$out/smoke-summary.txt"
} >"$out/summary.txt"
cat "$out/summary.txt" >&2
log "output: $out"
exit "$rc"
