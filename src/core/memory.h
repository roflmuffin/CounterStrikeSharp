#pragma once

#ifdef _WIN32
    #define ROOTBIN "/bin/win64/"
    #define GAMEBIN "/csgo/bin/win64/"
#define MODULE_PREFIX ""
#define MODULE_EXT ".dll"
#else
    #define ROOTBIN "/bin/linuxsteamrt64/"
    #define GAMEBIN "/csgo/bin/linuxsteamrt64/"
#define MODULE_PREFIX "lib"
#define MODULE_EXT ".so"
#endif

#if __linux__
int GetModuleInformation(void *hModule, void **base, size_t *length);
#endif
void* FindSignature(const char* moduleName, const char* bytesStr);
