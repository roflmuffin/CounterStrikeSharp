"""Function fingerprinting & cross-build matching."""
import sys, os, re, hashlib, pickle, bisect, collections
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from elfimg import Img
from funcmap import FM
from builds import *
import capstone

md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
md.skipdata = True
RIP = re.compile(r'\[rip ([+-]) (0x[0-9a-f]+|\d+)\]')
HEX = re.compile(r'0x[0-9a-f]+')
REG = re.compile(r'\b(r[a-z0-9]+|e[a-z]{2}|[a-d][lhx]|[sd]il|[sb]pl|[xy]mm\d+)\b')


class Mod:
    def __init__(self, build, lib):
        self.build, self.lib = build, lib
        self.im = Img(path(build, lib))
        self.fm = FM(self.im, f'{S}/cache/fm_{lib}_{build}.pkl')
        self._fp = {}
        # refs by function
        self.refs_by_insn = {}
        for a, t, m in self.fm.refs:
            self.refs_by_insn[a] = t
        self.calls_sorted = sorted(self.fm.calls)
        self.refs_sorted = sorted(self.fm.refs)
        self.call_addrs = [c[0] for c in self.calls_sorted]
        self.ref_addrs = [r[0] for r in self.refs_sorted]

    def end(self, f):
        return self.fm.func_end(f)

    def body(self, f):
        e = min(self.end(f), f + 0x20000)
        return self.im.read(f, e - f)

    def calls_in(self, f):
        e = self.end(f)
        i, j = bisect.bisect_left(self.call_addrs, f), bisect.bisect_left(self.call_addrs, e)
        return [t for _, t in self.calls_sorted[i:j]]

    def refs_in(self, f):
        e = self.end(f)
        i, j = bisect.bisect_left(self.ref_addrs, f), bisect.bisect_left(self.ref_addrs, e)
        return [(a, t, m) for a, t, m in self.refs_sorted[i:j]]

    def strrefs(self, f):
        out = []
        st = self.im.strings
        for a, t, m in self.refs_in(f):
            if t in st:
                out.append(st[t])
        return out

    def insns(self, f):
        b = self.body(f)
        return list(md.disasm_lite(b, f))

    def fp(self, f):
        """returns dict(raw, norm, mnem, size, ninsn)"""
        if f in self._fp:
            return self._fp[f]
        b = self.body(f)
        # strip trailing padding (nop/int3)
        ins = list(md.disasm_lite(b, f))
        while ins and (ins[-1][2] in ('nop', 'int3') or ins[-1][2].startswith('nop')):
            ins.pop()
        st = self.im.strings
        norm, mn = [], []
        for addr, size, m, ops in ins:
            mn.append(m)
            o = ops
            r = RIP.search(o)
            if r:
                d = int(r.group(2), 0)
                t = addr + size + (d if r.group(1) == '+' else -d)
                tag = 'S:' + st[t][:40] if t in st else 'M'
                o = RIP.sub(lambda _m: '[' + tag + ']', o)
            elif m in ('call', 'jmp') or m.startswith('j'):
                if o.startswith('0x'):
                    t = int(o, 16)
                    o = 'L' if f <= t < f + len(b) else 'F'
            o = HEX.sub(lambda x: x.group() if int(x.group(), 16) < 0x100000 else 'X', o)
            norm.append(m + ' ' + o)
        size = (ins[-1][0] + ins[-1][1] - f) if ins else 0
        raw = hashlib.sha1(self.im.read(f, size) or b'').hexdigest()
        r = dict(raw=raw, norm=hashlib.sha1('\n'.join(norm).encode()).hexdigest(),
                 mnem=hashlib.sha1(' '.join(mn).encode()).hexdigest(), size=size, ninsn=len(ins),
                 normlist=None)
        self._fp[f] = r
        return r

    def norm_tokens(self, f):
        """normalized instruction list (registers abstracted too) for fuzzy matching"""
        b = self.body(f)
        out = []
        for addr, size, m, ops in md.disasm_lite(b, f):
            o = RIP.sub('[M]', ops)
            o = HEX.sub('I', o)
            o = REG.sub('R', o)
            out.append(m + ' ' + o)
        return out
