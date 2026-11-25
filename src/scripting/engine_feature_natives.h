#pragma once

// A native of an Engine Feature checks it first (ADR-0002): while the Engine Feature is disabled, the native throws in
// the wording of the managed typed exception instead of calling into the engine.

#include <string_view>

#include "core/core_bindings.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {

// True if the Engine Feature is available; otherwise throws the native error describing it and returns false.
inline bool RequireEngineFeature(ScriptContext& scriptContext, std::string_view feature)
{
    if (bindings::IsEngineFeatureAvailable(feature)) return true;
    scriptContext.ThrowNativeError("%s", bindings::DescribeUnavailable(feature).c_str());
    return false;
}

} // namespace counterstrikesharp
