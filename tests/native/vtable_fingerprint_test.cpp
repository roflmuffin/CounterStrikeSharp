#include <doctest.h>

#include <cstdint>
#include <string>
#include <vector>

#include "core/vtable_fingerprint.h"

using namespace counterstrikesharp::vtable;

namespace {
std::vector<std::uint8_t> Hex(std::string_view hex)
{
    std::vector<std::uint8_t> bytes;
    for (std::size_t i = 0; i + 1 < hex.size(); i += 2)
        bytes.push_back(static_cast<std::uint8_t>(std::stoi(std::string(hex.substr(i, 2)), nullptr, 16)));
    return bytes;
}

struct OracleCase
{
    const char* name;
    const char* code;
    const char* fingerprint;
    std::size_t instructions;
    const char* canonical;
};

// Hand-written code with the fingerprints the research oracle (Python, capstone) computes for it.
const OracleCase kOracleCases[] = {
    { "ret only", "C3", "082fe507b4e9d81d", 1, "C3" },
    { "padding after ret is not decoded", "31C0C3CCCCCC90", "dc3e8eb9a1a8f4c4", 2, "31C0 C3" },
    { "tail call stops, target masked", "E911223344CC", "90f769c9959c9bfb", 1, "E900000000" },
    { "indirect jmp through RIP stops", "FF2511223344CC", "ca17441af0c69d65", 1, "FF2500000000" },
    { "RIP-relative load masked", "488B0511223344C3", "972e971c1fdaf40e", 2, "488B0500000000 C3" },
    { "struct displacement8 and imm8 masked", "C6470501C3", "07babcbaff6b6cc8", 2, "C6470000 C3" },
    { "struct displacement32 and imm32 masked", "C7871801000078563412C3", "b2a5487923384d27", 2, "C7870000000000000000 C3" },
    { "imm8 and imm32 encodings stay distinct", "4883C4784881C488000000C3", "f9a23ffcca4aa668", 3, "4883C400 4881C400000000 C3" },
    { "conditional jumps do not stop", "74050F8411223344C3", "24218185e155dc52", 3, "7400 0F8400000000 C3" },
    { "calls do not stop", "E8112233444889C7C3", "aefa0a50fcce4c8f", 3, "E800000000 4889C7 C3" },
    { "imm64 masked", "48B81122334455667788C3", "ec53451a4e2aaa2d", 2, "48B80000000000000000 C3" },
    { "both immediates of enter masked", "C8100001C3", "b7bb4cb55416d0a7", 2, "C8000000 C3" },
    { "SSE RIP-relative masked", "F30F100511223344C3", "c045328ec5145c16", 2, "F30F100500000000 C3" },
    { "VEX RIP-relative masked", "C5FA100511223344C3", "4734a568cf2adac7", 2, "C5FA100500000000 C3" },
    { "SIB displacement masked", "8B44241CC3", "84296e3f8fabf210", 2, "8B442400 C3" },
    { "at most 24 instructions", "909090909090909090909090909090909090909090909090909090909090", "57e03cbe7cc69205", 24,
      "90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90 90" },
    { "frame prologue", "554889E541574156534883EC68488D0511223344C3", "4a52e1b038b7eb0b", 8,
      "55 4889E5 4157 4156 53 4883EC00 488D0500000000 C3" },
};

constexpr std::uintptr_t kCodeAddress = 0x401000;

// A synthetic module: functions laid out in one code segment, and a vtable of their addresses.
struct FakeVtable
{
    std::vector<std::uint8_t> code;
    std::vector<std::uintptr_t> slots;
    std::vector<CodeSegment> segments;

    // Each function is its bytes; "pure" and "deleted" put the pure/deleted virtual markers in the slot.
    explicit FakeVtable(const std::vector<std::string>& functions)
    {
        for (const auto& function : functions)
        {
            if (function == "pure") slots.push_back(kPure);
            else if (function == "deleted")
                slots.push_back(kDeleted);
            else
            {
                slots.push_back(kCodeAddress + code.size());
                const auto bytes = Hex(function);
                code.insert(code.end(), bytes.begin(), bytes.end());
            }
        }
        segments.push_back({ kCodeAddress, code.data(), code.size() });
    }

