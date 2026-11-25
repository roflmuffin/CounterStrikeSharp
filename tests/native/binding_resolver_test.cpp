#include <doctest.h>

#include <cstdint>
#include <string>
#include <vector>

#include "core/binding_resolver.h"

using namespace counterstrikesharp::bindings;
using nlohmann::json;

namespace {
constexpr std::uintptr_t kServerBase = 0x10000;

// A synthetic module: owns its segment bytes and hands out an image over them.
struct FakeModule
{
    std::vector<std::vector<std::uint8_t>> segments;
    std::uintptr_t base = kServerBase;

    ModuleImage Image() const
    {
        ModuleImage image;
        std::uintptr_t address = base;
        for (const auto& bytes : segments)
        {
            image.segments.push_back({ address, bytes.data(), bytes.size() });
            address += 0x1000;
        }
        return image;
    }
};

json SignatureEntry(const std::string& linuxPattern, const std::string& library = "server")
{
    return { { "signatures", { { "library", library }, { "linux", linuxPattern }, { "windows", "00" } } } };
}

BindingResult ResolveOne(const json& gamedata, const ModuleImages& modules, const BindingDeclaration& declaration)
{
    const std::vector<BindingDeclaration> declarations{ declaration };
    auto report = ResolveBindings("test-build", ParseGamedata(gamedata, "linux"), modules, declarations);
    REQUIRE(report.Bindings().size() == 1);
    return report.Bindings()[0];
}

BindingResult ResolveSignature(const std::string& pattern, const FakeModule& server)
{
    return ResolveOne({ { "Host_Say", SignatureEntry(pattern) } }, { { "server", server.Image() } },
                      { "Host_Say", BindingKind::Signature });
}
} // namespace

TEST_CASE("A signature that matches exactly once resolves to its address")
{
    FakeModule server{ { { 0x90, 0x55, 0x48, 0x89, 0xE5, 0x90 } } };

    auto result = ResolveSignature("55 48 ? E5", server);

    CHECK(result.Resolved());
    CHECK(result.reason == FailureReason::None);
    CHECK(result.matchCount == 1);
    CHECK(result.address == kServerBase + 1);
}

TEST_CASE("A signature with no match fails as not found")
{
    FakeModule server{ { { 0x90, 0x90, 0x90 } } };

    auto result = ResolveSignature("55 48", server);

    CHECK_FALSE(result.Resolved());
    CHECK(result.reason == FailureReason::NotFound);
    CHECK(result.matchCount == 0);
    CHECK(result.address == 0);
}

TEST_CASE("A signature that matches twice fails as ambiguous with the count, counted across segments")
{
    SUBCASE("within one segment")
    {
        FakeModule server{ { { 0x55, 0x48, 0x90, 0x55, 0x48 } } };
        auto result = ResolveSignature("55 48", server);
        CHECK(result.reason == FailureReason::Ambiguous);
        CHECK(result.matchCount == 2);
        CHECK(result.address == 0);
    }
    SUBCASE("one match in each of two segments")
    {
        FakeModule server{ { { 0x55, 0x48 }, { 0x90, 0x55, 0x48 } } };
        auto result = ResolveSignature("55 48", server);
        CHECK(result.reason == FailureReason::Ambiguous);
        CHECK(result.matchCount == 2);
    }
    SUBCASE("overlapping matches count separately")
    {
        FakeModule server{ { { 0xAA, 0xAA, 0xAA } } };
        auto result = ResolveSignature("AA AA", server);
        CHECK(result.reason == FailureReason::Ambiguous);
        CHECK(result.matchCount == 2);
    }
}

TEST_CASE("A signature matching at the final position of a segment resolves")
{
    FakeModule server{ { { 0x90, 0x90 }, { 0x90, 0x90, 0x55, 0x48 } } };

    auto result = ResolveSignature("55 48", server);

    CHECK(result.Resolved());
    CHECK(result.address == kServerBase + 0x1000 + 2);
}

TEST_CASE("A signature with invalid hex fails as invalid pattern")
{
    FakeModule server{ { { 0x55, 0x48 } } };

    CHECK(ResolveSignature("55 GG", server).reason == FailureReason::InvalidPattern);
    CHECK(ResolveSignature("", server).reason == FailureReason::InvalidPattern);
    CHECK(ResolveSignature("@", server).reason == FailureReason::InvalidPattern);
}

TEST_CASE("A Binding whose key is missing from Gamedata fails as missing from Gamedata")
{
    FakeModule server{ { { 0x55, 0x48 } } };
    const ModuleImages modules{ { "server", server.Image() } };

    SUBCASE("key absent")
    {
        auto result = ResolveOne({ { "Other", SignatureEntry("55 48") } }, modules, { "Host_Say", BindingKind::Signature });
        CHECK(result.reason == FailureReason::MissingFromGamedata);
    }
    SUBCASE("key present without a signatures section")
    {
        auto result =
            ResolveOne({ { "Host_Say", { { "offsets", { { "linux", 1 } } } } } }, modules, { "Host_Say", BindingKind::Signature });
        CHECK(result.reason == FailureReason::MissingFromGamedata);
    }
    SUBCASE("signature without a library")
    {
        auto result =
            ResolveOne({ { "Host_Say", { { "signatures", { { "linux", "55 48" } } } } } }, modules, { "Host_Say", BindingKind::Signature });
        CHECK(result.reason == FailureReason::MissingFromGamedata);
    }
}

