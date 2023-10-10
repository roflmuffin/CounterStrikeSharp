using System.IO;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public class Addresses
{
    public static string EnginePath = Path.Join(Server.GameDirectory, Constants.ROOT_BINARY_PATH, "libengine2.so");
    public static string Tier0Path = Path.Join(Server.GameDirectory, Constants.ROOT_BINARY_PATH, "libtier0.so");
    public static string ServerPath = Path.Join(Server.GameDirectory, Constants.GAME_BINARY_PATH, "libserver.so");

    public static string SchemaSystemPath =
        Path.Join(Server.GameDirectory, Constants.ROOT_BINARY_PATH, "libschemasystem.so");

    public static string VScriptPath = Path.Join(Server.GameDirectory, Constants.ROOT_BINARY_PATH, "libvscript.so");
}