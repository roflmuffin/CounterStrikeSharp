import sys,json; sys.path.insert(0, sys.argv[1])
from builds import S
mine=json.load(open(S+'/cache/schema_static.json'))['1.42.0.0']
es=json.load(open(sys.argv[2]))
tr={}
for c in es['classes']:
    if c['module'] in ('server','libserver.so','server.dll') or True:
        tr.setdefault(c['module'],{})[c['name']]={f['name']:int(f['offset']) for f in c['fields']}
print({m:len(v) for m,v in tr.items()})
srv=tr.get('server') or tr.get('libserver.so')
match=mis=miss=0; ex=[]
for cn,c in mine.items():
    t=srv.get(cn.split('@')[0])
    if t is None: miss+=1; continue
    for f,o in c['fields'].items():
        if t.get(f)==o: match+=1
        else: mis+=1; ex.append((cn,f,o,t.get(f)))
print('mine classes',len(mine),'not in tracker',miss,'field offsets agree',match,'disagree',mis, ex[:5])
print('tracker server classes',len(srv),'with fields',sum(1 for v in srv.values() if v),'fields',sum(len(v) for v in srv.values()))
