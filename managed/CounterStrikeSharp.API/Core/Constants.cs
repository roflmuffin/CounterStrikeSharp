using System;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API.Core;

public static class Constants
{
    public static string ROOT_BINARY_PATH
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

    public static string GAME_BINARY_PATH
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