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

#pragma once

#include <memory>
#include <string>
#include <unordered_map>
#include <vector>

class PluginContext
{
    friend class CDotNetManager;

  public:
    PluginContext(std::string dll_path) : m_dll_path(dll_path) {}

  private:
    std::string m_dll_path;
};

class CDotNetManager
{
    friend class PluginContext;

  public:
    CDotNetManager();
    ~CDotNetManager();

    bool Initialize();
    void UnloadPlugin(PluginContext* context);
    void Shutdown();
    PluginContext* FindContext(std::string path);

  private:
    std::vector<std::shared_ptr<PluginContext>> m_app_domains;
};
