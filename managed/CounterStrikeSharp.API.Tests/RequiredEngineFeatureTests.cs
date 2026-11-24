using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Core.Plugin;

namespace CounterStrikeSharp.API.Tests;

// The plugin load gate for Required Engine Features (ADR-0002), from hand-built Resolution Reports (no native calls).
public class RequiredEngineFeatureTests
{
    private static readonly EngineFeatureDeclaration SayHooks =
        new("cssharp.chat/say-hooks", [BindingDeclaration.Signature("Host_Say")]);

    private static EngineFeatureSet Features(BindingResult hostSay) =>
        new(new ResolutionReport("1.42.0.1/2000950", [hostSay], [SayHooks]), []);

    private static readonly EngineFeatureSet SayHooksAvailable =
        Features(new BindingResult("Host_Say", BindingKind.Signature, true, BindingFailureReason.None, 1, 0x1000, 0, "server"));

    private static readonly EngineFeatureSet SayHooksNotFound =
        Features(new BindingResult("Host_Say", BindingKind.Signature, false, BindingFailureReason.NotFound, 0, 0, 0, "server"));

    private class DeclaresNothing;

    [RequiredEngineFeatures("cssharp.chat/say-hooks")]
    private class RequiresSayHooks;

    [RequiredEngineFeatures(EngineFeature.ChatSayHooks)]
    private class RequiresSayHooksByEnum;

    [RequiredEngineFeatures("cssharp.made/up")]
    private class RequiresUnknown;

    [RequiredEngineFeatures("cssharp.chat/say-hooks", "cssharp.made/up")]
    private class RequiresSayHooksAndUnknown;

    [RequiredEngineFeatures("cssharp.chat/say-hooks")]
    [RequiredEngineFeatures("cssharp.made/up")]
    private class RequiresTwoViaTwoAttributes;

    [Fact]
    public void APluginDeclaringNothingLoads()
    {
        Assert.Null(RequiredEngineFeatureCheck.GetRefusal(typeof(DeclaresNothing), "Plugin.dll", SayHooksNotFound));
    }

    [Fact]
    public void APluginRequiringAnAvailableEngineFeatureLoads()
    {
        Assert.Null(RequiredEngineFeatureCheck.GetRefusal(typeof(RequiresSayHooks), "Plugin.dll", SayHooksAvailable));
        Assert.Null(RequiredEngineFeatureCheck.GetRefusal(typeof(RequiresSayHooksByEnum), "Plugin.dll", SayHooksAvailable));
    }

    [Fact]
    public void APluginRequiringADisabledEngineFeatureIsRefusedNamingFeatureBindingAndReason()
    {
        var refusal = RequiredEngineFeatureCheck.GetRefusal(typeof(RequiresSayHooks), "Plugin.dll", SayHooksNotFound);

        Assert.Equal(
            "Plugin \"Plugin.dll\" requires Engine Features that are not available on build 1.42.0.1/2000950: " +
            "cssharp.chat/say-hooks disabled (Host_Say: not found).",
            refusal);
    }

    [Fact]
    public void TheEnumFormIsCheckedByName()
    {
        var refusal = RequiredEngineFeatureCheck.GetRefusal(typeof(RequiresSayHooksByEnum), "Plugin.dll", SayHooksNotFound);

        Assert.Contains("cssharp.chat/say-hooks disabled (Host_Say: not found)", refusal);
    }

    [Fact]
    public void AnUnknownEngineFeatureNameIsReportedClearly()
    {
        var refusal = RequiredEngineFeatureCheck.GetRefusal(typeof(RequiresUnknown), "Plugin.dll", SayHooksAvailable);

        Assert.Equal(
            "Plugin \"Plugin.dll\" requires Engine Features that are not available on build 1.42.0.1/2000950: " +
            "cssharp.made/up is not an Engine Feature of this CounterStrikeSharp version.",
            refusal);
    }

    [Fact]
    public void EveryUnavailableRequiredEngineFeatureIsListed()
    {
        var expected =
            "Plugin \"Plugin.dll\" requires Engine Features that are not available on build 1.42.0.1/2000950: " +
            "cssharp.chat/say-hooks disabled (Host_Say: not found); " +
            "cssharp.made/up is not an Engine Feature of this CounterStrikeSharp version.";

        Assert.Equal(expected,
            RequiredEngineFeatureCheck.GetRefusal(typeof(RequiresSayHooksAndUnknown), "Plugin.dll", SayHooksNotFound));
        Assert.Equal(expected,
            RequiredEngineFeatureCheck.GetRefusal(typeof(RequiresTwoViaTwoAttributes), "Plugin.dll", SayHooksNotFound));
    }

    [Fact]
    public void OnlyTheUnavailableOnesAreListed()
    {
        var refusal = RequiredEngineFeatureCheck.GetRefusal(typeof(RequiresSayHooksAndUnknown), "Plugin.dll", SayHooksAvailable);

        Assert.DoesNotContain("say-hooks", refusal);
        Assert.Contains("cssharp.made/up is not an Engine Feature", refusal);
    }

    [Fact]
    public void AnEnumValueFromANewerVersionIsReportedAsUnknown()
    {
        var attribute = new RequiredEngineFeatures((EngineFeature)12345);

        Assert.Equal(["#12345"], attribute.Names);
    }
}
