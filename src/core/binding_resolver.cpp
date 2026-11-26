#include "core/binding_resolver.h"

#include <algorithm>
#include <tuple>
#include <utility>

#include "core/signature.h"

namespace counterstrikesharp::bindings {
namespace {

BindingResult Failed(BindingResult result, FailureReason reason)
{
    result.status = BindingStatus::Failed;
    result.reason = reason;
    return result;
}

BindingResult ResolvedAt(BindingResult result, std::uintptr_t address)
{
    result.status = BindingStatus::Resolved;
    result.reason = FailureReason::None;
    result.address = address;
    return result;
}

// Counts matches across every segment (overlapping matches count separately); records the first.
std::size_t CountMatches(const ModuleImage& image, const std::vector<int16_t>& pattern, std::uintptr_t& firstMatch)
{
    std::size_t count = 0;
    for (const auto& segment : image.segments)
    {
        const std::uint8_t* cursor = segment.data;
        const std::uint8_t* end = segment.data + segment.size;
        while (cursor < end)
        {
            const auto* match = FindPattern(cursor, static_cast<std::size_t>(end - cursor), pattern);
            if (match == nullptr) break;
            if (count++ == 0) firstMatch = segment.address + static_cast<std::uintptr_t>(match - segment.data);
            cursor = match + 1;
        }
    }
    return count;
}

BindingResult ResolveSignature(BindingResult result, const Gamedata& gamedata, const ModuleImages& modules)
{
    const auto entry = gamedata.find(result.key);
    if (entry == gamedata.end() || !entry->second.hasSignatures) return Failed(std::move(result), FailureReason::MissingFromGamedata);
    if (!entry->second.signature) return Failed(std::move(result), FailureReason::MissingForThisOs);
    if (!entry->second.library) return Failed(std::move(result), FailureReason::MissingFromGamedata);

    result.library = *entry->second.library;
    const std::string& signature = *entry->second.signature;

    const bool isSymbol = !signature.empty() && signature[0] == '@';
    std::vector<int16_t> pattern;
    if (!isSymbol)
    {
        pattern = ParseSignature(signature);
        if (pattern.empty()) return Failed(std::move(result), FailureReason::InvalidPattern);
    }
    else if (signature.size() == 1)
    {
        return Failed(std::move(result), FailureReason::InvalidPattern);
    }

    const auto module = modules.find(result.library);
    if (module == modules.end()) return Failed(std::move(result), FailureReason::NotFound);

    if (isSymbol)
    {
        const std::uintptr_t address = module->second.findSymbol ? module->second.findSymbol(signature.substr(1)) : 0;
        if (address == 0) return Failed(std::move(result), FailureReason::NotFound);
        result.matchCount = 1;
        return ResolvedAt(std::move(result), address);
    }

    std::uintptr_t first = 0;
    result.matchCount = CountMatches(module->second, pattern, first);
    if (result.matchCount == 0) return Failed(std::move(result), FailureReason::NotFound);
    if (result.matchCount > 1) return Failed(std::move(result), FailureReason::Ambiguous);
    return ResolvedAt(std::move(result), first);
}

BindingResult ResolveOffset(BindingResult result, const Gamedata& gamedata)
{
    const auto entry = gamedata.find(result.key);
    if (entry == gamedata.end() || !entry->second.hasOffsets) return Failed(std::move(result), FailureReason::MissingFromGamedata);
    if (!entry->second.offset) return Failed(std::move(result), FailureReason::MissingForThisOs);

    result.status = BindingStatus::Resolved;
    result.reason = FailureReason::None;
    result.offset = *entry->second.offset;
    return result;
}

BindingResult ResolveRttiVtable(BindingResult result, const ModuleImages& modules)
{
    const auto module = modules.find(result.library);
    const auto lookup = module != modules.end() && module->second.findVirtualTable ? module->second.findVirtualTable(result.key)
                                                                                   : rtti::VtableLookup{};
    result.matchCount = lookup.matchCount;
    if (lookup.Ambiguous()) return Failed(std::move(result), FailureReason::Ambiguous);
    if (!lookup.Found() || lookup.address == 0) return Failed(std::move(result), FailureReason::RttiVtableNotFound);
    return ResolvedAt(std::move(result), lookup.address);
}

// Key is "Class::field"; a key without "::" names a class and no field.
BindingResult ResolveSchemaField(BindingResult result, const ModuleImages& modules)
{
    const std::string_view key = result.key;
    const auto separator = key.rfind("::");
    const std::string_view className = separator == std::string_view::npos ? key : key.substr(0, separator);
    const std::string_view fieldName = separator == std::string_view::npos ? std::string_view{} : key.substr(separator + 2);

    const auto module = modules.find(result.library);
    const SchemaFieldLookup lookup = module != modules.end() && module->second.findSchemaField
                                         ? module->second.findSchemaField(className, fieldName)
                                         : SchemaFieldLookup{};

    switch (lookup.status)
    {
        case SchemaFieldStatus::Found:
            result.status = BindingStatus::Resolved;
            result.reason = FailureReason::None;
            result.offset = lookup.offset;
            return result;
        case SchemaFieldStatus::FieldMissing:
            return Failed(std::move(result), FailureReason::SchemaFieldMissing);
        case SchemaFieldStatus::ClassMissing:
            break;
    }
    return Failed(std::move(result), FailureReason::SchemaClassMissing);
}

// Live fingerprints of one class's vtable, computed once per Resolve and shared by all its Slot Bindings.
struct LiveVtable
{
    rtti::VtableLookup lookup;
    std::vector<vtable::Fingerprint> fingerprints;
};
using LiveVtables = std::map<std::pair<std::string, std::string>, LiveVtable>;

const LiveVtable& FingerprintLiveVtable(LiveVtables& cache, const ModuleImage& module, const std::string& library, const std::string& className)
{
    const auto key = std::make_pair(library, className);
    if (const auto cached = cache.find(key); cached != cache.end()) return cached->second;

    LiveVtable live;
    live.lookup = module.findVirtualTable ? module.findVirtualTable(className) : rtti::VtableLookup{};
    if (live.lookup.Found() && live.lookup.address != 0 && module.readVtableSlot)
    {
        const auto address = live.lookup.address;
        live.fingerprints = vtable::FingerprintVtable({ [&](std::size_t index) {
                                                           return module.readVtableSlot(address, index);
                                                       },
                                                        module.segments, module.pureVirtual, module.deletedVirtual });
    }
    return cache.emplace(key, std::move(live)).first->second;
}

BindingResult Unverified(BindingResult result, std::int64_t index)
{
    result.status = BindingStatus::Unverified;
    result.reason = FailureReason::None;
    result.offset = index;
    return result;
}

// A compiled call goes to its header index whatever the Vtable Check found: verified elsewhere, the call would hit
// another function. Only verified results are compared; an unverified Binding is used at the index Gamedata states.
BindingResult CheckCompiledIndex(BindingResult result, const std::optional<std::int64_t>& compiledIndex)
{
    const bool verified = result.status == BindingStatus::Resolved || result.status == BindingStatus::Healed;
    if (!compiledIndex || !verified || result.offset == *compiledIndex) return result;
    return Failed(std::move(result), FailureReason::CompiledSlotMoved);
}

BindingResult ResolveSlot(BindingResult result,
                          bool headerIndex,
                          const Gamedata& gamedata,
                          const ModuleImages& modules,
                          Platform platform,
                          bool heal,
                          LiveVtables& cache)
{
    const auto entry = gamedata.find(result.key);
    const bool hasEntry = entry != gamedata.end();
    const SlotGamedata* slot = hasEntry && entry->second.slot ? &*entry->second.slot : nullptr;

    result.slot = SlotEvidence{};
    if (slot != nullptr)
    {
        result.slot->className = slot->className.value_or("");
        result.slot->recordedVtableSize = slot->vtableSize;
        result.slot->window = slot->window;
        if (slot->library) result.library = *slot->library;
    }
    if (hasEntry && entry->second.offset) result.slot->statedIndex = *entry->second.offset;

    // No Vtable Check on Windows: the Gamedata index if there is one, else the caller's compiled header index (-1), if
    // it has one.
    if (platform == Platform::Windows && headerIndex) return Unverified(std::move(result), result.slot->statedIndex);

    if (!hasEntry || !entry->second.hasOffsets) return Failed(std::move(result), FailureReason::MissingFromGamedata);
    if (!entry->second.offset || *entry->second.offset < 0) return Failed(std::move(result), FailureReason::MissingForThisOs);
    if (platform == Platform::Windows) return Unverified(std::move(result), *entry->second.offset);
    const auto index = *entry->second.offset;

    // Gamedata without a window (e.g. from before Slot Bindings) is used as it is, and reported.
    if (slot == nullptr || !slot->window) return Unverified(std::move(result), index);
    if (!slot->className || !slot->library) return Failed(std::move(result), FailureReason::MissingFromGamedata);

    const auto module = modules.find(*slot->library);
    if (module == modules.end()) return Failed(std::move(result), FailureReason::NotFound);

    const auto& live = FingerprintLiveVtable(cache, module->second, *slot->library, *slot->className);
    result.matchCount = live.lookup.matchCount;
    if (live.lookup.Ambiguous()) return Failed(std::move(result), FailureReason::Ambiguous);
    if (!live.lookup.Found() || live.lookup.address == 0) return Failed(std::move(result), FailureReason::RttiVtableNotFound);
    result.address = live.lookup.address;
    result.slot->vtableSize = live.fingerprints.size();

    const vtable::SlotRecord recorded{ static_cast<std::size_t>(index), *slot->window, slot->uniqueNearby, slot->vtableSize };
    // Heal off: the Vtable Check at the Gamedata index only.
    const auto location = heal ? vtable::LocateSlot(recorded, live.fingerprints)
                               : vtable::SlotLocation{ vtable::CheckSlot(recorded, live.fingerprints), recorded.index, 0 };
    result.slot->match = location.match;
    if (location.match == vtable::SlotMatch::None)
    {
        if (location.windowMatches < 2) return Failed(std::move(result), FailureReason::FingerprintMismatch);
        result.matchCount = location.windowMatches;
        return Failed(std::move(result), FailureReason::AmbiguousHeal);
    }

    result.status = location.match == vtable::SlotMatch::Heal ? BindingStatus::Healed : BindingStatus::Resolved;
    result.reason = FailureReason::None;
    result.offset = static_cast<std::int64_t>(location.index);
    return result;
}

std::optional<vtable::FingerprintWindow> ParseWindow(const nlohmann::json& value)
{
    if (!value.is_array() || value.size() != 3) return std::nullopt;
    vtable::FingerprintWindow window;
    for (std::size_t position = 0; position < 3; ++position)
    {
        if (value[position].is_string()) window.slots[position] = value[position].get<std::string>();
        else if (!value[position].is_null())
            return std::nullopt;
    }
    if (!window.slots[1]) return std::nullopt; // the slot itself is always inside its vtable
    return window;
}

SlotGamedata ParseSlot(const nlohmann::json& value, const std::string& os)
{
    SlotGamedata slot;
    if (const auto className = value.find("class"); className != value.end() && className->is_string())
        slot.className = className->get<std::string>();
    if (const auto library = value.find("library"); library != value.end() && library->is_string()) slot.library = library->get<std::string>();
    if (const auto platform = value.find(os); platform != value.end() && platform->is_object())
    {
        if (const auto window = platform->find("window"); window != platform->end()) slot.window = ParseWindow(*window);
        if (const auto unique = platform->find("uniqueNearby"); unique != platform->end() && unique->is_boolean())
            slot.uniqueNearby = unique->get<bool>();
        if (const auto size = platform->find("size"); size != platform->end() && size->is_number_unsigned())
            slot.vtableSize = size->get<std::size_t>();
    }
    return slot;
}

} // namespace

SignatureScan ScanSignature(const ModuleImage& image, std::string_view signature)
{
    SignatureScan scan;
    const auto pattern = ParseSignature(signature);
    if (pattern.empty()) return scan;

    scan.validPattern = true;
    scan.matchCount = CountMatches(image, pattern, scan.firstMatch);
    return scan;
}

Gamedata ParseGamedata(const nlohmann::json& root, std::string_view platform)
{
    Gamedata gamedata;
    if (!root.is_object()) return gamedata;

    const std::string os(platform);
    for (const auto& [key, value] : root.items())
    {
        GamedataEntry entry;
        if (value.is_object())
        {
            if (const auto signatures = value.find("signatures"); signatures != value.end() && signatures->is_object())
            {
                entry.hasSignatures = true;
                if (const auto library = signatures->find("library"); library != signatures->end() && library->is_string())
                    entry.library = library->get<std::string>();
                if (const auto signature = signatures->find(os); signature != signatures->end() && signature->is_string())
                    entry.signature = signature->get<std::string>();
            }
            if (const auto offsets = value.find("offsets"); offsets != value.end() && offsets->is_object())
            {
                entry.hasOffsets = true;
                if (const auto offset = offsets->find(os); offset != offsets->end() && offset->is_number_integer())
                    entry.offset = offset->get<std::int64_t>();
            }
            if (const auto slot = value.find("vtable"); slot != value.end() && slot->is_object()) entry.slot = ParseSlot(*slot, os);
        }
        gamedata.emplace(key, std::move(entry));
    }
    return gamedata;
}

ResolutionReport::ResolutionReport(std::string buildId, std::vector<BindingResult> bindings, bool healEnabled)
    : m_buildId(std::move(buildId)), m_bindings(std::move(bindings)), m_healEnabled(healEnabled)
{
    // The first Binding of a key wins, as a scan in order would find it.
    for (std::size_t position = 0; position < m_bindings.size(); ++position)
        m_index.emplace(m_bindings[position].key, position);
}

const BindingResult* ResolutionReport::Find(std::string_view key) const
{
    const auto it = m_index.find(key);
    return it == m_index.end() ? nullptr : &m_bindings[it->second];
}

std::optional<int> SlotIndexOf(const ResolutionReport& report, std::string_view key)
{
    const auto* binding = report.Find(key);
    if (binding == nullptr || !binding->Resolved() || binding->kind != BindingKind::Slot || binding->offset < 0) return std::nullopt;
    return static_cast<int>(binding->offset);
}

ResolutionReport BuiltinResolver::Resolve(std::string buildId,
                                          const Gamedata& gamedata,
                                          const ModuleImages& modules,
                                          std::span<const BindingDeclaration> declarations) const
{
    std::vector<BindingResult> results;
    results.reserve(declarations.size());
    LiveVtables liveVtables;

    for (const auto& declaration : declarations)
    {
        BindingResult result;
        result.key = declaration.key;
        result.kind = declaration.kind;
        result.library = declaration.library;

        switch (declaration.kind)
        {
            case BindingKind::Signature:
                results.push_back(ResolveSignature(std::move(result), gamedata, modules));
                break;
            case BindingKind::Offset:
                results.push_back(ResolveOffset(std::move(result), gamedata));
                break;
            case BindingKind::RttiVtable:
                results.push_back(ResolveRttiVtable(std::move(result), modules));
                break;
            case BindingKind::SchemaField:
                results.push_back(ResolveSchemaField(std::move(result), modules));
                break;
            case BindingKind::Slot:
                results.push_back(CheckCompiledIndex(
                    ResolveSlot(std::move(result), declaration.headerIndex, gamedata, modules, m_platform, m_options.heal, liveVtables), declaration.compiledIndex));
                break;
        }
    }

    // Heal needs a Vtable Check, so where there is none (Windows) there is no healing to turn off.
    return ResolutionReport(std::move(buildId), std::move(results), m_options.heal || m_platform == Platform::Windows);
}

ResolutionReport ResolveBindings(std::string buildId,
                                 const Gamedata& gamedata,
                                 const ModuleImages& modules,
                                 std::span<const BindingDeclaration> declarations)
{
    return BuiltinResolver(Platform::Linux).Resolve(std::move(buildId), gamedata, modules, declarations);
}

EngineFeatureStatus EvaluateEngineFeature(const ResolutionReport& report, const EngineFeatureDeclaration& feature)
{
    for (const auto& declaration : feature.bindings)
    {
        const auto* binding = report.Find(declaration.key);
        if (binding == nullptr || !binding->Resolved()) return { false, binding };
    }
    return { true, nullptr };
}

std::string DescribeFailure(const BindingResult& binding)
{
    switch (binding.reason)
    {
        case FailureReason::None:
            return binding.Resolved() ? "resolved" : "failed";
        case FailureReason::MissingFromGamedata:
            return "missing from gamedata";
        case FailureReason::MissingForThisOs:
            return "missing for this OS";
        case FailureReason::InvalidPattern:
            return "invalid pattern";
        case FailureReason::NotFound:
            return "not found";
        case FailureReason::Ambiguous:
            return "ambiguous (" + std::to_string(binding.matchCount) + " matches)";
        case FailureReason::RttiVtableNotFound:
            return "RTTI vtable not found";
        case FailureReason::SchemaClassMissing:
            return "schema class missing";
        case FailureReason::SchemaFieldMissing:
            return "schema field missing";
        case FailureReason::FingerprintMismatch:
            return "fingerprint mismatch";
        case FailureReason::AmbiguousHeal:
            return "ambiguous heal (" + std::to_string(binding.matchCount) + " matches)";
        case FailureReason::CompiledSlotMoved:
            return "compiled call's slot moved";
    }
    return "unknown";
}

std::string DescribeUnavailable(const ResolutionReport& report, const EngineFeatureDeclaration& feature)
{
    const std::string prefix = "Engine Feature " + feature.name;
    for (const auto& declaration : feature.bindings)
    {
        const auto* binding = report.Find(declaration.key);
        if (binding == nullptr) return prefix + " is disabled (" + declaration.key + ": missing from gamedata)";
        if (!binding->Resolved()) return prefix + " is disabled (" + binding->key + ": " + DescribeFailure(*binding) + ")";
    }
    return prefix + " is available";
}

std::string FormatBindingSummary(const ResolutionReport& report)
{
    const auto& bindings = report.Bindings();
    std::string failed;
    std::size_t failedCount = 0;
    std::size_t unverifiedCount = 0;
    std::size_t healedCount = 0;
    for (const auto& binding : bindings)
    {
        if (binding.status == BindingStatus::Unverified) ++unverifiedCount;
        if (binding.status == BindingStatus::Healed) ++healedCount;
        if (binding.Resolved()) continue;
        failed += (failedCount++ == 0 ? "" : ", ") + binding.key + " (" + DescribeFailure(binding) + ")";
    }

    std::string line = "Bindings for build " + report.BuildId() + ": ";
    if (failedCount == 0) line += "all " + std::to_string(bindings.size()) + " resolved";
    else
        line += std::to_string(failedCount) + " of " + std::to_string(bindings.size()) + " failed: " + failed;
    const std::string separator = failedCount == 0 ? ", " : "; ";
    if (healedCount > 0) line += separator + std::to_string(healedCount) + " healed";
    if (unverifiedCount > 0) line += separator + std::to_string(unverifiedCount) + " unverified";
    if (!report.HealEnabled()) line += separator + "healing off";
    return line;
}

std::vector<std::string> FormatHeals(const ResolutionReport& report)
{
    std::vector<std::string> lines;
    for (const auto& binding : report.Bindings())
    {
        if (binding.status != BindingStatus::Healed || !binding.slot) continue;
        const auto& slot = *binding.slot;
        std::string window;
        if (slot.window)
            for (const auto& fingerprint : slot.window->slots)
                window += (window.empty() ? "" : ", ") + fingerprint.value_or("null");
        lines.push_back("Healed " + binding.key + ": " + slot.className + " (" + binding.library + ") slot " + std::to_string(slot.statedIndex) +
                        " -> " + std::to_string(binding.offset) + ", window [" + window + "] matched once within " +
                        std::to_string(vtable::kSearchRange) + " slots");
    }
    return lines;
}

std::vector<std::string> FormatVtableSizeWarnings(const ResolutionReport& report)
{
    // One line per vtable, naming every Slot Binding in it.
    std::vector<std::tuple<std::string, std::string, std::size_t, std::size_t, std::string>> vtables;
    for (const auto& binding : report.Bindings())
    {
        if (!binding.slot || !binding.slot->SizeChanged()) continue;
        const auto& slot = *binding.slot;
        const auto same = std::find_if(vtables.begin(), vtables.end(), [&](const auto& vtable) {
            return std::get<0>(vtable) == slot.className && std::get<1>(vtable) == binding.library;
        });
        if (same != vtables.end()) std::get<4>(*same) += ", " + binding.key;
        else
            vtables.emplace_back(slot.className, binding.library, slot.vtableSize, *slot.recordedVtableSize, binding.key);
    }

    std::vector<std::string> lines;
    for (const auto& [className, library, size, recorded, keys] : vtables)
        lines.push_back("Vtable of " + className + " (" + library + ") has " + std::to_string(size) + " slots, Gamedata recorded " +
                        std::to_string(recorded) + " (" + keys + ")");
    return lines;
}

namespace {
const char* KindName(BindingKind kind)
{
    switch (kind)
    {
        case BindingKind::Signature:
            return "Signature";
        case BindingKind::Offset:
            return "Offset";
        case BindingKind::RttiVtable:
            return "RttiVtable";
        case BindingKind::SchemaField:
            return "SchemaField";
        case BindingKind::Slot:
            return "Slot";
    }
    return "Unknown";
}

const char* StatusName(BindingStatus status)
{
    switch (status)
    {
        case BindingStatus::Resolved:
            return "Resolved";
        case BindingStatus::Failed:
            return "Failed";
        case BindingStatus::Unverified:
            return "Unverified";
        case BindingStatus::Healed:
            return "Healed";
    }
    return "Unknown";
}

const char* MatchName(vtable::SlotMatch match)
{
    switch (match)
    {
        case vtable::SlotMatch::Window:
            return "Window";
        case vtable::SlotMatch::OwnFingerprint:
            return "OwnFingerprint";
        case vtable::SlotMatch::Heal:
            return "Heal";
        case vtable::SlotMatch::None:
            return "None";
    }
    return "Unknown";
}

const char* ReasonName(FailureReason reason)
{
    switch (reason)
    {
        case FailureReason::None:
            return "None";
        case FailureReason::MissingFromGamedata:
            return "MissingFromGamedata";
        case FailureReason::MissingForThisOs:
            return "MissingForThisOs";
        case FailureReason::InvalidPattern:
            return "InvalidPattern";
        case FailureReason::NotFound:
            return "NotFound";
        case FailureReason::Ambiguous:
            return "Ambiguous";
        case FailureReason::RttiVtableNotFound:
            return "RttiVtableNotFound";
        case FailureReason::SchemaClassMissing:
            return "SchemaClassMissing";
        case FailureReason::SchemaFieldMissing:
            return "SchemaFieldMissing";
        case FailureReason::FingerprintMismatch:
            return "FingerprintMismatch";
        case FailureReason::AmbiguousHeal:
            return "AmbiguousHeal";
        case FailureReason::CompiledSlotMoved:
            return "CompiledSlotMoved";
    }
    return "Unknown";
}
} // namespace

std::string ResolutionReportToJson(const ResolutionReport& report, std::span<const EngineFeatureDeclaration> nativeFeatures)
{
    auto bindings = nlohmann::json::array();
    for (const auto& binding : report.Bindings())
    {
        nlohmann::json entry{ { "key", binding.key },
                              { "kind", KindName(binding.kind) },
                              { "resolved", binding.Resolved() },
                              { "status", StatusName(binding.status) },
                              { "reason", ReasonName(binding.reason) },
                              { "matchCount", static_cast<std::uint64_t>(binding.matchCount) },
                              { "address", static_cast<std::uint64_t>(binding.address) },
                              { "offset", binding.offset },
                              { "library", binding.library } };
        if (binding.slot)
        {
            const auto& slot = *binding.slot;
            entry["slot"] = { { "class", slot.className },
                              { "statedIndex", slot.statedIndex },
                              { "match", MatchName(slot.match) },
                              { "vtableSize", static_cast<std::uint64_t>(slot.vtableSize) },
                              { "recordedVtableSize", slot.recordedVtableSize ? nlohmann::json(*slot.recordedVtableSize) : nlohmann::json(nullptr) },
                              { "window", nullptr } };
            if (slot.window)
            {
                auto& window = entry["slot"]["window"] = nlohmann::json::array();
                for (const auto& fingerprint : slot.window->slots)
                    window.push_back(fingerprint ? nlohmann::json(*fingerprint) : nlohmann::json(nullptr));
            }
        }
        bindings.push_back(std::move(entry));
    }

    auto features = nlohmann::json::array();
    for (const auto& feature : nativeFeatures)
    {
        auto keys = nlohmann::json::array();
        for (const auto& binding : feature.bindings)
            keys.push_back(binding.key);
        features.push_back({ { "name", feature.name }, { "bindings", std::move(keys) } });
    }

    return nlohmann::json{
        { "buildId", report.BuildId() },
        { "healEnabled", report.HealEnabled() },
        { "bindings", std::move(bindings) },
        { "features", std::move(features) }
    }.dump();
}

std::string ParseBuildId(std::string_view steamInf)
{
    auto value = [&](std::string_view name) -> std::string {
        std::size_t pos = 0;
        while (pos < steamInf.size())
        {
            std::size_t end = steamInf.find('\n', pos);
            if (end == std::string_view::npos) end = steamInf.size();
            std::string_view line = steamInf.substr(pos, end - pos);
            if (!line.empty() && line.back() == '\r') line.remove_suffix(1);
            if (line.size() > name.size() && line.substr(0, name.size()) == name && line[name.size()] == '=')
                return std::string(line.substr(name.size() + 1));
            pos = end + 1;
        }
        return {};
    };

    std::string patch = value("PatchVersion");
    const std::string server = value("ServerVersion");
    if (patch.empty()) return "unknown";
    return server.empty() ? patch : patch + "/" + server;
}

} // namespace counterstrikesharp::bindings
