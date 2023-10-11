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

#pragma once

#include "core/global_listener.h"
#include "core/globals.h"
#include "scripting/script_engine.h"

#define REGISTER_NATIVE(name, func) \
    counterstrikesharp::ScriptEngine::RegisterNativeHandler(#name, func);

#define REGISTER_NATIVES(name, method)                             \
    class Natives##name : public counterstrikesharp::GlobalClass { \
    public:                                                        \
        void OnAllInitialized() override method                    \
    };                                                             \
                                                                   \
    Natives##name g_natives_##name;

#define CREATE_GETTER_FUNCTION(object_name, parameter_type, parameter_name, from_type, getter) \
    static parameter_type object_name##Get##parameter_name(ScriptContext &script_context) {    \
        auto obj = script_context.GetArgument<from_type>(0);                                   \
        return getter;                                                                         \
    }

#define CREATE_STATIC_GETTER_FUNCTION(parameter_name, parameter_type, getter) \
    static parameter_type Get##parameter_name(ScriptContext &script_context) { return getter; }

#define CREATE_SETTER_FUNCTION(type_name, get_type, name, from_type, setter) \
    static void type_name##Set##name(ScriptContext &script_context) {        \
        auto obj = script_context.GetArgument<from_type>(0);                 \
        auto value = script_context.GetArgument<get_type>(1);                \
        setter;                                                              \
    }