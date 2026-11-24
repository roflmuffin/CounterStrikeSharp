using System.Linq;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Core.Logging;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

/// <summary>
/// Where a core memory function (a <see cref="VirtualFunctions"/> member) gets its address. Nothing is resolved until the
/// function is first called, hooked or its handle read, so a failed Binding or missing Gamedata key only affects its own
/// helpers, never the whole table (ADR-0002).
/// <list type="bullet">
/// <item><see cref="FromEngineFeature"/>: the address of a Signature Binding from the Resolution Report. Calling while the
/// Engine Feature is disabled throws <see cref="EngineFeatureUnavailableException"/>; hooking registers, logs once and
/// never fires.</item>
/// <item><see cref="FromGamedata"/>: core Gamedata key not yet declared as an Engine Feature (ticket 12 migrates them);
/// resolved by signature scan on first use, failures throw for that helper only.</item>
/// </list>
/// </summary>
internal sealed class CoreFunction
{
    // Seams for unit tests (NativeAPI is not callable outside the server).
    internal static Func<nint, DataType, DataType[], IntPtr> CreateFunction = (address, returnType, argumentTypes) =>
        NativeAPI.CreateVirtualFunctionFromAddress(address, argumentTypes.Length, (int)returnType,
            argumentTypes.Cast<object>().ToArray());

    internal static Func<string, DataType, DataType[], IntPtr> CreateFunctionFromGamedata = (key, returnType, argumentTypes) =>
        SignatureFunctions.Create(GameData.GetSignature(key), Addresses.ServerPath, returnType, argumentTypes, strict: false);

    private static ILogger? _logger;

    // Setting null restores the core logger.
    [System.Diagnostics.CodeAnalysis.AllowNull]
    internal static ILogger Logger
    {
        get => _logger ??= CoreLogging.Factory.CreateLogger("MemoryFunction");
        set => _logger = value;
    }

    private CoreFunction(EngineFeatureDeclaration? feature, string key)
    {
        Feature = feature;
        Key = key;
    }

    /// <summary>null for <see cref="FromGamedata"/>.</summary>
    public EngineFeatureDeclaration? Feature { get; }

    /// <summary>The core Gamedata key.</summary>
    public string Key { get; }

    /// <summary>The Signature Binding <paramref name="key"/> of <paramref name="feature"/>, from the Resolution Report.</summary>
    public static CoreFunction FromEngineFeature(EngineFeatureDeclaration feature, string key) => new(feature, key);

    /// <summary>A core Gamedata signature not yet declared as an Engine Feature.</summary>
    public static CoreFunction FromGamedata(string key) => new(null, key);

    /// <summary>Creates the native function. Throws <see cref="EngineFeatureUnavailableException"/> if the Engine Feature is disabled.</summary>
    internal IntPtr Bind(DataType returnType, DataType[] parameters)
    {
        if (Feature == null)
        {
            var function = CreateFunctionFromGamedata(Key, returnType, parameters);
            if (function == IntPtr.Zero)
                throw new NativeException($"Core memory function {Key} is not available: signature could not be bound");
            return function;
        }

        return CreateFunction(EngineFeatures.Current.GetAddress(Feature, Key), returnType, parameters);
    }

    /// <summary>
    /// Whether a hook may be installed. On a disabled Engine Feature the hook stays a no-op: warned once, never fires.
    /// </summary>
    internal bool CanHook() =>
        Feature == null || EngineFeatures.Current.CheckRegistration(Feature.Name, $"Memory function hook on {Key}", Logger);

    /// <summary>Whether anything can have been hooked (false on a disabled Engine Feature, where hooks are no-ops).</summary>
    internal bool CanUnhook() => Feature == null || EngineFeatures.Current.IsAvailable(Feature.Name);
}
