using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Menu;

public interface IScreenMenu
{
    string Title { get; set; }
    Color TextColor { get; set; }
    string FontName { get; set; }
}

public interface IScreenMenuInstance
{
    void NextPage();
    void PrevPage();
    void Reset();
    void Close();
    void Display();
}
