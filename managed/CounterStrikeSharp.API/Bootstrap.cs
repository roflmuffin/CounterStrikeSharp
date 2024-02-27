using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Core.Hosting;
using CounterStrikeSharp.API.Core.Logging;
using CounterStrikeSharp.API.Core.Plugin;
using CounterStrikeSharp.API.Core.Plugin.Host;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
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
                    services.AddSingleton<IPlayerLanguageManager, PlayerLanguageManager>();
                    services.AddScoped<IPluginContextQueryHandler, PluginContextQueryHandler>();
                    services.AddSingleton<ICommandManager, CommandManager>();

                    services.Scan(i => i.FromCallingAssembly()
                        .AddClasses(c => c.AssignableTo<IStartupService>())
                        .AsSelfWithInterfaces()
                        .WithSingletonLifetime());
                })
                .Build();

            using IServiceScope rootScope = host.Services.CreateScope();

            // TODO: Improve static singleton access
            GameData.GameDataProvider = rootScope.ServiceProvider.GetRequiredService<GameDataProvider>();
            AdminManager.CommandManagerProvider = rootScope.ServiceProvider.GetRequiredService<ICommandManager>();

            var application = rootScope.ServiceProvider.GetRequiredService<Application>();
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