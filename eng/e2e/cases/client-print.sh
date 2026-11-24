#!/usr/bin/env bash
# End-to-end cases for the cssharp.chat/client-print Engine Feature (Bindings: ClientPrint, UTIL_ClientPrintAll), the
# first managed-consumed Engine Feature (VirtualFunctions.ClientPrint*, Server.PrintToChatAll).
#
#   eng/e2e/cases/client-print.sh [run.sh options...]     e.g. --no-build
#
# valid:  PrintToChatAll prints, a UTIL_ClientPrintAll hook fires, entity creation works.
# broken: ClientPrint is made ambiguous; the server stays up, the startup report names it, PrintToChatAll throws
#         EngineFeatureUnavailableException, the hook registers (one warning) but never fires, entity creation still works.
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

echo "== client-print: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved' \
    --expect-log 'Engine Features for build [^:]+: all 36 available' \
    --expect-no-log 'cssharp\.chat/client-print disabled' \
    --expect-no-log 'do not match the loaded image|Cannot get original bytes' \
    --check "$(outcome passed ClientPrintTests.PrintToChatAll_Prints ClientPrintTests.PrintToChatAll_ThrowsTypedExceptionWhenDisabled \
        ClientPrintTests.ClientPrintAllHook_FiresOnlyWhenAvailable ClientPrintTests.UnrelatedCoreFunction_StillWorks)" ||
    status=1

echo "== client-print: ClientPrint ambiguous"
typed_exception='\[ClientPrintTests\] PrintToChatAll threw EngineFeatureUnavailableException: '
typed_exception+='Engine Feature cssharp\.chat/client-print is disabled \(ClientPrint: ambiguous'
"$run" "$@" --no-build \
    --gamedata-jq '.ClientPrint.signatures.linux = "55 48 89 E5"' \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*ClientPrint \(ambiguous \([0-9]+ matches\)\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-no-log 'not registering|Failed to initialize GameSystem' \
    --expect-log '^ +cssharp\.chat/client-print disabled \(ClientPrint: ambiguous \([0-9]+ matches\)\)' \
    --expect-log "$typed_exception" \
    --expect-log 'Memory function hook on UTIL_ClientPrintAll registered on cssharp\.chat/client-print disabled \(ClientPrint: ambiguous' \
    --check "$(outcome failed ClientPrintTests.PrintToChatAll_Prints)" \
    --check "$(outcome passed ClientPrintTests.PrintToChatAll_ThrowsTypedExceptionWhenDisabled \
        ClientPrintTests.ClientPrintAllHook_FiresOnlyWhenAvailable ClientPrintTests.UnrelatedCoreFunction_StillWorks)" ||
    status=1

exit $status
