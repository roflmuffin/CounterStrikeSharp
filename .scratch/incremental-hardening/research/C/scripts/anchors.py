"""Anchor coverage + anchor-based re-derivation of signature targets across builds."""
import sys, os, json, collections, bisect, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from fp import Mod
from builds import *
from multiscan import load_jsonc, sigs as jsigs
from sigscan import load_sigs


class Anch:
    def __init__(self, m):
        self.m = m
        im = m.im
        st = im.strings
        # string -> set(functions referencing it)
        self.str_funcs = collections.defaultdict(set)
        self.func_strs = collections.defaultdict(list)
        for a, t, mn in m.fm.refs:
            if t in st:
                f = m.fm.func_of(a)
                self.str_funcs[st[t]].add(f)
                self.func_strs[f].append(st[t])
        # vtable slots
        self.slots = collections.defaultdict(list)
        for ti, vts in im.vtables.items():
            for vaddr, ott, fns in vts:
                if ott != 0:
                    continue
                for i, f in enumerate(fns):
                    if isinstance(f, int):
                        self.slots[f].append((im.rtti[ti]['name'], i))
        self.callers = collections.defaultdict(list)
        for a, t in m.fm.calls:
            self.callers[t].append(a)
        self.datarefs = collections.defaultdict(list)
        for o, v in im.rel.items():
            if im.in_text(v):
                self.datarefs[v].append(o)

    def uniq_strings(self, f):
        return sorted(set(s for s in self.func_strs.get(f, []) if self.str_funcs[s] == {f} and len(s) >= 4))

    def anchors(self, f):
        a = {}
        us = self.uniq_strings(f)
        if us:
            a['ustring'] = us[0]
        # vtable slot where class unique for this function
        sl = self.slots.get(f, [])
        if sl:
            a['vtable'] = sl[0]
        # caller with unique string, call ordinal within caller
        for c in self.callers.get(f, []):
            cf = self.m.fm.func_of(c)
            cus = self.uniq_strings(cf)
            if cus:
                calls = self.m.calls_in(cf)
                k = [i for i, (x) in enumerate(sorted(ca for ca, _ in self.m.calls_sorted[bisect.bisect_left(self.m.call_addrs, cf):bisect.bisect_left(self.m.call_addrs, self.m.end(cf))])) if x == c]
                a['caller'] = (cus[0], k[0] if k else None)
                break
        # caller vtable slot
        if 'caller' not in a:
            for c in self.callers.get(f, []):
                cf = self.m.fm.func_of(c)
                if self.slots.get(cf):
                    ca = sorted(x for x, _ in self.m.calls_sorted[bisect.bisect_left(self.m.call_addrs, cf):bisect.bisect_left(self.m.call_addrs, self.m.end(cf))])
                    a['caller_vt'] = (self.slots[cf][0], ca.index(c))
                    break
        a['ncallers'] = len(self.callers.get(f, []))
        a['ndatarefs'] = len(self.datarefs.get(f, []))
        a['nstr'] = len(self.func_strs.get(f, []))
        return a

    def resolve(self, a, kind):
        m = self.m
        if kind == 'ustring':
            fs = self.str_funcs.get(a['ustring'], set())
            return list(fs)[0] if len(fs) == 1 else None
        if kind == 'vtable':
            c, i = a['vtable']
            p = m.im.primary_vtable(c)
            return p[2][i] if p and i < len(p[2]) else None
        if kind in ('caller', 'caller_vt'):
            if kind == 'caller':
                s, k = a['caller']
                fs = self.str_funcs.get(s, set())
                if len(fs) != 1:
                    return None
                cf = list(fs)[0]
            else:
                (c, i), k = a['caller_vt']
                p = m.im.primary_vtable(c)
                if not p or i >= len(p[2]):
                    return None
                cf = p[2][i];
            ca = sorted(self.m.calls_sorted[bisect.bisect_left(m.call_addrs, cf):bisect.bisect_left(m.call_addrs, m.end(cf))])
            return ca[k][1] if k is not None and k < len(ca) else None
        return None


def targets():
    t = {}
    for k, (lib, s) in load_sigs('/home/markus/dev/other/CounterStrikeSharp/configs/addons/counterstrikesharp/gamedata/gamedata.json').items():
        t['cssharp:' + k] = (lib, s)
    for f, tag in ((S + '/dl/gh/cs2fixes.jsonc', 'cs2fixes'), (S + '/dl/gh/swiftly_sigs.jsonc', 'swiftly')):
        for k, (lib, s) in jsigs(load_jsonc(f)).items():
            if lib == 'server':
                t[f'{tag}:{k}'] = (lib, s)
    return t


if __name__ == '__main__':
    old, new = sys.argv[1], sys.argv[2]
    A, B = Mod(old, 'server'), Mod(new, 'server')
    AA, BB = Anch(A), Anch(B)
    T = targets()
    cov = collections.Counter(); heal = collections.Counter(); rows = []
    for name, (lib, sig) in sorted(T.items()):
        ma, mb = A.im.scan(sig, limit=3), B.im.scan(sig, limit=3)
        if len(ma) != 1 or len(mb) != 1:
            continue
        fa, fb = ma[0], mb[0]
        isstart = fa in set([fa]) and A.fm.func_of(fa) == fa
        an = AA.anchors(fa)
        cov['n'] += 1
        cov['sig_at_func_start'] += isstart
        healed_by = []
        for kind in ('ustring', 'vtable', 'caller', 'caller_vt'):
            if kind in an:
                cov[kind] += 1
                r = BB.resolve(an, kind)
                if r == fb:
                    heal[kind] += 1; healed_by.append(kind)
                else:
                    heal[kind + '_wrong_or_none'] += 1
        if any(k in an for k in ('ustring', 'vtable', 'caller', 'caller_vt')):
            cov['any_anchor'] += 1
        if healed_by:
            heal['any'] += 1
        rows.append((name, hex(fa), hex(fb), {k: v for k, v in an.items()}, healed_by))
    for r in rows:
        print(r[0], r[1], '->', r[2], r[3], 'healed:', r[4])
    print('coverage:', dict(cov))
    print('heal:', dict(heal))
    pickle.dump(rows, open(f'{S}/cache/anchors_{old}_{new}.pkl', 'wb'))
