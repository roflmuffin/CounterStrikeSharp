using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace CounterStrikeSharp.API.Core.Logging;

public static class CoreLogging
{
    public static ILoggerFactory Factory { get; }
    
    static CoreLogging()
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.With<SourceContextEnricher>()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u4}] (cssharp:{SourceContext}) {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Join(new[] {GlobalContext.RootDirectory, "logs", $"log-cssharp.txt"}), rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] (cssharp:{SourceContext}) {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(Path.Join(new[] {GlobalContext.RootDirectory, "logs", $"log-all.txt"}), rollingInterval: RollingInterval.Day, shared: true, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] (cssharp:{SourceContext}) {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        Factory =
            LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(logger);
            });
    }
}