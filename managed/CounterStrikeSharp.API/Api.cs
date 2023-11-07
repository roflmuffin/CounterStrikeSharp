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
}