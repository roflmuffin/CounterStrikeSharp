import pickle, difflib, collections, os
D = pickle.load(open('/home/markus/.claude/jobs/a0ac098e/tmp/specb/q3.pkl', 'rb'))
G = '/home/markus/dev/other/cssharp-hardening-research/scratchpad/G2/cache'
B = ['1.41.8.1', '1.41.8.2', '1.41.9.0', '1.41.9.1a', '1.41.9.1b', '1.41.9.2', '1.41.9.6', '1.41.9.7']
# hooked/called slot indices valid at 1.41.9.7
H = {'CSource2Server': {'PreWorldUpdate': 15, 'GameFrame': 19, 'ServerHibernationUpdate': 22, 'IsPaused': 31, 'SteamAPIActivated': 41, 'SteamAPIDeactivated': 42, 'OnHostNameChanged': 43, 'PreFatalShutdown': 44, 'UpdateWhenNotInGame': 45, 'GetEconItemSystem': 46},
     'CSource2GameClients': {'ClientConnect': 12, 'ClientPutInServer': 13, 'ClientDisconnect': 16, 'ClientCommand': 17, 'ClientVoice': 27, 'ClientSvcUserMessage': 38},
     'CGameEventManager': {'LoadEventsFromFile': 2, 'AddListener': 4, 'FindListener': 5, 'RemoveListener': 6, 'CreateEvent': 7, 'FireEvent': 8, 'FreeEvent': 11},
     'CSource2GameEntities': {'CheckTransmit': 13},
     'CCSPlayerController': {'ChangeTeam': 104, 'Respawn': 276}, 'CCSPlayer_ItemServices': {'GiveNamedItem': 24, 'DropActivePlayerWeapon': 27, 'RemoveWeapons': 28},
     'CGameSceneNode': {'GetSkeletonInstance': 13}, 'CCSGameRules': {'FindPickerEntity': 26}, 'CBaseEntity': {'IsPlayerPawn': 170, 'Teleport': 164}, 'CBasePlayerPawn': {'CommitSuicide': 387}}
a, b = '1.41.8.1', '1.41.8.2'
gm = pickle.load(open(f'{G}/glob_{a}_{b}.pkl', 'rb'))[0]
for c in ['CSource2Server', 'CCSGameRules']:
    fa, fb = D[c][a]['fp'], D[c][b]['fp']
    sm = difflib.SequenceMatcher(None, [x['noimm'] for x in fa], [x['noimm'] for x in fb], autojunk=False)
    print(c, [(op, i1, i2, j1, j2) for op, i1, i2, j1, j2 in sm.get_opcodes() if op != 'equal'])
    pos = {x['addr']: j for j, x in enumerate(fb)}
    print('  glob map old->new idx (only moved):', [(i, pos.get(gm.get(x['addr']))) for i, x in enumerate(fa) if pos.get(gm.get(x['addr'])) not in (i, None)][:12])
# per hooked slot, per pair: map new idx back to old via alignment; report fp equality
lv = 'noimm'
cnt = collections.Counter()
for c, slots in H.items():
    for a, b in zip(B, B[1:]):
        fa, fb = D[c][a]['fp'], D[c][b]['fp']
        sm = difflib.SequenceMatcher(None, [x[lv] for x in fa], [x[lv] for x in fb], autojunk=False)
        back = {}
        for op, i1, i2, j1, j2 in sm.get_opcodes():
            if op == 'equal' or (op == 'replace' and i2 - i1 == j2 - j1):
                for k in range(j2 - j1): back[j1 + k] = i1 + k
        for n, j in slots.items():
            # j valid at 1.41.9.7; sizes stable from 8.2 on, so for pair 8.1->8.2 j is new idx
            i = back.get(j)
            moved = i is not None and i != j
            same_at_j = j < len(fa) and fa[j][lv] == fb[j][lv]
            same_raw = j < len(fa) and fa[j]['raw'] == fb[j]['raw']
            same_norm = j < len(fa) and fa[j]['norm'] == fb[j]['norm']
            if moved:
                cnt['moved'] += 1; cnt['moved_detected_noimm'] += (not same_at_j)
                print(f'  MOVED {c}.{n}: old idx {i} -> new {j} ({a}->{b}); fp@{j} changed={not same_at_j}')
            else:
                cnt['stay'] += 1; cnt['stay_fp_noimm'] += (not same_at_j); cnt['stay_fp_norm'] += (not same_norm); cnt['stay_fp_raw'] += (not same_raw)
                if not same_at_j: print(f'  FP(noimm) {c}.{n} idx {j} {a}->{b}')
            # neighbour uniqueness
dup = collections.Counter()
for c, slots in H.items():
    fp = D[c]['1.41.9.7']['fp']; nc = collections.Counter(x['noimm'] for x in fp); ac = collections.Counter(x['addr'] for x in fp)
    for n, j in slots.items():
        dup['n'] += 1; dup['shared_body'] += nc[fp[j]['noimm']] > 1; dup['shared_addr'] += ac[fp[j]['addr']] > 1
        if nc[fp[j]['noimm']] > 1: print('  shared-body hooked slot', c, n, j, 'x', nc[fp[j]['noimm']], 'ninsn', fp[j].get('ninsn'), 'neighbour-identical', (j > 0 and fp[j - 1]['noimm'] == fp[j]['noimm']) or (j + 1 < len(fp) and fp[j + 1]['noimm'] == fp[j]['noimm']))
print(dict(cnt)); print(dict(dup))
