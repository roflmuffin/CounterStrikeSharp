import sys,collections,pickle; sys.path.insert(0, sys.argv[1])
from fp import Mod
from anchors import Anch, targets
from match import all_fps
A=Mod('1.41.9.7','server'); B=Mod('14201','server')
m=Mod('1.42.0.0','server'); im=m.im; an=Anch(m)
nf=len([f for f in m.fm.starts if im.in_text(f)])
withs=sum(1 for f in m.fm.starts if an.func_strs.get(f))
uniq=sum(1 for f in m.fm.starts if an.uniq_strings(f))
print('funcs',nf,'with>=1 string ref',withs, '%.1f%%'%(100*withs/nf),'with unique string',uniq,'%.1f%%'%(100*uniq/nf))
inv=sum(1 for f in m.fm.starts if an.slots.get(f)); print('funcs in some primary vtable',inv,'%.1f%%'%(100*inv/nf))
# reachable: unique-string OR vtable OR called by such
anch=set(f for f in m.fm.starts if an.uniq_strings(f) or an.slots.get(f))
reach=set(anch)
for a,t in m.fm.calls:
    if m.fm.func_of(a) in anch: reach.add(t)
print('anchored (ustring|vtable)',len(anch),'%.1f%%'%(100*len(anch)/nf),'+1 call hop',len(reach),'%.1f%%'%(100*len(reach)/nf))
# RTTI example
st={v:k for k,v in im.strings.items()}
nm='13CCSPlayerPawn'; ti=im.ti_by_name[nm]; vt=im.primary_vtable('CCSPlayerPawn')
print('RTTI example: name str',hex([a for a,s in im.strings.items() if s==nm][0]),'typeinfo',hex(ti),im.rtti[ti]['kind'],'base',im.rtti.get(im.rtti[ti]['bases'][0][0],{}).get('name'),'vtable addrpoint',hex(vt[0]),'slots',len(vt[2]),'slot356(PostThink)',hex(vt[2][356]))
# unique whole-function norm hash anchor across builds for targets
fa,fb=all_fps(A),all_fps(B)
ca=collections.Counter(d['norm'] for d in fa.values()); cb=collections.Counter(d['norm'] for d in fb.values())
ok=n=0
for name,(lib,sig) in targets().items():
    x=A.im.scan(sig,limit=2); y=B.im.scan(sig,limit=2)
    if len(x)!=1 or len(y)!=1 or A.fm.func_of(x[0])!=x[0]: continue
    n+=1; h=fa[x[0]]['norm']
    if ca[h]==1 and cb[h]==1 and fb.get(y[0],{}).get('norm')==h: ok+=1
print('targets(func-start)',n,'healable by unique normalized whole-function hash',ok)
