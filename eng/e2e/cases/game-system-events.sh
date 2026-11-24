#!/usr/bin/env bash
# End-to-end cases for the cssharp.server/game-system-events Engine Feature (Binding: IGameSystem_InitAllSystems_pFirst,
# plus the schema fields player think reads).
#
#   eng/e2e/cases/game-system-events.sh [run.sh options...]     e.g. --no-build
#
# valid:  every Binding resolves at boot (schema fields included), the game system is registered, precache and
#         pre/post entity think listeners fire.
# broken: the game-system signature is not found; CounterStrikeSharp loads and the server stays up, the Engine Feature
#         is reported disabled, the game system is not registered, and its listeners register, log once, never fire.
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

tests=(GameSystemTests.PrecacheResourcesListenerFiredOnMapLoad GameSystemTests.EntityThinkListenersAreFired)
status=0

echo "== game-system-events: valid gamedata"
"$run" "$@" --exec css_engine_features \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved' \
    --expect-log 'Engine Features for build [^:]+: all 36 available' \
    --expect-log 'Initialized GameSystem' \
    --expect-log '\[EngineFeatures\] cssharp\.server/game-system-events available' \
    --expect-no-log 'cssharp\.server/game-system-events disabled' \
    --expect-no-log 'registered on cssharp\.server/game-system-events' \
    --expect-no-log 'do not match the loaded image|Cannot get original bytes' \
    --check "$(outcome passed "${tests[@]}")" ||
    status=1

echo "== game-system-events: IGameSystem_InitAllSystems_pFirst not found"
precache_never_fires='OnServerPrecacheResources listener registered on cssharp\.server/game-system-events disabled '
precache_never_fires+='(IGameSystem_InitAllSystems_pFirst: not found); it will never fire'
"$run" "$@" --no-build --exec css_engine_features \
    --gamedata-jq '.IGameSystem_InitAllSystems_pFirst.signatures.linux = "DE AD BE EF 13 37 C0 DE 42 42"' \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*IGameSystem_InitAllSystems_pFirst \(not found\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-no-log 'not registering|Failed to initialize GameSystem' \
    --expect-log '^ +cssharp\.server/game-system-events disabled \(IGameSystem_InitAllSystems_pFirst: not found\)' \
    --expect-log '\[EngineFeatures\] cssharp\.server/game-system-events disabled' \
    --expect-no-log 'Initialized GameSystem' \
    --check "[ \"\$(grep -c \"$precache_never_fires\" server-console.log)\" = 1 ]" \
    --check '[ "$(grep -c "OnServerPreEntityThink listener registered on cssharp\.server/game-system-events" server-console.log)" = 1 ]' \
    --check "$(outcome failed "${tests[@]}")" ||
    status=1

exit $status
