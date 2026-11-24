import subprocess,re,sys,difflib
repo=sys.argv[2] if len(sys.argv)>2 else '/home/markus/dev/other/cssharp-hardening-research/scratchpad/B/hl2sdk-cs2'
def git(*a): return subprocess.run(['git','-C',repo,*a],capture_output=True,text=True,errors='replace').stdout
def strip(s): s=re.sub(r'/\*.*?\*/','',s,flags=re.S); return re.sub(r'//[^\n]*','',s)
def body(src,cls):
    m=re.search(r'\b(?:abstract_class|class|struct)\s+(?:[A-Z_]+\s+)?'+cls+r'\b([^;{]*)\{',src)
    if not m: return None,None
    i=m.end(); d=1; j=i
    while j<len(src) and d:
        d+= (src[j]=='{') - (src[j]=='}'); j+=1
    return m.group(1),src[i:j]
def virts(sha,cls):
    for f in git('grep','-l','-E',r'(abstract_class|class|struct)\s+(\w+\s+)?'+cls+r'\b[^;]*$',sha,'--','*.h').split():
        f=f.split(':',1)[1]
        base,b=body(strip(git('show',f'{sha}:{f}')),cls)
        if b is None: continue
        out=[];d=0
        for line in b.split('\n'):
            if d==0 and re.match(r'\s*virtual\b',line):
                m=re.search(r'(~?\w+)\s*\(',line); out.append(m.group(1) if m else '?')
            d+=line.count('{')-line.count('}'); d=max(d,0)
        return base.strip(),out
    return None,None
CL=['ISource2Server','ISource2GameClients','ISource2GameEntities','IVEngineServer2','ISource2Engine','ICvar','IGameEventManager2','INetworkServerService','IEngineService','IEngineServiceMgr','IGameEventSystem','IAppSystem','IGameSystem','CBaseGameSystem','CEntityInstance','INetworkMessages','IFileSystem']
USED={'ISource2Server':'GameFrame ServerHibernationUpdate GameServerSteamAPIActivated GameServerSteamAPIDeactivated OnHostNameChanged PreFatalShutdown UpdateWhenNotInGame PreWorldUpdate IsPaused GetEconItemSystem'.split(),
'ISource2GameClients':'ClientConnect ClientPutInServer ClientDisconnect ClientCommand ClientVoice ClientSvcUserMessage'.split(),
'IVEngineServer2':'SetClientListening GetPlayerUserId ServerCommand IsClientFullyAuthenticated GetPlayerNetInfo GetClientConVarValue ClientPrintf SetFakeClientConVarValue DisconnectClient PrecacheGeneric IsMapValid GetGameDir GetClientSteamID ClientCommand GetServerGlobals'.split(),
'ICvar':'DispatchConCommand FindConCommand GetConCommandData UnregisterConCommandCallbacks RemoveGlobalChangeCallback InstallGlobalChangeCallback GetConVarData FindConVar RegisterConCommand'.split(),
'IGameEventManager2':'FireEvent CreateEvent RemoveListener LoadEventsFromFile FreeEvent FindListener AddListener'.split(),
'INetworkServerService':['StartupServer'],'IEngineServiceMgr':['RegisterLoopMode','FindService'],'IGameEventSystem':['PostEventAbstract'],'ISource2GameEntities':['CheckTransmit'],'INetworkMessages':['FindNetworkMessagePartial','FindNetworkMessageById']}
for l in open(sys.argv[1]):
    if not l.strip(): continue
    tag,a,b=l.split()
    print('==',tag)
    for c in CL:
        ba,va=virts(a,c); bb,vb=virts(b,c)
        if va is None and vb is None: continue
        if va==vb and ba==bb: continue
        va=va or []; vb=vb or []
        sm=difflib.SequenceMatcher(a=va,b=vb,autojunk=False)
        ops=[(t,va[i1:i2],vb[j1:j2],i1) for t,i1,i2,j1,j2 in sm.get_opcodes() if t!='equal']
        moved=[(m,va.index(m),vb.index(m)) for m in USED.get(c,[]) if m in va and m in vb and va.index(m)!=vb.index(m)]
        print(f'  {c}: {len(va)}->{len(vb)} virtuals; base {ba!r}->{bb!r}; moved-used={moved}')
        for t,x,y,i in ops[:6]: print('     ',t,'@',i,x[:4],'->',y[:4])
