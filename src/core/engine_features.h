#pragma once

// The native Engine Feature catalog: which core Bindings each Engine Feature needs (ADR-0002). Pure data.
// To add one, add a row to the table in engine_features.cpp (and a name constant below if native code gates on it).
// Names are permanent: never rename or reuse one.

#include <span>
#include <string_view>
#include <vector>

#include "core/binding_resolver.h"

namespace counterstrikesharp::bindings {

namespace features {
inline constexpr std::string_view ChatSayHooks = "cssharp.chat/say-hooks";
} // namespace features

// Every native Engine Feature, in declaration order.
std::span<const EngineFeatureDeclaration> NativeEngineFeatures();

// Every core Binding resolved at boot: the Bindings of all catalog rows, each key once.
const std::vector<BindingDeclaration>& CoreBindingDeclarations();

// nullptr if the name is not in the catalog.
const EngineFeatureDeclaration* FindNativeEngineFeature(std::string_view name);

} // namespace counterstrikesharp::bindings
