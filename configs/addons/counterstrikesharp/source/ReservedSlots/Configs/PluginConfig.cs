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

using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace ReservedSlots.Configs;

public class PluginConfig : BasePluginConfig
{
    // Number of reserved player slots
    [JsonPropertyName("ReservedSlots")]
    public int ReservedSlots { get; set; } = 0;
    
    // If set to true, reserved slots will hidden (subtracted from the max slot count)
    [JsonPropertyName("HideSlots")]
    public bool HideSlots { get; set; } = false;
    
    // Method of reserving slots
    [JsonPropertyName("ReserveType")]
    public int ReserveType { get; set; } = 0;
    
    // Maximum amount of admins to let in the server with reserve type 2
    [JsonPropertyName("MaxAdmins")]
    public int MaxAdmins { get; set; } = 1;
    
    // How to select a client to kick (if appropriate)
    [JsonPropertyName("KickType")]
    public int KickType { get; set; } = 0;
}
