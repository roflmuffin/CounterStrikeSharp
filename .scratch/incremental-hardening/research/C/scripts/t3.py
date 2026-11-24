import sys,difflib; sys.path.insert(0, sys.argv[1])
from fp import Mod
A,B=Mod('1.42.0.0','server'),Mod('14201','server')
va=A.im.primary_vtable('CSource2Server')[2]; vb=B.im.primary_vtable('CSource2Server')[2]
for i in range(10,18):
    fa=va[i] if i<len(va) else None; fb=vb[i]
    print(i, hex(fa) if fa else None, A.strrefs(fa)[:2] if fa else '', '|', hex(fb), B.strrefs(fb)[:2], B.fp(fb)['ninsn'])
print('new slot 13/14 disasm:')
for i in (13,14):
  for a,s,m,o in B.insns(vb[i])[:12]: print('  ',i,hex(a),m,o)
# CheckTransmit diff
fa=A.im.primary_vtable('CSource2GameEntities')[2][13]; fb=B.im.primary_vtable('CSource2GameEntities')[2][13]
ta=A.norm_tokens(fa); tb=B.norm_tokens(fb)
ia=[m+' '+o for _,_,m,o in A.insns(fa)]; ib=[m+' '+o for _,_,m,o in B.insns(fb)]
import re
sa=[re.sub(r'0x[0-9a-f]{5,}','X',x) for x in ia]; sb=[re.sub(r'0x[0-9a-f]{5,}','X',x) for x in ib]
print('CheckTransmit ninsn',len(ia),len(ib))
for l in list(difflib.unified_diff(sa,sb,lineterm='',n=1))[:60]: print(l)
