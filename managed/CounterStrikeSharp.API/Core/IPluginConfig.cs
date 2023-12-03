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

namespace CounterStrikeSharp.API.Core
{
    /// <summary>
    /// An interface that describes a plugin configuration.
    /// </summary>
    public interface IPluginConfig<T> where T: IBasePluginConfig, new()
    {
        T Config { get; set; }

        /// <summary>
        /// Called when the `ConfigManager` has parsed the configuration file for this plugin
        /// </summary>
        /// <param name="config">Parsed config instance</param>
        public void OnConfigParsed(T config);
    }
}
