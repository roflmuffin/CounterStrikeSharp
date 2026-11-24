"""Schema offset churn across tracker builds (third-party runtime dumps; validated vs own static extraction on 26798866)."""
import sys, os, json, glob, collections
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from builds import S
D = S + '/dl/gh'

KEYC = ['CBaseEntity', 'CCSPlayerPawn', 'CCSPlayerController', 'CBasePlayerPawn', 'CCSPlayerPawnBase', 'CBaseModelEntity',
        'CCSGameRules', 'CBasePlayerController', 'CCSWeaponBase', 'CBasePlayerWeapon', 'CCSPlayer_ItemServices', 'CGameSceneNode']


def load(b):
    try:
        es = json.load(open(f'{D}/es_{b}.json'))
    except Exception:
        return None
    out = {}
    for c in es['classes']:
        if c['module'] != 'libserver.so':
            continue
        out[c['name']] = (int(c['size']), {f['name']: int(f['offset']) for f in c['fields']})
    return out


def prov(b):
    try:
        p = json.load(open(f'{D}/prov_{b}.json'))
        for i in p['inputs']:
            if i['path'].endswith('libserver.so'):
                return i['sha256']
    except Exception:
        return None


builds = sorted(int(os.path.basename(f)[3:-5]) for f in glob.glob(D + '/es_*.json'))
prev = None; pb = None
tot = collections.Counter()
lines = []
key_changes = collections.Counter()
for b in builds:
    cur = load(b)
    if cur is None or not cur:
        continue
    if prev is not None:
        common = set(prev) & set(cur)
        fch = fall = csz = 0
        kc = []
        for c in common:
            s0, f0 = prev[c]; s1, f1 = cur[c]
            csz += s0 != s1
            for f, o in f0.items():
                if f in f1:
                    fall += 1
                    if f1[f] != o:
                        fch += 1
                        if c in KEYC:
                            kc.append(f'{c}.{f}')
        added = len(set(cur) - set(prev)); removed = len(set(prev) - set(cur))
        same_bin = prov(b) == prov(pb) and prov(b) is not None
        lines.append((pb, b, len(cur), fall, fch, csz, added, removed, len(kc), same_bin))
        tot['pairs'] += 1
        if fch:
            tot['pairs_with_offset_change'] += 1
        if kc:
            tot['pairs_with_keyclass_offset_change'] += 1
        if not same_bin:
            tot['pairs_libserver_changed'] += 1
            if fch:
                tot['libserver_changed_and_offsets_changed'] += 1
        for k in kc:
            key_changes[k.split('.')[0]] += 1
    prev, pb = cur, b
print('prev_build build classes common_fields offset_changed class_size_changed classes_added classes_removed keyclass_field_changes libserver_same_sha')
for l in lines:
    if l[4] or l[5] or l[6] or l[7]:
        print(*l)
print(dict(tot))
print('key-class field-offset change events by class:', dict(key_changes))
print('builds analysed', len(builds), builds[0], builds[-1])
