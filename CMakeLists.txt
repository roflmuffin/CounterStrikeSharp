cmake_minimum_required(VERSION 3.18)

Project(counterstrikesharp C CXX)

include("makefiles/shared.cmake")

add_subdirectory(libraries/spdlog)

SET(SOURCE_FILES
        src/sample_mm.cpp
        src/sample_mm.h
        libraries/hl2sdk-cs2/tier1/convar.cpp
        libraries/hl2sdk-cs2/public/tier0/memoverride.cpp
        libraries/dotnet/hostfxr.h
        libraries/dotnet/coreclr_delegates.h
        "libraries/metamod-source/core/sourcehook/sourcehook.cpp"
        "libraries/metamod-source/core/sourcehook/sourcehook_impl_chookidman.cpp"
        "libraries/metamod-source/core/sourcehook/sourcehook_impl_chookmaninfo.cpp"
        "libraries/metamod-source/core/sourcehook/sourcehook_impl_cvfnptr.cpp"
        "libraries/metamod-source/core/sourcehook/sourcehook_impl_cproto.cpp"
        src/scripting/dotnet_host.h
        src/scripting/dotnet_host.cpp
        src/core/utils.h
        src/core/globals.h
        src/core/globals.cpp
        src/core/log.h
        src/core/log.cpp
        src/scripting/script_engine.h
        src/scripting/script_engine.cpp
        src/core/global_listener.h
        src/scripting/callback_manager.h
        src/scripting/callback_manager.cpp
        src/core/managers/event_manager.h
        src/core/managers/event_manager.cpp
        src/core/timer_system.h
        src/core/timer_system.cpp
)


# Sources
add_library(${PROJECT_NAME} MODULE ${SOURCE_FILES} ${NATIVES_SOURCES} ${CONVERSIONS_SOURCES} ${CONVERSIONS_HEADERS})

target_include_directories(
        ${PROJECT_NAME}
        PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}/src
)

include("makefiles/linux.base.cmake")


# Libraries
target_link_libraries(${PROJECT_NAME} ${COUNTER_STRIKE_SHARP_LINK_LIBRARIES})
set_target_properties(${PROJECT_NAME} PROPERTIES
        PREFIX ""
        LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/addons/counterstrikesharp/bin/linuxsteamrt64"
)
add_custom_target(build-time-make-directory ALL
        COMMAND ${CMAKE_COMMAND} -E make_directory "${CMAKE_BINARY_DIR}/addons/metamod")
configure_file(configs/counterstrikesharp.vdf "${CMAKE_BINARY_DIR}/addons/metamod/counterstrikesharp.vdf" COPYONLY)