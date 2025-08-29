using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace CounterStrikeSharp.API.Core.Plugin.Host;

public class PluginContextNuGetDependencyResolver : IPluginContextDependencyResolver
{
    private const string NuGetPackagesEnvName = "NUGET_PACKAGES";

    private readonly string _rootAssemblyName;
    private readonly string _rootAssemblyPath;
    private readonly AssemblyName _assemblyName;

    public PluginContextNuGetDependencyResolver(string rootAssemblyName,
        string rootAssemblyPath,
        AssemblyName assemblyName)
    {
        _rootAssemblyName = rootAssemblyName;
        _rootAssemblyPath = rootAssemblyPath;
        _assemblyName = assemblyName;
    }

    public string? ResolvePath()
    {
        var packagesRoot = GetNuGetPackagesRoot();
        if (string.IsNullOrWhiteSpace(packagesRoot))
        {
            return null;
        }

        var packageName = _assemblyName.Name;
        if (string.IsNullOrWhiteSpace(packageName))
        {
            return null;
        }

        var dependenciesPath = Path.Combine(_rootAssemblyPath, $"{_rootAssemblyName}.deps.json");
        if (!File.Exists(dependenciesPath))
        {
            return null;
        }

        using var dependenciesStream = File.OpenRead(dependenciesPath);

        using var dependencyReader = new DependencyContextJsonReader();
        var context = dependencyReader.Read(dependenciesStream);

        var dependencyPath = string.Empty;
        foreach (var dependency in context.RuntimeLibraries)
        {
            if (dependency.Name == packageName)
            {
                if (string.IsNullOrWhiteSpace(dependency.Path) || !dependency.RuntimeAssemblyGroups.Any())
                {
                    return null;
                }

                var runtimeAssemblyGroup = dependency.RuntimeAssemblyGroups[0];
                if (!runtimeAssemblyGroup.AssetPaths.Any())
                {
                    return null;
                }

                dependencyPath = Path.Combine(dependency.Path, runtimeAssemblyGroup.AssetPaths[0]);
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(dependencyPath))
        {
            return null;
        }

        return Path.Combine(packagesRoot, dependencyPath);
    }

    private static string? GetNuGetPackagesRoot()
    {
        var nugetPath = Environment.GetEnvironmentVariable(NuGetPackagesEnvName);
        if (!string.IsNullOrWhiteSpace(nugetPath) && Directory.Exists(nugetPath))
        {
            return nugetPath;
        }

        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrWhiteSpace(userProfilePath))
        {
            return null;
        }

        return Path.Combine(userProfilePath, ".nuget", "packages");
    }
}
