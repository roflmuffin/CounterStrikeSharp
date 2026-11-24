import sys, struct, collections
sys.path.insert(0,'.')
exec(open(sys.argv[0].replace('vt2.py','vt.py')).read().split('def main')[0])
def vtab(p, cls):
    d,S=load(p)
    rel={}
    _,_,_,off,sz=S['.rela.dyn']
    for i in range(0,sz,24):
        o,info,add=struct.unpack_from('<QQq',d,off+i)
        if info&0xffffffff==8: rel[o]=add
    byadd=collections.defaultdict(list)
    for o,a in rel.items(): byadd[a].append(o)
    tx=S['.text']; tlo,thi=tx[2],tx[2]+tx[4]
    ro=S['.rodata']; name=cls.encode()+b'\0'
    i=d.find(name, ro[3], ro[3]+ro[4])
    while i!=-1:
        if d[i-1]==0:
            sa=ro[2]+i-ro[3]
            for o in byadd.get(sa,[]):
                for vo in byadd.get(o-8,[]):
                    ott,=struct.unpack_from('<q',d,v2o(S,vo-8))
                    if ott==0:
                        slots=[];a=vo+8
                        while a in rel and tlo<=rel[a]<thi: slots.append(rel[a]); a+=8
                        return d,S,slots
        i=d.find(name,i+1,ro[3]+ro[4])
if __name__=='__main__':
    d,S,sl=vtab(sys.argv[1],sys.argv[2])
    c=collections.Counter(sl)
    print('nslots',len(sl),'distinct',len(c),'shared:',[(i,hex(a)) for i,a in enumerate(sl) if c[a]>1])
    print(' '.join(hex(a) for a in sl))
