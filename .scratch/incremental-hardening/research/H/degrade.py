import json,ast,sys,datetime as dt
S=sys.argv[1]
src=open(S+'/G1/model.py').read()
I=ast.literal_eval(src[src.index('I=[')+2:src.index(']\nsc=')+1])
rows={r['h']:r for r in json.load(open(S+'/A/gamedata_history.json'))['rows']}
P=lambda s: dt.datetime.fromisoformat(s.replace('Z','+00:00'))
rel={}
for l in open(S+'/G1/fix_tags.txt'):
    p=l.split(); rel[p[0]]=P(p[3]) if len(p)>3 else P(p[1])
CORE={'Chat cmds','Game events','Net writes','Tick/timers'}
B2G={'UTIL_ClientPrintAll':'Chat/HUD out','ClientPrint':'Chat/HUD out',
 'CCSPlayerController_SwitchTeam':'Player mgmt','CCSPlayerController_ChangeTeam':'Player mgmt','CCSPlayerController_Respawn':'Player mgmt','CCSPlayerPawn_Respawn':'Player mgmt',
 'CBasePlayerController_SetPawn':'Player mgmt','CBasePlayerPawn_CommitSuicide':'Player mgmt','CBaseEntity_Teleport':'Player mgmt',
 'CCSPlayerPawnBase_PostThink':'PostThink hook',
 'CGameEventManager_Init':'Game events','LegacyGameEventListener':'Game events','GameEventManager':'Game events',
 'GiveNamedItem':'Weapons','CCSPlayer_ItemServices_GiveNamedItem':'Weapons','CCSPlayer_ItemServices_DropActivePlayerWeapon':'Weapons','CCSPlayer_ItemServices_RemoveWeapons':'Weapons',
 'CBasePlayerPawn_RemovePlayerItem':'Weapons','CCSPlayer_WeaponServices_CanUse':'Weapons','CCSPlayer_ItemServices_CanAcquire':'Weapons','GetCSWeaponDataFromKey':'Weapons',
 'UTIL_Remove':'Entity spawn/model','UTIL_CreateEntityByName':'Entity spawn/model','CBaseEntity_DispatchSpawn':'Entity spawn/model','CBaseModelEntity_SetModel':'Entity spawn/model',
 'CBaseEntity_IsPlayerPawn':'Entity spawn/model','CGameSceneNode_GetSkeletonInstance':'Entity spawn/model',
 'CCSGameRules_TerminateRound':'Round control','CCSGameRules_FindPickerEntity':'Round control',
 'CTakeDamageInfo_HitGroup':'Damage','CBaseEntity_TakeDamageOld':'Damage',
 'CBaseEntity_EmitSoundFilter':'Sounds',
 'CEntityInstance_AcceptInput':'Entity I/O','CEntitySystem_AddEntityIOEvent':'Entity I/O','CEntityIOOutput_FireOutputInternal':'Entity I/O',
 'CBaseTrigger_StartTouch':'Triggers','CBaseTrigger_EndTouch':'Triggers',
 'IGameSystem_InitAllSystems_pFirst':'Map lifecycle','CEntityResourceManifest_AddResource':'Map lifecycle',
 'CheckTransmitPlayerSlot':'Transmit','ISource2GameEntities::CheckTransmit':'Transmit','CheckTransmit':'Transmit',
 'NetworkStateChanged':'Net writes','SetStateChanged':'Net writes','StateChanged':'Net writes',
 'Host_Say':'Chat cmds','CCSNavArea_IsValidNavMesh':'Nav mesh'}
# extra groups per incident: (group, commit-it-is-fixed-by or None=first, kind) kind: 'iface' (V: data in T1, auto in Ideal), 'gsys' (G: code in T1, auto in Ideal),
# 'code' (needs code in Ideal too), 'schema' (renames: T1 degraded, Ideal aliased unless Ideal outcome S)
X={
 6:[('Player mgmt',None,'code'),('Map lifecycle',None,'gsys')],
 8:[('Engine calls',  '9a221b4e','iface'),('Schema renames',None,'schema')],
 9:[('Game events',None,'code'),('Map lifecycle',None,'gsys'),('Engine calls',None,'iface'),('Schema renames',None,'schema')],
 14:[('Map lifecycle',None,'gsys')],
 16:[('Entity listeners','ba860a92','code_t1only')],
 18:[('ConVars',None,'code_t1only')],
 20:[('Net writes',None,'code'),('Map lifecycle',None,'code'),('ConVars',None,'code'),('Chat cmds','356152a5','code'),
     ('Engine calls',None,'iface'),('Transmit',None,'iface'),('Schema renames',None,'schema')],
 21:[('Map lifecycle',None,'gsys'),('Chat cmds',None,'iface'),('ConVars',None,'iface'),('Engine calls',None,'iface'),('Damage','624ca0f0','code_t1only')],
 24:[('Map lifecycle',None,'code'),('Engine calls',None,'iface')],
 25:[('Damage',None,'code'),('Sounds',None,'code'),('Entity spawn/model',None,'code'),('Chat cmds',None,'iface'),('ConVars',None,'iface'),('Map lifecycle',None,'gsys'),('Schema renames',None,'schema')],
 28:[('Map lifecycle',None,'gsys')],
 29:[('Map lifecycle',None,'gsys'),('Schema renames',None,'schema')],
 32:[('Net writes',None,'code'),('Map lifecycle',None,'code'),('Weapons',None,'code'),('Engine calls',None,'iface'),('Transmit',None,'iface'),('Schema renames',None,'schema')],
 33:[('Entity spawn/model',None,'code'),('Entity listeners',None,'code'),('Chat cmds',None,'iface'),('ConVars',None,'iface'),('Engine calls',None,'iface'),('Map lifecycle',None,'gsys'),('Schema renames',None,'schema')],
 35:[('ConVars',None,'code_t1only')],
 37:[('Engine calls',None,'iface')],
 38:[('Chat/HUD out',None,'code')],
}
def ch(i,c):  # hours from build to release of commit c
    f=i[7][0][0]; return round(i[5]+(rel[c]-rel[f]).total_seconds()/3600,1)
