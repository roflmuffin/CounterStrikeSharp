import sys, os, pickle, collections
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from backtest import *
broken, unbroken = pickle.load(open(f'{G}/cache/final.pkl', 'rb'))
s = sorted(r['sim_gt'] for r in unbroken if not (r['name'] == 'ClientPrint'))
print('unbroken sim_gt min/5pct/median', s[:5], s[len(s)//2])
print('broken sim_gt', sorted(r['sim_gt'] for r in broken))
w = sorted(v for r in broken + unbroken for v in r['sim_wrong'].values())
print('wrong-candidate sims', w)
# caller strings of real ClientPrint 8.1 vs 8.2 global-mapped
A, B = ctx('1.41.8.1'), ctx('1.41.8.2')
def cs(X, f): return sorted(set(s for c in X.a.callers.get(f, []) for s in X.a.uniq_strings(X.m.fm.func_of(c))[:1]))
a, b = cs(A, 0x17c7b30), cs(B, 0x1801fa0)
print('caller-ustring overlap real ClientPrint 8.1 vs 8.2 global:', len(set(a) & set(b)), 'of', len(a), len(b))
c = cs(B, 0x1d11eb0); print('8.2 sig hit callers ustrings', c)
