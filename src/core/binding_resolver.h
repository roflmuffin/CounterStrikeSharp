#pragma once

// Binding resolver: resolves declared Bindings against parsed Gamedata and the loaded module images into an immutable
// Resolution Report (see GLOSSARY.md, ADR-0002). Pure: no SDK, Metamod or globals, so tests/native can drive it with
// synthetic images. core/core_bindings.h is the adapter that feeds it the real modules at boot.

#include <cstddef>
#include <cstdint>
#include <functional>
#include <map>
#include <optional>
#include <span>
#include <string>
#include <string_view>
#include <vector>

// metamod_oslink.h maps snprintf to _snprintf on Windows, which breaks nlohmann/json (same as gameconfig.h).
#undef snprintf
#include <nlohmann/json.hpp>

#include "core/schema_field_status.h"

namespace counterstrikesharp::bindings {

// --- Inputs ---

// One Gamedata key as seen by this OS.
struct GamedataEntry
{
    bool hasSignatures = false;
    std::optional<std::string> library; // "signatures.library"
    std::optional<std::string> signature; // "signatures.<os>"
    bool hasOffsets = false;
    std::optional<std::int64_t> offset; // "offsets.<os>"
};

using Gamedata = std::map<std::string, GamedataEntry, std::less<>>;

// Reads the Gamedata json for one platform ("linux" or "windows"). Never throws on unexpected shapes; anything it
// cannot read is simply absent, and the resolver reports it.
Gamedata ParseGamedata(const nlohmann::json& root, std::string_view platform);

enum class BindingKind : std::uint8_t
{
    Signature, // Gamedata "signatures" (hex pattern, or "@symbol"), scanned in the module named by "library"
    Offset, // Gamedata "offsets"
    RttiVtable, // vtable found by RTTI class name (key) in the declaration's library; not in Gamedata
    SchemaField, // schema field "Class::field" (key) in the declaration's library's schema scope; not in Gamedata
};

struct BindingDeclaration
{
    std::string key; // Gamedata key, or the RTTI class name for RttiVtable
    BindingKind kind = BindingKind::Signature;
    std::string library{}; // RttiVtable, SchemaField: Gamedata-style library name to search ("server", "engine", ...)
};

struct ModuleSegment
{
    std::uintptr_t address = 0; // where data[0] lives in the process
    const std::uint8_t* data = nullptr;
    std::size_t size = 0;
};

struct SchemaFieldLookup
{
    SchemaFieldStatus status = SchemaFieldStatus::ClassMissing;
    std::int64_t offset = 0; // Found only
};

struct ModuleImage
{
    std::vector<ModuleSegment> segments; // scanned for signatures (on-disk bytes where available)
    std::function<std::uintptr_t(std::string_view)> findSymbol; // optional, for "@symbol" signatures
    std::function<std::uintptr_t(std::string_view)> findVirtualTable; // optional, for RttiVtable
    std::function<SchemaFieldLookup(std::string_view className, std::string_view fieldName)> findSchemaField; // optional, for SchemaField
};

// Keyed by the Gamedata "library" name.
using ModuleImages = std::map<std::string, ModuleImage, std::less<>>;

// --- Output: the Resolution Report ---

enum class BindingStatus : std::uint8_t
{
    Resolved,
    Failed,
};

// Closed set (spec A). Append only: values cross the native/managed boundary.
enum class FailureReason : std::uint8_t
{
    None,
    MissingFromGamedata, // key (or its section/library) absent
    MissingForThisOs, // key present, no entry for this OS
    InvalidPattern, // signature is not valid hex
    NotFound, // zero matches (or module not loaded)
    Ambiguous, // more than one match; see matchCount
    RttiVtableNotFound,
    SchemaClassMissing,
    SchemaFieldMissing,
};

struct BindingResult
{
    std::string key;
    BindingKind kind = BindingKind::Signature;
    BindingStatus status = BindingStatus::Failed;
    FailureReason reason = FailureReason::None;
    std::size_t matchCount = 0; // signatures: matches across all scanned segments
    std::uintptr_t address = 0; // Signature, RttiVtable
    std::int64_t offset = 0; // Offset, SchemaField
    std::string library{}; // module searched, if any

