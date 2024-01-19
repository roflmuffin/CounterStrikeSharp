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

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ConsoleMenu : BaseMenu
    {
        public ConsoleMenu(string title) : base(title)
        {
        }
    }

    public class ConsoleMenuInstance : BaseMenuInstance
    {
        public ConsoleMenuInstance(CCSPlayerController player, IMenu menu) : base(player, menu)
        {
        }

        public override void Display()
        {
            Player.PrintToConsole(Menu.Title);
            Player.PrintToConsole("---");

            var keyOffset = 1;

            if (HasPrevButton)
            {
                Player.PrintToConsole($"!1 -> Prev");
                keyOffset++;
            }

            for (var i = CurrentOffset;
                 i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count);
                 i++)
            {
                var option = Menu.MenuOptions[i];

                Player.PrintToConsole(
                    $" {(option.Disabled ? "[Enabled]" : "[Disabled] - ")} !{keyOffset++} {option.Text}");
            }

            if (HasNextButton)
            {
                Player.PrintToConsole($"!8 -> Next");
            }
        }
    }
}
