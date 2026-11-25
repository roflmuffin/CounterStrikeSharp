#!/usr/bin/env bash
# End-to-end cases for the interfaces that never shifted a slot we use (game event manager, game event system, network
# server service, network game server, engine service manager): their slots are Slot Bindings verified at boot; hooks
# go to the verified index, direct calls stay compiled. Network messages and the schema system are reported unverified.
#
#   eng/e2e/cases/never-moved.sh [run.sh options...]     e.g. --no-build
#
# valid:     every never-moved Slot Binding verifies at its Gamedata index; startup, loop-mode, global-vars,
#            user-messages and game-events are available; network messages and schema system are listed unverified.
# healed:    StartupServer (hook only) and FireEvent (hook and compiled call) one slot too low in Gamedata: both heal to
#            the compiled index, so both stay available; the map starts and game events work.
# corrupted: FireEvent's own fingerprint in Gamedata is wrong: only game-events is disabled, the server stays up.
# moved:     Gamedata states FireEvent at slot 7 with slot 7's window (as if Valve had moved it and Gamedata followed):
#            it verifies there, but the compiled call still uses slot 8, so game-events is disabled instead of calling
#            the wrong function.
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
fire_event='IGameEventManager2::FireEvent'
unverified='4 Slot Binding\(s\) unverified \(no Vtable Check on this platform or no fingerprint window in Gamedata\): '
unverified+='CSchemaSystem::FindTypeScopeForModule, CSchemaSystem::GlobalTypeScope, '
unverified+='INetworkMessages::FindNetworkMessageById, INetworkMessages::FindNetworkMessagePartial$'

echo "== never-moved: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 4 unverified$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-log "^ +$unverified" \
    --expect-no-log '[Hh]ealed|compiled call|Vtable of [A-Za-z0-9_]+ \([a-z0-9]+\) has [0-9]+ slots' \
    --check "$(outcome passed NeverMovedTests.NeverMovedEngineFeatures_AreVerifiedAndAvailable NeverMovedTests.GlobalVars_AreReadThroughTheVerifiedSlots \
        GameEventTests.CreateEvent_ReturnsValidPointer GameEventTests.GameEvent_HighLevelApi_Works \
        UserMessageTests.CreateUserMessage_ReturnsValidPointer GameFrameTests.GameFrame_Ticks TimerTests.CreateTimer_ExecutesCallback)" ||
    status=1

echo "== never-moved: StartupServer and FireEvent one slot too low"
"$run" "$@" --no-build \
    --gamedata-jq '."INetworkServerService::StartupServer".offsets.linux = 26 | ."'"$fire_event"'".offsets.linux = 7' \
    --exec css_engine_features \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 2 healed, 4 unverified$' \
    --expect-log 'Healed INetworkServerService::StartupServer: CNetworkServerService \(engine\) slot 26 -> 27, window ' \
    --expect-log 'Healed IGameEventManager2::FireEvent: CGameEventManager \(server\) slot 7 -> 8, window ' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-log '\[EngineFeatures\] cssharp\.server/startup available, healed \(INetworkServerService::StartupServer: slot 26 -> 27\)' \
    --expect-log '\[EngineFeatures\] cssharp\.events/game-events available, healed \(IGameEventManager2::FireEvent: slot 7 -> 8\)' \
    --check "$(outcome passed NeverMovedTests.GlobalVars_AreReadThroughTheVerifiedSlots GameEventTests.CreateEvent_ReturnsValidPointer \
        GameEventTests.GameEvent_HighLevelApi_Works GameFrameTests.GameFrame_Ticks)" \
    --check 'cmp gamedata.installed.json gamedata.after-run.json' ||
    status=1

echo "== never-moved: FireEvent's fingerprint corrupted"
"$run" "$@" --no-build \
    --gamedata-jq '."'"$fire_event"'".vtable.linux.window[1] = "0000000000000000"' \
    --expect-log 'Bindings for build [^:]+: 1 of [0-9]+ failed: IGameEventManager2::FireEvent \(fingerprint mismatch\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log '^ +cssharp\.events/game-events disabled \(IGameEventManager2::FireEvent: fingerprint mismatch\)' \
    --check "$(outcome passed NeverMovedTests.GlobalVars_AreReadThroughTheVerifiedSlots GameFrameTests.GameFrame_Ticks \
        UserMessageTests.CreateUserMessage_ReturnsValidPointer)" \
    --check "$(outcome failed GameEventTests.GameEvent_HighLevelApi_Works)" ||
    status=1

echo "== never-moved: FireEvent verified at another slot than its compiled call"
"$run" "$@" --no-build \
    --gamedata-jq '."'"$fire_event"'" = ."IGameEventManager2::CreateEvent"' \
    --expect-log 'Bindings for build [^:]+: 1 of [0-9]+ failed: IGameEventManager2::FireEvent \(compiled call.s slot moved\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log "^ +cssharp\\.events/game-events disabled \\(IGameEventManager2::FireEvent: compiled call's slot moved\\)" \
    --check "$(outcome passed NeverMovedTests.GlobalVars_AreReadThroughTheVerifiedSlots GameFrameTests.GameFrame_Ticks)" \
    --check "$(outcome failed GameEventTests.GameEvent_HighLevelApi_Works)" ||
    status=1

exit $status
