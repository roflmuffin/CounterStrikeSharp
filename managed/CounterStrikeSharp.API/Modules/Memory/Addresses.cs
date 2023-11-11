using System.IO;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public class Addresses
{
    public static string EnginePath => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}engine2{Constants.ModuleSuffix}");
    public static string Tier0Path => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}tier0{Constants.ModuleSuffix}");

    public static string ServerPath => Path.Join(Server.GameDirectory, Constants.GameBinaryPath, $"{Constants.ModulePrefix}server{Constants.ModuleSuffix}");

    public static string SchemaSystemPath => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}schemasystem{Constants.ModuleSuffix}");
    public static string VScriptPath => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}vscript{Constants.ModuleSuffix}");

}