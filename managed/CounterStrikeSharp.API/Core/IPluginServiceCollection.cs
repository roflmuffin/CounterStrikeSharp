using Microsoft.Extensions.DependencyInjection;

namespace CounterStrikeSharp.API.Core;

/// <summary>
/// Represents a service collection configuration for a plugin.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPluginServiceCollection<T> where T : IPlugin
{
    /// <summary>
    /// Used to configure services exposed for dependency injection.
    /// </summary>
    public void ConfigureServices(IServiceCollection serviceCollection);
}