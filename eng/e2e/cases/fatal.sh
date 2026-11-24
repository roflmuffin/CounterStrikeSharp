#!/usr/bin/env bash
# End-to-end cases for the loader-level failures that stay fatal (ADR-0002): CounterStrikeSharp refuses to load,
# Metamod reports why, and the game server itself keeps running without it.
#
#   eng/e2e/cases/fatal.sh [run.sh options...]     e.g. --no-build
#
# dotnet:   the CounterStrikeSharp API assembly is removed, so .NET fails to start. Used to log an error and keep a
#           half-loaded CounterStrikeSharp (native hooks, no plugins); now Load fails and undoes what it registered.
# gamedata: gamedata.json is not valid JSON.
# Exits non-zero if any case fails; each case's output dir is printed by run.sh.
set -uo pipefail

run=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/run.sh

status=0

echo "== fatal: .NET fails to start"
"$run" "$@" --no-css-ready \
    --remove-installed api/CounterStrikeSharp.API.dll \
    --expect-log 'Failed to initialize \.NET runtime, CounterStrikeSharp will not load' \
    --expect-log '\[META\] Failed to load plugin .*counterstrikesharp.*Failed to initialize \.NET runtime' \
    --expect-no-log 'CounterStrikeSharp\.API Loaded Successfully|Initialized GameSystem' ||
    status=1

broken=$(mktemp --suffix=.json)
trap 'rm -f "$broken"' EXIT
printf '{ "Host_Say": { "signatures": ' >"$broken"

echo "== fatal: gamedata unparseable"
"$run" "$@" --no-build --no-css-ready \
    --gamedata "$broken" \
    --expect-log "Could not read '.*gamedata\.json'\. Error: Failed to parse gamedata file" \
    --expect-log '\[META\] Failed to load plugin .*counterstrikesharp' \
    --expect-no-log 'Bindings for build|Loading \.NET runtime' ||
    status=1

exit $status
