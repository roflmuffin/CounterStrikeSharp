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
