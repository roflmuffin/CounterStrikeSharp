// Queries a compiled plugin's metadata without calling Load or starting CS2.
#include <ISmmPlugin.h>
#include <dlfcn.h>
#include <iostream>

int main(int argc, char** argv) {
    if (argc != 2) {
        std::cerr << "Usage: probe_plugin /path/to/counterstrikesharp.so\n";
        return 2;
    }
    auto library = dlopen(argv[1], RTLD_NOW | RTLD_LOCAL);
    if (!library) {
        std::cerr << dlerror() << '\n';
        return 1;
    }
    using Factory = void* (*)(const char*, int*);
    auto factory = reinterpret_cast<Factory>(dlsym(library, "CreateInterface"));
    if (!factory) {
        std::cerr << "CreateInterface not exported\n";
        return 1;
    }
    auto plugin = static_cast<ISmmPlugin*>(factory(METAMOD_PLAPI_NAME, nullptr));
    if (!plugin) {
        std::cerr << "ISmmPlugin not exposed\n";
        return 1;
    }
    std::cout << plugin->GetName() << " " << plugin->GetVersion()
              << " API " << plugin->GetApiVersion() << '\n';
    return plugin->GetApiVersion() == 18 ? 0 : 1;
}
