"""Classify historical CSSharp signature changes by disassembling old vs new sig bytes."""
import sys, os, json, glob, re, difflib, collections, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from builds import S
import capstone
md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
HEX = re.compile(r'0x[0-9a-f]+')
REG = re.compile(r'\b(r[a-z0-9]+|e[a-z]{2}|[a-d][lhx]|[sd]il|[sb]pl|[xy]mm\d+)\b')


def tok(sig):
    sig = sig.strip()
    if '\\x' in sig:
        return [None if t.upper() == '2A' else int(t, 16) for t in sig.split('\\x') if t]
    return [None if t in ('?', '??') else int(t, 16) for t in sig.split()]


def dis(sig):
    b = bytes(0 if x is None else x for x in tok(sig))
    out = []
    for a, s, m, o in md.disasm_lite(b, 0):
        if a + s > len(b):
            break
        out.append((m, o))
    # drop trailing truncated / int3 padding
    return out


def classify(old, new):
    A, B = dis(old), dis(new)
    k = min(len(A), len(B))
    if k == 0:
        return 'undecodable', A, B
    if A[:k] == B[:k]:
        if len(B) > len(A):
            return 'same-code, sig lengthened (old became ambiguous/rewrite for uniqueness)', A, B
        if len(B) < len(A):
            return 'same-code prefix, sig shortened (old tail broke)', A, B
        return 'same decoded code (only wildcard placement changed)', A, B
    ma, mb = [m for m, _ in A], [m for m, _ in B]
    ra = [m + ' ' + REG.sub('R', o) for m, o in A]
    rb = [m + ' ' + REG.sub('R', o) for m, o in B]
    ia = [m + ' ' + HEX.sub('I', o) for m, o in A]
    ib = [m + ' ' + HEX.sub('I', o) for m, o in B]
    if ia[:k] == ib[:k]:
        return 'immediate/displacement change (stack frame size, struct offset)', A, B
    if ra[:k] == rb[:k]:
        return 'register allocation change', A, B
    if ma[:k] == mb[:k]:
        return 'register alloc + immediates', A, B
    r = difflib.SequenceMatcher(None, ma, mb).ratio()
    if r >= 0.5:
        return 'instruction insert/remove/reorder (local code change)', A, B
    return 'different prologue (rewritten / inlined / other function)', A, B


if __name__ == '__main__':
    files = sorted(glob.glob(S + '/out/gd/*.json'))
    seq = collections.defaultdict(list)  # (name, plat) -> [(date, commit, sig)]
    for fn in files:
        date, commit = os.path.basename(fn)[:-5].split('_')
        try:
            d = json.load(open(fn))
        except Exception:
            continue
        for k, v in d.items():
            s = v.get('signatures') or {}
            for plat in ('linux', 'windows'):
                if s.get(plat):
                    seq[(k, plat)].append((date, commit, s[plat]))
    trans = []
    for (k, plat), l in seq.items():
        l.sort()
        for (d0, c0, s0), (d1, c1, s1) in zip(l, l[1:]):
            if s0 != s1:
                cls, A, B = classify(s0, s1)
                trans.append(dict(name=k, plat=plat, date=d1, commit=c1, cls=cls, old=s0, new=s1,
                                  oldd=[f'{m} {o}' for m, o in A][:8], newd=[f'{m} {o}' for m, o in B][:8]))
    pickle.dump(trans, open(S + '/cache/sighist.pkl', 'wb'))
    for plat in ('linux', 'windows'):
        t = [x for x in trans if x['plat'] == plat]
        print(f'== {plat}: {len(t)} signature changes across {len(set(x["commit"] for x in t))} commits')
        for c, n in collections.Counter(x['cls'] for x in t).most_common():
            print(f'  {n:4d}  {c}')
    # recent (2025-07 onward) linux
    t = [x for x in trans if x['plat'] == 'linux' and x['date'] >= '2025-07-01']
    print('== linux since 2025-07:', len(t))
    for c, n in collections.Counter(x['cls'] for x in t).most_common():
        print(f'  {n:4d}  {c}')
    print('\nexamples:')
    shown = set()
    for x in sorted(trans, key=lambda x: x['date'], reverse=True):
        if x['plat'] != 'linux' or x['cls'] in shown:
            continue
        shown.add(x['cls'])
        print(f"-- {x['name']} {x['date']} {x['commit']}: {x['cls']}\n   old: {x['oldd'][:7]}\n   new: {x['newd'][:7]}")
