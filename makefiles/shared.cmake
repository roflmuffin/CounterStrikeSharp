Set(CMAKE_CONFIGURATION_TYPES "Debug;Release" CACHE STRING
        "Only do Release and Debug"
        FORCE
)

Set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fPIC")
set(CMAKE_STATIC_LIBRARY_PREFIX "")

Set(SOURCESDK_DIR ${CMAKE_CURRENT_SOURCE_DIR}/libraries/hl2sdk-cs2)
Set(METAMOD_DIR ${CMAKE_CURRENT_SOURCE_DIR}/libraries/metamod-source)

Set(SOURCESDK ${SOURCESDK_DIR}/${BRANCH})
Set(SOURCESDK_LIB ${SOURCESDK}/lib)

add_definitions(-DMETA_IS_SOURCE2)

include_directories(
        ${SOURCESDK}
        ${SOURCESDK}/common
        ${SOURCESDK}/game/shared
        ${SOURCESDK}/game/server
        ${SOURCESDK}/public
        ${SOURCESDK}/public/engine
        ${SOURCESDK}/public/mathlib
        ${SOURCESDK}/public/tier0
        ${SOURCESDK}/public/tier1
        ${SOURCESDK}/public/game/server
        ${METAMOD_DIR}/core
        ${METAMOD_DIR}/core/sourcehook
        libraries/spdlog/include
        libraries/tl
        libraries
)

Project(counterstrikesharp C CXX)
