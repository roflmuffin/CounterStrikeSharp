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

#include "core/core_bindings.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {

// The core Resolution Report and the native Engine Feature catalog, as JSON (see bindings::ResolutionReportToJson).
// Managed reads it once at startup.
static std::string GetResolutionReport(ScriptContext& script_context)
{
    return bindings::ResolutionReportToJson(bindings::CoreResolutionReport(), bindings::NativeEngineFeatures());
}

REGISTER_NATIVES(bindings, { ScriptEngine::RegisterNativeHandler("GET_RESOLUTION_REPORT", GetResolutionReport); })
} // namespace counterstrikesharp
