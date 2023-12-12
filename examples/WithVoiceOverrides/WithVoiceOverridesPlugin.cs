using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;

namespace WithVoiceOverrides;

[MinimumApiVersion(80)]
public class WithVoiceOverridesPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Voice Overrides";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A plugin that manipulates voice flags";

    [ConsoleCommand("css_hearall")]
    public void OnHearAllCommand(CCSPlayerController? caller, CommandInfo command)
    {
        if (caller is null) return;

        if (caller.VoiceFlags.HasFlag(VoiceFlags.ListenAll))
        {
            caller.VoiceFlags = VoiceFlags.Normal;
            command.ReplyToCommand("Voice set back to default");
        }
        else
        {
            caller.VoiceFlags = VoiceFlags.ListenAll;
            command.ReplyToCommand("Can hear both teams");
        }
    }
    
    [ConsoleCommand("css_muteself")]
    public void OnMuteSelfCommand(CCSPlayerController? caller, CommandInfo command)
    {
        if (caller is null) return;

        if (caller.VoiceFlags.HasFlag(VoiceFlags.Muted))
        {
            caller.VoiceFlags = VoiceFlags.Normal;
            command.ReplyToCommand("Unmuted yourself");
        }
        else
        {
            caller.VoiceFlags = VoiceFlags.Muted;
            command.ReplyToCommand("Muted yourself");
        }
    }
    
    [ConsoleCommand("css_muteothers")]
    [CommandHelper(minArgs: 1, usage: "[target]")]
    public void OnMuteOthersCommand(CCSPlayerController? caller, CommandInfo command)
    {
        if (caller is null) return;

        var targetResult = command.GetArgTargetResult(1);
        
        foreach (var player in targetResult.Players)
        {
            if (player == caller) continue;


            var existingOverride = caller.GetListenOverride(player);
            if (existingOverride == ListenOverride.Mute)
            {
                caller.SetListenOverride(player, ListenOverride.Default);
                command.ReplyToCommand($"Now hearing {player.PlayerName}");
            }
            else
            {
                caller.SetListenOverride(player, ListenOverride.Mute);
                command.ReplyToCommand($"Muted {player.PlayerName}");
            }
        }
        
    }
}