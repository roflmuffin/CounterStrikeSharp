import os
S='/tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/C'
LOCAL=os.path.expanduser('~/.steam/steam/steamapps/common/Counter-Strike Global Offensive/game')
BUILDS={
 '1.41.9.7': S+'/dl/b14197/game',
 '1.42.0.0': LOCAL,
 '14201': S+'/dl/b14201/game',
}
LIBS={'server':'csgo/bin/linuxsteamrt64/libserver.so','engine':'bin/linuxsteamrt64/libengine2.so','schemasystem':'bin/linuxsteamrt64/libschemasystem.so','tier0':'bin/linuxsteamrt64/libtier0.so'}
def path(build, lib): return os.path.join(BUILDS[build], LIBS[lib])
