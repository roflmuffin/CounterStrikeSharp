#include "core/core_bindings.h"

#include <algorithm>
#include <fstream>
#include <sstream>
#include <string>

#include "core/gameconfig.h"
#include "core/globals.h"
#include "core/log.h"
#include "core/memory_module.h"
#include "core/signature.h"
#include "core/utils.h"
#include "schema.h"

namespace counterstrikesharp::bindings {
namespace {

ResolutionReport coreReport;

ModuleImage ImageOf(modules::CModule* module)
{
    ModuleImage image;
    for (const auto& segment : module->GetSegments())
    {
        image.segments.push_back({ segment.address, segment.bytes.data(), segment.bytes.size() });
    }
    image.findSymbol = [module](std::string_view name) {
        return reinterpret_cast<std::uintptr_t>(module->FindSymbol(std::string(name)));
    };
    image.findVirtualTable = [module](std::string_view name) {
        return reinterpret_cast<std::uintptr_t>(module->FindVirtualTable(std::string(name)));
    };
    return image;
}

void AddModule(ModuleImages& images, const char* library, modules::CModule* module)
{
    if (module == nullptr) return;
    images.emplace(library, ImageOf(module));
}

// Schema fields live in the server module's schema type scope.
SchemaFieldLookup FindServerSchemaField(std::string_view className, std::string_view fieldName)
{
    const std::string classString(className);
    const std::string fieldString(fieldName);
    const auto lookup = schema::LookupField(classString.c_str(), hash_32_fnv1a_const(classString.c_str()), fieldString.c_str(),
                                            hash_32_fnv1a_const(fieldString.c_str()));
    switch (lookup.status)
    {
        case schema::LookupStatus::Found:
            return { SchemaFieldStatus::Found, lookup.key.offset };
        case schema::LookupStatus::FieldMissing:
            return { SchemaFieldStatus::FieldMissing };
        case schema::LookupStatus::ClassMissing:
            break;
    }
    return { SchemaFieldStatus::ClassMissing };
}

std::string ReadBuildId()
{
    std::ifstream stream(utils::GameDirectory() + "/steam.inf");
    if (!stream) return "unknown";
    std::stringstream contents;
    contents << stream.rdbuf();
    return ParseBuildId(contents.str());
}

} // namespace

void ResolveCoreBindings()
{
    // Same library names as CGameConfig::GetModule.
    ModuleImages images;
    AddModule(images, "engine", modules::engine);
    AddModule(images, "server", modules::server);
    AddModule(images, "vscript", modules::vscript);
    AddModule(images, "tier0", modules::tier0);
    if (const auto server = images.find("server"); server != images.end()) server->second.findSchemaField = FindServerSchemaField;

    // Every core Gamedata key, including those only managed code consumes (managed declares their Engine Features).
    const auto& gamedata = globals::gameConfig->GetGamedata();
    coreReport = ResolveBindings(ReadBuildId(), gamedata, images, CoreBindingDeclarations(gamedata));

    const auto summary = FormatBindingSummary(coreReport);
    const bool anyFailed = std::any_of(coreReport.Bindings().begin(), coreReport.Bindings().end(), [](const BindingResult& binding) {
        return !binding.Resolved();
    });
    if (anyFailed) CSSHARP_CORE_ERROR("{}", summary);
    else
        CSSHARP_CORE_INFO("{}", summary);
}

const ResolutionReport& CoreResolutionReport() { return coreReport; }

bool IsEngineFeatureAvailable(std::string_view feature)
{
    const auto* declaration = FindNativeEngineFeature(feature);
    return declaration != nullptr && EvaluateEngineFeature(coreReport, *declaration).available;
}

void* GetBindingAddress(std::string_view key)
{
    const auto* binding = coreReport.Find(key);
    if (binding == nullptr || !binding->Resolved()) return nullptr;
    return reinterpret_cast<void*>(binding->address);
}

std::optional<std::int64_t> GetBindingOffset(std::string_view key)
{
    const auto* binding = coreReport.Find(key);
    if (binding == nullptr || !binding->Resolved() || (binding->kind != BindingKind::Offset && binding->kind != BindingKind::SchemaField))
        return std::nullopt;
    return binding->offset;
}

SignatureScan ScanModuleSignature(const char* moduleName, std::string_view signature)
{
    auto* module = modules::GetModuleByName(moduleName);
    if (module == nullptr)
    {
        SignatureScan scan;
        scan.validPattern = !ParseSignature(signature).empty();
        return scan;
    }
    return ScanSignature(ImageOf(module), signature);
}

bool ShouldRegisterHooks(std::string_view feature, std::string_view what)
{
    const auto* declaration = FindNativeEngineFeature(feature);
    if (declaration == nullptr)
    {
        CSSHARP_CORE_ERROR("Engine Feature {} is not declared, not registering {}", feature, what);
        return false;
    }

    const auto status = EvaluateEngineFeature(coreReport, *declaration);
    if (status.available) return true;

    if (status.failedBinding != nullptr)
        CSSHARP_CORE_WARN("Engine Feature {} disabled ({}: {}), not registering {}", feature, status.failedBinding->key,
                          DescribeFailure(*status.failedBinding), what);
    else
        CSSHARP_CORE_WARN("Engine Feature {} disabled (Bindings not resolved), not registering {}", feature, what);
    return false;
}

} // namespace counterstrikesharp::bindings