TEST_CASE("A Binding without an entry for this OS fails as missing for this OS")
{
    FakeModule server{ { { 0x55, 0x48 } } };
    const json gamedata = { { "Host_Say", { { "signatures", { { "library", "server" }, { "windows", "55 48" } } } } },
                            { "Slot", { { "offsets", { { "windows", 7 } } } } } };

    CHECK(ResolveOne(gamedata, { { "server", server.Image() } }, { "Host_Say", BindingKind::Signature }).reason ==
          FailureReason::MissingForThisOs);
    CHECK(ResolveOne(gamedata, {}, { "Slot", BindingKind::Offset }).reason == FailureReason::MissingForThisOs);
}

TEST_CASE("Signatures are scanned in the module named by the Gamedata library field")
{
    FakeModule server{ { { 0x55, 0x48 } } };
    FakeModule engine{ { { 0x90, 0x55, 0x48 } }, 0x80000 };
    const ModuleImages modules{ { "server", server.Image() }, { "engine", engine.Image() } };

    auto inEngine = ResolveOne({ { "Sig", SignatureEntry("55 48", "engine") } }, modules, { "Sig", BindingKind::Signature });
    CHECK(inEngine.Resolved());
    CHECK(inEngine.library == "engine");
    CHECK(inEngine.address == 0x80000 + 1);

    auto unknownLibrary = ResolveOne({ { "Sig", SignatureEntry("55 48", "nosuchlib") } }, modules, { "Sig", BindingKind::Signature });
    CHECK(unknownLibrary.reason == FailureReason::NotFound);
    CHECK(unknownLibrary.library == "nosuchlib");
}

TEST_CASE("An offset resolves to its value for this OS")
{
    const json gamedata = { { "Slot", { { "offsets", { { "linux", 42 }, { "windows", 41 } } } } } };

    auto result = ResolveOne(gamedata, {}, { "Slot", BindingKind::Offset });

    CHECK(result.Resolved());
    CHECK(result.offset == 42);
    CHECK(ResolveOne(gamedata, {}, { "Missing", BindingKind::Offset }).reason == FailureReason::MissingFromGamedata);
}

TEST_CASE("A symbol signature resolves through the module's symbol lookup")
{
    ModuleImage server;
    server.findSymbol = [](std::string_view name) -> std::uintptr_t {
        return name == "Host_Say" ? 0x1234 : 0;
    };
    const ModuleImages modules{ { "server", server } };

    auto found = ResolveOne({ { "Sig", SignatureEntry("@Host_Say") } }, modules, { "Sig", BindingKind::Signature });
    CHECK(found.Resolved());
    CHECK(found.address == 0x1234);

    auto missing = ResolveOne({ { "Sig", SignatureEntry("@Nope") } }, modules, { "Sig", BindingKind::Signature });
    CHECK(missing.reason == FailureReason::NotFound);
}

TEST_CASE("An RTTI vtable Binding resolves through the declared library's vtable lookup")
{
    ModuleImage server;
    server.findVirtualTable = [](std::string_view name) -> counterstrikesharp::rtti::VtableLookup {
        if (name == "CGameEventManager") return { 0x5000, 1 };
        if (name == "CTwice") return { 0, 2 };
        return {};
    };
    const ModuleImages modules{ { "server", server } };

    auto found = ResolveOne({}, modules, { "CGameEventManager", BindingKind::RttiVtable, "server" });
    CHECK(found.Resolved());
    CHECK(found.address == 0x5000);
    CHECK(found.matchCount == 1);

    CHECK(ResolveOne({}, modules, { "CNope", BindingKind::RttiVtable, "server" }).reason == FailureReason::RttiVtableNotFound);
    CHECK(ResolveOne({}, {}, { "CGameEventManager", BindingKind::RttiVtable, "server" }).reason == FailureReason::RttiVtableNotFound);

    auto twice = ResolveOne({}, modules, { "CTwice", BindingKind::RttiVtable, "server" });
    CHECK_FALSE(twice.Resolved());
    CHECK(twice.reason == FailureReason::Ambiguous);
    CHECK(twice.matchCount == 2);
    CHECK(twice.address == 0);
}

