import sys, re, collections
sys.path.insert(0, sys.argv[0].rsplit('/',1)[0])
from elfvt import vtab, v2o
import capstone
md=capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
RIP=re.compile(r'\[rip [+-] 0x[0-9a-f]+\]')
def fp(d,S,a):
    o=v2o(S,a); out=[]
    for ad,sz,m,ops in md.disasm_lite(d[o:o+400],a):
        ops=RIP.sub('[rip]',ops)
        if m=='call' or (m.startswith('j') and not ops.startswith('q')): ops='T'
        out.append(m+' '+ops)
        if m in('ret','jmp') or m.startswith('ud'): break
        if len(out)>60: break
    return tuple(out)
def main(p1,p2,cls):
    d1,S1,a=vtab(p1,cls); d2,S2,b=vtab(p2,cls)
    fa=[fp(d1,S1,x) for x in a]; fb=[fp(d2,S2,x) for x in b]
    ca=collections.Counter(fa)
    print(cls,len(a),len(b),'old slots w/ non-unique fp (incl ICF):',sum(1 for f in fa if ca[f]>1),' distinct-address-but-same-fp:',len({(f,x) for f,x in zip(fa,a) if ca[f]>1})-len({f for f in fa if ca[f]>1}))
    print('tiny (<=4 insns) slots old:',sum(1 for f in fa if len(f)<=4))
    for i in [int(x) for x in sys.argv[4].split(',')] if len(sys.argv)>4 else []:
        hits=[j for j,f in enumerate(fb) if f==fa[i]]
        print(' old slot',i,'ninsn',len(fa[i]),'matches in new at',hits, '| old-dups',[j for j,f in enumerate(fa) if f==fa[i] and j!=i])
main(sys.argv[1],sys.argv[2],sys.argv[3])
