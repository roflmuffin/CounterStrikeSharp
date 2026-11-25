#!/usr/bin/env bash
# End-to-end cases for the Slot Binding contract: every hook and call on a migrated interface (ISource2Server,
# IServerGameClients, IVEngineServer2, ICvar) and every game-class virtual goes through a Slot Binding verified at
# boot, never through a compiled header slot position.
#
#   eng/e2e/cases/contract.sh [run.sh options...]     e.g. --no-build
#
# valid:      the committed Gamedata verifies every Slot Binding at its index: no Heal, no failure, no size warning,
#             only network messages and the schema system unverified; every Engine Feature available; no ICvar
#             method without a Slot Binding is reached; the whole smoke suite passes except known flaky tests.
# windowless: Gamedata without any fingerprint window (e.g. auto-updated from before Slot Bindings): every Slot
#             Binding is unverified at its Gamedata index, none fails, frames, timers and game-class virtuals work.
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

# Smoke tests known to fail intermittently on every build (timing and test-order dependent), not because of a Binding.
flaky='^(NativeTestsPlugin\.)?(CommandTests\.|GameEventTests\.CanRegisterAndDeregisterEventHandlers$|ConVarTests\.BoolConVar$|'
flaky+='RealConVarTests\.|SchemaPropertyTests\.|ListenerTests\.|ServerGameClientsTests\.ClientCommand_ReachesCommandListeners|'
flaky+='NativeObjectsTests\.EnsureNativeHandle_IsFreed)'
# Bash for --check: the smoke suite ran, and every failed test is a known flaky one (each is printed).
only_flaky_fail="failed=\$(jq -r '.tests[] | select(.outcome == \"failed\") | .name' \"\$E2E_RESULTS\"); echo \"failed: \${failed//\$'\\n'/, }\"; "
only_flaky_fail+="! grep -vE '$flaky' <<<\"\$failed\" | grep -q . && jq -e '.tests | length > 200' \"\$E2E_RESULTS\" >/dev/null"

status=0
unverified='4 Slot Binding\(s\) unverified \(no Vtable Check on this platform or no fingerprint window in Gamedata\): '
unverified+='CSchemaSystem::FindTypeScopeForModule, CSchemaSystem::GlobalTypeScope, '
unverified+='INetworkMessages::FindNetworkMessageById, INetworkMessages::FindNetworkMessagePartial$'

echo "== contract: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 4 unverified$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-log "^ +$unverified" \
    --expect-no-log '[Hh]ealed|[0-9]+ of [0-9]+ failed|cssharp\.[a-z-]+/[a-z-]+ (is )?disabled|compiled call|is not a Slot Binding|Vtable of [A-Za-z0-9_]+ \([a-z0-9]+\) has [0-9]+ slots' \
    --check "$(outcome passed GameFrameTests.GameFrame_Ticks TimerTests.CreateTimer_ExecutesCallback ICvarTests.ServerConsoleCommand_ReachesItsCallbackAndListenersThroughTheDispatchHook)" \
    --check "$only_flaky_fail" ||
    status=1

echo "== contract: gamedata without fingerprint windows"
"$run" "$@" --no-build \
    --gamedata-jq 'with_entries(.value |= del(.vtable))' \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, [0-9]+ unverified$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-no-log '[Hh]ealed|[0-9]+ of [0-9]+ failed|cssharp\.[a-z-]+/[a-z-]+ (is )?disabled|is not a Slot Binding' \
    --check "$(outcome passed GameFrameTests.GameFrame_Ticks TimerTests.CreateTimer_ExecutesCallback GameClassSlotTests.GameClassVirtuals_ReadByKey_FollowTheVtableCheck)" \
    --check 'cmp gamedata.installed.json gamedata.after-run.json' ||
    status=1

exit $status
