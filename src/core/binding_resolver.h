#pragma once

// Binding resolver: resolves declared Bindings against parsed Gamedata and the loaded module images into an immutable
// Resolution Report (see GLOSSARY.md, ADR-0002). Resolution sits behind one seam (ADR-0004): IBindingResolver, whose
// only output is the report. BuiltinResolver is the built-in adapter: signature scanning, Gamedata offsets, RTTI vtables,
// schema fields and the Vtable Check of Slot Bindings (ADR-0003). Pure: no SDK, Metamod or globals, so tests/native can
// drive it with synthetic images. core/core_bindings.h feeds it the real modules at boot.

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

#include "core/rtti_vtable.h"
#include "core/schema_field_status.h"
#include "core/vtable_fingerprint.h"

namespace counterstrikesharp::bindings {

// --- Inputs ---

// The Slot Binding fields of a Gamedata key (additive; "offsets.<os>" keeps the slot index):
//   "vtable": { "class": "CSource2Server", "library": "server",
//               "linux": { "window": ["<fp index-1>" | null, "<fp index>", "<fp index+1>" | null],
//                          "uniqueNearby": true, "size": 105 } }
// class: the RTTI class whose primary vtable holds the slot; library: its module, named as for signatures. The window,
// uniqueNearby and size were recorded by the Gamedata generator on one build (see core/vtable_fingerprint.h); null
// marks a neighbour outside the vtable.
struct SlotGamedata
{
    std::optional<std::string> className; // "vtable.class"
    std::optional<std::string> library; // "vtable.library"
    std::optional<vtable::FingerprintWindow> window; // "vtable.<os>.window"
    bool uniqueNearby = false; // "vtable.<os>.uniqueNearby"
    std::optional<std::size_t> vtableSize; // "vtable.<os>.size"
};

// One Gamedata key as seen by this OS.
struct GamedataEntry
{
    bool hasSignatures = false;
    std::optional<std::string> library; // "signatures.library"
    std::optional<std::string> signature; // "signatures.<os>"
    bool hasOffsets = false;
    std::optional<std::int64_t> offset; // "offsets.<os>"
    std::optional<SlotGamedata> slot; // "vtable", if present
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
    Slot, // Slot Binding: the Gamedata "offsets" slot index, verified by a Vtable Check against Gamedata "vtable"
};

enum class Platform : std::uint8_t
{
    Linux, // Slot Bindings are verified (Vtable Check)
    Windows, // Slot Bindings are unverified: used at their Gamedata (header-derived) index
};

struct BindingDeclaration
{
    std::string key; // Gamedata key, or the RTTI class name for RttiVtable
    BindingKind kind = BindingKind::Signature;
    std::string library{}; // RttiVtable, SchemaField: Gamedata-style library name to search ("server", "engine", ...)
    // Slot: the caller has a compiled header index to fall back on (interface methods from SDK headers), so on Windows a
    // Slot Binding without a Gamedata index is unverified at it (offset -1). Game-class virtuals have none: there it
    // fails as missing for this OS.
    bool headerIndex = true;
    // Slot only: the slot index a compiled (header) call on this Binding uses. A compiled call cannot follow a Heal, so a
    // Binding verified at any other index fails with CompiledSlotMoved. Unverified Bindings are not compared.
    std::optional<std::int64_t> compiledIndex{};
};

// Executable code: where data[0] lives in the process, and its bytes (on-disk bytes where available).
using ModuleSegment = vtable::CodeSegment;

struct SchemaFieldLookup
{
    SchemaFieldStatus status = SchemaFieldStatus::ClassMissing;
    std::int64_t offset = 0; // Found only
};

struct ModuleImage
{
    std::vector<ModuleSegment> segments; // scanned for signatures (on-disk bytes where available)
    std::function<std::uintptr_t(std::string_view)> findSymbol; // optional, for "@symbol" signatures
    std::function<rtti::VtableLookup(std::string_view)> findVirtualTable; // optional, for RttiVtable
    std::function<SchemaFieldLookup(std::string_view className, std::string_view fieldName)> findSchemaField; // optional, for SchemaField
    // For Slot: the original target of a vtable's slot (as loaded, even if a hook replaced the entry since), nullopt
    // where the vtable's memory ends. Code is read from segments, so others' inline patches do not change fingerprints.
    std::function<std::optional<std::uintptr_t>(std::uintptr_t vtable, std::size_t index)> readVtableSlot;
    std::uintptr_t pureVirtual = 0; // address of __cxa_pure_virtual, 0 if unknown
    std::uintptr_t deletedVirtual = 0; // address of __cxa_deleted_virtual, 0 if unknown
};

// Keyed by the Gamedata "library" name.
using ModuleImages = std::map<std::string, ModuleImage, std::less<>>;

// --- Output: the Resolution Report ---

enum class BindingStatus : std::uint8_t
{
    Resolved,
    Failed,
    Unverified, // Slot Binding used at its Gamedata index without a Vtable Check (Windows, or no window in Gamedata)
    Healed, // Slot Binding found at another index than Gamedata states (a Heal); see SlotEvidence
};

// Closed set. Append only: values cross the native/managed boundary.
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
    FingerprintMismatch, // Slot Binding: the fingerprint window does not match the live vtable
    AmbiguousHeal, // Slot Binding: the window does not match at its index and matches more than once nearby; see matchCount
    CompiledSlotMoved, // Slot Binding of a compiled call: verified (or healed) at another index than the compiled one
};

