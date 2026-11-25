#pragma once

// The offline Gamedata generator (ADR-0004): its only output is Gamedata in the Slot Binding shape that
// bindings::ParseGamedata reads (see core/binding_resolver.h). Two modes: bootstrap (record a Slot Binding at an index
// known correct for one build) and carry-forward (move every Slot Binding of one build's Gamedata to the next build by
// aligning vtables). Uses the same fingerprint core as the boot-time Vtable Check (core/vtable_fingerprint.h). Never
// runs at server boot and is not shipped with the plugin.

#include <cstddef>
#include <functional>
#include <span>
#include <string_view>
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
    GamedataJson entry; // the key's Gamedata json with its Slot Binding fields set
    std::vector<std::string> warnings; // things a maintainer should look at; the entry is still emitted
};

// Bootstrap: records the Slot Binding for request.index of a class's vtable (its fingerprints on one build, e.g.
// LoadedModule::Fingerprints), merged into the key's existing Gamedata json (other fields, e.g. a Windows index, are
// kept). Throws std::runtime_error if the index is outside the vtable.
GeneratedSlotBinding BootstrapSlotBinding(std::span<const vtable::Fingerprint> fingerprints,
                                          const SlotBindingRequest& request,
                                          GamedataJson entry = GamedataJson::object());

// The "vtable.linux" object for a record: { "window": [...], "uniqueNearby": ..., "size": ... }.
GamedataJson SlotRecordJson(const vtable::SlotRecord& record);

// --- Carry-forward ---

// One build's class vtables: the fingerprints of every slot of a class's primary vtable in a module (by Gamedata
// library name), e.g. LoadedModule::Fingerprints. Throws std::runtime_error when the class cannot be found exactly once.
using VtableFingerprints = std::function<std::vector<vtable::Fingerprint>(std::string_view library, std::string_view className)>;

struct CarriedSlotBinding
{
    std::string key;
    std::size_t from = 0; // index on the previous build
    std::size_t to = 0; // index on the next build (== from when the slot did not move)
};

// A Slot Binding the generator could not carry forward unambiguously. Its entry is left as it was, for a human.
struct HeldSlotBinding
{
    std::string key;
    std::string reason;
};

struct CarryForwardResult
{
    GamedataJson gamedata; // the previous Gamedata with every carried Slot Binding regenerated; all else as it was
    std::vector<CarriedSlotBinding> carried;
    std::vector<HeldSlotBinding> held;
    std::vector<std::string> warnings; // as for a bootstrap, about carried entries
};

// Carries every Slot Binding (a Gamedata key with "vtable") of the previous build's Gamedata forward to the next build.
// Never guesses: the slot must verify on the previous build at its Gamedata index (by the boot-time Vtable Check, see
// vtable::CheckSlot), its window there must occur exactly once in the previous vtable, and that window must match
// exactly once in the next build's vtable; the new index is where it matches. Its window, uniqueNearby and size are
// then recorded on the next build. Anything else (no window, class not found, ambiguous, not found) is held: listed
// with a reason and left unchanged. Signatures and other entries pass through untouched; key order is kept.
CarryForwardResult CarryForward(const GamedataJson& previousGamedata, const VtableFingerprints& previousBuild, const VtableFingerprints& nextBuild);

// The Gamedata library names the Slot Bindings of a Gamedata file live in (each once, sorted), so a caller can check the
// archive has both builds' modules before carrying forward. Slot Bindings without a window need none: they are held.
std::vector<std::string> SlotBindingLibraries(const GamedataJson& gamedata);

} // namespace counterstrikesharp::generator
