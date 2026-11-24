import sys, math, statistics, html
p=sys.argv[1]; s=open(p,encoding="utf-8").read()
B=["AG2 Apr","Jul","Aug","Sep","Oct 22","Nov 5","Nov 11","1.42.0.1"]
# (name, sub, cat, {build:hours}, status142, statuscls)
R=[("s2script","Metamod · pre-1.0 · 17 bundled plugins","chk",{"Jul":3,"Oct 22":19,"Nov 11":1.2,"1.42.0.1":1.5},"hooks off safely; fixed 1.5 h","ok"),
   ("SwiftlyS2","own loader · ~165 plugins","data",{"AG2 Apr":45,"Jul":36,"Aug":11,"Sep":5,"Oct 22":1.5,"Nov 11":1.4,"1.42.0.1":4.6},"JSON fix 4.6 h, unreleased; silent until then","warn"),
   ("cs2kz","Metamod · single-purpose","hdr",{"AG2 Apr":25,"Jul":21,"Sep":15.5,"Oct 22":1.2,"Nov 11":4.4,"1.42.0.1":3.2},"SDK fork 3.2 h","ok"),
   ("Metamod:Source","foundation · ~100 plugins","hdr",{"AG2 Apr":10,"Jul":11},"unaffected","ok"),
   ("Plugify s2sdk","own / Metamod · ~20 plugins","hdr",{"AG2 Apr":20,"Jul":10.5,"Aug":31,"Sep":14,"Oct 22":8,"Nov 11":1.5},"broken; needs binary release","bad"),
   ("ModSharp","own loader · ~50 plugins","mix",{"AG2 Apr":36,"Jul":25,"Aug":12.5,"Sep":20,"Oct 22":5,"Nov 5":9,"Nov 11":6},"runs; Steam hooks silently wrong","warn"),
   ("CS2Fixes","Metamod · single-purpose","hdr",{"AG2 Apr":27,"Jul":21,"Aug":2,"Sep":17,"Oct 22":5,"Nov 5":3,"Nov 11":26},"unknown","na"),
   ("CounterStrikeSharp","Metamod · ~350–525 plugins, hosting default","hdr",{"AG2 Apr":35,"Jul":35,"Aug":14,"Sep":36,"Oct 22":15,"Nov 11":13},"crashing; not fixed","bad")]
CAT={"data":("slots in data files","c-data"),"chk":("compiled, checked at boot","c-chk"),"mix":("detours + data","c-mix"),"hdr":("compiled from headers","c-hdr")}
LO,HI=1,48
def x(h): return (math.log(h)-math.log(LO))/(math.log(HI)-math.log(LO))*100
ticks=[1,2,4,8,16,32]
o=['    <div class="sv" role="group" aria-label="Hours to fix each breaking CS2 build, per stack">',
   '      <div class="sv-legend">'+''.join(f'<span><i class="sw {c}"></i>{t}</span>' for k,(t,c) in CAT.items())+'<span><i class="sv-med-key"></i>median</span></div>',
   '      <div class="sv-row sv-axis"><span></span><span class="sv-track sv-ticks">'+''.join(f'<span style="left:{x(t):.1f}%">{t} h</span>' for t in ticks)+'</span><span class="sv-mh">median</span><span class="sv-sh">on 1.42.0.1</span></div>']
for name,sub,cat,d,st,stc in R:
    vals=list(d.values()); med=statistics.median(vals)
    dl=[];placed=[]
    for b,h in sorted(d.items(),key=lambda kv:kv[1]):
        xp=x(h); near=[q for q in placed if abs(q[0]-xp)<2.6]
        dy=[0,-7,7,-14,14][min(len(near),4)]
        placed.append((xp,dy))
        dl.append(f'<span class="sv-dot {CAT[cat][1]}" style="left:{xp:.1f}%;margin-top:{dy-5}px" data-tip="{b}: {h:g} h" tabindex="0"></span>')
    dots=''.join(dl)
    extra=''
    if name=="CounterStrikeSharp":
        extra=''
    grid=''.join(f'<span class="sv-grid" style="left:{x(t):.1f}%"></span>' for t in ticks)
    o.append(f'      <div class="sv-row"><span class="sv-name"><strong>{name}</strong><small>{sub}</small></span>'
             f'<span class="sv-track">{grid}{dots}{extra}<span class="sv-med" style="left:{x(med):.1f}%"></span></span>'
             f'<span class="sv-mv">{(round(med) if med>=10 else round(med,1)):g} h</span><span class="sv-st {stc}">{st}</span></div>')
