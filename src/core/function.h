/**
 * =============================================================================
 * Source Python
 * Copyright (C) 2012-2015 Source Python Development Team.  All rights reserved.
 * =============================================================================
 *
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License, version 3.0, as published by the
 * Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
 * details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * As a special exception, the Source Python Team gives you permission
 * to link the code of this program (as well as its derivative works) to
 * "Half-Life 2," the "Source Engine," and any Game MODs that run on software
 * by the Valve Corporation.  You must obey the GNU General Public License in
 * all respects for all other code used.  Additionally, the Source.Python
 * Development Team grants this exception to all derivative works.
 *
 * This file has been modified from its original form, under the GNU General
 * Public License, version 3.0.
 */

#pragma once

#include "scripting/callback_manager.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {

enum DataType_t {
    DATA_TYPE_VOID,
    DATA_TYPE_BOOL,
    DATA_TYPE_CHAR,
    DATA_TYPE_UCHAR,
    DATA_TYPE_SHORT,
    DATA_TYPE_USHORT,
    DATA_TYPE_INT,
    DATA_TYPE_UINT,
    DATA_TYPE_LONG,
    DATA_TYPE_ULONG,
    DATA_TYPE_LONG_LONG,
    DATA_TYPE_ULONG_LONG,
    DATA_TYPE_FLOAT,
    DATA_TYPE_DOUBLE,
    DATA_TYPE_POINTER,
    DATA_TYPE_STRING,
    DATA_TYPE_VARIANT
};

enum Protection_t {
    PROTECTION_NONE,
    PROTECTION_READ,
    PROTECTION_READ_WRITE,
    PROTECTION_EXECUTE,
    PROTECTION_EXECUTE_READ,
    PROTECTION_EXECUTE_READ_WRITE
};

enum Convention_t { CONV_CUSTOM, CONV_CDECL, CONV_THISCALL, CONV_STDCALL, CONV_FASTCALL };

class ValveFunction {
public:
    ValveFunction(void* ulAddr,
                  Convention_t callingConvention,
                  std::vector<DataType_t> args,
                  DataType_t returnType);
    ValveFunction(void* ulAddr,
                  Convention_t callingConvention,
                  DataType_t* args,
                  int argCount,
                  DataType_t returnType);

    ~ValveFunction();

    bool IsCallable();
    // bool IsHookable();

    // bool IsHooked();
    // CHook* GetHook();

    // ValveFunction* GetTrampoline();

    void SetOffset(int offset) { m_offset = offset; }
    void SetSignature(const char* signature) { m_signature = signature; }

    void Call(ScriptContext& args, int offset = 0);
    // CHook* AddHook(HookType_t eType, void* callable);
    // void DeleteHook();

    void* m_ulAddr;
    std::vector<DataType_t> m_Args;
    DataType_t m_eReturnType;

    // Shared built-in calling convention identifier
    Convention_t m_eCallingConvention;

    // DynCall calling convention
    int m_iCallingConvention;

    int m_offset;
    const char* m_signature;
};

}  // namespace counterstrikesharp