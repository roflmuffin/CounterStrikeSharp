#!/usr/bin/env -S deno run --allow-run --allow-read --allow-env --allow-net --allow-write

// Run this TRUSTED script against prebuilt artifacts; never build/execute PR code
// on the runner that holds server credentials. Unlike run-benchmarks.ts, no .env
// is loaded and a fresh native build is mandatory.
import { z } from "https://deno.land/x/zod@v3.22.4/mod.ts";

const timeout = z.coerce.number().int().positive().max(1800);
const remotePath = z.string().regex(/^\/[a-zA-Z0-9_./-]+$/);
const config = z.object({
    GS_HOST: z.string().min(1),
    GS_PORT: z.string().regex(/^\d+$/),
    GS_PASS: z.string().min(1),
    SFTP_HOST: z.string().startsWith("sftp://"),
    SFTP_USER: z.string().min(1),
    SFTP_PASS: z.string().min(1),
    GS_ADDON_DIR: remotePath.default("/game/csgo/addons/counterstrikesharp"),
    GS_STEAM_INF: remotePath.default("/game/csgo/steam.inf"),
    PTERO_URL: z.string().url(),
    PTERO_API_KEY: z.string().min(1),
    PTERO_SERVER_ID: z.string().regex(/^[a-zA-Z0-9-]+$/),
    GS_RESTART_TIMEOUT: timeout.default(120),
    GS_TEST_TIMEOUT: timeout.default(600),
    SMOKE_COMMIT: z.string().regex(/^[a-f0-9]{40}$/).optional(),
}).parse(Deno.env.toObject());

const [artifactDir, resultsDir] = Deno.args;
if (!artifactDir || !resultsDir) {
    throw new Error("Usage: run-smoke-tests.ts <artifact-directory> <results-directory>");
}
const root = await Deno.realPath(artifactDir);
await Deno.mkdir(resultsDir, { recursive: true });
const results = await Deno.realPath(resultsDir);
const runId = crypto.randomUUID();
const addon = config.GS_ADDON_DIR;
const plugin = `${addon}/plugins/NativeTestsPlugin`;
const rcon = new URL("./rcon", import.meta.url).pathname;

// Never include command arguments or stderr in errors: they can contain secrets.
async function command(program: string, args: string[]) {
    const result = await new Deno.Command(program, {
        args,
        stdout: "piped",
        stderr: "piped",
    }).output();
    if (!result.success) throw new Error(`${program.split("/").pop()} failed (exit ${result.code})`);
    return new TextDecoder().decode(result.stdout);
}
const quote = (value: string) => '"' + value.replaceAll("\\", "\\\\").replaceAll('"', '\\"') + '"';
const sftp = (commands: string) =>
    command("lftp", [
        "-u",
        `${config.SFTP_USER},${config.SFTP_PASS}`,
        config.SFTP_HOST,
        "-e",
        `set cmd:fail-exit yes; set net:timeout 15; set net:max-retries 1; set xfer:clobber on; ${commands}; bye`,
    ]);
const consoleCommand = (text: string) =>
    command(rcon, [
        "-a",
        `${config.GS_HOST}:${config.GS_PORT}`,
        "-p",
        config.GS_PASS,
        "-T",
        "15s",
        text,
    ]);

async function panel(path: string, signal?: string) {
    const response = await fetch(`${config.PTERO_URL.replace(/\/$/, "")}/api/client/servers/${config.PTERO_SERVER_ID}/${path}`, {
        method: signal ? "POST" : "GET",
        headers: {
            Authorization: `Bearer ${config.PTERO_API_KEY}`,
            "Content-Type": "application/json",
            Accept: "application/json",
        },
        body: signal ? JSON.stringify({ signal }) : undefined,
        signal: AbortSignal.timeout(15_000),
    });
    if (!response.ok) throw new Error(`Panel ${path} failed (HTTP ${response.status})`);
    return response;
}

async function poll(seconds: number, label: string, check: () => Promise<void>) {
    const deadline = Date.now() + seconds * 1000;
    while (Date.now() < deadline) {
        try {
            await check();
            return;
        } catch {
            console.log(`${label}…`);
            await new Promise((resolve) => setTimeout(resolve, 5000));
        }
    }
    throw new Error(`${label}: timed out after ${seconds}s`);
}

// Fail before touching the server if any build is missing.
for (
    const file of [
        "native/addons/counterstrikesharp/bin/linuxsteamrt64/counterstrikesharp.so",
        "api/CounterStrikeSharp.API.dll",
        "plugin/NativeTestsPlugin.dll",
    ]
) {
    if (!(await Deno.stat(`${root}/${file}`)).isFile) throw new Error(`Missing build: ${file}`);
}

console.log("Stopping the dedicated test server…");
await panel("power", "stop");
await poll(config.GS_RESTART_TIMEOUT, "Waiting for server to stop", async () => {
    const response = await (await panel("resources")).json();
    if (response.attributes.current_state !== "offline") throw new Error("Not offline");
});

