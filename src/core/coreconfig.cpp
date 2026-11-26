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

#include <algorithm>
#include <cctype>
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
        AutoUpdateEnabled = m_json.value("AutoUpdateEnabled", AutoUpdateEnabled);
        AutoUpdateURL = m_json.value("AutoUpdateURL", AutoUpdateURL);
        ReadOperatorSettings();
    }
    catch (const std::exception& ex)
    {
        V_snprintf(conf_error, conf_error_size, "Failed to parse CoreConfig file: %s", ex.what());
        return false;
    }

    return true;
}

// Same rules as managed CoreConfig: case-insensitive, surrounding whitespace ignored, and a value that is not one of the
// modes (or not of the right type) falls back to the default for that key only, with a warning.
void CCoreConfig::ReadOperatorSettings()
{
    FailMode = "disable";
    if (const auto failMode = m_json.find("FailMode"); failMode != m_json.end() && !failMode->is_null())
    {
        std::string mode = failMode->is_string() ? failMode->get<std::string>() : std::string();
        mode.erase(0, mode.find_first_not_of(" \t\r\n"));
        mode.erase(mode.find_last_not_of(" \t\r\n") + 1);
        std::transform(mode.begin(), mode.end(), mode.begin(), [](unsigned char c) { return std::tolower(c); });
        if (failMode->is_string() && (mode.empty() || mode == "disable" || mode == "crash")) FailMode = mode.empty() ? "disable" : mode;
        else
            CSSHARP_CORE_WARN("CoreConfig FailMode {} is not \"disable\" or \"crash\", using \"disable\"", failMode->dump());
    }

    HealEnabled = true;
    if (const auto heal = m_json.find("HealEnabled"); heal != m_json.end() && !heal->is_null())
    {
        if (heal->is_boolean()) HealEnabled = heal->get<bool>();
        else
            CSSHARP_CORE_WARN("CoreConfig HealEnabled {} is not true or false, using true", heal->dump());
    }
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
