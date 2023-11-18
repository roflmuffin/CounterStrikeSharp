using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace CounterStrikeSharp.API.Core.Logging;

public class CoreLogging
{
    /// <summary>
    /// Creates a core logger scoped to CSS.
    /// <remarks>Eventually this should probably come from a service collection</remarks>
    /// </summary>
    public static ILogger CreateCoreLogger()
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u4}] (cssharp) {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Join(new[] {GlobalContext.RootDirectory, "logs", $"log-cssharp.txt"}), rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] cssharp {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Join(new[] {GlobalContext.RootDirectory, "logs", $"log-all.txt"}), rollingInterval: RollingInterval.Day, shared: true, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] cssharp {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        using ILoggerFactory loggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(logger);
            });

        return loggerFactory.CreateLogger("CounterStrikeSharp");
    }
}