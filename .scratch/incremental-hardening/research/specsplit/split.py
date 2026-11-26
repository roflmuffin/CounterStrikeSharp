# Per-spec split of the T1 backtest: today -> A -> A+B -> A+B+C (-> +D).
# Starts from G1/model.py's hand-labelled T1 outcomes and removes what each spec does not deliver:
#  A (I1-I3 fail soft): sig/schema misses disable a feature instead of crashing; fixes still ship as releases.
#  B (I4-I5 vtable checks, slot bindings, heal): moved vfunc offsets (O) and engine-interface slots (V) heal at boot.
#  C (I6 signed build-keyed updater, I7 CI): data fixes ship without a release; a pushed blank binding is a kill
#     switch, which is what made G (IGameSystem) and M (semantic) crashes "softer" in the original T1 labels.
#  D (own loader, opt-in): shares root cause with CSSharp in 7/9 Metamod load failures -> no change on these 39.
# Run: python3 -P specsplit/split.py .   (from research/)
import sys
exec(open(sys.argv[1]+'/H/degrade.py').read().split("for pb in (False,True):")[0])
import re
def items(r): return set(t[0] for t in r[3].replace('(',' ').split() if t[0] in 'SOAVGTLRNM')
OUT={'C','L','C?'}
def outcome(r,sc):
    t1=r[8]; it=items(r); crash=r[4] in OUT
    if sc=='today': return 'U'
    if sc=='ABC':
        return 'A' if it<= {'O','V'} else t1
    if sc=='AB':
        if t1=='D': return 'A' if it<={'O','V'} else 'S'
        if t1=='S': return 'U' if it&{'G','M'} else 'S'
        return t1
    if sc=='A':
        if t1=='D': return 'U' if (crash and it&{'O','V'}) else 'S'
        if t1=='S': return 'U' if it&{'G','M','V','O'} else 'S'
        return t1
SC=['today','A','AB','ABC']
flags={'today':'','A':'a','AB':'ab','ABC':'abc'}
CK={'S':'D','S1':'D','CD':'D','O':'A','V':'A','G':'C','T':'C','N':'C','C':'C','CI':'C'}
def commit_counts(sc,fu_only=False):
    n=0
    for r in I:
        for c,k,fu in r[7]:
            if fu_only and not fu: continue
            if sc in('today','A'): n+=1
            elif sc=='AB': n+= k not in('O','V')
            else: n+= CK[k]=='C'
    return n
def groups_s(i,sc):
    o=outcome(i,sc); first=i[5]
    if first is None or o=='A': return o,{}
    if o=='U': return o,{'*ALL*':first}
    b='b' in flags[sc]; g={}
    for n,(kind,h) in bindings(i).items():
        if b and kind=='offsets': continue
        g[B2G[n]]=first
    for gr,c,kind in X.get(i[0],[]):
        if b and kind=='iface': continue
        g[gr]=first
    if not g: g={'(none)':first}
    return o,g
print('id  items                         imp  T1 | A AB ABC')
for r in I: print(f"{r[0]:2d} {r[3][:30]:30s} {r[4]:3s} {r[8]}  | {outcome(r,'A')} {outcome(r,'AB')} {outcome(r,'ABC')}")
for sc in SC:
    nrel=sum(outcome(r,sc) in "SU" for r in I)
    out=0; deg=0; core=0
    for i in I:
        o,g=groups_s(i,sc)
        if not g: continue
        if '*ALL*' in g:
            if i[4] in OUT: out+=g['*ALL*']; continue
            g={'ConVars':g['*ALL*']} if sc!='today' or True else g
        deg+=max(g.values()); core+=max([h for gr,h in g.items() if gr in CORE],default=0)
    if sc=='today':  # today: F incidents are degraded, core per the same group map
        pass
    print(f"{sc:6s} releases={nrel:2d} commits={commit_counts(sc):2d} followups={commit_counts(sc,True):2d} outage={out:6.1f}h degraded={deg:6.1f}h core={core:5.1f}h effective={out+core:6.1f}h broken_or_degraded={out+deg:6.1f}h")
