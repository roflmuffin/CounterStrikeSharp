# Credit for this protobuf generation cmake file goes to Poggicek.
# Based on their work at https://github.com/Poggicek/StickerInspect

set(PROTO_TARGETS
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/network_connection.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/networkbasetypes.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/cs_gameevents.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/engine_gcmessages.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/gcsdk_gcmessages.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/cstrike15_gcmessages.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/cstrike15_usermessages.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/netmessages.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/steammessages.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/usermessages.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/gameevents.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/clientmessages.proto
    ${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo/te.proto
)

if(UNIX)
    set(PROTOC_EXECUTABLE ${PROJECT_SOURCE_DIR}/libraries/hl2sdk-cs2/devtools/bin/linux/protoc)
elseif(WIN32)
    set(PROTOC_EXECUTABLE ${PROJECT_SOURCE_DIR}/libraries/hl2sdk-cs2/devtools/bin/protoc.exe)
endif()

foreach(PROTO_TARGET ${PROTO_TARGETS})
    get_filename_component(PROTO_FILENAME ${PROTO_TARGET} NAME_WLE)
    list(APPEND PROTO_OUTPUT ${PROTO_FILENAME}.pb.cc ${PROTO_FILENAME}.pb.h)
    list(APPEND PROTO_INPUT ${PROTO_FILENAME}.proto)
    get_filename_component(PROTO_PATH ${PROTO_TARGET} DIRECTORY)
    list(APPEND PROTO_PATHS "--proto_path=${PROTO_PATH}")
endforeach()

list(REMOVE_DUPLICATES PROTO_PATHS)
list(TRANSFORM PROTO_OUTPUT PREPEND ${CMAKE_CURRENT_BINARY_DIR}/protobufcompiler/)
file(MAKE_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}/protobufcompiler)

add_custom_command(
    OUTPUT ${PROTO_OUTPUT}
    COMMAND "${PROTOC_EXECUTABLE}" -I ${PROJECT_SOURCE_DIR}/libraries/hl2sdk-cs2/thirdparty/protobuf-3.21.8/src --proto_path=${PROJECT_SOURCE_DIR}/libraries/Protobufs/csgo ${PROTO_PATHS} --cpp_out=${CMAKE_CURRENT_BINARY_DIR}/protobufcompiler ${PROTO_INPUT}
    COMMENT "Generating protobuf file"
)

add_library(Protobufs STATIC
            ${PROTO_OUTPUT}
)

target_include_directories(Protobufs
                           PUBLIC ${CMAKE_CURRENT_BINARY_DIR}/protobufcompiler
                           PUBLIC ${PROJECT_SOURCE_DIR}/libraries/hl2sdk-cs2/thirdparty/protobuf-3.21.8/src
)

if(WIN32)
    target_link_libraries(Protobufs PUBLIC ${PROJECT_SOURCE_DIR}/libraries/hl2sdk-cs2/lib/public/win64/2015/libprotobuf.lib)
elseif(UNIX)
    target_link_libraries(Protobufs PUBLIC ${PROJECT_SOURCE_DIR}/libraries/hl2sdk-cs2/lib/linux64/release/libprotobuf.a)
endif()
set_target_properties(Protobufs PROPERTIES LINKER_LANGUAGE CXX)
set_target_properties(Protobufs PROPERTIES FOLDER SDK)
