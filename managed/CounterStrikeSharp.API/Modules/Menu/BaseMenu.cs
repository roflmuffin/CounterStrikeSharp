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

using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public abstract class BaseMenu : IMenu
    {
        public string Title { get; set; }
        public List<ChatMenuOption> MenuOptions { get; } = new();
     
        protected BaseMenu(string title)
        {
            Title = title;
        }
        
        public virtual ChatMenuOption AddMenuOption(string display, Action<CCSPlayerController, ChatMenuOption> onSelect, bool disabled = false)
        {
            var option = new ChatMenuOption(display, disabled, onSelect);
            MenuOptions.Add(option);
            return option;
        }
    }
    
    // This must be called ChatMenuOption to maintain backwards compatibility with the old API
    public class ChatMenuOption
    {
        public string Text { get; set; }
        public bool Disabled { get; set; }
        public Action<CCSPlayerController, ChatMenuOption> OnSelect { get; set; }

        public ChatMenuOption(string display, bool disabled, Action<CCSPlayerController, ChatMenuOption> onSelect)
        {
            Text = display;
            Disabled = disabled;
            OnSelect = onSelect;
        }
    }

    public abstract class BaseMenuInstance : IMenuInstance
    {
        public int NumPerPage => 6;
        public Stack<int> PrevPageOffsets { get; } = new();
        public IMenu Menu { get; }
        public CCSPlayerController Player { get; }

        public int Page { get; set; }
        public int CurrentOffset { get; set; }

        protected BaseMenuInstance(CCSPlayerController player, IMenu menu)
        {
            Menu = menu;
            Player = player;
        }

        protected bool HasPrevButton => Page > 0;
        protected bool HasNextButton => CurrentOffset + NumPerPage < Menu.MenuOptions.Count;
        protected int MenuItemsPerPage => NumPerPage + 2 - (HasNextButton ? 1 : 0) - (HasPrevButton ? 1 : 0);

        public virtual void Display()
        {
            throw new NotImplementedException();
        }

        public void OnKeyPress(CCSPlayerController player, int key)
        {
            if (player.Handle != Player.Handle) return;

            if (key == 8 && HasNextButton)
            {
                NextPage();
                return;
            }

            if (key == 1 && HasPrevButton)
            {
                PrevPage();
                return;
            }

            if (key == 9)
            {
                Reset();
                return;
            }

            var desiredValue = key;
            if (HasPrevButton) desiredValue = key - 1;

            var menuItemIndex = CurrentOffset + desiredValue - 1;

            if (menuItemIndex >= 0 && menuItemIndex < Menu.MenuOptions.Count)
            {
                var menuOption = Menu.MenuOptions[menuItemIndex];
                
                if (!menuOption.Disabled)
                {
                    menuOption.OnSelect(Player, menuOption);
                    Reset();
                }
            }
        }

        public virtual void Reset()
        {
            CurrentOffset = 0;
            Page = 0;
            PrevPageOffsets.Clear();
        }

        public void NextPage()
        {
            PrevPageOffsets.Push(CurrentOffset);
            CurrentOffset += MenuItemsPerPage;
            Page++;
            Display();
        }

        public void PrevPage()
        {
            Page--;
            CurrentOffset = PrevPageOffsets.Pop();
            Display();
        }
    }
}
