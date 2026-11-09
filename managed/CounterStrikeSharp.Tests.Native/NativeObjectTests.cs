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
            Assert.True(NativeHandleTracker.Shared.Count > 0);
            GC.KeepAlive(vector);
        });

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Allow several slices; the cleaner's budget is intentionally bounded.
        for (var attempt = 0; attempt < 100 && NativeHandleTracker.Shared.Count > 0; attempt++)
        {
            await Task.Delay(20);
        }

        Assert.Equal(0, NativeHandleTracker.Shared.Count);
    }
}
