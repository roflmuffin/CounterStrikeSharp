"""Compare primary vtables per RTTI class between two builds."""
import sys, os, difflib, collections, json
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from fp import Mod

KEY = ['CCSPlayerPawn', 'CCSPlayerController', 'CBaseEntity', 'CCSGameRules', 'CBasePlayerPawn', 'CCSPlayerPawnBase',
       'CCSPlayer_ItemServices', 'CCSPlayer_WeaponServices', 'CBaseTrigger', 'CGameSceneNode', 'CBaseModelEntity',
       'CBasePlayerController', 'CSource2GameEntities', 'CEntityInstance', 'CCSWeaponBase', 'CBasePlayerWeapon',
       'CSource2GameClients', 'CSource2Server', 'CGameEventManager', 'CTriggerMultiple', 'CPointEntity']
# CSSharp linux offsets that are vtable indices
OFFS = {'CCSPlayerController_ChangeTeam': ('CCSPlayerController', 104), 'CCSPlayerController_Respawn': ('CCSPlayerController', 276),
        'CCSPlayer_ItemServices_GiveNamedItem': ('CCSPlayer_ItemServices', 24), 'CCSPlayer_ItemServices_DropActivePlayerWeapon': ('CCSPlayer_ItemServices', 27),
        'CCSPlayer_ItemServices_RemoveWeapons': ('CCSPlayer_ItemServices', 28), 'CGameSceneNode_GetSkeletonInstance': ('CGameSceneNode', 13),
        'CCSGameRules_FindPickerEntity': ('CCSGameRules', 26), 'CBaseEntity_IsPlayerPawn': ('CBaseEntity', 170), 'CBaseEntity_Teleport': ('CBaseEntity', 164),
        'CBasePlayerPawn_CommitSuicide': ('CBasePlayerPawn', 387), 'ISource2GameEntities::CheckTransmit': ('CSource2GameEntities', 13)}


def vt(m, cls):
    p = m.im.primary_vtable(cls)
    return p[2] if p else None


def hashes(m, fns):
    return [m.fp(f)['norm'] if isinstance(f, int) else f for f in fns]


def main(b1, b2, lib='server', full=True):
    A, B = Mod(b1, lib), Mod(b2, lib)
    print(f'=== {lib}: {b1} -> {b2}')
    print('key classes: size old->new, first changed index, alignment ops')
    for c in KEY:
        va, vb = vt(A, c), vt(B, c)
        if not va or not vb:
            print(f'  {c}: missing ({bool(va)},{bool(vb)})'); continue
        ha, hb = hashes(A, va), hashes(B, vb)
        sm = difflib.SequenceMatcher(None, ha, hb, autojunk=False)
        ops = [(t, i1, i2, j1, j2) for t, i1, i2, j1, j2 in sm.get_opcodes() if t != 'equal']
        ins = [o for o in ops if o[0] in ('insert', 'delete') or (o[0] == 'replace' and (o[2] - o[1]) != (o[4] - o[3]))]
        print(f'  {c:28s} {len(va):4d}->{len(vb):4d}  changed-slots={sum(max(o[2]-o[1],o[4]-o[3]) for o in ops):3d}  shifts={[(o[0],o[1],o[2],o[3],o[4]) for o in ins][:4]}')
    print('CSSharp vtable offsets: same function at same index?')
    for k, (c, i) in OFFS.items():
        va, vb = vt(A, c), vt(B, c)
        if not va or not vb or i >= len(va) or i >= len(vb):
            print(f'  {k}: n/a'); continue
        fa, fb = va[i], vb[i]
        sa, sb = A.strrefs(fa) if isinstance(fa, int) else [], B.strrefs(fb) if isinstance(fb, int) else []
        same = hashes(A, [fa]) == hashes(B, [fb])
        # where did old function go?
        hb = hashes(B, vb); ha0 = hashes(A, [fa])[0]
        newidx = [j for j, h in enumerate(hb) if h == ha0]
        print(f'  {k:46s} idx {i}: {"SAME" if same else "DIFF"} old={hex(fa) if isinstance(fa,int) else fa} new={hex(fb) if isinstance(fb,int) else fb} old-fn-now-at={newidx[:3]} strs={sa[:2]}')
    if not full:
        return
    names = set(A.im.ti_by_name) & set(B.im.ti_by_name)
    stats = collections.Counter()
    sizech = []
    for n in names:
        va, vb = vt(A, n), vt(B, n)
        if not va or not vb:
            continue
        stats['both'] += 1
        if len(va) != len(vb):
            stats['size_changed'] += 1; sizech.append((n, len(va), len(vb)))
        ha, hb = hashes(A, va), hashes(B, vb)
        if ha != hb:
            stats['any_slot_changed'] += 1
            sm = difflib.SequenceMatcher(None, ha, hb, autojunk=False)
            if any(t in ('insert', 'delete') or (t == 'replace' and i2 - i1 != j2 - j1) for t, i1, i2, j1, j2 in sm.get_opcodes()):
                stats['index_shift'] += 1
    stats['classes_only_old'] = len(set(A.im.ti_by_name) - set(B.im.ti_by_name))
    stats['classes_only_new'] = len(set(B.im.ti_by_name) - set(A.im.ti_by_name))
    print('ALL classes:', dict(stats))
    sizech.sort(key=lambda x: -abs(x[2] - x[1]))
    print('size changes (top 40):', sizech[:40])
    print('new classes sample:', sorted(set(B.im.ti_by_name) - set(A.im.ti_by_name))[:30])
    print('removed classes sample:', sorted(set(A.im.ti_by_name) - set(B.im.ti_by_name))[:30])


if __name__ == '__main__':
    main(sys.argv[1], sys.argv[2], sys.argv[3] if len(sys.argv) > 3 else 'server')