console.log("Deploying native binaries, configs, API and test plugin…");
// Do not delete the server's runtime or unrelated addons. API/plugin directories
// are replaced completely to avoid accidentally using assemblies from an old PR.
await sftp(
    `mirror -R ${quote(`${root}/native/addons/counterstrikesharp`)} ${quote(addon)}; ` +
        `mkdir -p ${quote(`${addon}/api`)} ${quote(plugin)}; ` +
        `mirror -R --delete ${quote(`${root}/api`)} ${quote(`${addon}/api`)}; ` +
        `mirror -R --delete ${quote(`${root}/plugin`)} ${quote(plugin)}`,
);

console.log("Starting the server…");
await panel("power", "start");
await poll(config.GS_RESTART_TIMEOUT, "Waiting for RCON", async () => {
    if (!(await consoleCommand("status")).trim()) throw new Error("Empty RCON response");
});
// Allow plugin startup and map initialization to finish.
await new Promise((resolve) => setTimeout(resolve, 10_000));
// Read after startup: the hosting panel may update CS2 when starting it.
await sftp(`get ${quote(config.GS_STEAM_INF)} -o ${quote(`${results}/steam.inf`)}`);
const steamInfo = Object.fromEntries(
    (await Deno.readTextFile(`${results}/steam.inf`))
        .split(/\r?\n/).filter((line) => line.includes("=")).map((line) => {
            const separator = line.indexOf("=");
            return [line.slice(0, separator).trim(), line.slice(separator + 1).trim()];
        }),
);
const counterStrike = z.object({
    ClientVersion: z.string().regex(/^\d+$/),
    ServerVersion: z.string().regex(/^\d+$/),
    PatchVersion: z.string().regex(/^\d+(\.\d+)+$/),
    SourceRevision: z.string().regex(/^\d+$/),
    VersionDate: z.string().regex(/^[a-zA-Z0-9 ,/-]+$/),
    VersionTime: z.string().regex(/^[0-9:]+$/),
}).parse(steamInfo);
await Deno.writeTextFile(`${results}/server-version.json`, JSON.stringify(counterStrike, null, 2) + "\n");

console.log("Running all non-benchmark native tests…");
try {
    await consoleCommand(`css_smoke_test ${runId}`);
} catch {
    // Synchronous tests can delay RCON's reply. A matching, complete report is
    // authoritative; a crash or a missing plugin will instead time out below.
    console.log("RCON did not return cleanly; waiting for the test report.");
}

const count = z.number().int().nonnegative();
const reportSchema = z.object({
    runId: z.literal(runId),
    total: count,
    passed: count,
    failed: count,
    skipped: count,
    errors: z.array(z.string()),
    tests: z.array(z.object({ outcome: z.enum(["passed", "failed", "skipped"]) }).passthrough()),
}).superRefine((report, ctx) => {
    if (
        report.total !== report.passed + report.failed + report.skipped ||
        report.tests.length !== report.total ||
        ["passed", "failed", "skipped"].some((outcome) =>
            report.tests.filter((test) => test.outcome === outcome).length !== report[outcome as "passed" | "failed" | "skipped"]
        )
    ) ctx.addIssue({ code: "custom", message: "Inconsistent test counts" });
});

let report: z.infer<typeof reportSchema> | undefined;
await poll(config.GS_TEST_TIMEOUT, "Waiting for a complete report", async () => {
    await sftp(`get ${quote(`${plugin}/smoke-results.json`)} -o ${quote(`${results}/smoke-results.pending.json`)}`);
    report = reportSchema.parse(JSON.parse(await Deno.readTextFile(`${results}/smoke-results.pending.json`)));
});
if (!report) throw new Error("No report received");
await Deno.writeTextFile(
    `${results}/smoke-results.json`,
    JSON.stringify({ ...report, testedCommit: config.SMOKE_COMMIT, counterStrike }, null, 2) + "\n",
);
await Deno.remove(`${results}/smoke-results.pending.json`);
const success = report.passed > 0 && report.failed === 0 && report.errors.length === 0;
const summary = `### Native smoke test: ${success ? "passed" : "failed"}\n\n` +
    (config.SMOKE_COMMIT ? `**Tested commit:** ${config.SMOKE_COMMIT}\n\n` : "") +
    `${report.total} tests: **${report.passed} passed**, **${report.failed} failed**, **${report.skipped} skipped**.\n\n` +
    `Runner/cleanup errors: ${report.errors.length}. Benchmarks excluded.\n\n` +
    `**CS2:** ${counterStrike.PatchVersion} (server ${counterStrike.ServerVersion}, client ${counterStrike.ClientVersion})\n\n` +
    `**Source revision:** ${counterStrike.SourceRevision} — ${counterStrike.VersionDate} ${counterStrike.VersionTime}\n`;
await Deno.writeTextFile(`${results}/smoke-results.md`, summary);
console.log(summary);
if (!success) Deno.exit(1);
