namespace CounterStrikeSharp.API.Modules.Extensions;

public static class CCSCustomHudLayoutExtensions
{
    extension(CCSCustomHudLayout customHud)
    {
        public void SetHasClass(string panelid, string classname, bool hasclass) => NativeAPI.SetHasClass(customHud.Handle, panelid, classname, hasclass);

        public void SetHasClassForPlayer(CCSPlayerController player, string panelid, string classname, bool hasclass)
        {
            if (player == null || player.Handle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(player));
            }

            NativeAPI.SetHasClassForPlayer(customHud.Handle, player.Handle, panelid, classname, hasclass);
        }

        public void SetDialogVariableString(string panelid, string variablename, string value) => NativeAPI.SetDialogVariableString(customHud.Handle, panelid, variablename, value);

        public void SetDialogVariableStringForPlayer(CCSPlayerController player, string panelid, string variablename, string value)
        {
            if (player == null || player.Handle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(player));
            }

            NativeAPI.SetDialogVariableStringForPlayer(customHud.Handle, player.Handle, panelid, variablename, value);
        }

        public void SetInputCaptureEnabled(CCSPlayerController player, bool enable)
        {
            if (player == null || player.Handle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(player));
            }

            NativeAPI.SetInputCaptureEnabled(customHud.Handle, player.Handle, enable);
        }

        public bool IsInputCaptureEnabled(CCSPlayerController player)
        {
            if (player == null || player.Handle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(player));
            }

            return NativeAPI.IsInputCaptureEnabled(customHud.Handle, player.Handle);
        }
    }
}