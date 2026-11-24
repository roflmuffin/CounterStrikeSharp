#!/bin/bash
cd /tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/C/dl/dd
for pair in 14197:3634295490837307276 14192:5491505037461703699 14177:3204075245827788235 14182:2422872143697310485 14199:9019027683964018897 14201:4380993310429599899; do
  t=${pair%%:*}; m=${pair##*:}
  mkdir -p /tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/C/dl/b$t
  ./DepotDownloader -app 730 -depot 2347773 -manifest $m -filelist /tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/C/scripts/filelist.txt -dir /tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/C/dl/b$t -max-downloads 8 > /tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/C/out/dl_$t.log 2>&1
  echo "$t done rc=$?"
done
