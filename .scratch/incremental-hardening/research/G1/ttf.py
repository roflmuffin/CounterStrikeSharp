import json,sys,datetime as dt
S=sys.argv[1]
gt=json.load(open(S+'/E/gt_commits.json'))
P=lambda s: dt.datetime.fromisoformat(s.replace('Z','+00:00'))
gtp={}
for g in gt:
    if g.get('patch'): gtp.setdefault(g['patch'],[]).append(P(g['date']))
tags={}
for l in open(S+'/G1/fix_tags.txt'):
    p=l.split()
    tags[p[0]]=(P(p[1]), P(p[3]) if len(p)>3 else P(p[1]))
cur=[l.split()[0] for l in open(S+'/E/cssharp_curated_fixes.txt')]
inc=[l for l in open(S+'/E/incidents.txt') if l[:2]=='20' and ' n=' in l]
i=0
for l in inc:
    n=int(l.split('n=')[1].split()[0]); cs=cur[i:i+n]; i+=n
    patch=l.split('build ')[1].split()[1]
    b=min(gtp.get(patch,[None]) or [None])
    first=tags[cs[0]][1]; last=max(tags[c][1] for c in cs)
    h=lambda t: round((t-b).total_seconds()/3600,1) if b else None
    print(f"{l[:10]} {patch:9s} build={b.strftime('%m-%d %H:%M') if b else '?':12s} first_rel_h={h(first)} last_rel_h={h(last)} n={n} {' '.join(c[:8] for c in cs)}")
