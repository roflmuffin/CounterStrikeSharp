import sys; sys.path.insert(0, sys.argv[1])
from fp import Mod
m=Mod('1.42.0.0','server'); im=m.im; st=im.strings
inv={}
for a,s in st.items(): inv.setdefault(s,[]).append(a)
refs=sorted(m.fm.refs)
for name in ['AddOutput','KillHierarchy']:
    for sa in inv.get(name,[]):
        for i,(a,t,mn) in enumerate(refs):
            if t==sa:
                ctx=refs[max(0,i-3):i+5]
                print(name, hex(a), 'func', hex(m.fm.func_of(a)), [(hex(x), m2, ('S:'+st[y][:24]) if y in st else ('CODE' if im.in_text(y) else hex(y))) for x,y,m2 in ctx])
