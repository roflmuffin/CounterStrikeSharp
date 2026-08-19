#!/usr/bin/env python3
import importlib
import json
from pathlib import Path
import sys

GamedataPath = (
    Path(__file__).resolve().parent.parent
    / "configs"
    / "addons"
    / "counterstrikesharp"
    / "gamedata"
    / "gamedata.json"
)
LibraryBaseNames = {
    "engine": "engine2",
    "server": "server",
    "tier0": "tier0",
    "vscript": "vscript",
}

AgentSource = r"""
"use strict";

const Status = Object.freeze({
    Found: "FOUND",
    NotFound: "NOT_FOUND",
    NotUnique: "NOT_UNIQUE",
    Invalid: "INVALID",
});

const ExecutableProtection = "r-x";
const WildcardToken = "??";

function normalizeToken(tokenText) {
    if (tokenText.startsWith("?") || tokenText.toUpperCase() === "2A") {
        return WildcardToken;
    }

    return tokenText.slice(0, 2);
}

function normalizePattern(patternText) {
    const sourceText = patternText.trim();

    if (sourceText.length === 0) {
        throw new Error("empty signature");
    }

    if (sourceText.startsWith("@")) {
        throw new Error("symbol signatures are not byte patterns");
    }

    const tokens = sourceText.startsWith("\\x")
        ? sourceText.split("\\x").filter(Boolean)
        : sourceText.split(/\s+/);

    return tokens.map(normalizeToken).join(" ");
}

function findLoadedModule(moduleName) {
    if (typeof Process.findModuleByName === "function") {
        return Process.findModuleByName(moduleName);
    }

    const targetName = moduleName.toLowerCase();
    return Process.enumerateModules().find(moduleInfo => {
        return moduleInfo.name.toLowerCase() === targetName;
    }) || null;
}

function enumerateExecutableRanges(moduleInfo) {
    if (typeof moduleInfo.enumerateRanges === "function") {
        const moduleRanges = moduleInfo.enumerateRanges(ExecutableProtection);
        if (moduleRanges.length > 0) {
            return moduleRanges;
        }
    }

    const moduleStart = moduleInfo.base;
    const moduleEnd = moduleInfo.base.add(moduleInfo.size);

    return Process.enumerateRanges({ protection: ExecutableProtection, coalesce: true }).filter(rangeInfo => {
        const startsInsideModule = rangeInfo.base.compare(moduleStart) >= 0;
        const startsBeforeModuleEnd = rangeInfo.base.compare(moduleEnd) < 0;
        return startsInsideModule && startsBeforeModuleEnd;
    });
}

function scanRanges(ranges, patternText) {
    const pattern = normalizePattern(patternText);
    const matches = [];

    for (const rangeInfo of ranges) {
        const rangeMatches = Memory.scanSync(rangeInfo.base, rangeInfo.size, pattern);
        for (const matchInfo of rangeMatches) {
            matches.push(matchInfo.address.toString());
        }
    }

    return matches;
}

function statusForMatchCount(matchCount) {
    if (matchCount === 0) {
        return Status.NotFound;
    }

    if (matchCount === 1) {
        return Status.Found;
    }

    return Status.NotUnique;
}

function collectModuleRanges(moduleNames) {
    const moduleReports = {};
    const rangesByLibrary = {};

    for (const [libraryName, moduleName] of Object.entries(moduleNames)) {
        const moduleInfo = findLoadedModule(moduleName);
        if (moduleInfo === null) {
            moduleReports[libraryName] = { error: `module not loaded: ${moduleName}` };
            continue;
        }

        const ranges = enumerateExecutableRanges(moduleInfo);
        if (ranges.length === 0) {
            moduleReports[libraryName] = {
                error: `no executable ranges: ${moduleName}`,
                path: moduleInfo.path,
            };
            continue;
        }

        rangesByLibrary[libraryName] = ranges;
        moduleReports[libraryName] = {
            error: null,
            path: moduleInfo.path,
            baseAddress: moduleInfo.base.toString(),
            size: moduleInfo.size,
            segments: ranges.length,
        };
    }

    return { moduleReports, rangesByLibrary };
}

function scanSignature(signatureName, signatureInfo, platformName, moduleReports, rangesByLibrary) {
    const libraryName = signatureInfo.library || "";
    const patternText = signatureInfo[platformName];
    let addresses = [];
    let status = Status.Invalid;
    let note = "";

    if (libraryName.length === 0) {
        note = "missing library";
    } else if (!patternText) {
        status = Status.NotFound;
        note = `missing ${platformName} pattern`;
    } else if (moduleReports[libraryName]?.error) {
        status = Status.NotFound;
        note = moduleReports[libraryName].error;
    } else if (!rangesByLibrary[libraryName]) {
        status = Status.NotFound;
        note = `module not scanned: ${libraryName}`;
    } else {
        try {
            addresses = scanRanges(rangesByLibrary[libraryName], patternText);
            status = statusForMatchCount(addresses.length);
        } catch (error) {
            status = Status.Invalid;
            note = error.message;
        }
    }

    return {
        name: signatureName,
        library: libraryName,
        status,
        matchCount: addresses.length,
        addresses,
        note,
    };
}

rpc.exports = {
    scan(moduleNames, signatureData, platformName) {
        const { moduleReports, rangesByLibrary } = collectModuleRanges(moduleNames);
        const summary = {
            [Status.Found]: 0,
            [Status.NotFound]: 0,
            [Status.NotUnique]: 0,
            [Status.Invalid]: 0,
        };
        const results = [];

        for (const [signatureName, signatureInfo] of Object.entries(signatureData)) {
            const result = scanSignature(
                signatureName,
                signatureInfo,
                platformName,
                moduleReports,
                rangesByLibrary,
            );

            summary[result.status] += 1;
            results.push(result);
        }

        return { moduleReports, results, summary };
    },
};
"""

