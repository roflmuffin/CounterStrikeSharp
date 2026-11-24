using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;

namespace CounterStrikeSharp.API.Tests;

// Engine Feature policy from hand-built Resolution Reports (no native calls). See ADR-0002.
public class EngineFeatureTests
{
    private const string BuildId = "1.42.0.1/2000950";

    private static readonly EngineFeatureDeclaration SayHooks =
        new("cssharp.chat/say-hooks", [BindingDeclaration.Signature("Host_Say")]);

    private static readonly EngineFeatureDeclaration ClientPrint =
        new("cssharp.chat/client-print",
            [BindingDeclaration.Signature("ClientPrint"), BindingDeclaration.Signature("UTIL_ClientPrintAll")]);

    private static readonly EngineFeatureDeclaration Teleport =
        new("cssharp.entity/teleport", [BindingDeclaration.Offset("CBaseEntity_Teleport")]);

    private static BindingResult Resolved(string key, nint address = 0x1000) =>
        new(key, BindingKind.Signature, true, BindingFailureReason.None, 1, address, 0, "server");

    private static BindingResult ResolvedOffset(string key, long offset) =>
        new(key, BindingKind.Offset, true, BindingFailureReason.None, 0, 0, offset, "");

    private static BindingResult Failed(string key, BindingFailureReason reason, int matchCount = 0,
        BindingKind kind = BindingKind.Signature) =>
        new(key, kind, false, reason, matchCount, 0, 0, "server");

    private static EngineFeatureSet Features(BindingResult[] bindings, params EngineFeatureDeclaration[] managed) =>
        new(new ResolutionReport(BuildId, bindings, [SayHooks]), managed);

    [Fact]
    public void ANativeEngineFeatureIsAvailableWhenItsBindingResolved()
    {
        var features = Features([Resolved("Host_Say")]);

        Assert.True(features.IsAvailable("cssharp.chat/say-hooks"));
        var status = features.GetStatus("cssharp.chat/say-hooks");
        Assert.True(status.IsAvailable);
        Assert.Null(status.FailedBinding);
        Assert.Equal(BindingFailureReason.None, status.Reason);
    }

    [Fact]
    public void ANativeEngineFeatureIsDisabledWithTheFailedBindingAndReason()
    {
        var features = Features([Failed("Host_Say", BindingFailureReason.NotFound)]);

        Assert.False(features.IsAvailable("cssharp.chat/say-hooks"));
        var status = features.GetStatus("cssharp.chat/say-hooks");
        Assert.True(status.IsDeclared);
        Assert.False(status.IsAvailable);
        Assert.Equal("Host_Say", status.FailedBinding);
        Assert.Equal(BindingFailureReason.NotFound, status.Reason);
        Assert.Equal("Host_Say: not found", status.Describe());
    }

    [Fact]
    public void AnAmbiguousBindingCarriesItsMatchCount()
    {
        var status = Features([Failed("Host_Say", BindingFailureReason.Ambiguous, 2)]).GetStatus("cssharp.chat/say-hooks");

        Assert.Equal(BindingFailureReason.Ambiguous, status.Reason);
        Assert.Equal(2, status.MatchCount);
        Assert.Equal("Host_Say: ambiguous (2 matches)", status.Describe());
    }

    [Fact]
    public void AManagedEngineFeatureIsAvailableExactlyWhenAllItsBindingsResolved()
    {
        Assert.True(Features([Resolved("ClientPrint"), Resolved("UTIL_ClientPrintAll")], ClientPrint)
            .IsAvailable("cssharp.chat/client-print"));

        var status = Features([Resolved("ClientPrint"), Failed("UTIL_ClientPrintAll", BindingFailureReason.MissingForThisOs)],
            ClientPrint).GetStatus("cssharp.chat/client-print");
        Assert.False(status.IsAvailable);
        Assert.Equal("UTIL_ClientPrintAll", status.FailedBinding);
        Assert.Equal("UTIL_ClientPrintAll: missing for this OS", status.Describe());
    }

