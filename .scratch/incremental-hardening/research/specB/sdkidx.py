import sys
sys.argv = [sys.argv[0], '/dev/null', '/home/markus/dev/other/cssharp-hardening-research/scratchpad/B/s2sdk']
exec(open('/home/markus/.claude/jobs/a0ac098e/tmp/specb/sdkdiff.py').read().split('for l in open')[0])
sha = 'd4a8b46676ded25b9d956287ec40ef3c9e12c14c'
for c in ['IAppSystem', 'ISource2Server', 'ISource2GameClients', 'ISource2GameEntities', 'IGameEventManager2', 'IBaseInterface']:
    b, v = virts(sha, c)
    print(c, b, len(v), v)
