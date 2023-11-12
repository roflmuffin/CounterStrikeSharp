using System;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API.Core;

public static class Constants
{
    public static string ModulePrefix { get; }

    public static string ModuleSuffix { get; }

    public static string RootBinaryPath { get; }

    public static string GameBinaryPath { get; }

    static Constants()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ModulePrefix = "";
            ModuleSuffix = ".dll";
            GameBinaryPath = "/csgo/bin/win64/";
            RootBinaryPath = "/bin/win64";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            ModulePrefix = "lib";
            ModuleSuffix = ".so";
            GameBinaryPath = "/csgo/bin/linuxsteamrt64/";
            RootBinaryPath = "/bin/linuxsteamrt64/";
        }
        else
        {
            throw new NotSupportedException($"""Current platform is "{RuntimeInformation.OSDescription}", but does not supported.""");
        }
    }
}