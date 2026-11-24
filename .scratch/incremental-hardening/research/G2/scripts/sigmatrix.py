import sys, os, json, glob, pickle
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from g2img import ImgFlat
G = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
B = ['1.41.8.1', '1.41.8.2', '1.41.9.0', '1.41.9.1a', '1.41.9.1b', '1.41.9.2', '1.41.9.6', '1.41.9.7']
ims = {b: ImgFlat(f'{G}/img/server_{b}.img') for b in B}
sigs = {}
for fn in sorted(glob.glob(f'{G}/gd/*.json')):
    for k, v in json.load(open(fn)).items():
        s = v.get('signatures', {})
        if s.get('linux') and s.get('library', 'server') == 'server':
            sigs.setdefault((k, s['linux']), []).append(os.path.basename(fn)[:-5])
res = {}
for (k, s), src in sorted(sigs.items()):
    res[(k, s)] = {b: ims[b].scan(s, limit=20) for b in B}
    print(f'{k:40s} {",".join(src):60s}', ' '.join(f'{len(res[(k,s)][b]):2d}' for b in B))
pickle.dump(res, open(f'{G}/cache/sigmatrix.pkl', 'wb'))
