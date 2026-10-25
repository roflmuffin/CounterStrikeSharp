using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CodeGen.CsScript;

internal static class Program
{
    private const string SourceUrl = "https://raw.githubusercontent.com/SteamTracking/GameTracking-CS2/refs/heads/master/content/csgo_addons/cs_script_demo/maps/scripts/point_script.d.ts";

    private static async Task<int> Main(string[] args)
    {
        if (args is ["--help"])
        {
            Console.WriteLine("dotnet run --project tooling/CodeGen.CsScript -- [--check]");
            Console.WriteLine("Always downloads the current point_script.d.ts. --check compares outputs without writing them.");
            return 0;
        }
        if (args.Length > 1 || args.Length == 1 && args[0] != "--check")
        {
            Console.Error.WriteLine("Unknown arguments. Use --help for usage.");
            return 1;
        }
        try
        {
            var root = FindRepositoryRoot();
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30), MaxResponseContentBufferSize = 4 * 1024 * 1024 };
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            Console.WriteLine($"Fetching {SourceUrl}");

            var source = (await client.GetStringAsync(SourceUrl)).Replace("\r\n", "\n");
            var manifest = AstManifest.Parse(source, SourceUrl);
            var schemaEnums = SchemaEnums.Resolve(manifest, Path.Combine(root, "managed/CounterStrikeSharp.API/Generated/Schema/Enums"));
            var output = Emitter.Emit(manifest, schemaEnums);
            ValidateSchemaClasses(root, manifest);
            var files = new Dictionary<string, string>
            {
                ["tooling/CodeGen.CsScript/generated/point_script.d.ts"] = source,
                ["tooling/CodeGen.CsScript/generated/manifest.json"] = JsonSerializer.Serialize(manifest, Json.Indented) + '\n',
                ["managed/CounterStrikeSharp.API/Modules/CsScript/Generated/Api.g.cs"] = output.CSharp,
                ["managed/CounterStrikeSharp.API/Modules/CsScript/Generated/Entities.g.cs"] = output.Entities,
                ["src/core/cs_script_descriptors.h"] = output.Native
            };

            if (args.Contains("--check"))
            {
                var stale = files.Keys.Where(name => !File.Exists(Path.Combine(root, name)) ||
                    File.ReadAllText(Path.Combine(root, name)).Replace("\r\n", "\n") != files[name]).ToArray();
                if (stale.Length > 0) throw new InvalidDataException("Stale outputs:\n" + string.Join('\n', stale));
            }
            else
            {
                foreach (var (name, contents) in files)
                {
                    var destination = Path.Combine(root, name);
                    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                    await File.WriteAllTextAsync(destination, contents, new UTF8Encoding(false));
                }
            }
            Console.WriteLine($"Source Git blob: {manifest.Source.GitBlobSha}");
            Console.WriteLine($"{output.Descriptors.Count} supported methods; {manifest.Diagnostics.Count} explicit diagnostics");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine($"cs_script generation failed: {error.Message}");
            return 1;
        }
    }

    private static string FindRepositoryRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "CMakeLists.txt")) && Directory.Exists(Path.Combine(dir.FullName, "managed/CounterStrikeSharp.API")))
                return dir.FullName;
        throw new DirectoryNotFoundException("Run the generator from a CounterStrikeSharp checkout.");
    }

    private static void ValidateSchemaClasses(string root, Manifest manifest)
    {
        var entityTypes = manifest.Receivers.Where(r => r.Kind == "entity").Select(r => r.ExportName)
            .Concat(manifest.Receivers.SelectMany(r => r.Methods).Where(m => m.Supported)
                .SelectMany(m => m.Parameters.Select(p => p.Shape).Append(m.Returns))
                .Where(s => s.Kind == "entity").Select(s => s.EntityType!))
            .Distinct(StringComparer.Ordinal);
        foreach (var entityType in entityTypes)
        {
            var name = Emitter.EntityClass(entityType);
            if (!File.Exists(Path.Combine(root, $"managed/CounterStrikeSharp.API/Generated/Schema/Classes/{name}.g.cs")))
                throw new InvalidDataException($"Missing generated schema class: {name} (script entity {entityType})");
        }
    }
}
