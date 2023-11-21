using Serilog.Core;
using Serilog.Events;

namespace CounterStrikeSharp.API.Core.Logging;

public class PluginNameEnricher : ILogEventEnricher
{
    public const string PropertyName = "PluginName";

    public PluginNameEnricher(PluginContext pluginContext)
    {
        Context = pluginContext;
    }

    public PluginContext Context { get; }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var property = propertyFactory.CreateProperty(PropertyName, Context.PluginType.Name);
        logEvent.AddPropertyIfAbsent(property);
    }
}