    [Fact]
    public void ABindingAbsentFromTheReportFailsAsMissingFromGamedata()
    {
        var status = Features([Resolved("ClientPrint")], ClientPrint).GetStatus("cssharp.chat/client-print");

        Assert.False(status.IsAvailable);
        Assert.Equal("UTIL_ClientPrintAll", status.FailedBinding);
        Assert.Equal(BindingFailureReason.MissingFromGamedata, status.Reason);
    }

    [Fact]
    public void ABindingOfTheWrongKindFailsAsMissingFromGamedata()
    {
        // Gamedata has CBaseEntity_Teleport as a signature, but the Engine Feature needs an offset.
        var status = Features([Resolved("CBaseEntity_Teleport")], Teleport).GetStatus("cssharp.entity/teleport");

        Assert.False(status.IsAvailable);
        Assert.Equal(BindingFailureReason.MissingFromGamedata, status.Reason);
    }

    [Fact]
    public void AnUnknownNameIsNotAvailableAndSaysSo()
    {
        var status = Features([Resolved("Host_Say")]).GetStatus("cssharp.nope/nope");

        Assert.False(status.IsDeclared);
        Assert.False(status.IsAvailable);
        Assert.Equal("unknown Engine Feature", status.Describe());
        Assert.False(Features([Resolved("Host_Say")]).IsAvailable("cssharp.nope/nope"));
    }

    [Fact]
    public void ListingReturnsEveryDeclaredEngineFeatureByNameWithItsStatus()
    {
        var all = Features([Failed("Host_Say", BindingFailureReason.NotFound), ResolvedOffset("CBaseEntity_Teleport", 157)],
            Teleport).GetAll();

        Assert.Equal(["cssharp.chat/say-hooks", "cssharp.entity/teleport"], all.Select(s => s.Name));
        Assert.False(all[0].IsAvailable);
        Assert.True(all[1].IsAvailable);
    }

    [Fact]
    public void AnEngineFeatureDeclaredOnBothSidesNeedsTheBindingsOfBoth()
    {
        var managedSayHooks = new EngineFeatureDeclaration("cssharp.chat/say-hooks", [BindingDeclaration.Signature("ClientPrint")]);

        var features = Features([Resolved("Host_Say"), Failed("ClientPrint", BindingFailureReason.NotFound)], managedSayHooks);

        Assert.Single(features.GetAll());
        Assert.Equal("ClientPrint", features.GetStatus("cssharp.chat/say-hooks").FailedBinding);
    }

    [Fact]
    public void TheStartupReportSaysAllAvailableWhenNothingFailed()
    {
        var features = Features([Resolved("Host_Say"), ResolvedOffset("CBaseEntity_Teleport", 157)], Teleport);

        Assert.False(features.AnyDisabled);
        Assert.Equal("Engine Features for build 1.42.0.1/2000950: all 2 available", features.FormatStartupReport());
    }

    [Fact]
    public void TheStartupReportListsEachDisabledEngineFeatureWithItsFailedBindingAndReason()
    {
        var features = Features(
            [Failed("Host_Say", BindingFailureReason.Ambiguous, 3), Resolved("ClientPrint"), Resolved("UTIL_ClientPrintAll")],
            ClientPrint, Teleport);

        Assert.True(features.AnyDisabled);
        Assert.Equal(
            """
            Engine Features for build 1.42.0.1/2000950: 1 of 3 available, 2 disabled:
              cssharp.chat/say-hooks disabled (Host_Say: ambiguous (3 matches))
              cssharp.entity/teleport disabled (CBaseEntity_Teleport: missing from gamedata)
            """.ReplaceLineEndings("\n"),
            features.FormatStartupReport());
    }

