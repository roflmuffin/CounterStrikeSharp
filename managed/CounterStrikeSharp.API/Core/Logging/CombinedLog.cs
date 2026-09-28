using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace CounterStrikeSharp.API.Core.Logging;

internal static class CombinedLog
{
    internal static Logger CreateLogger(string contentRoot) => new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.File(new CombinedLogFormatter(), Path.Combine(contentRoot, "logs", "log-all.txt"),
            rollingInterval: RollingInterval.Day, shared: false)
        .CreateLogger();

    private sealed class CombinedLogFormatter : ITextFormatter
    {
        private readonly MessageTemplateTextFormatter core = new(
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] (cssharp:{SourceContext}) {Message:lj}{NewLine}{Exception}");
        private readonly MessageTemplateTextFormatter plugin = new(
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] plugin:{PluginName} {Message:lj}{NewLine}{Exception}");

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var formatter = logEvent.Properties.ContainsKey(PluginNameEnricher.PropertyName) ? plugin : core;
            formatter.Format(logEvent, output);
        }
    }
}
