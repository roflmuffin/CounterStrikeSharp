"""Fuzzy re-matching of broken historical CSSharp linux sigs against build 1.41.9.7 (ground truth = HEAD sig)."""
import sys, os, re, difflib, collections, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from fp import Mod
from builds import S
from sighist import dis, REG, HEX
from sigscan import load_sigs
import capstone
md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)

BUILD = sys.argv[1] if len(sys.argv) > 1 else '1.41.9.7'
m = Mod(BUILD, 'server')
res = pickle.load(open(S + '/cache/sigscan.pkl', 'rb'))
head = load_sigs('/home/markus/dev/other/CounterStrikeSharp/configs/addons/counterstrikesharp/gamedata/gamedata.json')
truth = {}
for k, (lib, s) in head.items():
    r = m.im.scan(s, limit=3)
    if len(r) == 1:
        truth[k] = r[0]

# prefix tokens of every function
starts = [f for f in m.fm.starts if m.im.in_text(f)]
pre = {}
for f in starts:
    b = m.im.read(f, 96)
    pre[f] = [(mn, o) for a, s, mn, o in md.disasm_lite(b, f)][:24]


def lv(t, level):
    mn, o = t
    if level == 'exact':
        return mn + ' ' + re.sub(r'0x[0-9a-f]{3,}', 'I', o) if 'rip' in o else mn + ' ' + o
    if level == 'imm':
        return mn + ' ' + HEX.sub('I', o)
    if level == 'reg':
        return mn + ' ' + REG.sub('R', HEX.sub('I', o))
    if level == 'mnem':
        return mn


import numpy as np
vocab = {}
MAT = np.full((len(starts), 24), -1, dtype=np.int32)
for i, f in enumerate(starts):
    for j, x in enumerate(pre[f][:24]):
        MAT[i, j] = vocab.setdefault(lv(x, 'reg'), len(vocab))
out = collections.defaultdict(collections.Counter)
rows = []
for (k, sig), r in res.items():
    if k not in truth or len(r['matches'].get(BUILD, [])) > 0:
        continue
    t = truth[k]
    if m.fm.func_of(t) != t:
        continue  # mid-function sig; skip
    A = dis(sig)
    if len(A) > 1:
        A = A[:-1]  # last may be truncated by sig end
    if len(A) < 3:
        continue
    out['n']['n'] += 1
    row = dict(name=k, first=r['first'], last=r['last'], ninsn=len(A))
    for level in ('imm', 'reg', 'mnem'):
        q = [lv(x, level) for x in A]
        hits = [f for f in starts if [lv(x, level) for x in pre[f][:len(q)]] == q]
        if len(hits) == 1 and hits[0] == t:
            c = 'unique-correct'
        elif t in hits:
            c = f'ambiguous(incl. truth)'
        elif hits:
            c = 'wrong'
        else:
            c = 'none'
        out[level][c] += 1
        row[level] = (c, len(hits))
    # best positional similarity (reg/imm-abstracted tokens, shifts -3..3), vectorized
    q = [vocab.setdefault(lv(x, 'reg'), len(vocab)) for x in A]
    sc = np.zeros(len(starts))
    for sh in range(-3, 4):
        cnt = np.zeros(len(starts))
        for i, tq in enumerate(q):
            j = i + sh
            if 0 <= j < MAT.shape[1]:
                cnt += (MAT[:, j] == tq)
        sc = np.maximum(sc, cnt)
    order = np.argsort(-sc)[:2]
    b0, b1 = starts[order[0]], starts[order[1]]
    c = 'top1-correct' if b0 == t else 'top1-wrong'
    if b0 == t and sc[order[1]] == sc[order[0]]:
        c = 'top1-tied'
    out['similarity'][c] += 1
    row['sim'] = (c, sc[order[0]] / len(q), sc[order[1]] / len(q))
    rows.append(row)

for r in sorted(rows, key=lambda r: r['last']):
    print(r)
for k, v in out.items():
    print(k, dict(v))
pickle.dump((rows, dict(out)), open(S + f'/cache/fuzzy_{BUILD}.pkl', 'wb'))
