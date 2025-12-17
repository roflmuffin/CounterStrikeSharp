using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Timers;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

public class VirtualFunctionTests
{
    [Fact]
    public async Task ShouldHook()
    {
        var mock = new Mock<Action>();

        var hookHandler = (DynamicHook hook) =>
        {
            mock.Object.Invoke();
            return HookResult.Continue;
        };

        try
        {
            VirtualFunctions.CCSPlayerPawnBase_PostThinkFunc.Hook(hookHandler, HookMode.Pre);

            // Verify hook
            await WaitOneFrame();
            mock.Verify(s => s(), Times.AtLeastOnce);
        }
        finally
        {
            VirtualFunctions.CCSPlayerPawnBase_PostThinkFunc.Unhook(hookHandler, HookMode.Pre);
            mock.Reset();

            // Verify unhook
            await WaitOneFrame();
            mock.Verify(s => s(), Times.Never);
        }
    }
}