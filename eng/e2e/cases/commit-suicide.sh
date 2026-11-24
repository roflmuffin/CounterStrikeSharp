#!/usr/bin/env bash
# End-to-end cases for the managed player/entity helpers (ticket 12), using cssharp.player/commit-suicide
# (Binding: the CBasePlayerPawn_CommitSuicide offset) as the broken one.
#
#   eng/e2e/cases/commit-suicide.sh [run.sh options...]     e.g. --no-build
#
# valid:  every Engine Feature is available, CommitSuicide, Respawn and Teleport work.
# broken: the CommitSuicide offset is missing for this OS; the server stays up, the startup report names it,
#         CommitSuicide throws EngineFeatureUnavailableException (instead of calling vtable slot 0), respawn and
#         teleport still work.
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

echo "== commit-suicide: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved' \
    --expect-log 'Engine Features for build [^:]+: all 36 available' \
    --expect-no-log 'do not match the loaded image|Cannot get original bytes' \
    --check "$(outcome passed GameTests.Offset_CBasePlayerPawn_CommitSuicide GameTests.Offset_CCSPlayerController_Respawn \
        GameTests.Offset_CBaseEntity_Teleport PlayerHelperFailSoftTests.CommitSuicide_ThrowsTypedExceptionWhenDisabled \
        PlayerHelperFailSoftTests.RespawnAndTeleport_WorkWithoutCommitSuicide)" ||
    status=1

echo "== commit-suicide: CBasePlayerPawn_CommitSuicide offset missing for this OS"
"$run" "$@" --no-build \
    --gamedata-jq 'del(.CBasePlayerPawn_CommitSuicide.offsets.linux)' \
    --expect-log '^ +cssharp\.player/commit-suicide disabled \(CBasePlayerPawn_CommitSuicide: missing for this OS\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-no-log 'not registering|Failed to initialize GameSystem' \
    --expect-log '\[PlayerHelperFailSoftTests\] CommitSuicide threw EngineFeatureUnavailableException: Engine Feature cssharp\.player/commit-suicide is disabled \(CBasePlayerPawn_CommitSuicide: missing for this OS\)' \
    --check "$(outcome failed GameTests.Offset_CBasePlayerPawn_CommitSuicide)" \
    --check "$(outcome passed GameTests.Offset_CBaseEntity_Teleport PlayerHelperFailSoftTests.CommitSuicide_ThrowsTypedExceptionWhenDisabled \
        PlayerHelperFailSoftTests.RespawnAndTeleport_WorkWithoutCommitSuicide)" ||
    status=1

exit $status
