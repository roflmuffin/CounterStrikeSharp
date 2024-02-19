using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace CounterStrikeSharp.API;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Decorates a given interface with a decorator class, keeping a local copy for singleton access.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TDecoratedService"></typeparam>
    /// <returns></returns>
    public static IServiceCollection DecorateSingleton<TService, TDecoratedService>(this IServiceCollection services) where TService : class
        where TDecoratedService : class, TService
    {
        TService? localCopy = default(TService?);
        services.Decorate<TService>((inner, provider) =>
        {
            if (localCopy == null)
            {
                localCopy = ActivatorUtilities.CreateInstance<TDecoratedService>(provider, inner);
            }

            return localCopy;
        });

        return services;
    }
}