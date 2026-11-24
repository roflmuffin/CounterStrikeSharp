# Backtest model: incidents x scenarios. Outcomes: A avoided, D data-only, S softer (code but server up), U unchanged
import sys
# id, date, build, items, impact(C crash, L load-fail/outage, F feature, ?), ttf_first_h, ttf_last_h, commits[(hash,kind,isFollowup)], T1,T2,Ideal, I1flag
# commit kind: S=gamedata sigs(+mixed), O=gamedata offsets only, V=SDK bump for vtables only, G=SDK bump incl IGameSystem impl, C=code (M/T/R/N/L)
# commit outcome per scenario (T1,T2,Ideal)
CK={'S':('D','D','D'),'S1':('D','D*','D*'),'O':('D','A','A'),'V':('D','A','A'),'G':('C','C','A'),
    'T':('C','C','D'),'N':('C','C','A'),'C':('C','C','C'),'CD':('D','D','D'),'CI':('C','C','D')}
I=[
(1,'2023-11-02','1.39.6.6','S2 O1','F',125,125,[('c01357e8','S',0)],'D','D','D',0),
(2,'2023-11-09','1.39.6.8-7.2','S5 O1 A','F',147,288,[('0ddf6bcd','S1',0),('0f72631e','O',1),('6147739c','S',1)],'D','D','D',1),
(3,'2023-11-21','1.39.7.3','S2 O1','F',174,174,[('e12a7cb1','S',0)],'D','D','D',0),
(4,'2023-12-14','1.39.8.1','O3','F',4.6,4.6,[('74fd0e08','O',0)],'D','A','A',0),
(5,'2024-01-19','1.39.8.5','O1','F',41.5,41.5,[('e5c22369','O',0)],'D','A','A',0),
(6,'2024-02-07','1.39.8.6 CtA','Lmm S11 A O4 T M','L',29.5,73,[('f2b8044b','C',0),('12c6f4d0','S',1),('8967c40b','S',1)],'U','U','S',1),
(7,'2024-02-28','1.39.9.5','O1','F',3.5,3.5,[('983d9149','O',0)],'D','A','A',0),
(8,'2024-04-25','1.40.0.4','S4 N V','F',6.9,120,[('7cae4be9','N',0),('14224274','S1',1),('9a221b4e','V',1)],'S','S','D',0),
(9,'2024-05-23','1.40.0.9','S6 O4 M V G N','C',35,169,[('cafc4e23','C',0),('3240a5e5','S1',1),('13ec19e4','S1',1),('eea64519','S1',1)],'S','S','S',0),
(10,'2024-06-11','1.40.1.4','S1 O1','F',36,37,[('54cc93e0','S1',0),('2eaf7c2d','O',1)],'D','D','D',0),
(11,'2024-07-02','1.40.2.0','S1','F',15,15,[('f8451c28','S1',0)],'D','D','D',0),
(12,'2024-08-09','1.40.2.5','S4','F',29,29,[('aa40d81a','S',0)],'D','D','D',0),
(13,'2024-08-14','1.40.2.6','S1','F',101,101,[('a82faeb5','S1',0)],'D','D','D',0),
(14,'2024-10-02','1.40.3.1 Armory','L S9 O7 V N','C',38.7,76,[('2c7f8961','S',0),('8a795de9','S1',1)],'U','U','D',0),
(15,'2024-10-15','1.40.3.7','S3 O1','F',73.5,121,[('761380df','S',0),('b54f5c3d','S',1)],'D','D','D',0),
(16,'2024-12-19','1.40.5.7','S2 T','L',12.8,13.3,[('461fc0ea','S',0),('ba860a92','T',1)],'U','U','D',0),
(17,'2025-03-31','1.40.7.3','S2','F',3,74,[('0ce4a290','S1',0),('68e6ffae','S1',1)],'D','D','D',0),
(18,'2025-05-07','1.40.7.5','R T(ConVar)','?',51.5,51.5,[('462ca522','CI',0)],'U','U','D',0),
(19,'2025-07-02','1.40.8.5','S2 A','C',32,98,[('bebeedf2','S',0),('018bcba6','S1',1)],'D','D','D',1),
(20,'2025-07-28','1.40.8.9 P19388062','Lmm S26 A O7 T M R V G N','L',146,157,[('b7abd1d5','S',0),('1f30e561','C',0),('356152a5','C',1)],'U','U','S',1),
(21,'2025-08-14','1.40.9.5 P19602992','V G S3 O5 T','C',4,148,[('f7c69a60','G',0),('bf1ce2e8','S',1),('624ca0f0','T',1),('936b88d5','S1',1)],'S','S','D',0),
(22,'2025-08-26','1.40.9.7','O2','F',108,108,[('72d66cb5','S',0)],'D','A','A',0),
(23,'2025-09-03','1.40.9.9','O2','F',1.3,1.3,[('54f8d5ef','O',0)],'D','A','A',0),
(24,'2025-09-16','1.41.0.4 P20022951','V G S6 O3 M','C?',31.5,31.5,[('2c809713','C',0)],'S','S','S',0),
(25,'2025-10-14','1.41.1.3','S3 O3 T M N','L',32,32,[('37951f28','C',0),('34598dd5','C',1)],'U','U','S',0),
(26,'2025-10-29','1.41.1.9','S1','F',2,2,[('b4e83dfb','S1',0)],'D','D','D',0),
(27,'2025-11-12','1.41.2.2','S1','C',52,52,[('6461235c','S1',0)],'D','D','D',0),
(28,'2025-12-06','1.41.2.6','G V(SDK-only)','C',23,23,[('ad731c16','G',0)],'S','S','A',0),
(29,'2026-01-21','1.41.3.0','S7 A O10 V G T N','L',35,78,[('f9617b9a','N',0),('fe3e32f3','CI',0),('32628275','O',1),('9841c92e','S',1)],'U','U','D',1),
(30,'2026-02-23','1.41.3.6','S1 A','F',51,51,[('4006494c','S1',0)],'D','D','D',1),
(31,'2026-03-12','1.41.4.0','S2','F',173,173,[('03a452ca','S',0)],'D','D','D',0),
(32,'2026-04-20','1.41.5.1 AG2','S21 A O11 V G M N','L',35,38,[('8a9fbe8f','C',0),('d935b08c','S1',1)],'S','S','S',1),
(33,'2026-07-08','1.41.6.8','L S14 A O6 V G M N','L',35,35,[('0586daf3','C',0)],'U','U','S',1),
(34,'2026-08-24','1.41.7.7','S1','F',14,14,[('faa97820','S1',0)],'D','D','D',0),
(35,'2026-09-23','1.41.8.2','S11 A O6 V R N','C',36,76,[('2f984988','CD',0),('bd1d9365','S1',0),('6d22cb54','O',1)],'D','D','D',1),
(36,'2026-10-15','1.41.9.1','S1(no anchor)','F',104,104,[('c67396fa','S1',0)],'D','D','D',0),
(37,'2026-10-22','1.41.9.2','V S2 R','C',15.3,15.3,[('5465e2aa','CD',0)],'D','D','D',0),
(38,'2026-11-11','1.41.9.7','M(netmsg) S2 A','C',13,38,[('2eb9a57d','C',0),('c1b3d1f3','S1',1)],'S','S','S',1),
(39,'2026-11-23','1.42.0.1','V(ISource2Server)','C',None,None,[],'D','A','A',0),
]
sc=['T1','T2','Ideal']; idx={'T1':8,'T2':9,'Ideal':10}
n=len(I); print('incidents',n)
for s in sc:
    c={k:0 for k in 'ADSU'}
    for r in I: c[r[idx[s]]]+=1
    print(s, c, 'A+D=%d (%.0f%%)'%(c['A']+c['D'],100*(c['A']+c['D'])/n), 'A=%.0f%%'%(100*c['A']/n))
