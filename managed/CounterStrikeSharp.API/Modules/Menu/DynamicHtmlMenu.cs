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

namespace CounterStrikeSharp.API.Modules.Menu;

using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

using CounterStrikeSharp.API.Core;
using System;

/// <summary>
/// Dynamic Html Menu
/// </summary>
public class DynamicHtmlMenu : BaseMenu
{
    private readonly BasePlugin? _plugin;

    /// <summary>
    /// Default color for enabled option
    /// </summary>
    public string EnabledColor { get; set; } = "white";

    /// <summary>
    /// Default color for disabled option
    /// </summary>
    public string DisabledColor { get; set; } = "grey";

    /// <summary>
    /// Default color for selected option
    /// </summary>
    public string SelectedColor { get; set; } = "blue";

    /// <summary>
    /// Set this to <see langword="true"/> if you manage the selection manually or on your own way
    /// </summary>
    public bool CustomResolver { get; set; } = false;

    /// <summary>
    /// Should the player be frozen while the menu is displayed
    /// </summary>
    public bool FreezePlayer { get; set; } = true;

    /// <summary>
    /// Display the current page number and how much pages this menu have (this will cost you one line less per page)
    /// </summary>
    public bool ShowPagesNum { get; set; } = true;

    /// <summary>
    /// Setting it to null will disable the select sound
    /// </summary>
    public string? SelectedSound { get; set; } = "sounds/ui/csgo_ui_page_scroll.vsnd_c";

    /// <summary>
    /// Custom formatter for selected option
    /// </summary>
    public Func<string, string> SelectedFormatter { get; set; } = (optionText) => optionText;

    /// <summary>
    /// Custom formatter for the previous option
    /// </summary>
    public Func<string, string> PreviousFormatter { get; set; } = (optionText) => optionText;

    /// <summary>
    /// Custom formatter for the next option
    /// </summary>
    public Func<string, string> NextFormatter { get; set; } = (optionText) => optionText;

    /// <summary>
    /// Custom formatter for the exit option
    /// </summary>
    public Func<string, string> ExitFormatter { get; set; } = (optionText) => optionText;

    /// <summary>
    /// Custom formatter for the title
    /// </summary>
    public Func<string, string> TitleFormatter { get; set; } = (title) => $"<b><font class='fontSize-l' color='silver'>{title}</font></b>";

    /// <summary>
    /// Custom formatter for the pages text (Page x/n)
    /// </summary>
    public Func<int, int, string> PagesFormatter { get; set; } = (currentPage, maxPages) => $"<font class='fontSize-s'>Page {currentPage}/{maxPages}</font>";

    /// <summary>
    /// Button for selecting the next option
    ///
    /// (Only if CustomResolver is false)
    /// </summary>
    public PlayerButtons NextOptionButton { get; set; } = PlayerButtons.Back;

    /// <summary>
    /// Button for selecting the previous option
    ///
    /// (Only if CustomResolver is false)
    /// </summary>
    public PlayerButtons PreviousOptionButton { get; set; } = PlayerButtons.Forward;

    /// <summary>
    /// Button for closing the menu
    ///
    /// (Only if CustomResolver is false)
    /// </summary>
    public PlayerButtons ExitOptionButton { get; set; } = PlayerButtons.Jump;

    public DynamicHtmlMenu(string title, BasePlugin plugin) : base(ModifyTitle(title))
    {
        _plugin = plugin;
    }
    
    public override void Open(CCSPlayerController player)
    {
        if (_plugin == null)
        {
            throw new InvalidOperationException("This method is unsupported with the DynamicHtmlMenu constructor used." +
                                                "Please provide a BasePlugin in the constructor.");
        };

        MenuManager.OpenDynamicHtmlMenu(_plugin, player, this);
    }

    private static string ModifyTitle(string title)
    {
        if (title.Length > 32)
        {
            Application.Instance.Logger.LogWarning("Title should not be longer than 32 characters for a DynamicHtmlMenu");
            return title[..32];
        }

        return title;
    }
}

