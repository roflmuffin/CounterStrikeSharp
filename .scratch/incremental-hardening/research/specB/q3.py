import sys, os, re, pickle, collections, difflib, json
SP = '/home/markus/dev/other/cssharp-hardening-research/scratchpad'
sys.path.insert(0, SP + '/C/scripts'); sys.path.insert(0, SP + '/G2/scripts')
import elfimg  # noqa
from g2mod import M2
import capstone
md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
HEX = re.compile(r'0x[0-9a-f]+|\b\d+\b')
RIP = re.compile(r'\[rip [+-] 0x[0-9a-f]+\]')
BUILDS = ['1.41.8.1', '1.41.8.2', '1.41.9.0', '1.41.9.1a', '1.41.9.1b', '1.41.9.2', '1.41.9.6', '1.41.9.7']
CLS = ['CSource2Server', 'CSource2GameClients', 'CSource2GameEntities', 'CGameEventManager', 'CNavPhysicsInterface',
       'CCSPlayerController', 'CBasePlayerPawn', 'CCSPlayerPawn', 'CCSPlayer_ItemServices', 'CBaseEntity', 'CCSGameRules', 'CGameSceneNode']
mods = {}
def mod(b):
    if b not in mods:
        m = M2(b); m.all_fps(); mods[b] = m
    return mods[b]
def noimm(m, f):
    if isinstance(f, str): return f
    e = min(m.end(f), f + 0x4000)
    b = m.im.read(f, e - f)
    out = []
    for a, s, mn, ops in md.disasm_lite(b, f):
        if mn in ('nop', 'int3'): continue
        ops = RIP.sub('[M]', ops); ops = HEX.sub('I', ops)
        out.append(mn + ' ' + ops)
    return '\n'.join(out)
def fps(m, f):
    if isinstance(f, str): return dict(addr=f, raw=f, norm=f, mnem=f, noimm=f)
    x = m._fp.get(f) or m.fp(f)
    return dict(addr=f, raw=x['raw'], norm=x['norm'], mnem=x['mnem'], noimm=noimm(m, f), ninsn=x['ninsn'])
def info(b, c):
    m = mod(b)
    ti = m.im.ti_by_name.get(f'{len(c)}{c}')
    if ti is None: return None
    vts = m.im.vtables.get(ti, [])
    prim = [v for v in vts if v[1] == 0]
    return dict(n_vtables=len(vts), otts=[v[1] for v in vts], sizes=[len(v[2]) for v in vts], slots=prim[0][2] if prim else None, kind=m.im.rtti[ti]['kind'], bases=[(m.im.rtti.get(bb, {}).get('name'), o) for bb, o in m.im.rtti[ti]['bases']])
out = {}
for c in CLS:
    out[c] = {}
    for b in BUILDS:
        i = info(b, c)
        if not i or not i['slots']:
            out[c][b] = None; continue
        m = mod(b)
        i['fp'] = [fps(m, f) for f in i['slots']]
        out[c][b] = i
    print(c, 'done', flush=True)
pickle.dump(out, open('/home/markus/.claude/jobs/a0ac098e/tmp/specb/q3.pkl', 'wb'))
