using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Menu;

public class ChatMenuOption
{
    public ChatMenuOption(string text, bool disabled, Action<CCSPlayerController, ChatMenuOption> onSelect)
    {
        Text = text;
        Disabled = disabled;
        OnSelect = onSelect;
    }

    public Action<CCSPlayerController, ChatMenuOption> OnSelect { get; set; }

    public string Text { get; set; }
    public bool Disabled { get; set; }
}

public class ChatMenu
{
    public string Title { get; set; }
    public List<ChatMenuOption> MenuOptions { get; } = new();

    public ChatMenu(string title)
    {
        Title = title;
    }

    public ChatMenuOption AddMenuOption(string display, Action<CCSPlayerController, ChatMenuOption> onSelect, bool disabled = false)
    {
        var option = new ChatMenuOption(display, disabled, onSelect);
        MenuOptions.Add(option);
        return option;
    }
}

public class ChatMenuInstance
{
    // Six items seems to be able to fit all options in the chat bot without having to open it
    readonly int _numPerPage = 6;
    private readonly Stack<int> _prevPageOffsets = new();
    private readonly ChatMenu _menu;

    int _page = 0;
    int _currentOffset = 0;
    private CCSPlayerController _player;

    public ChatMenuInstance(CCSPlayerController player, ChatMenu menu)
    {
        _menu = menu;
        _player = player;
    }

    private bool HasPrevButton => _page > 0;
    private bool HasNextButton => (_currentOffset + _numPerPage) < _menu.MenuOptions.Count;
    private int MenuItemsPerPage => _numPerPage + 2 - (HasNextButton ? 1 : 0) - (HasPrevButton ? 1 : 0);

    public void Display()
    {
        _player.PrintToChat(_menu.Title);
        _player.PrintToChat("---");

        int keyOffset = 1;

        if (HasPrevButton)
        {
            _player.PrintToChat($" {ChatColors.Yellow}[!1] {ChatColors.Default}-> Prev");
            keyOffset++;
        }

        for (int i = _currentOffset;
             i < Math.Min(_currentOffset + MenuItemsPerPage, _menu.MenuOptions.Count);
             i++)
        {
            var option = _menu.MenuOptions[i];

            _player.PrintToChat(
                $" {(option.Disabled ? ChatColors.Grey : ChatColors.Green)} [!{keyOffset++}] {ChatColors.Default}{option.Text}");
        }

        if (HasNextButton)
        {
            _player.PrintToChat($" {ChatColors.Yellow}[!8] {ChatColors.Default}-> Next");
        }
    }

    internal void OnKeyPress(CCSPlayerController player, int key)
    {
        if (_player == null || player.Handle != _player.Handle) return;

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

        var desiredValue = key;
        if (HasPrevButton) desiredValue = key - 1;

        var menuItemIndex = _currentOffset + desiredValue - 1;
        var menuOption = _menu.MenuOptions[menuItemIndex];

        if (!menuOption.Disabled)
        {
            menuOption.OnSelect(_player, menuOption);
            Reset();
        }
    }

    public void Reset()
    {
        _currentOffset = 0;
        _page = 0;
        _prevPageOffsets.Clear();
        _player = null;
    }

    public void NextPage()
    {
        _prevPageOffsets.Push(_currentOffset);
        _currentOffset += MenuItemsPerPage;
        _page++;
        Display();
    }

    public void PrevPage()
    {
        _page--;
        _currentOffset = _prevPageOffsets.Pop();
        Display();
    }
}

public static class ChatMenus
{
    private static readonly Dictionary<IntPtr, ChatMenuInstance> ActiveMenus = new();

    public static void OpenMenu(CCSPlayerController player, ChatMenu menu)
    {
        ActiveMenus[player.Handle] = new ChatMenuInstance(player, menu);
        ActiveMenus[player.Handle].Display();
    }

    public static void OnKeyPress(CCSPlayerController player, int key)
    {
        if (!ActiveMenus.ContainsKey(player.Handle)) return;

        ActiveMenus[player.Handle].OnKeyPress(player, key);
    }
}