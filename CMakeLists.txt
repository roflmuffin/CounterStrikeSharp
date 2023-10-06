cmake_minimum_required(VERSION 3.18)

Project(counterstrikesharp C CXX)

include("makefiles/shared.cmake")

SET(SOURCE_FILES
        src/sample_mm.cpp
        src/sample_mm.h
        libraries/hl2sdk-cs2/tier1/convar.cpp
        libraries/hl2sdk-cs2/public/tier0/memoverride.cpp
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