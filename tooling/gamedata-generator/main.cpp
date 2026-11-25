// gamedata-generator: regenerates Slot Bindings in Gamedata from builds in the shared build archive. Offline only.
//
//   gamedata-generator bootstrap --build <id> --key <key> --class <Class> --library <server|engine|tier0> --index <n>
//                                [--archive <dir>] [--gamedata <gamedata.json>] [--out <file>]
//   gamedata-generator carry-forward --from <id> --to <id> --gamedata <gamedata.json> [--archive <dir>] [--out <file>]
//
// bootstrap: records the Slot Binding of slot <n> of <Class>'s vtable on build <id> (its index known correct, e.g.
// from SDK headers matching that build). With --gamedata, the entry is merged into that file's <key> and the whole file
// is written (key order kept); without, only {"<key>": entry} is.
//
// carry-forward: <gamedata.json> is Gamedata for build --from; writes it for build --to with every Slot Binding moved to
// where its window matches exactly once on the new build and its window, uniqueNearby and size re-recorded there. A
// Slot Binding it cannot carry unambiguously is left unchanged and listed on stderr ("held"); review the output as a
// diff. Signatures and other entries are not touched.
//
// Output goes to --out, else stdout. Warnings and the carry-forward summary go to stderr. The archive defaults to
// $CSSHARP_BUILD_ARCHIVE, else ~/dev/cs2-builds.
//
// Exit codes: 0 done, 1 usage, 2 build or module missing from the archive / unreadable, 3 class or slot not usable,
// 4 carry-forward done but some Slot Bindings held for a human (output still written).

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
    "                                    [--archive <dir>] [--gamedata <gamedata.json>] [--out <file>]\n"
    "       gamedata-generator carry-forward --from <id> --to <id> --gamedata <gamedata.json> [--archive <dir>] [--out <file>]\n";

constexpr int kExitHeld = 4;

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

std::filesystem::path ArchiveRoot(const std::map<std::string, std::string>& options)
{
    return options.contains("archive") ? std::filesystem::path(options.at("archive")) : DefaultArchiveRoot();
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
    const auto archive = ArchiveRoot(options);

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

int CarryForwardCommand(const std::map<std::string, std::string>& options)
{
    const auto from = Required(options, "from");
    const auto to = Required(options, "to");
    const auto gamedata = ReadGamedata(Required(options, "gamedata"));
    const auto archive = ArchiveRoot(options);

    // Both builds and every module the Slot Bindings live in, up front: a missing build fails the run, not one entry.
    const auto libraries = SlotBindingLibraries(gamedata);
    std::optional<ArchivedBuild> previous;
    std::optional<ArchivedBuild> next;
    try
    {
        previous.emplace(archive, from);
        next.emplace(archive, to);
        previous->Require(libraries);
        next->Require(libraries);
    }
    catch (const std::runtime_error& error)
    {
        throw ArchiveError(error.what());
    }

    const auto result = CarryForward(gamedata, previous->Vtables(), next->Vtables());
    Write(result.gamedata, options);

    std::size_t unmoved = 0;
    for (const auto& carried : result.carried)
    {
        if (carried.from == carried.to) ++unmoved;
        else
            std::cerr << "moved " << carried.key << ": " << carried.from << " -> " << carried.to << "\n";
    }
    for (const auto& warning : result.warnings)
        std::cerr << "warning: " << warning << "\n";
    for (const auto& held : result.held)
        std::cerr << "held " << held.key << ": " << held.reason << "\n";
    std::cerr << "carried " << result.carried.size() << " Slot Binding(s) from " << from << " to " << to << " (" << unmoved << " unmoved), held "
              << result.held.size() << "\n";
    return result.held.empty() ? 0 : kExitHeld;
}

} // namespace

int main(int argc, char** argv)
{
    try
    {
        if (argc < 2) throw UsageError("missing command");
        const std::string command = argv[1];
        if (command == "bootstrap") return Bootstrap(ParseOptions(argc, argv, 2));
        if (command == "carry-forward") return CarryForwardCommand(ParseOptions(argc, argv, 2));
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
