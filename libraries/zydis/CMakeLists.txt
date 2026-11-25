# Zydis 4.0.0 (https://github.com/zyantific/zydis, MIT, see LICENSE), vendored as the upstream amalgamation
# (Zydis.h, Zydis.c) copied from the one Metamod's KHook ships (third_party/khook/third_party/safetyhook/zydis); the only
# change is Zydis.c's own include, "Zydis.h" instead of <Zydis.h>. Decodes the instructions vtable slot fingerprints
# are built from (src/core/vtable_fingerprint.cpp).
#
# A static library with hidden symbols, so it never clashes with the Zydis inside Metamod's binary.
if(NOT TARGET zydis)
    add_library(zydis STATIC ${CMAKE_CURRENT_LIST_DIR}/Zydis.c)
    target_include_directories(zydis PUBLIC ${CMAKE_CURRENT_LIST_DIR})
    set_target_properties(zydis PROPERTIES
        POSITION_INDEPENDENT_CODE ON
        C_VISIBILITY_PRESET hidden
    )
    if(MSVC)
        target_compile_options(zydis PRIVATE /w)
    else()
        target_compile_options(zydis PRIVATE -w)
    endif()
endif()
