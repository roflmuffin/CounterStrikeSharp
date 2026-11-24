"""Cross-build function matching and drift statistics."""
import sys, os, pickle, collections, time
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from fp import Mod
from builds import S


def all_fps(m):
    cache = f'{S}/cache/fps_{m.lib}_{m.build}.pkl'
    if os.path.exists(cache):
        m._fp = pickle.load(open(cache, 'rb'))
        return m._fp
    t = time.time()
    for f in m.fm.starts:
        if m.im.in_text(f):
            m.fp(f)
    pickle.dump(m._fp, open(cache, 'wb'))
    print('fps', m.build, len(m._fp), time.time() - t, file=sys.stderr)
    return m._fp


def strset(m, f):
    return tuple(sorted(set(m.strrefs(f))))


def match(A, B):
    fa, fb = all_fps(A), all_fps(B)
    M = {}          # a -> b
    R = {}          # b -> a
    how = {}

    def add(a, b, h):
        if a in M or b in R:
            return False
        M[a] = b; R[b] = a; how[a] = h
        return True
    # 1. vtable slots of same class with same vtable length
    for n in set(A.im.ti_by_name) & set(B.im.ti_by_name):
        pa, pb = A.im.primary_vtable(n), B.im.primary_vtable(n)
        if pa and pb and len(pa[2]) == len(pb[2]):
            for x, y in zip(pa[2], pb[2]):
                if isinstance(x, int) and isinstance(y, int):
                    add(x, y, 'vtable')
    # 2. unique normalized hash
    for key in ('norm', 'mnem'):
        ia, ib = collections.defaultdict(list), collections.defaultdict(list)
        for f, d in fa.items():
            if f not in M and d['ninsn'] >= 6:
                ia[d[key]].append(f)
        for f, d in fb.items():
            if f not in R and d['ninsn'] >= 6:
                ib[d[key]].append(f)
        for h, l in ia.items():
            if len(l) == 1 and len(ib.get(h, [])) == 1:
                add(l[0], ib[h][0], key)
    # 3. unique string-set
    ia, ib = collections.defaultdict(list), collections.defaultdict(list)
    for f in fa:
        if f not in M:
            s = strset(A, f)
            if s: ia[s].append(f)
    for f in fb:
        if f not in R:
            s = strset(B, f)
            if s: ib[s].append(f)
    for h, l in ia.items():
        if len(l) == 1 and len(ib.get(h, [])) == 1:
            add(l[0], ib[h][0], 'strings')
    # 4. call-graph propagation
    changed = True
    while changed:
        changed = False
        for a, b in list(M.items()):
            ca, cb = A.calls_in(a), B.calls_in(b)
            if len(ca) == len(cb):
                for x, y in zip(ca, cb):
                    if x in fa and y in fb and add(x, y, 'callgraph'):
                        changed = True
    return M, how


def stats(A, B, M, how):
    fa, fb = A._fp, B._fp
    n = len(M)
    c = collections.Counter()
    for a, b in M.items():
        da, db = fa[a], fb[b]
        c['raw_same'] += da['raw'] == db['raw']
        c['norm_same'] += da['norm'] == db['norm']
        c['mnem_same'] += da['mnem'] == db['mnem']
        c['size_same'] += da['size'] == db['size']
    # layout: delta continuity
    pairs = sorted(M.items())
    cont = sum(1 for (a1, b1), (a2, b2) in zip(pairs, pairs[1:]) if (b2 - a2) == (b1 - a1))
    order = sum(1 for (a1, b1), (a2, b2) in zip(pairs, pairs[1:]) if b2 > b1)
    deltas = collections.Counter(b - a for a, b in pairs)
    return dict(funcsA=len(fa), funcsB=len(fb), matched=n, by=dict(collections.Counter(how.values())),
                pct_raw_identical=100 * c['raw_same'] / n, pct_norm_identical=100 * c['norm_same'] / n,
                pct_mnem_identical=100 * c['mnem_same'] / n, pct_size_same=100 * c['size_same'] / n,
                pct_same_delta_as_prev=100 * cont / (n - 1), pct_order_preserved=100 * order / (n - 1),
                distinct_deltas=len(deltas), top_deltas=[(hex(d), k) for d, k in deltas.most_common(5)])


if __name__ == '__main__':
    b1, b2, lib = sys.argv[1], sys.argv[2], sys.argv[3] if len(sys.argv) > 3 else 'server'
    A, B = Mod(b1, lib), Mod(b2, lib)
    M, how = match(A, B)
    pickle.dump((M, how), open(f'{S}/cache/match_{lib}_{b1}_{b2}.pkl', 'wb'))
    st = stats(A, B, M, how)
    for k, v in st.items():
        print(f'{k}: {v}')
