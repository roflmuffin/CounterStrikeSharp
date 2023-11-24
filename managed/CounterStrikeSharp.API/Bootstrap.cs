using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Hosting;
using CounterStrikeSharp.API.Core.Logging;
using CounterStrikeSharp.API.Core.Plugin;
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
        // Path to /game/csgo/addons/counterstrikesharp
        var contentRoot = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.FullName;

        using var host = Host.CreateDefaultBuilder()
            .UseContentRoot(contentRoot)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IScriptHostConfiguration, ScriptHostConfiguration>();
                services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog(new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .Enrich.With<SourceContextEnricher>()
                        .MinimumLevel.Debug()
                        .WriteTo.Console(
                            outputTemplate:
                            "{Timestamp:HH:mm:ss} [{Level:u4}] (cssharp:{SourceContext}) {Message:lj}{NewLine}{Exception}")
                        .WriteTo.File(Path.Join(new[] { contentRoot, "logs", $"log-cssharp.txt" }),
                            rollingInterval: RollingInterval.Day,
                            outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] (cssharp:{SourceContext}) {Message:lj}{NewLine}{Exception}")
                        .WriteTo.File(Path.Join(new[] { contentRoot, "logs", $"log-all.txt" }),
                            rollingInterval: RollingInterval.Day, shared: true,
                            outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] (cssharp:{SourceContext}) {Message:lj}{NewLine}{Exception}")
                        .CreateLogger());
                });

                services.AddScoped<Core.Application>();
                services.AddSingleton<IPluginHostContext, PluginHostContext>();
                services.AddScoped<IPluginContextQueryHandler, PluginContextQueryHandler>();

                services.Scan(i => i.FromCallingAssembly()
                    .AddClasses(c => c.AssignableTo<IStartupService>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime());
            })
            .Build();

        using IServiceScope scope = host.Services.CreateScope();

        GameData.GameDataProvider = scope.ServiceProvider.GetRequiredService<GameDataProvider>();

        var context = scope.ServiceProvider.GetRequiredService<Core.Application>();
        context.Start();

        return 1;
    }
}