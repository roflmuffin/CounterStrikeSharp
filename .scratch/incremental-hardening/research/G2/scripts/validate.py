import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from g2img import ImgFlat, elf_as_flat
import elfimg
real = sys.argv[1]; ida = sys.argv[2]
fr = elf_as_flat(real); fi = open(ida, 'rb').read()
A = ImgFlat('real', fr); B = ImgFlat('ida', fi); E = elfimg.Img(real)
reltargets = set()
for o in list(A.rel) + list(A.relsym):
    reltargets.update(range(o, o + 8))
n = min(len(fr), len(fi)); diff = 0; diff_nonrel = 0; ex = []
import itertools
# compare in chunks
CH = 1 << 16
for s in range(0, n, CH):
    a, b = fr[s:s + CH], fi[s:s + CH]
    if a != b:
        for i in range(len(a)):
            if a[i] != b[i]:
                diff += 1
                if s + i not in reltargets:
                    diff_nonrel += 1
                    if len(ex) < 10: ex.append(hex(s + i))
print('len real/ida', hex(len(fr)), hex(len(fi)), 'diff bytes', diff, 'outside reloc targets', diff_nonrel, ex)
print('rel equal', A.rel == B.rel, len(A.rel), 'relsym equal', A.relsym == B.relsym, 'syms equal', A.syms == B.syms)
print('real-ELF Img: rel', len(E.rel), 'rtti', len(E.rtti), 'vt', len(E.vtables), 'funcs', len(E.funcs))
for nm, X in (('flat-real', A), ('flat-ida', B)):
    print(nm, 'rtti', len(X.rtti), 'vt', len(X.vtables), 'funcs', len(X.funcs), 'strings', len(X.strings))
vE = {E.rtti[t]['name']: [(o, len(f)) for o, _, f in v] for t, v in E.vtables.items()}
vB = {B.rtti[t]['name']: [(o, len(f)) for o, _, f in v] for t, v in B.vtables.items()}
print('vtables identical (ELF-sections Img vs IDA flat):', vE == vB, len(set(vE) ^ set(vB)))
print('funcs identical:', E.funcs == B.funcs)
sigs = ['55 48 8D 05 ? ? ? ? 48 89 E5 41 57 4D 89 CF 41 56 4D 89 C6 41 55 49 89 F5']
for s in sigs: print('scan ELF', E.scan(s), 'IDA', B.scan(s))
