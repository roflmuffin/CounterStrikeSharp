# Handoff: a streamlined upstream edition of the CS2 breakage study

Date: 2026-11-25. Writing task only: no code changes, no posting anywhere, no contact with upstream.

## Goal

Write a **new, short, standalone page**, designed from scratch (see "Design freedom"), for a CounterStrikeSharp upstream maintainer (roflmuffin and co-maintainers) who has never seen this work. Done means they can read it in about 5 minutes and come away knowing:

1. why CS2 updates break CounterStrikeSharp, and which part is avoidable;
2. what an incremental hardening path looks like (A–C, plus optional D);
3. how much that path would have saved over the last 3 years (backtest);
4. what it does not fix.

It is a discussion starter, not a pitch and not a roadmap. The existing "upstream edition" (v1) is a lightly redacted copy of the internal report: ~140 KB, every research section included. It is too long and still reads like an internal working document. **Do not edit v1. Write a new page from scratch** and use the sources below for facts only.

## Sources (read in this order)

- **Internal report, current:** `research/report.html` (live: https://claude.ai/artifact/CkhPT5n5hbWJyW3SZcy7y4; other sessions also edit it, so if they differ, read the live one with the Artifact tool). This is the source of truth for numbers. The sections that matter: Short answer, What breaks, The plan (four specs, "Behaviour changes to raise with the maintainer", "Which spec moves which number"), Backtest, Every stack compared, Adversarial review.
- **Backwards compatibility:** the maintainer's one condition is backwards compatibility. The plan's compatibility table (additive API; three behaviour changes that need their OK: schema misses that throw, strict core signatures, Linux updater) belongs on the streamlined page in neutral form, as open questions for the maintainer.
- **Upstream v1:** `research/report-upstream-v1.html` (https://claude.ai/artifact/GxRotmbCQqTzWzrLvxXdiX). Shows which tone fixes were already made (see "Tone rules" below). Don't copy its structure.
- **Specs, for one-paragraph summaries only:** `.scratch/hardening-a-fail-soft/spec.md`, `.scratch/hardening-b-boot-verify/spec.md`, `.scratch/hardening-d-own-loader/spec.md`. Spec C isn't written yet; per `incremental-hardening/README.md` it is I6 (signed, build-keyed gamedata updater on both OSes) + I7 (CI that boots each new build).
- **Per-spec backtest numbers:** `research/specsplit/split.py` (run from `research/` with `python3 -P specsplit/split.py .`). The table is below, so you shouldn't need to re-run it.

## Design freedom

The internal report is a **fact source only**. Don't carry over its styling, colours, fonts, layout, section order, section names, chart types or component patterns: no ledger bars, dot plot, cards or `<details>` stacks unless you independently decide they're the best fit. Design the page from scratch for this audience and for the four questions in Goal. The shape below is one possible starting point, not a spec. Reorder, merge, rename or drop sections, and pick different visuals, as long as the page answers the four questions, uses the canonical numbers and follows the tone rules.

## Suggested shape (optional starting point, high-level first)

Aim for about 1,200–1,800 words of prose plus 2–3 visuals, with secondary detail kept out of the main reading path.

1. **Title and one-paragraph framing.**
   - What this is and that it comes from public data: CounterStrikeSharp git history, issue trackers, the s2sdk/Metamod history, Linux server binaries.
   - The backtest is a model, not a measurement.
2. **The short answer**, 3–4 bullets:
   - Breaks come mostly from binding *by position* (byte-pattern signatures, vtable slot numbers, struct offsets), not by name. Schema fields, game events, ConVars and net messages had 0 value fixes in 3 years.
   - Some of it is intrinsic: no Valve ABI, stripped binaries rebuilt every update, interface version strings never bumped, gameplay refactors.
   - Much of it is avoidable without a rewrite.
   - The 1.42.0.1 example: one virtual inserted into `ISource2Server`; stacks with slots in data fixed it with a JSON edit, header-based stacks waited on an SDK release.
3. **What breaks:** a compact table of the ~6 main classes with how often and whether each is avoidable. Drop protobuf and the root-cause column.
4. **A possible incremental path:** four short cards (A, B, C, D). Each gets one sentence on what it does and one on its effect. Mark D clearly as optional and separate, with default behaviour unchanged.
5. **Backtest:** one visual (stepped bars or a small table) for releases and outage hours across today → A → A–B → A–C, with the ideal stack as a reference. One line saying D doesn't move these numbers and why.
6. **What it does not fix:**
   - tier0/loader link breaks and hard-coded struct layouts;
   - IGameSystem (CounterStrikeSharp implements it);
   - schema renames and behaviour changes (8 incidents irreducible in every scenario);
   - human fix latency: total broken time only drops with auto-publishing regenerated gamedata (T2).
7. **How confident are we**, a short list:
   - small samples (the heal test is n=16, Sep–Nov 2026);
   - Linux-only binary analysis;
   - the model's assumptions;
   - an adversarial review was done; give the one-line result and the main corrections.
8. **Collapsed appendix (optional, max 2–3 blocks):**
   - per-step numbers table;
   - how other stacks compare: median hours to fix, with a neutral note that surface size confounds the comparison;
   - method notes.

Leave out of the page entirely: the progress panel, activity log, census of every stack, code audit per stack, per-incident table, evidence tracks, the degraded-time feature map, server-type tables, and the cheat-side or offset-dumper comparison.

## Canonical numbers (use these; they're consistent with the internal v35)

| Metric (Nov 2023–Nov 2026, 39 incidents) | Today | A | A–B | A–C | A–D | Ideal |
|---|---|---|---|---|---|---|
| Releases needed | 39 | 39 | 33 | 15 | 15 | 8 |
| Fix commits | 66 | 66 | 57 | 18 | 18 | 10 |
| Follow-up corrections | 25 | 25 | 20 | 4 | 4 | 2 |
| Outage hours (crash / not loading) | 606 | 522 | 471 | 329 | 329 | 0 |
| Effective outage (crash or core feature down) | 635 | 583 | 532 | 460 | 460 | 415 |
| Broken or degraded hours | 1,871 | 1,871 | 1,712 | 1,712 | 1,712 | 1,689 |

Other figures you may need:
- ~11% of CS2 builds needed a CounterStrikeSharp fix.
- Two agreeing anchors healed 6/10 broken signatures and 6/6 moved offsets with 0 wrong; a single anchor was 20% silently wrong. Small n: four update pairs, Sep–Nov 2026.
- Auto-publish (T2, beyond A–D): broken-or-degraded ~1,396 h (−25%), effective outage ~426 h. These come from the earlier model run, so say "about".
- D: in 7 of 9 Metamod load failures CounterStrikeSharp shared the root cause, so a loader alone saves ≤8% of Metamod's 162 h. Its gain is Metamod-only bugs (Metamod #278, #288).
- Dates (GameTracking UTC): 1.41.9.7 Nov 11, 1.42.0.0 Nov 20, 1.42.0.1 Nov 23 2026.
- Metamod's core was **not** affected by 1.42.0.1: it hooks no `ISource2Server` slot.

Figures that need care (reconciled in the internal report; if you use them, keep the qualifier):
- gamedata offsets: 17 entries = 14 vtable indices + 3 struct offsets; 11 of the 14 were checkable in libserver;
- signature churn: 154 entry changes in update clusters vs 230 per-OS value changes; say which one you mean;
- the old "82% auto-heal" was refuted; don't use it;
- "−46% / −62%": always say these are A–C.

## Tone rules (from the review of v1)

- **Neutral, no blame:** say "avoidable design choices", not "self-inflicted" or "fragility CSSharp adds itself". No remarks about maintainers' time zones or working hours; at most "fixes typically land 09–14 UTC after ~23:00 UTC releases". No scare quotes around anyone's fix.
- **Credit what works:** schema-by-name, gamedata as data, the existing native test suite.
- **Other projects:** only claims backed in the internal report, hedged the same way ("from its code", "inferred"). No asides about AlliedModders' AGENTS.md or commit counts. Nothing on skin plugins or Valve's token rules. No "lock-in".
- **No internal planning:**
  - no ticket counts, must/later, "ready", ADRs, fork-first decisions or "no upstream PRs for now";
  - no file paths, agent or session mentions;
  - no "spec" jargon: call them proposals or steps A–D.
- **Define terms on first use:** binding, slot, gamedata, T2/auto-publish, ideal stack, effective outage. Avoid letter overload: steps A–D are the only letters; write outcomes as words.
- **Issue numbers** always name the repo (CounterStrikeSharp #1502, Metamod #278, s2sdk PR #128).

## Deliverable

- **Artifact:** a new private artifact (not v1's URL), titled e.g. "CS2 Update Breakage: Summary", icon `chart`. Load the `artifact-design` skill first. Follow the user's preference: high-level first, detail collapsed.
- **Local copy:** keep the HTML at `.scratch/incremental-hardening/research/report-upstream-v2.html` as well. The user's artifact sharing is limited to their organization, so an external maintainer will get the file, not the link. The page must therefore be self-contained, with no runtime capabilities and only Google Fonts and inline CSS/JS.
- **Before reporting done:** have a fresh subagent cold-read it as an upstream maintainer, checking consistency, tone, jargon and claims vs evidence. Fix what holds up.
- **Report back to the user** tersely: the link, the local path, the word count, what was cut, and any numbers you couldn't reconcile.
