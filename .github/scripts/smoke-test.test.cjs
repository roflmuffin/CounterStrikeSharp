const { test } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { authorize, report, resultSummary, versionSummary } = require('./smoke-test.cjs');

const sha = 'a'.repeat(40);
function request({ permission = 'write', role = 'maintain', body = '/smoke-test', state = 'open', pullRequest = true } = {}) {
  const calls = [];
  const outputs = {};
  const context = {
    repo: { owner: 'owner', repo: 'repo' }, actor: 'maintainer', serverUrl: 'https://github.com', runId: 123,
    payload: {
      issue: { number: 42, pull_request: pullRequest ? {} : undefined },
      comment: { body, user: { login: 'maintainer', type: 'User' } },
    },
  };
  const github = { rest: {
    repos: { getCollaboratorPermissionLevel: async (args) => {
      calls.push(['permission', args]);
      return { data: { permission, role_name: role } };
    } },
    pulls: { get: async () => ({ data: { state, head: { sha } } }) },
    checks: { create: async (args) => { calls.push(['check', args]); return { data: { id: 1 } }; } },
    issues: { createComment: async (args) => { calls.push(['comment', args]); return { data: { id: 2 } }; } },
  } };
  const core = { info() {}, setOutput: (key, value) => { outputs[key] = value; } };
  return { github, context, core, calls, outputs };
}

test('maintainer command snapshots the latest head and attaches the check to it', async () => {
  const input = request();
  await authorize(input);
  assert.equal(input.outputs.approved, 'true');
  assert.equal(input.outputs.sha, sha);
  assert.equal(input.calls.find(([name]) => name === 'check')[1].head_sha, sha);
});

test('admin can trigger', async () => {
  const input = request({ permission: 'admin', role: 'admin' });
  await authorize(input);
  assert.equal(input.outputs.approved, 'true');
});

for (const role of ['write', 'read', 'triage', 'none']) {
  test(`${role} permission cannot trigger a server run`, async () => {
    const input = request({ permission: role, role });
    await authorize(input);
    assert.equal(input.outputs.approved, undefined);
    assert.ok(input.calls.every(([name]) => name === 'permission'));
  });
}

for (const options of [{ body: '/smoke-test something' }, { state: 'closed' }, { pullRequest: false }]) {
  test(`ignores invalid request ${JSON.stringify(options)}`, async () => {
    const input = request(options);
    await authorize(input);
    assert.equal(input.outputs.approved, undefined);
    assert.ok(!input.calls.some(([name]) => name === 'check'));
  });
}

test('an unauthorized rerun actor cannot reuse a maintainer request', async () => {
  const input = request();
  // context.actor is used when running locally without TRIGGERING_ACTOR.
  input.context.actor = 'other';
  input.github.rest.repos.getCollaboratorPermissionLevel = async ({ username }) => ({
    data: { permission: 'write', role_name: username === 'maintainer' ? 'maintain' : 'write' },
  });
  await authorize(input);
  assert.equal(input.outputs.approved, undefined);
});

const passing = { total: 2, passed: 1, failed: 0, skipped: 1, errors: [], tests: [{ outcome: 'passed' }, { outcome: 'skipped' }] };
test('summarizes passing results including skips', () => {
  assert.equal(resultSummary(passing).success, true);
  assert.match(resultSummary(passing).text, /1 passed/);
});
test('test failures, runner errors, zero tests and all-skipped runs cannot pass', () => {
  assert.equal(resultSummary({ ...passing, errors: ['cleanup failure'] }).success, false);
  assert.equal(resultSummary({ ...passing, passed: 0, failed: 1, tests: [{ outcome: 'failed' }, { outcome: 'skipped' }] }).success, false);
  assert.equal(resultSummary({ total: 0, passed: 0, failed: 0, skipped: 0, errors: [], tests: [] }).success, false);
  assert.equal(resultSummary({ total: 1, passed: 0, failed: 0, skipped: 1, errors: [], tests: [{ outcome: 'skipped' }] }).success, false);
});
test('rejects malformed or inconsistent artifact counts', () => {
  for (const report of [{}, { ...passing, total: 99 }, { ...passing, passed: '1' }, { ...passing, tests: [] }]) {
    assert.throws(() => resultSummary(report));
  }
});
const version = {
  ClientVersion: '2000899', ServerVersion: '2000899', PatchVersion: '1.41.7.8',
  SourceRevision: '10948930', VersionDate: 'Aug 28 2026', VersionTime: '13:05:38',
};
test('reports the CS2 version from steam.inf fields', () => {
  assert.match(versionSummary(version), /1\.41\.7\.8/);
  assert.match(versionSummary(version), /server 2000899/);
  assert.match(versionSummary(version), /10948930 — Aug 28 2026 13:05:38/);
});
test('never renders arbitrary server-controlled Markdown', () => {
  assert.throws(() => versionSummary({ ...version, PatchVersion: '[click](https://example.org)' }));
  assert.throws(() => versionSummary({ ...version, VersionDate: '@everyone' }));
});

for (const scenario of ['success', 'new-head', 'build-failure', 'timeout', 'cancelled']) {
  test(`publishes check and PR comment: ${scenario}`, async () => {
    const originalCwd = process.cwd();
    const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'smoke-report-'));
    const values = {
      TESTED_SHA: sha, CHECK_ID: '1', COMMENT_ID: '2', NATIVE_RESULT: 'success',
      MANAGED_RESULT: scenario === 'build-failure' ? 'failure' : 'success',
      SMOKE_RESULT: scenario === 'cancelled' ? 'cancelled' : scenario === 'timeout' ? 'failure' : 'success',
    };
    const originalEnv = Object.fromEntries(Object.keys(values).map((key) => [key, process.env[key]]));
    try {
      process.chdir(directory);
      Object.assign(process.env, values);
      fs.mkdirSync('smoke-results');
      if (!['build-failure', 'timeout', 'cancelled'].includes(scenario)) {
        fs.writeFileSync('smoke-results/smoke-results.json', JSON.stringify(passing));
        fs.writeFileSync('smoke-results/server-version.json', JSON.stringify(version));
      }
      const input = request();
      const published = {};
      input.github.rest.pulls.get = async () => ({ data: { head: { sha: scenario === 'new-head' ? 'b'.repeat(40) : sha } } });
      input.github.rest.checks.update = async (args) => { published.check = args; };
      input.github.rest.issues.updateComment = async (args) => { published.comment = args; };
      input.core.summary = { addRaw(text) { published.summary = text; return this; }, async write() {} };
      await report(input);
      assert.equal(published.check.status, 'completed');
      assert.equal(published.check.conclusion, ['success', 'new-head'].includes(scenario) ? 'success' : scenario === 'cancelled' ? 'cancelled' : 'failure');
      assert.match(published.comment.body, new RegExp(sha));
      assert.match(published.comment.body, /actions\/runs\/123/);
      if (scenario === 'new-head') assert.match(published.comment.body, /does \*\*not\*\* cover the latest head/);
      if (scenario === 'success') assert.match(published.comment.body, /CS2:\*\* 1\.41\.7\.8/);
      if (scenario === 'timeout') assert.match(published.comment.body, /No valid, complete native test report/);
    } finally {
      process.chdir(originalCwd);
      fs.rmSync(directory, { recursive: true, force: true });
      for (const [key, value] of Object.entries(originalEnv)) {
        if (value === undefined) delete process.env[key];
        else process.env[key] = value;
      }
    }
  });
}
