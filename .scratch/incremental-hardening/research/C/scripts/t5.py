import sys; sys.path.insert(0, sys.argv[1])
from elfimg import Img; from builds import *
im=Img(path('1.42.0.0','server')); st=im.strings
a=0x2731360
while im.rel.get(a-0x20) in st and st[im.rel[a-0x20]].startswith('m_'): a-=0x20
print('array start',hex(a), st[im.rel[a]])
refs=[o for o,v in im.rel.items() if v==a]; print('refs to start',[hex(x) for x in refs])
import subprocess
for r in refs: print(subprocess.run([sys.executable,'-I',sys.argv[1]+'/dump.py',sys.argv[1],'1.42.0.0','server',hex(r)],capture_output=True,text=True).stdout)
