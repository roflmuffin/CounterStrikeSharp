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

namespace CounterStrikeSharp.API.Modules.Menu;

public interface IMenu
{
    public string Title { get; set; }
    public List<ChatMenuOption> MenuOptions { get; }
    public PostSelectAction PostSelectAction
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }
        
    public ChatMenuOption AddMenuOption(string display, Action<CCSPlayerController, ChatMenuOption> onSelect, bool disabled = false);
}

public interface IMenuInstance
{
    protected IMenu Menu { get; }
    protected CCSPlayerController? Player { get; }
    protected bool CloseOnSelect { get; }
    protected int Page { get; }
    protected int CurrentOffset { get; }
    protected int NumPerPage { get; }
    protected Stack<int> PrevPageOffsets { get; }
        
    public void NextPage();
    public void PrevPage();
    public void Reset();

    public void Close()
    {
        // Fallback for backwards compatibility
        throw new NotImplementedException();
    }
        
    public void Display();
    public void OnKeyPress(CCSPlayerController player, int key);
}