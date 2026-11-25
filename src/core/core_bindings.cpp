#include "core/core_bindings.h"

#include <algorithm>
#include <fstream>
#include <memory>
#include <sstream>
#include <string>

#ifndef _WIN32
#include <dlfcn.h>
#endif

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

#ifdef _WIN32
constexpr Platform kPlatform = Platform::Windows;
#else
constexpr Platform kPlatform = Platform::Linux;
#endif

// The resolver seam (ADR-0004): the only place core Bindings are located. Swap the adapter here.
const IBindingResolver& Resolver()
{
    static const std::unique_ptr<IBindingResolver> resolver = std::make_unique<BuiltinResolver>(kPlatform);
    return *resolver;
}

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
        return module->LookupVirtualTable(std::string(name));
    };
#ifndef _WIN32
    // A slot as the loader relocated it, not as it is now: Metamod patches ISource2Server::Init in place while plugins
    // load, and hooks (ours, other plugins') replace the slots they hook.
    image.readVtableSlot = [module](std::uintptr_t vtable, std::size_t index) {
        return module->Relocations().At(vtable + index * sizeof(void*));
    };
    image.pureVirtual = reinterpret_cast<std::uintptr_t>(dlsym(RTLD_DEFAULT, "__cxa_pure_virtual"));
    image.deletedVirtual = reinterpret_cast<std::uintptr_t>(dlsym(RTLD_DEFAULT, "__cxa_deleted_virtual"));
#endif
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
    if (lookup.status != SchemaFieldStatus::Found) return { lookup.status };
    return { SchemaFieldStatus::Found, lookup.key.offset };
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
    const auto declarations = CoreBindingDeclarations(gamedata);
    coreReport = Resolver().Resolve(ReadBuildId(), gamedata, images, declarations);

    const auto summary = FormatBindingSummary(coreReport);
    const bool anyFailed = std::any_of(coreReport.Bindings().begin(), coreReport.Bindings().end(), [](const BindingResult& binding) {
        return !binding.Resolved();
    });
    // Same severity as the managed startup report: a failed Binding disables Engine Features, CounterStrikeSharp keeps running.
    if (anyFailed) CSSHARP_CORE_WARN("{}", summary);
    else
        CSSHARP_CORE_INFO("{}", summary);
    for (const auto& warning : FormatVtableSizeWarnings(coreReport))
        CSSHARP_CORE_WARN("{}", warning);
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

std::optional<int> GetBindingSlot(std::string_view key)
{
    const auto* binding = coreReport.Find(key);
    if (binding == nullptr || !binding->Resolved() || binding->kind != BindingKind::Slot || binding->offset < 0) return std::nullopt;
    return static_cast<int>(binding->offset);
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

    if (EvaluateEngineFeature(coreReport, *declaration).available) return true;

    // Not a report: the failed Binding is already in the boot summary line and the startup report.
    CSSHARP_CORE_DEBUG("{}, not registering {}", DescribeUnavailable(coreReport, *declaration), what);
    return false;
}

std::string DescribeUnavailable(std::string_view feature)
{
    const auto* declaration = FindNativeEngineFeature(feature);
    if (declaration == nullptr) return "Engine Feature " + std::string(feature) + " is not declared";
    return bindings::DescribeUnavailable(coreReport, *declaration);
}

} // namespace counterstrikesharp::bindings