def bindings(i):
    out={}  # name->(kind sig/off, hours of last fixing commit)
    for c,k,fu in i[7]:
        r=rows.get(c)
        if not r: continue
        for e in r['changed']+r['added']:
            out[e[0]]=(e[1], ch(i,c), out.get(e[0],(None,0))[0] or e[1])
    return {n:(v[0],v[1]) for n,v in out.items()}
OUT={'C','L','C?'}
def groups(i,scen,perbinding):
    o={'T1':i[8],'Ideal':i[10]}[scen]; first=i[5]
    if first is None: return o,{}
    if o=='A': return o,{}
    if o=='U': return o,{'*ALL*':first}
    g={}
    def add(gr,h): g[gr]=max(g.get(gr,0), h if perbinding else first)
    for n,(kind,h) in bindings(i).items():
        if scen=='Ideal' and kind=='offsets': continue   # vfunc offsets auto-derived (RTTI alignment)
        add(B2G[n],h)
    for gr,c,kind in X.get(i[0],[]):
        h=ch(i,c) if c else first
        if scen=='T1': add(gr,h)
        else:
            if kind=='code' and o=='S': add(gr,h)
            if kind=='code_t1only': add(gr,h)  # Ideal outcome D -> data fix by human
            if kind=='schema' and o=='S': add(gr,h)
    return o,g
for pb in (False,True):
  print('\n######## per-binding durations' if pb else '######## first-fix durations')
  for scen in ('T1','Ideal'):
    tot={}; inc_by={}; deg_h=0; core_h=0; out_h=0; deg_inc=[]
    for i in I:
        o,g=groups(i,scen,pb)
        if not g: continue
        if '*ALL*' in g:
            if i[4] in OUT: out_h+=g['*ALL*']
            else:  # '?' impact (#18) under U -> treat as degraded convars
                g={'ConVars':g['*ALL*']}
            if i[4] in OUT: continue
        d=max(g.values()); deg_h+=d
        c=max([h for gr,h in g.items() if gr in CORE],default=0); core_h+=c
        deg_inc.append((i[0],i[2],o,d,c,sorted(g)))
        for gr,h in g.items():
            tot[gr]=tot.get(gr,0)+h; inc_by.setdefault(gr,[]).append((i[0],h))
    print(f'== {scen}: outage {out_h:.0f}h; degraded wall-clock {deg_h:.0f}h; of which core group down {core_h:.0f}h ({core_h/deg_h:.0%})')
    for gr,h in sorted(tot.items(),key=lambda x:-x[1]):
        top=sorted(inc_by[gr],key=lambda x:-x[1])[:4]
        print(f'   {gr:20s} {h:6.0f}h n={len(inc_by[gr]):2d} {"CORE" if gr in CORE else "":4s} top:{top}')
    if not pb:
        for r in deg_inc: print('     ',r[0],r[1][:14].ljust(14),r[2],f'{r[3]:6.1f} core={r[4]:5.1f}',','.join(r[5]))

# ---- today baseline + server types
F=[i for i in I if i[4] not in OUT and i[5] is not None]
print('\nToday: outage',sum(i[5] for i in I if i[4] in OUT and i[5]),'h; degraded (F/? incidents)',sum(i[5] for i in F),'h n=',len(F))
COMMON={'Tick/timers','Chat cmds','Game events','Chat/HUD out','Map lifecycle'}
TYPES={
 'Competitive (MatchZy)': COMMON|{'Net writes','Player mgmt','Entity spawn/model','Entity listeners','ConVars','Engine calls'},
 'Retakes':               COMMON|{'Player mgmt','Weapons','Round control','Entity I/O','Entity spawn/model','Engine calls'},
 'Surf/KZ (SharpTimer)':  COMMON|{'Net writes','Player mgmt','Damage','Entity I/O','Entity spawn/model','Triggers','ConVars','Engine calls'},
 'DM + WeaponPaints':     COMMON|{'Net writes','Player mgmt','Weapons','Damage','Entity I/O','Entity spawn/model','Entity listeners','ConVars','Engine calls','Sounds'},
 'Admin-only (SimpleAdmin)': COMMON|{'Net writes','Player mgmt','Sounds','Transmit','ConVars','Engine calls'},
}
for scen in ('T1','Ideal'):
    degs=[]
    for i in I:
        o,g=groups(i,scen,False)
        if not g or '*ALL*' in g and i[4] in OUT: continue
        if '*ALL*' in g: g={'ConVars':g['*ALL*']}
        degs.append(g)
    tot=sum(max(g.values()) for g in degs)
    print(f'== {scen} server types (degraded wall-clock {tot:.0f}h)')
    for t,dep in TYPES.items():
        imp=sum(max([h for gr,h in g.items() if gr in dep],default=0) for g in degs)
        core=sum(max([h for gr,h in g.items() if gr in CORE],default=0) for g in degs)
        print(f'   {t:26s} impaired {imp:5.0f}h ({imp/tot:.0%}) | untouched {tot-imp:5.0f}h')
# Ideal sensitivity: core only via code (sigs for core auto-heal)
