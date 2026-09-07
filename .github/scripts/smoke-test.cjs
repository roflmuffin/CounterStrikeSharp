const fs = require('node:fs');

const checkName = 'Game server smoke test';
const runUrl = (context) => `${context.serverUrl}/${context.repo.owner}/${context.repo.repo}/actions/runs/${context.runId}`;

async function isMaintainerRequest({ github, context }) {
  // Check live repository permissions, not author_association (a contributor
  // or organization member is not necessarily a maintainer). Check reruns too.
  for (const username of new Set([context.payload.comment.user.login, process.env.TRIGGERING_ACTOR || context.actor])) {
    const { data } = await github.rest.repos.getCollaboratorPermissionLevel({ ...context.repo, username });
    if (data.permission !== 'admin' && data.permission !== 'maintain' && data.role_name !== 'maintain') {
      return false;
    }
  }
  return true;
}

async function authorize({ github, context, core }) {
  const { issue, comment } = context.payload;
  if (!issue?.pull_request || comment?.body.trim() !== '/smoke-test' || comment.user.type !== 'User') return;
  if (!await isMaintainerRequest({ github, context })) {
    core.info('Ignoring smoke-test request: maintain/admin permission required.');
    return;
  }
  const { data: pr } = await github.rest.pulls.get({ ...context.repo, pull_number: issue.number });
  if (pr.state !== 'open') return;
  // Snapshot the latest head ONCE. All builds use this immutable SHA, not a
  // mutable branch or refs/pull/N/head. Works for fork PRs as well.
  const sha = pr.head.sha;
  if (!/^[a-f0-9]{40}$/.test(sha)) throw new Error('Invalid PR head SHA');
  const details_url = runUrl(context);
  const { data: check } = await github.rest.checks.create({
    ...context.repo,
    name: checkName,
    head_sha: sha,
    status: 'in_progress',
    details_url,
    output: { title: 'Preparing smoke test', summary: `Building commit ${sha}. Benchmarks excluded.\n\n[Workflow run](${details_url})` },
  });
  const { data: reply } = await github.rest.issues.createComment({
    ...context.repo,
    issue_number: issue.number,
    body: `### Game server smoke test\n\nRequested for commit ${sha}. Building, then waiting for the dedicated server.\n\n[Follow the run](${details_url})`,
  });
  core.setOutput('sha', sha);
  core.setOutput('check_id', check.id);
  core.setOutput('comment_id', reply.id);
  core.setOutput('approved', 'true');
}

function readJson(path) {
  // Artifacts and server files are untrusted data, never code or Markdown.
  if (fs.statSync(path).size > 10 * 1024 * 1024) throw new Error('Report too large');
  return JSON.parse(fs.readFileSync(path, 'utf8'));
}

function resultSummary(report) {
  const keys = ['total', 'passed', 'failed', 'skipped'];
  if (!keys.every((key) => Number.isSafeInteger(report[key]) && report[key] >= 0) ||
      report.total !== report.passed + report.failed + report.skipped ||
      !Array.isArray(report.errors) || !Array.isArray(report.tests) || report.tests.length !== report.total ||
      ['passed', 'failed', 'skipped'].some((outcome) => report.tests.filter((test) => test?.outcome === outcome).length !== report[outcome])) {
    throw new Error('Invalid test report');
  }
  return {
    success: report.passed > 0 && report.failed === 0 && report.errors.length === 0,
    text: `${report.total} native tests: **${report.passed} passed**, **${report.failed} failed**, **${report.skipped} skipped**.\n\n` +
      `Runner/cleanup errors: ${report.errors.length}. Benchmarks excluded.`,
  };
}

function versionSummary(version) {
  // Only render bounded, validated values. Raw steam.inf is also an artifact.
  const patterns = {
    PatchVersion: /^\d+(\.\d+)+$/,
    ServerVersion: /^\d+$/,
    ClientVersion: /^\d+$/,
    SourceRevision: /^\d+$/,
    VersionDate: /^[a-zA-Z0-9 ,/-]+$/,
    VersionTime: /^[0-9:]+$/,
  };
  if (!Object.entries(patterns).every(([key, pattern]) =>
    typeof version[key] === 'string' && version[key].length <= 100 && pattern.test(version[key]))) {
    throw new Error('Invalid CS2 version');
  }
  return `**CS2:** ${version.PatchVersion} (server ${version.ServerVersion}, client ${version.ClientVersion})\n\n` +
    `**Source revision:** ${version.SourceRevision} — ${version.VersionDate} ${version.VersionTime}`;
}

async function report({ github, context, core }) {
  const env = process.env;
  const jobs = [env.NATIVE_RESULT, env.MANAGED_RESULT, env.SMOKE_RESULT];
  let success = jobs.every((result) => result === 'success');
  let text = '';
  try {
    const result = resultSummary(readJson('smoke-results/smoke-results.json'));
    success = success && result.success;
    text += result.text;
  } catch {
    success = false;
    text += 'No valid, complete native test report was received. See the workflow logs for build, deployment, or timeout errors.';
  }
  try {
    text += `\n\n${versionSummary(readJson('smoke-results/server-version.json'))}`;
  } catch {
    success = false;
    text += '\n\nCS2 version unavailable (the server may not have reached the test stage).';
  }
  const conclusion = success ? 'success' : jobs.includes('cancelled') ? 'cancelled' : 'failure';
  const title = `${checkName}: ${conclusion === 'success' ? 'passed' : conclusion}`;
  const { data: pr } = await github.rest.pulls.get({ ...context.repo, pull_number: context.payload.issue.number });
  let summary = `**Tested commit:** ${env.TESTED_SHA}\n\n${text}\n\n` +
    `Native build: ${env.NATIVE_RESULT}. Managed build/unit tests: ${env.MANAGED_RESULT}. Server run: ${env.SMOKE_RESULT}.\n\n` +
    `[Logs and artifacts (JSON, Markdown, steam.inf, unit-test TRX)](${runUrl(context)})`;
  if (pr.head.sha !== env.TESTED_SHA) summary += '\n\n⚠️ The PR has newer commits. This result does **not** cover the latest head; comment `/smoke-test` again to test it.';
  await github.rest.checks.update({
    ...context.repo,
    check_run_id: Number(env.CHECK_ID),
    status: 'completed',
    conclusion,
    completed_at: new Date().toISOString(),
    output: { title, summary },
  });
  await github.rest.issues.updateComment({
    ...context.repo,
    comment_id: Number(env.COMMENT_ID),
    body: `### ${title}\n\n${summary}`,
  });
  await core.summary.addRaw(`### ${title}\n\n${summary}`).write();
}

module.exports = { authorize, isMaintainerRequest, report, resultSummary, versionSummary };
