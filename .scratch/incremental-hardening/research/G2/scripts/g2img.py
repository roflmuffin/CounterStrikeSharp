"""Img variant built from a flat VA image (extracted from IDA .i64) using only program headers + PT_DYNAMIC.
Also an ELF-file variant (ImgELFDyn) that derives sections the same way, for apples-to-apples validation."""
import sys, os, struct
C = '/tmp/claude-1000/-home-markus-dev-other-CounterStrikeSharp/69075f48-1e3b-474a-8b82-b03d2f8f4eda/scratchpad/C/scripts'
sys.path.insert(0, C)
import elfimg

class _Sym(dict):
    def __init__(self, name, value):
        super().__init__(st_value=value); self.name = name

class ImgFlat(elfimg.Img):
    def __init__(self, path, data=None):
        self.path = path
        self.data = data if data is not None else open(path, 'rb').read()
        d = self.data
        self.segs = [(0, len(d), 0)]
        phoff = struct.unpack_from('<Q', d, 0x20)[0]; phn = struct.unpack_from('<H', d, 0x38)[0]
        loads = []; dyn = relro = eh = None
        for i in range(phn):
            t, fl, off, va, pa, fsz, msz, al = struct.unpack_from('<IIQQQQQQ', d, phoff + 56 * i)
            if t == 1: loads.append((va, fsz, fl))
            elif t == 2: dyn = va
            elif t == 0x6474e550: eh = (va, msz)
            elif t == 0x6474e552: relro = (va, fsz)
        D = {}
        i = 0
        while True:
            tag, val = struct.unpack_from('<qQ', d, dyn + 16 * i); i += 1
            if tag == 0: break
            D.setdefault(tag, val)
        self.dyn = D
        X = [l for l in loads if l[2] & 1][0]
        R = [l for l in loads if l[2] == 4][0]
        self.secs = {'.text': (X[0], X[1], X[0], 1), '.rodata': (R[0], R[1], R[0], 1),
                     '.data.rel.ro': (relro[0], relro[1], relro[0], 1), '.eh_frame_hdr': (eh[0], eh[1], eh[0], 1)}
        # dynsym
        symtab, strtab = D[6], D[5]
        nsym = self._gnu_nsyms(D[0x6ffffef5])
        syms = []
        self.syms = {}; self.symidx = {}
        for k in range(nsym):
            st_name, st_info, st_other, st_shndx, st_value, st_size = struct.unpack_from('<IBBHQQ', d, symtab + 24 * k)
            e = d.find(b'\0', strtab + st_name)
            nm = d[strtab + st_name:e].decode('utf-8', 'replace')
            s = _Sym(nm, st_value if st_shndx != 0 else 0)
            syms.append(s)
            if s['st_value']: self.syms[nm] = s['st_value']
        self.rel = {}; self.relsym = {}
        def relas(addr, size):
            for p in range(addr, addr + size, 24):
                off, info, add = struct.unpack_from('<QQq', d, p)
                yield off, info & 0xffffffff, info >> 32, add
        tabs = []
        if 7 in D: tabs.append((D[7], D[8]))
        if 23 in D: tabs.append((D[23], D[2]))
        for a, sz in tabs:
            for off, t, si, add in relas(a, sz):
                if t == 8:
                    self.rel[off] = add
                elif t in (1, 6, 7):
                    sym = syms[si]
                    a2 = add if t == 1 else 0
                    self.relsym[off] = (sym.name, a2)
                    if sym['st_value']:
                        self.rel[off] = sym['st_value'] + a2
        assert 36 not in D, 'RELR unsupported'
        self.text_lo, self.text_hi = X[0], X[0] + X[1]

    def _gnu_nsyms(self, gh):
        d = self.data
        nb, symoff, bloom, shift = struct.unpack_from('<IIII', d, gh)
        b0 = gh + 16 + bloom * 8
        buckets = struct.unpack_from('<%dI' % nb, d, b0)
        mx = max(buckets)
        if mx < symoff: return symoff
        ch = b0 + nb * 4
        k = mx
        while True:
            h = struct.unpack_from('<I', d, ch + (k - symoff) * 4)[0]
            if h & 1: return k + 1
            k += 1

def elf_as_flat(path):
    """Map a real ELF file into a flat VA image (PT_LOAD filesz) so it can go through ImgFlat too."""
    d = open(path, 'rb').read()
    phoff = struct.unpack_from('<Q', d, 0x20)[0]; phn = struct.unpack_from('<H', d, 0x38)[0]
    L = []
    for i in range(phn):
        t, fl, off, va, pa, fsz, msz, al = struct.unpack_from('<IIQQQQQQ', d, phoff + 56 * i)
        if t == 1: L.append((off, va, fsz))
    img = bytearray(max(va + fsz for _, va, fsz in L))
    for off, va, fsz in L:
        img[va:va + fsz] = d[off:off + fsz]
    return bytes(img)
