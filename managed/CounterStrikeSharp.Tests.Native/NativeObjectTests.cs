using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;

namespace NativeTestsPlugin;

public class NativeObjectsTests
{
    [Fact]
    public async Task EnsureNativeHandle_IsFreed_Vector3()
    {
        await Server.NextFrameAsync(() =>
        {
            var vector = new Vector(0, 0, 500);
            Assert.Equal(IntPtr.Zero, vector.RawHandle);
            Assert.Equal(500, NativeAPI.VectorGetZ(vector.Handle));
            Assert.Single(NativeHandleTracker._entries);
        });

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        await Task.Delay(1000);
        Assert.Empty(NativeHandleTracker._entries);
    }
}