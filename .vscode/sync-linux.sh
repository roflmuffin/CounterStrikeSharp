#!/usr/bin/env bash
set -euo pipefail

workspace=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)

if [[ -f "$workspace/.env" ]]; then
    source "$workspace/.env"
fi

username=${LINUX_SERVER_SFTP_USERNAME:-${SFTP_USER:-}}
password=${LINUX_SERVER_SFTP_PASSWORD:-${SFTP_PASS:-}}
host=${LINUX_SERVER_SFTP_HOST:-${SFTP_HOST:-}}

if [[ -z "$username" || -z "$password" || -z "$host" ]]; then
    echo 'Set LINUX_SERVER_SFTP_USERNAME/PASSWORD/HOST or SFTP_USER/PASS/HOST in your environment or workspace .env.' >&2
    exit 1
fi

lftp_quote() {
    local value=$1
    value=${value//\\/\\\\}
    value=${value//\"/\\\"}
    value=${value//\$/\\\$}
    value=${value//\`/\\\`}
    printf '"%s"' "$value"
}

lftp <<EOF
set cmd:fail-exit yes
open -u $(lftp_quote "$username"),$(lftp_quote "$password") $(lftp_quote "$host")
mirror -R $(lftp_quote "$workspace/build/addons") /game/csgo/addons
mirror -R $(lftp_quote "$workspace/managed/CounterStrikeSharp.API/bin/Release/net10.0/") /game/csgo/addons/counterstrikesharp/api
mirror -R $(lftp_quote "$workspace/managed/CounterStrikeSharp.Tests.Native/bin/Debug/net10.0/") /game/csgo/addons/counterstrikesharp/plugins/NativeTestsPlugin
EOF
