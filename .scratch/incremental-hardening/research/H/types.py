import sys
sys.argv=[sys.argv[0],sys.argv[1]]
exec(open(sys.argv[1]+'/H/degrade.py').read().split('# ---- today baseline')[0].split("for pb in (False,True):")[0])
B2S={'CBaseEntity_Teleport':'Teleport','CCSPlayerController_Respawn':'Respawn','CCSPlayerPawn_Respawn':'Respawn','CBasePlayerPawn_CommitSuicide':'Slay',
 'CCSPlayerController_ChangeTeam':'Team','CCSPlayerController_SwitchTeam':'Team','CBasePlayerController_SetPawn':'SetPawn',
 'GiveNamedItem':'Give','CCSPlayer_ItemServices_GiveNamedItem':'Give','CCSPlayer_ItemServices_DropActivePlayerWeapon':'Strip','CCSPlayer_ItemServices_RemoveWeapons':'Strip','CBasePlayerPawn_RemovePlayerItem':'Strip',
 'CCSPlayer_WeaponServices_CanUse':'Restrict','CCSPlayer_ItemServices_CanAcquire':'Restrict','GetCSWeaponDataFromKey':'WData',
 'UTIL_CreateEntityByName':'Create','CBaseEntity_DispatchSpawn':'Create','UTIL_Remove':'Create','CBaseModelEntity_SetModel':'SetModel','CGameSceneNode_GetSkeletonInstance':'Skeleton',
 'CCSGameRules_TerminateRound':'Terminate','CCSGameRules_FindPickerEntity':'Picker','CBaseEntity_TakeDamageOld':'Damage','CTakeDamageInfo_HitGroup':'Damage','CBaseEntity_IsPlayerPawn':'Damage',
 'CBaseEntity_EmitSoundFilter':'Sound','CEntityIOOutput_FireOutputInternal':'IOhook','CEntityInstance_AcceptInput':'AcceptInput','CEntitySystem_AddEntityIOEvent':'AcceptInput',
 'CBaseTrigger_StartTouch':'TrigSig','CBaseTrigger_EndTouch':'TrigSig','CheckTransmitPlayerSlot':'Transmit','ISource2GameEntities::CheckTransmit':'Transmit','CheckTransmit':'Transmit',
 'CCSPlayerPawnBase_PostThink':'PostThink','IGameSystem_InitAllSystems_pFirst':'GameSys','CEntityResourceManifest_AddResource':'GameSys','UTIL_ClientPrintAll':'PrintAll','ClientPrint':'ClientPrintSig',
 'Host_Say':'Chat cmds','CGameEventManager_Init':'Game events','LegacyGameEventListener':'Game events','GameEventManager':'Game events',
 'NetworkStateChanged':'Net writes','SetStateChanged':'Net writes','StateChanged':'Net writes','CCSNavArea_IsValidNavMesh':'Nav'}
X2S={'Player mgmt':['Teleport','Team','Respawn'],'Map lifecycle':['GameSys'],'Engine calls':['ExecCmd'],'Schema renames':[],'Game events':['Game events'],'Entity listeners':['EntListen'],
 'ConVars':['ConVars'],'Net writes':['Net writes'],'Chat cmds':['Chat cmds'],'Transmit':['Transmit'],'Damage':['Damage'],'Sounds':['Sound'],'Entity spawn/model':['Create','SetModel'],
 'Weapons':['Give','Strip','WData'],'Chat/HUD out':['PrintP']}
def sgroups(i,scen):
    o={'T1':i[8],'Ideal':i[10]}[scen]; first=i[5]
    if first is None or o=='A': return o,{}
    if o=='U': return o,({'*ALL*':first} if i[4] in OUT else {'ConVars':first})
    g={}
    for n,(kind,h) in bindings(i).items():
        if scen=='Ideal' and kind=='offsets': continue
        g[B2S[n]]=first
    for gr,c,kind in X.get(i[0],[]):
        ok = scen=='T1' or (kind=='code' and o=='S') or kind=='code_t1only' or (kind=='schema' and o=='S')
        if ok:
            for s in X2S[gr]: g[s]=first
    return o,g
CORE={'Chat cmds','Game events','Net writes','Tick/timers'}
COM={'Chat cmds','Game events','Tick/timers','PrintP'}
T={'Competitive (MatchZy)':COM|{'Teleport','Respawn','Slay','Team','Give','Strip','Create','AcceptInput','EntListen','PrintAll','ExecCmd','ConVars','Net writes'},
 'Retakes':COM|{'Teleport','Respawn','Slay','Team','Give','Strip','Create','SetModel','Terminate','AcceptInput','PrintAll','ExecCmd'},
 'Surf/KZ (SharpTimer)':COM|{'Teleport','Team','Give','Strip','Create','SetModel','IOhook','Damage','PrintAll','ExecCmd','ConVars','Net writes'},
 'DM + WeaponPaints':COM|{'Teleport','Respawn','Slay','Give','Strip','Restrict','Create','SetModel','Terminate','AcceptInput','Sound','Damage','EntListen','PrintAll','ExecCmd','ConVars','Net writes'},
 'Admin-only (SimpleAdmin)':COM|{'Teleport','Respawn','Slay','Team','Give','Strip','AcceptInput','Sound','Transmit','GameSys','PrintAll','ExecCmd','ConVars','Net writes'}}
for scen in ('T1','Ideal'):
    D=[(i,g) for i in I for o,g in [sgroups(i,scen)] if g and '*ALL*' not in g]
    tot=sum(i[5] for i,g in D); 
    print(f'== {scen} degraded {tot:.0f}h')
    for t,dep in T.items():
        imp=sum(i[5] for i,g in D if set(g)&dep); br=sum(i[5] for i,g in D if set(g)&dep&CORE)
        print(f'  {t:26s} some-used-feature-off {imp:5.0f}h ({imp/tot:.0%})  core-off {br:4.0f}h ({br/tot:.0%})  unaffected {tot-imp:5.0f}h ({1-imp/tot:.0%})')
    # sub-binding hours
    sub={}
    for i,g in D:
        for s in g: sub[s]=sub.get(s,0)+i[5]
    print('   subgroups:',sorted(((k,round(v)) for k,v in sub.items()),key=lambda x:-x[1]))
