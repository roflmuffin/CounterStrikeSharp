import sys, struct, collections
def load(p):
    d=open(p,'rb').read()
    shoff,=struct.unpack_from('<Q',d,0x28); shentsize,shnum,shstrndx=struct.unpack_from('<HHH',d,0x3A)
    secs=[]
    for i in range(shnum):
        nm,ty,fl,addr,off,sz,lk,inf,al,es=struct.unpack_from('<IIQQQQIIQQ',d,shoff+i*shentsize)
        secs.append([nm,ty,addr,off,sz])
    so=secs[shstrndx][3]
    S={}
    for s in secs:
        n=d[so+s[0]:d.index(b'\0',so+s[0])].decode(); S[n]=s
    return d,S
def v2o(S,a):
    for n,(nm,ty,addr,off,sz) in S.items():
        if ty!=8 and addr and addr<=a<addr+sz: return off+a-addr
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
