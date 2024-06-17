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

#include "core/coreconfig.h"

#include <fstream>

#include "core/log.h"

namespace counterstrikesharp {

CCoreConfig::CCoreConfig(const std::string& path) { m_sPath = path; }

CCoreConfig::~CCoreConfig() = default;

bool CCoreConfig::Init(char* conf_error, int conf_error_size)
{
    std::ifstream ifs(std::string(m_sPath + ".json"));

    if (!ifs)
    {
        std::ifstream exampleIfs(std::string(m_sPath + ".example.json"));

        if (!exampleIfs)
        {
            V_snprintf(conf_error, conf_error_size, "CoreConfig file not found.");
            return false;
        }

        CSSHARP_CORE_INFO("CoreConfig file not found, creating one from example.");
        std::ofstream ofs(std::string(m_sPath + ".json"));
        ofs << exampleIfs.rdbuf();
        ofs.close();

        return Init(conf_error, conf_error_size);
    }

    m_json = json::parse(ifs);

    try
    {
        PublicChatTrigger = m_json.value("PublicChatTrigger", PublicChatTrigger);
        SilentChatTrigger = m_json.value("SilentChatTrigger", SilentChatTrigger);
        FollowCS2ServerGuidelines = m_json.value("FollowCS2ServerGuidelines", FollowCS2ServerGuidelines);
        PluginHotReloadEnabled = m_json.value("PluginHotReloadEnabled", PluginHotReloadEnabled);
        PluginAutoLoadEnabled = m_json.value("PluginAutoLoadEnabled", PluginAutoLoadEnabled);
        ServerLanguage = m_json.value("ServerLanguage", ServerLanguage);
        UnlockConCommands = m_json.value("UnlockConCommands", UnlockConCommands);
        UnlockConVars = m_json.value("UnlockConVars", UnlockConVars);
    }
    catch (const std::exception& ex)
    {
        V_snprintf(conf_error, conf_error_size, "Failed to parse CoreConfig file: %s", ex.what());
        return false;
    }

    return true;
}

const std::string CCoreConfig::GetPath() const { return m_sPath; }

bool CCoreConfig::IsTriggerInternal(std::vector<std::string> triggers, const std::string& message, std::string& prefix) const
{
    for (std::string& trigger : triggers)
    {
        if (message.rfind(trigger, 0) == 0)
        {
            prefix = trigger;
            CSSHARP_CORE_TRACE("Trigger found, prefix is {}", prefix);
            return true;
        }
    }

    return false;
}

bool CCoreConfig::IsSilentChatTrigger(const std::string& message, std::string& prefix) const
{
    return IsTriggerInternal(SilentChatTrigger, message, prefix);
}

bool CCoreConfig::IsPublicChatTrigger(const std::string& message, std::string& prefix) const
{
    return IsTriggerInternal(PublicChatTrigger, message, prefix);
}
} // namespace counterstrikesharp
