#include <doctest.h>

#include <cstdint>
#include <vector>

#include "core/signature.h"

using namespace counterstrikesharp;

TEST_CASE("ParseSignature reads space-separated hex with ? wildcards")
{
    CHECK(ParseSignature("55 48 ? 89 E5") == std::vector<int16_t>{ 0x55, 0x48, -1, 0x89, 0xE5 });
    CHECK(ParseSignature("55 ?? e5") == std::vector<int16_t>{ 0x55, -1, 0xE5 });
}

TEST_CASE("ParseSignature reads code-style \\x hex with \\x2A wildcards")
{
    CHECK(ParseSignature(R"(\x55\x2A\xE5)") == std::vector<int16_t>{ 0x55, -1, 0xE5 });
}

TEST_CASE("ParseSignature rejects invalid patterns")
{
    CHECK(ParseSignature("").empty());
    CHECK(ParseSignature("55 GG E5").empty());
    CHECK(ParseSignature("55 4 E5").empty());
    CHECK(ParseSignature("55 488 E5").empty());
    CHECK(ParseSignature(R"(\x55\xZZ)").empty());
}

TEST_CASE("FindPattern finds a match at the final candidate position")
{
    const std::vector<std::uint8_t> bytes{ 0x00, 0x11, 0x22, 0x33 };
    const auto* match = FindPattern(bytes.data(), bytes.size(), ParseSignature("22 33"));
    REQUIRE(match != nullptr);
    CHECK(match - bytes.data() == 2);
}

TEST_CASE("FindPattern honours wildcards, including a leading one")
{
    const std::vector<std::uint8_t> bytes{ 0x10, 0xAA, 0x20, 0x30 };
    const auto* match = FindPattern(bytes.data(), bytes.size(), ParseSignature("? 20 30"));
    REQUIRE(match != nullptr);
    CHECK(match - bytes.data() == 1);
}

TEST_CASE("FindPattern returns nullptr when absent or when the range is shorter than the pattern")
{
    const std::vector<std::uint8_t> bytes{ 0x00, 0x11 };
    CHECK(FindPattern(bytes.data(), bytes.size(), ParseSignature("11 22")) == nullptr);
    CHECK(FindPattern(bytes.data(), bytes.size(), ParseSignature("00 11 22")) == nullptr);
    CHECK(FindPattern(bytes.data(), 0, ParseSignature("00")) == nullptr);
}

namespace {
std::vector<std::uint8_t> PseudoCode(std::size_t size)
{
    std::vector<std::uint8_t> bytes(size);
    std::uint32_t state = 12345;
    for (auto& byte : bytes)
    {
        state = state * 1103515245u + 12345u;
        byte = static_cast<std::uint8_t>(state >> 16);
    }
    return bytes;
}
} // namespace

TEST_CASE("SegmentCopyMatchesImage accepts an identical copy and one with a few hooked bytes")
{
    const auto live = PseudoCode(64 * 1024);
    auto disk = live;
    CHECK(SegmentCopyMatchesImage(disk.data(), live.data(), live.size()));

    for (std::size_t hook = 0; hook < 100; ++hook) // 100 five-byte detours
        for (std::size_t i = 0; i < 5; ++i)
            disk[hook * 600 + i] ^= 0xFF;
    CHECK(SegmentCopyMatchesImage(disk.data(), live.data(), live.size()));
}

TEST_CASE("SegmentCopyMatchesImage rejects a copy read from the wrong file offset")
{
    // libserver.so: the executable PT_LOAD has p_offset 0x9e40fc but p_vaddr 0x9e50fc. Reading the file at the
    // virtual address yields the image shifted by 0x1000 bytes, so signatures resolve 0x1000 away from the function.
    const auto file = PseudoCode(64 * 1024 + 0x1000);
    const std::uint8_t* live = file.data();
    const std::uint8_t* shifted = file.data() + 0x1000;
    CHECK_FALSE(SegmentCopyMatchesImage(shifted, live, 64 * 1024));
}
