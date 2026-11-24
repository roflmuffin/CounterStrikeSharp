find_package(Git REQUIRED)

set(HL2SDK_PATCH "${CMAKE_CURRENT_LIST_DIR}/../patches/hl2sdk-cs2.patch")
set_property(DIRECTORY APPEND PROPERTY CMAKE_CONFIGURE_DEPENDS
    "${HL2SDK_PATCH}"
    "${SOURCESDK_DIR}/public/engine2/isource2server.h"
)

execute_process(
    COMMAND "${GIT_EXECUTABLE}" apply --reverse --check --ignore-space-change "${HL2SDK_PATCH}"
    WORKING_DIRECTORY "${SOURCESDK_DIR}"
    RESULT_VARIABLE HL2SDK_PATCH_RESULT
    OUTPUT_QUIET ERROR_QUIET
)

if(HL2SDK_PATCH_RESULT EQUAL 0)
    message(STATUS "hl2sdk-cs2 already contains the ISource2Server patch")
else()
    execute_process(
        COMMAND "${GIT_EXECUTABLE}" apply --ignore-space-change "${HL2SDK_PATCH}"
        WORKING_DIRECTORY "${SOURCESDK_DIR}"
        RESULT_VARIABLE HL2SDK_PATCH_RESULT
        OUTPUT_QUIET ERROR_VARIABLE HL2SDK_PATCH_ERROR
    )
    if(NOT HL2SDK_PATCH_RESULT EQUAL 0)
        message(FATAL_ERROR "Cannot apply ${HL2SDK_PATCH}:\n${HL2SDK_PATCH_ERROR}")
    endif()
    message(STATUS "Applied the ISource2Server patch")
endif()
