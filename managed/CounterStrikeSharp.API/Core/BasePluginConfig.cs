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

namespace CounterStrikeSharp.API.Core
{
    public interface IBasePluginConfig
    {
        [JsonPropertyName("ConfigVersion")]
        int Version { get; set; }
    }

    public class BasePluginConfig : IBasePluginConfig
    {
        [JsonPropertyName("ConfigVersion")]
        public virtual int Version { get; set; } = 1;
    }
}
