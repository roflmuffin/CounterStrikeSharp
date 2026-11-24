#!/usr/bin/env bash
# End-to-end cases for cssharp.entity/take-damage-hooks (Bindings: CBaseEntity_TakeDamageOld, CBaseEntity_IsPlayerPawn)
# and cssharp.entity/take-damage-hit-group (CTakeDamageInfo_HitGroup).
#
#   eng/e2e/cases/take-damage.sh [run.sh options...]     e.g. --no-build
#
# valid:  both available; the take-damage probe registers listeners silently and reads a hit group.
# broken: the CBaseEntity_TakeDamageOld key is removed and the hit-group offset has no linux entry. The server stays up,
#         both Engine Features are disabled independently, the take-damage detour is not installed, listeners register
#         but warn once, reading the hit group throws the typed exception, and a plugin raw-signature memory function
#         still binds (no read of the missing take-damage key). RequiresTakeDamageHooksTestPlugin is refused at load.
# Exits non-zero if any case fails; each case's output dir is printed by run.sh.
set -uo pipefail

run=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/run.sh
plugin=managed/CounterStrikeSharp.Tests.RequiresTakeDamageHooks/RequiresTakeDamageHooksTestPlugin.csproj

status=0

# "fired" in the valid case relies on bots damaging each other during the smoke run (they do on de_dust2 warmup); it
# is the positive control for "never fires" in the broken case.
echo "== take-damage: valid gamedata"
"$run" "$@" --plugin-project "$plugin" \
    --exec css_take_damage_probe \
    --expect-log 'Finished loading plugin Requires Take Damage Hooks Test' \
    --expect-log '\[TakeDamageProbe\] OnPlayerTakeDamagePre fired' \
    --expect-log 'Bindings for build [^:]+: all [0-9]+ resolved' \
    --expect-log 'Engine Features for build [^:]+: all 36 available' \
    --expect-log '\[TakeDamageProbe\] registered take-damage listeners' \
    --expect-log '\[TakeDamageProbe\] hit group: HITGROUP_INVALID' \
    --expect-log '\[TakeDamageProbe\] raw signature memory function bound: True' \
    --expect-no-log 'take-damage-hooks disabled|take-damage-hit-group disabled|registered on [^ ]+ disabled' \
    --expect-no-log 'do not match the loaded image|Cannot get original bytes' ||
    status=1

echo "== take-damage: CBaseEntity_TakeDamageOld missing, hit-group offset missing for linux"
hit_group_exception='\[TakeDamageProbe\] hit group: EngineFeatureUnavailableException: Engine Feature '
hit_group_exception+='cssharp\.entity/take-damage-hit-group is disabled \(CTakeDamageInfo_HitGroup: missing for this OS\)'
refusal='Plugin "RequiresTakeDamageHooksTestPlugin\.dll" requires Engine Features that are not available on build [^:]+: '
refusal+='cssharp\.entity/take-damage-hooks disabled \(CBaseEntity_TakeDamageOld: missing from gamedata\)\.'
"$run" "$@" --no-build --plugin-project "$plugin" \
    --gamedata-jq 'del(.CBaseEntity_TakeDamageOld)' \
    --gamedata-jq 'del(.CTakeDamageInfo_HitGroup.offsets.linux)' \
    --exec css_take_damage_probe \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*CBaseEntity_TakeDamageOld \(missing from gamedata\)' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 2 disabled:' \
    --expect-no-log 'not registering|Failed to initialize GameSystem' \
    --expect-log '^ +cssharp\.entity/take-damage-hooks disabled \(CBaseEntity_TakeDamageOld: missing from gamedata\)' \
    --expect-log '^ +cssharp\.entity/take-damage-hit-group disabled \(CTakeDamageInfo_HitGroup: missing for this OS\)' \
    --expect-log 'OnPlayerTakeDamagePre listener registered on cssharp\.entity/take-damage-hooks disabled' \
    --expect-log "$hit_group_exception" \
    --expect-log '\[TakeDamageProbe\] raw signature memory function bound: True' \
    --expect-log "$refusal" \
    --expect-log 'Finished loading plugin Native Tests' \
    --expect-no-log 'Finished loading plugin Requires Take Damage Hooks Test' \
    --expect-no-log '\[TakeDamageProbe\] .* fired' ||
    status=1

exit $status
