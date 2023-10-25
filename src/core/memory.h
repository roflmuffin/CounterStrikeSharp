#include <cstring>


#ifdef _WIN32
    #define ROOTBIN "/bin/win64/"
    #define GAMEBIN "/csgo/bin/win64/"
#else
    #define ROOTBIN "/bin/linuxsteamrt64/"
    #define GAMEBIN "/csgo/bin/linuxsteamrt64/"
#endif

#define MODULE_PREFIX "lib"
#define MODULE_EXT ".so"

int GetModuleInformation(void *hModule, void **base, size_t *length);
void* FindSignature(const char* moduleName, const char* bytesStr);
