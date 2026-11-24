#!/bin/bash
# usage: commit_files.sh <hash>...  -> per-commit file list (excluding generated schema bulk), plus submodule bumps
cd /home/markus/dev/other/CounterStrikeSharp
for h in "$@"; do
  echo "=== $(git log -1 --format='%h %ad %s' --date=short $h)"
  git show --stat=200 --format= $h | grep -v -E 'managed/CounterStrikeSharp.API/Generated/Schema|Core/Schema/Classes|Schema/Classes/|schema_classes|schema_enums|Generated/Schema' | sed 's/^/   /' | head -40
  git show $h --format= -- libraries | grep -E '^[-+]Subproject' | sed 's/^/   SUBMODULE /'
done
