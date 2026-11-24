import re, sys
p = sys.argv[1]
s = open(p, encoding="utf-8").read()

def grab(sec_id):
    m = re.search(r'  <section class="r" id="%s">\n(.*?)\n  </section>\n' % sec_id, s, re.S)
    return m.group(0), m.group(1)

def inner_wo_h2(body):
    return re.sub(r'^\s*<h2>.*?</h2>\n', '', body, count=1, flags=re.S)

def details(sec_id, title, takeaway, body, cls="ev"):
    return (f'  <details class="{cls}" id="{sec_id}">\n'
            f'    <summary><span class="st">{title}</span><span class="tk">{takeaway}</span></summary>\n'
            f'    <div class="db">\n{body}\n    </div>\n  </details>\n')

css_add = """
.lede{font-size:1.08rem;font-weight:500;max-width:68ch}
.keys{margin:6px 0 0;padding-left:18px;display:grid;gap:6px;max-width:72ch}
.stats{display:grid;grid-template-columns:repeat(auto-fit,minmax(150px,1fr));gap:10px;margin-top:14px}
.stat{border-top:2px solid var(--accent);padding-top:6px}
.stat b{font-family:var(--display);font-size:1.5rem;display:block;font-variant-numeric:tabular-nums}
.stat span{color:var(--muted);font-size:.85rem}
details{background:var(--surface);border:1px solid var(--line);border-radius:8px;min-width:0}
section.r>details{margin-top:8px}
.evs{display:grid;gap:8px}
summary{cursor:pointer;list-style:none;padding:12px 16px;display:grid;gap:2px}
summary::-webkit-details-marker{display:none}
summary .st{font-family:var(--display);font-weight:700;font-size:1.02rem}
summary .st::before{content:"+";display:inline-block;width:1.1em;color:var(--accent);font-family:var(--mono)}
details[open] summary .st::before{content:"\\2212"}
summary .tk{color:var(--muted);font-size:.9rem;padding-left:1.1em}
.db{padding:0 16px 16px;display:grid;gap:4px}
.v{font-family:var(--mono);font-size:.72rem;letter-spacing:.04em;text-transform:uppercase;white-space:nowrap}
.v.i{color:var(--run)} .v.a{color:var(--done)} .v.p{color:var(--accent)}
.group{display:grid;gap:36px}
.compact li{margin-bottom:4px}
"""
s = s.replace("a{color:var(--accent)}", "a{color:var(--accent)}" + css_add, 1)

# --- answer ---
ans_full, _ = grab("answer")
ans_new = '''  <section class="r" id="answer">
    <h2>Short answer</h2>
    <p class="lede">Partly intrinsic, mostly self-inflicted. A much more resilient toolkit is plausible, though not one that never breaks.</p>
    <ul class="keys">
      <li><strong>Intrinsic:</strong> no Valve plugin ABI; stripped binaries rebuilt on nearly every update; engine interfaces change layout about monthly while their version strings stay the same; gameplay refactors.</li>
      <li><strong>Self-inflicted:</strong> engine vtable slots compiled in from a reverse-engineered header chain; first-match prologue signatures; hand-kept indices and offsets; all-or-nothing startup.</li>
      <li><strong>Proof point:</strong> build 26879581 added one virtual to <code>ISource2Server</code>. CounterStrikeSharp is still down, ModSharp needed nothing, and cs2kz was back in 3.5 h.</li>
      <li><strong>Feasibility:</strong> anchor-based re-resolution found 82% of 139 targets again on a real update; fuzzy signatures found 0%. Expect CI to fix most updates within an hour, with a human needed every 2–3 months.</li>
    </ul>
    <div class="stats">
      <div class="stat"><b>~11%</b><span>of CS2 builds needed a CounterStrikeSharp fix</span></div>
      <div class="stat"><b>42</b><span>forced SDK ABI episodes in ~3 years</span></div>
      <div class="stat"><b>0</b><span>fixes needed for name-based bindings in 3 years</span></div>
      <div class="stat"><b>82%</b><span>targets auto-healed via anchors (upper bound)</span></div>
    </div>
  </section>
'''

# --- taxonomy: compact visible + full in details ---
tax_full, tax_body = grab("taxonomy")
tax_new = '''  <section class="r" id="taxonomy">
    <h2>What breaks, and why</h2>
    <div class="tbl"><table>
      <thead><tr><th>Class</th><th>How often</th><th>Avoidable?</th></tr></thead>
      <tbody>
        <tr><td>Byte-pattern signatures</td><td>154 changes; each rewritten 1.5–2× a year</td><td><span class="v a">mostly avoidable</span></td></tr>
        <tr><td>Vfunc indices in gamedata</td><td>75 changes; 6–11 of 17 move at once</td><td><span class="v a">mostly avoidable</span></td></tr>
        <tr><td>Engine vtables compiled from headers</td><td>42 SDK episodes; causing today's outage</td><td><span class="v a">mostly avoidable</span></td></tr>
        <tr><td>tier0 link errors</td><td>4–5 incidents</td><td><span class="v a">avoidable</span></td></tr>
        <tr><td>Loader (<code>gameinfo.gi</code>)</td><td>every update</td><td><span class="v p">partly</span></td></tr>
        <tr><td>Non-schema struct offsets</td><td>~12 incidents</td><td><span class="v p">reducible</span></td></tr>
        <tr><td>SDK API refactors</td><td>~11</td><td><span class="v p">partly</span></td></tr>
        <tr><td>Schema renames / removals</td><td>7 structural builds in 16 months</td><td><span class="v i">intrinsic</span></td></tr>
        <tr><td>Behaviour changes</td><td>~9</td><td><span class="v i">intrinsic</span></td></tr>
        <tr><td>Protobuf</td><td>0</td><td>not a problem</td></tr>
      </tbody>
    </table></div>
''' + details("taxonomy-full", "Root causes and mitigations per class",
              "The full table: what causes each class and how a toolkit would handle it",
              inner_wo_h2(tax_body), cls="ev sub") + '  </section>\n'

