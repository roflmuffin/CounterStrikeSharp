import sys, re, collections, difflib
SP = '/home/markus/dev/other/cssharp-hardening-research/scratchpad'
sys.path.insert(0, SP + '/C/scripts')
from elfimg import Img
import capstone
md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
RIP = re.compile(r'\[rip [+-] 0x[0-9a-f]+\]'); HEX = re.compile(r'0x[0-9a-f]+|\b\d+\b')
P = {'1.41.9.7': SP + '/C/dl/b14197/game', '1.42.0.1(14201)': SP + '/C/dl/b14201/game'}
L = {'engine': 'bin/linuxsteamrt64/libengine2.so', 'tier0': 'bin/linuxsteamrt64/libtier0.so', 'server': 'csgo/bin/linuxsteamrt64/libserver.so'}
CL = {'engine': ['CEngineServer', 'CEngineServiceMgr', 'CNetworkServerService', 'CGameEventSystem', 'CNetworkGameServer'], 'tier0': ['CCvar'],
      'server': ['CSource2Server', 'CSource2GameClients', 'CSource2GameEntities', 'CGameEventManager', 'CCSPlayerController', 'CBaseEntity', 'CBasePlayerPawn', 'CCSPlayer_ItemServices', 'CCSGameRules', 'CGameSceneNode']}
def fp(im, f, mode):
    if isinstance(f, str): return f
    b = im.read(f, 600) or b''
    out = []
    for a, s, m, ops in md.disasm_lite(b, f):
        if mode == 'raw': out.append(m + ' ' + ops)
        else:
            o = RIP.sub('[M]', ops)
            if m == 'call' or m.startswith('j'): o = 'T' if not o.startswith('q') else o
            if mode == 'noimm': o = HEX.sub('I', o)
            out.append(m + ' ' + o)
        if m in ('ret', 'jmp') or m.startswith('ud') or len(out) > 60: break
    return '\n'.join(out)
for lib, cls in CL.items():
    a, b = [Img(f'{P[k]}/{L[lib]}') for k in P]
    for c in cls:
        va, vb = a.primary_vtable(c), b.primary_vtable(c)
        if not va or not vb: print(lib, c, 'missing'); continue
        sa, sb = va[2], vb[2]
        res = {}
        for mode in ('raw', 'mask', 'noimm'):
            fa = [fp(a, f, mode) for f in sa]; fb = [fp(b, f, mode) for f in sb]
            sm = difflib.SequenceMatcher(None, fa, fb, autojunk=False)
            ops = [(o, i1, i2, j1, j2) for o, i1, i2, j1, j2 in sm.get_opcodes() if o != 'equal']
            res[mode] = (sum(1 for i in range(min(len(fa), len(fb))) if fa[i] != fb[i]), ops[:5])
            if mode == 'noimm':
                cnt = collections.Counter(fa); addr = collections.Counter(sa)
                shared = sum(1 for x in fa if cnt[x] > 1); sh_addr = sum(1 for x in sa if addr[x] > 1)
        print(f'{lib} {c}: size {len(sa)}->{len(sb)} | slots-with-changed-fp-at-same-idx raw {res["raw"][0]} mask {res["mask"][0]} noimm {res["noimm"][0]} | noimm ops {res["noimm"][1]} | shared-body(noimm) {shared}/{len(sa)} shared-addr {sh_addr}')
