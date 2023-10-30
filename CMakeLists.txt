cmake_minimum_required(VERSION 3.18)

Project(counterstrikesharp C CXX)

include("makefiles/shared.cmake")

add_subdirectory(libraries/spdlog)
add_subdirectory(libraries/dyncall)
add_subdirectory(libraries/funchook)

set_property(TARGET funchook-static PROPERTY POSITION_INDEPENDENT_CODE ON)

SET(SOURCE_FILES
        src/mm_plugin.cpp
        src/mm_plugin.h
        libraries/hl2sdk-cs2/tier1/convar.cpp
        libraries/hl2sdk-cs2/tier1/generichash.cpp
        libraries/hl2sdk-cs2/entity2/entitysystem.cpp
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
        src/scripting/autonative.h
        src/scripting/natives/natives_engine.cpp
        src/core/engine_trace.h
        src/core/engine_trace.cpp
        src/scripting/natives/natives_callbacks.cpp
        src/core/managers/player_manager.h
        src/core/managers/player_manager.cpp
        src/scripting/natives/natives_vector.cpp
        src/scripting/natives/natives_timers.cpp
        src/utils/virtual.h
        src/scripting/natives/natives_events.cpp
        src/core/memory.cpp
        src/core/memory.h
        src/core/managers/con_command_manager.cpp
        src/core/managers/con_command_manager.h
        src/scripting/natives/natives_commands.cpp
        src/core/memory_module.h
        src/core/cs2_sdk/interfaces/cgameresourceserviceserver.h
        src/core/cs2_sdk/interfaces/cschemasystem.h
        src/core/cs2_sdk/interfaces/cs2_interfaces.h
        src/core/cs2_sdk/interfaces/cs2_interfaces.cpp
        src/core/cs2_sdk/schema.h
        src/core/cs2_sdk/schema.cpp
        src/core/function.cpp
        src/core/function.h
        src/scripting/natives/natives_memory.cpp
        src/scripting/natives/natives_schema.cpp
        src/scripting/natives/natives_entities.cpp
        src/core/managers/entity_manager.cpp
        src/core/managers/entity_manager.h
        src/core/managers/chat_manager.cpp
        src/core/managers/chat_manager.h
        src/core/managers/client_command_manager.cpp
        src/core/managers/client_command_manager.h
)

set(PROTO_DIRS -I${CMAKE_CURRENT_SOURCE_DIR}/libraries/GameTracking-CS2/Protobufs)
file(GLOB PROTOS "${CMAKE_CURRENT_SOURCE_DIR}/libraries/GameTracking-CS2/Protobufs/*.proto")

## Generate protobuf source & headers
#add_custom_command(
#        OUTPUT protobuf_output_stamp
#        COMMAND bash ${CMAKE_CURRENT_SOURCE_DIR}/src/protobuf/compile.sh
#        COMMENT "Generating protobuf files using compile.sh script"
#        WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/src/protobuf
#        DEPENDS ${CMAKE_CURRENT_SOURCE_DIR}/src/protobuf/compile.sh
#        VERBATIM
#)
#
#SET(SOURCE_FILES ${SOURCE_FILES} protobuf_output_stamp)

# Sources
add_library(${PROJECT_NAME} SHARED ${SOURCE_FILES} ${NATIVES_SOURCES} ${CONVERSIONS_SOURCES} ${CONVERSIONS_HEADERS})

target_include_directories(
        ${PROJECT_NAME}
        PUBLIC
        ${CMAKE_CURRENT_SOURCE_DIR}/src
        ${CMAKE_CURRENT_SOURCE_DIR}/src/core/cs2_sdk
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
configure_file(configs/gamedata.json "${CMAKE_BINARY_DIR}/addons/counterstrikesharp/gamedata/gamedata.json" COPYONLY)