TEST_CASE("A schema field Binding resolves to the field offset through the declared library's schema lookup")
{
    ModuleImage server;
    server.findSchemaField = [](std::string_view className, std::string_view fieldName) -> SchemaFieldLookup {
        if (className != "CBaseEntity") return { SchemaFieldStatus::ClassMissing };
        if (fieldName != "m_iTeamNum") return { SchemaFieldStatus::FieldMissing };
        return { SchemaFieldStatus::Found, 0x3E3 };
    };
    const ModuleImages modules{ { "server", server } };

    auto found = ResolveOne({}, modules, { "CBaseEntity::m_iTeamNum", BindingKind::SchemaField, "server" });
    CHECK(found.Resolved());
    CHECK(found.reason == FailureReason::None);
    CHECK(found.offset == 0x3E3);

    auto noField = ResolveOne({}, modules, { "CBaseEntity::m_nope", BindingKind::SchemaField, "server" });
    CHECK_FALSE(noField.Resolved());
    CHECK(noField.reason == FailureReason::SchemaFieldMissing);

    auto noClass = ResolveOne({}, modules, { "CNope::m_iTeamNum", BindingKind::SchemaField, "server" });
    CHECK_FALSE(noClass.Resolved());
    CHECK(noClass.reason == FailureReason::SchemaClassMissing);

    SUBCASE("a module without a schema lookup, or not loaded, has no classes")
    {
        CHECK(ResolveOne({}, { { "server", ModuleImage{} } }, { "CBaseEntity::m_iTeamNum", BindingKind::SchemaField, "server" }).reason ==
              FailureReason::SchemaClassMissing);
        CHECK(ResolveOne({}, {}, { "CBaseEntity::m_iTeamNum", BindingKind::SchemaField, "server" }).reason ==
              FailureReason::SchemaClassMissing);
    }

    SUBCASE("a key without a field part names no field")
    {
        CHECK(ResolveOne({}, modules, { "CBaseEntity", BindingKind::SchemaField, "server" }).reason == FailureReason::SchemaFieldMissing);
    }
}

TEST_CASE("The Resolution Report records the build id and one result per declaration, findable by key")
{
    FakeModule server{ { { 0x55, 0x48 } } };
    const std::vector<BindingDeclaration> declarations{ { "Host_Say", BindingKind::Signature }, { "Slot", BindingKind::Offset } };
    const json gamedata = { { "Host_Say", SignatureEntry("55 48") } };

    auto report = ResolveBindings("1.42.0.1/2000950", ParseGamedata(gamedata, "linux"), { { "server", server.Image() } }, declarations);

    CHECK(report.BuildId() == "1.42.0.1/2000950");
    REQUIRE(report.Bindings().size() == 2);
    REQUIRE(report.Find("Host_Say") != nullptr);
    CHECK(report.Find("Host_Say")->Resolved());
    REQUIRE(report.Find("Slot") != nullptr);
    CHECK(report.Find("Slot")->reason == FailureReason::MissingFromGamedata);
    CHECK(report.Find("Nope") == nullptr);
}

TEST_CASE("ParseGamedata ignores entries it cannot read instead of throwing")
{
    const json gamedata = { { "A", 5 },
                            { "B", { { "signatures", "oops" } } },
                            { "C", { { "signatures", { { "library", 1 }, { "linux", 2 } } }, { "offsets", { { "linux", "x" } } } } } };

    Gamedata parsed;
    CHECK_NOTHROW(parsed = ParseGamedata(gamedata, "linux"));
    CHECK_FALSE(parsed.at("A").hasSignatures);
    CHECK_FALSE(parsed.at("B").hasSignatures);
    CHECK(parsed.at("C").hasSignatures);
    CHECK_FALSE(parsed.at("C").library.has_value());
    CHECK_FALSE(parsed.at("C").signature.has_value());
    CHECK(parsed.at("C").hasOffsets);
    CHECK_FALSE(parsed.at("C").offset.has_value());
    CHECK(ParseGamedata(json::array(), "linux").empty());
}

TEST_CASE("ScanSignature counts every match of a raw signature across segments and reports the first")
{
    FakeModule server{ { { 0x55, 0x48, 0x90 }, { 0x90, 0x55, 0x48 } } };

    SUBCASE("ambiguous")
    {
        auto scan = ScanSignature(server.Image(), "55 48");
        CHECK(scan.validPattern);
        CHECK(scan.matchCount == 2);
        CHECK(scan.firstMatch == kServerBase);
    }
    SUBCASE("code style")
    {
        auto scan = ScanSignature(server.Image(), R"(\x55\x48\x90)");
        CHECK(scan.validPattern);
        CHECK(scan.matchCount == 1);
        CHECK(scan.firstMatch == kServerBase);
    }
    SUBCASE("not found")
    {
        auto scan = ScanSignature(server.Image(), "AA BB");
        CHECK(scan.validPattern);
        CHECK(scan.matchCount == 0);
        CHECK(scan.firstMatch == 0);
    }
    SUBCASE("invalid hex")
    {
        auto scan = ScanSignature(server.Image(), "55 ZZ");
        CHECK_FALSE(scan.validPattern);
        CHECK(scan.matchCount == 0);
    }
}
