/*
 * Copyright (c) 2014 Bas Timmer/NTAuthority et al.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * This file has been modified from its original form for use in this program
 * under GNU Lesser General Public License, version 2.
 */

#include "scripting/script_engine.h"

#include <stack>
#include <unordered_map>

#include "core/log.h"
#include "core/utils.h"

static std::unordered_map<uint64_t, counterstrikesharp::TNativeHandler> g_registeredHandlers;

namespace counterstrikesharp {

std::stack<std::string> errors;

void ScriptContext::ThrowNativeError(const char* msg, ...)
{
    va_list arglist;
    char dest[256];
    va_start(arglist, msg);
    vsprintf(dest, msg, arglist);
    va_end(arglist);
    char buff[256];
    snprintf(buff, sizeof(buff), dest, arglist);

    auto error_string = std::string(buff);
    errors.push(error_string);

    const char* ptr = errors.top().c_str();
    this->SetResult(ptr);
    *this->m_has_error = 1;
}

void ScriptContext::Reset()
{
    if (*m_has_error)
    {
        errors.pop();
    }

    m_numResults = 0;
    m_numArguments = 0;
    *m_has_error = 0;

    for (int i = 0; i < 32; i++)
    {
        m_native_context->arguments[i] = 0;
    }

    m_native_context->result = 0;
}

tl::optional<TNativeHandler> ScriptEngine::GetNativeHandler(uint64_t nativeIdentifier)
{
    auto it = g_registeredHandlers.find(nativeIdentifier);

    if (it != g_registeredHandlers.end())
    {
        return it->second;
    }

    return tl::optional<TNativeHandler>();
}

tl::optional<TNativeHandler> ScriptEngine::GetNativeHandler(std::string identifier)
{
    auto it = g_registeredHandlers.find(hash_string(identifier.c_str()));

    if (it != g_registeredHandlers.end())
    {
        return it->second;
    }

    return tl::optional<TNativeHandler>();
}

bool ScriptEngine::CallNativeHandler(uint64_t nativeIdentifier, ScriptContext& context)
{
    auto h = GetNativeHandler(nativeIdentifier);
    if (h)
    {
        (*h)(context);

        return true;
    }

    return false;
}

void ScriptEngine::RegisterNativeHandlerInt(uint64_t nativeIdentifier, TNativeHandler function)
{
    g_registeredHandlers[nativeIdentifier] = function;
}

void ScriptEngine::InvokeNative(counterstrikesharp::fxNativeContext& context)
{
    if (context.nativeIdentifier == 0) return;

    auto nativeHandler = counterstrikesharp::ScriptEngine::GetNativeHandler(context.nativeIdentifier);

    if (nativeHandler)
    {
        counterstrikesharp::ScriptContextRaw scriptContext(context);

        (*nativeHandler)(scriptContext);
    }
    else
    {
        CSSHARP_CORE_WARN("Native Handler was requested but not found: {0:x}", context.nativeIdentifier);
        assert(false);
    }
}

ScriptContextRaw ScriptEngine::m_context;

} // namespace counterstrikesharp
