import sys, os, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from backtest import *
def show(b, gtsig, cls, idx_list):
    X = ctx(b); m = X.m
    gt = m.fm.func_of(m.im.scan(gtsig)[0])
    callers = [m.fm.func_of(c) for c in X.a.callers.get(gt, [])]
    print(b, 'GT', hex(gt), 'ninsn', m._fp[gt]['ninsn'], 'callers', [hex(c) for c in callers], [X.a.slots.get(c, [])[:2] for c in callers])
    vt = X.vt(cls)
    for i in idx_list:
        f = vt[i]; print('  slot', i, hex(f), 'ninsn', m._fp[f]['ninsn'], 'calls', [hex(t) for t in m.calls_in(f)][:6], 'calls GT' if gt in m.calls_in(f) else '')
sig82 = gd('2f984988')['CBaseTrigger_StartTouch']['signatures']['linux']
sig91 = gd('c67396fa')['CBaseTrigger_StartTouch']['signatures']['linux']
show('1.41.8.2', sig82, '12CBaseTrigger', range(146, 152))
show('1.41.9.0', sig82, '12CBaseTrigger', range(146, 152))
show('1.41.9.1a', sig91, '12CBaseTrigger', range(146, 152))
X = ctx('1.41.8.1'); vt = X.vt('12CBaseTrigger'); print('8.1 slot147', hex(vt[147]), 'ninsn', X.m._fp[vt[147]]['ninsn'], 'calls', [hex(t) for t in X.m.calls_in(vt[147])][:8])
# EndTouch on 8.1: containing func & slots
