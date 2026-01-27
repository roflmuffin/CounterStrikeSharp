#!/bin/bash

HERE=`dirname $0`

set -a
[ -f .env ] && source .env
set +a

set -e 
set -u

# Game server RCON details - must come from the environment
: "${GS_HOST:?GS_HOST env var is required}"
: "${GS_PORT:?GS_PORT env var is required}"
: "${GS_PASS:?GS_PASS env var is required}"

# SFTP connection details - must come from the environment
: "${SFTP_HOST:?SFTP_HOST env var is required}"
: "${SFTP_USER:?SFTP_USER env var is required}"
: "${SFTP_PASS:?SFTP_PASS env var is required}"

OUTPUT=$($HERE/rcon -a $GS_HOST:$GS_PORT -p $GS_PASS "dump_schema all")
FILEPATH=$(echo "$OUTPUT" | grep -oP 'Wrote file output to \K.*')
TRIMMED_PATH="${FILEPATH#/home/container}"

lftp -u "$SFTP_USER,$SFTP_PASS" "$SFTP_HOST" <<EOF
set xfer:clobber on
get "$TRIMMED_PATH" -o "$HERE/../managed/CounterStrikeSharp.SchemaGen/Schema/server.json"
bye
EOF

dotnet run --project managed/CounterStrikeSharp.SchemaGen/CounterStrikeSharp.SchemaGen.csproj
