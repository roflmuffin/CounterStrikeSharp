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
# new test files in CSSHARP_TEST_SOURCES.
cmake_minimum_required(VERSION 3.18)

if(CMAKE_SOURCE_DIR STREQUAL CMAKE_CURRENT_SOURCE_DIR)
    project(counterstrikesharp_native_tests CXX)
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
)

set(CSSHARP_TEST_SOURCES
    main.cpp
    tick_scheduler_test.cpp
    signature_test.cpp
    binding_resolver_test.cpp
    engine_features_test.cpp
    real_binary_test.cpp
)

add_executable(native_tests ${CSSHARP_TEST_SOURCES} ${CSSHARP_TESTED_SOURCES})

target_include_directories(native_tests PRIVATE
    ${CSSHARP_ROOT}/src
    ${CSSHARP_ROOT}/libraries/doctest
    ${CSSHARP_ROOT}/libraries # vendored single headers only (nlohmann/json.hpp); no submodules
)

target_compile_definitions(native_tests PRIVATE
    CSSHARP_GAMEDATA_PATH="${CSSHARP_ROOT}/configs/addons/counterstrikesharp/gamedata/gamedata.json"
)

add_test(NAME native_tests COMMAND native_tests)
