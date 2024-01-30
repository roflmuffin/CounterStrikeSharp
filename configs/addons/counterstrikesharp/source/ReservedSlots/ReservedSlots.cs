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

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using ReservedSlots.Configs;

namespace ReservedSlots;

[MinimumApiVersion(159)]
public class ReservedSlots : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "Reserved Slots";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleDescription => "Provides basic reserved slots";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";

    public PluginConfig Config { get; set; }

    public void OnConfigParsed(PluginConfig config)
    {
        if (config.ReservedSlots < 0)
        {
            config.ReservedSlots = 0;
        }
        
        if (config.ReserveType < 0 || config.ReserveType > 2)
        {
            throw new Exception($"[CSS] Invalid value has been set to config value 'ReserveType': {config.ReserveType}");
        }
        
        if (config.MaxAdmins < 0)
        {
            config.MaxAdmins = 0;
        }
        
        if (config.KickType < 0 || config.KickType > 2)
        {
            throw new Exception($"[CSS] Invalid value has been set to config value 'KickType': {config.KickType}");
        }

        // Once we've validated the config, we can set it to the instance
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Console.WriteLine("[CSS] Reserved Slots loaded");
    }
}
