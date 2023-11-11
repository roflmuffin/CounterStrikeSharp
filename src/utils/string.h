/**
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

#pragma once

#include <string>
#include <sstream>
namespace counterstrikesharp {
std::wstring widen(const std::string& str)
{
    std::wostringstream wstm;
    const auto& ctfacet = std::use_facet<std::ctype<wchar_t>>(wstm.getloc());
    for (size_t i = 0; i < str.size(); ++i) {
        wstm << ctfacet.widen(str[i]);
    }
    return wstm.str();
}

std::string narrow(const std::wstring& str)
{
    std::ostringstream stm;

    // Incorrect code from the link
    // const ctype<char>& ctfacet = use_facet<ctype<char>>(stm.getloc());

    // Correct code.
    const auto& ctfacet = std::use_facet<std::ctype<wchar_t>>(stm.getloc());

    for (size_t i = 0; i < str.size(); ++i) {
        stm << ctfacet.narrow(str[i], 0);
    }
    return stm.str();
}
}