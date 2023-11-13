if (WIN32)
	set(COUNTERSTRIKESHARP_VDF_PLATFORM "win64")
else()
	set(COUNTERSTRIKESHARP_VDF_PLATFORM "linuxsteamrt64")
endif()

configure_file(
	${CMAKE_CURRENT_LIST_DIR}/counterstrikesharp.vdf.in
	${PROJECT_SOURCE_DIR}/configs/addons/metamod/counterstrikesharp.vdf
)