# commits
allc=[(cm,r) for r in I for cm in r[7]]
print('commits',len(allc),'followups',sum(cm[2] for cm,_ in allc))
for j,s in enumerate(sc):
    for label,sel in (('all',lambda cm:True),('followups',lambda cm:cm[2]==1)):
        cc=[CK[cm[1]][j] for cm,_ in allc if sel(cm)]
        A=cc.count('A'); Ds=cc.count('D*'); D=cc.count('D'); C=cc.count('C')
        # D* = single-sig commit: 25% chance A in T2/Ideal (two-anchor)
        exA=A+0.25*Ds
        print(f'  {s} {label}: n={len(cc)} A={A} D={D} D*={Ds} code={C} | A+D={(A+D+Ds)/len(cc):.0%} expectedA={exA/len(cc):.0%}')
# outage hours
out=[r for r in I if r[4] in ('C','L','C?') and r[5] is not None]
base=sum(r[5] for r in out); print('outage incidents',len(out),'baseline outage h',base)
for s in sc:
    rem=sum(r[5] for r in out if r[idx[s]]=='U')
    soft=sum(r[5] for r in out if r[idx[s]]=='S')
    dd=sum(r[5] for r in out if r[idx[s]] in 'AD')
    print(f'  {s}: outage remaining {rem}h ({rem/base:.0%}); converted to degraded (S) {soft}h; A/D {dd}h')
# per-incident print
for r in I:
    print(f"{r[0]:2d} {r[1]} {r[2]:22s} {r[3]:28s} {r[4]:2s} {str(r[5]):>5s}/{str(r[6]):>5s} fix={len(r[7])} fu={sum(c[2] for c in r[7])} {r[8]}{r[9]}{r[10]} {'I1' if r[11] else ''}")