public class DynamicHtmlMenuInstance : BaseMenuInstance
{
    private readonly BasePlugin _plugin;
    public override int NumPerPage => 5; // one less than the actual number of items per page to avoid truncated options
    protected override int MenuItemsPerPage => (Menu.ExitButton ? 0 : 1) + ((HasPrevButton && HasNextButton) ? NumPerPage - 1 : NumPerPage) + ((Menu as DynamicHtmlMenu)!.ShowPagesNum ? -1 : 0);
    private int CurrentSelectionIndex = 0;
    private float LastPressedButtonTime = 0f;
    private static readonly float DelayBetweenButtonPress = 0.15f;

    /// <summary>
    /// Sliced list of options, only the displayed options are in this list (if its not null)
    /// </summary>
    public List<ChatMenuOption>? CurrentMenuOptions;
    // private static readonly long PlayerButton_Tab = 8589934592;

    public DynamicHtmlMenuInstance(BasePlugin plugin, CCSPlayerController player, IMenu menu) : base(player, menu)
    {
        _plugin = plugin;
        RemoveOnTickListener();
        plugin.RegisterListener<Listeners.OnTick>(Display);
    }

    public override void Display()
    {
        if (MenuManager.GetActiveMenu(Player) != this)
        {
            Reset();
            return;
        }

        if (Menu is not DynamicHtmlMenu dynamicHtmlMenu)
        {
            return;
        }

        var builder = new StringBuilder();
        builder.Append(dynamicHtmlMenu.TitleFormatter(dynamicHtmlMenu.Title));
        builder.AppendLine("<br>");

        if (dynamicHtmlMenu.ShowPagesNum)
        {
            builder.Append(dynamicHtmlMenu.PagesFormatter(Page + 1, GetPagesCount()));
            builder.AppendLine("<br>");
        }

        // GetRange might be slow? not sure
        List<ChatMenuOption> menuOptions = dynamicHtmlMenu.MenuOptions.GetRange(CurrentOffset, Math.Min(MenuItemsPerPage, dynamicHtmlMenu.MenuOptions.Count - CurrentOffset));

        if (HasNextButton)
        {
            menuOptions.Add(new(dynamicHtmlMenu.NextFormatter("Next page"), false, (player, option) =>
            {
                NextPage();
            }));
        }

        if (HasPrevButton)
        {
            menuOptions.Add(new (dynamicHtmlMenu.PreviousFormatter("Previous page"), false, (player, option) =>
            {
                PrevPage();
            }));
        }

        if (dynamicHtmlMenu.ExitButton)
        {
            menuOptions.Add(new (dynamicHtmlMenu.ExitFormatter("Close"), false, (player, option) =>
            {
                Close();
            }));
        }

        for (var i = 0; i < menuOptions.Count; i++)
        {
            var option = menuOptions[i];

            if (CurrentSelectionIndex == i)
            {
                builder.Append($"<font color='{dynamicHtmlMenu.SelectedColor}'>{dynamicHtmlMenu.SelectedFormatter(option.Text)}</font>");
            } else
            {
                builder.Append($"<font color='{(option.Disabled ? dynamicHtmlMenu.DisabledColor : dynamicHtmlMenu.EnabledColor)}'>{option.Text}</font>");
            }

            builder.AppendLine("<br>");
        }

        if (!dynamicHtmlMenu.CustomResolver && Player.Buttons != 0 && (Server.CurrentTime - LastPressedButtonTime) >= DelayBetweenButtonPress)
        {
            if (Player.Buttons == dynamicHtmlMenu.NextOptionButton)
            {
                CurrentSelectionIndex++;
            } else if (Player.Buttons == dynamicHtmlMenu.PreviousOptionButton)
            {
                CurrentSelectionIndex--;
            } else if(Player.Buttons == dynamicHtmlMenu.ExitOptionButton)
            {
                OnOptionSelected(Player, CurrentSelectionIndex, menuOptions);

                if (!string.IsNullOrEmpty(dynamicHtmlMenu.SelectedSound))
                {
                    Player.ExecuteClientCommand($"play {dynamicHtmlMenu.SelectedSound}");
                }
            }

            CurrentSelectionIndex = Math.Clamp(CurrentSelectionIndex, 0, menuOptions.Count - 1);
            LastPressedButtonTime = Server.CurrentTime;
        }

        var currentPageText = builder.ToString();
        Player.PrintToCenterHtml(currentPageText);

        if (dynamicHtmlMenu.FreezePlayer)
        {
            SetPlayerMoveType(Player, MoveType_t.MOVETYPE_NONE);
        }

        CurrentMenuOptions = menuOptions;
    }

