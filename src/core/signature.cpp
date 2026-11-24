#include "core/signature.h"

#include <algorithm>
#include <string>

namespace counterstrikesharp {

std::vector<int16_t> ParseSignature(std::string_view src)
{
    if (src.empty())
    {
        return {};
    }

    auto hex_char_to_byte = [](char c) -> int16_t {
        if (c >= '0' && c <= '9') return c - '0';
        if (c >= 'A' && c <= 'F') return c - 'A' + 10;
        if (c >= 'a' && c <= 'f') return c - 'a' + 10;

        return -1;
    };

    std::vector<int16_t> result{};

    const bool is_code_style = src[0] == '\\';

    const std::string_view pattern = is_code_style ? R"(\x)" : " ";
    const std::string_view wildcard = is_code_style ? "2A" : "?";

    std::string::size_type pos = 0;

    while (pos < src.size())
    {
        std::string::size_type found = src.find(pattern, pos);
        if (found == std::string::npos)
        {
            found = src.size();
        }
        std::string_view str = src.substr(pos, found - pos);
        pos = found + pattern.size();

        if (str.empty()) continue;

        std::string byte(str.data(), str.size());

        if (byte.substr(0, wildcard.size()) == wildcard)
        {
            result.emplace_back(-1);
            continue;
        }

        if (byte.size() != 2)
        {
            return {};
        }

        const auto high = hex_char_to_byte(byte[0]);
        const auto low = hex_char_to_byte(byte[1]);

        if (high < 0 || low < 0)
        {
            return {};
        }

        result.emplace_back((high << 4) | low);
    }

    return result;
}

const std::uint8_t* FindPattern(const std::uint8_t* data, std::size_t size, const std::vector<int16_t>& pattern)
{
    if (pattern.empty() || size < pattern.size()) return nullptr;

    const auto first_byte = pattern[0];
    const std::uint8_t* end = data + size - pattern.size() + 1; // one past the last candidate start

    for (const std::uint8_t* current = data; current < end; ++current)
    {
        if (first_byte != -1)
        {
            current = std::find(current, end, first_byte);
            if (current == end) break;
        }

        if (std::equal(pattern.begin() + 1, pattern.end(), current + 1, [](auto opt, auto byte) {
            return opt == -1 || opt == byte;
        }))
        {
            return current;
        }
    }

    return nullptr;
}

} // namespace counterstrikesharp