    [Fact]
    public void HelpersTakeAddressesAndOffsetsFromTheReport()
    {
        var features = Features(
            [Resolved("ClientPrint", 0x1234), Resolved("UTIL_ClientPrintAll", 0x5678), ResolvedOffset("CBaseEntity_Teleport", 157)],
            ClientPrint, Teleport);

        Assert.Equal((nint)0x5678, features.GetAddress(ClientPrint, "UTIL_ClientPrintAll"));
        Assert.Equal(157, features.GetOffset(Teleport, "CBaseEntity_Teleport"));
    }

    [Fact]
    public void AHelperOfADisabledEngineFeatureThrowsTheTypedException()
    {
        var features = Features([Resolved("ClientPrint", 0x1234), Failed("UTIL_ClientPrintAll", BindingFailureReason.NotFound)],
            ClientPrint);

        // The whole Engine Feature is disabled, so even its resolved Binding is not handed out.
        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => features.GetAddress(ClientPrint, "ClientPrint"));
        Assert.Equal("cssharp.chat/client-print", exception.EngineFeature);
        Assert.Equal("UTIL_ClientPrintAll", exception.Binding);
        Assert.Equal(BindingFailureReason.NotFound, exception.Reason);
        Assert.Equal("Engine Feature cssharp.chat/client-print is disabled (UTIL_ClientPrintAll: not found)", exception.Message);

        Assert.Throws<EngineFeatureUnavailableException>(() => features.ThrowIfUnavailable("cssharp.chat/client-print"));
    }

    [Fact]
    public void AskingForABindingTheEngineFeatureDoesNotDeclareIsAProgrammingError()
    {
        var features = Features([Resolved("ClientPrint"), Resolved("UTIL_ClientPrintAll")], ClientPrint);

        Assert.Throws<ArgumentException>(() => features.GetAddress(ClientPrint, "Host_Say"));
    }

    [Fact]
    public void TheReportIsReadFromTheNativeJson()
    {
        // Shape written by bindings::ResolutionReportToJson (src/core/binding_resolver.h).
        const string json = """
            {"buildId":"1.42.0.1/2000950",
             "bindings":[
               {"key":"Host_Say","kind":"Signature","resolved":false,"reason":"Ambiguous","matchCount":2,
                "address":0,"offset":0,"library":"server"},
               {"key":"CBaseEntity_Teleport","kind":"Offset","resolved":true,"reason":"None","matchCount":0,
                "address":0,"offset":157,"library":""},
               {"key":"ClientPrint","kind":"Signature","resolved":true,"reason":"None","matchCount":1,
                "address":139637976732212,"offset":0,"library":"server"},
               {"key":"CBaseEntity::m_iTeamNum","kind":"SchemaField","resolved":false,"reason":"SchemaFieldMissing",
                "matchCount":0,"address":0,"offset":0,"library":"server"}],
             "features":[{"name":"cssharp.chat/say-hooks","bindings":["Host_Say"]}]}
            """;

        var report = ResolutionReport.FromJson(json);

        Assert.Equal("1.42.0.1/2000950", report.BuildId);
        Assert.Equal(new BindingResult("Host_Say", BindingKind.Signature, false, BindingFailureReason.Ambiguous, 2, 0, 0, "server"),
            report.Find("Host_Say"));
        Assert.Equal(157, report.Find("CBaseEntity_Teleport")!.Offset);
        Assert.Equal(unchecked((nint)0x7f0000001234), report.Find("ClientPrint")!.Address);
        Assert.Equal(BindingKind.SchemaField, report.Find("CBaseEntity::m_iTeamNum")!.Kind);
        Assert.Equal(BindingFailureReason.SchemaFieldMissing, report.Find("CBaseEntity::m_iTeamNum")!.Reason);
        Assert.Equal("cssharp.chat/say-hooks", Assert.Single(report.NativeFeatures).Name);

        var status = new EngineFeatureSet(report, []).GetStatus("cssharp.chat/say-hooks");
        Assert.Equal("Host_Say: ambiguous (2 matches)", status.Describe());
    }
}
