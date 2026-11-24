import os,re,sys
G={
 'Tick/timers':r'AddTimer|new Timer\(|NextFrame|RegisterListener<Listeners\.OnTick>|OnTick\b|NextWorldUpdate',
 'Chat cmds':r'\[ConsoleCommand|AddCommand\(|AddCommandListener\(\"say|\[CommandHelper',
 'Chat out(all)':r'PrintToChatAll|PrintToCenterAll',
 'Chat out(player)':r'\.PrintToChat\(|PrintToCenter\(|PrintToCenterHtml|PrintToCenterAlert',
 'Events':r'RegisterEventHandler|\[GameEventHandler|HookEvent',
 'NetWrites':r'SetStateChanged|SetStateChangedByOffset',
 'Player mgmt':r'\.Respawn\(|ChangeTeam\(|SwitchTeam\(|CommitSuicide\(|\.Teleport\(|SetPawn',
 'Weapons':r'GiveNamedItem|RemoveWeapons\(|DropActiveWeapon|RemovePlayerItem|RemoveItemByDesignerName|CanAcquireFunc|CanUseFunc|GetCSWeaponDataFromKey|WeaponServices_CanUse|ItemServices_CanAcquire',
 'Damage':r'TakeDamageOld|OnTakeDamage|OnPlayerTakeDamage|OnEntityTakeDamage',
 'Round ctl':r'TerminateRound',
 'Entity I/O':r'HookEntityOutput|AcceptInput|EntityIO|OnEntityOutput|AddEntityIOEvent',
 'Entity spawn':r'CreateEntityByName|DispatchSpawn|\.Remove\(\)|SetModel\(',
 'Entity listeners':r'OnEntityCreated|OnEntitySpawned|OnEntityDeleted',
 'Triggers':r'StartTouch|EndTouch|OnStartTouch',
 'Sounds':r'EmitSound',
 'Transmit':r'CheckTransmit',
 'Map lifecycle':r'OnMapStart|OnMapEnd|OnServerPrecacheResources|OnServerPreEntityThink|OnServerPostEntityThink',
 'ConVars':r'ConVar\.Find|FakeConVar|ConVar<|GetPrimaryValue|SetValue\(',
 'Client listeners':r'OnClientConnect|OnClientDisconnect|OnClientPutInServer|OnClientAuthorized',
 'PostThink':r'PostThink',
 'UserMsg/Voice':r'HookUserMessage|UserMessage|VoiceFlags|SetListenOverride',
 'ServerCmd':r'Server\.ExecuteCommand',
 'MemoryFunc custom':r'new MemoryFunction|VirtualFunction\.Create|GameData\.Get',
}
for repo in sorted(os.listdir(sys.argv[1])):
  cnt={k:0 for k in G}
  for dp,dn,fn in os.walk(os.path.join(sys.argv[1],repo)):
    if '/.git' in dp: continue
    for f in fn:
      if f.endswith('.cs'):
        t=open(os.path.join(dp,f),errors='ignore').read()
        for k,p in G.items(): cnt[k]+=len(re.findall(p,t))
  print(repo[:22].ljust(23),' '.join(f'{k}={v}' for k,v in cnt.items() if v))
