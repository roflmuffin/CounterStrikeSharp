import sys, os, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from backtest import *
def info(b, sig, name):
    X = ctx(b); m = X.m
    hits = m.im.scan(sig, limit=5)
    for h in hits:
        f = m.fm.func_of(h)
        print(f'  {b} {name} hit {hex(h)} func {hex(f)} slots {X.a.slots.get(f)} ncallers {len(X.a.callers.get(f,[]))} datarefs {len(X.a.datarefs.get(f,[]))} ninsn {m._fp.get(f,{}).get("ninsn")} strs {m.strrefs(f)[:4]}')
    return hits
o = gd('2f984988p')['CBaseTrigger_StartTouch']['signatures']['linux']
n = gd('2f984988')['CBaseTrigger_StartTouch']['signatures']['linux']
n2 = gd('c67396fa')['CBaseTrigger_StartTouch']['signatures']['linux']
print('old', o); print('fixed8.2', n); print('fixed9.1', n2)
for b in ('1.41.8.1', '1.41.8.2', '1.41.9.0', '1.41.9.1a'):
    for s, nm in ((o, 'old'), (n, 'fix82'), (n2, 'fix91')):
        info(b, s, nm)
for nm in ('CBaseTrigger_EndTouch',):
    s = gd('2f984988')[nm]['signatures']['linux']
    for b in ('1.41.8.1', '1.41.8.2', '1.41.9.0', '1.41.9.1a'): info(b, s, nm)
