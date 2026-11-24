using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

// cssharp.chat/client-print fails soft (ADR-0002). PrintToChatAll passes only while the Engine Feature is available; the
// other tests pass either way and check the behaviour that applies (E2E breaks the ClientPrint key to see both sides).
public class ClientPrintTests
{
    private static bool ClientPrintAvailable => EngineFeatures.IsAvailable(EngineFeature.ChatClientPrint);

    [Fact]
    public void PrintToChatAll_Prints()
    {
        Server.PrintToChatAll("[ClientPrintTests] PrintToChatAll");
    }

    [Fact]
    public void PrintToChatAll_ThrowsTypedExceptionWhenDisabled()
    {
        if (ClientPrintAvailable) return;

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => Server.PrintToChatAll("unreachable"));
        Assert.Equal(EngineFeatureNames.ChatClientPrint, exception.EngineFeature);
        Assert.NotNull(exception.Binding);
        Console.WriteLine($"[ClientPrintTests] PrintToChatAll threw {exception.GetType().Name}: {exception.Message}");
    }

    [Fact]
    public void ClientPrintAllHook_FiresOnlyWhenAvailable()
    {
        var mock = new Mock<Action>();
        Func<DynamicHook, HookResult> handler = _ =>
        {
            mock.Object.Invoke();
            return HookResult.Continue;
        };

        // Registering never throws, even on a disabled Engine Feature.
        VirtualFunctions.ClientPrintAllFunc.Hook(handler, HookMode.Pre);
        try
        {
            if (ClientPrintAvailable)
            {
                Server.PrintToChatAll("[ClientPrintTests] hooked");
                mock.Verify(s => s(), Times.AtLeastOnce);
            }
            else
            {
                Assert.Throws<EngineFeatureUnavailableException>(() => Server.PrintToChatAll("unreachable"));
                mock.Verify(s => s(), Times.Never);
            }
        }
        finally
        {
            VirtualFunctions.ClientPrintAllFunc.Unhook(handler, HookMode.Pre);
        }
    }

    [Fact]
    public void UnrelatedCoreFunction_StillWorks()
    {
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");

        Assert.NotNull(entity);
        Assert.True(entity.IsValid);
        entity.Remove();
    }
}
