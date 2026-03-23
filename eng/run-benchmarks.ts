#!/usr/bin/env -S deno run --allow-run --allow-read --allow-env --allow-net --allow-write

import $ from "https://deno.land/x/dax@0.39.2/mod.ts";
import { load } from "https://deno.land/std@0.224.0/dotenv/mod.ts";
import { z } from "https://deno.land/x/zod@v3.22.4/mod.ts";

const env = await load({ export: true });

const config = z.object({
  // Game server RCON details
  GS_HOST: z.string().min(1, "GS_HOST env var is required"),
  GS_PORT: z.string().min(1, "GS_PORT env var is required"),
  GS_PASS: z.string().min(1, "GS_PASS env var is required"),
  // SFTP connection details
  SFTP_HOST: z.string().min(1, "SFTP_HOST env var is required"),
  SFTP_USER: z.string().min(1, "SFTP_USER env var is required"),
  SFTP_PASS: z.string().min(1, "SFTP_PASS env var is required"),
  // Remote paths on the game server
  GS_PLUGIN_DIR: z.string().default("/game/csgo/addons/counterstrikesharp/plugins/NativeTestsPlugin"),
  GS_API_DIR: z.string().default("/game/csgo/addons/counterstrikesharp/api"),
  GS_NATIVE_DIR: z.string().default("/game/csgo/addons/counterstrikesharp/bin/linuxsteamrt64"),
  // Pterodactyl panel details for server restart
  PTERO_URL: z.string().min(1, "PTERO_URL env var is required (e.g. https://panel.example.com)"),
  PTERO_API_KEY: z.string().min(1, "PTERO_API_KEY env var is required (client API key)"),
  PTERO_SERVER_ID: z.string().min(1, "PTERO_SERVER_ID env var is required (short server identifier)"),
  // How long to wait for the server to come back after restart (seconds)
  GS_RESTART_TIMEOUT: z.string().default("120"),
}).parse(env);

const HERE = $.path(import.meta).parentOrThrow();
const PROJECT_ROOT = HERE.parentOrThrow();
const API_PROJECT = PROJECT_ROOT.join("managed/CounterStrikeSharp.API");
const API_BUILD_OUTPUT = PROJECT_ROOT.join("managed/CounterStrikeSharp.API/bin/Release/net8.0");
const NATIVE_TESTS_PROJECT = PROJECT_ROOT.join("managed/CounterStrikeSharp.Tests.Native/NativeTestsPlugin.csproj");
const BUILD_OUTPUT = PROJECT_ROOT.join("managed/CounterStrikeSharp.Tests.Native/bin/Debug/net8.0");
const NATIVE_SO = PROJECT_ROOT.join("build/addons/counterstrikesharp/bin/linuxsteamrt64/counterstrikesharp.so");
const RESULTS_OUTPUT = PROJECT_ROOT.join("TestResults/Benchmarks");

// ── Step 1: Build the API and the native tests plugin ───────────────────

$.logStep("Building CounterStrikeSharp.API...");
await $`dotnet build -c Release`.cwd(API_PROJECT.toString());

$.logStep("Building NativeTestsPlugin...");
await $`dotnet build ${NATIVE_TESTS_PROJECT} -c Debug`;

// ── Step 2: Upload the API and plugin to the game server via SFTP ───────

$.logStep("Uploading CounterStrikeSharp.API to game server...");
const remoteApiDir = config.GS_API_DIR;
const remotePluginDir = config.GS_PLUGIN_DIR;

await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; mkdir -p ${remoteApiDir}; mirror -R --delete ${API_BUILD_OUTPUT} ${remoteApiDir}; bye`
  }`.quiet();

// Upload native .so if it exists (built with ninja)
if (await Deno.stat(NATIVE_SO.toString()).then(() => true).catch(() => false)) {
  $.logStep("Uploading native counterstrikesharp.so to game server...");
  const remoteNativeDir = config.GS_NATIVE_DIR;
  await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; mkdir -p ${remoteNativeDir}; put ${NATIVE_SO} -o ${remoteNativeDir}/counterstrikesharp.so; bye`
    }`.quiet();
}

