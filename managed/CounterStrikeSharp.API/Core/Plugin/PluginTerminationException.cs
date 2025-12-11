using System;

namespace CounterStrikeSharp.API.Core.Plugin
{
    public class PluginTerminationException : Exception
    {
        public string PluginName { get; }
        public string TerminationReason { get; }

        public PluginTerminationException(string reason) : base($"Plugin terminated: {reason}")
        {
            TerminationReason = reason;
        }

        public PluginTerminationException(string pluginName, string reason) : base($"Plugin '{pluginName}' terminated: {reason}")
        {
            PluginName = pluginName;
            TerminationReason = reason;
        }

        public PluginTerminationException(string reason, Exception innerException) : base($"Plugin terminated: {reason}", innerException)
        {
            TerminationReason = reason;
        }

        public PluginTerminationException(string pluginName, string reason, Exception innerException) : base($"Plugin '{pluginName}' terminated: {reason}", innerException)
        {
            PluginName = pluginName;
            TerminationReason = reason;
        }
    }
}