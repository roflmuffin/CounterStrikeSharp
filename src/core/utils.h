#pragma once

#include <public/eiface.h>
#include <string>
#include <filesystem>
#include <regex>
#include <algorithm>

#include "core/globals.h"

namespace counterstrikesharp {
namespace utils {

static std::string gameDirectory;
inline std::string GameDirectory()
{
    if (gameDirectory.empty())
    {
        CBufferStringGrowable<255> gamePath;
        globals::engine->GetGameDir(gamePath);
        gameDirectory = std::string(gamePath.Get());
    }

    return gameDirectory;
}

// clang-format off
inline std::string RelativeDirectory(const std::string& initPath = "")
{
    static std::string storedPath;
    static bool isInitialized = false;

    if (!initPath.empty() && !isInitialized)
    {
        std::string processedPath = initPath;

        processedPath.erase(processedPath.begin(), std::find_if(processedPath.begin(), processedPath.end(), [](unsigned char ch) {
            return !std::isspace(ch);
        }));

        processedPath.erase(std::find_if(processedPath.rbegin(), processedPath.rend(), [](unsigned char ch) {
            return !std::isspace(ch);
        }).base(), processedPath.end());

        processedPath = std::regex_replace(processedPath, std::regex(R"([\\/]+)"), "/");

        if (!processedPath.empty())
        {
            if (processedPath[0] != '/')
            {
                processedPath = "/" + processedPath;
            }
            if (processedPath.back() == '/' && processedPath.length() > 1)
            {
                processedPath.pop_back();
            }
        }

        std::string fullPath = GameDirectory() + processedPath;
        if (std::filesystem::exists(fullPath) && std::filesystem::is_directory(fullPath))
        {
            storedPath = processedPath;
            isInitialized = true;
        }
        else
        {
            return "NotFound";
        }
    }

    return isInitialized ? storedPath : "/addons/counterstrikesharp";
}
// clang-format on

inline std::string GetRootDirectory() { return GameDirectory() + RelativeDirectory(); }
inline std::string PluginsDirectory() { return GameDirectory() + RelativeDirectory() + "/plugins"; }
inline std::string ConfigsDirectory() { return GameDirectory() + RelativeDirectory() + "/configs"; }
inline std::string GamedataDirectory() { return GameDirectory() + RelativeDirectory() + "/gamedata"; }

} // namespace utils
} // namespace counterstrikesharp
