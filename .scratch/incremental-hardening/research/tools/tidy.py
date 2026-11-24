import sys
p=sys.argv[1]; L=open(p,encoding="utf-8").read().split("\n")
def ln(n): return n-1  # 1-based -> index
def det(id_,title,tk,body,cls="ev sub"):
    return [f'    <details class="{cls}" id="{id_}">',
            f'      <summary><span class="st">{title}</span><span class="tk">{tk}</span></summary>',
            '      <div class="db">']+body+['      </div>','    </details>']
def rng(a,b): return L[ln(a):ln(b)+1]
out=[]
# assemble pieces bottom-up by building new list via segments
seg=[]
seg+=L[:ln(75)]
# progress
seg+=['  <details class="ev" id="progress">','    <summary><span class="st">Research steps</span><span class="tk">All tracks, the backtest and the adversarial review are done</span></summary>','    <div class="db">']+rng(77,88)+['    </div>','  </details>']
seg+=rng(90,240)
seg+=det("stacks-gaps","Seven gaps in the best stacks","Runtime slot checks, two anchors everywhere, safe failure, reliable watchers, live smoke tests, build-keyed caches, several anchors per target",rng(241,250))
seg+=rng(251,333)
seg+=rng(350,361)
seg+=det("backtest-scenarios","Scenarios and full results","What T1, T2 and the ideal stack include, and the result for each",rng(334,348))
seg+=det("backtest-meaning","What this means","T1 halves downtime; T2 saves effort, not downtime; 8 incidents are irreducible; humans stay on the critical path",rng(363,370))
seg+=det("backtest-empirical","Empirical check on real breaking updates","Two agreeing anchors healed 6/10 broken signatures and 6/6 moved offsets with 0 wrong; also found the root cause of #1494",rng(372,386))
seg+=rng(387,454)
seg+=det("review-table","Verdict per claim","2 refuted, 6 weakened, 1 confirmed",rng(455,468))
seg+=L[ln(469):]
s="\n".join(seg)
nav='''  <nav class="toc" aria-label="Sections">
    <a href="#answer">Answer</a><a href="#taxonomy">What breaks</a><a href="#design">Toolkit design</a><a href="#stacks">Stacks</a><a href="#backtest">Backtest</a><a href="#review">Review</a><a href="#evidence">Evidence</a>
  </nav>
'''
s=s.replace("  </header>\n","  </header>\n"+nav,1)
s=s.replace("a{color:var(--accent)}","a{color:var(--accent)}\n.toc{display:flex;flex-wrap:wrap;gap:6px 14px;font-family:var(--mono);font-size:.8rem;margin-top:-18px}\n.toc a{text-decoration:none;border-bottom:1px solid var(--line)}\n.toc a:hover{border-color:var(--accent)}",1)
open(p,"w",encoding="utf-8").write(s)
print("ok")
