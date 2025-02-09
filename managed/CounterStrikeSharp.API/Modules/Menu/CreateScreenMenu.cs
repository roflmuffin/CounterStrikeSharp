using System;
using System.Collections.Generic;
using System.Drawing;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public static class CreateScreenMenu
    {
        public static Dictionary<uint, CCSPlayerController> WorldTextOwners = new Dictionary<uint, CCSPlayerController>();


        public static CPointWorldText Create(
            CCSPlayerController player,
            string text,
            int size = 100,
            Color? color = null,
            string font = "",
            float shiftX = 0f,
            float shiftY = 0f,
            bool drawBackground = false,
            bool isMenu = true)
        {
            CCSPlayerPawn pawn = player.PlayerPawn.Value!;

            var handle = new CHandle<CCSGOViewModel>(
                (IntPtr)(pawn.ViewModelServices!.Handle +
                         Schema.GetSchemaOffset("CCSPlayer_ViewModelServices", "m_hViewModel") + 4));

            if (!handle.IsValid)
            {
                CCSGOViewModel viewmodel = Utilities.CreateEntityByName<CCSGOViewModel>("predicted_viewmodel")!;
                viewmodel.DispatchSpawn();
                handle.Raw = viewmodel.EntityHandle.Raw;
                Utilities.SetStateChanged(pawn, "CCSPlayerPawnBase", "m_pViewModelServices");
            }

            CPointWorldText worldText = Utilities.CreateEntityByName<CPointWorldText>("point_worldtext")!;
            worldText.MessageText = text;
            worldText.Enabled = true;
            worldText.FontSize = size;
            worldText.Fullbright = true;
            worldText.Color = color ?? Color.Aquamarine;
            worldText.WorldUnitsPerPx = (0.25f / 1050) * size;
            worldText.FontName = font;
            worldText.JustifyHorizontal = PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_LEFT;
            worldText.JustifyVertical = PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_TOP;
            worldText.ReorientMode = PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_NONE;

            if (drawBackground)
            {
                worldText.DrawBackground = true;
                if (isMenu)
                {
                    worldText.BackgroundBorderHeight = 0.2f;
                    worldText.BackgroundBorderWidth = 0.15f;
                }
            }

            QAngle eyeAngles = pawn.EyeAngles;
            Vector forward = new(), right = new(), up = new();
            NativeAPI.AngleVectors(eyeAngles.Handle, forward.Handle, right.Handle, up.Handle);

            Vector offset = new();
            offset += forward * 7;
            offset += right * shiftX;
            offset += up * shiftY;
            QAngle angles = new()
            {
                Y = eyeAngles.Y + 270,
                Z = 90 - eyeAngles.X,
                X = 0
            };


            worldText.DispatchSpawn();
            worldText.Teleport(pawn.AbsOrigin! + offset + new Vector(0, 0, pawn.ViewOffset.Z), angles, null);
            worldText.AcceptInput("SetParent", handle.Value, null, "!activator");

            WorldTextOwners[worldText.Index] = player;

            return worldText;
        }
    }
}
