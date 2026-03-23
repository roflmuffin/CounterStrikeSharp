#!/usr/bin/env -S deno run --allow-run --allow-read --allow-env --allow-net --allow-write

import $ from "https://deno.land/x/dax@0.39.2/mod.ts";
import { load } from "https://deno.land/std@0.224.0/dotenv/mod.ts";
import { z } from "https://deno.land/x/zod@v3.22.4/mod.ts";

const env = await load({ export: true });

const config = z.object({
  GS_HOST: z.string().min(1),
  GS_PORT: z.string().min(1),
  GS_PASS: z.string().min(1),
  SFTP_HOST: z.string().min(1),
  SFTP_USER: z.string().min(1),
  SFTP_PASS: z.string().min(1),
  GS_PLUGIN_DIR: z.string().default("/game/csgo/addons/counterstrikesharp/plugins/NativeTestsPlugin"),
  GS_API_DIR: z.string().default("/game/csgo/addons/counterstrikesharp/api"),
  GS_NATIVE_DIR: z.string().default("/game/csgo/addons/counterstrikesharp/bin/linuxsteamrt64"),
  PTERO_URL: z.string().min(1),
  PTERO_API_KEY: z.string().min(1),
  PTERO_SERVER_ID: z.string().min(1),
  GS_RESTART_TIMEOUT: z.string().default("120"),
}).parse(env);

const HERE = $.path(import.meta).parentOrThrow();
const ROOT = HERE.parentOrThrow();

const paths = {
  apiProject: ROOT.join("managed/CounterStrikeSharp.API"),
  apiBuild: ROOT.join("managed/CounterStrikeSharp.API/bin/Release/net8.0"),
  testProject: ROOT.join("managed/CounterStrikeSharp.Tests.Native/NativeTestsPlugin.csproj"),
  testBuild: ROOT.join("managed/CounterStrikeSharp.Tests.Native/bin/Debug/net8.0"),
  nativeSo: ROOT.join("build/addons/counterstrikesharp/bin/linuxsteamrt64/counterstrikesharp.so"),
  results: ROOT.join("TestResults/Benchmarks"),
};

const rcon = `${HERE}/rcon`;
const remoteJson = `${config.GS_PLUGIN_DIR}/benchmark-results.json`;
const remoteMd = `${config.GS_PLUGIN_DIR}/benchmark-results.md`;

async function poll(timeoutSec: number, intervalMs: number, label: string, check: () => Promise<void>) {
  const start = Date.now();
  while (Date.now() - start < timeoutSec * 1000) {
    try {
      await check();
      return;
    } catch {
      const elapsed = Math.round((Date.now() - start) / 1000);
      $.logLight(`  ${label} (${elapsed}s elapsed)...`);
    }
    await $.sleep(intervalMs);
  }
  throw new Error(`${label}: timed out after ${timeoutSec}s`);
}

// ── Build ───────────────────────────────────────────────────────────────

$.logStep("Building API...");
await $`dotnet build -c Release`.cwd(paths.apiProject.toString());

$.logStep("Building NativeTestsPlugin...");
await $`dotnet build ${paths.testProject} -c Debug`;

// ── Upload ──────────────────────────────────────────────────────────────

$.logStep("Uploading API...");
await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; mkdir -p ${config.GS_API_DIR}; mirror -R --delete ${paths.apiBuild} ${config.GS_API_DIR}; bye`}`.quiet();

if (await Deno.stat(paths.nativeSo.toString()).then(() => true).catch(() => false)) {
  $.logStep("Uploading native .so...");
  await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; mkdir -p ${config.GS_NATIVE_DIR}; put ${paths.nativeSo} -o ${config.GS_NATIVE_DIR}/counterstrikesharp.so; bye`}`.quiet();
}

$.logStep("Uploading NativeTestsPlugin...");
await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; mkdir -p ${config.GS_PLUGIN_DIR}; mirror -R --delete ${paths.testBuild} ${config.GS_PLUGIN_DIR}; bye`}`.quiet();

// ── Restart server ──────────────────────────────────────────────────────

const pteroHeaders = {
  "Authorization": `Bearer ${config.PTERO_API_KEY}`,
  "Content-Type": "application/json",
  "Accept": "application/json",
};

$.logStep("Restarting game server...");
const resp = await fetch(`${config.PTERO_URL}/api/client/servers/${config.PTERO_SERVER_ID}/power`, {
  method: "POST",
  headers: pteroHeaders,
  body: JSON.stringify({ signal: "restart" }),
});
if (!resp.ok) {
  $.logError(`Restart failed (${resp.status}): ${await resp.text()}`);
  Deno.exit(1);
}

await $.sleep(10_000);

const restartTimeout = parseInt(config.GS_RESTART_TIMEOUT, 10);
await poll(restartTimeout, 5_000, "Server not ready yet", async () => {
  const out = await $`${rcon} -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} "status"`.text();
  if (!out) throw new Error();
});

$.logStep("Server is back online.");
await $.sleep(5_000);

// ── Run benchmarks ──────────────────────────────────────────────────────

$.logStep("Loading plugin...");
try {
  await $`${rcon} -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} "css_plugins load NativeTestsPlugin"`.text();
} catch {
  await $`${rcon} -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} "css_plugins reload NativeTestsPlugin"`.text();
}

// Delete stale results so we can detect when new ones land
$.logStep("Clearing old results...");
try {
  await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`rm -f ${remoteJson}; rm -f ${remoteMd}; bye`}`.quiet();
} catch { /* may not exist */ }

$.logStep("Running: css_itest benchmark");
console.log(await $`${rcon} -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} -T 120s "css_itest benchmark"`.text());

// Async benchmarks (entity creation) span multiple frames, so the RCON
// command returns before results are written. Poll until the file appears.
await poll(300, 5_000, "Waiting for results", async () => {
  await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`cat ${remoteJson}; bye`}`.quiet();
});

await $.sleep(2_000); // let the .md flush too

// ── Download results ────────────────────────────────────────────────────

$.logStep("Downloading results...");
await Deno.mkdir(paths.results.toString(), { recursive: true });

const localJson = paths.results.join("benchmark-results.json").toString();
const localMd = paths.results.join("benchmark-results.md").toString();

await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; get ${remoteJson} -o ${localJson}; get ${remoteMd} -o ${localMd}; bye`}`.quiet();

$.logLight(`  JSON: ${localJson}`);
$.logLight(`  MD:   ${localMd}`);
console.log("\n" + await Deno.readTextFile(localMd));