    [[nodiscard]] bool Resolved() const { return status == BindingStatus::Resolved; }
};

class ResolutionReport
{
  public:
    ResolutionReport() = default;
    ResolutionReport(std::string buildId, std::vector<BindingResult> bindings);

    [[nodiscard]] const std::string& BuildId() const { return m_buildId; }
    [[nodiscard]] const std::vector<BindingResult>& Bindings() const { return m_bindings; }
    [[nodiscard]] const BindingResult* Find(std::string_view key) const;

  private:
    std::string m_buildId;
    std::vector<BindingResult> m_bindings;
};

// Resolves every declaration. Signatures must match exactly once across all segments of their module.
ResolutionReport ResolveBindings(std::string buildId,
                                 const Gamedata& gamedata,
                                 const ModuleImages& modules,
                                 std::span<const BindingDeclaration> declarations);

// --- Raw (plugin-supplied) signatures ---

struct SignatureScan
{
    bool validPattern = false;
    std::size_t matchCount = 0; // matches across all segments
    std::uintptr_t firstMatch = 0; // 0 if none
};

// Counts every match of a raw signature (space-separated hex or code style) across all segments of a module, the same
// way core signatures are counted. Unlike core Bindings, raw signatures need not be unique: the caller decides.
SignatureScan ScanSignature(const ModuleImage& image, std::string_view signature);

// --- Engine Features ---

struct EngineFeatureDeclaration
{
    std::string name; // stable, namespaced, never renamed or reused (e.g. "cssharp.chat/say-hooks")
    std::vector<BindingDeclaration> bindings;
};

struct EngineFeatureStatus
{
    bool available = false;
    const BindingResult* failedBinding = nullptr; // first failed Binding; nullptr if available or not in the report
};

// Available exactly when every Binding of the feature resolved in the report.
EngineFeatureStatus EvaluateEngineFeature(const ResolutionReport& report, const EngineFeatureDeclaration& feature);

// --- Text ---

// "not found", "ambiguous (3 matches)", "missing for this OS", ...
std::string DescribeFailure(const BindingResult& binding);

// For natives called while their Engine Feature is disabled; same wording as the managed typed exception, e.g.
// "Engine Feature cssharp.chat/say-hooks is disabled (Host_Say: not found)". A Binding absent from the report counts
// as missing from Gamedata.
std::string DescribeUnavailable(const ResolutionReport& report, const EngineFeatureDeclaration& feature);

// One compact line for the early native log, e.g.
// "Bindings for build 1.42.0.1/2000950: 1 of 12 failed: Host_Say (not found)"
// "Bindings for build 1.42.0.1/2000950: all 12 resolved"
std::string FormatBindingSummary(const ResolutionReport& report);

// --- Native -> managed boundary ---

// The report as one JSON document for managed (the GET_RESOLUTION_REPORT native):
// { "buildId": "...",
//   "bindings": [ { "key", "kind": "Signature"|"Offset"|"RttiVtable"|"SchemaField", "resolved", "reason": <FailureReason name>,
//                   "matchCount", "address", "offset", "library" } ],
//   "features": [ { "name", "bindings": [ "<key>", ... ] } ] }   <- the native-owned Engine Features
// Enum values travel by name (the C++ enumerator names), so managed never depends on their numeric values.
std::string ResolutionReportToJson(const ResolutionReport& report, std::span<const EngineFeatureDeclaration> nativeFeatures);

// Build id from the game's steam.inf ("PatchVersion/ServerVersion"), or "unknown".
std::string ParseBuildId(std::string_view steamInf);

} // namespace counterstrikesharp::bindings
