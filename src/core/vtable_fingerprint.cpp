#include "core/vtable_fingerprint.h"

#include <algorithm>
#include <cstdio>

#include <Zydis.h>

namespace counterstrikesharp::vtable {
namespace {

constexpr std::uint64_t kFnvOffset = 0xcbf29ce484222325ULL;
constexpr std::uint64_t kFnvPrime = 0x100000001b3ULL;
// Far more slots than any real vtable has; only stops a scan that never meets a non-code word.
constexpr std::size_t kMaxVtableSlots = 8192;

std::uint64_t Fnv1a(std::uint64_t hash, std::uint8_t byte) { return (hash ^ byte) * kFnvPrime; }

const CodeSegment* SegmentOf(std::span<const CodeSegment> code, std::uintptr_t address)
{
    for (const auto& segment : code)
        if (segment.data != nullptr && address >= segment.address && address - segment.address < segment.size) return &segment;
    return nullptr;
}

void ZeroField(std::uint8_t* bytes, std::size_t length, std::uint8_t offset, std::uint8_t sizeInBits)
{
    const std::size_t size = sizeInBits / 8;
    if (size == 0 || offset + size > length) return;
    std::fill_n(bytes + offset, size, std::uint8_t{ 0 });
}

} // namespace

FunctionFingerprint FingerprintFunction(std::span<const CodeSegment> code, std::uintptr_t function)
{
    FunctionFingerprint result;
    const auto* segment = SegmentOf(code, function);
    if (segment == nullptr) return result;

    ZydisDecoder decoder;
    ZydisDecoderInit(&decoder, ZYDIS_MACHINE_MODE_LONG_64, ZYDIS_STACK_WIDTH_64);

    std::uint64_t hash = kFnvOffset;
    std::size_t offset = function - segment->address;
    while (result.instructions < kMaxInstructions && offset < segment->size)
    {
        ZydisDecodedInstruction instruction;
        if (!ZYAN_SUCCESS(ZydisDecoderDecodeInstruction(&decoder, nullptr, segment->data + offset, segment->size - offset, &instruction)))
            break;

        std::uint8_t bytes[ZYDIS_MAX_INSTRUCTION_LENGTH];
        std::copy_n(segment->data + offset, instruction.length, bytes);
        for (const auto& immediate : instruction.raw.imm)
            ZeroField(bytes, instruction.length, immediate.offset, immediate.size);
        ZeroField(bytes, instruction.length, instruction.raw.disp.offset, instruction.raw.disp.size);

        hash = Fnv1a(hash, instruction.length);
        if (result.instructions > 0) result.canonical += ' ';
        for (std::size_t i = 0; i < instruction.length; ++i)
        {
            hash = Fnv1a(hash, bytes[i]);
            char text[3];
            std::snprintf(text, sizeof(text), "%02X", bytes[i]);
            result.canonical += text;
        }

        ++result.instructions;
        offset += instruction.length;
        if (instruction.meta.category == ZYDIS_CATEGORY_RET || instruction.mnemonic == ZYDIS_MNEMONIC_JMP) break;
    }

    char text[17];
    std::snprintf(text, sizeof(text), "%016llx", static_cast<unsigned long long>(hash));
    result.fingerprint = text;
    return result;
}

std::vector<Fingerprint> FingerprintVtable(const VtableSource& source)
{
    std::vector<Fingerprint> fingerprints;
    for (std::size_t index = 0; index < kMaxVtableSlots; ++index)
    {
        const auto target = source.slot(index);
        if (!target) break;
        if (*target != 0 && *target == source.pureVirtual) fingerprints.emplace_back(kPureVirtual);
        else if (*target != 0 && *target == source.deletedVirtual)
            fingerprints.emplace_back(kDeletedVirtual);
        else if (SegmentOf(source.code, *target) != nullptr)
            fingerprints.push_back(FingerprintFunction(source.code, *target).fingerprint);
        else
            break;
    }
    return fingerprints;
}

FingerprintWindow WindowAt(std::span<const Fingerprint> vtable, std::size_t index)
{
    FingerprintWindow window;
    for (std::size_t position = 0; position < 3; ++position)
    {
        const std::size_t slot = index + position; // index - 1 + position, shifted to stay unsigned
        if (slot >= 1 && slot - 1 < vtable.size()) window.slots[position] = vtable[slot - 1];
    }
    return window;
}

bool WindowMatchesAt(const FingerprintWindow& window, std::span<const Fingerprint> vtable, std::size_t index)
{
    if (index >= vtable.size()) return false;
    for (std::size_t position = 0; position < 3; ++position)
    {
        const auto& recorded = window.slots[position];
        if (!recorded) continue;
        const std::size_t slot = index + position;
        if (slot < 1 || slot - 1 >= vtable.size() || vtable[slot - 1] != *recorded) return false;
    }
    return true;
}

bool UniqueNearby(std::span<const Fingerprint> vtable, std::size_t index)
{
    if (index >= vtable.size()) return false;
    const std::size_t first = index >= kSearchRange ? index - kSearchRange : 0;
    const std::size_t last = std::min(vtable.size() - 1, index + kSearchRange);
    std::size_t count = 0;
    for (std::size_t slot = first; slot <= last; ++slot)
        if (vtable[slot] == vtable[index]) ++count;
    return count == 1;
}

SlotRecord RecordSlot(std::span<const Fingerprint> vtable, std::size_t index)
{
    return { index, WindowAt(vtable, index), UniqueNearby(vtable, index), vtable.size() };
}

SlotMatch CheckSlot(const SlotRecord& recorded, std::span<const Fingerprint> live)
{
    const std::size_t index = recorded.index;
    if (WindowMatchesAt(recorded.window, live, index)) return SlotMatch::Window;

    const auto& own = recorded.window.slots[1];
    if (recorded.uniqueNearby && own && index < live.size() && live[index] == *own && UniqueNearby(live, index))
        return SlotMatch::OwnFingerprint;
    return SlotMatch::None;
}

} // namespace counterstrikesharp::vtable
