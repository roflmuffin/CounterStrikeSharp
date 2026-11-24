import sys; sys.path.insert(0, sys.argv[1])
from fp import Mod
A=Mod('1.42.0.0','server'); im=A.im
st=im.strings
inv={}
for a,s in st.items(): inv.setdefault(s,[]).append(a)
datarefs={}
for o,v in im.rel.items(): datarefs.setdefault(v,[]).append(o)
coderefs={}
for a,t,m in A.fm.refs: coderefs.setdefault(t,[]).append((a,m))
for s in ['m_iHealth','m_pWeaponServices','CCSPlayerPawn','C_CSPlayerPawn','m_hPawn','player_death','weapon_ak47','prop_physics','mp_restartgame','sv_cheats','InputSetHealth','SetHealth','CBaseEntity','info_player_terrorist','m_vecAbsOrigin','m_flSimulationTime']:
    for a in inv.get(s,[])[:2]:
        print(f'{s!r:28s} str@{hex(a)} code-xrefs={[(hex(x),m) for x,m in coderefs.get(a,[])[:3]]} n={len(coderefs.get(a,[]))} data-xrefs={[hex(x) for x in datarefs.get(a,[])[:3]]} n={len(datarefs.get(a,[]))}')
