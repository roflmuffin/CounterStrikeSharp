"""Independent-ground-truth backtest of anchor-based re-resolution across CSSharp-breaking CS2 updates."""
import sys, os, json, pickle, collections, difflib, bisect
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from g2mod import M2, G
import anchors, match
match.all_fps = lambda m: m.all_fps()

PAIRS = [  # (N, N+1, gamedata valid at N, fixed gamedata for N+1)
    ('1.41.8.1', '1.41.8.2', '2f984988p', '2f984988'),
    ('1.41.9.0', '1.41.9.1a', 'c67396fap', 'c67396fa'),
    ('1.41.9.1b', '1.41.9.2', '5465e2aap', '5465e2aa'),
    ('1.41.9.6', '1.41.9.7', '2eb9a57dp', '2eb9a57d'),
]
NONVT = {'CTakeDamageInfo_HitGroup', 'GameEntitySystem', 'GameEventManager', 'CheckTransmitPlayerSlot', 'SetStateChanged'}

def gd(n): return json.load(open(f'{G}/gd/{n}.json'))
def lsig(e):
    s = e.get('signatures', {})
    return s['linux'] if s.get('linux') and s.get('library', 'server') == 'server' else None
def loff(e): return e.get('offsets', {}).get('linux')
def cls_of(name):
    if '::' in name:
        c = name.split('::')[0]; return 'C' + c[1:] if c.startswith('I') else c
    return name.rsplit('_', 1)[0]

class Ctx:
    cache = {}
    def __init__(self, b):
        self.m = M2(b); self.m.all_fps(); self.a = anchors.Anch(self.m)
        self._norm = None
    def norm_index(self):
        if self._norm is None:
            d = collections.defaultdict(list)
            for f, x in self.m._fp.items():
                if x['ninsn'] >= 6: d[x['norm']].append(f)
            self._norm = d
        return self._norm
    def vt(self, cls):
        p = self.m.im.primary_vtable(cls)
        return p[2] if p else None
def ctx(b):
    if b not in Ctx.cache: Ctx.cache[b] = Ctx(b)
    return Ctx.cache[b]

def tok(m, f):
    return f if isinstance(f, str) else m.fp(f)['mnem']

def vt_align(A, B, cls):
    """map old slot idx -> (new idx, how) by mnemonic-hash sequence alignment, fuzzy inside unequal replace blocks."""
    va, vb = A.vt(cls), B.vt(cls)
    if va is None or vb is None: return None, None, None
    ta, tb = [tok(A.m, f) for f in va], [tok(B.m, f) for f in vb]
    sm = difflib.SequenceMatcher(None, ta, tb, autojunk=False)
    mp = {}
    for op, i1, i2, j1, j2 in sm.get_opcodes():
        if op == 'equal':
            for k in range(i2 - i1): mp[i1 + k] = (j1 + k, 'eq')
        elif op == 'replace':
            if i2 - i1 == j2 - j1:
                for k in range(i2 - i1): mp[i1 + k] = (j1 + k, 'repl-pos')
            else:
                for i in range(i1, i2):
                    fa = va[i]
                    if isinstance(fa, str): continue
                    na = A.m.norm_tokens(fa)
                    best = max(((difflib.SequenceMatcher(None, na, B.m.norm_tokens(vb[j]), autojunk=False).ratio(), j)
                                for j in range(j1, j2) if not isinstance(vb[j], str)), default=(0, None))
                    if best[1] is not None and best[0] >= 0.5: mp[i] = (best[1], 'fuzzy%.2f' % best[0])
    return mp, len(va), len(vb)

def slot_class(A, f, name):
    sl = A.a.slots.get(f, [])
    pref = cls_of(name)
    for c, i in sl:
        if c.lstrip('0123456789') == pref: return c, i
    return sl[0] if sl else None

