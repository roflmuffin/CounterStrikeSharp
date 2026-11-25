#pragma once

// The offline Gamedata generator (ADR-0004): its only output is Gamedata in the Slot Binding shape that
// bindings::ParseGamedata reads (see core/binding_resolver.h). Uses the same fingerprint core as the boot-time Vtable
// Check (core/vtable_fingerprint.h). Never runs at server boot and is not shipped with the plugin.

#include <cstddef>
#include <span>
#include <string>
#include <vector>

#include <nlohmann/json.hpp>

#include "build_archive.h"
#include "core/vtable_fingerprint.h"

namespace counterstrikesharp::generator {

// Gamedata as the maintainer reviews it: key order kept.
using GamedataJson = nlohmann::ordered_json;

struct SlotBindingRequest
{
    std::string key; // Gamedata key, e.g. "ISource2Server::GameFrame"
    std::string className; // RTTI class whose primary vtable holds the slot, e.g. "CSource2Server"
    std::string library; // Gamedata library name of its module: "server", "engine" or "tier0"
    std::size_t index = 0; // the slot index on that build (bootstrap: from SDK headers known correct for it)
};

struct GeneratedSlotBinding
{
    GamedataJson entry; // the key's Gamedata entry with its Slot Binding fields set
    std::vector<std::string> warnings; // things a maintainer should look at; the entry is still emitted
};

// Bootstrap: records the Slot Binding for request.index of a class's vtable (its fingerprints on one build, e.g.
// LoadedModule::Fingerprints), merged into the key's existing Gamedata entry (other fields, e.g. a Windows index, are
// kept). Throws std::runtime_error if the index is outside the vtable.
GeneratedSlotBinding BootstrapSlotBinding(std::span<const vtable::Fingerprint> fingerprints,
                                          const SlotBindingRequest& request,
                                          GamedataJson entry = GamedataJson::object());

// The "vtable.linux" object for a record: { "window": [...], "uniqueNearby": ..., "size": ... }.
GamedataJson SlotRecordJson(const vtable::SlotRecord& record);

} // namespace counterstrikesharp::generator
