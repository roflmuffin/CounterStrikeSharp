#!/usr/bin/env bash
# End-to-end cases for the game-class virtuals called through Gamedata offsets (respawn, commit suicide, change team,
# teleport, item services, ...) as Slot Bindings, with CBasePlayerPawn_CommitSuicide as the example.
#
#   eng/e2e/cases/game-class-virtuals.sh [run.sh options...]     e.g. --no-build
#
# valid:     every game-class Slot Binding is verified at its Gamedata index; no Heal, nothing unverified; the helpers
#            work and plugins reading the offsets by key get the verified index.
# one off:   Gamedata states CommitSuicide one slot too low (386); its window matches once, at 387, so it heals there:
#            the native log and the startup report list the Heal, CommitSuicide works, a plugin reading the offset by key
#            gets 387, and the installed Gamedata is unchanged.
# corrupted: CommitSuicide's own fingerprint in Gamedata is wrong; only commit-suicide is disabled (fingerprint
#            mismatch), CommitSuicide throws the typed exception, respawn and teleport still work, reading the offset by
#            key throws instead of returning a wrong index, and the server stays up.
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

key=CBasePlayerPawn_CommitSuicide
read_by_key='\[GameClassSlotTests\] CBasePlayerPawn_CommitSuicide read by key: '
status=0

echo "== game-class-virtuals: valid gamedata"
"$run" "$@" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved(, [0-9]+ unverified)?$' \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-no-log 'unverified.*(CCSPlayer|CBase|CGameSceneNode|CCSGameRules|ISource2GameEntities)|[Hh]ealed|Vtable of [A-Za-z0-9_]+ \([a-z0-9]+\) has [0-9]+ slots' \
    --expect-log "${read_by_key}387 \\(verified\\)" \
    --expect-log '\[GameClassSlotTests\] CCSPlayerController_ChangeTeam read by key: 104 \(verified\)' \
    --check "$(outcome passed GameClassSlotTests.GameClassVirtuals_ReadByKey_FollowTheVtableCheck \
        GameTests.Offset_CBasePlayerPawn_CommitSuicide GameTests.Offset_CCSPlayerController_Respawn \
        GameTests.Offset_CBaseEntity_Teleport GameTests.Offset_CCSPlayerController_ChangeTeam \
        GameTests.Offset_CCSPlayer_ItemServices_GiveNamedItem GameTests.Offset_CCSPlayer_ItemServices_RemoveWeapons)" ||
    status=1

echo "== game-class-virtuals: CommitSuicide one slot too low"
"$run" "$@" --no-build \
    --gamedata-jq ".$key.offsets.linux = 386" \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved, 1 healed(, [0-9]+ unverified)?$' \
    --expect-log "Healed $key: CBasePlayerPawn \\(server\\) slot 386 -> 387, window \\[[0-9a-f]{16}, [0-9a-f]{16}, [0-9a-f]{16}\\] matched once within 16 slots" \
    --expect-log 'Engine Features for build [^:]+: all [0-9]+ available' \
    --expect-log "^ +Healed $key: CBasePlayerPawn \\(server\\) slot 386 -> 387, window " \
    --expect-log "${read_by_key}387 \\(healed from 386\\)" \
    --check "$(outcome passed GameClassSlotTests.GameClassVirtuals_ReadByKey_FollowTheVtableCheck \
        GameTests.Offset_CBasePlayerPawn_CommitSuicide GameTests.Offset_CCSPlayerController_Respawn)" \
    --check "cmp gamedata.installed.json gamedata.after-run.json && jq -e '.$key.offsets.linux == 386' gamedata.after-run.json" ||
    status=1

echo "== game-class-virtuals: CommitSuicide's fingerprint corrupted"
typed_exception='\[PlayerHelperFailSoftTests\] CommitSuicide threw EngineFeatureUnavailableException: '
typed_exception+="Engine Feature cssharp\\.player/commit-suicide is disabled \\($key: fingerprint mismatch\\)"
"$run" "$@" --no-build \
    --gamedata-jq ".$key.vtable.linux.window[1] = \"0000000000000000\"" \
    --expect-log "Bindings for build [^:]+: 1 of [0-9]+ failed: $key \\(fingerprint mismatch\\)" \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-log "^ +cssharp\\.player/commit-suicide disabled \\($key: fingerprint mismatch\\)" \
    --expect-log "$typed_exception" \
    --expect-log "${read_by_key}Offset $key failed its Vtable Check on this build \\(fingerprint mismatch\\)" \
    --check "$(outcome failed GameTests.Offset_CBasePlayerPawn_CommitSuicide)" \
    --check "$(outcome passed GameClassSlotTests.GameClassVirtuals_ReadByKey_FollowTheVtableCheck \
        GameTests.Offset_CBaseEntity_Teleport PlayerHelperFailSoftTests.CommitSuicide_ThrowsTypedExceptionWhenDisabled \
        PlayerHelperFailSoftTests.RespawnAndTeleport_WorkWithoutCommitSuicide)" ||
    status=1

exit $status
