#!/usr/bin/env bash
# End-to-end case for cssharp.network/check-transmit (Bindings: ISource2GameEntities::CheckTransmit and
# CheckTransmitPlayerSlot offsets). The valid case is covered by game-events.sh.
#
#   eng/e2e/cases/check-transmit.sh [run.sh options...]     e.g. --no-build
#
# offset missing: the CheckTransmit offset used to throw "Invalid virtual hook index" and abort the whole load; now
#                 CounterStrikeSharp loads, check-transmit is reported disabled and its hook is not registered.
set -uo pipefail

run=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/run.sh

echo "== check-transmit: ISource2GameEntities::CheckTransmit missing"
"$run" "$@" \
    --gamedata-jq 'del(."ISource2GameEntities::CheckTransmit")' \
    --expect-log 'Bindings for build [^:]+: [0-9]+ of [0-9]+ failed: .*ISource2GameEntities::CheckTransmit \(missing from gamedata\)' \
    --expect-log 'Engine Feature cssharp\.network/check-transmit disabled \(ISource2GameEntities::CheckTransmit: missing from gamedata\), not registering CheckTransmit hook' \
    --expect-log '^ +cssharp\.network/check-transmit disabled \(ISource2GameEntities::CheckTransmit: missing from gamedata\)' \
    --expect-log 'CheckTransmit listener registered on disabled cssharp\.network/check-transmit disabled \(ISource2GameEntities::CheckTransmit: missing from gamedata\); it will never fire' \
    --expect-no-log 'Invalid virtual hook index'
