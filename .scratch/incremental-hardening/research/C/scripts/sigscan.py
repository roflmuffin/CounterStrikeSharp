"""Scan CSSharp linux signatures (current + historical gamedata) against downloaded builds."""
import sys, os, json, glob, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from elfimg import Img
from builds import *

LIBMAP = {'server': 'server', 'engine': 'engine'}
imgs = {}


def img(b, lib):
    k = (b, lib)
    if k not in imgs:
        imgs[k] = Img(path(b, lib))
    return imgs[k]


def load_sigs(fn):
    try:
        d = json.load(open(fn))
    except Exception:
        return {}
    out = {}
    for k, v in d.items():
        s = v.get('signatures')
        if s and s.get('linux'):
            out[k] = (s.get('library', 'server'), s['linux'])
    return out


if __name__ == '__main__':
    builds = list(BUILDS)
    gd = sorted(glob.glob(S + '/out/gd/*.json'))
    # distinct (name, sig) pairs across history, first/last commit date seen
    hist = {}
    for fn in gd:
        date = os.path.basename(fn).split('_')[0]
        for k, (lib, sig) in load_sigs(fn).items():
            e = hist.setdefault((k, sig), dict(lib=lib, first=date, last=date))
            e['first'] = min(e['first'], date); e['last'] = max(e['last'], date)
    head = load_sigs('/home/markus/dev/other/CounterStrikeSharp/configs/addons/counterstrikesharp/gamedata/gamedata.json')
    res = {}
    for (k, sig), e in hist.items():
        lib = LIBMAP.get(e['lib'], e['lib'])
        r = {}
        for b in builds:
            r[b] = img(b, lib).scan(sig, limit=10)
        res[(k, sig)] = dict(e, matches=r, head=(head.get(k, (None, None))[1] == sig))
    pickle.dump(res, open(S + '/cache/sigscan.pkl', 'wb'))
    print('HEAD signatures:')
    print(f"{'name':42s} " + ' '.join(f'{b:>9s}' for b in builds))
    for (k, sig), r in sorted(res.items()):
        if r['head']:
            print(f"{k:42s} " + ' '.join(f"{len(r['matches'][b]):>9d}" for b in builds), ' ', [hex(x) for x in r['matches']['1.42.0.0'][:2]])
    print('\nhistorical distinct sigs:', len(res))
    for b in builds:
        alive = [x for x in res.values() if len(x['matches'][b]) >= 1]
        uniq = [x for x in res.values() if len(x['matches'][b]) == 1]
        print(b, 'matching>=1:', len(alive), 'unique:', len(uniq))
