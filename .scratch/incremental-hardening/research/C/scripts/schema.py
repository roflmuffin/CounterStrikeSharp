"""Static extraction of schema class/field tables from libserver.so (no game run needed)."""
import sys, os, struct, json
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from elfimg import Img
from builds import *


def extract(im):
    st = im.strings
    out = {}
    for o, v in im.rel.items():
        # candidate: o = classinfo+0x30 (fields ptr), name ptr at classinfo+8
        nm = im.rel.get(o - 0x28)
        if nm not in st:
            continue
        cname = st[nm]
        if not cname or not (cname[0].isalpha() or cname[0] == '_'):
            continue
        b = im.read(o - 0x10, 8)
        if not b:
            continue
        size, nfields = struct.unpack_from('<IH', b)
        if nfields == 0 or nfields > 2000:
            continue
        fields = {}
        ok = True
        for i in range(nfields):
            e = v + 0x20 * i
            fn = im.rel.get(e)
            if fn not in st:
                ok = False; break
            off = im.u32(e + 0x10)
            fields[st[fn]] = off
        if not ok:
            continue
        if cname in out and out[cname]['fields'] != fields:
            cname = cname + '@' + hex(o)
        out[cname] = dict(size=size, fields=fields, addr=o - 0x30)
    return out


if __name__ == '__main__':
    res = {}
    for b in BUILDS:
        im = Img(path(b, 'server'))
        res[b] = extract(im)
        print(b, 'classes', len(res[b]), 'fields', sum(len(c['fields']) for c in res[b].values()))
    json.dump(res, open(S + '/cache/schema_static.json', 'w'))
    for c in ['CBaseEntity', 'CCSPlayerPawn', 'CCSPlayerController', 'CCSPlayerPawnBase', 'CCSGameRules']:
        x = res['1.42.0.0'].get(c)
        if x:
            print(c, hex(x['addr']), 'size', hex(x['size']), 'nfields', len(x['fields']), list(x['fields'].items())[:4])
