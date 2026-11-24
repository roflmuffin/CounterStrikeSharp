#!/usr/bin/env python3
"""Walk gamedata.json history (incl. pre-rename configs/gamedata.json) and emit per-commit changes."""
import json, subprocess, sys, re, collections
REPO="/home/markus/dev/other/CounterStrikeSharp"
def git(*a): return subprocess.run(["git","-C",REPO,*a],capture_output=True,text=True).stdout
log=git("log","--follow","--format=%H|%h|%ad|%s","--date=short","--name-only","--","configs/addons/counterstrikesharp/gamedata/gamedata.json")
commits=[]; cur=None
for line in log.splitlines():
    if not line.strip(): continue
    if re.match(r"^[0-9a-f]{40}\|",line):
        cur=line.split("|",3); commits.append(cur+[None])
    else: commits[-1][4]=line.strip()
def load(h,p):
    t=git("show",f"{h}:{p}")
    # strip // comments
    t=re.sub(r"^\s*//.*$","",t,flags=re.M)
    try: return json.loads(t)
    except Exception as e:
        try:
            import ast
            return json.loads(re.sub(r",(\s*[}\]])",r"\1",t))
        except Exception as e2: print("PARSEFAIL",h,e2,file=sys.stderr); return None
def flat(d):
    out={}
    if not d: return out
    for k,v in d.items():
        if not isinstance(v,dict): continue
        for kind in ("signatures","offsets","addresses","patches"):
            if kind in v and isinstance(v[kind],dict):
                for plat,val in v[kind].items():
                    if plat=="library": continue
                    out[(k,kind,plat)]=val
    return out
commits.reverse()
prev={}
churn=collections.Counter(); rows=[]
for full,h,date,subj,path in commits:
    d=load(full,path); f=flat(d)
    if d is None: continue
    added=[k for k in f if k not in prev]; removed=[k for k in prev if k not in f]
    changed=[k for k in f if k in prev and prev[k]!=f[k]]
    for k in changed: churn[k]+=1
    rows.append(dict(h=h,date=date,subj=subj,added=added,removed=removed,changed=changed,n=len({k[0] for k in f})))
    prev=f
json.dump(dict(rows=[{**r,"added":[list(x) for x in r["added"]],"removed":[list(x) for x in r["removed"]],"changed":[list(x) for x in r["changed"]]} for r in rows],
               churn=[[list(k),v] for k,v in churn.most_common()]),open(sys.argv[1],"w"),indent=1)
for r in rows:
    def s(lst):
        c=collections.Counter((k[1][:3],k[2][:3]) for k in lst); return " ".join(f"{a}/{b}:{n}" for (a,b),n in sorted(c.items()))
    print(f"{r['h']} {r['date']} entries={r['n']} +[{s(r['added'])}] -[{s(r['removed'])}] ~[{s(r['changed'])}] | {r['subj'][:70]}")
    if r['changed']:
        print("     ~ "+", ".join(sorted({f'{k[0]}({k[1][:3]}/{k[2][:3]})' for k in r['changed']})))