def run_pair(N, N1, gN, gN1, GM):
    A, B = ctx(N), ctx(N1)
    old, new = gd(gN), gd(gN1)
    rows, orows = [], []
    for k, e in old.items():
        so, sn = lsig(e), (lsig(new[k]) if k in new else None)
        if so and so != sn:
            r = dict(pair=f'{N}->{N1}', name=k)
            mN = A.m.im.scan(so, limit=20)
            r['old_on_N'] = len(mN)
            if not sn:
                r['note'] = 'removed in fixed gamedata (no GT)'; rows.append(r); continue
            gt = B.m.im.scan(sn, limit=20)
            r['gt_n'] = len(gt)
            if len(gt) != 1 or not mN:
                r['note'] = 'GT not unique / not found on N'; rows.append(r); continue
            gt = gt[0]
            m1 = B.m.im.scan(so, limit=20)
            r['old_on_N1'] = len(m1)
            r['old_first_correct'] = bool(m1) and m1[0] == gt
            r['old_unique_correct'] = (m1[0] == gt) if len(m1) == 1 else None
            aN = mN[0]
            fN = A.m.fm.func_of(aN); fGT = B.m.fm.func_of(gt)
            r['midfunc'] = aN != fN
            an = A.a.anchors(fN)
            res = {}
            if 'ustring' in an: res['ustring'] = B.a.resolve(an, 'ustring')
            if 'caller' in an: res['caller'] = B.a.resolve(an, 'caller')
            elif 'caller_vt' in an: res['caller'] = B.a.resolve(an, 'caller_vt')
            sc = slot_class(A, fN, k)
            if sc:
                mp, la, lb = vt_align(A, B, sc[0])
                j = mp.get(sc[1]) if mp else None
                res['vtable'] = B.vt(sc[0])[j[0]] if j else None
                r['vt'] = (sc[0], sc[1], j, la, lb)
            h = A.m._fp.get(fN, {}).get('norm')
            if h and len(A.norm_index().get(h, [])) == 1:
                c = B.norm_index().get(h, [])
                res['hash'] = c[0] if len(c) == 1 else None
            M = GM[0]
            if fN in M: res['global'] = M[fN]
            r['anchors'] = {t: ('OK' if v == fGT else ('none' if v is None else 'WRONG')) for t, v in res.items()}
            r['gt_func_identical'] = A.m._fp[fN]['norm'] == B.m._fp.get(fGT, {}).get('norm')
            rows.append(r)
    for k, e in old.items():
        o, n = loff(e), (loff(new[k]) if k in new else None)
        if o is None or k in NONVT: continue
        c = cls_of(k)
        va, vb = A.vt(c), B.vt(c)
        if va is None or vb is None or o >= len(va): continue
        mp, la, lb = vt_align(A, B, c)
        pred = mp.get(o)
        # second anchor: global matcher identity of old slot function -> its index in new vtable
        fo = va[o]; M = GM[0]
        g = None
        if isinstance(fo, int) and fo in M and M[fo] in vb:
            idx = [i for i, x in enumerate(vb) if x == M[fo]]; g = idx[0] if len(idx) == 1 else ('amb', idx)
        orows.append(dict(pair=f'{N}->{N1}', name=k, cls=c, old=o, fixed=n, changed=o != n, vt_len=(la, lb),
                          size_flag=la != lb, pred=pred, glob=g))
    return rows, orows

if __name__ == '__main__':
    allr, allo = [], []
    for N, N1, gN, gN1 in PAIRS:
        A, B = ctx(N), ctx(N1)
        gm = f'{G}/cache/glob_{N}_{N1}.pkl'
        if os.path.exists(gm): GM = pickle.load(open(gm, 'rb'))
        else:
            GM = match.match(A.m, B.m); pickle.dump(GM, open(gm, 'wb'))
        r, o = run_pair(N, N1, gN, gN1, GM)
        allr += r; allo += o
        for x in r: print('SIG', x)
        for x in o: print('OFF', x)
        sys.stdout.flush()
    pickle.dump((allr, allo), open(f'{G}/cache/backtest.pkl', 'wb'))
