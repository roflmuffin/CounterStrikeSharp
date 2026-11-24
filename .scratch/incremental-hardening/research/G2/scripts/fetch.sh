#!/bin/bash
G=/tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/G2
for pair in 1.41.8.1:cs2-3039db9e0bae700e08591779596c1a8a438fd68d-a079f93c19774d01b853dc581cc3035e 1.41.8.2:cs2-fb01c85b38d0a5e9840a313eb32a4ef9b07a1d9e-edd5a0caf0c54fb7aaebc7e7409ace3a 1.41.9.7:cs2-83c960c4045b80b2376c4a1fa66259a966f66db1-c2f4b3ceafc844bcb90aa644823ee73b 1.41.9.0:cs2-d9dcd9e5c96638134e6b2774c9bbd1dfa1c2b061-73149f324c9947cb95803cf1694b7ded 1.41.9.1a:cs2-5ed38a02dd0297ef249540c3c0db45e5504096fd-bf750be7070443d39d14fba1199e922a 1.41.9.1b:cs2-da46fc3a096c63b04767a9783efa6c8cf65b9fcb-d2452f94ee334e3b98af11d285b6c734 1.41.9.6:cs2-aa1103f44c7544be8c7c348e24425c9d1c857d0b-a902109ec3c74988b83e930d948cc55a; do
  b=${pair%%:*}; t=${pair#*:}
  [ -f $G/img/server_$b.img ] && continue
  d=$G/dl/$b; mkdir -p $d; cd $d
  gh release download $t -R Swiftly-Tracker/CS2-IDA-Dumps -p libserver.so.i64.7z --clobber || { echo "dl fail $b"; continue; }
  7z x -y libserver.so.i64.7z >/dev/null || { echo "7z fail $b"; continue; }
  sha256sum libserver.so.i64.7z > archive.sha256
  f=$(find . -name 'libserver.so.i64' | head -1)
  python3 -I $G/scripts/i64extract.py "$f" $G/img/server_$b > $G/img/server_$b.log 2>&1 && echo "ok $b"
  rm -f "$f" libserver.so.i64.7z
done
