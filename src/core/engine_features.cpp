#include "core/engine_features.h"

#include <algorithm>
#include <string>

namespace counterstrikesharp::bindings {
namespace {

BindingDeclaration Signature(std::string key) { return { std::move(key), BindingKind::Signature }; }
BindingDeclaration Offset(std::string key) { return { std::move(key), BindingKind::Offset }; }
[[maybe_unused]] BindingDeclaration RttiVtable(std::string className, std::string library)
{
    return { std::move(className), BindingKind::RttiVtable, std::move(library) };
}
// A schema field native core code reads directly; key "Class::field", looked up in the server schema scope.
[[maybe_unused]] BindingDeclaration SchemaField(const std::string& className, const std::string& fieldName)
{
    return { className + "::" + fieldName, BindingKind::SchemaField, "server" };
}

// The declaration table. One row per Engine Feature: its stable name and the Bindings (Gamedata keys) it needs.
// Additive only. A Binding may appear in several rows; it is resolved once.
const std::vector<EngineFeatureDeclaration> kEngineFeatures{
    { std::string(features::ChatSayHooks), { Signature("Host_Say") } },
    // ticket 08
    { std::string(features::EntityOutputHooks), { Signature("CEntityIOOutput_FireOutputInternal") } },
    { std::string(features::EntityAcceptInput), { Signature("CEntityInstance_AcceptInput") } },
    { std::string(features::EntityAddIoEvent), { Signature("CEntitySystem_AddEntityIOEvent") } },
    { std::string(features::EntityEmitSound), { Signature("CBaseEntity_EmitSoundFilter") } },
    { std::string(features::EntityDispatchSpawn), { Signature("CBaseEntity_DispatchSpawn") } },
    // ticket 11
    { std::string(features::EntityTakeDamageHooks), { Signature("CBaseEntity_TakeDamageOld"), Offset("CBaseEntity_IsPlayerPawn") } },
};

bool Declared(const std::vector<BindingDeclaration>& bindings, std::string_view key)
{
    return std::any_of(bindings.begin(), bindings.end(), [&](const BindingDeclaration& b) {
        return b.key == key;
    });
}

} // namespace

std::span<const EngineFeatureDeclaration> NativeEngineFeatures() { return kEngineFeatures; }

std::vector<BindingDeclaration> CoreBindingDeclarations(const Gamedata& gamedata)
{
    std::vector<BindingDeclaration> bindings;
    for (const auto& feature : kEngineFeatures)
        for (const auto& binding : feature.bindings)
            if (!Declared(bindings, binding.key)) bindings.push_back(binding);

    for (const auto& [key, entry] : gamedata)
    {
        if (Declared(bindings, key)) continue;
        if (entry.hasSignatures) bindings.push_back(Signature(key));
        else if (entry.hasOffsets)
            bindings.push_back(Offset(key));
    }
    return bindings;
}

const EngineFeatureDeclaration* FindNativeEngineFeature(std::string_view name)
{
    const auto it = std::find_if(kEngineFeatures.begin(), kEngineFeatures.end(), [&](const EngineFeatureDeclaration& feature) {
        return feature.name == name;
    });
    return it == kEngineFeatures.end() ? nullptr : &*it;
}

} // namespace counterstrikesharp::bindings
