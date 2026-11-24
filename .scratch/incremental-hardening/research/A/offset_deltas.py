#!/usr/bin/env python3
"""Show per-commit deltas of vtable offsets to expose cascade shifts (inserted vfuncs shift all later indices)."""
import json,subprocess,re,sys
REPO="/home/markus/dev/other/CounterStrikeSharp"
def git(*a): return subprocess.run(["git","-C",REPO,*a],capture_output=True,text=True).stdout
H=json.load(open(sys.argv[1]))
prev={}
for r in H["rows"]:
    p="configs/addons/counterstrikesharp/gamedata/gamedata.json"
    t=git("show",f"{r['h']}:{p}") or git("show",f"{r['h']}:configs/gamedata.json")
    try: d=json.loads(re.sub(r"^\s*//.*$","",t,flags=re.M))
    except: continue
    cur={k:v["offsets"] for k,v in d.items() if isinstance(v,dict) and "offsets" in v}
    deltas=[]
    for k,v in cur.items():
        if k in prev:
            for pl in ("linux","windows"):
                if pl in v and pl in prev[k] and v[pl]!=prev[k][pl]:
                    deltas.append(f"{k.replace('CCSPlayerController_','PC_').replace('CCSPlayer_ItemServices_','IS_').replace('CBasePlayerPawn_','Pawn_').replace('CBaseEntity_','BE_')}[{pl[0]}]{v[pl]-prev[k][pl]:+d}")
    if deltas: print(r['h'],r['date'],' '.join(deltas))
    prev=cur
