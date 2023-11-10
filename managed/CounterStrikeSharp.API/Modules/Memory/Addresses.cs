using System.IO;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public class Addresses
{
    public static string EnginePath
    {
        get
        {
            var engine2 = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                engine2 = "libengine2.so";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                engine2 = "engine2.dll";
            }
            return Path.Join(Server.GameDirectory, Constants.ROOT_BINARY_PATH, engine2);

        }
    }

    public static string Tier0Path
    {
        get
        {
            var tier0 = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                tier0 = "libtier0.so";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                tier0 = "tier0.dll";
            }
            return Path.Join(Server.GameDirectory, Constants.ROOT_BINARY_PATH, tier0);

        }
    }

    public static string ServerPath
    {
        get
        {
            var server = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                server = "libserver.so";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                server = "server.dll";
            }
            return Path.Join(Server.GameDirectory, Constants.GAME_BINARY_PATH, server);

        }
    }

    public static string SchemaSystemPath
    {
        get
        {
            var schemasystem = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                schemasystem = "libschemasystem.so";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                schemasystem = "schemasystem.dll";
            }
            return Path.Join(Server.GameDirectory, Constants.ROOT_BINARY_PATH, schemasystem);


        }
    }

    public static string VScriptPath
    {
        get
        {
            var vscript = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                vscript = "libvscript.so";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                vscript = "vscript.dll";
            }
            return Path.Join(Server.GameDirectory, Constants.ROOT_BINARY_PATH, vscript);
        }
    }
}