$.logStep("Uploading NativeTestsPlugin to game server...");
await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; mkdir -p ${remotePluginDir}; mirror -R --delete ${BUILD_OUTPUT} ${remotePluginDir}; bye`
  }`.quiet();

// ── Step 3: Restart the game server via Pterodactyl and wait ────────────

const rcon = `${HERE}/rcon`;
const restartTimeout = parseInt(config.GS_RESTART_TIMEOUT, 10);
const pteroHeaders = {
  "Authorization": `Bearer ${config.PTERO_API_KEY}`,
  "Content-Type": "application/json",
  "Accept": "application/json",
};
const pteroBaseUrl = `${config.PTERO_URL}/api/client/servers/${config.PTERO_SERVER_ID}`;

$.logStep("Restarting game server via Pterodactyl...");
const restartResp = await fetch(`${pteroBaseUrl}/power`, {
  method: "POST",
  headers: pteroHeaders,
  body: JSON.stringify({ signal: "restart" }),
});

if (!restartResp.ok) {
  const body = await restartResp.text();
  $.logError(`Pterodactyl restart failed (${restartResp.status}): ${body}`);
  Deno.exit(1);
}

$.logStep(`Waiting for game server to come back (timeout: ${restartTimeout}s)...`);
// Wait a bit before polling to give the server time to shut down
await $.sleep(10_000);

const startTime = Date.now();
const pollInterval = 5_000; // 5 seconds between polls
let serverReady = false;

while (Date.now() - startTime < restartTimeout * 1000) {
  try {
    const response = await $`${rcon} -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} "status"`.text();
    if (response) {
      serverReady = true;
      break;
    }
  } catch {
    const elapsed = Math.round((Date.now() - startTime) / 1000);
    $.logLight(`  Server not ready yet (${elapsed}s elapsed)...`);
  }
  await $.sleep(pollInterval);
}

if (!serverReady) {
  $.logError(`Game server did not come back within ${restartTimeout}s!`);
  Deno.exit(1);
}

$.logStep("Game server is back online!");

// Give the server a few extra seconds to fully initialize plugins
await $.sleep(5_000);

// ── Step 4: Load the plugin and run benchmarks via RCON ─────────────────

$.logStep("Loading plugin via RCON...");
try {
  await $`${rcon} -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} "css_plugins load NativeTestsPlugin"`.text();
} catch {
  // Plugin may already be loaded; try reloading
  await $`${rcon} -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} "css_plugins reload NativeTestsPlugin"`.text();
}

$.logStep("Running benchmarks via RCON: css_itest benchmark ...");
const benchOutput = await $`${rcon} -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} -T 120s "css_itest benchmark"`.text();
console.log(benchOutput);

const remoteJsonPath = `${remotePluginDir}/benchmark-results.json`;
const remoteMdPath = `${remotePluginDir}/benchmark-results.md`;

// ── Step 5: Download the benchmark results ──────────────────────────────

$.logStep("Downloading benchmark results...");
await Deno.mkdir(RESULTS_OUTPUT.toString(), { recursive: true });

const localJsonPath = RESULTS_OUTPUT.join("benchmark-results.json").toString();
const localMdPath = RESULTS_OUTPUT.join("benchmark-results.md").toString();

await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; get ${remoteJsonPath} -o ${localJsonPath}; get ${remoteMdPath} -o ${localMdPath}; bye`
  }`.quiet();

$.logStep("Benchmark results downloaded successfully!");
$.logLight(`  JSON: ${localJsonPath}`);
$.logLight(`  MD:   ${localMdPath}`);

// Print the markdown summary
const mdContent = await Deno.readTextFile(localMdPath);
console.log("\n" + mdContent);
