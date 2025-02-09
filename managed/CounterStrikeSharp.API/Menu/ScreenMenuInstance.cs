using static CounterStrikeSharp.API.Modules.Menu.CreateScreenMenu;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Modules.Menu
{
    public class ScreenMenuInstance : BaseMenuInstance, IScreenMenuInstance
    {
        private CPointWorldText? _hudText;
        private readonly BasePlugin _plugin;
        public override int NumPerPage => 6;

        private int CurrentSelection = 0;
        private PlayerButtons _oldButtons;
        private bool _useHandled = false;
        private bool _menuJustOpened = true;
        private int _ticksSinceOpen = 0;

        private Core.Listeners.OnTick _onTickDelegate;
        private Core.Listeners.CheckTransmit _checkTransmitDelegate;
        private Core.Listeners.OnEntityDeleted _onEntityDeletedDelegate;
        private BasePlugin.GameEventHandler<EventRoundStart> _onRoundStartDelegate;
        public ScreenMenuInstance(BasePlugin plugin, CCSPlayerController player, ScreenMenu menu)
            : base(player, menu)
        {
            _plugin = plugin;
            Reset();
            CurrentSelection = 0;
            _oldButtons = player.Buttons;
            _useHandled = true;
            _menuJustOpened = true;
            _ticksSinceOpen = 0;

            _onTickDelegate = new Core.Listeners.OnTick(Update);
            _checkTransmitDelegate = new Core.Listeners.CheckTransmit(CheckTransmitListener);
            _onEntityDeletedDelegate = new Core.Listeners.OnEntityDeleted(OnEntityDeleted);
            _plugin.RegisterListener<Core.Listeners.CheckTransmit>(_checkTransmitDelegate);
            _plugin.RegisterListener<Core.Listeners.OnTick>(_onTickDelegate);
            _plugin.RegisterListener<Core.Listeners.OnEntityDeleted>(_onEntityDeletedDelegate);


            _onRoundStartDelegate = new BasePlugin.GameEventHandler<EventRoundStart>(OnRoundStart);
            _plugin.RegisterEventHandler<EventRoundStart>(_onRoundStartDelegate, HookMode.Pre);
            

            if ((player.Buttons & PlayerButtons.Use) == 0)
                _useHandled = false;
        }
        private void OnEntityDeleted(CEntityInstance entity)
        {

            uint entityIndex = entity.Index;
            if (WorldTextOwners.ContainsKey(entityIndex))
            {
                WorldTextOwners.Clear();
            }
        }
        public void CheckTransmitListener(CCheckTransmitInfoList infoList)
        {
            foreach ((CCheckTransmitInfo info, CCSPlayerController? client) in infoList)
            {
                if (client is null || client.IsValid is not true)
                    continue;

                foreach (var kvp in CreateScreenMenu.WorldTextOwners)
                {
                    uint worldTextIndex = kvp.Key;
                    CCSPlayerController owner = kvp.Value;

                    if (client.Slot != owner.Slot)
                    {
                        info.TransmitEntities.Remove((int)worldTextIndex);
                    }
                }
            }
        }
        public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            if (MenuManager.GetActiveMenu(Player) != this)
            {
                return HookResult.Continue;
            }

            _plugin.AddTimer(0.1f, () =>
            {
                RecreateHud();
            });

            return HookResult.Continue;
        }


        public void Update()
        {
            if (MenuManager.GetActiveMenu(Player) != this)
                return;

            var currentButtons = Player.Buttons;
            _ticksSinceOpen++;

            if (_ticksSinceOpen < 3)
            {
                if (_hudText == null || !_hudText.IsValid)
                    Display();
            }

            if (_menuJustOpened)
            {
                _menuJustOpened = false;
                _oldButtons = currentButtons;
                return;
            }

            if (((_oldButtons & PlayerButtons.Forward) == 0) && ((currentButtons & PlayerButtons.Forward) != 0))
            {
                int totalLines = GetTotalLines();
                CurrentSelection = (CurrentSelection - 1 + totalLines) % totalLines;
                Display();
            }

            if (((_oldButtons & PlayerButtons.Back) == 0) && ((currentButtons & PlayerButtons.Back) != 0))
            {
                int totalLines = GetTotalLines();
                CurrentSelection = (CurrentSelection + 1) % totalLines;
                Display();
            }

            if (((_oldButtons & PlayerButtons.Use) == 0) && ((currentButtons & PlayerButtons.Use) != 0))
            {
                if (!_useHandled)
                {
                    HandleSelection();
                    Display();
                    _useHandled = true;
                }
            }
            else if ((currentButtons & PlayerButtons.Use) == 0)
            {
                _useHandled = false;
            }

            _oldButtons = currentButtons;
        }

        private int GetTotalLines()
        {
            int selectable = Math.Min(NumPerPage, Menu.MenuOptions.Count - CurrentOffset);
            int navCount = 0;
            if (Menu is ScreenMenu screenMenu)
            {
                if (screenMenu.IsSubMenu)
                {
                    navCount = (Page == 0)
                        ? (Menu.MenuOptions.Count > NumPerPage ? 3 : 2)
                        : ((Menu.MenuOptions.Count - CurrentOffset) > NumPerPage ? 3 : 2);
                }
                else
                {
                    navCount = (Page == 0)
                        ? (Menu.MenuOptions.Count > NumPerPage ? 2 : 1)
                        : ((Menu.MenuOptions.Count - CurrentOffset) > NumPerPage ? 3 : 2);
                }
            }
            else
            {
                navCount = (Page == 0)
                    ? (Menu.MenuOptions.Count > NumPerPage ? 2 : 1)
                    : ((Menu.MenuOptions.Count - CurrentOffset) > NumPerPage ? 3 : 2);
            }
            return selectable + navCount;
        }

        public override void Display()
        {
            if (!(Menu is ScreenMenu screenMenu))
                return;

            var builder = new StringBuilder();
            string pad = new string('\u00A0', 30);
            builder.AppendLine("\u200B");
            builder.AppendLine($" {" "}{screenMenu.Title}");
            builder.AppendLine("\u200B");

            int selectable = Math.Min(NumPerPage, Menu.MenuOptions.Count - CurrentOffset);
            int navCount = 0;
            if (screenMenu.IsSubMenu)
            {
                navCount = (Page == 0)
                    ? (Menu.MenuOptions.Count > NumPerPage ? 3 : 2)
                    : ((Menu.MenuOptions.Count - CurrentOffset) > NumPerPage ? 3 : 2);
            }
            else
            {
                navCount = (Page == 0)
                    ? (Menu.MenuOptions.Count > NumPerPage ? 2 : 1)
                    : ((Menu.MenuOptions.Count - CurrentOffset) > NumPerPage ? 3 : 2);
            }

            int totalLines = selectable + navCount;
            if (CurrentSelection >= totalLines)
                CurrentSelection = totalLines - 1;
            if (CurrentSelection < 0)
                CurrentSelection = 0;

            for (int i = 0; i < selectable; i++)
            {
                var option = Menu.MenuOptions[CurrentOffset + i];
                string prefix = (CurrentSelection == i) ? "> " : "  ";
                builder.AppendLine($"{prefix}{i + 1}. {option.Text}");
            }

            builder.AppendLine("\u200B");

            if (Page == 0)
            {
                if (screenMenu.IsSubMenu)
                {
                    if (navCount == 3)
                    {
                        string prefix = (CurrentSelection == selectable + 0) ? "> " : "  ";
                        builder.AppendLine($"{prefix}7. Back");
                        string prefix2 = (CurrentSelection == selectable + 1) ? "> " : "  ";
                        builder.AppendLine($"{prefix2}8. Next");
                        string prefix3 = (CurrentSelection == selectable + 2) ? "> " : "  ";
                        builder.AppendLine($"{prefix3}9. Close");
                    }
                    else // navCount == 2: Back, Close
                    {
                        string prefix = (CurrentSelection == selectable + 0) ? "> " : "  ";
                        builder.AppendLine($"{prefix}7. Back");
                        string prefix2 = (CurrentSelection == selectable + 1) ? "> " : "  ";
                        builder.AppendLine($"{prefix2}9. Close");
                    }
                }
                else
                {
                    if (navCount == 2)
                    {
                        string prefix = (CurrentSelection == selectable + 0) ? "> " : "  ";
                        builder.AppendLine($"{prefix}8. Next");
                        string prefix2 = (CurrentSelection == selectable + 1) ? "> " : "  ";
                        builder.AppendLine($"{prefix2}9. Close");
                    }
                    else
                    {
                        string prefix = (CurrentSelection == selectable) ? "> " : "  ";
                        builder.AppendLine($"{prefix}9. Close");
                    }
                }
            }
            else
            {
                if (screenMenu.IsSubMenu)
                {
                    if (navCount == 3)
                    {
                        string prefix = (CurrentSelection == selectable + 0) ? "> " : "  ";
                        builder.AppendLine($"{prefix}7. Back");
                        string prefix2 = (CurrentSelection == selectable + 1) ? "> " : "  ";
                        builder.AppendLine($"{prefix2}8. Next");
                        string prefix3 = (CurrentSelection == selectable + 2) ? "> " : "  ";
                        builder.AppendLine($"{prefix3}9. Close");
                    }
                    else
                    {
                        string prefix = (CurrentSelection == selectable + 0) ? "> " : "  ";
                        builder.AppendLine($"{prefix}7. Back");
                        string prefix2 = (CurrentSelection == selectable + 1) ? "> " : "  ";
                        builder.AppendLine($"{prefix2}9. Close");
                    }
                }
                else
                {
                    if (navCount == 3)
                    {
                        string prefix = (CurrentSelection == selectable + 0) ? "> " : "  ";
                        builder.AppendLine($"{prefix}7. Back");
                        string prefix2 = (CurrentSelection == selectable + 1) ? "> " : "  ";
                        builder.AppendLine($"{prefix2}8. Next");
                        string prefix3 = (CurrentSelection == selectable + 2) ? "> " : "  ";
                        builder.AppendLine($"{prefix3}9. Close");
                    }
                    else if (navCount == 2)
                    {
                        string prefix = (CurrentSelection == selectable + 0) ? "> " : "  ";
                        builder.AppendLine($"{prefix}7. Back");
                        string prefix2 = (CurrentSelection == selectable + 1) ? "> " : "  ";
                        builder.AppendLine($"{prefix2}9. Close");
                    }
                }
            }

            builder.AppendLine();
            builder.AppendLine(" [W/S] Scroll" + "\n" + " [E] Select");
            builder.AppendLine(pad);

            string menuText = builder.ToString();

            if (_hudText == null)
            {
                _hudText = Create(
                    Player,
                    menuText,
                    35,
                    screenMenu.TextColor,
                    screenMenu.FontName,
                    -5.5f,
                    2.8f,
                    true,
                    true
                );
            }

            if (_hudText != null && _hudText.IsValid)
            {
                _hudText.AcceptInput("SetMessage", _hudText, _hudText, menuText);
            }
        }

        private void HandleSelection()
        {
            int selectable = Math.Min(NumPerPage, Menu.MenuOptions.Count - CurrentOffset);
            int navCount = 0;

            ScreenMenu? screenMenu = Menu as ScreenMenu;
            if (screenMenu != null)
            {
                if (screenMenu.IsSubMenu)
                {
                    navCount = (Page == 0)
                        ? (Menu.MenuOptions.Count > NumPerPage ? 3 : 2)
                        : ((Menu.MenuOptions.Count - CurrentOffset) > NumPerPage ? 3 : 2);
                }
                else
                {
                    navCount = (Page == 0)
                        ? (Menu.MenuOptions.Count > NumPerPage ? 2 : 1)
                        : ((Menu.MenuOptions.Count - CurrentOffset) > NumPerPage ? 3 : 2);
                }
            }
            else
            {
                navCount = (Page == 0)
                    ? (Menu.MenuOptions.Count > NumPerPage ? 2 : 1)
                    : ((Menu.MenuOptions.Count - CurrentOffset) > NumPerPage ? 3 : 2);
            }
            int totalLines = selectable + navCount;
            if (CurrentSelection < 0 || CurrentSelection >= totalLines)
                return;

            if (CurrentSelection < selectable)
            {
                int optionIndex = CurrentOffset + CurrentSelection;
                if (optionIndex < Menu.MenuOptions.Count)
                {
                    var option = Menu.MenuOptions[optionIndex];
                    if (!option.Disabled)
                    {
                        option.OnSelect(Player, option);

                        switch (Menu.PostSelectAction)
                        {
                            case PostSelectAction.Close:
                                Close();
                                break;
                            case PostSelectAction.Reset:
                                Reset();
                                break;
                            case PostSelectAction.Nothing:
                                break;
                        }
                    }
                }
            }
            else
            {
                int navIndex = CurrentSelection - selectable;
                if (screenMenu != null)
                {
                    if (screenMenu.IsSubMenu)
                    {
                        if (Page == 0)
                        {
                            if (navCount == 3)
                            {
                                if (navIndex == 0)
                                {
                                    if (screenMenu.ParentMenu != null)
                                    {
                                        Close();
                                        MenuManager.OpenScreenMenu(_plugin, Player, screenMenu.ParentMenu);
                                    }
                                }
                                else if (navIndex == 1)
                                {
                                    NextPage();
                                    CurrentSelection = 0;
                                }
                                else if (navIndex == 2)
                                {
                                    Close();
                                }
                            }
                            else
                            {
                                if (navIndex == 0)
                                {
                                    if (screenMenu.ParentMenu != null)
                                    {
                                        Close();
                                        MenuManager.OpenScreenMenu(_plugin, Player, screenMenu.ParentMenu);
                                    }
                                }
                                else if (navIndex == 1)
                                {
                                    Close();
                                }
                            }
                        }
                        else 
                        {
                            if (navCount == 3)
                            {
                                if (navIndex == 0)
                                {
                                    PrevPage();
                                    CurrentSelection = 0;
                                }
                                else if (navIndex == 1)
                                {
                                    NextPage();
                                    CurrentSelection = 0;
                                }
                                else if (navIndex == 2)
                                {
                                    if (screenMenu.ParentMenu != null)
                                    {
                                        Close();
                                        MenuManager.OpenScreenMenu(_plugin, Player, screenMenu.ParentMenu);
                                    }
                                }
                            }
                            else if (navCount == 2)
                            {
                                if (navIndex == 0)
                                {
                                    PrevPage();
                                    CurrentSelection = 0;
                                }
                                else if (navIndex == 1)
                                {
                                    if (screenMenu.ParentMenu != null)
                                    {
                                        Close();
                                        MenuManager.OpenScreenMenu(_plugin, Player, screenMenu.ParentMenu);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Page == 0)
                        {
                            if (navCount == 2)
                            {
                                if (navIndex == 0)
                                {
                                    NextPage();
                                    CurrentSelection = 0;
                                }
                                else if (navIndex == 1)
                                {
                                    Close();
                                }
                            }
                            else
                            {
                                Close();
                            }
                        }
                        else
                        {
                            if (navCount == 3)
                            {
                                if (navIndex == 0)
                                {
                                    PrevPage();
                                    CurrentSelection = 0;
                                }
                                else if (navIndex == 1)
                                {
                                    NextPage();
                                    CurrentSelection = 0;
                                }
                                else if (navIndex == 2)
                                {
                                    Close();
                                }
                            }
                            else if (navCount == 2)
                            {
                                if (navIndex == 0)
                                {
                                    PrevPage();
                                    CurrentSelection = 0;
                                }
                                else if (navIndex == 1)
                                {
                                    Close();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RecreateHud()
        {
            _hudText = null;
            Display();
        }

        public override void Close()
        {
            base.Close();
            if (_hudText != null)
            {
                _hudText.Enabled = false;
                _hudText.AcceptInput("Kill", _hudText);
                WorldTextOwners.Clear();
            }
            _plugin.RemoveListener("OnTick", _onTickDelegate);
        }
    }
}
