/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core;

public class NativeBridge
{
    private readonly ILogger<NativeBridge> _logger;
    private static ILogger<NativeBridge>? _staticLogger;

    private static nint _libHandle;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void InvokeNativeDelegate(IntPtr ptr);

    private static InvokeNativeDelegate? _invokeNative;

    public NativeBridge(ILogger<NativeBridge> logger)
    {
        _logger = logger;
        _staticLogger ??= logger;
    }

    public bool Initialize()
    {
        var libName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "counterstrikesharp.dll"
            : "counterstrikesharp.so";

        try
        {
            _libHandle = NativeLibrary.Load(libName);
            _invokeNative = GetDelegate<InvokeNativeDelegate>("InvokeNative");

            _logger.LogInformation("Loaded native lib OK ({Library})", libName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load native lib '{Library}'", libName);
            return false;
        }
    }

    private static T? GetDelegate<T>(string exportName) where T : class
    {
        try
        {
            var ptr = NativeLibrary.GetExport(_libHandle, exportName);
            return Marshal.GetDelegateForFunctionPointer<T>(ptr);
        }
        catch (Exception ex)
        {
            _staticLogger?.LogError(ex, "Missing export {ExportName}", exportName);
            return null;
        }
    }

    [SecurityCritical]
    public static void InvokeNative(IntPtr ptr)
    {
        if (_invokeNative == null)
        {
            _staticLogger?.LogError("InvokeNative not initialized!");
            return;
        }

        try
        {
            _invokeNative.Invoke(ptr);
        }
        catch (Exception ex)
        {
            _staticLogger?.LogError(ex, "Error while invoking native function");
        }
    }
}
