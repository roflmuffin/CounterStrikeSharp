import sys; sys.path.insert(0, sys.argv[1])
from fp import Mod
A=Mod('1.42.0.0','server'); im=A.im; st=im.strings
inv={}
for a,s in st.items(): inv.setdefault(s,[]).append(a)
refs=sorted(A.fm.refs)
for name in ['noclip','kill','mp_switchteams','bot_add_t','mp_restartgame']:
    for sa in inv.get(name,[]):
        for i,(a,t,m) in enumerate(refs):
            if t==sa:
                ctx=refs[max(0,i-4):i+6]
                print(name, hex(a), 'func', hex(A.fm.func_of(a)), [(hex(x), m2, ('S:'+st[y][:30]) if y in st else ('CODE' if im.in_text(y) else hex(y))) for x,y,m2 in ctx])