    VtableSource Source(std::size_t readable) const
    {
        return { [this, readable](std::size_t index) -> std::optional<std::uintptr_t> {
                    if (index >= readable) return std::nullopt;
                    return index < slots.size() ? slots[index] : 0x7777;
                },
                 segments, kPure, kDeleted };
    }

    static constexpr std::uintptr_t kPure = 0x100;
    static constexpr std::uintptr_t kDeleted = 0x200;
};

std::vector<Fingerprint> Fingerprints(std::initializer_list<const char*> names) { return { names.begin(), names.end() }; }
} // namespace

TEST_CASE("Fingerprints match the research oracle on hand-written code")
{
    for (const auto& oracle : kOracleCases)
    {
        CAPTURE(oracle.name);
        const auto code = Hex(oracle.code);
        const CodeSegment segment{ kCodeAddress, code.data(), code.size() };

        const auto result = FingerprintFunction({ &segment, 1 }, kCodeAddress);

        CHECK(result.fingerprint == oracle.fingerprint);
        CHECK(result.instructions == oracle.instructions);
        CHECK(result.canonical == oracle.canonical);
    }
}

TEST_CASE("A function outside the code has no fingerprint, and decoding stops at the end of its segment")
{
    const auto code = Hex("4889C7"); // mov rdi, rax; then the segment ends
    const CodeSegment segment{ kCodeAddress, code.data(), code.size() };

    CHECK(FingerprintFunction({ &segment, 1 }, 0x1234).fingerprint.empty());
    const auto truncated = FingerprintFunction({ &segment, 1 }, kCodeAddress);
    CHECK(truncated.instructions == 1);
    CHECK(truncated.canonical == "4889C7");
}

TEST_CASE("A vtable's fingerprints end at the first slot that is not code; pure and deleted virtuals count")
{
    const FakeVtable vtable({ "C3", "pure", "31C0C3", "deleted" });

    const auto fingerprints = FingerprintVtable(vtable.Source(100));

    REQUIRE(fingerprints.size() == 4);
    CHECK(fingerprints[0] == "082fe507b4e9d81d");
    CHECK(fingerprints[1] == "pure");
    CHECK(fingerprints[2] == "dc3e8eb9a1a8f4c4");
    CHECK(fingerprints[3] == "deleted");
    CHECK(FingerprintVtable(vtable.Source(2)).size() == 2); // readable memory ends
}

TEST_CASE("A window records the slot and its neighbours; at the recorded index a neighbour outside the vtable matches anything")
{
    const auto vtable = Fingerprints({ "a", "b", "c" });

    CHECK(WindowAt(vtable, 1) == FingerprintWindow{ { "a", "b", "c" } });
    const auto first = WindowAt(vtable, 0);
    CHECK(first == FingerprintWindow{ { std::nullopt, "a", "b" } });
    CHECK(WindowAt(vtable, 2) == FingerprintWindow{ { "b", "c", std::nullopt } });

    CHECK(WindowMatchesAt(first, Fingerprints({ "a", "b", "x" }), 0));
    CHECK(WindowMatchesAt(first, Fingerprints({ "z", "a", "b" }), 1)); // the wildcard matches a real slot too
    CHECK_FALSE(WindowMatchesAt(first, Fingerprints({ "a", "x" }), 0));
    CHECK_FALSE(WindowMatchesAt(WindowAt(vtable, 1), Fingerprints({ "a", "b" }), 1)); // recorded neighbour missing live
    CHECK_FALSE(WindowMatchesAt(WindowAt(vtable, 1), Fingerprints({ "a" }), 5));
}

