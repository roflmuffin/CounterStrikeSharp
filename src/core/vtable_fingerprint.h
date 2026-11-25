#pragma once

// Vtable slot fingerprints and fingerprint windows (ADR-0003). The one implementation shared by the boot-time Vtable
// Check (core/binding_resolver.cpp) and the offline Gamedata generator (tooling/gamedata-generator), so the two can
// never disagree. Pure: reads code and vtable slots through what the caller hands in.
//
// Fingerprint (FP24): decode the slot's target function from its first instruction, at most 24 instructions, stopping
// after the first return or unconditional jump (any form) or at a decode error. Mask every immediate field (relative
// branch and call targets included) and every memory displacement field (RIP-relative and base + displacement alike)
// by zeroing those bytes in place; keep prefixes, opcode, ModRM and SIB. Hash the stream [length][masked bytes]... with
// FNV-1a 64 and print it as 16 lowercase hex digits. Slots holding __cxa_pure_virtual or __cxa_deleted_virtual get the
// literal fingerprints "pure" and "deleted". The research oracle (Python, capstone) defines the same thing.

#include <array>
#include <cstddef>
#include <cstdint>
#include <functional>
#include <optional>
#include <span>
#include <string>
#include <string_view>
#include <vector>

namespace counterstrikesharp::vtable {

// Code as the process (or a loaded image) sees it: data[0] is at address.
struct CodeSegment
{
    std::uintptr_t address = 0;
    const std::uint8_t* data = nullptr;
    std::size_t size = 0;
};

inline constexpr std::size_t kMaxInstructions = 24;
// How far from its stated index a Slot Binding's fingerprint is compared (the uniqueness rule) or searched.
inline constexpr std::size_t kSearchRange = 16;
inline constexpr std::string_view kPureVirtual = "pure";
inline constexpr std::string_view kDeletedVirtual = "deleted";

using Fingerprint = std::string;

struct FunctionFingerprint
{
    Fingerprint fingerprint; // empty if the address is not in the code
    std::string canonical; // masked instructions as uppercase hex, separated by spaces (for diagnostics and oracle checks)
    std::size_t instructions = 0;
};

FunctionFingerprint FingerprintFunction(std::span<const CodeSegment> code, std::uintptr_t function);

// A class's vtable, read slot by slot.
struct VtableSource
{
    // The original target of slot index (even if someone hooked the slot since), or nullopt where readable memory ends.
    std::function<std::optional<std::uintptr_t>(std::size_t index)> slot;
    std::span<const CodeSegment> code;
    std::uintptr_t pureVirtual = 0; // address of __cxa_pure_virtual, 0 if unknown
    std::uintptr_t deletedVirtual = 0; // address of __cxa_deleted_virtual, 0 if unknown
};

// Fingerprints of every slot. The vtable ends at the first slot that is neither code nor a pure or deleted virtual, so
// the result's size is the vtable's size.
std::vector<Fingerprint> FingerprintVtable(const VtableSource& source);

// --- Fingerprint windows ---

// The fingerprints of slot index - 1, index and index + 1. A neighbour outside the vtable is nullopt and matches anything.
struct FingerprintWindow
{
    std::array<std::optional<Fingerprint>, 3> slots;

    friend bool operator==(const FingerprintWindow&, const FingerprintWindow&) = default;
};

FingerprintWindow WindowAt(std::span<const Fingerprint> vtable, std::size_t index);

// Every recorded fingerprint of the window equals the slot at the same position around index (which must be in vtable).
bool WindowMatchesAt(const FingerprintWindow& window, std::span<const Fingerprint> vtable, std::size_t index);

// The slot's fingerprint occurs exactly once within kSearchRange slots on either side (itself included).
bool UniqueNearby(std::span<const Fingerprint> vtable, std::size_t index);

// What Gamedata records for a Slot Binding on the build it was generated from.
struct SlotRecord
{
    std::size_t index = 0;
    FingerprintWindow window;
    bool uniqueNearby = false; // UniqueNearby on that build
    std::optional<std::size_t> vtableSize; // size of the class's vtable on that build
};

// The record for slot index of a vtable (the generator's bootstrap). index must be in vtable.
SlotRecord RecordSlot(std::span<const Fingerprint> vtable, std::size_t index);

// --- Vtable Check ---

enum class SlotMatch : std::uint8_t
{
    Window, // the whole window matches at the recorded index
    OwnFingerprint, // only the slot itself matches there, and it is unique nearby on both builds (a neighbour changed)
    Heal, // the whole window matches exactly once elsewhere within kSearchRange: the slot moved there
    None,
};

// Checks a recorded Slot Binding against a live vtable at its recorded index (Window, OwnFingerprint or None).
SlotMatch CheckSlot(const SlotRecord& recorded, std::span<const Fingerprint> live);

// Positions within kSearchRange of index, index itself excluded, where the whole window matches the live vtable.
std::vector<std::size_t> WindowMatchesNear(const FingerprintWindow& window, std::span<const Fingerprint> live, std::size_t index);

// Where a recorded Slot Binding is in a live vtable.
struct SlotLocation
{
    SlotMatch match = SlotMatch::None;
    std::size_t index = 0; // the live slot index; the recorded one unless healed (match != None only)
    std::size_t windowMatches = 0; // None: positions nearby where the whole window matched (2 or more: ambiguous)
};

// The Vtable Check with Heal: CheckSlot at the recorded index, else the window's one match within kSearchRange. A slot
// whose recorded neighbour has the same fingerprint (identical bodies) is never healed: which copy moved is unknowable.
SlotLocation LocateSlot(const SlotRecord& recorded, std::span<const Fingerprint> live);

} // namespace counterstrikesharp::vtable
