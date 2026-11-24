using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;

namespace CounterStrikeSharp.API.Tests;

// The plugin-facing query surface over the process-wide report. Uses static state, so not run in parallel with others
// that install a report.
[Collection(nameof(EngineFeaturesApiTests))]
public class EngineFeaturesApiTests : IDisposable
{
    public EngineFeaturesApiTests()
    {
        var sayHooks = new EngineFeatureDeclaration("cssharp.chat/say-hooks", [BindingDeclaration.Signature("Host_Say")]);
        var report = new ResolutionReport("1.42.0.1/2000950",
            [new BindingResult("Host_Say", BindingKind.Signature, false, BindingFailureReason.NotFound, 0, 0, 0, "server")],
            [sayHooks]);
        EngineFeatures.Current = new EngineFeatureSet(report, []);
    }

    public void Dispose() => EngineFeatures.Reset();

    [Fact]
    public void PluginsAskByStableStringName()
    {
        Assert.False(EngineFeatures.IsAvailable("cssharp.chat/say-hooks"));
        Assert.Equal("Host_Say: not found", EngineFeatures.GetStatus("cssharp.chat/say-hooks").Describe());
        Assert.Equal("1.42.0.1/2000950", EngineFeatures.BuildId);
    }

    [Fact]
    public void PluginsAskByTheGeneratedEnum()
    {
        Assert.False(EngineFeatures.IsAvailable(EngineFeature.ChatSayHooks));
        Assert.Equal("cssharp.chat/say-hooks", EngineFeatures.GetStatus(EngineFeature.ChatSayHooks).Name);
    }

    [Fact]
    public void AnEnumValueThisVersionDoesNotKnowIsNotAvailable()
    {
        Assert.False(EngineFeatures.IsAvailable((EngineFeature)12345));
        Assert.False(EngineFeatures.GetStatus((EngineFeature)12345).IsDeclared);
    }

    [Fact]
    public void PluginsListEveryEngineFeatureWithItsStatus()
    {
        var status = Assert.Single(EngineFeatures.GetAll());
        Assert.Equal("cssharp.chat/say-hooks disabled (Host_Say: not found)", status.ToString());
    }
}

public class EngineFeatureEnumTests
{
    [Fact]
    public void TheEnumIsGeneratedFromTheNames()
    {
        Assert.Equal("cssharp.chat/say-hooks", EngineFeature.ChatSayHooks.GetName());
        Assert.Equal("cssharp.chat/say-hooks", EngineFeatureNames.ChatSayHooks);
    }

    [Fact]
    public void EnumValuesNeverChange()
    {
        // Plugins compile the value in. It is a hash of the name, so adding other Engine Features never shifts it.
        Assert.Equal(2095797270, (int)EngineFeature.ChatSayHooks);
    }

    [Fact]
    public void EveryManagedEngineFeatureHasAnEnumMember()
    {
        var generated = Enum.GetValues<EngineFeature>().Select(f => f.GetName()).ToHashSet();
        foreach (var feature in ManagedEngineFeatures.All)
        {
            Assert.True(generated.Contains(feature.Name),
                $"{feature.Name} has no EngineFeature member; run: dotnet run --project tooling/CodeGen.Natives -- engine-features");
        }
    }
}
