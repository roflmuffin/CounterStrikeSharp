#!/usr/bin/env bash
# End-to-end cases for the operator settings in core.json (FailMode, HealEnabled, SchemaMissMode) and the plugin
# signatures report.
#
#   eng/e2e/cases/fail-mode.sh [run.sh options...]     e.g. --no-build
#
# crash, broken:   FailMode crash and Host_Say ambiguous: the startup report is logged, then a fatal error naming
#                  cssharp.chat/say-hooks and Host_Say, and the server exits with code 1 before any plugin loads.
# crash, healed:   FailMode crash and every ISource2Server Slot Binding one slot too low: all heal, so the server runs.
# heal off:        HealEnabled false and the same Gamedata: the moved Slot Bindings fail as fingerprint mismatch, both
#                  logs say healing is off, and (FailMode disable) the server stays up. No smoke test: frames stop.
# schema warn:     the default SchemaMissMode: the smoke suite's schema tests pass in warn mode and the missing field
#                  is warned about once.
# schema throw:    SchemaMissMode throw: the same schema tests pass in throw mode: every access to a missing field throws.
# plugin sigs:     a fixture plugin resolves a unique, an ambiguous and a not-found signature: the plugin signatures
#                  block lists the last two with plugin and match count, the query API returns all three, and the block
#                  is logged again after the plugin is reloaded.
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

# Bash for --check: every SchemaTests test passed.
schema_passed='jq -e "[.tests[] | select(.name | startswith(\"NativeTestsPlugin.SchemaTests.\"))] | length > 10 and all(.outcome == \"passed\")" "$E2E_RESULTS"'

# Every ISource2Server Slot Binding one slot too low (1.41.9.7 Gamedata on 1.42.0.1), as in isource2server.sh.
slots=(PreWorldUpdate GameFrame ServerHibernationUpdate IsPaused GameServerSteamAPIActivated GameServerSteamAPIDeactivated
    OnHostNameChanged PreFatalShutdown UpdateWhenNotInGame GetEconItemSystem)
shift_all='reduce ('
for method in "${slots[@]}"; do shift_all+="\"ISource2Server::$method\","; done
shift_all="${shift_all%,}) as \$k (.; .[\$k].offsets.linux -= 1 | .[\$k].vtable.linux.size = 104)"

status=0

echo "== fail-mode: FailMode crash, Host_Say ambiguous"
out=$(mktemp -d)
"$run" "$@" --out "$out" --no-css-ready \
    --core-jq '.FailMode = "crash"' \
    --gamedata-jq '.Host_Say.signatures.linux = "55 48 89 E5"' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log 'FailMode is crash and 1 of [0-9]+ core Engine Features is disabled on build [^,]+, stopping the server:' \
    --expect-log '^ +cssharp\.chat/say-hooks disabled \(Host_Say: ambiguous \([0-9]+ matches\)\)' \
    --expect-log 'FailMode is crash and a core Engine Feature is disabled: stopping the server' \
    --expect-no-log 'Finished loading plugin|Host activate: Loading' \
    --check 'awk "/Engine Features for build/ && !r { r = NR } /FailMode is crash and 1 of/ && !f { f = NR } END { exit !(r && f && r < f) }" server-console.log'
rc=$?
if [ "$rc" != 2 ] || ! grep -q '^server_exit_code: 1$' "$out/summary.txt"; then
    echo "fail-mode: expected a boot failure with server exit code 1 (run.sh exit $rc)"; status=1
fi

echo "== fail-mode: FailMode crash, every ISource2Server Slot Binding healed"
"$run" "$@" --no-build --no-smoke \
    --core-jq '.FailMode = "crash"' \
    --gamedata-jq "$shift_all" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 10 healed(, [0-9]+ unverified)?$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-no-log 'FailMode is crash' ||
    status=1

echo "== fail-mode: HealEnabled false, every ISource2Server Slot Binding one slot too low"
"$run" "$@" --no-build --no-smoke \
    --core-jq '.HealEnabled = false' \
    --gamedata-jq "$shift_all" \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*ISource2Server::GameFrame \(fingerprint mismatch\).*; healing off' \
    --expect-log '^ +cssharp\.server/game-frame disabled \(ISource2Server::GameFrame: fingerprint mismatch\)' \
    --expect-log '^ +Healing is off \(HealEnabled: false\): a Slot Binding that moved fails as fingerprint mismatch' \
    --expect-no-log '[Hh]ealed ISource2Server' ||
    status=1

echo "== fail-mode: SchemaMissMode warn (default)"
"$run" "$@" --no-build \
    --expect-log 'Schema field CBaseEntity::m_cssharpDoesNotExist not found \(field missing\); reads return the default value and writes are dropped' \
    --check 'test "$(grep -hc "Schema field CBaseEntity::m_cssharpDoesNotExist not found" server-console.log)" = 1' \
    --check "$schema_passed" \
    --check "$(outcome passed SchemaTests.GetSchemaValue_MissingField_InWarnMode_ReadsDefaultAndDropsWritesWithoutTouchingOffsetZero \
        SchemaTests.SetStateChanged_ClassWithoutChainEntity_DoesNotThrow)" ||
    status=1

echo "== fail-mode: SchemaMissMode throw"
"$run" "$@" --no-build \
    --core-jq '.SchemaMissMode = "throw"' \
    --expect-log 'Schema field CBaseEntity::m_cssharpDoesNotExist not found \(field missing\); accessing it throws' \
    --check "$schema_passed" ||
    status=1

echo "== fail-mode: plugin signatures in the report, again after a reload"
sigs='Plugin signatures: 2 of [0-9]+ ambiguous or not found:'
"$run" "$@" --no-build --no-smoke \
    --plugin-project managed/CounterStrikeSharp.Tests.PluginSignatures/PluginSignaturesTestPlugin.csproj \
    --exec 'css_plugins reload PluginSignaturesTest' \
    --expect-log "$sigs" \
    --expect-log '^ +PluginSignaturesTestPlugin: "55 48 89 E5" in libserver\.so: ambiguous \([0-9]+ matches\), first match used' \
    --expect-log '^ +PluginSignaturesTestPlugin: "DE AD BE EF 13 37 C0 DE 42 42 99 88" in libserver\.so: not found' \
    --expect-no-log '^ +PluginSignaturesTestPlugin: Host_Say ' \
    --expect-log 'Plugin signature entry: PluginSignaturesTestPlugin: Host_Say "[0-9A-F ?]+" in libserver\.so: resolved \(match count 1\)' \
    --expect-log 'Plugin signature entry: PluginSignaturesTestPlugin: "55 48 89 E5" in libserver\.so: ambiguous' \
    --check "test \"\$(grep -cE '$sigs' server-console.log)\" -ge 2" \
    --expect-log 'Unloading plugin PluginSignaturesTest' ||
    status=1

exit $status
