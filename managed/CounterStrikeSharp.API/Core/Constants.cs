using System;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API.Core;

public static class Constants
{
    public static string ModulePrefix
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "lib";
            }

            throw new NotSupportedException("Not supported.");
        }
    }

    public static string ModuleSuffix
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return ".so";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return ".dll";
            }

            throw new NotSupportedException("Not supported.");
        }
    }

    public static string RootBinaryPath
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "/bin/win64";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

                return "/bin/linuxsteamrt64/";
            }

            throw new NotSupportedException("Not supported.");
        }
    }

    public static string GameBinaryPath
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "/csgo/bin/win64/";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "/csgo/bin/linuxsteamrt64/";
            }

            throw new NotSupportedException("Not supported");
        }
    }

}