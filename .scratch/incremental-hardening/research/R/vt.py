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
def main(p, cls):
    d,S=load(p)
    rel={}
    _,_,_,off,sz=S['.rela.dyn']
    for i in range(0,sz,24):
        o,info,add=struct.unpack_from('<QQq',d,off+i)
        if info&0xffffffff==8: rel[o]=add
    byadd=collections.defaultdict(list)
    for o,a in rel.items(): byadd[a].append(o)
    tx=S['.text']; tlo,thi=tx[2],tx[2]+tx[4]
    name=(cls).encode()+b'\0'
    ro=S['.rodata']; 
    res=[]
    i=d.find(name, ro[3], ro[3]+ro[4])
    while i!=-1:
        if d[i-1]==0 or True:
            sa=ro[2]+i-ro[3]
            if d[i-1]==0: res.append(sa)
        i=d.find(name,i+1,ro[3]+ro[4])
    for sa in res:
        for o in byadd.get(sa,[]):
            ti=o-8
            for vo in byadd.get(ti,[]):
                ott,=struct.unpack_from('<q',d,v2o(S,vo-8))
                slots=[]; a=vo+8
                while a in rel and tlo<=rel[a]<thi: slots.append(rel[a]); a+=8
                print(cls,'typeinfo',hex(ti),'vtable@',hex(vo+8),'offset_to_top',ott,'nslots',len(slots))
                if ott==0:
                    for k in (12,13,14,15,18,19,20,21,32,33): 
                        if k<len(slots): print('  slot',k,hex(slots[k]))
main(sys.argv[1], sys.argv[2])
