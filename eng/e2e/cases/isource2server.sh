#!/usr/bin/env bash
# End-to-end cases for the ISource2Server Slot Bindings: the GameFrame, PreWorldUpdate, hibernation, Steam API,
# hostname, fatal shutdown and not-in-game hooks and the IsPaused and GetEconItemSystem direct calls, each hooked or
# called at the slot index the boot Vtable Check verified, each backing its own Engine Feature.
#
#   eng/e2e/cases/isource2server.sh [run.sh options...]     e.g. --no-build
#
# valid:     every ISource2Server Slot Binding resolves by its window; no Heal; their Engine Features are available,
#            the PreWorldUpdate and hostname listeners fire, the direct calls return, frames and timers run.
# healed:    Gamedata states all ten one slot too low, as 1.41.9.7 Gamedata would on 1.42.0.1 (Valve inserted a
#            virtual before them); each heals one slot up, the startup report lists all ten Heals, the same listeners
#            fire and calls return, and the installed Gamedata is unchanged.
# corrupted: OnHostNameChanged's and IsPaused's own fingerprints are wrong; exactly hostname-changed and is-paused are
#            disabled (the hostname listener never fires, the IsPaused native throws), everything else still works.
# Hibernation, Steam API (the test server runs with sv_lan 1), fatal shutdown and not-in-game updates cannot be
# triggered here; their Engine Feature status is checked.
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

# Bash for --check: the ISource2ServerTests that did not pass are exactly the given ones (name prefixes), all 12 ran.
only_failed() {
    local prefixes=
    [[ $# -gt 0 ]] && prefixes=$(printf '"NativeTestsPlugin.ISource2ServerTests.%s",' "$@")
    printf 'jq -e '\''[.tests[] | select(.name | startswith("NativeTestsPlugin.ISource2ServerTests."))] as $t | ($t | length) == 12 and ([$t[] | select(.outcome != "passed") | .name] | sort) == ([$t[] | .name | select(. as $n | [%s] | any(. as $p | $n | startswith($p)))] | sort)'\'' "$E2E_RESULTS"' \
        "${prefixes%,}"
}

# Slot index on 1.42.0.1 (the committed Gamedata) of every ISource2Server Slot Binding.
slots=(PreWorldUpdate:16 GameFrame:20 ServerHibernationUpdate:23 IsPaused:32 GameServerSteamAPIActivated:42
    GameServerSteamAPIDeactivated:43 OnHostNameChanged:44 PreFatalShutdown:45 UpdateWhenNotInGame:46 GetEconItemSystem:47)

frames="GameFrameTests.GameFrame_Ticks TimerTests.CreateTimer_ExecutesCallback
    FrameSchedulingTests.QueueTaskForNextFrame_ExecutesCallback FrameSchedulingTests.NextWorldUpdate_HighLevelApi_ExecutesCallback"

status=0

echo "== isource2server: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-no-log '[Hh]ealed|Vtable of CSource2Server' \
    --check "$(only_failed)" \
    --check "$(outcome passed $frames)" ||
    status=1

echo "== isource2server: every ISource2Server Slot Binding one slot too low (1.41.9.7 Gamedata on 1.42.0.1)"
shift_all='reduce ('
heals=()
for slot in "${slots[@]}"; do
    method=${slot%%:*} index=${slot##*:}
    shift_all+="\"ISource2Server::$method\","
    heals+=(--expect-log "^ +Healed ISource2Server::$method: CSource2Server \\(server\\) slot $((index - 1)) -> $index, window ")
done
shift_all="${shift_all%,}) as \$k (.; .[\$k].offsets.linux -= 1 | .[\$k].vtable.linux.size = 104)"
"$run" "$@" --no-build \
    --gamedata-jq "$shift_all" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 10 healed$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    "${heals[@]}" \
    --check "$(only_failed)" \
    --check "$(outcome passed $frames)" \
    --check 'cmp gamedata.installed.json gamedata.after-run.json && jq -e ".\"ISource2Server::IsPaused\".offsets.linux == 31" gamedata.after-run.json' ||
    status=1

echo "== isource2server: OnHostNameChanged's and IsPaused's fingerprints corrupted"
"$run" "$@" --no-build \
    --gamedata-jq '."ISource2Server::OnHostNameChanged".vtable.linux.window[1] = "0000000000000000" | ."ISource2Server::IsPaused".vtable.linux.window[1] = "0000000000000000"' \
    --expect-log 'Bindings for build [^:]+: 2 of [0-9]+ failed: .*ISource2Server::IsPaused \(fingerprint mismatch\)' \
    --expect-log 'Bindings for build [^:]+: 2 of [0-9]+ failed: .*ISource2Server::OnHostNameChanged \(fingerprint mismatch\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 2 disabled:' \
    --expect-log '^ +cssharp\.server/hostname-changed disabled \(ISource2Server::OnHostNameChanged: fingerprint mismatch\)' \
    --expect-log '^ +cssharp\.server/is-paused disabled \(ISource2Server::IsPaused: fingerprint mismatch\)' \
    --check "$(only_failed 'EngineFeature_IsAvailable(feature: ServerHostnameChanged)' \
        'EngineFeature_IsAvailable(feature: ServerIsPaused)' HostNameChanged_ListenerFiresWithTheNewName \
        IsServerPaused_IsFalseOnARunningServer)" \
    --check 'jq -e "[.tests[] | select(.name | endswith(\"IsServerPaused_IsFalseOnARunningServer\")) | .messages[0] | contains(\"Engine Feature cssharp.server/is-paused is disabled (ISource2Server::IsPaused: fingerprint mismatch)\")] == [true]" "$E2E_RESULTS"' \
    --check "$(outcome passed $frames)" ||
    status=1

exit $status
