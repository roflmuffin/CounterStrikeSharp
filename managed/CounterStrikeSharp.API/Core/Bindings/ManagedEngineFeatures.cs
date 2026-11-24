namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// The managed Engine Feature catalog: Engine Features whose Bindings managed helpers consume (ADR-0002). Native-owned
/// Engine Features are declared in <c>src/core/engine_features.cpp</c> and arrive with the Resolution Report.
/// <para>
/// To add one: add a field (name as a string literal, never renamed or reused; Gamedata keys verbatim) and list it in
/// <see cref="All"/>, then regenerate the <see cref="EngineFeature"/> enum with
/// <c>dotnet run --project tooling/CodeGen.Natives -- engine-features</c> (it reads the names from this file).
/// Helpers take their addresses/offsets via <c>EngineFeatures.Current.GetAddress(feature, key)</c> /
/// <c>GetOffset(feature, key)</c>, which throw <see cref="EngineFeatureUnavailableException"/> when it is disabled.
/// </para>
/// </summary>
internal static class ManagedEngineFeatures
{
    // Example (ticket 06):
    // public static readonly EngineFeatureDeclaration ChatClientPrint = new("cssharp.chat/client-print",
    //     [BindingDeclaration.Signature("ClientPrint"), BindingDeclaration.Signature("UTIL_ClientPrintAll")]);

    /// <summary>Every managed Engine Feature. Keep after the fields above (static initialisation order).</summary>
    public static readonly IReadOnlyList<EngineFeatureDeclaration> All = [];
}
