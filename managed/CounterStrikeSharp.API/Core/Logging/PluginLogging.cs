using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace CounterStrikeSharp.API.Core.Logging;

public class PluginLogging
{
    /// <summary>
    /// Creates a logger scoped to a specific plugin
    /// <remarks>Eventually this should probably come from a service collection</remarks>
    /// </summary>
    public static ILogger CreatePluginLogger(PluginContext pluginContext)
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.With(new PluginNameEnricher(pluginContext))
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u4}] (plugin:{PluginName}) {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Join(new[] {GlobalContext.RootDirectory, "logs", $"log-{pluginContext.PluginType.Name}.txt"}), rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] plugin:{PluginName} {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Join(new[] {GlobalContext.RootDirectory, "logs", $"log-all.txt"}), rollingInterval: RollingInterval.Day, shared: true, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] plugin:{PluginName} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        using ILoggerFactory loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(logger);
            });
        
        return loggerFactory.CreateLogger(pluginContext.PluginType);
    }
}