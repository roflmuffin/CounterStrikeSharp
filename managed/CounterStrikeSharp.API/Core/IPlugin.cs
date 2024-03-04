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

using System;
using CounterStrikeSharp.API.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core
{
    /// <summary>
    /// Interface which every CounterStrikeSharp plugin must implement. Module will be created with parameterless constructor and then Load method will be called.
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// Module version.
        /// </summary>
        string ModuleVersion { get; }

        string ModuleAuthor { get; }

        string ModuleDescription { get; }

        /// <summary>
        /// This method is called by CounterStrikeSharp on plugin load and should be treated as plugin constructor.
        /// Called with `true` on a hot reload (DLL file replaced in plugins folder)
        /// </summary>
        void Load(bool hotReload);

        /// <summary>
        /// Will be called by CounterStrikeSharp on plugin unload. In this method the plugin should cleanup any extra resources.
        /// Event handlers, listeners etc. will automatically be deregistered.
        /// </summary>
        void Unload(bool hotReload);

        /// <summary>
        /// Will be called by CounterStrikeSharp after all plugins have been loaded.
        /// This will also be called for convenience after a reload or a late l oad, so that you don't have to handle
        /// re-wiring everything.
        /// </summary>
        /// <param name="hotReload"></param>
        void OnAllPluginsLoaded(bool hotReload);

        string ModulePath { get; internal set; }

        ILogger Logger { get; set; }
        
        IStringLocalizer Localizer { get; set; }
        
        ICommandManager CommandManager { get; set; }

        void RegisterAllAttributes(object instance);

        void InitializeConfig(object instance, Type pluginType);
    }
}