import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from backtest import *
o = gd('5465e2aap')['GetCSWeaponDataFromKey']['signatures']['linux']; n = gd('5465e2aa')['GetCSWeaponDataFromKey']['signatures']['linux']
for b, s in (('1.41.9.1b', o), ('1.41.9.1b', n), ('1.41.9.2', o), ('1.41.9.2', n)):
    X = ctx(b); m = X.m
    for h in m.im.scan(s, limit=5):
        f = m.fm.func_of(h)
        cs = X.a.callers.get(f, [])
        cfs = sorted(set(m.fm.func_of(c) for c in cs))
        jm = [a for a, t in m.fm.jmps if t == f]
        print(b, 'hit', hex(h), 'func', hex(f), 'ninsn', m._fp[f]['ninsn'], 'size', m._fp[f]['size'], 'ncallers', len(cs), 'caller funcs', len(cfs), 'tailjmps', len(jm), 'drefs', len(X.a.datarefs.get(f, [])), 'strs', m.strrefs(f)[:5])
        print('   callee', [hex(t) for t in m.calls_in(f)][:10])
        print('   callers w/ ustr', [(hex(c), X.a.uniq_strings(c)[:1]) for c in cfs[:8]])