    public override void Close()
    {
        base.Close();
        RemoveOnTickListener();

        // Send a blank message to clear the menu
        Player.PrintToCenterHtml(" ");

        if ((Menu as DynamicHtmlMenu)!.FreezePlayer)
        {
            // this is needed
            Server.NextFrame(() =>
            {
                SetPlayerMoveType(Player, MoveType_t.MOVETYPE_WALK);
            });
        }
    }

    public void SetCurrentSelection(int index)
    {
        if (CurrentMenuOptions != null)
        {
            CurrentSelectionIndex = Math.Clamp(index, 0, CurrentMenuOptions.Count - 1);
        } else
        {
            Application.Instance.Logger.LogError("Invalid DynamicHtmlMenuInstance action {0}", index);
        }
    }

    public void SelectNextOption()
    {
        SetCurrentSelection(++CurrentSelectionIndex);
    }

    public void SelectPreviousOption()
    {
        SetCurrentSelection(--CurrentSelectionIndex);
    }

    public void InvokeSelection()
    {
        // maybe more sanity check?
        if (!Player.IsValid)
            return;

        if (CurrentMenuOptions != null)
        {
            OnOptionSelected(Player, CurrentSelectionIndex, CurrentMenuOptions);
        }
    }

    private void RemoveOnTickListener()
    {
        var onTick = new Listeners.OnTick(Display);
        _plugin.RemoveListener<Listeners.OnTick>(onTick);
    }

    private void SetPlayerMoveType(CCSPlayerController player, MoveType_t moveType)
    {
        if (player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid)
        {
            player.PlayerPawn.Value.MoveType = moveType;
            player.PlayerPawn.Value.ActualMoveType = moveType;

            Utilities.SetStateChanged(player.PlayerPawn.Value, "CBaseEntity", "m_MoveType");
        }
    }

    private void OnOptionSelected(CCSPlayerController player, int menuItemIndex, List<ChatMenuOption> slicedOptions)
    {
        if (player.Handle != Player.Handle)
            return;

        if (menuItemIndex >= 0 && menuItemIndex < slicedOptions.Count)
        {
            // previous, next, back options should not trigger the PostSelectAction
            DynamicHtmlMenu? menu = (Menu as DynamicHtmlMenu)!;
            PostSelectAction cacheAction = menu.PostSelectAction;
            int extraButtons = GetExtraButtonCount();

            if (menuItemIndex > (slicedOptions.Count - extraButtons) - 1)
            {
                // one of the spicy buttons pressed, do nothing
                menu.PostSelectAction = PostSelectAction.Nothing;
            }

            InvokeOptionCallback(slicedOptions[menuItemIndex]);
            menu.PostSelectAction = cacheAction;
        } else
        {
            Application.Instance.Logger.LogError("Invalid menu option selected (index {0})", menuItemIndex);
        }
    }

    // these had to be rewritten in the same way instead of calling the base method because calling Display() while switching pages were messing with the options ranging for some reason

    public override void NextPage()
    {
        // base.NextPage();
        PrevPageOffsets.Push(CurrentOffset);
        CurrentOffset += MenuItemsPerPage;
        Page++;

        CurrentSelectionIndex = 0;
    }

    public override void PrevPage()
    {
        // base.PrevPage();
        Page--;
        CurrentOffset = PrevPageOffsets.Pop();

        CurrentSelectionIndex = 0;
    }
}
