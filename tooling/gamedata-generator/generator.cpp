#include "generator.h"

#include <algorithm>
#include <stdexcept>

namespace counterstrikesharp::generator {
namespace {

// Other indices within the search range where the window matches too. A boot-time Heal could not tell them apart.
std::vector<std::size_t> OtherWindowMatchesNearby(std::span<const vtable::Fingerprint> fingerprints, const vtable::SlotRecord& record)
{
    std::vector<std::size_t> matches;
    const std::size_t first = record.index >= vtable::kSearchRange ? record.index - vtable::kSearchRange : 0;
    const std::size_t last = std::min(fingerprints.size() - 1, record.index + vtable::kSearchRange);
    for (std::size_t index = first; index <= last; ++index)
        if (index != record.index && vtable::WindowMatchesAt(record.window, fingerprints, index)) matches.push_back(index);
    return matches;
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

    if (const auto others = OtherWindowMatchesNearby(fingerprints, record); !others.empty())
    {
        std::string indices;
        for (const auto index : others)
            indices += (indices.empty() ? "" : ", ") + std::to_string(index);
        generated.warnings.push_back(request.key + ": its window also matches at " + indices + " (identical bodies nearby); a slot shift there fails instead of healing");
    }
    const auto& own = *record.window.slots[1];
    if (own == vtable::kPureVirtual || own == vtable::kDeletedVirtual)
        generated.warnings.push_back(request.key + ": slot " + std::to_string(request.index) + " is a " + own + " virtual");
    return generated;
}

} // namespace counterstrikesharp::generator
