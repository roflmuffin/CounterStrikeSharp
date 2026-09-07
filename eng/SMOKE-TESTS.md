# PR smoke tests

A repository **maintainer or admin** can post this as a new PR conversation comment:

```text
/smoke-test
```

No SHA is needed. The workflow snapshots the PR's latest head when it accepts the
request, then builds and tests that exact commit (including fork PRs). It updates a
reply on the PR, adds a **Game server smoke test** check to the tested commit, and
writes an Actions job summary. Results include pass/fail/skip counts and the CS2
patch, server/client versions, source revision and build date/time from the
server's `/game/csgo/steam.inf`.

Each request gets its own reply and check. If the PR changes during the run, the
reply warns that the result is outdated. Post another `/smoke-test` to test the
new head. Editing a comment does not trigger a run. Plain `write`, `triage`,
organization membership and previous contribution do not grant permission.

This is optional: **do not add this check to required branch-protection checks**.
The `issue_comment` workflow and trusted scripts must first be merged to the
repository's default branch before the command will work.

## Repository setup

1. Provision a **dedicated, disposable Linux CS2 test server** managed by
   Pterodactyl, with Metamod, CS#, and the .NET 10 runtime installed. Configure a
   playable map, RCON, bots and an actively ticking server (for example,
   `sv_hibernate_when_empty 0`). Tests modify entities, players, bots and convars;
   do not use a production server. Avoid other plugins and automatic updates or
   restarts during tests. CS# must auto-load `NativeTestsPlugin` at startup.
2. Allow the GitHub-hosted runner to reach RCON, SFTP and the panel API. Pin and
   verify the SFTP host key; host verification is not disabled.
3. Create a GitHub environment named **`smoke-test`**. Store these environment
   secrets (not repository-wide secrets):

   | Secret | Purpose |
   | --- | --- |
   | `GS_HOST`, `GS_PORT`, `GS_PASS` | RCON host, port and password |
   | `SFTP_HOST` | SFTP URL, e.g. `sftp://host:2022` |
   | `SFTP_USER`, `SFTP_PASS` | SFTP credentials for the dedicated server |
   | `SFTP_KNOWN_HOSTS` | Verified OpenSSH known_hosts entry; use `[host]:port` for a nonstandard port |
   | `PTERO_URL` | Panel base URL, e.g. `https://panel.example.com` |
   | `PTERO_API_KEY` | Client API key with resources/read and power control for this server only |
   | `PTERO_SERVER_ID` | Server identifier for `/api/client/servers/{identifier}` |

   Optional environment variables:

   | Variable | Default |
   | --- | --- |
   | `GS_ADDON_DIR` | `/game/csgo/addons/counterstrikesharp` |
   | `GS_STEAM_INF` | `/game/csgo/steam.inf` |
   | `GS_RESTART_TIMEOUT` | `120` seconds per stop/start readiness wait |
   | `GS_TEST_TIMEOUT` | `600` seconds for the test report |

   Paths are SFTP-visible absolute paths. If increasing timeouts substantially,
   also increase the smoke job's 25-minute timeout.
4. Consider environment required reviewers as an additional approval barrier.
   Restrict deployment branches to the default branch: this workflow runs in
   default-branch context, not PR context. Allow Actions to create checks and
   issue comments; permissions are scoped to the authorization/reporting jobs.

## What runs

- Fresh Linux native build (Steam Runtime SDK), API and native test plugin builds.
- The managed `CounterStrikeSharp.API.Tests` unit suite on the Actions runner.
- Stop the game server, deploy the native binaries/configs and replace the API
  and test-plugin directories, then start the server and wait for RCON.
- Read `steam.inf` **after startup**, since the panel may update CS2 on startup.
- Run `css_smoke_test <unique-run-id>` through RCON. The native plugin discovers
  all tests, excluding `Category=Benchmark` and benchmark-named classes/methods.
  Add `[Trait("Category", "Benchmark")]` to any new benchmark classes.
- Wait for an atomically published JSON report matching this run's unique ID.
  Async/frame-based tests are awaited. Missing/stale/malformed reports, timeouts,
  build/deployment errors, failed tests, runner/cleanup errors, zero tests and
  all-skipped runs fail the check. Skipped tests are counted and shown.

Artifacts include individual native test outcomes and failure messages/stack
traces (`smoke-results.json`), a Markdown summary, the original `steam.inf`, parsed
`server-version.json`, and managed unit-test TRX results. The PR links to the run
for logs and artifacts, including when a build fails or the server crashes.

The shared server is serialized across PRs with `cancel-in-progress: false`.
GitHub keeps only one pending job per concurrency group, so a newer queued job
can replace an older pending one. A failed-job rerun reuses the approved commit;
post a fresh comment to request the latest head. Live maintainer permissions are
checked again before deployment, including on reruns.

## Trust and operational boundaries

PR code is built on isolated GitHub-hosted jobs with no server secrets and no
write-enabled repository token. The deployment and reporting jobs use automation
from the default-branch commit, never scripts from the PR. Downloaded artifacts
are treated as data on these runners; reports are validated rather than rendered
as arbitrary Markdown.

**A maintainer command authorizes arbitrary PR code to execute on the game
server.** Review the PR before requesting a run. Keep that server/container and
its network isolated from production and other tenants; use least-privilege,
server-scoped panel/SFTP credentials. Code running there can access the server's
files and RCON configuration. This is not a sandbox for hostile plugins.

The runner does not restore the previous installation: the tested build is left
on the dedicated server, and an upload failure can leave it stopped. Reprovision
it after testing suspicious changes or before relying on a clean baseline. Do
not run the local benchmark runner concurrently with this workflow.

## Local development

`eng/run-smoke-tests.ts` takes **prebuilt** artifacts and reads configuration from
the process environment (it deliberately does not load `.env`):

```text
payload/native/addons/counterstrikesharp/  # CMake output including native .so/configs
payload/api/                             # API bin/Release/net10.0 contents
payload/plugin/                          # native test plugin bin/Release/net10.0 contents
```

With RCON/SFTP/panel configuration exported and SSH known_hosts configured:

```sh
deno run --allow-run --allow-read --allow-env --allow-net --allow-write \
  eng/run-smoke-tests.ts payload TestResults/Smoke
```

Automation validation (no game server needed):

```sh
node --test .github/scripts/smoke-test.test.cjs
deno check eng/run-smoke-tests.ts
deno lint eng/run-smoke-tests.ts
```

`eng/run-benchmarks.ts`, `css_itest <filter>` and `css_run_tests` retain their
existing behavior. For a manual non-benchmark server run, use
`css_smoke_test manual-1` from the server console/RCON and read
`NativeTestsPlugin/smoke-results.json`.
