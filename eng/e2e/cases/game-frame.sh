#!/usr/bin/env bash
# End-to-end cases for cssharp.server/game-frame (Slot Binding ISource2Server::GameFrame), hooked at the slot index the
# boot Vtable Check verified.
#
#   eng/e2e/cases/game-frame.sh [run.sh options...]     e.g. --no-build
#
# valid:     GameFrame's fingerprint window matches the live CSource2Server vtable at its Gamedata index; every Binding
#            resolves (none healed, GameFrame not unverified, no vtable size warning); ticks, timers and next-frame tasks
#            run. Slot Bindings in modules not fingerprinted yet (network messages, schema system) are unverified.
# healed:    Gamedata states GameFrame one slot too low, as 1.41.9.7 Gamedata would on 1.42.0.1; the window matches
#            once, one slot later, so the Binding heals at 20: the native log and the startup report list the Heal,
#            plugins see it in the Engine Feature status, ticks and timers run, and the installed Gamedata is unchanged.
# corrupted: GameFrame's own fingerprint in Gamedata is wrong; the Binding fails with "fingerprint mismatch", only
#            game-frame is disabled, the startup report names it and the server stays up. No smoke test (it waits on
#            frames, which no longer tick).
# Exits non-zero if any case fails; each case's output dir is printed by run.sh.
set -uo pipefail

run=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/run.sh

# Bash for --check: every named smoke test has the given outcome in smoke-results.json.
outcome() {
    local want=$1; shift
    local names
    names=$(printf '"NativeTestsPlugin.%s",' "$@")
    printf 'jq -e --arg want %q '\''[.tests[] | select(.name | IN(%s)) | .outcome == $want] | length == %d and all'\'' "$E2E_RESULTS"' \
        "$want" "${names%,}" "$#"
}

status=0

echo "== game-frame: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved(, [0-9]+ unverified)?$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-no-log 'unverified.*ISource2Server::GameFrame|[Hh]ealed|Vtable of [A-Za-z0-9_]+ \([a-z0-9]+\) has [0-9]+ slots' \
    --check "$(outcome passed GameFrameTests.GameFrame_IsVerifiedAndAvailable GameFrameTests.GameFrame_Ticks \
        TimerTests.CreateTimer_ExecutesCallback FrameSchedulingTests.QueueTaskForNextFrame_ExecutesCallback \
        FrameSchedulingTests.NextFrame_HighLevelApi_ExecutesCallback)" ||
    status=1

echo "== game-frame: GameFrame one slot too low (1.41.9.7 Gamedata on 1.42.0.1)"
"$run" "$@" --no-build \
    --gamedata-jq '."ISource2Server::GameFrame".offsets.linux = 19 | ."ISource2Server::GameFrame".vtable.linux.size = 104' \
    --exec css_engine_features \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 1 healed(, [0-9]+ unverified)?$' \
    --expect-log 'Healed ISource2Server::GameFrame: CSource2Server \(server\) slot 19 -> 20, window \[[0-9a-f]{16}, [0-9a-f]{16}, [0-9a-f]{16}\] matched once within 16 slots' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-log '^ +Healed ISource2Server::GameFrame: CSource2Server \(server\) slot 19 -> 20, window ' \
    --expect-log '\[EngineFeatures\] cssharp\.server/game-frame available, healed \(ISource2Server::GameFrame: slot 19 -> 20\)' \
    --check "$(outcome passed GameFrameTests.GameFrame_IsVerifiedAndAvailable GameFrameTests.GameFrame_Ticks \
        TimerTests.CreateTimer_ExecutesCallback FrameSchedulingTests.QueueTaskForNextFrame_ExecutesCallback \
        FrameSchedulingTests.NextFrame_HighLevelApi_ExecutesCallback)" \
    --check 'cmp gamedata.installed.json gamedata.after-run.json && jq -e ".\"ISource2Server::GameFrame\".offsets.linux == 19" gamedata.after-run.json' ||
    status=1

echo "== game-frame: GameFrame's fingerprint corrupted"
"$run" "$@" --no-build --no-smoke \
    --gamedata-jq '."ISource2Server::GameFrame".vtable.linux.window[1] = "0000000000000000"' \
    --expect-log 'Bindings for build [^:]+: 1 of [0-9]+ failed: ISource2Server::GameFrame \(fingerprint mismatch\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log '^ +cssharp\.server/game-frame disabled \(ISource2Server::GameFrame: fingerprint mismatch\)' ||
    status=1

exit $status
