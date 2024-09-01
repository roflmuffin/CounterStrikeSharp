cmake_minimum_required(VERSION 3.18)

# You must set ASM, otherwise CMAKE_ASM_COMPILE_OBJECT will not work on MSVC
project(counterstrikesharp C CXX ASM)

include("makefiles/shared.cmake")

set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

add_subdirectory(libraries/spdlog)
add_subdirectory(libraries/dyncall)
add_subdirectory(libraries/funchook)
add_subdirectory(libraries/DynoHook)

set_property(TARGET dynohook PROPERTY DYNO_ARCH_X86 64)
set_property(TARGET funchook-static PROPERTY POSITION_INDEPENDENT_CODE ON)

include("makefiles/protobuf.cmake")

set(SOURCE_FILES
    libraries/hl2sdk-cs2/public/tier0/memoverride.cpp
    src/mm_plugin.cpp
    src/mm_plugin.h
    libraries/hl2sdk-cs2/tier1/convar.cpp
    libraries/hl2sdk-cs2/tier1/generichash.cpp
    libraries/hl2sdk-cs2/tier1/keyvalues3.cpp
    libraries/hl2sdk-cs2/entity2/entityidentity.cpp
    libraries/hl2sdk-cs2/entity2/entitysystem.cpp
    libraries/hl2sdk-cs2/entity2/entitykeyvalues.cpp
    libraries/dotnet/hostfxr.h
    libraries/dotnet/coreclr_delegates.h
    libraries/metamod-source/core/sourcehook/sourcehook.cpp
    libraries/metamod-source/core/sourcehook/sourcehook_impl_chookidman.cpp
    libraries/metamod-source/core/sourcehook/sourcehook_impl_chookmaninfo.cpp
    libraries/metamod-source/core/sourcehook/sourcehook_impl_cvfnptr.cpp
    libraries/metamod-source/core/sourcehook/sourcehook_impl_cproto.cpp
    src/scripting/dotnet_host.h
    src/scripting/dotnet_host.cpp
    src/core/utils.h
    src/core/globals.h
    src/core/globals.cpp
    src/core/coreconfig.h
    src/core/coreconfig.cpp
    src/core/gameconfig.h
    src/core/gameconfig.cpp
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
    src/core/tick_scheduler.h
    src/core/tick_scheduler.cpp
    src/scripting/autonative.h
    src/scripting/natives/natives_engine.cpp
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
    src/core/memory_module.cpp
    src/core/cs2_sdk/interfaces/cgameresourceserviceserver.h
    src/core/cs2_sdk/interfaces/cs2_interfaces.h
    src/core/cs2_sdk/interfaces/cs2_interfaces.cpp
    src/core/cs2_sdk/schema.h
    src/core/cs2_sdk/schema.cpp
    src/core/function.cpp
    src/core/function.h
    src/scripting/natives/natives_memory.cpp
    src/scripting/natives/natives_schema.cpp
    src/scripting/natives/natives_entities.cpp
    src/scripting/natives/natives_voice.cpp
    src/scripting/natives/natives_metamod.cpp
    src/scripting/natives/natives_cvariant.cpp
    src/scripting/natives/natives_cutil.cpp
    src/core/managers/entity_manager.cpp
    src/core/managers/entity_manager.h
    src/core/managers/chat_manager.cpp
    src/core/managers/chat_manager.h
    src/core/managers/server_manager.cpp
    src/core/managers/server_manager.h
    src/scripting/natives/natives_server.cpp
    src/scripting/natives/natives_usermessages.cpp
    libraries/nlohmann/json.hpp
    src/core/managers/voice_manager.cpp
    src/core/managers/voice_manager.h
    src/core/managers/usermessage_manager.cpp
    src/core/managers/usermessage_manager.h
    src/scripting/natives/natives_dynamichooks.cpp
    src/core/game_system.h
    src/core/game_system.cpp
    src/core/UserMessage.h
    src/core/UserMessage.cpp
    src/core/recipientfilters.h
)

add_library(${PROJECT_NAME} SHARED ${SOURCE_FILES} ${NATIVES_SOURCES} ${CONVERSIONS_SOURCES} ${CONVERSIONS_HEADERS})

target_include_directories(
    ${PROJECT_NAME}
    PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}/src
    ${CMAKE_CURRENT_SOURCE_DIR}/src/core/cs2_sdk
)

if(LINUX)
    include("makefiles/linux.base.cmake")
    set_target_properties(${PROJECT_NAME} PROPERTIES
                          PREFIX ""
                          LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/addons/counterstrikesharp/bin/linuxsteamrt64"
    )
elseif(WIN32)
    include("makefiles/windows.base.cmake")
    set_target_properties(${PROJECT_NAME} PROPERTIES
                          PREFIX ""
                          LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/addons/counterstrikesharp/bin/win64"
    )
endif()

# Libraries
target_link_libraries(${PROJECT_NAME} ${COUNTER_STRIKE_SHARP_LINK_LIBRARIES} Protobufs)

add_custom_command(
    TARGET ${PROJECT_NAME} PRE_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_directory
    ${CMAKE_SOURCE_DIR}/configs ${CMAKE_BINARY_DIR}
)