o.append('      <p class="meta">Each dot is one breaking build since April 2026: hours from build to the first commit that restored function (log scale, n = 2–7 per stack). Hover for the build. Metamod was unaffected by most builds. Builds a stack was not broken by are not shown; CounterStrikeSharp\'s 1.42.0.1 break is still open, so it has no dot.</p>')
o.append('    </div>')
viz="\n".join(o)
a=s.index('    <div class="tbl"><table>\n      <thead><tr><th>Stack</th><th>Loader</th>')
m=s.index('<p class="meta">Median hours from build to first restoring commit',a)
b=s.index('</p>',m)+4
old=s[a:b]
det=('    <details class="ev sub" id="stacks-table"><summary><span class="st">Details per stack</span><span class="tk">Loader, where vtable slots come from, median fix, 1.42.0.1 status, ecosystem size</span></summary><div class="db">\n'+old+'\n    </div></details>')
s=s[:a]+viz+"\n"+det+s[b:]
css='''
.sv{display:grid;gap:4px;margin:8px 0}
.sv-legend{display:flex;flex-wrap:wrap;gap:6px 16px;font-size:.8rem;color:var(--muted);margin-bottom:6px}
.sv-legend span{display:inline-flex;align-items:center;gap:6px}
.sw.c-data{background:var(--s-inc)} .sw.c-chk{background:var(--s-chk)} .sw.c-mix{background:var(--s-ide)} .sw.c-hdr{background:var(--s-hdr)}
.sv-med-key{display:inline-block;width:2px;height:14px;background:var(--fg)}
.sv-row{display:grid;grid-template-columns:minmax(120px,1.1fr) 3fr 56px minmax(110px,1.2fr);gap:12px;align-items:center;padding:7px 0;border-bottom:1px solid var(--line);font-size:.86rem}
.sv-axis{border-bottom:0;padding:0;font-family:var(--mono);font-size:.72rem;color:var(--muted)}
.sv-name{display:grid;min-width:0}.sv-name small{color:var(--muted);font-size:.74rem;line-height:1.3}
.sv-track{position:relative;height:34px}
.sv-ticks{height:16px}.sv-ticks span{position:absolute;transform:translateX(-50%);white-space:nowrap}
.sv-ticks span:first-child{transform:none}.sv-ticks span:last-child{transform:translateX(-100%)}
.sv-grid{position:absolute;top:0;bottom:0;width:1px;background:var(--line)}
.sv-dot{position:absolute;top:50%;width:10px;height:10px;margin-left:-5px;border-radius:50%;box-shadow:0 0 0 1.5px var(--bg);cursor:default}
.sv-dot.c-data{background:var(--s-inc)}.sv-dot.c-chk{background:var(--s-chk)}.sv-dot.c-mix{background:var(--s-ide)}.sv-dot.c-hdr{background:var(--s-hdr)}
.sv-med{position:absolute;top:4px;bottom:4px;width:2px;margin-left:-1px;background:var(--fg)}
.sv-open{position:absolute;top:50%;transform:translateY(-50%);font-family:var(--mono);font-size:.72rem;color:var(--run);border:1px dashed var(--run);border-radius:4px;padding:0 4px;white-space:nowrap;cursor:default}
.sv-mv{font-family:var(--mono);font-variant-numeric:tabular-nums;text-align:right}
.sv-mh,.sv-sh{font-size:.72rem}.sv-mh{text-align:right}
.sv-st{font-size:.8rem;line-height:1.3;padding-left:8px;border-left:3px solid var(--todo)}
.sv-st.ok{border-color:var(--done)}.sv-st.warn{border-color:var(--run)}.sv-st.bad{border-color:var(--bad)}
.sv-dot[data-tip]:hover::after,.sv-dot[data-tip]:focus-visible::after,.sv-open[data-tip]:hover::after{content:attr(data-tip);position:absolute;left:50%;bottom:calc(100% + 6px);transform:translateX(-50%);z-index:5;background:var(--fg);color:var(--bg);font-size:.72rem;font-family:var(--body);padding:3px 7px;border-radius:4px;white-space:nowrap;pointer-events:none}
@media (max-width:620px){.sv-row{grid-template-columns:1fr 46px;row-gap:4px}.sv-row .sv-track{grid-column:1/-1;grid-row:2}.sv-st{grid-column:1/-1}.sv-axis .sv-mh,.sv-axis .sv-sh,.sv-axis>span:first-child{display:none}.sv-axis .sv-track{grid-row:1}}
'''
s=s.replace(".toc{display:flex;",css+"\n.toc{display:flex;",1)
s=s.replace("--track:#e6e9ee; --s-inc:#2a78d6; --s-ide:#eb6834;","--track:#e6e9ee; --s-inc:#2a78d6; --s-ide:#eb6834; --s-chk:#1baf7a; --s-hdr:#8b93a1; --bad:#c43c3b;",1)
s=s.replace("--track:#262e3a; --s-inc:#3987e5; --s-ide:#d95926;","--track:#262e3a; --s-inc:#3987e5; --s-ide:#d95926; --s-chk:#199e70; --s-hdr:#6c7586; --bad:#e66767;")
open(p,"w",encoding="utf-8").write(s); print(s.count("--s-chk:"))
