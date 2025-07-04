/*
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

#include <public/entity2/entitysystem.h>

#include <ios>
#include <sstream>

#include "scripting/autonative.h"
#include "scripting/script_engine.h"

#include "utlsymbollarge.h"

namespace counterstrikesharp {

const char* GetStringFromSymbolLarge(ScriptContext& script_context)
{
    CUtlSymbolLarge* pSymbolLarge = script_context.GetArgument<CUtlSymbolLarge*>(0);
    if (!pSymbolLarge)
    {
        script_context.ThrowNativeError("Invalid CUtlSymbolLarge pointer");
        return "";
    }
    return pSymbolLarge->String();
}

REGISTER_NATIVES(cutil, { ScriptEngine::RegisterNativeHandler("GET_STRING_FROM_SYMBOL_LARGE", GetStringFromSymbolLarge); })
} // namespace counterstrikesharp
