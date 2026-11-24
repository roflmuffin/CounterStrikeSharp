#!/bin/bash
# For each commit touching the schema dump (server.json), count removed/added generated C# schema properties & classes.
cd /home/markus/dev/other/CounterStrikeSharp
for h in $(git log --format=%h --reverse -- managed/CounterStrikeSharp.SchemaGen/Schema/server.json); do
  d=$(git show -s --format='%ad' --date=short $h); s=$(git show -s --format='%s' $h | cut -c1-70)
  diff=$(git show --format= $h -- 'managed/CounterStrikeSharp.API/Core/Schema' 'managed/CounterStrikeSharp.API/Generated' 2>/dev/null)
  rp=$(echo "$diff" | grep -cE '^-\s+public (new )?[^(]+ [A-Za-z0-9_]+ (\{|=>)')
  ap=$(echo "$diff" | grep -cE '^\+\s+public (new )?[^(]+ [A-Za-z0-9_]+ (\{|=>)')
  rc=$(git show --format= --diff-filter=D --name-only $h -- 'managed/CounterStrikeSharp.API/Core/Schema' 'managed/CounterStrikeSharp.API/Generated' | wc -l)
  ac=$(git show --format= --diff-filter=A --name-only $h -- 'managed/CounterStrikeSharp.API/Core/Schema' 'managed/CounterStrikeSharp.API/Generated' | wc -l)
  echo "$h $d props:-$rp/+$ap files:-$rc/+$ac | $s"
done
