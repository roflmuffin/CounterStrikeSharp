import sys, os, pickle, collections, difflib
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from backtest import *
TECH = ['hash', 'ustring', 'vtable', 'caller']  # a-priori priority order; 'global' reported separately
def sim(A, fa, B, fb):
    return difflib.SequenceMatcher(None, A.m.norm_tokens(fa), B.m.norm_tokens(fb), autojunk=False).ratio()
def resolve_all(A, B, fN, name, M):
    an = A.a.anchors(fN); res = {}
    if 'ustring' in an: res['ustring'] = B.a.resolve(an, 'ustring')
    if 'caller' in an: res['caller'] = B.a.resolve(an, 'caller')
    elif 'caller_vt' in an: res['caller'] = B.a.resolve(an, 'caller_vt')
    sc = slot_class(A, fN, name)
    if sc:
        mp, la, lb = vt_align(A, B, sc[0]); j = mp.get(sc[1]) if mp else None
        res['vtable'] = B.vt(sc[0])[j[0]] if j else None
    h = A.m._fp.get(fN, {}).get('norm')
    if h and len(A.norm_index().get(h, [])) == 1:
        c = B.norm_index().get(h, []); res['hash'] = c[0] if len(c) == 1 else None
    if fN in M: res['global'] = M[fN]
    # post-hoc variant: vtable wrapper that tail-jumps to target
    jsrc = [a for a, t in A.m.fm.jmps if t == fN]
    for a in jsrc:
        w = A.m.fm.func_of(a); s2 = slot_class(A, w, name)
        if s2:
            mp, la, lb = vt_align(A, B, s2[0]); j = mp.get(s2[1]) if mp else None
            if j:
                w2 = B.vt(s2[0])[j[0]]; e = B.m.end(w2)
                tj = [t for x, t in B.m.fm.jmps if w2 <= x < e + 0x40 and B.m.im.in_text(t) and not (w2 <= t < e + 0x40)]
                res['vt_tailjmp'] = tj[0] if len(set(tj)) == 1 else None
            break
    return res
def policies(res, gtf):
    pre = {k: v for k, v in res.items() if k in TECH and v is not None}
    out = {}
    out['any'] = any(v == gtf for v in pre.values())
    pr = next((pre[k] for k in TECH if k in pre), None)
    out['priority'] = 'none' if pr is None else ('OK' if pr == gtf else 'WRONG')
    cnt = collections.Counter(pre.values()); agree = [f for f, c in cnt.items() if c >= 2]
    out['agree2'] = 'abstain' if not agree else ('OK' if agree[0] == gtf else 'WRONG')
    pre5 = {k: v for k, v in res.items() if k in TECH + ['global'] and v is not None}
    cnt = collections.Counter(pre5.values()); agree = [f for f, c in cnt.items() if c >= 2]
    out['agree2+global'] = 'abstain' if not agree else ('OK' if agree[0] == gtf else 'WRONG')
    return out
if __name__ == '__main__':
    broken, unbroken = [], []
    for N, N1, gN, gN1 in PAIRS:
        A, B = ctx(N), ctx(N1); M = pickle.load(open(f'{G}/cache/glob_{N}_{N1}.pkl', 'rb'))[0]
        old, new = gd(gN), gd(gN1)
        for k, e in old.items():
            so = lsig(e); sn = lsig(new[k]) if k in new else None
            if not so or not sn: continue
            mN = A.m.im.scan(so, limit=5); gt = B.m.im.scan(sn, limit=5)
            if not mN or len(gt) != 1: continue
            fN = A.m.fm.func_of(mN[0]); gtf = B.m.fm.func_of(gt[0])
            res = resolve_all(A, B, fN, k, M)
            lab = {t: ('OK' if v == gtf else ('none' if v is None else 'WRONG')) for t, v in res.items()}
            sims = {t: round(sim(A, fN, B, v), 2) for t, v in res.items() if v is not None and v != gtf}
            row = dict(pair=f'{N}->{N1}', name=k, lab=lab, pol=policies(res, gtf), sim_gt=round(sim(A, fN, B, gtf), 2), sim_wrong=sims)
            (broken if so != sn else unbroken).append(row)
    for r in broken: print('BROKEN', r)
    print()
    for grp, rows in (('broken', broken), ('unbroken', unbroken)):
        c = collections.Counter()
        for r in rows:
            for t, l in r['lab'].items(): c[(t, l)] += 1
            for p, v in r['pol'].items(): c[('pol:' + p, v)] += 1
        print(grp, len(rows))
        for k in sorted(c): print('  ', k, c[k])
    pickle.dump((broken, unbroken), open(f'{G}/cache/final.pkl', 'wb'))
    for r in unbroken:
        if any(l == 'WRONG' for l in r['lab'].values()): print('UNBROKEN-WRONG', r)
