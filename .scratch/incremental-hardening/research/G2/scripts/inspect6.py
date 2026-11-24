import sys, os, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from backtest import *
so = gd('2eb9a57dp')['ClientPrint']['signatures']['linux']; sn = gd('2eb9a57d')['ClientPrint']['signatures']['linux']
for b in ['1.41.8.1', '1.41.8.2', '1.41.9.0', '1.41.9.1a', '1.41.9.1b', '1.41.9.2', '1.41.9.6', '1.41.9.7']:
    X = ctx(b); m = X.m
    ho = m.im.scan(so); hn = m.im.scan(sn)
    d = []
    for h in ho:
        f = m.fm.func_of(h)
        cf = sorted(set(m.fm.func_of(c) for c in X.a.callers.get(f, [])))
        d.append((hex(h), 'func' if f == h else 'MID', m._fp[f]['ninsn'], len(X.a.callers.get(f, [])), [X.a.uniq_strings(c)[:1] for c in cf][:3]))
    print(b, 'old', d, 'fixed', [hex(x) for x in hn])
M = pickle.load(open(f'{G}/cache/glob_1.41.8.1_1.41.8.2.pkl', 'rb'))[0]
A, B = ctx('1.41.8.1'), ctx('1.41.8.2')
f = A.m.im.scan(so)[0]; g = M.get(f); print('8.1 func', hex(f), '-> global 8.2', hex(g), 'ninsn', B.m._fp[g]['ninsn'], 'ncallers', len(B.a.callers.get(g, [])))
print('bytes 8.2 global target:', B.m.im.read(g, 32).hex(' '))
print('bytes 8.1 sig hit      :', A.m.im.read(f, 32).hex(' '))
