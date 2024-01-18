using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu;

public abstract class BaseMenu: IMenuInstance
{
    public int NumPerPage => 6;
    public Stack<int> PrevPageOffsets { get; } = new();
    public IMenu Menu { get; }
    public CCSPlayerController? Player { get; private set; }


    private readonly Stack<int> _prevPageOffsets = new();

    public int Page { get; set; }
    public int CurrentOffset { get; set; }

    protected BaseMenu(CCSPlayerController player, IMenu menu)
    {
        Menu = menu;
        Player = player;
    }

    private bool HasPrevButton => Page > 0;
    private bool HasNextButton => CurrentOffset + NumPerPage < Menu.MenuOptions.Count;
    private int MenuItemsPerPage => NumPerPage + 2 - (HasNextButton ? 1 : 0) - (HasPrevButton ? 1 : 0);

    public void Display()
    {
        throw new NotImplementedException();
    }
    
    internal void OnKeyPress(CCSPlayerController player, int key)
    {
        if (Player == null || player.Handle != Player.Handle) return;

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

    public void Reset()
    {
        CurrentOffset = 0;
        Page = 0;
        PrevPageOffsets.Clear();
        Player = null;
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
