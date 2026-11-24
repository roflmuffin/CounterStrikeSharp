import os,re,sys
G={'Teleport':r'\.Teleport\(','Respawn':r'\.Respawn\(','Slay':r'CommitSuicide\(','Team':r'ChangeTeam\(|SwitchTeam\(','SetPawn':r'SetPawn\(',
'Give':r'GiveNamedItem','Strip':r'RemoveWeapons\(|DropActiveWeapon\(|RemoveItemByDesignerName|RemovePlayerItem\(','Restrict':r'CanUse|CanAcquire','WData':r'GetCSWeaponDataFromKey',
'Create':r'CreateEntityByName|DispatchSpawn\(','Remove':r'\.Remove\(\)','SetModel':r'\.SetModel\(','Terminate':r'TerminateRound','IOhook':r'HookEntityOutput|EntityOutputHook',
'AcceptInput':r'AcceptInput\(','TrigSig':r'CBaseTrigger_StartTouch|CBaseTrigger_EndTouch','Sound':r'EmitSound','Transmit':r'CheckTransmit','Damage':r'TakeDamage','PostThink':r'PostThink',
'EntListen':r'OnEntityCreated|OnEntitySpawned|OnEntityDeleted','Precache':r'OnServerPrecacheResources|AddResource','ButtonsL':r'OnPlayerButtonsChanged','PrintAll':r'PrintToChatAll|PrintToCenterAll','PrintP':r'\.PrintToChat\(|PrintToCenter','Voice':r'VoiceFlags|SetListenOverride|ListenOverride','ExecCmd':r'Server\.ExecuteCommand','Console':r'PrintToConsole'}
for repo in sorted(os.listdir(sys.argv[1])):
  cnt={k:0 for k in G}
  for dp,dn,fn in os.walk(os.path.join(sys.argv[1],repo)):
    if '/.git' in dp: continue
    for f in fn:
      if f.endswith('.cs'):
        t=open(os.path.join(dp,f),errors='ignore').read()
        for k,p in G.items(): cnt[k]+=len(re.findall(p,t))
  print(repo[:18].ljust(19),' '.join(f'{k}={v}' for k,v in cnt.items() if v))
