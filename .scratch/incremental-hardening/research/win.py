import sys,glob,datetime as D,re
builds=[("1.41.5.1","2026-04-20T23:02:18Z"),("1.41.6.8","2026-07-08T22:54:05Z"),("1.41.6.8b","2026-07-09T15:48:29Z"),("1.41.7.7","2026-08-24T23:42:44Z"),("1.41.8.2","2026-09-23T00:05:28Z"),("1.41.8.3","2026-09-23T23:10:20Z"),("1.41.9.1-2000934","2026-10-20T22:33:13Z"),("1.41.9.2","2026-10-22T22:56:41Z"),("1.41.9.6","2026-11-05T00:55:25Z"),("1.41.9.7","2026-11-11T23:05:21Z"),("1.42.0.0","2026-11-20T00:59:22Z"),("1.42.0.1","2026-11-23T23:02:09Z")]
p=lambda s:D.datetime.fromisoformat(s.replace("Z","+00:00"))
H=float(sys.argv[2]) if len(sys.argv)>2 else 72
f=sys.argv[1]
rows=[l.rstrip("\n").split("\t") for l in open(f) if l.strip() and not l.startswith("{")]
for b,t in builds:
  bt=p(t); print("###",b,t)
  for r in sorted(rows):
    try: ct=p(r[0])
    except: continue
    h=(ct-bt).total_seconds()/3600
    if -2<=h<=H: print(f"  +{h:6.1f}h {r[0][:16]} {' '.join(r[2:])[:110]}")
