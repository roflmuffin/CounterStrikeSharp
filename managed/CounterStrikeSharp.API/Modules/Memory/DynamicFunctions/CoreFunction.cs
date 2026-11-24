using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Core.Logging;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

/// <summary>
/// Where a core memory function (a <see cref="VirtualFunctions"/> member) gets its address. Nothing is resolved until the
/// function is first called, hooked or its handle read, so a failed Binding or missing Gamedata key only affects its own
/// helpers, never the whole table (ADR-0002). The address is that of a Signature Binding from the Resolution Report
/// (<see cref="FromEngineFeature"/>). Calling while the Engine Feature is disabled throws
/// <see cref="EngineFeatureUnavailableException"/>; hooking registers, logs once and never fires.
/// </summary>
internal sealed class CoreFunction
{
    // Seams for unit tests (NativeAPI is not callable outside the server).
    internal static Func<nint, DataType, DataType[], IntPtr> CreateFunction = (address, returnType, argumentTypes) =>
        NativeAPI.CreateVirtualFunctionFromAddress(address, argumentTypes.Length, (int)returnType,
            argumentTypes.Cast<object>().ToArray());

    private static ILogger? _logger;

    // Setting null restores the core logger.
    [System.Diagnostics.CodeAnalysis.AllowNull]
    internal static ILogger Logger
    {
        get => _logger ??= CoreLogging.Factory.CreateLogger("MemoryFunction");
        set => _logger = value;
    }

    private CoreFunction(EngineFeatureDeclaration feature, string key)
    {
        Feature = feature;
        Key = key;
    }

    public EngineFeatureDeclaration Feature { get; }

    /// <summary>The core Gamedata key.</summary>
    public string Key { get; }

    /// <summary>The Signature Binding <paramref name="key"/> of <paramref name="feature"/>, from the Resolution Report.</summary>
    public static CoreFunction FromEngineFeature(EngineFeatureDeclaration feature, string key) => new(feature, key);

    /// <summary>Creates the native function. Throws <see cref="EngineFeatureUnavailableException"/> if the Engine Feature is disabled.</summary>
    internal IntPtr Bind(DataType returnType, DataType[] parameters) =>
        CreateFunction(EngineFeatures.Current.GetAddress(Feature, Key), returnType, parameters);

    /// <summary>
    /// Whether a hook may be installed. On a disabled Engine Feature the hook stays a no-op: warned once per registrant
    /// (the assembly of the hooking plugin), never fires.
    /// </summary>
    internal bool CanHook(object registrant) =>
        EngineFeatures.Current.CheckRegistration(Feature.Name, $"Memory function hook on {Key}", Logger, registrant);

    /// <summary>Whether anything can have been hooked (false on a disabled Engine Feature, where hooks are no-ops).</summary>
    internal bool CanUnhook() => EngineFeatures.Current.IsAvailable(Feature.Name);
}
