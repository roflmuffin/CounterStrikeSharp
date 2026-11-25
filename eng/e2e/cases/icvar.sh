#!/usr/bin/env bash
# End-to-end cases for the ICvar Slot Bindings (CCvar in tier0): the DispatchConCommand hook and every ICvar method
# CounterStrikeSharp calls, directly or through the SDK's convar.cpp (via the verified g_pCVar), at the slot index the
# boot Vtable Check verified, backing the console-command and ConVar Engine Features.
#
#   eng/e2e/cases/icvar.sh [run.sh options...]     e.g. --no-build
#
# valid:     every ICvar Slot Binding resolves by its window; no Heal; their Engine Features are available; commands
#            dispatch (a server console command reaches its callback and listeners) and ConVars are found and read.
# healed:    Gamedata states DispatchConCommand one slot too low (as if Valve had inserted a virtual before it); it heals
#            one slot up, the startup report lists the Heal, commands still dispatch through the healed hook, and the
#            installed Gamedata is unchanged.
# corrupted: InstallGlobalChangeCallback's own fingerprint is wrong; exactly cssharp.convars/change-events is disabled
#            (OnConVarChanged never fires), every other command and ConVar feature still works.
# Exits non-zero if any case fails; each case's output dir is printed by run.sh.
set -uo pipefail

run=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/run.sh

# Bash for --check: the ICvarTests that did not pass are exactly the given ones (name prefixes), all 10 ran.
only_failed() {
    local prefixes=
    [[ $# -gt 0 ]] && prefixes=$(printf '"NativeTestsPlugin.ICvarTests.%s",' "$@")
    printf 'jq -e '\''[.tests[] | select(.name | startswith("NativeTestsPlugin.ICvarTests."))] as $t | ($t | length) == 10 and ([$t[] | select(.outcome != "passed") | .name] | sort) == ([$t[] | .name | select(. as $n | [%s] | any(. as $p | $n | startswith($p)))] | sort)'\'' "$E2E_RESULTS"' \
        "${prefixes%,}"
}

status=0

echo "== icvar: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-no-log 'Healed ICvar|Vtable of CCvar|not registering core console commands' \
    --check "$(only_failed)" ||
    status=1

echo "== icvar: DispatchConCommand one slot too low"
"$run" "$@" --no-build \
    --gamedata-jq '."ICvar::DispatchConCommand".offsets.linux = 19' \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 1 healed$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-log '^ +Healed ICvar::DispatchConCommand: CCvar \(tier0\) slot 19 -> 20, window ' \
    --check "$(only_failed)" \
    --check 'cmp gamedata.installed.json gamedata.after-run.json && jq -e ".\"ICvar::DispatchConCommand\".offsets.linux == 19" gamedata.after-run.json' ||
    status=1

echo "== icvar: InstallGlobalChangeCallback's fingerprint corrupted"
"$run" "$@" --no-build \
    --gamedata-jq '."ICvar::InstallGlobalChangeCallback".vtable.linux.window[1] = "0000000000000000"' \
    --expect-log 'Bindings for build [^:]+: 1 of [0-9]+ failed: ICvar::InstallGlobalChangeCallback \(fingerprint mismatch\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log '^ +cssharp\.convars/change-events disabled \(ICvar::InstallGlobalChangeCallback: fingerprint mismatch\)' \
    --expect-no-log 'not registering core console commands' \
    --check "$(only_failed 'EngineFeature_IsAvailable(feature: ConvarsChangeEvents)')" ||
    status=1

exit $status
