#include "core/gameconfig_updater.h"
#include "core/utils.h"
#include "core/coreconfig.h"
#include "core/log.h"
#include "core/globals.h"

#include <string>
#include <fstream>
#include <filesystem>

#include "httplib/httplib.h"

namespace counterstrikesharp::update {

/// @brief
/// Attempts to update the game configuration by checking the ETag of the remote gamedata.json file.
/// If the ETag has changed, it downloads the new gamedata.json and updates the local
/// etag file with the new ETag.
/// @return
bool TryUpdateGameConfig()
{
    CSSHARP_CORE_INFO("AutoUpdate enabled, checking for gamedata updates from {}", globals::coreConfig->AutoUpdateURL);

    auto gamedata_path = std::string(utils::GamedataDirectory() + "/gamedata.json");
    auto etag_path = std::string(utils::GamedataDirectory() + "/gamedata.etag");

    httplib::Client client(globals::coreConfig->AutoUpdateURL);
    client.set_follow_location(true);
    client.set_read_timeout(10, 0);

    auto headRes = client.Head("/");
    if (!headRes || headRes->status != 200)
    {
        return false;
    }

    auto etag = headRes->get_header_value("ETag");
    std::string localETag = "";
    if (std::filesystem::exists(etag_path))
    {
        std::ifstream etag_file(etag_path);
        if (etag_file.is_open())
        {
            std::getline(etag_file, localETag);
            etag_file.close();
        }
    }

    if (etag == localETag)
    {
        CSSHARP_CORE_INFO("Gamedata is up to date, ETag: {}", etag);
        return true;
    }

    auto res = client.Get("/");
    if (res && res->status == 200)
    {
        auto etag = res->get_header_value("ETag");

        std::ofstream gamedata_file(gamedata_path);
        if (gamedata_file.is_open())
        {
            gamedata_file << res->body;
            gamedata_file.close();
            CSSHARP_CORE_INFO("Gamedata file written to: {} with ETag {}", gamedata_path, res->get_header_value("ETag"));
        }

        std::ofstream etag_file_out(etag_path);
        if (etag_file_out.is_open())
        {
            etag_file_out << etag;
            etag_file_out.close();
        }
    }
    else
    {
        CSSHARP_CORE_ERROR("Failed to connect to the auto update server at: {}", globals::coreConfig->AutoUpdateURL);
        return false;
    }

    return true;
}
} // namespace counterstrikesharp::update
