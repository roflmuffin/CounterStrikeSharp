#pragma once

// The process-wide Resolution Report for core Bindings, and the queries native code uses instead of resolving
// signatures itself. Adapter over core/binding_resolver.h: feeds it the loaded modules and the parsed Gamedata.

#include <cstdint>
#include <optional>
#include <string_view>

#include "core/binding_resolver.h"
#include "core/engine_features.h"

namespace counterstrikesharp::bindings {

// Resolves every core Binding once. Call at boot after the modules are located and before anything registers hooks.
// Logs one summary line naming each failed Binding and its reason.
void ResolveCoreBindings();

// The report from ResolveCoreBindings (empty before it ran).
const ResolutionReport& CoreResolutionReport();

// True if the named Engine Feature is in the native catalog and all of its Bindings resolved.
bool IsEngineFeatureAvailable(std::string_view feature);

// Address of a resolved Signature or RttiVtable Binding; nullptr if it failed or is not declared.
void* GetBindingAddress(std::string_view key);

// Value of a resolved Offset or SchemaField Binding; nullopt if it failed or is not declared.
std::optional<std::int64_t> GetBindingOffset(std::string_view key);

// Scans a raw (plugin-supplied) signature in the module named as for FindSignature, counting matches across all of its
// segments the same way core Bindings are counted. matchCount is 0 if the module is not loaded.
SignatureScan ScanModuleSignature(const char* moduleName, std::string_view signature);

// Gate for hook registration: true if the Engine Feature is available, otherwise logs why (naming what is skipped)
// and returns false. The caller then skips registration instead of failing.
bool ShouldRegisterHooks(std::string_view feature, std::string_view what);

} // namespace counterstrikesharp::bindings
