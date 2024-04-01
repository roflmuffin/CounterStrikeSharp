using System.Reflection;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API;

public class Api
{
    /// <summary>
    /// Returns the API version of CounterStrikeSharp running on the server
    /// </summary>
    /// <returns></returns>
    public static int GetVersion()
    {
        return Assembly.GetAssembly(typeof(BasePlugin))!.GetName().Version!.Build;
    }
    
    /// <summary>
    /// Returns the assembly version of CounterStrikeSharp running on the server as a string including git commit hash
    /// </summary>
    /// <example>1.0.0+9d8b6be</example>
    public static string GetVersionString()
    {
        return Assembly.GetAssembly(typeof(BasePlugin))!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
            .InformationalVersion;
    }
}