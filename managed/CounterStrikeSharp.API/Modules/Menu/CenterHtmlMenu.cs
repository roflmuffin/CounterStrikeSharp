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

using System.Text;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Modules.Menu;

public class CenterHtmlMenu : BaseMenu
{
    private readonly BasePlugin? _plugin;
    public string TitleColor { get; set; } = "yellow";
    public string EnabledColor { get; set; } = "green";
    public string DisabledColor { get; set; } = "grey";
    public string PrevPageColor { get; set; } = "yellow";
    public string NextPageColor { get; set; } = "yellow";
    public string CloseColor { get; set; } = "red";

    public CenterHtmlMenu(string title, BasePlugin plugin) : base(ModifyTitle(title))
    {
        _plugin = plugin;
    }
    
    [Obsolete("Use the constructor that takes a BasePlugin")]
    public CenterHtmlMenu(string title) : base(ModifyTitle(title))
    {
    }

    public override void Open(CCSPlayerController player)
    {
        if (_plugin == null)
        {
            throw new InvalidOperationException("This method is unsupported with the CenterHtmlMenu constructor used." +
                                                "Please provide a BasePlugin in the constructor.");
        }; 
        
        MenuManager.OpenCenterHtmlMenu(_plugin, player, this);
    }

    public override ChatMenuOption AddMenuOption(string display, Action<CCSPlayerController, ChatMenuOption> onSelect,
        bool disabled = false)
    {
        var option = new ChatMenuOption(ModifyOptionDisplay(display), disabled, onSelect);
        MenuOptions.Add(option);
        return option;
    }

    private static string ModifyTitle(string title)
    {
        if (title.Length > 32)
        {
            Application.Instance.Logger.LogWarning("Title should not be longer than 32 characters for a CenterHtmlMenu");
            return title[..32];
        }

        return title;
    }

    private static string ModifyOptionDisplay(string display)
    {
        if (display.Length > 26)
        {
            Application.Instance.Logger.LogWarning("Display should not be longer than 26 characters for a CenterHtmlMenu item");
            return display[..26];
        }

        return display;
    }
}

public class CenterHtmlMenuInstance : BaseMenuInstance
{
    private readonly BasePlugin _plugin;
    public override int NumPerPage => 5; // one less than the actual number of items per page to avoid truncated options
    protected override int MenuItemsPerPage => (Menu.ExitButton ? 0 : 1) + ((HasPrevButton && HasNextButton) ? NumPerPage - 1 : NumPerPage);

    public CenterHtmlMenuInstance(BasePlugin plugin, CCSPlayerController player, IMenu menu) : base(player, menu)
    {
        _plugin = plugin;
        RemoveOnTickListener();
        plugin.RegisterListener<Core.Listeners.OnTick>(Display);
    }

    public override void Display()
    {
        if (MenuManager.GetActiveMenu(Player) != this)
        {
            Reset();
            return;
        }

        if (Menu is not CenterHtmlMenu centerHtmlMenu)
        {
            return;
        }

        var builder = new StringBuilder();
        builder.Append($"<b><font color='{centerHtmlMenu.TitleColor}'>{centerHtmlMenu.Title}</font></b>");
        builder.AppendLine("<br>");

        var keyOffset = 1;

        for (var i = CurrentOffset; i < Math.Min(CurrentOffset + MenuItemsPerPage, centerHtmlMenu.MenuOptions.Count); i++)
        {
            var option = centerHtmlMenu.MenuOptions[i];
            string color = option.Disabled ? centerHtmlMenu.DisabledColor : centerHtmlMenu.EnabledColor;
            builder.Append($"<font color='{color}'>!{keyOffset++}</font> {option.Text}");
            builder.AppendLine("<br>");
        }

        if (HasPrevButton)
        {
            builder.AppendFormat($"<font color='{centerHtmlMenu.PrevPageColor}'>!7</font> &#60;- Prev");
            builder.AppendLine("<br>");
        }

        if (HasNextButton)
        {
            builder.AppendFormat($"<font color='{centerHtmlMenu.NextPageColor}'>!8</font> -> Next");
            builder.AppendLine("<br>");
        }

        if (centerHtmlMenu.ExitButton)
        {
            builder.AppendFormat($"<font color='{centerHtmlMenu.CloseColor}'>!9</font> -> Close");
            builder.AppendLine("<br>");
        }

        var currentPageText = builder.ToString();
        Player.PrintToCenterHtml(currentPageText);
    }

    public override void Close()
    {
        base.Close();
        RemoveOnTickListener();

        // Send a blank message to clear the menu
        Player.PrintToCenterHtml(" ");
    }

    private void RemoveOnTickListener()
    {
        var onTick = new Core.Listeners.OnTick(Display);
        _plugin.RemoveListener("OnTick", onTick);
    }
}