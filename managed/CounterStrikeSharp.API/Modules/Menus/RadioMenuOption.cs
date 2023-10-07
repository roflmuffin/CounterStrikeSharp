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

namespace CounterStrikeSharp.API.Modules.Menus
{
    public interface IRadioMenuOption
    {
        string Text { get; set; }
        string Value { get; set; }
        bool Disabled { get; set; }
        IntPtr Handle { get; }
    }

    public class RadioMenuOption : NativeObject, IRadioMenuOption
    {
        public RadioMenuOption(IntPtr handle) : base(handle)
        {
        }

        public RadioMenuOption(string text, bool disabled, string value = null) : base(IntPtr.Zero)
        {
            Text = text;
            Disabled = disabled;
            Value = value;
        }

        public string Text { get; set; }
        public bool Disabled { get; set; }
        public string Value { get; set; }
    }
}