import sys, os, pickle, time
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from g2img import ImgFlat, C
import fp, funcmap
G = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

class M2(fp.Mod):
    def __init__(self, build):
        self.build, self.lib = build, 'server'
        self.im = ImgFlat(f'{G}/img/server_{build}.img')
        self.fm = funcmap.FM(self.im, f'{G}/cache/fm_{build}.pkl')
        self._fp = {}
        self.refs_by_insn = {a: t for a, t, m in self.fm.refs}
        self.calls_sorted = sorted(self.fm.calls)
        self.refs_sorted = sorted(self.fm.refs)
        self.call_addrs = [c[0] for c in self.calls_sorted]
        self.ref_addrs = [r[0] for r in self.refs_sorted]

    def all_fps(self):
        cache = f'{G}/cache/fps_{self.build}.pkl'
        if os.path.exists(cache):
            self._fp = pickle.load(open(cache, 'rb')); return self._fp
        for f in self.fm.starts:
            if self.im.in_text(f):
                self.fp(f)
        pickle.dump(self._fp, open(cache, 'wb'))
        return self._fp

if __name__ == '__main__':
    t = time.time(); m = M2(sys.argv[1]); print(sys.argv[1], 'fm', len(m.fm.starts), time.time() - t, flush=True)
    m.all_fps(); print(sys.argv[1], 'fps', len(m._fp), time.time() - t)