TEST_CASE("The whole window matches only where a neighbour recorded outside the vtable is outside it too")
{
    const auto last = WindowAt(Fingerprints({ "a", "b", "c" }), 2); // [b, c, null]

    CHECK(WholeWindowMatchesAt(last, Fingerprints({ "x", "b", "c" }), 2));
    CHECK_FALSE(WholeWindowMatchesAt(last, Fingerprints({ "x", "b", "c", "d" }), 2));
    CHECK(WindowMatchesAt(last, Fingerprints({ "x", "b", "c", "d" }), 2));
    CHECK_FALSE(WholeWindowMatchesAt(WindowAt(Fingerprints({ "a", "b" }), 0), Fingerprints({ "z", "a", "b" }), 1));
    CHECK_FALSE(WholeWindowMatchesAt(last, Fingerprints({ "b", "c" }), 5));

    // The Heal search uses the whole window; the carry-forward search too, except at the recorded index.
    CHECK(WindowMatchesNear(last, Fingerprints({ "n", "x", "b", "c", "d" }), 2).empty());
    CHECK(WindowMatchesNear(last, Fingerprints({ "n", "x", "b", "c" }), 2) == std::vector<std::size_t>{ 3 });
    CHECK(WindowMatchesAnywhere(last, Fingerprints({ "x", "b", "c", "d" }), 2) == std::vector<std::size_t>{ 2 });
    CHECK(WindowMatchesAnywhere(last, Fingerprints({ "n", "x", "b", "c", "d" }), 2).empty());
}

TEST_CASE("A slot is unique nearby when its fingerprint occurs once within the search range")
{
    std::vector<Fingerprint> vtable(40, "x");
    for (std::size_t i = 0; i < vtable.size(); ++i)
        vtable[i] = "f" + std::to_string(i);
    CHECK(UniqueNearby(vtable, 20));

    vtable[20 + kSearchRange] = vtable[20];
    CHECK_FALSE(UniqueNearby(vtable, 20));
    vtable[20 + kSearchRange] = "other";
    vtable[20 + kSearchRange + 1] = vtable[20]; // just outside the range
    CHECK(UniqueNearby(vtable, 20));
}

TEST_CASE("Recording a slot keeps its index, window, uniqueness and the vtable size")
{
    const auto vtable = Fingerprints({ "a", "b", "c", "d" });

    const auto record = RecordSlot(vtable, 2);

    CHECK(record.index == 2);
    CHECK(record.window == FingerprintWindow{ { "b", "c", "d" } });
    CHECK(record.uniqueNearby);
    CHECK(record.vtableSize == 4u);
}

TEST_CASE("Checking a recorded slot against a live vtable")
{
    const auto recorded = RecordSlot(Fingerprints({ "a", "b", "c", "d", "e" }), 2);

    SUBCASE("the same vtable matches by window")
    {
        CHECK(CheckSlot(recorded, Fingerprints({ "a", "b", "c", "d", "e" })) == SlotMatch::Window);
    }
    SUBCASE("a size change elsewhere still matches by window")
    {
        CHECK(CheckSlot(recorded, Fingerprints({ "a", "b", "c", "d", "e", "f", "g" })) == SlotMatch::Window);
    }
    SUBCASE("a changed neighbour still matches by the slot's own fingerprint, unique nearby on both builds")
    {
        CHECK(CheckSlot(recorded, Fingerprints({ "a", "b", "c", "D", "e" })) == SlotMatch::OwnFingerprint);
    }
    SUBCASE("a changed neighbour fails when the slot's fingerprint is not unique nearby live")
    {
        CHECK(CheckSlot(recorded, Fingerprints({ "a", "b", "c", "D", "c" })) == SlotMatch::None);
    }
    SUBCASE("a changed neighbour fails when the slot's fingerprint was not unique nearby when recorded")
    {
        const auto shared = RecordSlot(Fingerprints({ "c", "b", "c", "d", "e" }), 2);
        CHECK_FALSE(shared.uniqueNearby);
        CHECK(CheckSlot(shared, Fingerprints({ "x", "b", "c", "D", "e" })) == SlotMatch::None);
    }
    SUBCASE("a changed slot fails")
    {
        CHECK(CheckSlot(recorded, Fingerprints({ "a", "b", "C", "d", "e" })) == SlotMatch::None);
    }
    SUBCASE("a vtable too short for the slot fails")
    {
        CHECK(CheckSlot(recorded, Fingerprints({ "a", "b" })) == SlotMatch::None);
    }
}
