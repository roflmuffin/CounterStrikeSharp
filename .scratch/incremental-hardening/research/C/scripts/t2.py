import sys; sys.path.insert(0, sys.argv[1])
from elfimg import Img; from builds import *
for lib in ['server','engine','schemasystem','tier0']:
    im=Img(path('1.42.0.0',lib))
    cov=sum(n for s,n in im.funcs); a,buf=im.sec('.text')
    print(lib,'FDEs',len(im.funcs),'text',len(buf),'covered %.1f%%'%(100*cov/len(buf)),'push rbp;mov rbp,rsp:',buf.count(b'\x55\x48\x89\xe5'), 'typeinfos',len(im.rtti),'vtables',sum(len(v) for v in im.vtables.values()))
