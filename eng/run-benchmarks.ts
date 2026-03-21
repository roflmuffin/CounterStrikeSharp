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
  // Remote plugin path on the game server (under /home/container)
  GS_PLUGIN_DIR: z.string().default("/game/csgo/addons/counterstrikesharp/plugins/NativeTestsPlugin"),
}).parse(env);

const HERE = $.path(import.meta).parentOrThrow();
const PROJECT_ROOT = HERE.parentOrThrow();
const NATIVE_TESTS_PROJECT = PROJECT_ROOT.join("managed/CounterStrikeSharp.Tests.Native/NativeTestsPlugin.csproj");
const BUILD_OUTPUT = PROJECT_ROOT.join("managed/CounterStrikeSharp.Tests.Native/bin/Debug/net8.0");
const RESULTS_OUTPUT = PROJECT_ROOT.join("benchmark-results");

// ── Step 1: Build the native tests plugin ───────────────────────────────

$.logStep("Building NativeTestsPlugin...");
await $`dotnet build ${NATIVE_TESTS_PROJECT} -c Debug`;

// ── Step 2: Upload the built plugin to the game server via SFTP ─────────

$.logStep("Uploading NativeTestsPlugin to game server...");
const remotePluginDir = config.GS_PLUGIN_DIR;

await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${`set xfer:clobber on; mkdir -p ${remotePluginDir}; mirror -R --delete ${BUILD_OUTPUT} ${remotePluginDir}; bye`
  }`.quiet();

// ── Step 3: Reload the plugin and run benchmarks via RCON ───────────────

$.logStep("Reloading plugin via RCON...");
const rcon = `${HERE}/rcon`;
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

// ── Step 4: Download the benchmark results ──────────────────────────────

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
