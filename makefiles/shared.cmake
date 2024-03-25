if (UNIX AND NOT APPLE)
    set(LINUX TRUE)
endif()

if (WIN32 AND NOT MSVC)
    message(FATAL "MSVC restricted.")
endif()

set(CMAKE_CONFIGURATION_TYPES "Debug;Release" CACHE STRING
    "Only do Release and Debug"
    FORCE
)

# TODO: Use C++20 instead.
set(CMAKE_CXX_STANDARD 17)

if (LINUX)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fPIC")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fPIC")
endif()

set(CMAKE_STATIC_LIBRARY_PREFIX "")

set(SOURCESDK_DIR ${CMAKE_CURRENT_SOURCE_DIR}/libraries/hl2sdk-cs2)
set(METAMOD_DIR ${CMAKE_CURRENT_SOURCE_DIR}/libraries/metamod-source)

set(SOURCESDK ${SOURCESDK_DIR}/${BRANCH})
set(SOURCESDK_LIB ${SOURCESDK}/lib)

add_definitions(-DMETA_IS_SOURCE2)

if(DEFINED ENV{GITHUB_SHA_SHORT})
    add_definitions(-DGITHUB_SHA="$ENV{GITHUB_SHA_SHORT}")
else()
    add_definitions(-DGITHUB_SHA="Local")
endif()

if(DEFINED ENV{BUILD_NUMBER})
    add_definitions(-DBUILD_NUMBER="$ENV{BUILD_NUMBER}")
else()
    add_definitions(-DBUILD_NUMBER="0")
endif()

include_directories(
    ${SOURCESDK}
    ${SOURCESDK}/thirdparty/protobuf-3.21.8/src
    ${SOURCESDK}/common
    ${SOURCESDK}/game/shared
    ${SOURCESDK}/game/server
    ${SOURCESDK}/public
    ${SOURCESDK}/public/engine
    ${SOURCESDK}/public/mathlib
    ${SOURCESDK}/public/tier0
    ${SOURCESDK}/public/tier1
    ${SOURCESDK}/public/entity2
    ${SOURCESDK}/public/game/server
    ${SOURCESDK}/public/entity2
    ${METAMOD_DIR}/core
    ${METAMOD_DIR}/core/sourcehook
    libraries/dyncall/dynload
    libraries/dyncall/dyncall
    libraries/spdlog/include
    libraries/tl
    libraries/funchook/include
    libraries/DynoHook/src
    libraries/moodycamel
    libraries
)

include(${CMAKE_CURRENT_LIST_DIR}/metamod/configure_metamod.cmake)