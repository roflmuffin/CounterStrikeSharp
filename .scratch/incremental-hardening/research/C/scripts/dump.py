import sys,struct; sys.path.insert(0, sys.argv[1])
from elfimg import Img; from builds import *
im=Img(path(sys.argv[2],sys.argv[3]))
st=im.strings
def desc(v):
    if v in st: return 'STR '+repr(st[v][:50])
    if v in im.rtti: return 'TI '+im.rtti[v]['name']
    if im.in_text(v): return 'CODE'
    return ''
for a0 in sys.argv[4:]:
    a=int(a0,16); print('---',a0)
    for p in range(a-0x30, a+0x50, 8):
        r = 'R' if p in im.rel else ('S:'+im.relsym[p][0] if p in im.relsym else ' ')
        v=im.q(p); print(f'{p:#x} {r:2s} {v:#018x} {desc(v)}')
