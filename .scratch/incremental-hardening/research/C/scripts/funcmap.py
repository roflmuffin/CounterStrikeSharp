"""Linear-sweep disassembly of .text -> function starts, call edges, rip-relative refs. Cached as pickle."""
import sys, os, re, pickle, time, bisect
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from elfimg import Img
import capstone

RIP = re.compile(r'\[rip ([+-]) (0x[0-9a-f]+|\d+)\]')


def build(im, cache):
    if os.path.exists(cache):
        return pickle.load(open(cache, 'rb'))
    md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
    md.skipdata = True
    a, buf = im.sec('.text')
    calls = []   # (insn_addr, target)
    jmps = []    # tail jumps (E9) (insn_addr, target)
    refs = []    # (insn_addr, target, mnemonic)
    t = time.time()
    n = 0
    for addr, size, mnem, ops in md.disasm_lite(buf, a):
        n += 1
        if mnem == 'call':
            if ops.startswith('0x'):
                calls.append((addr, int(ops, 16)))
            elif 'rip' in ops:
                m = RIP.search(ops)
                if m:
                    d = int(m.group(2), 0)
                    refs.append((addr, addr + size + (d if m.group(1) == '+' else -d), 'callm'))
        elif mnem == 'jmp' and ops.startswith('0x'):
            jmps.append((addr, int(ops, 16)))
        elif 'rip' in ops:
            m = RIP.search(ops)
            if m:
                d = int(m.group(2), 0)
                refs.append((addr, addr + size + (d if m.group(1) == '+' else -d), mnem))
    starts = set(s for s, _ in im.funcs)
    starts.update(tgt for _, tgt in calls if im.in_text(tgt))
    for o, v in im.rel.items():
        if im.in_text(v):
            starts.add(v)
    # lea of code address (function pointers / callbacks)
    starts.update(tgt for _, tgt, m in refs if m == 'lea' and im.in_text(tgt))
    res = dict(ninsn=n, calls=calls, jmps=jmps, refs=refs, starts=sorted(starts), secs=time.time() - t)
    pickle.dump(res, open(cache, 'wb'))
    return res


class FM:
    def __init__(self, im, cache):
        self.im = im
        d = build(im, cache)
        self.__dict__.update(d)
        self.ss = self.starts

    def func_of(self, va):
        i = bisect.bisect_right(self.ss, va) - 1
        return self.ss[i] if i >= 0 else None

    def func_end(self, start):
        i = bisect.bisect_right(self.ss, start)
        return self.ss[i] if i < len(self.ss) else self.im.text_hi


if __name__ == '__main__':
    im = Img(sys.argv[1])
    r = build(im, sys.argv[2])
    print(r['ninsn'], len(r['calls']), len(r['refs']), len(r['starts']), r['secs'])
