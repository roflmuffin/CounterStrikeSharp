using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;

namespace WithConfig;

public class TestPluginServiceCollection : IPluginServiceCollection<WithConfigPlugin>
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {   
        serviceCollection
            .AddOptions<SampleConfig>()
            .BindConfiguration(string.Empty)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}