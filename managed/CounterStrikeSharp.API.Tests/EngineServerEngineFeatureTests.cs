using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.ValveConstants.Protobuf;
using Microsoft.Extensions.Logging.Abstractions;

namespace CounterStrikeSharp.API.Tests;

// The APIs on IVEngineServer2 calls (and its SetClientListening hook) are one Engine Feature each (ADR-0002). A failed
// IVEngineServer2 Slot Binding disables only the Engine Features using it, and calling one of those throws the typed
// exception before any native call. Installs a process-wide report, so shares the collection of the other tests that do.
[Collection(nameof(EngineFeaturesApiTests))]
public class EngineServerEngineFeatureTests : IDisposable
{
    // The native catalog's IVEngineServer2 rows, as the report carries them.
    private static readonly Dictionary<string, string[]> Catalog = new()
    {
        ["cssharp.player/set-listening"] = ["IVEngineServer2::SetClientListening"],
        ["cssharp.server/execute-command"] = ["IVEngineServer2::ServerCommand"],
        ["cssharp.server/is-map-valid"] = ["IVEngineServer2::IsMapValid"],
        ["cssharp.server/precache-model"] = ["IVEngineServer2::PrecacheGeneric"],
        ["cssharp.player/user-id"] = ["IVEngineServer2::GetPlayerUserId"],
        ["cssharp.player/print-to-console"] = ["IVEngineServer2::ClientPrintf"],
        ["cssharp.player/get-convar-value"] = ["IVEngineServer2::GetClientConVarValue"],
        ["cssharp.player/set-fake-client-convar"] = ["IVEngineServer2::SetFakeClientConVarValue"],
        ["cssharp.player/execute-client-command"] = ["IVEngineServer2::ClientCommand"],
        ["cssharp.player/disconnect"] = ["IVEngineServer2::DisconnectClient"],
        ["cssharp.player/authorization"] = ["IVEngineServer2::IsClientFullyAuthenticated", "IVEngineServer2::GetClientSteamID"],
    };

    // Not a valid entity: an API whose Engine Feature is available gets past its check and fails on the entity check,
    // before any native call.
    private static readonly CCSPlayerController InvalidController = new(IntPtr.Zero);

    public void Dispose() => EngineFeatures.Reset();

    // Every IVEngineServer2 Slot Binding resolves except failedKey (fingerprint mismatch).
    private static void InstallReport(string? failedKey)
    {
        var bindings = Catalog.Values.SelectMany(keys => keys).Distinct()
            .Select(key => key == failedKey
                ? new BindingResult(key, BindingKind.Slot, false, BindingFailureReason.FingerprintMismatch, 0, 0, 0, "engine")
                : new BindingResult(key, BindingKind.Slot, true, BindingFailureReason.None, 1, 0, 33, "engine"))
            .ToList();
        var features = Catalog
            .Select(row => new EngineFeatureDeclaration(row.Key, row.Value.Select(BindingDeclaration.Any).ToList()))
            .ToList();
        EngineFeatures.Current = new EngineFeatureSet(new ResolutionReport("1.42.0.1/2000950", bindings, features), []);
    }

    public static TheoryData<string, string, Action> Calls => new()
    {
        { "IVEngineServer2::ServerCommand", EngineFeatureNames.ServerExecuteCommand, () => Server.ExecuteCommand("echo hi") },
        { "IVEngineServer2::IsMapValid", EngineFeatureNames.ServerIsMapValid, () => Server.IsMapValid("de_dust2") },
        { "IVEngineServer2::PrecacheGeneric", EngineFeatureNames.ServerPrecacheModel, () => Server.PrecacheModel("models/x.vmdl") },
        { "IVEngineServer2::GetPlayerUserId", EngineFeatureNames.PlayerUserId, () => _ = InvalidController.UserId },
        { "IVEngineServer2::ClientPrintf", EngineFeatureNames.PlayerPrintToConsole, () => InvalidController.PrintToConsole("hi") },
        { "IVEngineServer2::GetClientConVarValue", EngineFeatureNames.PlayerGetConvarValue, () => InvalidController.GetConVarValue("cl_x") },
        {
            "IVEngineServer2::SetFakeClientConVarValue", EngineFeatureNames.PlayerSetFakeClientConvar,
            () => InvalidController.SetFakeClientConVar("cl_x", "1")
        },
        {
            "IVEngineServer2::ClientCommand", EngineFeatureNames.PlayerExecuteClientCommand,
            () => InvalidController.ExecuteClientCommand("say hi")
        },
        {
            "IVEngineServer2::DisconnectClient", EngineFeatureNames.PlayerDisconnect,
            () => InvalidController.Disconnect(NetworkDisconnectionReason.NETWORK_DISCONNECT_KICKED)
        },
        {
            "IVEngineServer2::SetClientListening", EngineFeatureNames.PlayerSetListening,
            () => InvalidController.SetListenOverride(InvalidController, ListenOverride.Mute)
        },
        { "IVEngineServer2::SetClientListening", EngineFeatureNames.PlayerSetListening, () => InvalidController.VoiceFlags = VoiceFlags.Muted },
        { "IVEngineServer2::GetClientSteamID", EngineFeatureNames.PlayerAuthorization, () => _ = InvalidController.AuthorizedSteamID },
        {
            "IVEngineServer2::IsClientFullyAuthenticated", EngineFeatureNames.PlayerAuthorization,
            () => _ = InvalidController.AuthorizedSteamID
        },
    };

    [Theory]
    [MemberData(nameof(Calls))]
    public void CallingAnApiWhoseSlotBindingFailedThrowsTheTypedException(string failedKey, string feature, Action call)
    {
        InstallReport(failedKey);

        var exception = Assert.Throws<EngineFeatureUnavailableException>(call);

        Assert.Equal(feature, exception.EngineFeature);
        Assert.Equal(failedKey, exception.Binding);
        Assert.Equal(BindingFailureReason.FingerprintMismatch, exception.Reason);
    }

    [Fact]
    public void AFailedSlotBindingDisablesOnlyTheEngineFeaturesUsingIt()
    {
        InstallReport("IVEngineServer2::GetClientSteamID");

        var disabled = Catalog.Keys.Where(name => !EngineFeatures.IsAvailable(name));

        Assert.Equal(["cssharp.player/authorization"], disabled);
    }

    [Fact]
    public void AnEntityApiWhoseEngineFeatureIsAvailableGetsPastTheCheck()
    {
        InstallReport("IVEngineServer2::GetClientSteamID");

        // The next check is the entity's, still before any native call.
        Assert.Throws<InvalidOperationException>(() => InvalidController.PrintToConsole("hi"));
    }

    [Fact]
    public void OnClientAuthorizedRegisteredWhileAuthorizationIsDisabledWarnsThatItNeverFires()
    {
        InstallReport("IVEngineServer2::IsClientFullyAuthenticated");

        Assert.False(ListenerEngineFeatures.CheckRegistration(EngineFeatures.Current, "OnClientAuthorized",
            NullLogger.Instance));
    }
}
