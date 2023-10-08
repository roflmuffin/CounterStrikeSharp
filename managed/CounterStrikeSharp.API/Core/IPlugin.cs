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
    /// Interface which every CounterStrikeSharp plugin must implement. Module will be created with parameterless constructor and then Load method will be called.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string ModuleName
        {
            get;
        }
        /// <summary>
        /// Module version.
        /// </summary>
        string ModuleVersion
        {
            get;
        }
        /// <summary>
        /// This method is called by CounterStrikeSharp on plugin load and should be treated as plugin constructor.
        /// </summary>
        void Load(bool hotReload);
        /// <summary>
        /// Will be called by CounterStrikeSharp on plugin unload. In this method plugin must cleanup and unregister with CSGO native data.
        /// </summary>
        void Unload(bool hotReload);
    }
}