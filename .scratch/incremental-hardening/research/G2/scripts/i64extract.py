"""Extract loaded-segment bytes from an IDA 9.1+ (v910, uncompressed inline) .i64 into a flat VA image.
Output: <out>.img (bytes indexed by VA from 0 to max end; unloaded = 0) and <out>.json (segments, ivl coverage)."""
import sys, struct, json
src, out = sys.argv[1], sys.argv[2]
f = open(src, 'rb')
h = f.read(0x100)
assert h[:4] == b'IDA2'
header_size, data_start = struct.unpack_from('<QQ', h, 6)
sig, ver = struct.unpack_from('<IH', h, 26)
assert sig == 0xAABBCCDD and ver == 910, (hex(sig), ver)
comp = h[32]
sizes = struct.unpack_from('<6Q', h, 33)
assert comp == 0, 'compressed idb not supported (%d)' % comp
id0, id1 = sizes[0], sizes[1]
f.seek(data_start + id0)
page = f.read(0x2000)
assert page[:4] == b'VA*\x00', page[:4]
a3, nseg, a2048, npages = struct.unpack_from('<IIII', page, 4)
assert a3 == 3 and a2048 == 2048
segs = [struct.unpack_from('<QQ', page, 20 + 16 * i) for i in range(nseg)]
maxend = max(e for s, e in segs)
img = bytearray(maxend)
info = []
for s, e in segs:
    raw = f.read((e - s) * 4)
    img[s:e] = raw[0::4]
    ivl = raw[1::4].translate(bytes((b & 1) for b in range(256)))
    info.append(dict(start=s, end=e, ivl=ivl.count(1)))
open(out + '.img', 'wb').write(img)
json.dump(dict(segs=info, sizes=sizes), open(out + '.json', 'w'), indent=0)
print(nseg, 'segments, image', hex(maxend))
for i in info:
    print(hex(i['start']), hex(i['end']), i['end'] - i['start'] - i['ivl'], 'bytes w/o value')
