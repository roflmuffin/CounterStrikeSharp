using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace WithRadarElements;

public class WithRadarElementsPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Radar Elements";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "Read radar element colors through the existing schema API";

    [ConsoleCommand("css_radar_color", "Read radar colors, optionally filtered by targetname or *suffix")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnRadarColorCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount > 2)
        {
            command.ReplyToCommand("Usage: css_radar_color [targetname|*suffix]");
            return;
        }

        string? filter = command.ArgCount == 2 ? command.GetArg(1) : null;
        bool found = false;
        // Resolve entities on each query; do not retain handles across map changes.
        foreach (var radar in Utilities.FindAllEntitiesByDesignerName<CCSRadarElement>("radar_element"))
        {
            if (!radar.IsValid) continue;
            string name = radar.Entity?.Name ?? "";
            if (filter != null && !(filter.StartsWith('*')
                    ? name.EndsWith(filter[1..], StringComparison.Ordinal)
                    : name.Equals(filter, StringComparison.Ordinal))) continue;

            // This copies the existing ref-returning property without writing it.
            uint rawColor = radar.ElementColor;
            string color = Enum.IsDefined(typeof(RadarElementColor), rawColor)
                ? ((RadarElementColor)rawColor).ToString()
                : "unknown";
            command.ReplyToCommand($"[Radar] entity={radar.Index}; name={name}; elementColor={rawColor} ({color})");
            found = true;
        }

        if (!found) command.ReplyToCommand("[Radar] No matching radar_element found.");
    }
}
