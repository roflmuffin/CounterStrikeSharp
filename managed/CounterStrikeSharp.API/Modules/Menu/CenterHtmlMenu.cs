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
    
    public bool InlinePageOptions { get; set; } = true;
    public int MaxTitleLength { get; set; } = 0; // defaults to 0 = no limit, if enabled, recommended value is 32
    public int MaxOptionLength  { get; set; } = 0; // defaults to 0 = no limit, if enabled, recommended value is 26

    public CenterHtmlMenu(string title, BasePlugin plugin, bool inlinePageOptions = true, int maxTitleLength = 0, int maxOptionLength = 0): base(title)
    {
        Title = title.TruncateHtml(MaxTitleLength);
        _plugin = plugin;
        InlinePageOptions = inlinePageOptions;
        MaxTitleLength = maxTitleLength;
        MaxOptionLength = maxOptionLength;
    }

    [Obsolete("Use the constructor that takes a BasePlugin")]
    public CenterHtmlMenu(string title) : base(title)
    {
        Title = title.TruncateHtml(MaxTitleLength);
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
        var option = new ChatMenuOption(display.TruncateHtml(MaxOptionLength), disabled, onSelect);
        MenuOptions.Add(option);
        return option;
    }
}

public class CenterHtmlMenuInstance : BaseMenuInstance
{
    private readonly BasePlugin _plugin;
    public override int NumPerPage => 5; // one less than the actual number of items per page to avoid truncated options
    public bool InlinePageOptions { get; set; } = true;
    protected override int MenuItemsPerPage
    {
        get
        {
            int count = NumPerPage;
            if (InlinePageOptions == false)
            {
                if (!HasPrevButton)
                    count++;

                if (!HasNextButton)
                    count++;
            }
            else
            {
                count++;
                if (!HasExitButton && !HasPrevButton && !HasNextButton)
                    count++;
            }

            return count;
        }
    }

    public CenterHtmlMenuInstance(BasePlugin plugin, CCSPlayerController player, IMenu menu) : base(player, menu)
    {
        _plugin = plugin;
        RemoveOnTickListener();
        plugin.RegisterListener<Core.Listeners.OnTick>(Display);
        
        if (menu is CenterHtmlMenu centerHtmlMenu)
            InlinePageOptions = centerHtmlMenu.InlinePageOptions;
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
        
        AddPageOptions(centerHtmlMenu, builder);

        var currentPageText = builder.ToString();
        Player.PrintToCenterHtml(currentPageText);
    }
    
    private void AddPageOptions(CenterHtmlMenu centerHtmlMenu, StringBuilder builder)
    {
        string prevText = $"<font color='{centerHtmlMenu.PrevPageColor}'>!7 &#60;</font> Prev";
        string closeText = $"<font color='{centerHtmlMenu.CloseColor}'>!9 X</font> Close";
        string nextText = $"<font color='{centerHtmlMenu.NextPageColor}'>!8 ></font> Next";

        if (InlinePageOptions)
            AddInlinePageOptions(prevText, closeText, nextText, centerHtmlMenu.ExitButton, builder);
        else
            AddMultilinePageOptions(prevText, closeText, nextText, centerHtmlMenu.ExitButton, builder);
    }


    private void AddInlinePageOptions(string prevText, string closeText, string nextText, bool hasExitButton, StringBuilder builder)
    {
        if (HasPrevButton && HasExitButton && HasNextButton)
        {
            builder.Append($"{prevText} | {closeText} | {nextText}");
            return;
        }

        string doubleOptionSplitString = " \u200e \u200e \u200e \u200e | \u200e \u200e \u200e \u200e "; // empty characters that are not trimmed

        int optionsCount = 0;
        if (HasPrevButton)
        {
            builder.AppendFormat(prevText);
            optionsCount++;
        }

        if (hasExitButton)
        {
            if (optionsCount++ > 0)
                builder.Append(doubleOptionSplitString);

            builder.AppendFormat(closeText);
        }

        if (HasNextButton)
        {
            if (optionsCount > 0)
                builder.Append(doubleOptionSplitString);

            builder.AppendFormat(nextText);
        }
    }

    private void AddMultilinePageOptions(string prevText, string closeText, string nextText, bool hasExitButton, StringBuilder builder)
    {
        if (HasPrevButton)
        {
            builder.AppendFormat(prevText);
            builder.AppendLine("<br>");
        }

        if (hasExitButton)
        {
            builder.AppendFormat(closeText);
            builder.AppendLine("<br>");
        }

        if (HasNextButton)
        {
            builder.AppendFormat(nextText);
            builder.AppendLine("<br>");
        }
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