// A Slot Binding's Vtable Check, as evidence for the report.
struct SlotEvidence
{
    std::string className;
    std::int64_t statedIndex = -1; // the Gamedata index; -1 if Gamedata has none for this OS
    vtable::SlotMatch match = vtable::SlotMatch::None; // how the window matched at the resolved index (Heal: elsewhere)
    std::optional<vtable::FingerprintWindow> window; // the window Gamedata recorded; a Heal matched it at the new index
    std::size_t vtableSize = 0; // live; 0 if not checked
    std::optional<std::size_t> recordedVtableSize; // as Gamedata recorded it

    // A warning, never a failure by itself: a Game update touched the class even if our slot did not move.
    [[nodiscard]] bool SizeChanged() const { return vtableSize != 0 && recordedVtableSize && *recordedVtableSize != vtableSize; }
};

struct BindingResult
{
    std::string key;
    BindingKind kind = BindingKind::Signature;
    BindingStatus status = BindingStatus::Failed;
    FailureReason reason = FailureReason::None;
    // Signatures: matches across all scanned segments; RttiVtable, Slot: primary vtables of the class; Slot failed with
    // AmbiguousHeal: positions where the window matched
    std::size_t matchCount = 0;
    std::uintptr_t address = 0; // Signature, RttiVtable
    // Offset, SchemaField; Slot: the slot index to use, the new one if healed (-1 on Windows without a Gamedata index)
    std::int64_t offset = 0;
    std::string library{}; // module searched, if any
    std::optional<SlotEvidence> slot{}; // Slot only

    // True when the Binding can be used: resolved, or (Slot Bindings) healed or unverified.
    [[nodiscard]] bool Resolved() const { return status != BindingStatus::Failed; }
};

class ResolutionReport
{
  public:
    ResolutionReport() = default;
    ResolutionReport(std::string buildId, std::vector<BindingResult> bindings, bool healEnabled = true);

    [[nodiscard]] const std::string& BuildId() const { return m_buildId; }
    // False when the operator turned Heal off: Slot Bindings were only checked at their Gamedata index. Always true on
    // Windows, which has no Vtable Check and so no Heal to turn off.
    [[nodiscard]] bool HealEnabled() const { return m_healEnabled; }
    [[nodiscard]] const std::vector<BindingResult>& Bindings() const { return m_bindings; }
    // By key, through an index built with the report (no scan): native code reads Bindings by key on hot paths.
    [[nodiscard]] const BindingResult* Find(std::string_view key) const;

  private:
    std::string m_buildId;
    std::vector<BindingResult> m_bindings;
    bool m_healEnabled = true;
    std::map<std::string, std::size_t, std::less<>> m_index; // key -> position in m_bindings
};

// The slot index to use for a usable Slot Binding (resolved, healed or unverified); nullopt if it failed, is not a Slot
// Binding, is not in the report, or (Windows) has no Gamedata index, where the caller's compiled header index applies.
std::optional<int> SlotIndexOf(const ResolutionReport& report, std::string_view key);

// The resolver seam (ADR-0004): Gamedata and the loaded modules in, the Resolution Report out. Every Binding is resolved
// through one of these; no other code locates Bindings in binaries. An external resolver plugs in as another adapter.
class IBindingResolver
{
  public:
    virtual ~IBindingResolver() = default;

