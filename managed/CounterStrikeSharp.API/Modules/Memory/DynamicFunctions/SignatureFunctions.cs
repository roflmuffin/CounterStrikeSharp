using System.Collections.Concurrent;
using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Core.Logging;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

/// <summary>
/// Creates native functions from raw signatures passed in by plugins (directly, or from Gamedata files they ship).
/// Keeps first-match binding, but counts the matches once per signature: an ambiguous signature is warned about once
/// with its match count, or rejected in strict mode. A signature that cannot be bound is logged once at creation and
/// remembered, so calling the function throws a clear error instead of "Invalid function pointer". Every outcome is
/// recorded for the plugin signatures report (<see cref="Core.Bindings.PluginSignatures"/>).
/// </summary>
internal static class SignatureFunctions
{
    // Seams for unit tests (NativeAPI is not callable outside the server).
    internal static Func<string, string, int> CountMatches = NativeAPI.CountSignatureMatches;

    internal static Func<string, string, DataType, DataType[], IntPtr> CreateFunction =
        (binaryPath, signature, returnType, argumentTypes) =>
            NativeAPI.CreateVirtualFunctionBySignature(IntPtr.Zero, binaryPath, signature, argumentTypes.Length, (int)returnType,
                argumentTypes.Cast<object>().ToArray());

    private static ILogger? _logger;

    // Setting null restores the core logger.
    [System.Diagnostics.CodeAnalysis.AllowNull]
    internal static ILogger Logger
    {
        get => _logger ??= CoreLogging.Factory.CreateLogger("MemoryFunction");
        set => _logger = value;
    }

    private static readonly ConcurrentDictionary<string, IntPtr> CreatedFunctions = new();

    // Match count per binary:signature, scanned once (or InvalidPattern / Unknown).
    private static readonly ConcurrentDictionary<string, int> MatchCounts = new();

    private static readonly ConcurrentDictionary<string, byte> AmbiguityWarned = new();

    // Why a binary:signature could not be bound ("not found", "ambiguous (3 matches)"); logged once when added.
    private static readonly ConcurrentDictionary<string, string> Failures = new();

    internal static IntPtr Create(string signature, string binaryPath, DataType returnType, DataType[] argumentTypes, bool strict)
    {
        var signatureKey = $"{binaryPath}:{signature}";
        var matchCount = MatchCounts.GetOrAdd(signatureKey, _ => Count(binaryPath, signature));
        var function = Bind(signatureKey, signature, binaryPath, returnType, argumentTypes, strict, matchCount);

        var outcome = function != IntPtr.Zero
            ? matchCount > 1 ? PluginSignatureOutcome.Ambiguous : PluginSignatureOutcome.Resolved
            : matchCount switch
            {
                > 1 when strict => PluginSignatureOutcome.AmbiguousRejected,
                0 => PluginSignatureOutcome.NotFound,
                InvalidPattern => PluginSignatureOutcome.InvalidPattern,
                _ => PluginSignatureOutcome.Unbound, // matched (or not counted), but no function could be created
            };
        PluginSignatures.Record(binaryPath, signature, matchCount, outcome);
        return function;
    }

    private static IntPtr Bind(string signatureKey, string signature, string binaryPath, DataType returnType, DataType[] argumentTypes,
        bool strict, int matchCount)
    {
        // Before the cache: a strict caller rejects an ambiguous signature even if a non-strict one already bound it.
        if (matchCount > 1 && strict)
        {
            Fail(signatureKey, signature, binaryPath, $"ambiguous ({matchCount} matches)");
            return IntPtr.Zero;
        }

        var cacheKey = BaseMemoryFunction.FunctionCacheKey(signatureKey, returnType, argumentTypes);
        if (CreatedFunctions.TryGetValue(cacheKey, out var function)) return function;

        if (matchCount > 1)
        {
            if (AmbiguityWarned.TryAdd(signatureKey, 0))
            {
                Logger.LogWarning(
                    "Signature \"{Signature}\" in {Binary} is ambiguous ({MatchCount} matches); using the first match. " +
                    "Lengthen it so it matches exactly once, or pass strict: true to reject ambiguous matches",
                    signature, Path.GetFileName(binaryPath), matchCount);
            }
        }

        try
        {
            function = CreateFunction(binaryPath, signature, returnType, argumentTypes);
        }
        catch (Exception)
        {
            function = IntPtr.Zero;
        }

        if (function == IntPtr.Zero)
        {
            Fail(signatureKey, signature, binaryPath, DescribeUnbound(matchCount));
            return IntPtr.Zero;
        }

        CreatedFunctions[cacheKey] = function;
        return function;
    }

    /// <summary>Why the signature could not be bound, or null if it was not rejected.</summary>
    internal static string? FailureReason(string binaryPath, string signature) =>
        Failures.TryGetValue($"{binaryPath}:{signature}", out var reason) ? reason : null;

    // Native returns -1 for an invalid pattern; a scan that throws counts as unknown and never blocks binding.
    private const int InvalidPattern = -1;
    private const int Unknown = -2;

    private static int Count(string binaryPath, string signature)
    {
        try
        {
            return CountMatches(binaryPath, signature);
        }
        catch (Exception)
        {
            return Unknown;
        }
    }

    private static string DescribeUnbound(int matchCount) => matchCount switch
    {
        0 => "not found",
        InvalidPattern => "invalid pattern",
        _ => "could not be bound",
    };

    private static void Fail(string signatureKey, string signature, string binaryPath, string reason)
    {
        if (!Failures.TryAdd(signatureKey, reason)) return;

        Logger.LogError(
            "Signature \"{Signature}\" in {Binary} could not be used: {Reason}. Calling or hooking this memory function will throw",
            signature, Path.GetFileName(binaryPath), reason);
    }
}
