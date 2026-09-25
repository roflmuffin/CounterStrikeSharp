using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace CounterStrikeSharp.API.Core.Logging;

public static class CoreLogging
{
    private static readonly object SyncRoot = new();
    public static ILoggerFactory Factory { get; private set; }
    private static Logger? SerilogLogger { get; set; }
    internal static Logger CombinedLogger { get; private set; } = null!;

    public static void AddCoreLogging(this ILoggingBuilder builder, string contentRoot)
    {
        lock (SyncRoot)
        {
            if (SerilogLogger == null)
            {
                CombinedLogger = CombinedLog.CreateLogger(contentRoot);
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
                    .WriteTo.Logger(CombinedLogger)
                    .CreateLogger();

                Factory =
                    LoggerFactory.Create(builder => { builder.AddSerilog(SerilogLogger); });
                AppDomain.CurrentDomain.ProcessExit += (_, _) =>
                {
                    SerilogLogger.Dispose();
                    CombinedLogger.Dispose();
                };
            }
        }

        builder.AddSerilog(SerilogLogger);
    }
}
