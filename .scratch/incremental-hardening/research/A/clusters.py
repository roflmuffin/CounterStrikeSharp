#!/usr/bin/env python3
"""Aggregate gamedata changes into game-update clusters; classify sig(1) / vfunc offset(2) / struct offset(3)."""
import json,sys
H={r["h"]:r for r in json.load(open(sys.argv[1]))["rows"]}
STRUCT={"CTakeDamageInfo_HitGroup","GameEntitySystem","CheckTransmitPlayerSlot"}
C=[ # (label, inferred game-update date, commits)
("2023-11-02 update","2023-11-02?",["c01357e8"]),
("2023-11-14 update?","2023-11-14?",["0ddf6bcd"]),
("2023-11-17 update","2023-11-17",["0f72631e"]),
("2023-11-20 update?","2023-11-20?",["6147739c"]),
("2023-12-13 update?","2023-12-13?",["74fd0e08"]),
("A Call to Arms","2024-02-06",["f2b8044b","12c6f4d0","8967c40b"]),
("late-Feb 2024","2024-02-28?",["983d9149"]),
("2024-04-25 update","2024-04-25",["7cae4be9","14224274"]),
("2024-05-23 update","2024-05-23",["cafc4e23","3240a5e5","13ec19e4","eea64519"]),
("Jun-Jul 2024 SetPawn/AddResource","2024-06?",["54cc93e0","2eaf7c2d","f8451c28"]),
("Aug 2024 update","2024-08-08?",["aa40d81a","a82faeb5"]),
("The Armory","2024-10-02",["2c7f8961","8a795de9"]),
("mid-Oct 2024","2024-10-16?",["b54f5c3d"]),
("2024-12-19 update","2024-12-19",["461fc0ea"]),
("Mar/Apr 2025","2025-03-31?",["0ce4a290","68e6ffae"]),
("1.40.8.5","2025-07-02",["bebeedf2","018bcba6"]),
("Patch 19388062","2025-07-28",["b7abd1d5","1f30e561"]),
("Patch 19602992","2025-08-14",["bf1ce2e8","936b88d5"]),
("late-Aug fixups","2025-08-?",["72d66cb5","54f8d5ef"]),
("Patch 20022951","2025-09-17",["2c809713"]),
("1.41.1.3","2025-10-14",["37951f28","34598dd5","b4e83dfb"]),
("Nov 2025","2025-11-12?",["6461235c"]),
("1.41.3.x","2026-01-21",["fe3e32f3","32628275","9841c92e"]),
("Feb/Mar 2026","2026-02/03?",["4006494c","03a452ca"]),
("AG2 (AnimGraph2)","2026-04-21",["8a9fbe8f","d935b08c"]),
("1.41.6.9","2026-07-09",["0586daf3"]),
("1.41.7.7","2026-08-25?",["faa97820"]),
("1.41.8.2","2026-09-22/23",["2f984988","bd1d9365","6d22cb54"]),
("Oct-15 2026 update","2026-10-15",["c67396fa"]),
("1.41.9.2","2026-10-22",["5465e2aa"]),
("1.41.9.7","2026-11-11/12",["2eb9a57d","c1b3d1f3"]),
]
tot={1:set(),2:set(),3:set()}; T=[0,0,0]
print("| update | date | commits | (1) sigs changed entries [lin/win] | (2) vfunc offsets entries | (3) struct offs (gamedata) |")
print("|---|---|---|---|---|---|")
for lab,d,cs in C:
    s={};o={};st={}
    for c in cs:
        r=H.get(c)
        if not r: continue
        for n,k,p in r["changed"]+[x for x in r["removed"]]:
            tgt = s if k=="signatures" else (st if n in STRUCT else o)
            tgt.setdefault(n,set()).add(p[0])
    sl=sum(1 for v in s.values() if 'l' in v); sw=sum(1 for v in s.values() if 'w' in v)
    T[0]+=len(s);T[1]+=len(o);T[2]+=len(st)
    print(f"| {lab} | {d} | {' '.join(cs)} | {len(s)} [{sl}/{sw}] | {len(o)} | {len(st)} |")
print(f"\nTOTAL entry-changes: sig={T[0]} vfunc={T[1]} struct={T[2]} over {len(C)} clusters")
