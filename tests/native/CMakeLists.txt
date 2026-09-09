cmake_minimum_required(VERSION 3.18)
project(css_khook_tests LANGUAGES C CXX)
enable_testing()
if(NOT CMAKE_SYSTEM_NAME STREQUAL "Linux")
    message(FATAL_ERROR "These native compatibility tests target Linux.")
endif()
set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
get_filename_component(CSS_ROOT "${CMAKE_CURRENT_SOURCE_DIR}/../.." ABSOLUTE)
set(KHOOK "${CSS_ROOT}/libraries/metamod-source/third_party/khook")
set(SAFETY "${KHOOK}/third_party/safetyhook")
# Match the pinned Metamod AMBuild source list, including its bundled Zydis.
# This test engine is never linked into the CounterStrikeSharp plugin.
add_library(test_khook SHARED
    ${KHOOK}/src/detour.cpp ${KHOOK}/src/ranges.cpp
    ${SAFETY}/src/allocator.cpp ${SAFETY}/src/easy.cpp
    ${SAFETY}/src/inline_hook.cpp ${SAFETY}/src/mid_hook.cpp
    ${SAFETY}/src/utility.cpp ${SAFETY}/src/vmt_hook.cpp
    ${SAFETY}/src/os.linux.cpp ${SAFETY}/zydis/Zydis.c)
target_include_directories(test_khook PUBLIC ${KHOOK}/include
    PRIVATE ${SAFETY}/include ${SAFETY}/zydis)
target_compile_definitions(test_khook PUBLIC KHOOK_STANDALONE PRIVATE KHOOK_EXPORT)
target_link_libraries(test_khook PUBLIC pthread)
# Keep the engine in its own DSO, as it is in Metamod. SafetyHook temporarily
# removes execute permission from the target page while installing inline hooks.
# A static Debug build can put shared libstdc++ helpers on that same page.
set_target_properties(test_khook PROPERTIES CXX_VISIBILITY_PRESET hidden
    C_VISIBILITY_PRESET hidden VISIBILITY_INLINES_HIDDEN YES)
set(BUILD_GMOCK OFF CACHE BOOL "" FORCE)
set(INSTALL_GTEST OFF CACHE BOOL "" FORCE)
add_subdirectory(${KHOOK}/third_party/googletest googletest EXCLUDE_FROM_ALL)
add_executable(khook_tests
    ${KHOOK}/test/main.cpp ${KHOOK}/test/test_functions.cpp
    ${KHOOK}/test/test_members.cpp ${KHOOK}/test/test_regressions.cpp
    ${KHOOK}/test/test_static_hook.cpp ${KHOOK}/test/test_virtuals.cpp
    ${KHOOK}/test/test_virtual_hook.cpp ${KHOOK}/test/test_ranges.cpp
    migration_tests.cpp)
target_link_libraries(khook_tests PRIVATE test_khook GTest::gtest)
if(CMAKE_CXX_COMPILER_ID STREQUAL "GNU")
    target_compile_options(khook_tests PRIVATE -fno-devirtualize)
endif()
include(GoogleTest)
# Upstream requires each detour test to run in its own process.
gtest_discover_tests(khook_tests DISCOVERY_TIMEOUT 60 PROPERTIES TIMEOUT 30)

add_executable(probe_plugin probe_plugin.cpp)
target_compile_definitions(probe_plugin PRIVATE META_NO_HL2SDK)
target_include_directories(probe_plugin PRIVATE
    ${CSS_ROOT}/libraries/metamod-source/core ${KHOOK}/include)
target_link_libraries(probe_plugin PRIVATE ${CMAKE_DL_LIBS})
