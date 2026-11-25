// gamedata-generator: regenerates Slot Bindings in Gamedata from builds in the shared build archive. Offline only.
//
//   gamedata-generator bootstrap --build <id> --key <key> --class <Class> --library <server|engine|tier0> --index <n>
//                                [--archive <dir>] [--gamedata <gamedata.json>] [--out <file>]
//
// bootstrap: records the Slot Binding of slot <n> of <Class>'s vtable on build <id> (its index known correct, e.g.
// from SDK headers matching that build). With --gamedata, the entry is merged into that file's <key> and the whole file
// is written (key order kept); without, only {"<key>": entry} is. Output goes to --out, else stdout. Warnings (a
// window that also matches nearby, a pure virtual) go to stderr. The archive defaults to $CSSHARP_BUILD_ARCHIVE, else
// ~/dev/cs2-builds.
//
// Exit codes: 0 done, 1 usage, 2 build or module missing from the archive / unreadable, 3 class or slot not usable.

#include <cstdio>
#include <fstream>
#include <iostream>
#include <map>
#include <stdexcept>
#include <string>

#include "build_archive.h"
#include "generator.h"

using namespace counterstrikesharp::generator;

namespace {

constexpr const char* kUsage =
    "usage: gamedata-generator bootstrap --build <id> --key <key> --class <Class> --library <server|engine|tier0> --index <n>\n"
    "                                    [--archive <dir>] [--gamedata <gamedata.json>] [--out <file>]\n";

struct UsageError : std::runtime_error
{
    using std::runtime_error::runtime_error;
};
struct ArchiveError : std::runtime_error
{
    using std::runtime_error::runtime_error;
};

std::map<std::string, std::string> ParseOptions(int argc, char** argv, int first)
{
    std::map<std::string, std::string> options;
    for (int i = first; i < argc; i += 2)
    {
        const std::string name = argv[i];
        if (name.rfind("--", 0) != 0 || i + 1 >= argc) throw UsageError("expected --option value, got " + name);
        options[name.substr(2)] = argv[i + 1];
    }
    return options;
}

std::string Required(const std::map<std::string, std::string>& options, const std::string& name)
{
    const auto it = options.find(name);
    if (it == options.end() || it->second.empty()) throw UsageError("missing --" + name);
    return it->second;
}

GamedataJson ReadGamedata(const std::string& path)
{
    std::ifstream stream(path);
    if (!stream) throw ArchiveError("cannot read " + path);
    return GamedataJson::parse(stream);
}

void Write(const GamedataJson& json, const std::map<std::string, std::string>& options)
{
    const auto text = json.dump(2) + "\n";
    if (const auto out = options.find("out"); out != options.end())
    {
        std::ofstream stream(out->second);
        if (!stream) throw ArchiveError("cannot write " + out->second);
        stream << text;
    }
    else
        std::cout << text;
}

int Bootstrap(const std::map<std::string, std::string>& options)
{
    SlotBindingRequest request;
    request.key = Required(options, "key");
    request.className = Required(options, "class");
    request.library = Required(options, "library");
    const auto build = Required(options, "build");
    try
    {
        request.index = std::stoul(Required(options, "index"));
    }
    catch (const std::logic_error&)
    {
        throw UsageError("--index must be a slot index");
    }
    const auto archive = options.contains("archive") ? std::filesystem::path(options.at("archive")) : DefaultArchiveRoot();

    GamedataJson gamedata = options.contains("gamedata") ? ReadGamedata(options.at("gamedata")) : GamedataJson::object();

    std::optional<LoadedModule> module;
    try
    {
        module.emplace(LoadedModule::FromArchive(archive, build, request.library));
    }
    catch (const std::runtime_error& error)
    {
        throw ArchiveError(error.what());
    }

    const auto generated =
        BootstrapSlotBinding(module->Fingerprints(request.className), request, gamedata.value(request.key, GamedataJson::object()));
    for (const auto& warning : generated.warnings)
        std::cerr << "warning: " << warning << "\n";

    if (options.contains("gamedata")) gamedata[request.key] = generated.entry;
    else
        gamedata = GamedataJson{ { request.key, generated.entry } };
    Write(gamedata, options);
    return 0;
}

} // namespace

int main(int argc, char** argv)
{
    try
    {
        if (argc < 2) throw UsageError("missing command");
        const std::string command = argv[1];
        if (command == "bootstrap") return Bootstrap(ParseOptions(argc, argv, 2));
        throw UsageError("unknown command " + command);
    }
    catch (const UsageError& error)
    {
        std::cerr << "gamedata-generator: " << error.what() << "\n" << kUsage;
        return 1;
    }
    catch (const ArchiveError& error)
    {
        std::cerr << "gamedata-generator: " << error.what() << "\n";
        return 2;
    }
    catch (const std::exception& error)
    {
        std::cerr << "gamedata-generator: " << error.what() << "\n";
        return 3;
    }
}
