import sys, os, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from backtest import *
for N, N1, gN, gN1, nm in (('1.41.8.1', '1.41.8.2', '2f984988p', '2f984988', 'CCSGameRules_TerminateRound'), ('1.41.9.6', '1.41.9.7', '2eb9a57dp', '2eb9a57d', 'ClientPrint'), ('1.41.8.1', '1.41.8.2', '2f984988p', '2f984988', 'IGameSystem_InitAllSystems_pFirst')):
    A, B = ctx(N), ctx(N1); M = pickle.load(open(f'{G}/cache/glob_{N}_{N1}.pkl', 'rb'))[0]
    so = gd(gN)[nm]['signatures']['linux']; sn = gd(gN1)[nm]['signatures']['linux']
    ha = A.m.im.scan(so); hb = B.m.im.scan(so); gt = B.m.im.scan(sn)
    print(nm, 'old on N', [hex(x) for x in ha], 'old on N1', [hex(x) for x in hb], 'GT', [hex(x) for x in gt])
    for x in ha:
        f = A.m.fm.func_of(x); print('  N  hit func', hex(f), 'ninsn', A.m._fp[f]['ninsn'], 'strs', A.m.strrefs(f)[:3], '-> global', hex(M[f]) if f in M else None)
    for x in hb:
        f = B.m.fm.func_of(x); print('  N1 hit func', hex(f), 'ninsn', B.m._fp[f]['ninsn'], 'strs', B.m.strrefs(f)[:3], 'ncallers', len(B.a.callers.get(f, [])), 'is GT func', f == B.m.fm.func_of(gt[0]))
