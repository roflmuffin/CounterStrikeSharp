import sys,pickle,collections; sys.path.insert(0, sys.argv[1])
from fp import Mod
from match import all_fps
from builds import S
for a,b in (('1.41.9.7','1.42.0.0'),('1.42.0.0','14201')):
    A,B=Mod(a,'server'),Mod(b,'server'); fa,fb=all_fps(A),all_fps(B)
    M,how=pickle.load(open(f'{S}/cache/match_server_{a}_{b}.pkl','rb'))
    ch=[(x,y) for x,y in M.items() if fa[x]['norm']!=fb[y]['norm']]
    big=[(x,y) for x,y in ch if fa[x]['size']!=fb[y]['size']]
    print(a,b,'matched',len(M),'norm-changed',len(ch),'size-changed',len(big))
    cnt=collections.Counter()
    for x,y in ch:
        for s in set(B.strrefs(y)):
            if '.cpp' in s or '::' in s: cnt[s[:60]]+=1
    print('  sample strings in changed funcs:',cnt.most_common(8))
    # unmatched in B
    mv=set(M.values()); un=[f for f in fb if f not in mv]
    print('  unmatched new funcs',len(un),'of which ninsn>=20:',sum(1 for f in un if fb[f]['ninsn']>=20))
