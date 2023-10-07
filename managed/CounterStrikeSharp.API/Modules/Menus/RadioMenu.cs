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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Players;

namespace CounterStrikeSharp.API.Modules.Menus
{
    [Flags]
    public enum RadioMenuOptionFlags
    {
        DISABLED = (1 << 0)
    }

    public interface IRadioMenu
    {
        string Title { get; set; }
        IRadioMenuOption AddMenuOption(string display, string value, RadioMenuOptionFlags? flags);
        void Display(int client);
        void Close();
        Action<Player, IRadioMenuOption> Handler { get; set; }
    }

    public class RadioMenu : NativeObject, IRadioMenu
    {
        public RadioMenu(IntPtr handle) : base(handle)
        {
        }

        public RadioMenu(string title) : base(IntPtr.Zero)
        {
            Handle = NativeAPI.CreateMenu(title);
        }

        public string Title { get; set; }

        public void Display(int client)
        {
            NativeAPI.ShowMenu(client, Handle);
        }

        public void Close()
        {
        }

        FunctionReference.CallbackDelegate _internalHandler;
        private Action<Player, IRadioMenuOption> _handler;

        public Action<Player, IRadioMenuOption> Handler
        {
            get
            {
                return _handler;
            }
            set
            {
                unsafe
                {
                    _handler = value;

                    _internalHandler = context =>
                    {
                        var scriptContext = new ScriptContext(context);
                        var playerIndex = scriptContext.GetArgument<int>(0);
                        var displayValue = scriptContext.GetArgument<string>(1);
                        var value = scriptContext.GetArgument<string>(2);

                        var player = Player.FromIndex(playerIndex);
                        var option = _menuOptions.FirstOrDefault(x => x.Value == value);

                        _handler?.Invoke(player, option);
                    };


                    NativeAPI.AddMenuHandler(Handle, Marshal.GetFunctionPointerForDelegate(_internalHandler));
                }
            }
        }

        private List<IRadioMenuOption> _menuOptions = new List<IRadioMenuOption>();

        public IRadioMenuOption AddMenuOption(string display, string value, RadioMenuOptionFlags? flags = null)
        {
            var intPtr = NativeAPI.AddMenuOption(Handle, display, value, (int) (flags ?? 0));
            var option = new RadioMenuOption(intPtr);
            option.Text = display;
            option.Value = value;

            _menuOptions.Add(option);

            return option;
        }
    }
}