import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from g2img import ImgFlat
import capstone
G = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
b, a, n = sys.argv[1], int(sys.argv[2], 16), int(sys.argv[3])
im = ImgFlat(f'{G}/img/server_{b}.img')
md = capstone.Cs(capstone.CS_ARCH_X86, capstone.CS_MODE_64)
for i, (ad, sz, mn, op) in enumerate(md.disasm_lite(im.read(a, n * 8), a)):
    if i >= n: break
    print(hex(ad), mn, op)