    // Resolves every declaration, in order, into one report.
    [[nodiscard]] virtual ResolutionReport Resolve(std::string buildId,
                                                   const Gamedata& gamedata,
                                                   const ModuleImages& modules,
                                                   std::span<const BindingDeclaration> declarations) const = 0;
};

// The built-in adapter. Signatures must match exactly once across all segments of their module. A Slot Binding on Linux
// resolves at its Gamedata index when its fingerprint window matches the live vtable of its class there (or only its
// own fingerprint does, unique within the search range on both builds); otherwise it heals at the one index within the
// search range where the whole window matches, and fails with FingerprintMismatch (no match, or identical neighbouring
// bodies) or AmbiguousHeal (several). A size change of the vtable alone is a warning. On Windows, or without a window
// in Gamedata, it is unverified. A Slot Binding declared with a compiledIndex that resolves or heals at another index
// fails with CompiledSlotMoved (its evidence kept). Healed indices live only in the report; Gamedata is never written.
// With Heal off (the operator's HealEnabled: false), a window that does not match at the Gamedata index fails with
// FingerprintMismatch; nothing is searched. No effect on Windows, which has no Vtable Check.
struct ResolverOptions
{
    bool heal = true;
};

class BuiltinResolver final : public IBindingResolver
{
  public:
    explicit BuiltinResolver(Platform platform, ResolverOptions options = {}) : m_platform(platform), m_options(options) {}

    [[nodiscard]] ResolutionReport Resolve(std::string buildId,
                                           const Gamedata& gamedata,
                                           const ModuleImages& modules,
                                           std::span<const BindingDeclaration> declarations) const override;

  private:
    Platform m_platform;
    ResolverOptions m_options;
};

// Shorthand for BuiltinResolver(Platform::Linux).Resolve(...).
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
// "Bindings for build 1.42.0.1/2000950: all 12 resolved, 1 unverified"
// "Bindings for build 1.42.0.1/2000950: all 12 resolved, 1 healed" (healed Bindings count as resolved)
// "Bindings for build 1.42.0.1/2000950: 1 of 12 failed: ISource2Server::GameFrame (fingerprint mismatch); healing off"
std::string FormatBindingSummary(const ResolutionReport& report);

// One line per healed Slot Binding, with its evidence (managed's startup report uses the same wording), e.g.
// "Healed ISource2Server::GameFrame: CSource2Server (server) slot 19 -> 20, window [<fp>, <fp>, <fp>] matched once within
//  16 slots"; a neighbour outside the vtable prints as null.
std::vector<std::string> FormatHeals(const ResolutionReport& report);

// One line per Slot Binding whose class's vtable size differs from what Gamedata recorded, e.g.
// "Vtable of CSource2Server (server) has 105 slots, Gamedata recorded 104 (ISource2Server::GameFrame)"
std::vector<std::string> FormatVtableSizeWarnings(const ResolutionReport& report);

// --- Native -> managed boundary ---

// The report as one JSON document for managed (the GET_RESOLUTION_REPORT native):
// { "buildId": "...", "healEnabled": true|false,
//   "bindings": [ { "key", "kind": "Signature"|"Offset"|"RttiVtable"|"SchemaField"|"Slot", "resolved",
//                   "status": "Resolved"|"Failed"|"Unverified"|"Healed", "reason": <FailureReason name>,
//                   "matchCount", "address", "offset", "library",
//                   "slot": { "class", "statedIndex", "match": "Window"|"OwnFingerprint"|"Heal"|"None", "vtableSize",
//                             "recordedVtableSize": <n>|null,
//                             "window": [<fp>|null, <fp>, <fp>|null]|null }   <- Slot only } ],
// A Heal's evidence: statedIndex is the old index, offset the new one, window the recorded window that matched there.
//   "features": [ { "name", "bindings": [ "<key>", ... ] } ] }   <- the native-owned Engine Features
// Enum values travel by name (the C++ enumerator names), so managed never depends on their numeric values.
std::string ResolutionReportToJson(const ResolutionReport& report, std::span<const EngineFeatureDeclaration> nativeFeatures);

// Build id from the game's steam.inf ("PatchVersion/ServerVersion"), or "unknown".
std::string ParseBuildId(std::string_view steamInf);

} // namespace counterstrikesharp::bindings
