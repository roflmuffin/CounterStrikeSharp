#!/usr/bin/env bash
# End-to-end cases for the cssharp.chat/say-hooks Engine Feature (Binding: Host_Say).
#
#   eng/e2e/cases/say-hooks.sh [run.sh options...]     e.g. --no-build
#
# valid:  Host_Say resolves, chat hooks are registered and chat triggers work (commands reached via "say !x").
# broken: Host_Say is made ambiguous; the server stays up, the early native log names Host_Say and its reason,
#         and the chat hook is not registered.
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

echo "== say-hooks: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved' \
    --expect-log 'Engine Features for build [^:]+: all 36 available' \
    --expect-no-log 'cssharp\.chat/say-hooks disabled' \
    --expect-no-log 'do not match the loaded image|Cannot get original bytes' \
    --check "$(outcome passed CommandTests.CanTriggerCommandsWithSilentChatTrigger \
        CommandTests.CanTriggerCommandsWithPublicChatTrigger CommandTests.AddCommandHandler)" ||
    status=1

echo "== say-hooks: Host_Say ambiguous"
"$run" "$@" --no-build \
    --gamedata-jq '.Host_Say.signatures.linux = "55 48 89 E5"' \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*Host_Say \(ambiguous \([0-9]+ matches\)\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log '^ +cssharp\.chat/say-hooks disabled \(Host_Say: ambiguous \([0-9]+ matches\)\)' \
    --expect-no-log 'not registering|Failed to initialize GameSystem' \
    --check "$(outcome failed CommandTests.CanTriggerCommandsWithSilentChatTrigger)" ||
    status=1

exit $status
