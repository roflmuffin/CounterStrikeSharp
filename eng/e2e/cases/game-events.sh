#!/usr/bin/env bash
# End-to-end cases for cssharp.events/game-events (Bindings: CGameEventManager_Init signature, CGameEventManager RTTI
# vtable in libserver). The valid case also covers the other event and network Engine Features of ticket 09.
#
#   eng/e2e/cases/game-events.sh [run.sh options...]     e.g. --no-build
#
# valid:       every Binding resolves, no event/network Engine Feature is disabled, game events work.
# init:        CGameEventManager_Init not found: CounterStrikeSharp loads, game-events is reported disabled (not silently
#              dead), event handlers register with one warning (the test plugin registers one at load) and never fire,
#              creating an event throws.
# rtti-vtable: the CGameEventManager typeinfo name is patched in the server copy's libserver.so (harness-only,
#              --server-patch; Gamedata cannot break an RTTI lookup). Used to abort the whole load; now only
#              game-events is disabled.
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

echo "== game-events: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved' \
    --expect-no-log 'Engine Feature cssharp\.(events|network)/[a-z-]+ disabled' \
    --expect-no-log 'do not match the loaded image|Cannot get original bytes' \
    --check "$(outcome passed GameEventTests.CreateEvent_ReturnsValidPointer GameEventTests.GameEvent_HighLevelApi_Works)" ||
    status=1

echo "== game-events: CGameEventManager_Init not found"
"$run" "$@" --no-build \
    --gamedata-jq '.CGameEventManager_Init.signatures.linux = "DE AD BE EF 13 37 C0 DE 42 42"' \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*CGameEventManager_Init \(not found\)' \
    --expect-log 'Engine Feature cssharp\.events/game-events disabled \(CGameEventManager_Init: not found\), not registering game event hooks' \
    --expect-log '^ +cssharp\.events/game-events disabled \(CGameEventManager_Init: not found\)' \
    --expect-log 'Game event handler registered on disabled cssharp\.events/game-events disabled \(CGameEventManager_Init: not found\); it will never fire' \
    --check "$(outcome failed GameEventTests.CanRegisterAndDeregisterEventHandlers GameEventTests.GameEvent_HighLevelApi_Works)" ||
    status=1

echo "== game-events: CGameEventManager RTTI vtable not found"
"$run" "$@" --no-build \
    --server-patch 'game/csgo/bin/linuxsteamrt64/libserver.so:17CGameEventManager:17CGameEventManageX' \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*CGameEventManager \(RTTI vtable not found\)' \
    --expect-log 'Engine Feature cssharp\.events/game-events disabled \(CGameEventManager: RTTI vtable not found\), not registering game event hooks' \
    --expect-log '^ +cssharp\.events/game-events disabled \(CGameEventManager: RTTI vtable not found\)' \
    --expect-log 'Game event handler registered on disabled cssharp\.events/game-events disabled \(CGameEventManager: RTTI vtable not found\); it will never fire' \
    --check "$(outcome failed GameEventTests.CanRegisterAndDeregisterEventHandlers GameEventTests.GameEvent_HighLevelApi_Works)" ||
    status=1

exit $status
