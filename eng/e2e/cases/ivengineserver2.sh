#!/usr/bin/env bash
# End-to-end cases for the IVEngineServer2 Slot Bindings (engine2's CEngineServer): the SetClientListening hook and
# every direct call (server command, map validity, precache, user id, console print, client convars, client command,
# disconnect, authorization), each hooked or called at the slot index the boot Vtable Check verified, each backing its
# own Engine Feature.
#
#   eng/e2e/cases/ivengineserver2.sh [run.sh options...]     e.g. --no-build
#
# valid:     every IVEngineServer2 Slot Binding resolves by its window; no Heal; their Engine Features are available
#            and the calls work (EngineServerTests on a test bot, EngineTests' IsMapValid tests).
# healed:    Gamedata states IsMapValid one slot too low; it heals one slot up, the native log, the startup report and
#            the plugin-visible status show the Heal, IsMapValid answers correctly, and the installed Gamedata is
#            unchanged.
# corrupted: IsMapValid's own fingerprint is wrong; only is-map-valid is disabled, Server.IsMapValid throws the typed
#            exception, every other engine call and the server keep working.
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

# Bash for --check: the EngineServerTests that did not pass are exactly the given ones (full names after the class),
# and all 17 ran.
only_failed() {
    local names=
    [[ $# -gt 0 ]] && names=$(printf '"NativeTestsPlugin.EngineServerTests.%s",' "$@")
    printf 'jq -e '\''[.tests[] | select(.name | startswith("NativeTestsPlugin.EngineServerTests."))] as $t | ($t | length) == 17 and ([$t[] | select(.outcome != "passed") | .name] | sort) == ([%s] | sort)'\'' "$E2E_RESULTS"' \
        "${names%,}"
}

is_map_valid="EngineTests.IsMapValid_ReturnsTrueForCurrentMap EngineTests.IsMapValid_ReturnsFalseForInvalidMap"

status=0

echo "== ivengineserver2: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-no-log '[Hh]ealed|Vtable of CEngineServer' \
    --check "$(only_failed)" \
    --check "$(outcome passed $is_map_valid)" ||
    status=1

echo "== ivengineserver2: IsMapValid one slot too low"
"$run" "$@" --no-build \
    --gamedata-jq '."IVEngineServer2::IsMapValid".offsets.linux = 32' \
    --exec css_engine_features \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 1 healed$' \
    --expect-log 'Healed IVEngineServer2::IsMapValid: CEngineServer \(engine\) slot 32 -> 33, window \[[0-9a-f]{16}, [0-9a-f]{16}, [0-9a-f]{16}\] matched once within 16 slots' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-log '^ +Healed IVEngineServer2::IsMapValid: CEngineServer \(engine\) slot 32 -> 33, window ' \
    --expect-log '\[EngineFeatures\] cssharp\.server/is-map-valid available, healed \(IVEngineServer2::IsMapValid: slot 32 -> 33\)' \
    --check "$(only_failed)" \
    --check "$(outcome passed $is_map_valid)" \
    --check 'cmp gamedata.installed.json gamedata.after-run.json && jq -e ".\"IVEngineServer2::IsMapValid\".offsets.linux == 32" gamedata.after-run.json' ||
    status=1

echo "== ivengineserver2: IsMapValid's fingerprint corrupted"
"$run" "$@" --no-build \
    --gamedata-jq '."IVEngineServer2::IsMapValid".vtable.linux.window[1] = "0000000000000000"' \
    --expect-log 'Bindings for build [^:]+: 1 of [0-9]+ failed: IVEngineServer2::IsMapValid \(fingerprint mismatch\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log '^ +cssharp\.server/is-map-valid disabled \(IVEngineServer2::IsMapValid: fingerprint mismatch\)' \
    --expect-log 'IsMapValid threw EngineFeatureUnavailableException: Engine Feature cssharp\.server/is-map-valid is disabled \(IVEngineServer2::IsMapValid: fingerprint mismatch\)' \
    --check "$(only_failed 'EngineFeature_IsAvailable(feature: ServerIsMapValid)')" \
    --check "$(outcome failed $is_map_valid)" \
    --check "$(outcome passed GameFrameTests.GameFrame_Ticks TimerTests.CreateTimer_ExecutesCallback)" ||
    status=1

exit $status
