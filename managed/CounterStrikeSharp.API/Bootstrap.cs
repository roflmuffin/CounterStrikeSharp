using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Hosting;
using CounterStrikeSharp.API.Core.Logging;
using CounterStrikeSharp.API.Core.Plugin;
using CounterStrikeSharp.API.Core.Plugin.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CounterStrikeSharp.API;

public static class Bootstrap
{
    [UnmanagedCallersOnly]
    // Used by .NET Host in C++ to initiate loading
    public static int Run()
    {
        try
        {
            // Path to /game/csgo/addons/counterstrikesharp
            var contentRoot = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.FullName;

            using var host = Host.CreateDefaultBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(services =>
                {
                    services.AddLogging(builder =>
                    {
                        builder.ClearProviders();
                        builder.AddCoreLogging(contentRoot);
                    });

                    services.AddSingleton<IScriptHostConfiguration, ScriptHostConfiguration>();
                    services.AddScoped<Application>();
                    services.AddSingleton<IPluginManager, PluginManager>();
                    services.AddScoped<IPluginContextQueryHandler, PluginContextQueryHandler>();

                    services.Scan(i => i.FromCallingAssembly()
                        .AddClasses(c => c.AssignableTo<IStartupService>())
                        .AsSelfWithInterfaces()
                        .WithSingletonLifetime());
                })
                .Build();

            using IServiceScope scope = host.Services.CreateScope();

            // TODO: Improve static singleton access
            GameData.GameDataProvider = scope.ServiceProvider.GetRequiredService<GameDataProvider>();

            var application = scope.ServiceProvider.GetRequiredService<Application>();
            application.Start();

            return 1;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Log.Fatal(e, "Failed to start application");
            return 0;
        }
    }
}