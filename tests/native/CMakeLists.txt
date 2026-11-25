# Native unit tests (doctest).
#
# Links only pure sources from src/ (no SDK, Metamod or .NET), so it builds in
# seconds without submodules. Two ways to build it:
#
#   standalone:  cmake -S tests/native -B build-tests && cmake --build build-tests
#                cd build-tests && ctest --output-on-failure
#   in the plugin tree (opt-in, off by default):
#                cmake -DCOUNTERSTRIKESHARP_BUILD_TESTS=ON .. && cmake --build . && ctest
#
# New pure sources (e.g. the Binding resolver) go in CSSHARP_TESTED_SOURCES and
# new test files in CSSHARP_TEST_SOURCES. Also builds the offline Gamedata
# generator (gamedata-generator, Linux only); see tooling/gamedata-generator/main.cpp.
# Real-build tests (archive_test.cpp) read ~/dev/cs2-builds (or $CSSHARP_BUILD_ARCHIVE)
# and skip what is absent.
cmake_minimum_required(VERSION 3.18)

if(CMAKE_SOURCE_DIR STREQUAL CMAKE_CURRENT_SOURCE_DIR)
    project(counterstrikesharp_native_tests C CXX)
    set(CMAKE_CXX_STANDARD 20)
endif()

enable_testing()

set(CSSHARP_ROOT ${CMAKE_CURRENT_SOURCE_DIR}/../..)

# Production sources under test. Only add files that compile without the SDK.
set(CSSHARP_TESTED_SOURCES
    ${CSSHARP_ROOT}/src/core/tick_scheduler.cpp
    ${CSSHARP_ROOT}/src/core/signature.cpp
    ${CSSHARP_ROOT}/src/core/binding_resolver.cpp
    ${CSSHARP_ROOT}/src/core/engine_features.cpp
    ${CSSHARP_ROOT}/src/core/rtti_vtable.cpp
    ${CSSHARP_ROOT}/src/core/vtable_fingerprint.cpp
    ${CSSHARP_ROOT}/src/core/elf_relocations.cpp
)

set(CSSHARP_TEST_SOURCES
    main.cpp
    tick_scheduler_test.cpp
    signature_test.cpp
    binding_resolver_test.cpp
    engine_features_test.cpp
    rtti_vtable_test.cpp
    vtable_fingerprint_test.cpp
    slot_binding_test.cpp
    slot_calls_test.cpp
    icvar_bindings_test.cpp
    real_binary_test.cpp
)

# Vendored x86-64 decoder (plain files, no submodule); the plugin tree may have added it already.
if(NOT TARGET zydis)
    add_subdirectory(${CSSHARP_ROOT}/libraries/zydis ${CMAKE_CURRENT_BINARY_DIR}/zydis)
endif()

# The offline Gamedata generator (tooling/gamedata-generator) is built here too: it shares the fingerprint core with
# the boot-time Vtable Check, and its library is tested here. It reads Linux ELF images, so Linux only.
set(CSSHARP_GENERATOR_DIR ${CSSHARP_ROOT}/tooling/gamedata-generator)
if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    list(APPEND CSSHARP_TESTED_SOURCES ${CSSHARP_GENERATOR_DIR}/build_archive.cpp ${CSSHARP_GENERATOR_DIR}/generator.cpp)
    list(APPEND CSSHARP_TEST_SOURCES generator_test.cpp archive_test.cpp icvar_archive_test.cpp)
endif()

add_executable(native_tests ${CSSHARP_TEST_SOURCES} ${CSSHARP_TESTED_SOURCES})
target_link_libraries(native_tests PRIVATE zydis)

target_include_directories(native_tests PRIVATE
    ${CSSHARP_ROOT}/src
    ${CSSHARP_ROOT}/libraries/doctest
    ${CSSHARP_ROOT}/libraries # vendored single headers only (nlohmann/json.hpp); no submodules
    ${CSSHARP_GENERATOR_DIR}
)

target_compile_definitions(native_tests PRIVATE
    CSSHARP_GAMEDATA_PATH="${CSSHARP_ROOT}/configs/addons/counterstrikesharp/gamedata/gamedata.json"
)

add_test(NAME native_tests COMMAND native_tests)

if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
    add_executable(gamedata-generator
        ${CSSHARP_GENERATOR_DIR}/main.cpp
        ${CSSHARP_GENERATOR_DIR}/build_archive.cpp
        ${CSSHARP_GENERATOR_DIR}/generator.cpp
        ${CSSHARP_ROOT}/src/core/binding_resolver.cpp
        ${CSSHARP_ROOT}/src/core/signature.cpp
        ${CSSHARP_ROOT}/src/core/rtti_vtable.cpp
        ${CSSHARP_ROOT}/src/core/vtable_fingerprint.cpp
        ${CSSHARP_ROOT}/src/core/elf_relocations.cpp
    )
    target_link_libraries(gamedata-generator PRIVATE zydis)
    target_include_directories(gamedata-generator PRIVATE ${CSSHARP_ROOT}/src ${CSSHARP_ROOT}/libraries ${CSSHARP_GENERATOR_DIR})
endif()
