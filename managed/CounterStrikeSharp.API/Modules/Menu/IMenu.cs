using System.Collections.Generic;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public interface IMenu
    {
        public string Title { get; set; }
        public List<IMenuOption> MenuOptions { get; }
        
        public IMenuOption AddMenuOption(string display, Action<CCSPlayerController, IMenuOption> onSelect, bool disabled = false);
    }
    
    public interface IMenuOption
    {
        public string Text { get; set; }
        public bool Disabled { get; set; }
        
        public Action<CCSPlayerController, IMenuOption> OnSelect { get; set; }
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
    }
}