#pragma once

#include <cstddef>
#include <cstdint>
#include <string_view>
#include <vector>

namespace counterstrikesharp {

// Parses a signature into bytes, with -1 for a wildcard. Accepts space-separated hex ("55 48 ? E5", wildcard ? or ??)
// and code style ("\x55\x2A\xE5", wildcard \x2A). Returns an empty vector for an invalid pattern.
std::vector<int16_t> ParseSignature(std::string_view src);

// Returns the first position in [data, data + size) where the pattern matches, or nullptr.
// Every candidate start is tested, including the last one (data + size - pattern size).
const std::uint8_t* FindPattern(const std::uint8_t* data, std::size_t size, const std::vector<int16_t>& pattern);

} // namespace counterstrikesharp
