#!/usr/bin/env python3
"""Per schema-update commit: count generated C# schema properties whose NAME disappeared from a class file (true removals/renames), vs type-only changes; and deleted class files."""
import subprocess,re,collections
REPO="/home/markus/dev/other/CounterStrikeSharp"
def git(*a): return subprocess.run(["git","-C",REPO,*a],capture_output=True,text=True).stdout
P=re.compile(r'^([-+])\s+public (?:new )?(?:unsafe )?(?:ref )?([^(=]+?) ([A-Za-z0-9_]+) (\{|=>)')
tot_rm=tot_ty=tot_cls=0
for h in git("log","--format=%h","--reverse","--","managed/CounterStrikeSharp.SchemaGen/Schema/server.json").split():
    d=git("show","--format=","-U0",h,"--","managed/CounterStrikeSharp.API/Core/Schema","managed/CounterStrikeSharp.API/Generated")
    f=None; rm=collections.defaultdict(dict); ad=collections.defaultdict(dict); deleted=set()
    for line in d.splitlines():
        if line.startswith("diff --git"): f=line.split(" b/")[-1]; continue
        if line.startswith("+++ /dev/null"): deleted.add(f); continue
        m=P.match(line)
        if m and f and "/Enums/" not in f:
            (rm if m.group(1)=="-" else ad)[f][m.group(3)]=m.group(2)
    removed=0; typech=0
    for f,props in rm.items():
        if f in deleted: continue
        for n,t in props.items():
            if n not in ad.get(f,{}): removed+=1
            elif ad[f][n]!=t: typech+=1
    cls=len([x for x in deleted if "/Enums/" not in x])
    if removed or typech or cls:
        s=git("show","-s","--format=%ad %s","--date=short",h)[:80]
        print(f"{h} removed/renamed={removed:3d} type-changed={typech:3d} classfiles-deleted={cls:3d} | {s}")
    tot_rm+=removed; tot_ty+=typech; tot_cls+=cls
print("TOTAL removed/renamed props:",tot_rm,"type changes:",tot_ty,"class files deleted:",tot_cls)
