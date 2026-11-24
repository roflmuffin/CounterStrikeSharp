import sys; sys.path.insert(0, sys.argv[1])
from elfimg import Img; from builds import *
im=Img(path('1.42.0.0','server')); st=im.strings
inv={}
for a,s in st.items(): inv.setdefault(s,[]).append(a)
dr={}
for o,v in im.rel.items(): dr.setdefault(v,[]).append(o)
for s in ['AddOutput','KillHierarchy','SetHealth','InputSetHealth','OnStartTouch','FireUser1','Use','player_connect']:
    for a in inv.get(s,[]):
        print(s, hex(a), 'datarefs', [hex(x) for x in dr.get(a,[])][:6])
