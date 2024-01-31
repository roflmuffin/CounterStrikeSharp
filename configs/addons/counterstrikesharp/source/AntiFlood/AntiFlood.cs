/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using AntiFlood.Configs;
using AntiFlood.Models;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;

namespace AntiFlood;

[MinimumApiVersion(159)]
public class AntiFlood : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "Anti Flood";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleDescription => "Protects against chat flooding";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";

    private readonly Dictionary<int, FloodData> _playerInfo = new();
    
    public PluginConfig Config { get; set; }

    public void OnConfigParsed(PluginConfig config)
    {
        if (config.FloodTime < 0)
        {
            Logger.LogWarning("[CSS] FloodTime must be a positive number. Setting to 0.");
            config.FloodTime = 0;
        }

        // Once we've validated the config, we can set it to the instance
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Console.WriteLine("[CSS] Anti Flood loaded");
        
        AddCommandListener("say", OnCommandSay);
        AddCommandListener("say_team", OnCommandSay);
    }

    [GameEventHandler]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player == null)
        {
            return HookResult.Continue;
        }
        
        _playerInfo[player.Slot] = new FloodData();

        return HookResult.Continue;
    }
    
    private HookResult OnCommandSay(CCSPlayerController? player, CommandInfo commandInfo)
    {
        var isBlocked = OnClientFloodCheck(player);
        OnClientFloodResult(player, isBlocked);
        
        return isBlocked ? HookResult.Handled : HookResult.Continue;
    }
    
    private bool OnClientFloodCheck(CCSPlayerController? player)
    {
        if (player == null || Config.FloodTime == 0 || AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            return false;
        }

        if (_playerInfo[player.Slot].LastTime >= Server.CurrentTime)
        {
            /* If player has 3 or more flood tokens, block their message */
            if (_playerInfo[player.Slot].TokenCount >= 3)
            {
                return true;
            }
        }
	
        return false;
    }
    
    private void OnClientFloodResult(CCSPlayerController? player, bool wasBlocked)
    {
        if (player == null || Config.FloodTime == 0 || AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            return;
        }
	
        var curTime = Server.CurrentTime;
        var newTime = curTime + Config.FloodTime;
	
        if (_playerInfo[player.Slot].LastTime >= curTime)
        {
            /* If the last message was blocked, update their time limit */
            if (wasBlocked)
            {
                newTime += 3.0f;
            }
            /* Add one flood token when player goes over chat time limit */
            else if (_playerInfo[player.Slot].TokenCount < 3)
            {
                _playerInfo[player.Slot].TokenCount++;
            }
        }
        else if (_playerInfo[player.Slot].TokenCount > 0)
        {
            /* Remove one flood token when player chats within time limit (slow decay) */
            _playerInfo[player.Slot].TokenCount--;
        }
	
        _playerInfo[player.Slot].LastTime = newTime;
    }
}
