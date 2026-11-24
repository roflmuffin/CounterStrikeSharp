import sys
p=sys.argv[1]; s=open(p,encoding="utf-8").read()
start=s.index('    <div class="tbl"><table>\n      <thead><tr><th>Over 3 years (39 incidents)</th>')
end=s.index('</table></div>',start)+len('</table></div>')
table=s[start:end]
rows=[("Releases needed","incidents needing a code change",39,15,8,"","/yr: 13 → 5 → 2.7"),
      ("Fix commits","needing code or a release",66,18,10,"",""),
      ("Follow-up corrections","needing code",25,4,2,"",""),
      ("Outage hours","crashed or not loading",606,329,0," h","ideal: all become degraded time")]
def bar(cls,label,v,today,unit):
    pct=0 if today==0 else v/today*100
    red=round((1-v/today)*100)
    tip=f"{label}: {v}{unit} of {today}{unit} today (−{red}%)"
    return (f'<div class="cb-row"><span class="cb-lab">{label}</span>'
            f'<span class="cb-track" data-tip="{tip}" tabindex="0"><span class="cb-fill {cls}" style="width:{pct:.1f}%"></span></span>'
            f'<span class="cb-val">{v}{unit}</span><span class="cb-red">−{red}%</span></div>')
out=['    <div class="churn" role="group" aria-label="Churn per metric, today versus incremental versus ideal">',
     '      <div class="cb-legend"><span><i class="sw track"></i>Today (full track)</span><span><i class="sw inc"></i>Incremental CounterStrikeSharp</span><span><i class="sw ide"></i>Ideal stack</span></div>']
for name,sub,t,i,d,u,note in rows:
    cap=round((t-i)/(t-d)*100)
    out.append(f'      <div class="cb">')
    out.append(f'        <div class="cb-head"><strong>{name}</strong> <span>{sub} · today {t}{u}</span><span class="cb-cap" title="Share of the ideal stack\'s reduction that the incremental changes already deliver">incremental captures <b>{cap}%</b> of ideal gain</span></div>')
    out.append('        '+bar("inc","Incremental",i,t,u))
    out.append('        '+bar("ide","Ideal",d,t,u))
    if note: out.append(f'        <div class="cb-note">{note}</div>')
    out.append('      </div>')
out.append('    </div>')
out.append('    <details class="ev sub" id="churn-table"><summary><span class="st">Table view</span><span class="tk">The same numbers, plus incidents fixed with no human (0% / 0% / ~18%)</span></summary><div class="db">')
out.append(table)
out.append('    </div></details>')
s=s[:start]+"\n".join(out)+s[end:]
css='''
.churn{display:grid;gap:14px;margin:6px 0 4px}
.cb-legend{display:flex;flex-wrap:wrap;gap:6px 18px;font-size:.82rem;color:var(--muted)}
.cb-legend span{display:inline-flex;align-items:center;gap:6px}
.sw{display:inline-block;width:14px;height:10px;border-radius:3px}
.sw.track{background:var(--track);outline:1px solid var(--line)}
.sw.inc{background:var(--s-inc)} .sw.ide{background:var(--s-ide)}
.cb{display:grid;gap:5px;padding:12px 14px;background:var(--surface);border:1px solid var(--line);border-radius:8px}
.cb-head{display:flex;flex-wrap:wrap;align-items:baseline;gap:4px 10px;font-size:.92rem}
.cb-head>span{color:var(--muted);font-size:.85rem}
.cb-cap{margin-left:auto;font-family:var(--mono);font-size:.76rem!important}
.cb-cap b{color:var(--fg);font-variant-numeric:tabular-nums}
.cb-row{display:grid;grid-template-columns:92px 1fr 64px 48px;gap:10px;align-items:center;font-size:.85rem}
.cb-lab{color:var(--muted)}
.cb-track{position:relative;height:14px;background:var(--track);border-radius:4px;cursor:default}
.cb-fill{position:absolute;inset:0 auto 0 0;border-radius:4px;min-width:0}
.cb-fill.inc{background:var(--s-inc)} .cb-fill.ide{background:var(--s-ide)}
.cb-val,.cb-red{font-family:var(--mono);font-variant-numeric:tabular-nums;text-align:right}
.cb-red{color:var(--done);font-weight:500}
.cb-note{font-size:.8rem;color:var(--muted);padding-left:102px}
.cb-track[data-tip]:hover::after,.cb-track[data-tip]:focus-visible::after{content:attr(data-tip);position:absolute;left:0;bottom:calc(100% + 6px);z-index:5;background:var(--fg);color:var(--bg);font-size:.75rem;padding:4px 8px;border-radius:4px;white-space:nowrap;pointer-events:none}
@media (max-width:520px){.cb-row{grid-template-columns:72px 1fr 52px 40px;gap:6px}.cb-note{padding-left:0}.cb-cap{margin-left:0}}
'''
s=s.replace(".toc{display:flex;",css+"\n.toc{display:flex;",1)
# tokens
s=s.replace("--display:\"Space Grotesk\"","--track:#e6e9ee; --s-inc:#2a78d6; --s-ide:#eb6834;\n  --display:\"Space Grotesk\"",1)
s=s.replace("--accent:#7ea6ec; --done:#5cc48a; --run:#e6a23c; --todo:#6c7586; --code:#202733; color-scheme:dark}}","--accent:#7ea6ec; --done:#5cc48a; --run:#e6a23c; --todo:#6c7586; --code:#202733; --track:#262e3a; --s-inc:#3987e5; --s-ide:#d95926; color-scheme:dark}}",1)
s=s.replace("--accent:#7ea6ec; --done:#5cc48a; --run:#e6a23c; --todo:#6c7586; --code:#202733; color-scheme:dark}\nbody","--accent:#7ea6ec; --done:#5cc48a; --run:#e6a23c; --todo:#6c7586; --code:#202733; --track:#262e3a; --s-inc:#3987e5; --s-ide:#d95926; color-scheme:dark}\nbody",1)
open(p,"w",encoding="utf-8").write(s)
print(s.count("--s-inc:"))
