#include "generator.h"

#include <algorithm>
#include <map>
#include <set>
#include <stdexcept>
#include <utility>

#include "core/binding_resolver.h"

namespace counterstrikesharp::generator {
namespace {

std::string JoinIndices(std::span<const std::size_t> indices)
{
    std::string joined;
    for (const auto index : indices)
        joined += (joined.empty() ? "" : ", ") + std::to_string(index);
    return joined;
}

// The previous Gamedata entry of a Slot Binding as the resolver reads it on Linux.
bindings::GamedataEntry ParseEntry(const std::string& key, const GamedataJson& entry)
{
    const auto parsed = bindings::ParseGamedata(nlohmann::json{ { key, nlohmann::json::parse(entry.dump()) } }, "linux");
    return parsed.at(key);
}

// Fingerprints of one build's vtables, each (library, class) read once.
class VtableCache
{
  public:
    explicit VtableCache(const VtableFingerprints& source) : m_source(source) {}

    const std::vector<vtable::Fingerprint>& Get(const std::string& library, const std::string& className)
    {
        const auto key = std::make_pair(library, className);
        auto it = m_vtables.find(key);
        if (it == m_vtables.end()) it = m_vtables.emplace(key, m_source(library, className)).first;
        return it->second;
    }

  private:
    const VtableFingerprints& m_source;
    std::map<std::pair<std::string, std::string>, std::vector<vtable::Fingerprint>> m_vtables;
};

// Thrown while carrying one Slot Binding: it is held with this reason.
struct Hold : std::runtime_error
{
    using std::runtime_error::runtime_error;
};

// Where the Slot Binding of a previous Gamedata entry sits on the next build, or Hold.
std::size_t AlignSlot(const bindings::GamedataEntry& entry, VtableCache& previousBuild, VtableCache& nextBuild)
{
    const auto& slot = *entry.slot;
    if (!slot.className || !slot.library) throw Hold("no vtable.class or vtable.library");
    if (!entry.offset || *entry.offset < 0) throw Hold("no offsets.linux index");
    if (!slot.window) throw Hold("no fingerprint window for linux");
    const auto index = static_cast<std::size_t>(*entry.offset);

    const auto lookup = [&](VtableCache& build, const char* which) -> const std::vector<vtable::Fingerprint>& {
        try
        {
            return build.Get(*slot.library, *slot.className);
        }
        catch (const std::runtime_error& error)
        {
            throw Hold(std::string(which) + " build: " + error.what());
        }
    };
    const auto& previous = lookup(previousBuild, "previous");
    const auto& next = lookup(nextBuild, "next");

    // The Gamedata must hold on the previous build the way the boot-time Vtable Check would accept it there; its window
    // on that build (which a changed neighbour may have refreshed) is what gets aligned.
    const vtable::SlotRecord recorded{ index, *slot.window, slot.uniqueNearby, slot.vtableSize };
    if (vtable::CheckSlot(recorded, previous) == vtable::SlotMatch::None)
        throw Hold("its window does not match the previous build at index " + std::to_string(index) + " (Gamedata is not for that build)");
    const auto window = vtable::WindowAt(previous, index);

    // The boot-time match rule (core/vtable_fingerprint.h) over the whole vtable: a carried index is one a Heal would
    // also accept, and a window found more than once anywhere is never carried.
    if (const auto inPrevious = vtable::WindowMatchesAnywhere(window, previous, index); inPrevious.size() != 1)
        throw Hold("ambiguous: its window occurs at " + JoinIndices(inPrevious) + " on the previous build");

    const auto inNext = vtable::WindowMatchesAnywhere(window, next, index);
    if (inNext.empty()) throw Hold("its window matches nowhere in " + *slot.className + "'s vtable on the next build");
    if (inNext.size() > 1) throw Hold("ambiguous: its window matches at " + JoinIndices(inNext) + " on the next build");
    return inNext.front();
}

} // namespace

GamedataJson SlotRecordJson(const vtable::SlotRecord& record)
{
    auto window = GamedataJson::array();
    for (const auto& fingerprint : record.window.slots)
        window.push_back(fingerprint ? GamedataJson(*fingerprint) : GamedataJson(nullptr));
    GamedataJson json{ { "window", std::move(window) }, { "uniqueNearby", record.uniqueNearby } };
    if (record.vtableSize) json["size"] = *record.vtableSize;
    return json;
}

GeneratedSlotBinding BootstrapSlotBinding(std::span<const vtable::Fingerprint> fingerprints, const SlotBindingRequest& request, GamedataJson entry)
{
    if (request.index >= fingerprints.size())
        throw std::runtime_error(request.key + ": index " + std::to_string(request.index) + " is outside " + request.className + "'s vtable (" +
                                 std::to_string(fingerprints.size()) + " slots)");

    const auto record = vtable::RecordSlot(fingerprints, request.index);

    GeneratedSlotBinding generated;
    if (!entry.is_object()) entry = GamedataJson::object();
    entry["offsets"]["linux"] = request.index;
    auto& slot = entry["vtable"];
    slot["class"] = request.className;
    slot["library"] = request.library;
    slot["linux"] = SlotRecordJson(record);
    generated.entry = std::move(entry);

    // Other indices nearby where the window matches too: a boot-time Heal could not tell them apart.
    if (const auto others = vtable::WindowMatchesNear(record.window, fingerprints, record.index); !others.empty())
        generated.warnings.push_back(request.key + ": its window also matches at " + JoinIndices(others) +
                                     " (identical bodies nearby); a slot shift there fails instead of healing");
    const auto& own = *record.window.slots[1];
    if (own == vtable::kPureVirtual || own == vtable::kDeletedVirtual)
        generated.warnings.push_back(request.key + ": slot " + std::to_string(request.index) + " is a " + own + " virtual");
    return generated;
}

CarryForwardResult CarryForward(const GamedataJson& previousGamedata, const VtableFingerprints& previousBuild, const VtableFingerprints& nextBuild)
{
    CarryForwardResult result;
    result.gamedata = previousGamedata;
    if (!previousGamedata.is_object()) return result;

    VtableCache previous(previousBuild);
    VtableCache next(nextBuild);
    for (const auto& [key, value] : previousGamedata.items())
    {
        const auto entry = ParseEntry(key, value);
        if (!entry.slot) continue;
        try
        {
            const auto to = AlignSlot(entry, previous, next);
            const SlotBindingRequest request{ key, *entry.slot->className, *entry.slot->library, to };
            auto generated = BootstrapSlotBinding(next.Get(request.library, request.className), request, value);
            result.gamedata[key] = std::move(generated.entry);
            result.carried.push_back({ key, static_cast<std::size_t>(*entry.offset), to });
            for (auto& warning : generated.warnings)
                result.warnings.push_back(std::move(warning));
        }
        catch (const Hold& hold)
        {
            result.held.push_back({ key, hold.what() });
        }
    }
    return result;
}

std::vector<std::string> SlotBindingLibraries(const GamedataJson& gamedata)
{
    std::set<std::string> libraries;
    if (gamedata.is_object())
        for (const auto& [key, value] : gamedata.items())
            // Without a window a Slot Binding is held before any module is read (e.g. one in a module not archived).
            if (const auto entry = ParseEntry(key, value); entry.slot && entry.slot->library && entry.slot->window)
                libraries.insert(*entry.slot->library);
    return { libraries.begin(), libraries.end() };
}

} // namespace counterstrikesharp::generator
