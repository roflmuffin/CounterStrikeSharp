namespace CounterStrikeSharp.SchemaGen;

public record NewSchemaModule(
    GameInfo game_info,
    DumperInfo dumper_info,
    string[] dump_flags,
    SchemaDef[] defs);

public record GameInfo(
    string ClientVersion,
    string ServerVersion,
    string PatchVersion,
    string ProductName,
    string appID,
    string ServerAppID,
    string SourceRevision,
    string VersionDate,
    string VersionTime);

public record DumperInfo(
    string version,
    string dump_date,
    int dump_format_version);

public record SchemaDef(
    string type,
    string name,
    string? scope,
    string? project,
    int? size,
    int? alignment,
    SchemaTraits? traits);

public record SchemaTraits(
    int? parent_class_idx,
    string[]? flags,
    SchemaMetaTag[]? metatags,
    int? multi_depth,
    int? single_depth,
    SchemaBaseClass[]? baseclasses,
    SchemaMember[]? members,
    SchemaEnumField[]? fields);

public record SchemaMetaTag(
    string name,
    string? value);

public record SchemaBaseClass(
    int offset,
    int ref_idx);

public record SchemaMember(
    string name,
    int offset,
    MemberTraits? traits);

public record MemberTraits(
    SchemaMetaTag[]? metatags,
    SchemaSubtype? subtype);

public record SchemaSubtype(
    string type,
    string? name,
    int? size,
    int? alignment,
    SchemaSubtype[]? template,
    int? ref_idx,
    int? element_size,
    int? count,
    SchemaSubtype? subtype);

public record SchemaEnumField(
    string name,
    long value);
