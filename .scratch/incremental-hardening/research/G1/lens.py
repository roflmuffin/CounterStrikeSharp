import pickle,sys,collections
S=sys.argv[1]
t=pickle.load(open(S+'/C/cache/sighist.pkl','rb'))
print(type(t), list(t.keys()) if isinstance(t,dict) else len(t))
for plat,rows in (t.items() if isinstance(t,dict) else [('all',t)]):
    print('==',plat, rows[0].keys() if rows else None)
    for x in rows:
        if 'lengthened' in x['cls'] or 'different prologue' in x['cls']:
            print(plat, x['date'], x['commit'][:8], x['name'], '|', x['cls'][:30])
