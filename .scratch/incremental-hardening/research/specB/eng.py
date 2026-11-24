import sys, re, collections
SP = '/home/markus/dev/other/cssharp-hardening-research/scratchpad'
sys.path.insert(0, SP + '/C/scripts')
from elfimg import Img
P = {'97': SP + '/C/dl/b14197/game', '201': SP + '/C/dl/b14201/game'}
L = {'engine': 'bin/linuxsteamrt64/libengine2.so', 'tier0': 'bin/linuxsteamrt64/libtier0.so', 'server': 'csgo/bin/linuxsteamrt64/libserver.so'}
pat = re.compile(sys.argv[1])
for lib in sys.argv[2].split(','):
    ims = {b: Img(f'{P[b]}/{L[lib]}') for b in P}
    for b, im in ims.items():
        names = sorted(n for n in im.ti_by_name if pat.search(n))
        print(lib, b, 'ntypeinfo', len(im.rtti))
        for n in names:
            ti = im.ti_by_name[n]
            vts = im.vtables.get(ti, [])
            print('   ', n, im.rtti[ti]['kind'], [(im.rtti.get(x, {}).get('name'), o) for x, o in im.rtti[ti]['bases']][:3], [(v[1], len(v[2])) for v in vts])
