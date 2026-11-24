namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// The managed Engine Feature catalog: Engine Features whose Bindings managed helpers consume (ADR-0002). Native-owned
/// Engine Features are declared in <c>src/core/engine_features.cpp</c> and arrive with the Resolution Report.
/// <para>
/// To add one: add a field (name as a string literal, never renamed or reused; Gamedata keys verbatim) and list it in
/// <see cref="All"/>, then regenerate the <see cref="EngineFeature"/> enum with
/// <c>dotnet run --project tooling/CodeGen.Natives -- engine-features</c> (it reads the names from this file).
/// Core memory functions bind to a Signature Binding lazily with <c>CoreFunction.FromEngineFeature(feature, key)</c>
/// (see <c>VirtualFunctions</c>); other helpers take their addresses/offsets via
/// <c>EngineFeatures.Current.GetAddress(feature, key)</c> / <c>GetOffset(feature, key)</c>. All of them throw
/// <see cref="EngineFeatureUnavailableException"/> when the Engine Feature is disabled.
/// </para>
/// </summary>
internal static class ManagedEngineFeatures
{
    // ticket 06
    public static readonly EngineFeatureDeclaration ChatClientPrint = new("cssharp.chat/client-print",
        [BindingDeclaration.Signature("ClientPrint"), BindingDeclaration.Signature("UTIL_ClientPrintAll")]);

    /// <summary>Every managed Engine Feature. Keep after the fields above (static initialisation order).</summary>
    public static readonly IReadOnlyList<EngineFeatureDeclaration> All =
    [
        // ticket 06
        ChatClientPrint,
    ];
}
