using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace CounterStrikeSharp.API.Core.Logging;

public static class CoreLogging
{
    public static ILoggerFactory Factory { get; private set; }
    private static Logger? SerilogLogger { get; set; }

    public static void AddCoreLogging(this ILoggingBuilder builder, string contentRoot)
    {
        if (SerilogLogger == null)
        {
            SerilogLogger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.With<SourceContextEnricher>()
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
                .CreateLogger();

            Factory =
                LoggerFactory.Create(builder => { builder.AddSerilog(SerilogLogger); });
        }

        builder.AddSerilog(SerilogLogger);
    }
}