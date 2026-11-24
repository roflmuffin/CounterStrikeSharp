#!/usr/bin/env bash
# End-to-end cases for Required Engine Features (ADR-0002), with RequiresSayHooksTestPlugin, which requires
# cssharp.chat/say-hooks (Binding: Host_Say).
#
#   eng/e2e/cases/required-engine-features.sh [run.sh options...]     e.g. --no-build
#
# valid:  Host_Say resolves; the plugin loads.
# broken: Host_Say is not found; the plugin is refused at load naming the Engine Feature, Binding and reason, while
#         the Native Tests plugin still loads and the server stays up.
# Exits non-zero if any case fails; each case's output dir is printed by run.sh.
set -uo pipefail

run=$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/run.sh
plugin=managed/CounterStrikeSharp.Tests.RequiredEngineFeatures/RequiresSayHooksTestPlugin.csproj

status=0

echo "== required-engine-features: valid gamedata"
"$run" "$@" --plugin-project "$plugin" \
    --expect-log 'Finished loading plugin Requires Say Hooks Test' \
    --expect-no-log 'requires Engine Features that are not available' \
    --expect-no-log 'do not match the loaded image|Cannot get original bytes' ||
    status=1

echo "== required-engine-features: Host_Say not found"
"$run" "$@" --no-build --plugin-project "$plugin" \
    --gamedata-jq '.Host_Say.signatures.linux = "DE AD BE EF 13 37 C0 DE 42 42"' \
    --expect-log 'Plugin "RequiresSayHooksTestPlugin\.dll" requires Engine Features that are not available on build [^:]+: cssharp\.chat/say-hooks disabled \(Host_Say: not found\)\.' \
    --expect-log 'Engine Features for build [^:]+: [0-9]+ of [0-9]+ available, 1 disabled:' \
    --expect-no-log 'not registering|Failed to initialize GameSystem' \
    --expect-log 'Finished loading plugin Native Tests' \
    --expect-no-log 'Finished loading plugin Requires Say Hooks Test' ||
    status=1

exit $status
