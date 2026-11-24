#include "core/binding_resolver.h"

#include <algorithm>
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
    const std::uintptr_t address =
        module != modules.end() && module->second.findVirtualTable ? module->second.findVirtualTable(result.key) : 0;
    if (address == 0) return Failed(std::move(result), FailureReason::RttiVtableNotFound);
    return ResolvedAt(std::move(result), address);
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
        }
        gamedata.emplace(key, std::move(entry));
    }
    return gamedata;
}

ResolutionReport::ResolutionReport(std::string buildId, std::vector<BindingResult> bindings)
    : m_buildId(std::move(buildId)), m_bindings(std::move(bindings))
{
}

const BindingResult* ResolutionReport::Find(std::string_view key) const
{
    const auto it = std::find_if(m_bindings.begin(), m_bindings.end(), [&](const BindingResult& binding) {
        return binding.key == key;
    });
    return it == m_bindings.end() ? nullptr : &*it;
}

ResolutionReport ResolveBindings(std::string buildId,
                                 const Gamedata& gamedata,
                                 const ModuleImages& modules,
                                 std::span<const BindingDeclaration> declarations)
{
    std::vector<BindingResult> results;
    results.reserve(declarations.size());

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
        }
    }

    return ResolutionReport(std::move(buildId), std::move(results));
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
    }
    return "unknown";
}

std::string FormatBindingSummary(const ResolutionReport& report)
{
    const auto& bindings = report.Bindings();
    std::string failed;
    std::size_t failedCount = 0;
    for (const auto& binding : bindings)
    {
        if (binding.Resolved()) continue;
        failed += (failedCount++ == 0 ? "" : ", ") + binding.key + " (" + DescribeFailure(binding) + ")";
    }

    std::string line = "Bindings for build " + report.BuildId() + ": ";
    if (failedCount == 0) return line + "all " + std::to_string(bindings.size()) + " resolved";
    return line + std::to_string(failedCount) + " of " + std::to_string(bindings.size()) + " failed: " + failed;
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
