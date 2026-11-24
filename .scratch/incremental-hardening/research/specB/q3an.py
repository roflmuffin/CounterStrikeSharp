import pickle, difflib, collections, os
D = pickle.load(open('/home/markus/.claude/jobs/a0ac098e/tmp/specb/q3.pkl', 'rb'))
G = '/home/markus/dev/other/cssharp-hardening-research/scratchpad/G2/cache'
B = ['1.41.8.1', '1.41.8.2', '1.41.9.0', '1.41.9.1a', '1.41.9.1b', '1.41.9.2', '1.41.9.6', '1.41.9.7']
LV = ['raw', 'norm', 'noimm', 'mnem']
glob = {}
for a, b in zip(B, B[1:]):
    p = f'{G}/glob_{a}_{b}.pkl'
    if os.path.exists(p): glob[(a, b)] = pickle.load(open(p, 'rb'))[0]
tot = collections.Counter()
print('== structure (1.41.9.7)')
for c, bb in D.items():
    i = bb['1.41.9.7']
    if not i: print(c, 'NOT FOUND'); continue
    fp = i['fp']
    addr = collections.Counter(x['addr'] for x in fp); nrm = collections.Counter(x['norm'] for x in fp); ni = collections.Counter(x['noimm'] for x in fp)
    print(f"{c}: kind={i['kind']} bases={i['bases'][:3]} vtables={len(i['otts'])} otts={i['otts']} sizes={i['sizes']} | shared-addr {sum(1 for x in fp if addr[x['addr']]>1)}/{len(fp)} shared-norm {sum(1 for x in fp if nrm[x['norm']]>1)} shared-noimm {sum(1 for x in fp if ni[x['noimm']]>1)} maxdup-addr {max(addr.values())} tiny(<=3insn) {sum(1 for x in fp if x.get('ninsn',99)<=3)}")
print('== pairs')
for c, bb in D.items():
    for a, b in zip(B, B[1:]):
        A, Bn = bb[a], bb[b]
        if not A or not Bn: continue
        fa, fb = A['fp'], Bn['fp']
        # ground truth mapping: global matcher if available, else norm alignment
        mp = {}
        how = 'align'
        if (a, b) in glob:
            gm = glob[(a, b)]; how = 'glob'
            pos = collections.defaultdict(list)
            for j, x in enumerate(fb): pos[x['addr']].append(j)
            for i, x in enumerate(fa):
                t = gm.get(x['addr'])
                if t is not None and pos.get(t):
                    cand = pos[t]; mp[i] = i if i in cand else min(cand, key=lambda j: abs(j - i))
        sm = difflib.SequenceMatcher(None, [x['noimm'] for x in fa], [x['noimm'] for x in fb], autojunk=False)
        for op, i1, i2, j1, j2 in sm.get_opcodes():
            if op == 'equal' or (op == 'replace' and i2 - i1 == j2 - j1):
                for k in range(i2 - i1): mp.setdefault(i1 + k, j1 + k)
        moved = [i for i in range(len(fa)) if mp.get(i) is not None and mp[i] != i]
        unk = [i for i in range(len(fa)) if mp.get(i) is None]
        stay = [i for i in range(len(fa)) if mp.get(i) == i]
        row = f'{c:22s} {a}->{b} size {len(fa)}->{len(fb)} moved={len(moved)} unmapped={len(unk)} stay={len(stay)}'
        for lv in LV:
            fpos = sum(1 for i in stay if fa[i][lv] != fb[i][lv])
            tp = sum(1 for i in moved if i < len(fb) and fa[i][lv] != fb[i][lv])
            miss = sum(1 for i in moved if i < len(fb) and fa[i][lv] == fb[i][lv])
            tot[(lv, 'stay')] += len(stay); tot[(lv, 'fp')] += fpos; tot[(lv, 'moved')] += len([i for i in moved if i < len(fb)]); tot[(lv, 'tp')] += tp
            row += f' | {lv}: FP {fpos} TP {tp} miss {miss}'
        if len(fa) != len(fb) or moved or any(fa[i]['raw'] != fb[i]['raw'] for i in stay):
            print(row, how)
        tot['pairs'] += 1; tot['size_changed'] += len(fa) != len(fb)
print('== totals', dict(tot))
for lv in LV:
    print(lv, 'FP rate %.2f%% (%d/%d)' % (100 * tot[(lv, 'fp')] / tot[(lv, 'stay')], tot[(lv, 'fp')], tot[(lv, 'stay')]), 'TP rate %.1f%% (%d/%d)' % (100 * tot[(lv, 'tp')] / max(1, tot[(lv, 'moved')]), tot[(lv, 'tp')], tot[(lv, 'moved')]))
