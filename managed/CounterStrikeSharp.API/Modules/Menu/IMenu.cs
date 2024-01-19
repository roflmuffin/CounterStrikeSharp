using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public interface IMenu
    {
        public string Title { get; set; }
        public List<ChatMenuOption> MenuOptions { get; }
        
        public ChatMenuOption AddMenuOption(string display, Action<CCSPlayerController, ChatMenuOption> onSelect, bool disabled = false);
    }

    public interface IMenuInstance
    {
        protected IMenu Menu { get; }
        protected CCSPlayerController? Player { get; }
        protected int Page { get; }
        protected int CurrentOffset { get; }
        protected int NumPerPage { get; }
        protected Stack<int> PrevPageOffsets { get; }
        
        public void NextPage();
        public void PrevPage();
        public void Reset();
        public void Display();
        public void OnKeyPress(CCSPlayerController player, int key);
    }
}