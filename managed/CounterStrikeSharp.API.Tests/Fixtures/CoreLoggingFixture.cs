using CounterStrikeSharp.API.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Tests.Fixtures;

public class CoreLoggingFixture : IDisposable
{
    public CoreLoggingFixture()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddCoreLogging(TestUtils.GetTestPath(""));
            })
            .BuildServiceProvider();
    }
    public void Dispose()
    {
        // TODO release managed resources here
    }
}