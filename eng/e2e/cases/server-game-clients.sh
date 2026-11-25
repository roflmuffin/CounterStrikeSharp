#!/usr/bin/env bash
# End-to-end cases for the IServerGameClients hooks (Slot Bindings on CSource2GameClients), hooked at the slot index the
# boot Vtable Check verified: cssharp.player/connection-events, client-commands, voice-events and
# cssharp.ui/custom-hud-layout.
#
#   eng/e2e/cases/server-game-clients.sh [run.sh options...]     e.g. --no-build
#
# valid:            every Binding resolves (none unverified or healed); the four Engine Features are available; bots
#                   fire the connection listeners and a client command reaches its command listener.
# connections-off-by-one:
#                   Gamedata states ClientConnect, ClientPutInServer and ClientDisconnect one slot too low; each window
#                   matches once, one slot later, so all three heal: the native log and the startup report list the
#                   Heals, plugins see them in the Engine Feature status, bots still fire the connection listeners and
#                   the installed Gamedata is unchanged.
# client-command-corrupted:
#                   ClientCommand's own fingerprint in Gamedata is wrong: only client-commands is disabled, the startup
#                   report names it; connections still work and a client command never reaches its listener.
# connect-corrupted:
#                   ClientConnect's own fingerprint is wrong: only connection-events is disabled; its listeners still
#                   register and never fire, client commands still work.
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

t=ServerGameClientsTests
listeners="$t.ConnectionListeners_FireForABot_OnlyWhileConnectionEventsIsAvailable $t.ClientCommand_ReachesCommandListeners_OnlyWhileClientCommandsIsAvailable"
available="$t.ConnectionEvents_IsAvailable $t.ClientCommands_IsAvailable $t.VoiceEvents_IsAvailable $t.CustomHudLayout_IsAvailable"
window='window \[[0-9a-f]{16}, [0-9a-f]{16}, [0-9a-f]{16}\] matched once within 16 slots'
status=0

echo "== server-game-clients: valid gamedata"
# shellcheck disable=SC2086
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-no-log 'unverified|[Hh]ealed|Vtable of CSource2GameClients' \
    --check "$(outcome passed $available $listeners)" \
    --expect-log '\[ServerGameClientsTests\] connection-events available: True; connects [1-9]' \
    --expect-log '\[ServerGameClientsTests\] client-commands available: True; sent True, listener calls 1' ||
    status=1

echo "== server-game-clients: connection hooks one slot too low"
"$run" "$@" --no-build \
    --gamedata-jq '."IServerGameClients::ClientConnect".offsets.linux = 11' \
    --gamedata-jq '."IServerGameClients::ClientPutInServer".offsets.linux = 12' \
    --gamedata-jq '."IServerGameClients::ClientDisconnect".offsets.linux = 15' \
    --exec css_engine_features \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 3 healed$' \
    --expect-log "Healed IServerGameClients::ClientConnect: CSource2GameClients \\(server\\) slot 11 -> 12, $window" \
    --expect-log "Healed IServerGameClients::ClientPutInServer: CSource2GameClients \\(server\\) slot 12 -> 13, $window" \
    --expect-log "Healed IServerGameClients::ClientDisconnect: CSource2GameClients \\(server\\) slot 15 -> 16, $window" \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-log '^ +Healed IServerGameClients::ClientConnect: CSource2GameClients \(server\) slot 11 -> 12, window ' \
    --expect-log '\[EngineFeatures\] cssharp\.player/connection-events available, healed \(IServerGameClients::ClientConnect: slot 11 -> 12, IServerGameClients::ClientPutInServer: slot 12 -> 13, IServerGameClients::ClientDisconnect: slot 15 -> 16\)' \
    --check "$(outcome passed $available $listeners)" \
    --expect-log '\[ServerGameClientsTests\] connection-events available: True; connects [1-9]' \
    --check 'cmp gamedata.installed.json gamedata.after-run.json && jq -e ".\"IServerGameClients::ClientConnect\".offsets.linux == 11" gamedata.after-run.json' ||
    status=1

echo "== server-game-clients: ClientCommand's fingerprint corrupted"
"$run" "$@" --no-build \
    --gamedata-jq '."IServerGameClients::ClientCommand".vtable.linux.window[1] = "0000000000000000"' \
    --expect-log 'Bindings for build [^:]+: 1 of [0-9]+ failed: IServerGameClients::ClientCommand \(fingerprint mismatch\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log '^ +cssharp\.player/client-commands disabled \(IServerGameClients::ClientCommand: fingerprint mismatch\)' \
    --check "$(outcome passed $t.ConnectionEvents_IsAvailable $t.VoiceEvents_IsAvailable $t.CustomHudLayout_IsAvailable $listeners)" \
    --check "$(outcome failed $t.ClientCommands_IsAvailable)" \
    --expect-log '\[ServerGameClientsTests\] connection-events available: True; connects [1-9]' \
    --expect-log '\[ServerGameClientsTests\] client-commands available: False; sent True, listener calls 0' ||
    status=1

echo "== server-game-clients: ClientConnect's fingerprint corrupted"
"$run" "$@" --no-build \
    --gamedata-jq '."IServerGameClients::ClientConnect".vtable.linux.window[1] = "0000000000000000"' \
    --expect-log 'Bindings for build [^:]+: 1 of [0-9]+ failed: IServerGameClients::ClientConnect \(fingerprint mismatch\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log '^ +cssharp\.player/connection-events disabled \(IServerGameClients::ClientConnect: fingerprint mismatch\)' \
    --check "$(outcome passed $t.ClientCommands_IsAvailable $t.VoiceEvents_IsAvailable $t.CustomHudLayout_IsAvailable $listeners)" \
    --check "$(outcome failed $t.ConnectionEvents_IsAvailable)" \
    --expect-log '\[ServerGameClientsTests\] connection-events available: False; connects 0, put in server 0, disconnects 0' \
    --expect-log '\[ServerGameClientsTests\] client-commands available: True; sent True, listener calls 1' ||
    status=1

exit $status
