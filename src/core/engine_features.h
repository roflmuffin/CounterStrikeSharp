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
// Server lifecycle (hooks on ISource2Server, by verified slot index)
inline constexpr std::string_view GameFrame = "cssharp.server/game-frame";
inline constexpr std::string_view HibernationEvents = "cssharp.server/hibernation-events";
inline constexpr std::string_view SteamApiEvents = "cssharp.server/steam-api-events";
inline constexpr std::string_view HostnameChanged = "cssharp.server/hostname-changed";
inline constexpr std::string_view FatalShutdown = "cssharp.server/fatal-shutdown";
inline constexpr std::string_view UpdateWhenNotInGame = "cssharp.server/update-when-not-in-game";
inline constexpr std::string_view PreWorldUpdate = "cssharp.server/pre-world-update";
// Direct calls on ISource2Server, by verified slot index
inline constexpr std::string_view EconItemSystem = "cssharp.server/econ-item-system";
inline constexpr std::string_view IsPaused = "cssharp.server/is-paused";
// Players and UI (hooks on IServerGameClients, by verified slot index)
inline constexpr std::string_view PlayerConnectionEvents = "cssharp.player/connection-events";
inline constexpr std::string_view PlayerClientCommands = "cssharp.player/client-commands";
inline constexpr std::string_view PlayerVoiceEvents = "cssharp.player/voice-events";
inline constexpr std::string_view UiCustomHudLayout = "cssharp.ui/custom-hud-layout";
// Events and networking
inline constexpr std::string_view GameEvents = "cssharp.events/game-events";
inline constexpr std::string_view FireEventToClient = "cssharp.events/fire-event-to-client";
inline constexpr std::string_view StateChanged = "cssharp.network/state-changed";
inline constexpr std::string_view CheckTransmit = "cssharp.network/check-transmit";
// Game system
inline constexpr std::string_view ServerGameSystemEvents = "cssharp.server/game-system-events";
// Entity IO and entity helpers
inline constexpr std::string_view EntityOutputHooks = "cssharp.entity/output-hooks";
inline constexpr std::string_view EntityAcceptInput = "cssharp.entity/accept-input";
inline constexpr std::string_view EntityAddIoEvent = "cssharp.entity/add-io-event";
inline constexpr std::string_view EntityEmitSound = "cssharp.entity/emit-sound";
inline constexpr std::string_view EntityDispatchSpawn = "cssharp.entity/dispatch-spawn";
// Take damage
inline constexpr std::string_view EntityTakeDamageHooks = "cssharp.entity/take-damage-hooks";
// Console commands and ConVars (ICvar, by verified slot index)
inline constexpr std::string_view ConsoleDispatch = "cssharp.commands/console-dispatch";
inline constexpr std::string_view AddCommand = "cssharp.commands/add-command";
inline constexpr std::string_view ExecuteClientCommandFromServer = "cssharp.player/execute-client-command-from-server";
inline constexpr std::string_view ConVarFind = "cssharp.convars/find";
inline constexpr std::string_view ConVarRead = "cssharp.convars/read";
inline constexpr std::string_view ConVarWrite = "cssharp.convars/write";
inline constexpr std::string_view ConVarCreate = "cssharp.convars/create";
inline constexpr std::string_view ConVarChangeEvents = "cssharp.convars/change-events";
// Interfaces that never shifted a slot we use: hooks by verified slot index, direct calls compiled but verified
inline constexpr std::string_view Startup = "cssharp.server/startup";
inline constexpr std::string_view LoopMode = "cssharp.server/loop-mode";
inline constexpr std::string_view GlobalVars = "cssharp.server/global-vars";
inline constexpr std::string_view UserMessages = "cssharp.network/user-messages";
} // namespace features

// Every native Engine Feature, in declaration order.
std::span<const EngineFeatureDeclaration> NativeEngineFeatures();

// Every core Binding resolved at boot, each key once: the Bindings of all catalog rows (with their declared kinds,
// even when Gamedata lacks the key, so the report says why), then every other key in the core Gamedata by its section
// ("vtable" -> Slot, else "signatures" -> Signature, else "offsets" -> Offset). So keys only managed code consumes are
// resolved and reported too; managed declares which of its Engine Features need them.
std::vector<BindingDeclaration> CoreBindingDeclarations(const Gamedata& gamedata);

// nullptr if the name is not in the catalog.
const EngineFeatureDeclaration* FindNativeEngineFeature(std::string_view name);

} // namespace counterstrikesharp::bindings