# --- intrinsic: collapsed ---
int_full, int_body = grab("intrinsic")
int_new = details("intrinsic", "Intrinsic or self-inflicted?",
                  "What any in-process addon must live with, and the seven choices that make today's stack fragile",
                  inner_wo_h2(int_body), cls="ev sub")

# --- design: compact visible + full in details ---
des_full, des_body = grab("design")
des_new = '''  <section class="r" id="design">
    <h2>A more resilient toolkit</h2>
    <ol class="compact">
      <li><strong>Loading:</strong> its own launch wrapper or server proxy, with no dependency on <code>gameinfo.gi</code>; cache everything per build ID.</li>
      <li><strong>Resolver:</strong> several anchors per target (RTTI slot, unique string, caller plus ordinal, ConVar callback); accept an address only when two agree, and never take a first match.</li>
      <li><strong>Engine interfaces:</strong> find the methods at load time and detour them, with no compiled slots.</li>
      <li><strong>Data:</strong> schema fields by name from the static tables; the few other offsets extracted from code and checked at boot.</li>
      <li><strong>Linking:</strong> no mangled tier0 imports; <code>dlsym</code> with fallbacks; minimal own types.</li>
      <li><strong>Failure:</strong> one feature switches off; the server keeps running.</li>
      <li><strong>Pipeline:</strong> a Steam watcher archives each build, re-derives everything, smoke-tests a headless server and publishes signed gamedata.</li>
      <li><strong>Plugin API:</strong> no raw offsets or SDK types; rename aliases; official surfaces where they fit.</li>
    </ol>
''' + details("design-full", "Design details, residual breakage, and an incremental path for CounterStrikeSharp",
              "How each piece works, what still needs a human, and changes CounterStrikeSharp could make without a rewrite",
              inner_wo_h2(des_body), cls="ev sub") + '  </section>\n'

# --- evidence sections ---
ev = [
 ("cssharp", "CounterStrikeSharp history", "383 gamedata changes over 31 update rounds; name-based bindings never broke; fixes take ~1 day plus follow-ups"),
 ("sdk", "hl2sdk-cs2 and Metamod:Source", "42 forced ABI episodes in ~3 years; interface versions never bumped; Metamod itself rarely changes"),
 ("binary", "The binaries", "1.42.0.0 broke nothing; 26879581 shifted ISource2Server; prologue signatures fragile by construction; 82% auto-heal"),
 ("ecosystem", "How other projects cope", "Newer stacks skip Metamod and headers; automatic regeneration exists; cs_script is map-bound"),
 ("baseline", "Source 1 comparison and update cadence", "Cadence only 1.3–1.9× CS:GO; break rate similar; losing Linux symbols is the big difference"),
]
ev_html = ['  <section class="r" id="evidence">\n    <h2>Evidence</h2>\n    <p>Research tracks A–E. Expand any of them for the numbers and sources.</p>\n    <div class="evs">\n']
for sid, title, tk in ev:
    full, body = grab(sid)
    ev_html.append(details(sid, title, tk, inner_wo_h2(body)))
    s = s.replace(full, "")

stacks_full, _ = grab("stacks")
review_full, _ = grab("review")

for blk in (ans_full, tax_full, int_full, des_full, stacks_full, review_full):
    s = s.replace(blk, "")

ev_html.append('    </div>\n  </section>\n')
new_main = ans_new + "\n" + tax_new + "\n" + int_new + "\n" + des_new + "\n"+stacks_full + "\n" + review_full + "\n" + "\n".join(ev_html)

# insert before activity panel
s = s.replace('  <section class="panel" aria-labelledby="logh">', new_main + '\n  <section class="panel" aria-labelledby="logh">', 1)

# activity log collapsed
s = s.replace('  <section class="panel" aria-labelledby="logh">\n    <h2 id="logh">Activity</h2>\n', '  <details class="ev" id="activity">\n    <summary><span class="st">Activity log</span><span class="tk">What happened when</span></summary>\n    <div class="db">\n', 1)
s = re.sub(r'(<div class="log" id="log">.*?\n    </div>\n)  </section>', r'\1    </div>\n  </details>', s, count=1, flags=re.S)
s = re.sub(r'\n{3,}', '\n\n', s)
open(p, "w", encoding="utf-8").write(s)
print("ok")
