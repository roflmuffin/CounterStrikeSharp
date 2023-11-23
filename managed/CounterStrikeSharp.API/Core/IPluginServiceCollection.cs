using Microsoft.Extensions.DependencyInjection;

namespace CounterStrikeSharp.API.Core;

public interface IPluginServiceCollection<T> where T: IPlugin
{
    /// <summary>
    /// Used to configure services exposed for dependency injection.
    /// </summary>
    public void ConfigureServices(IServiceCollection serviceCollection);
}