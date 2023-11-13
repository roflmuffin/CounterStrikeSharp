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

#include <fstream>
#include "core/log.h"
#include "core/coreconfig.h"

namespace counterstrikesharp {

CCoreConfig::CCoreConfig(const std::string& path) { m_sPath = path; }

CCoreConfig::~CCoreConfig() = default;

bool CCoreConfig::Init(char* conf_error, int conf_error_size)
{
    std::ifstream ifs(m_sPath);

    if (!ifs) {
        V_snprintf(conf_error, conf_error_size, "CoreConfig file not found.");
        return false;
    }

    m_json = json::parse(ifs);

    try {
        PublicChatTrigger = m_json["PublicChatTrigger"];
        SilentChatTrigger = m_json["SilentChatTrigger"];
        FollowCS2ServerGuidelines = m_json["FollowCS2ServerGuidelines"];
    } catch (const std::exception& ex) {
        V_snprintf(conf_error, conf_error_size, "Failed to parse CoreConfig file: %s", ex.what());
        return false;
    }
    return true;
}

const std::string CCoreConfig::GetPath() const
{
    return m_sPath;
}
} // namespace counterstrikesharp