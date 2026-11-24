"""Scan other frameworks' linux signatures (CS2Fixes, SwiftlyS2) across builds."""
import sys, os, re, json, collections
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from sigscan import img
from builds import *

LIB = {'server': 'server', 'engine': 'engine', 'engine2': 'engine', 'tier0': 'tier0', 'schemasystem': 'schemasystem'}


def load_jsonc(p):
    t = open(p).read()
    t = re.sub(r'^\s*//.*$', '', t, flags=re.M)
    t = re.sub(r'(?<!:)//[^"\n]*$', '', t, flags=re.M)
    t = re.sub(r',(\s*[}\]])', r'\1', t)
    return json.loads(t)


def sigs(d):
    out = {}
    def walk(x, name=None):
        if isinstance(x, dict):
            if 'linux' in x and isinstance(x['linux'], str) and ('lib' in x or 'library' in x):
                out[name] = (x.get('library') or x.get('lib'), x['linux'])
            for k, v in x.items():
                walk(v, k)
    walk(d)
    return out


if __name__ == '__main__':
    tot = collections.Counter()
    for f in sys.argv[1:]:
        d = sigs(load_jsonc(f))
        print('==', os.path.basename(f), len(d), 'linux sigs')
        for name, (lib, sig) in sorted(d.items()):
            l = LIB.get(lib)
            if not l:
                print(f'  {name}: lib {lib} not downloaded'); continue
            r = {b: len(img(b, l).scan(sig, limit=5)) for b in BUILDS}
            key = tuple(r.values())
            tot[(os.path.basename(f), key)] += 1
            if len(set(key)) > 1 or key[0] != 1:
                print(f'  {name:50s} {r}')
    print('summary (matches 1.41.9.7, 1.42.0.0, 14201) -> count')
    for k, v in sorted(tot.items()):
        print(' ', k, v)
