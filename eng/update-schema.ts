#!/usr/bin/env -S deno run --allow-run --allow-read --allow-env --allow-net

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
}).parse(env);

const HERE = $.path('.');

$.logStep("Dumping schema from game server via RCON...");
const output = await $`${HERE}/rcon -a ${config.GS_HOST}:${config.GS_PORT} -p ${config.GS_PASS} "dump_schema all"`.text();

// Extract the file path from RCON output
const match = output.match(/Wrote file output to (.+)/);
if (!match) {
  throw new Error("Could not find schema output path in RCON response");
}
const filepath = match[1].trim();
const trimmedPath = filepath.replace(/^\/home\/container/, "");

$.logStep("Downloading schema file from game server via SFTP...");
const schemaOutput = `${HERE}/../managed/CounterStrikeSharp.SchemaGen/Schema/server.json`;
await $`lftp -u ${config.SFTP_USER},${config.SFTP_PASS} ${config.SFTP_HOST} -e ${"set xfer:clobber on; get " + trimmedPath + " -o " + schemaOutput + "; bye"}`.quiet();

const schemaGenProject = `${HERE}/../managed/CounterStrikeSharp.SchemaGen/CounterStrikeSharp.SchemaGen.csproj`;
const generatedSchemaDir = `${HERE}/../managed/CounterStrikeSharp.API/Generated/Schema`;
$.logStep("Generating C# schema classes...");
await $`dotnet run --project ${schemaGenProject} -- ${generatedSchemaDir}`;
