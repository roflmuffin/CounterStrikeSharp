#!/usr/bin/env python3
"""Signature length/wildcard stats for current gamedata + churn per entry (excluding cosmetic reformat commit e3d2370e)."""
import json,sys,collections,statistics
H=json.load(open(sys.argv[1]))
cur=json.load(open("/home/markus/dev/other/CounterStrikeSharp/configs/addons/counterstrikesharp/gamedata/gamedata.json"))
COSMETIC={"e3d2370e"}
churn=collections.Counter(); commits_per=collections.defaultdict(set); first=dict()
for r in H["rows"]:
    for k in r["added"]:
        first.setdefault(k[0],r["date"])
    if r["h"] in COSMETIC: continue
    for k in r["changed"]:
        churn[(k[0],k[1],k[2])]+=1; commits_per[k[0]].add(r["h"])
print("# Current signatures: bytes, wildcards, wildcard%, churn(lin/win), first added")
rows=[]
for name,v in cur.items():
    if "signatures" in v:
        for plat in ("linux","windows"):
            s=v["signatures"][plat].split()
            w=sum(1 for b in s if b in("?","??","2A","\\x2A"))
            rows.append((name,plat,len(s),w))
for name in cur:
    if "signatures" in cur[name]:
        l=[r for r in rows if r[0]==name]
        print(f"{name:40s} lin {l[0][2]:3d}B {l[0][3]:2d}? ({100*l[0][3]/l[0][2]:4.0f}%)  win {l[1][2]:3d}B {l[1][3]:2d}? ({100*l[1][3]/l[1][2]:4.0f}%)  churn lin={churn[(name,'signatures','linux')]:2d} win={churn[(name,'signatures','windows')]:2d} commits={len(commits_per[name]):2d} since {first.get(name)}")
for plat in ("linux","windows"):
    L=[r[2] for r in rows if r[1]==plat]; W=[r[3]/r[2] for r in rows if r[1]==plat]
    print(f"{plat}: n={len(L)} len min/median/max={min(L)}/{statistics.median(L)}/{max(L)} wildcard% mean={100*statistics.mean(W):.0f} median={100*statistics.median(W):.0f}")
print("\n# Current offsets: churn")
for name,v in cur.items():
    if "offsets" in v:
        print(f"{name:45s} win={v['offsets']['windows']:4d} lin={v['offsets']['linux']:4d} churn lin={churn[(name,'offsets','linux')]:2d} win={churn[(name,'offsets','windows')]:2d} commits={len(commits_per[name]):2d} since {first.get(name)}")
print("\n# Top churn overall (entry, total value changes both platforms, distinct commits)")
tot=collections.Counter()
for (n,k,p),c in churn.items(): tot[n]+=c
for n,c in tot.most_common(): print(f"{n:45s} {c:3d} {len(commits_per[n]):3d}")
print("\nTotal value changes (excl cosmetic):",sum(churn.values()), "sig:",sum(c for (n,k,p),c in churn.items() if k=='signatures'), "off:",sum(c for (n,k,p),c in churn.items() if k=='offsets'))
print("lin sig:",sum(c for (n,k,p),c in churn.items() if k=='signatures' and p=='linux'),"win sig:",sum(c for (n,k,p),c in churn.items() if k=='signatures' and p=='windows'))
