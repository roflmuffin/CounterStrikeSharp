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

#include <algorithm>
#include <cstdio>
#include <cstdlib>

#include "core/core_bindings.h"
#include "core/coreconfig.h"
#include "core/globals.h"
#include "core/log.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {

// The core Resolution Report and the native Engine Feature catalog, as JSON (see bindings::ResolutionReportToJson).
// Managed reads it once at startup.
static std::string GetResolutionReport(ScriptContext& script_context)
{
    return bindings::ResolutionReportToJson(bindings::CoreResolutionReport(), bindings::NativeEngineFeatures());
}

// The operator's FailMode: crash. Managed decides (it has the whole Engine Feature catalog), logs the startup report and
// the fatal error naming the disabled Engine Features, then calls this. Natives are public, so native refuses too:
// unless the operator set FailMode to crash and a core Binding failed (no Engine Feature can be disabled otherwise).
static void FailModeStopServer(ScriptContext& script_context)
{
    if (!globals::coreConfig->IsFailModeCrash())
    {
        script_context.ThrowNativeError("FailMode is \"%s\", not \"crash\": not stopping the server", globals::coreConfig->FailMode.c_str());
        return;
    }
    const auto& bindings = bindings::CoreResolutionReport().Bindings();
    if (std::none_of(bindings.begin(), bindings.end(), [](const bindings::BindingResult& binding) { return !binding.Resolved(); }))
    {
        script_context.ThrowNativeError("Every core Binding resolved, so no core Engine Feature is disabled: not stopping the server");
        return;
    }

    CSSHARP_CORE_CRITICAL("FailMode is crash and a core Engine Feature is disabled: stopping the server");
    Log::GetCoreLogger()->flush();
    std::fflush(nullptr);
    // No destructors or exit handlers: the process is mid load (before plugins), and the game's own teardown is not safe here.
    std::_Exit(EXIT_FAILURE);
}

REGISTER_NATIVES(bindings, {
    ScriptEngine::RegisterNativeHandler("GET_RESOLUTION_REPORT", GetResolutionReport);
    ScriptEngine::RegisterNativeHandler("FAIL_MODE_STOP_SERVER", FailModeStopServer);
})
} // namespace counterstrikesharp
