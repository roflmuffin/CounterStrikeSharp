import sys,difflib,re; sys.path.insert(0, sys.argv[1])
from fp import Mod
A,B=Mod(sys.argv[2],'server'),Mod(sys.argv[3],'server')
cls,idx=sys.argv[4],int(sys.argv[5])
fa=A.im.primary_vtable(cls)[2][idx]; fb=B.im.primary_vtable(cls)[2][idx]
def norm(m,f):
    out=[]
    for a,s,mn,o in m.insns(f):
        r=re.search(r'\[rip ([+-]) (0x[0-9a-f]+)\]',o)
        if r:
            t=a+s+int(r.group(2),16)*(1 if r.group(1)=='+' else -1)
            o=o.replace(r.group(0),'[S:'+m.im.strings[t][:30]+']' if t in m.im.strings else '[G]')
        if mn=='call' or mn.startswith('j'): o=re.sub(r'0x[0-9a-f]+','T',o)
        out.append(mn+' '+o)
    return out
na,nb=norm(A,fa),norm(B,fb)
print(len(na),len(nb))
for l in list(difflib.unified_diff(na,nb,lineterm='',n=2))[:40]: print(l)
