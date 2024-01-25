using System.Text;

namespace CounterStrikeSharp.API.Modules.Menu;

public class CenterHtmlMenu : BaseMenu
{
    public CenterHtmlMenu(string title) : base(title)
    {
    }
}

public class CenterHtmlMenuInstance : BaseMenuInstance
{
    public override int NumPerPage { get; } = 4;
    private readonly BasePlugin _plugin;
    private string _message;
    private Core.Listeners.OnTick _onTick;
    public CenterHtmlMenuInstance(BasePlugin plugin, CCSPlayerController player, IMenu menu) : base(player, menu)
    {
        _plugin = plugin;
        _onTick = SendMenuHtml;
        _plugin.RegisterListener(_onTick);
    }

    private void SendMenuHtml()
    {
        Player.PrintToCenterHtml(_message);
    }

    public override void Display()
    {
        var keyOffset = 1;
        var builder = new StringBuilder();
        builder.Append($"{Menu.Title}<br>");
        for (var i = CurrentOffset;
             i < Math.Min(CurrentOffset + MenuItemsPerPage, Menu.MenuOptions.Count);
             i++)
        {
            var option = Menu.MenuOptions[i];

            builder.Append(
                $"<font color='{(option.Disabled ? "gray": "green")}'>[!{keyOffset++}]</font> {option.Text}<br>");
        }

        if (HasPrevButton)
        {
            builder.Append($"<font color='yellow'>[!7] -></font> Prev<br>");
        }
            
        if (HasNextButton)
        {
            builder.Append($"<font color='yellow'>[!8] -></font> Next<br>");
        }
        
        builder.Append($"<font color='red'>[!9] -></font> Exit<br>");

        _message = builder.ToString();
    }

    public override void Reset()
    {
        _plugin.RemoveListener("OnTick", _onTick);
        CurrentOffset = 0;
        Page = 0;
        PrevPageOffsets.Clear();
        Player = null;
    }
}