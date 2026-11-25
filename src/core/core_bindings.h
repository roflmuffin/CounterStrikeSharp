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

// Slot index of a verified (or unverified) Slot Binding; nullopt if it failed, is not declared, or (Windows) has no
// Gamedata index, in which case the caller's compiled header index applies (see core/slot_calls.h, core/slot_hooks.h).
//
// These lookups read the current report by key through its index on every call, so they stay cheap on hot paths and
// follow a new report when one replaces it (ADR-0002); callers do not cache their results.
std::optional<int> GetBindingSlot(std::string_view key);

// Scans a raw (plugin-supplied) signature in the module named as for FindSignature, counting matches across all of its
// segments the same way core Bindings are counted. matchCount is 0 if the module is not loaded.
SignatureScan ScanModuleSignature(const char* moduleName, std::string_view signature);

// Gate for hook registration: true if the Engine Feature is available, otherwise false and the caller skips registration
// instead of failing. Does not report the failure (debug level only, naming what is skipped): the boot summary line and
// the managed startup report are the only reports of Binding failures.
bool ShouldRegisterHooks(std::string_view feature, std::string_view what);

// Message for a native called while its native Engine Feature is disabled, in the wording of the managed typed
// exception: "Engine Feature cssharp.x/y is disabled (Key: reason)". Pass to ThrowNativeError.
std::string DescribeUnavailable(std::string_view feature);

} // namespace counterstrikesharp::bindings
