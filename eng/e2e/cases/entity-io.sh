#!/usr/bin/env bash
# End-to-end cases for the entity IO Engine Features: cssharp.entity/output-hooks
# (Binding: CEntityIOOutput_FireOutputInternal) next to accept-input, add-io-event, emit-sound and dispatch-spawn.
#
#   eng/e2e/cases/entity-io.sh [run.sh options...]     e.g. --no-build
#
# valid:  every entity IO Engine Feature is available; output hooks fire and accept-input works.
# broken: the output-hook Binding is not found; the server stays up, output hooks are disabled and reported (native
#         log, startup report, one warning per plugin hook registration), and accept-input still works.
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

echo "== entity-io: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved' \
    --expect-no-log 'cssharp\.entity/(output-hooks|accept-input|add-io-event|emit-sound|dispatch-spawn) disabled' \
    --expect-no-log 'do not match the loaded image|Cannot get original bytes' \
    --check "$(outcome passed EntityIOTests.AcceptInput_DoesNotThrow EntityIOTests.AddEntityIOEvent_WithLegacyOutputId_FiresOutput \
        EntityIOTests.AcceptInput_Kill_RemovesEntity EntityIOTests.HookEntityOutput_FiresOnlyWhenOutputHooksAreAvailable)" ||
    status=1

echo "== entity-io: CEntityIOOutput_FireOutputInternal not found"
"$run" "$@" --no-build \
    --gamedata-jq '.CEntityIOOutput_FireOutputInternal.signatures.linux = "DE AD BE EF 13 37 C0 DE 42 42"' \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*CEntityIOOutput_FireOutputInternal \(not found\)' \
    --expect-log 'Engine Feature cssharp\.entity/output-hooks disabled \(CEntityIOOutput_FireOutputInternal: not found\), not registering entity output hooks' \
    --expect-log '^ +cssharp\.entity/output-hooks disabled \(CEntityIOOutput_FireOutputInternal: not found\)' \
    --expect-log 'Entity output hook registered on disabled cssharp\.entity/output-hooks disabled \(CEntityIOOutput_FireOutputInternal: not found\); it will never fire' \
    --expect-no-log 'cssharp\.entity/(accept-input|add-io-event|emit-sound|dispatch-spawn) disabled' \
    --check "$(outcome passed EntityIOTests.AcceptInput_Kill_RemovesEntity EntityIOTests.HookEntityOutput_FiresOnlyWhenOutputHooksAreAvailable)" \
    --check "$(outcome failed EntityIOTests.AcceptInput_DoesNotThrow)" ||
    status=1

exit $status
