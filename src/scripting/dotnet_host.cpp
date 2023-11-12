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

#include "scripting/dotnet_host.h"

#include <dotnet/coreclr_delegates.h>
#include <dotnet/hostfxr.h>

#include <codecvt>
#include <locale>

#ifdef WIN32
#include <Windows.h>
#include <direct.h>

#define STR(s) L##s
#define CH(c) L##c
#define DIR_SEPARATOR L'\\'

#else
#define STR(s) s
#define CH(c) c
#define DIR_SEPARATOR '/'

#include <dlfcn.h>
#endif

#include <cassert>
#include <iostream>

#include "core/log.h"
#include "core/utils.h"

#include "utils/string.h"

std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;

namespace {
hostfxr_initialize_for_runtime_config_fn init_fptr;
hostfxr_get_runtime_delegate_fn get_delegate_fptr;
hostfxr_close_fn close_fptr;
hostfxr_handle cxt;

bool load_hostfxr();
load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* assembly);
} // namespace

namespace {
// Forward declarations
void* load_library(const char_t*);
void* get_export(void*, const char*);

#ifdef _WINDOWS
void* load_library(const char_t* path)
{
    HMODULE h = ::LoadLibraryW(path);
    assert(h != nullptr);
    return (void*)h;
}

void* get_export(void* h, const char* name)
{
    void* f = ::GetProcAddress((HMODULE)h, name);
    assert(f != nullptr);
    return f;
}
#else
void* load_library(const char_t* path)
{
    void* h = dlopen(path, RTLD_LAZY | RTLD_LOCAL);
    assert(h != nullptr);
    return h;
}
void* get_export(void* h, const char* name)
{
    void* f = dlsym(h, name);
    assert(f != nullptr);
    return f;
}
#endif

// <SnippetLoadHostFxr>
// Using the nethost library, discover the location of hostfxr and get exports
bool load_hostfxr()
{
    std::string base_dir = counterstrikesharp::utils::PluginDirectory();
    namespace css = counterstrikesharp;
#if _WIN32
    std::wstring buffer =
        std::wstring(css::widen(base_dir) + L"\\dotnet\\host\\fxr\\7.0.11\\hostfxr.dll");
    CSSHARP_CORE_INFO("Loading hostfxr from {0}", css::narrow(buffer).c_str());
#else
    std::string buffer = std::string(base_dir + "/dotnet/host/fxr/7.0.11/libhostfxr.so");
    CSSHARP_CORE_INFO("Loading hostfxr from {0}", buffer.c_str());
#endif

    // Load hostfxr and get desired exports
    void* lib = load_library(buffer.c_str());
    init_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(
        lib, "hostfxr_initialize_for_runtime_config");
    if (init_fptr == nullptr) {
        CSSHARP_CORE_CRITICAL(
            "unable to get export function: \"hostfxr_initialize_for_runtime_config\"");
        return false;
    }
    get_delegate_fptr =
        (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
    if (!get_delegate_fptr) {
        CSSHARP_CORE_CRITICAL("unable to get export function: \"hostfxr_get_runtime_delegate\"");
        return false;
    }
    close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");
    if (!close_fptr) {
        CSSHARP_CORE_CRITICAL("unable to get export function: \"hostfxr_close\"");
        return false;
    }

    return (init_fptr && get_delegate_fptr && close_fptr);
}
// </SnippetLoadHostFxr>

// <SnippetInitialize>
// Load and initialize .NET Core and get desired function pointer for scenario
load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* config_path)
{
    // Load .NET Core
    void* load_assembly_and_get_function_pointer = nullptr;
    int rc = init_fptr(config_path, nullptr, &cxt);
    if (rc != 0 || cxt == nullptr) {
        CSSHARP_CORE_CRITICAL("Init failed: {0:x}", rc);
        close_fptr(cxt);
        return nullptr;
    }

    // Get the load assembly function pointer
    rc = get_delegate_fptr(cxt, hdt_load_assembly_and_get_function_pointer,
                           &load_assembly_and_get_function_pointer);
    if (rc != 0 || load_assembly_and_get_function_pointer == nullptr) {
        CSSHARP_CORE_ERROR("Get delegate failed: {0:x}", rc);
    }

    // close_fptr(cxt);
    return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
}

} // namespace

CDotNetManager::CDotNetManager() {}

CDotNetManager::~CDotNetManager() {}

bool CDotNetManager::Initialize()
{
    const std::string base_dir = counterstrikesharp::utils::PluginDirectory();

    CSSHARP_CORE_INFO("Loading .NET runtime...");

    if (!load_hostfxr()) {
        CSSHARP_CORE_ERROR("Failed to initialize .NET runtime.");
        return false;
    }
    CSSHARP_CORE_INFO(".NET Runtime Initialised.");
    namespace css = counterstrikesharp;
#if _WIN32
    const auto wide_str =
        std::wstring(css::widen(base_dir) + L"\\api\\CounterStrikeSharp.API.runtimeconfig.json");
    CSSHARP_CORE_INFO("Loading CSS API, Runtime config: {}",
                      counterstrikesharp::narrow(wide_str).c_str());
#else
    std::string wide_str =
        std::string((base_dir + "/api/CounterStrikeSharp.API.runtimeconfig.json").c_str());
    CSSHARP_CORE_INFO("Loading CSS API, Runtime Config: {}", wide_str);
#endif
    
    const auto load_assembly_and_get_function_pointer = get_dotnet_load_assembly(wide_str.c_str());
    if (load_assembly_and_get_function_pointer == nullptr) {
        CSSHARP_CORE_ERROR("Failed to load CSS API.");
        return false;
    }

#if _WIN32
    const auto dotnetlib_path =
        std::wstring(css::widen(base_dir) + L"\\api\\CounterStrikeSharp.API.dll");
    CSSHARP_CORE_INFO("CSS API DLL: {}", counterstrikesharp::narrow(dotnetlib_path));
#else
    const std::string dotnetlib_path =
        std::string((base_dir + "/api/CounterStrikeSharp.API.dll").c_str());
#endif
    const auto dotnet_type = STR("CounterStrikeSharp.API.Core.Helpers, CounterStrikeSharp.API");
    // Namespace, assembly name

    typedef int(CORECLR_DELEGATE_CALLTYPE * custom_entry_point_fn)();
    custom_entry_point_fn entry_point = nullptr;
    const int rc = load_assembly_and_get_function_pointer(
        dotnetlib_path.c_str(), dotnet_type, STR("LoadAllPlugins"), UNMANAGEDCALLERSONLY_METHOD,
        nullptr, reinterpret_cast<void**>(&entry_point));
    if (entry_point == nullptr) {
        CSSHARP_CORE_ERROR("Trying to get entry point \"LoadAllPlugins\" but failed.");
        return false;
    }

    assert(rc == 0 && entry_point != nullptr &&
           "Failure: load_assembly_and_get_function_pointer()");

    if (const int invoke_result_code = entry_point(); invoke_result_code == 0) {
        CSSHARP_CORE_ERROR("LoadAllPlugins return failure.");
        return false;
    }

    CSSHARP_CORE_INFO("CounterStrikeSharp.API Loaded Successfully.");
    return true;
}

void CDotNetManager::UnloadPlugin(PluginContext* context) {}

void CDotNetManager::Shutdown()
{
    // CoreCLR does not currently supporting unloading... :(
    // I think this is intentionally, you should handle Init/Shutdown manually.
    // Better rework in the future, but not now.
}

PluginContext* CDotNetManager::FindContext(std::string path) { return nullptr; }
