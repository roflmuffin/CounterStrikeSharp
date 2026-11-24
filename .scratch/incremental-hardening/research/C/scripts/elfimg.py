"""Minimal ELF image helper for CS2 Linux modules: relocations, sections, RTTI, vtables, sig scan."""
import re, struct, bisect, functools
from elftools.elf.elffile import ELFFile
from elftools.elf.relocation import RelocationSection
from elftools.elf.sections import SymbolTableSection


class Img:
    def __init__(self, path):
        self.path = path
        self.f = open(path, 'rb')
        self.data = self.f.read()
        self.elf = ELFFile(open(path, 'rb'))
        self.secs = {}
        for s in self.elf.iter_sections():
            if s['sh_addr']:
                self.secs[s.name] = (s['sh_addr'], s['sh_size'], s['sh_offset'], s['sh_type'])
        self.segs = [(p['p_vaddr'], p['p_filesz'], p['p_offset']) for p in self.elf.iter_segments() if p['p_type'] == 'PT_LOAD']
        self.syms = {}      # name -> addr
        self.symidx = {}
        for s in self.elf.iter_sections():
            if isinstance(s, SymbolTableSection):
                for i, sym in enumerate(s.iter_symbols()):
                    self.symidx[(s.name, i)] = sym
                    if sym['st_value']:
                        self.syms[sym.name] = sym['st_value']
        self.rel = {}       # addr -> value (resolved pointer, internal only)
        self.relsym = {}    # addr -> (symname, addend)
        for s in self.elf.iter_sections():
            if isinstance(s, RelocationSection):
                symtab = self.elf.get_section(s['sh_link'])
                for r in s.iter_relocations():
                    t = r['r_info_type']
                    off = r['r_offset']
                    if t == 8:  # RELATIVE
                        self.rel[off] = r['r_addend']
                    elif t in (1, 6, 7):  # 64, GLOB_DAT, JUMP_SLOT
                        sym = symtab.get_symbol(r['r_info_sym'])
                        add = r['r_addend'] if t == 1 else 0
                        self.relsym[off] = (sym.name, add)
                        if sym['st_value']:
                            self.rel[off] = sym['st_value'] + add
        t = self.secs['.text']
        self.text_lo, self.text_hi = t[0], t[0] + t[1]

    def off(self, va):
        for v, sz, o in self.segs:
            if v <= va < v + sz:
                return o + va - v
        return None

    def read(self, va, n):
        o = self.off(va)
        return self.data[o:o + n] if o is not None else None

    def q(self, va):
        if va in self.rel:
            return self.rel[va]
        b = self.read(va, 8)
        return struct.unpack('<Q', b)[0] if b else None

    def u32(self, va):
        return struct.unpack('<I', self.read(va, 4))[0]

    def i32(self, va):
        return struct.unpack('<i', self.read(va, 4))[0]

    def cstr(self, va, maxlen=512):
        o = self.off(va)
        if o is None:
            return None
        e = self.data.find(b'\0', o, o + maxlen)
        if e < 0:
            return None
        try:
            return self.data[o:e].decode('utf-8')
        except UnicodeDecodeError:
            return None

    def sec(self, name):
        a, sz, o, _ = self.secs[name]
        return a, self.data[o:o + sz]

    def in_text(self, va):
        return va is not None and self.text_lo <= va < self.text_hi

    # ---------- signature scanning ----------
    @staticmethod
    def sig_to_regex(sig):
        sig = sig.strip()
        if '\\x' in sig:
            toks = [t for t in sig.split('\\x') if t]
        else:
            toks = sig.split()
        out = b''
        hexfmt = '\\x' in sig
        for t in toks:
            if t in ('?', '??') or (hexfmt and t.upper() == '2A'):
                out += b'.'
            else:
                out += re.escape(bytes([int(t, 16)]))
        return re.compile(out, re.S), len(toks)

    def scan(self, sig, sec='.text', limit=50):
        rx, n = self.sig_to_regex(sig)
        a, buf = self.sec(sec)
        res = []
        # overlapping search
        pos = 0
        while True:
            m = rx.search(buf, pos)
            if not m:
                break
            res.append(a + m.start())
            if len(res) >= limit:
                break
            pos = m.start() + 1
        return res

    # ---------- function boundaries from .eh_frame ----------
    @functools.cached_property
    def funcs(self):
        """sorted list of (start, size) from .eh_frame FDEs (via .eh_frame_hdr table)."""
        a, hdr = self.sec('.eh_frame_hdr')
        ver, eh_ptr_enc, fde_count_enc, table_enc = hdr[0], hdr[1], hdr[2], hdr[3]
        assert fde_count_enc == 0x03 and table_enc == 0x3b, (fde_count_enc, table_enc)
        p = 4
        p += 4  # eh_frame_ptr (sdata4 pcrel) assume
        cnt = struct.unpack_from('<I', hdr, p)[0]; p += 4
        out = []
        for i in range(cnt):
            loc, fde = struct.unpack_from('<ii', hdr, p + i * 8)
            start = a + loc
            fde_va = a + fde
            # parse FDE: length(4) cie_ptr(4) pc_begin(enc, assume pcrel sdata4) pc_range(sdata4/udata4)
            ln = self.u32(fde_va)
            pcb = self.i32(fde_va + 8) + fde_va + 8
            rng = self.u32(fde_va + 12)
            out.append((pcb, rng))
        out.sort()
        return out

    @functools.cached_property
    def func_starts(self):
        return [s for s, _ in self.funcs]

    def func_of(self, va):
        i = bisect.bisect_right(self.func_starts, va) - 1
        if i >= 0:
            s, n = self.funcs[i]
            if s <= va < s + n:
                return s, n
        return None

    # ---------- strings ----------
    @functools.cached_property
    def strings(self):
        """addr->str for NUL-terminated printable strings in rodata string sections."""
        out = {}
        for name in ('.rodata', '.rodata.str1.1', '.rodata.str1.8'):
            if name not in self.secs:
                continue
            a, buf = self.sec(name)
            for m in re.finditer(rb'[\x20-\x7e\t\n]{3,}\x00', buf):
                out[a + m.start()] = m.group()[:-1].decode()
        return out

    # ---------- RTTI ----------
    @functools.cached_property
    def rtti(self):
        """returns dict typeinfo_addr -> dict(name, kind, bases=[(ti, offset)])"""
        kinds = {
            '_ZTVN10__cxxabiv117__class_type_infoE': 'class',
            '_ZTVN10__cxxabiv120__si_class_type_infoE': 'si',
            '_ZTVN10__cxxabiv121__vmi_class_type_infoE': 'vmi',
        }
        kaddr = {}
        for k, v in kinds.items():
            if k in self.syms:
                kaddr[self.syms[k] + 16] = v
        tis = {}
        for off, val in self.rel.items():
            k = kaddr.get(val)
            if k:
                nameptr = self.q(off + 8)
                name = self.cstr(nameptr) if nameptr else None
                if not name:
                    continue
                bases = []
                if k == 'si':
                    bases.append((self.q(off + 16), 0))
                elif k == 'vmi':
                    cnt = self.u32(off + 20)
                    if cnt > 64:
                        continue
                    for i in range(cnt):
                        b = self.q(off + 24 + 16 * i)
                        of = struct.unpack('<q', self.read(off + 32 + 16 * i, 8))[0]
                        bases.append((b, of >> 8))
                tis[off] = dict(name=name, kind=k, bases=bases)
        return tis

    @functools.cached_property
    def ti_by_name(self):
        return {v['name']: k for k, v in self.rtti.items()}

    @functools.cached_property
    def vtables(self):
        """typeinfo -> list of (vtable_addr_point, offset_to_top, [funcs])  (scans data.rel.ro)"""
        tis = self.rtti
        res = {}
        a, sz = self.secs['.data.rel.ro'][0], self.secs['.data.rel.ro'][1]
        cand = sorted(o for o, v in self.rel.items() if v in tis and a <= o < a + sz)
        for o in cand:
            ti = self.rel[o]
            # offset-to-top at o-8 must not be relocated, and be 0 or negative multiple of 8
            if (o - 8) in self.rel:
                continue
            ott = struct.unpack('<q', self.read(o - 8, 8))[0]
            if ott > 0 or ott < -0x100000 or ott % 8:
                continue
            fns = []
            p = o + 8
            while True:
                if p in self.rel:
                    v = self.rel[p]
                    if self.in_text(v):
                        fns.append(v); p += 8; continue
                    break
                elif p in self.relsym:
                    nm = self.relsym[p][0]
                    if nm in ('__cxa_pure_virtual', '__cxa_deleted_virtual'):
                        fns.append(nm); p += 8; continue
                    break
                else:
                    break
            res.setdefault(ti, []).append((o + 8, ott, fns))
        return res

    def primary_vtable(self, cls):
        """cls given as mangled typeinfo-name like '13CCSPlayerPawn' or plain 'CCSPlayerPawn'"""
        ti = self.ti_by_name.get(cls) or self.ti_by_name.get(f'{len(cls)}{cls}')
        if ti is None:
            return None
        for vt in self.vtables.get(ti, []):
            if vt[1] == 0:
                return vt
        return None
