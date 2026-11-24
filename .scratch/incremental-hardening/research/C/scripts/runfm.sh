#!/bin/bash
S=/tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/C
L="$HOME/.steam/steam/steamapps/common/Counter-Strike Global Offensive/game"
$S/venv/bin/python -I $S/scripts/funcmap.py "$L/csgo/bin/linuxsteamrt64/libserver.so" $S/cache/fm_server_1.42.0.0.pkl &
$S/venv/bin/python -I $S/scripts/funcmap.py "$S/dl/b14197/game/csgo/bin/linuxsteamrt64/libserver.so" $S/cache/fm_server_1.41.9.7.pkl &
$S/venv/bin/python -I $S/scripts/funcmap.py "$S/dl/b14201/game/csgo/bin/linuxsteamrt64/libserver.so" $S/cache/fm_server_14201.pkl &
$S/venv/bin/python -I $S/scripts/funcmap.py "$L/bin/linuxsteamrt64/libengine2.so" $S/cache/fm_engine_1.42.0.0.pkl &
$S/venv/bin/python -I $S/scripts/funcmap.py "$S/dl/b14197/game/bin/linuxsteamrt64/libengine2.so" $S/cache/fm_engine_1.41.9.7.pkl &
wait
