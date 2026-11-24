import sys, time; sys.path.insert(0, sys.argv[1])
from elfimg import Img; from builds import *
t=time.time(); im=Img(path('1.42.0.0','server')); print('load',time.time()-t)
print('funcs(eh_frame)',len(im.funcs)); t=time.time()
r=im.rtti; print('typeinfos',len(r), {k:sum(1 for v in r.values() if v['kind']==k) for k in ('class','si','vmi')}, time.time()-t)
vt=im.vtables; print('classes with vtables',len(vt),'total vtables',sum(len(v) for v in vt.values()))
for c in ['CCSPlayerPawn','CCSPlayerController','CBaseEntity','CCSGameRules','CBasePlayerPawn','CCSPlayer_ItemServices','CCSPlayer_WeaponServices','CBaseTrigger','CBaseModelEntity','CGameSceneNode']:
    p=im.primary_vtable(c); print(c, hex(p[0]) if p else None, len(p[2]) if p else None, 'secondary:',[ (hex(a),o,len(f)) for a,o,f in vt.get(im.ti_by_name.get(f'{len(c)}{c}'),[]) if o!=0][:3])
print('strings',len(im.strings))
