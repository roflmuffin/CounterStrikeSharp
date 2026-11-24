#pragma once

// The native Engine Feature catalog: which core Bindings each Engine Feature needs (ADR-0002). Pure data.
// To add one, add a row to the table in engine_features.cpp (and a name constant below if native code gates on it),
// then regenerate the managed enum: dotnet run --project tooling/CodeGen.Natives -- engine-features
// Names are permanent: never rename or reuse one. Engine Features only managed code consumes are declared in managed
// (Core/Bindings/ManagedEngineFeatures.cs); native resolves their Gamedata keys anyway (CoreBindingDeclarations).

#include <span>
#include <string_view>
#include <vector>

#include "core/binding_resolver.h"

namespace counterstrikesharp::bindings {

namespace features {
inline constexpr std::string_view ChatSayHooks = "cssharp.chat/say-hooks";
// ticket 09
inline constexpr std::string_view GameEvents = "cssharp.events/game-events";
inline constexpr std::string_view FireEventToClient = "cssharp.events/fire-event-to-client";
inline constexpr std::string_view StateChanged = "cssharp.network/state-changed";
inline constexpr std::string_view CheckTransmit = "cssharp.network/check-transmit";
// ticket 08
inline constexpr std::string_view EntityOutputHooks = "cssharp.entity/output-hooks";
inline constexpr std::string_view EntityAcceptInput = "cssharp.entity/accept-input";
inline constexpr std::string_view EntityAddIoEvent = "cssharp.entity/add-io-event";
inline constexpr std::string_view EntityEmitSound = "cssharp.entity/emit-sound";
inline constexpr std::string_view EntityDispatchSpawn = "cssharp.entity/dispatch-spawn";
} // namespace features

// Every native Engine Feature, in declaration order.
std::span<const EngineFeatureDeclaration> NativeEngineFeatures();

// Every core Binding resolved at boot, each key once: the Bindings of all catalog rows (with their declared kinds,
// even when Gamedata lacks the key, so the report says why), then every other key in the core Gamedata by its section
// ("signatures" -> Signature, else "offsets" -> Offset). So keys only managed code consumes are resolved and reported
// too; managed declares which of its Engine Features need them.
std::vector<BindingDeclaration> CoreBindingDeclarations(const Gamedata& gamedata);

// nullptr if the name is not in the catalog.
const EngineFeatureDeclaration* FindNativeEngineFeature(std::string_view name);

} // namespace counterstrikesharp::bindings
