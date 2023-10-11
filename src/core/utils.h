#pragma once

#include <public/eiface.h>
#include <string>

#include "core/globals.h"

namespace counterstrikesharp {
namespace utils {

static std::string gameDirectory;

inline std::string GameDirectory() {
    if (gameDirectory.empty()) {
        CBufferStringGrowable<255> gamePath;
        globals::engine->GetGameDir(gamePath);
        gameDirectory = std::string(gamePath.Get());
    }

    return gameDirectory;
}

inline std::string PluginDirectory() { return GameDirectory() + "/addons/counterstrikesharp"; }

inline std::string ConfigDirectory() { return PluginDirectory() + "/config"; }
}  // namespace utils
}  // namespace counterstrikesharp