if __name__ == "__main__":
    try:
        if sys.platform.startswith("win"):
            platformName = "windows"
            processNames = {"cs2.exe"}
        elif sys.platform.startswith("linux"):
            platformName = "linux"
            processNames = {"cs2", "cs2_linux64"}
        else:
            raise RuntimeError(f"unsupported platform: {sys.platform}")

        try:
            frida = importlib.import_module("frida")
        except ImportError as exc:
            raise RuntimeError("missing dependency") from exc

        with GamedataPath.open("r", encoding="utf-8") as fileHandle:
            gamedata = json.load(fileHandle)

        signatureData = {
            name: value["signatures"]
            for name, value in gamedata.items()
            if isinstance(value, dict) and isinstance(value.get("signatures"), dict)
        }
        moduleNames = {}
        for signatureInfo in signatureData.values():
            libraryName = signatureInfo.get("library")
            if libraryName and libraryName not in moduleNames:
                baseName = LibraryBaseNames.get(libraryName, libraryName)
                moduleNames[libraryName] = (
                    f"{baseName}.dll"
                    if platformName == "windows"
                    else f"lib{baseName}.so"
                )

        device = frida.get_local_device()
        processInfo = next(
            (
                item
                for item in device.enumerate_processes()
                if item.name.lower() in processNames
            ),
            None,
        )
        if processInfo is None:
            print("FAILED: cs2 process not found")
            raise SystemExit(1)

        session = device.attach(processInfo.pid)
        try:
            script = session.create_script(AgentSource)
            script.load()
            exports = (
                script.exports_sync
                if hasattr(script, "exports_sync")
                else script.exports
            )
            report = exports.scan(moduleNames, signatureData, platformName)
        finally:
            session.detach()

        summary = report["summary"]
        print(f"Platform: {platformName}")
        print(f"Process: {processInfo.name} pid={processInfo.pid}")
        print(
            "Summary: "
            f"FOUND={summary['FOUND']} "
            f"NOT_FOUND={summary['NOT_FOUND']} "
            f"NOT_UNIQUE={summary['NOT_UNIQUE']} "
            f"INVALID={summary['INVALID']}"
        )
        for result in report["results"]:
            addressText = ", ".join(result["addresses"][:8])
            if result["matchCount"] > 8:
                addressText += f", ... (+{result['matchCount'] - 8})"
            if not addressText:
                addressText = "-"
            noteText = f" ({result['note']})" if result.get("note") else ""
            print(
                f"[{result['status']}] {result['name']} "
                f"library={result['library']} matches={result['matchCount']} address={addressText}{noteText}"
            )

        raise SystemExit(
            1
            if summary["NOT_FOUND"] + summary["NOT_UNIQUE"] + summary["INVALID"]
            else 0
        )
    except Exception as exc:
        print(f"FAILED: {exc}")
        raise SystemExit(1)
