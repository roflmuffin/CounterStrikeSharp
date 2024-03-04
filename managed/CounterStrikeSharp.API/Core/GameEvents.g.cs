
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core
{
    
            [EventName("achievement_earned")]
            public class EventAchievementEarned : GameEvent
            {
                public EventAchievementEarned(IntPtr pointer) : base(pointer){}
                public EventAchievementEarned(bool force) : base("achievement_earned", force){}

                
                
                // entindex of the player
                public CCSPlayerController Player 
                {
                    get => Get<CCSPlayerController>("player");
                    set => Set<CCSPlayerController>("player", value);
                }

                
                // achievement ID
                public int Achievement 
                {
                    get => Get<int>("achievement");
                    set => Set<int>("achievement", value);
                }
            }

            [EventName("achievement_earned_local")]
            public class EventAchievementEarnedLocal : GameEvent
            {
                public EventAchievementEarnedLocal(IntPtr pointer) : base(pointer){}
                public EventAchievementEarnedLocal(bool force) : base("achievement_earned_local", force){}

                
                
                // achievement ID
                public int Achievement 
                {
                    get => Get<int>("achievement");
                    set => Set<int>("achievement", value);
                }

                
                // splitscreen ID
                public int Splitscreenplayer 
                {
                    get => Get<int>("splitscreenplayer");
                    set => Set<int>("splitscreenplayer", value);
                }
            }

            [EventName("achievement_event")]
            public class EventAchievementEvent : GameEvent
            {
                public EventAchievementEvent(IntPtr pointer) : base(pointer){}
                public EventAchievementEvent(bool force) : base("achievement_event", force){}

                
                
                // non-localized name of achievement
                public string AchievementName 
                {
                    get => Get<string>("achievement_name");
                    set => Set<string>("achievement_name", value);
                }

                
                // # of steps toward achievement
                public int CurVal 
                {
                    get => Get<int>("cur_val");
                    set => Set<int>("cur_val", value);
                }

                
                // total # of steps in achievement
                public int MaxVal 
                {
                    get => Get<int>("max_val");
                    set => Set<int>("max_val", value);
                }
            }

            [EventName("achievement_info_loaded")]
            public class EventAchievementInfoLoaded : GameEvent
            {
                public EventAchievementInfoLoaded(IntPtr pointer) : base(pointer){}
                public EventAchievementInfoLoaded(bool force) : base("achievement_info_loaded", force){}

                
            }

            [EventName("achievement_write_failed")]
            public class EventAchievementWriteFailed : GameEvent
            {
                public EventAchievementWriteFailed(IntPtr pointer) : base(pointer){}
                public EventAchievementWriteFailed(bool force) : base("achievement_write_failed", force){}

                
            }

            [EventName("add_bullet_hit_marker")]
            public class EventAddBulletHitMarker : GameEvent
            {
                public EventAddBulletHitMarker(IntPtr pointer) : base(pointer){}
                public EventAddBulletHitMarker(bool force) : base("add_bullet_hit_marker", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Bone 
                {
                    get => Get<int>("bone");
                    set => Set<int>("bone", value);
                }

                
                
                public int PosX 
                {
                    get => Get<int>("pos_x");
                    set => Set<int>("pos_x", value);
                }

                
                
                public int PosY 
                {
                    get => Get<int>("pos_y");
                    set => Set<int>("pos_y", value);
                }

                
                
                public int PosZ 
                {
                    get => Get<int>("pos_z");
                    set => Set<int>("pos_z", value);
                }

                
                
                public int AngX 
                {
                    get => Get<int>("ang_x");
                    set => Set<int>("ang_x", value);
                }

                
                
                public int AngY 
                {
                    get => Get<int>("ang_y");
                    set => Set<int>("ang_y", value);
                }

                
                
                public int AngZ 
                {
                    get => Get<int>("ang_z");
                    set => Set<int>("ang_z", value);
                }

                
                
                public int StartX 
                {
                    get => Get<int>("start_x");
                    set => Set<int>("start_x", value);
                }

                
                
                public int StartY 
                {
                    get => Get<int>("start_y");
                    set => Set<int>("start_y", value);
                }

                
                
                public int StartZ 
                {
                    get => Get<int>("start_z");
                    set => Set<int>("start_z", value);
                }

                
                
                public bool Hit 
                {
                    get => Get<bool>("hit");
                    set => Set<bool>("hit", value);
                }
            }

            [EventName("add_player_sonar_icon")]
            public class EventAddPlayerSonarIcon : GameEvent
            {
                public EventAddPlayerSonarIcon(IntPtr pointer) : base(pointer){}
                public EventAddPlayerSonarIcon(bool force) : base("add_player_sonar_icon", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public float PosX 
                {
                    get => Get<float>("pos_x");
                    set => Set<float>("pos_x", value);
                }

                
                
                public float PosY 
                {
                    get => Get<float>("pos_y");
                    set => Set<float>("pos_y", value);
                }

                
                
                public float PosZ 
                {
                    get => Get<float>("pos_z");
                    set => Set<float>("pos_z", value);
                }
            }

            [EventName("ammo_pickup")]
            public class EventAmmoPickup : GameEvent
            {
                public EventAmmoPickup(IntPtr pointer) : base(pointer){}
                public EventAmmoPickup(bool force) : base("ammo_pickup", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item 
                {
                    get => Get<string>("item");
                    set => Set<string>("item", value);
                }

                
                // the weapon entindex
                public long Index 
                {
                    get => Get<long>("index");
                    set => Set<long>("index", value);
                }
            }

            [EventName("ammo_refill")]
            public class EventAmmoRefill : GameEvent
            {
                public EventAmmoRefill(IntPtr pointer) : base(pointer){}
                public EventAmmoRefill(bool force) : base("ammo_refill", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public bool Success 
                {
                    get => Get<bool>("success");
                    set => Set<bool>("success", value);
                }
            }

            [EventName("announce_phase_end")]
            public class EventAnnouncePhaseEnd : GameEvent
            {
                public EventAnnouncePhaseEnd(IntPtr pointer) : base(pointer){}
                public EventAnnouncePhaseEnd(bool force) : base("announce_phase_end", force){}

                
            }

            [EventName("begin_new_match")]
            public class EventBeginNewMatch : GameEvent
            {
                public EventBeginNewMatch(IntPtr pointer) : base(pointer){}
                public EventBeginNewMatch(bool force) : base("begin_new_match", force){}

                
            }

            [EventName("bomb_abortdefuse")]
            public class EventBombAbortdefuse : GameEvent
            {
                public EventBombAbortdefuse(IntPtr pointer) : base(pointer){}
                public EventBombAbortdefuse(bool force) : base("bomb_abortdefuse", force){}

                
                
                // player who was defusing
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("bomb_abortplant")]
            public class EventBombAbortplant : GameEvent
            {
                public EventBombAbortplant(IntPtr pointer) : base(pointer){}
                public EventBombAbortplant(bool force) : base("bomb_abortplant", force){}

                
                
                // player who is planting the bomb
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => Get<int>("site");
                    set => Set<int>("site", value);
                }
            }

            [EventName("bomb_beep")]
            public class EventBombBeep : GameEvent
            {
                public EventBombBeep(IntPtr pointer) : base(pointer){}
                public EventBombBeep(bool force) : base("bomb_beep", force){}

                
                
                // c4 entity
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("bomb_begindefuse")]
            public class EventBombBegindefuse : GameEvent
            {
                public EventBombBegindefuse(IntPtr pointer) : base(pointer){}
                public EventBombBegindefuse(bool force) : base("bomb_begindefuse", force){}

                
                
                // player who is defusing
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public bool Haskit 
                {
                    get => Get<bool>("haskit");
                    set => Set<bool>("haskit", value);
                }
            }

            [EventName("bomb_beginplant")]
            public class EventBombBeginplant : GameEvent
            {
                public EventBombBeginplant(IntPtr pointer) : base(pointer){}
                public EventBombBeginplant(bool force) : base("bomb_beginplant", force){}

                
                
                // player who is planting the bomb
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => Get<int>("site");
                    set => Set<int>("site", value);
                }
            }

            [EventName("bomb_defused")]
            public class EventBombDefused : GameEvent
            {
                public EventBombDefused(IntPtr pointer) : base(pointer){}
                public EventBombDefused(bool force) : base("bomb_defused", force){}

                
                
                // player who defused the bomb
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => Get<int>("site");
                    set => Set<int>("site", value);
                }
            }

            [EventName("bomb_dropped")]
            public class EventBombDropped : GameEvent
            {
                public EventBombDropped(IntPtr pointer) : base(pointer){}
                public EventBombDropped(bool force) : base("bomb_dropped", force){}

                
                
                // player who dropped the bomb
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("bomb_exploded")]
            public class EventBombExploded : GameEvent
            {
                public EventBombExploded(IntPtr pointer) : base(pointer){}
                public EventBombExploded(bool force) : base("bomb_exploded", force){}

                
                
                // player who planted the bomb
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => Get<int>("site");
                    set => Set<int>("site", value);
                }
            }

            [EventName("bomb_pickup")]
            public class EventBombPickup : GameEvent
            {
                public EventBombPickup(IntPtr pointer) : base(pointer){}
                public EventBombPickup(bool force) : base("bomb_pickup", force){}

                
                
                // player pawn who picked up the bomb
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("bomb_planted")]
            public class EventBombPlanted : GameEvent
            {
                public EventBombPlanted(IntPtr pointer) : base(pointer){}
                public EventBombPlanted(bool force) : base("bomb_planted", force){}

                
                
                // player who planted the bomb
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => Get<int>("site");
                    set => Set<int>("site", value);
                }
            }

            [EventName("bonus_updated")]
            public class EventBonusUpdated : GameEvent
            {
                public EventBonusUpdated(IntPtr pointer) : base(pointer){}
                public EventBonusUpdated(bool force) : base("bonus_updated", force){}

                
                
                
                public int Numadvanced 
                {
                    get => Get<int>("numadvanced");
                    set => Set<int>("numadvanced", value);
                }

                
                
                public int Numbronze 
                {
                    get => Get<int>("numbronze");
                    set => Set<int>("numbronze", value);
                }

                
                
                public int Numsilver 
                {
                    get => Get<int>("numsilver");
                    set => Set<int>("numsilver", value);
                }

                
                
                public int Numgold 
                {
                    get => Get<int>("numgold");
                    set => Set<int>("numgold", value);
                }
            }

            [EventName("bot_takeover")]
            public class EventBotTakeover : GameEvent
            {
                public EventBotTakeover(IntPtr pointer) : base(pointer){}
                public EventBotTakeover(bool force) : base("bot_takeover", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public CCSPlayerController Botid 
                {
                    get => Get<CCSPlayerController>("botid");
                    set => Set<CCSPlayerController>("botid", value);
                }

                
                
                public float P 
                {
                    get => Get<float>("p");
                    set => Set<float>("p", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float R 
                {
                    get => Get<float>("r");
                    set => Set<float>("r", value);
                }
            }

            [EventName("break_breakable")]
            public class EventBreakBreakable : GameEvent
            {
                public EventBreakBreakable(IntPtr pointer) : base(pointer){}
                public EventBreakBreakable(bool force) : base("break_breakable", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }

                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // BREAK_GLASS, BREAK_WOOD, etc
                public int Material 
                {
                    get => Get<int>("material");
                    set => Set<int>("material", value);
                }
            }

            [EventName("break_prop")]
            public class EventBreakProp : GameEvent
            {
                public EventBreakProp(IntPtr pointer) : base(pointer){}
                public EventBreakProp(bool force) : base("break_prop", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }

                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("broken_breakable")]
            public class EventBrokenBreakable : GameEvent
            {
                public EventBrokenBreakable(IntPtr pointer) : base(pointer){}
                public EventBrokenBreakable(bool force) : base("broken_breakable", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }

                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // BREAK_GLASS, BREAK_WOOD, etc
                public int Material 
                {
                    get => Get<int>("material");
                    set => Set<int>("material", value);
                }
            }

            [EventName("bullet_flight_resolution")]
            public class EventBulletFlightResolution : GameEvent
            {
                public EventBulletFlightResolution(IntPtr pointer) : base(pointer){}
                public EventBulletFlightResolution(bool force) : base("bullet_flight_resolution", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int PosX 
                {
                    get => Get<int>("pos_x");
                    set => Set<int>("pos_x", value);
                }

                
                
                public int PosY 
                {
                    get => Get<int>("pos_y");
                    set => Set<int>("pos_y", value);
                }

                
                
                public int PosZ 
                {
                    get => Get<int>("pos_z");
                    set => Set<int>("pos_z", value);
                }

                
                
                public int AngX 
                {
                    get => Get<int>("ang_x");
                    set => Set<int>("ang_x", value);
                }

                
                
                public int AngY 
                {
                    get => Get<int>("ang_y");
                    set => Set<int>("ang_y", value);
                }

                
                
                public int AngZ 
                {
                    get => Get<int>("ang_z");
                    set => Set<int>("ang_z", value);
                }

                
                
                public int StartX 
                {
                    get => Get<int>("start_x");
                    set => Set<int>("start_x", value);
                }

                
                
                public int StartY 
                {
                    get => Get<int>("start_y");
                    set => Set<int>("start_y", value);
                }

                
                
                public int StartZ 
                {
                    get => Get<int>("start_z");
                    set => Set<int>("start_z", value);
                }
            }

            [EventName("bullet_impact")]
            public class EventBulletImpact : GameEvent
            {
                public EventBulletImpact(IntPtr pointer) : base(pointer){}
                public EventBulletImpact(bool force) : base("bullet_impact", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("buymenu_close")]
            public class EventBuymenuClose : GameEvent
            {
                public EventBuymenuClose(IntPtr pointer) : base(pointer){}
                public EventBuymenuClose(bool force) : base("buymenu_close", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("buymenu_open")]
            public class EventBuymenuOpen : GameEvent
            {
                public EventBuymenuOpen(IntPtr pointer) : base(pointer){}
                public EventBuymenuOpen(bool force) : base("buymenu_open", force){}

                
            }

            [EventName("buytime_ended")]
            public class EventBuytimeEnded : GameEvent
            {
                public EventBuytimeEnded(IntPtr pointer) : base(pointer){}
                public EventBuytimeEnded(bool force) : base("buytime_ended", force){}

                
            }

            [EventName("cart_updated")]
            public class EventCartUpdated : GameEvent
            {
                public EventCartUpdated(IntPtr pointer) : base(pointer){}
                public EventCartUpdated(bool force) : base("cart_updated", force){}

                
            }

            [EventName("choppers_incoming_warning")]
            public class EventChoppersIncomingWarning : GameEvent
            {
                public EventChoppersIncomingWarning(IntPtr pointer) : base(pointer){}
                public EventChoppersIncomingWarning(bool force) : base("choppers_incoming_warning", force){}

                
                
                
                public bool Global 
                {
                    get => Get<bool>("global");
                    set => Set<bool>("global", value);
                }
            }

            [EventName("client_disconnect")]
            public class EventClientDisconnect : GameEvent
            {
                public EventClientDisconnect(IntPtr pointer) : base(pointer){}
                public EventClientDisconnect(bool force) : base("client_disconnect", force){}

                
            }

            [EventName("client_loadout_changed")]
            public class EventClientLoadoutChanged : GameEvent
            {
                public EventClientLoadoutChanged(IntPtr pointer) : base(pointer){}
                public EventClientLoadoutChanged(bool force) : base("client_loadout_changed", force){}

                
            }

            [EventName("clientside_lesson_closed")]
            public class EventClientsideLessonClosed : GameEvent
            {
                public EventClientsideLessonClosed(IntPtr pointer) : base(pointer){}
                public EventClientsideLessonClosed(bool force) : base("clientside_lesson_closed", force){}

                
                
                
                public string LessonName 
                {
                    get => Get<string>("lesson_name");
                    set => Set<string>("lesson_name", value);
                }
            }

            [EventName("cs_game_disconnected")]
            public class EventCsGameDisconnected : GameEvent
            {
                public EventCsGameDisconnected(IntPtr pointer) : base(pointer){}
                public EventCsGameDisconnected(bool force) : base("cs_game_disconnected", force){}

                
            }

            [EventName("cs_intermission")]
            public class EventCsIntermission : GameEvent
            {
                public EventCsIntermission(IntPtr pointer) : base(pointer){}
                public EventCsIntermission(bool force) : base("cs_intermission", force){}

                
            }

            [EventName("cs_match_end_restart")]
            public class EventCsMatchEndRestart : GameEvent
            {
                public EventCsMatchEndRestart(IntPtr pointer) : base(pointer){}
                public EventCsMatchEndRestart(bool force) : base("cs_match_end_restart", force){}

                
            }

            [EventName("cs_pre_restart")]
            public class EventCsPreRestart : GameEvent
            {
                public EventCsPreRestart(IntPtr pointer) : base(pointer){}
                public EventCsPreRestart(bool force) : base("cs_pre_restart", force){}

                
            }

            [EventName("cs_prev_next_spectator")]
            public class EventCsPrevNextSpectator : GameEvent
            {
                public EventCsPrevNextSpectator(IntPtr pointer) : base(pointer){}
                public EventCsPrevNextSpectator(bool force) : base("cs_prev_next_spectator", force){}

                
                
                
                public bool Next 
                {
                    get => Get<bool>("next");
                    set => Set<bool>("next", value);
                }
            }

            [EventName("cs_round_final_beep")]
            public class EventCsRoundFinalBeep : GameEvent
            {
                public EventCsRoundFinalBeep(IntPtr pointer) : base(pointer){}
                public EventCsRoundFinalBeep(bool force) : base("cs_round_final_beep", force){}

                
            }

            [EventName("cs_round_start_beep")]
            public class EventCsRoundStartBeep : GameEvent
            {
                public EventCsRoundStartBeep(IntPtr pointer) : base(pointer){}
                public EventCsRoundStartBeep(bool force) : base("cs_round_start_beep", force){}

                
            }

            [EventName("cs_win_panel_match")]
            public class EventCsWinPanelMatch : GameEvent
            {
                public EventCsWinPanelMatch(IntPtr pointer) : base(pointer){}
                public EventCsWinPanelMatch(bool force) : base("cs_win_panel_match", force){}

                
            }

            [EventName("cs_win_panel_round")]
            public class EventCsWinPanelRound : GameEvent
            {
                public EventCsWinPanelRound(IntPtr pointer) : base(pointer){}
                public EventCsWinPanelRound(bool force) : base("cs_win_panel_round", force){}

                
                
                
                public bool ShowTimerDefend 
                {
                    get => Get<bool>("show_timer_defend");
                    set => Set<bool>("show_timer_defend", value);
                }

                
                
                public bool ShowTimerAttack 
                {
                    get => Get<bool>("show_timer_attack");
                    set => Set<bool>("show_timer_attack", value);
                }

                
                
                public int TimerTime 
                {
                    get => Get<int>("timer_time");
                    set => Set<int>("timer_time", value);
                }

                
                // define in cs_gamerules.h
                public int FinalEvent 
                {
                    get => Get<int>("final_event");
                    set => Set<int>("final_event", value);
                }

                
                
                public string FunfactToken 
                {
                    get => Get<string>("funfact_token");
                    set => Set<string>("funfact_token", value);
                }

                
                
                public CCSPlayerController FunfactPlayer 
                {
                    get => Get<CCSPlayerController>("funfact_player");
                    set => Set<CCSPlayerController>("funfact_player", value);
                }

                
                
                public long FunfactData1 
                {
                    get => Get<long>("funfact_data1");
                    set => Set<long>("funfact_data1", value);
                }

                
                
                public long FunfactData2 
                {
                    get => Get<long>("funfact_data2");
                    set => Set<long>("funfact_data2", value);
                }

                
                
                public long FunfactData3 
                {
                    get => Get<long>("funfact_data3");
                    set => Set<long>("funfact_data3", value);
                }
            }

            [EventName("decoy_detonate")]
            public class EventDecoyDetonate : GameEvent
            {
                public EventDecoyDetonate(IntPtr pointer) : base(pointer){}
                public EventDecoyDetonate(bool force) : base("decoy_detonate", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("decoy_firing")]
            public class EventDecoyFiring : GameEvent
            {
                public EventDecoyFiring(IntPtr pointer) : base(pointer){}
                public EventDecoyFiring(bool force) : base("decoy_firing", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("decoy_started")]
            public class EventDecoyStarted : GameEvent
            {
                public EventDecoyStarted(IntPtr pointer) : base(pointer){}
                public EventDecoyStarted(bool force) : base("decoy_started", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("defuser_dropped")]
            public class EventDefuserDropped : GameEvent
            {
                public EventDefuserDropped(IntPtr pointer) : base(pointer){}
                public EventDefuserDropped(bool force) : base("defuser_dropped", force){}

                
                
                // defuser's entity ID
                public long Entityid 
                {
                    get => Get<long>("entityid");
                    set => Set<long>("entityid", value);
                }
            }

            [EventName("defuser_pickup")]
            public class EventDefuserPickup : GameEvent
            {
                public EventDefuserPickup(IntPtr pointer) : base(pointer){}
                public EventDefuserPickup(bool force) : base("defuser_pickup", force){}

                
                
                // defuser's entity ID
                public long Entityid 
                {
                    get => Get<long>("entityid");
                    set => Set<long>("entityid", value);
                }

                
                // player who picked up the defuser
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("demo_skip")]
            public class EventDemoSkip : GameEvent
            {
                public EventDemoSkip(IntPtr pointer) : base(pointer){}
                public EventDemoSkip(bool force) : base("demo_skip", force){}

                
                
                
                public int Local 
                {
                    get => Get<int>("local");
                    set => Set<int>("local", value);
                }

                
                // current playback tick
                public long PlaybackTick 
                {
                    get => Get<long>("playback_tick");
                    set => Set<long>("playback_tick", value);
                }

                
                // tick we're going to
                public long SkiptoTick 
                {
                    get => Get<long>("skipto_tick");
                    set => Set<long>("skipto_tick", value);
                }

                
                // CSVCMsgList_UserMessages
                public int UserMessageList 
                {
                    get => Get<int>("user_message_list");
                    set => Set<int>("user_message_list", value);
                }

                
                // CSVCMsgList_GameEvents
                public int DotaHeroChaseList 
                {
                    get => Get<int>("dota_hero_chase_list");
                    set => Set<int>("dota_hero_chase_list", value);
                }
            }

            [EventName("demo_start")]
            public class EventDemoStart : GameEvent
            {
                public EventDemoStart(IntPtr pointer) : base(pointer){}
                public EventDemoStart(bool force) : base("demo_start", force){}

                
                
                
                public int Local 
                {
                    get => Get<int>("local");
                    set => Set<int>("local", value);
                }

                
                // CSVCMsgList_GameEvents that are combat log events
                public int DotaCombatlogList 
                {
                    get => Get<int>("dota_combatlog_list");
                    set => Set<int>("dota_combatlog_list", value);
                }

                
                // CSVCMsgList_GameEvents
                public int DotaHeroChaseList 
                {
                    get => Get<int>("dota_hero_chase_list");
                    set => Set<int>("dota_hero_chase_list", value);
                }

                
                // CSVCMsgList_GameEvents
                public int DotaPickHeroList 
                {
                    get => Get<int>("dota_pick_hero_list");
                    set => Set<int>("dota_pick_hero_list", value);
                }
            }

            [EventName("demo_stop")]
            public class EventDemoStop : GameEvent
            {
                public EventDemoStop(IntPtr pointer) : base(pointer){}
                public EventDemoStop(bool force) : base("demo_stop", force){}

                
            }

            [EventName("difficulty_changed")]
            public class EventDifficultyChanged : GameEvent
            {
                public EventDifficultyChanged(IntPtr pointer) : base(pointer){}
                public EventDifficultyChanged(bool force) : base("difficulty_changed", force){}

                
                
                
                public int Newdifficulty 
                {
                    get => Get<int>("newDifficulty");
                    set => Set<int>("newDifficulty", value);
                }

                
                
                public int Olddifficulty 
                {
                    get => Get<int>("oldDifficulty");
                    set => Set<int>("oldDifficulty", value);
                }

                
                // new difficulty as string
                public string Strdifficulty 
                {
                    get => Get<string>("strDifficulty");
                    set => Set<string>("strDifficulty", value);
                }
            }

            [EventName("dm_bonus_weapon_start")]
            public class EventDmBonusWeaponStart : GameEvent
            {
                public EventDmBonusWeaponStart(IntPtr pointer) : base(pointer){}
                public EventDmBonusWeaponStart(bool force) : base("dm_bonus_weapon_start", force){}

                
                
                // The length of time that this bonus lasts
                public int Time 
                {
                    get => Get<int>("time");
                    set => Set<int>("time", value);
                }

                
                // Loadout position of the bonus weapon
                public int Pos 
                {
                    get => Get<int>("Pos");
                    set => Set<int>("Pos", value);
                }
            }

            [EventName("door_break")]
            public class EventDoorBreak : GameEvent
            {
                public EventDoorBreak(IntPtr pointer) : base(pointer){}
                public EventDoorBreak(bool force) : base("door_break", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }

                
                
                public long Dmgstate 
                {
                    get => Get<long>("dmgstate");
                    set => Set<long>("dmgstate", value);
                }
            }

            [EventName("door_close")]
            public class EventDoorClose : GameEvent
            {
                public EventDoorClose(IntPtr pointer) : base(pointer){}
                public EventDoorClose(bool force) : base("door_close", force){}

                
                
                // Who closed the door
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // Is the door a checkpoint door
                public bool Checkpoint 
                {
                    get => Get<bool>("checkpoint");
                    set => Set<bool>("checkpoint", value);
                }
            }

            [EventName("door_closed")]
            public class EventDoorClosed : GameEvent
            {
                public EventDoorClosed(IntPtr pointer) : base(pointer){}
                public EventDoorClosed(bool force) : base("door_closed", force){}

                
                
                // Who closed the door
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("door_moving")]
            public class EventDoorMoving : GameEvent
            {
                public EventDoorMoving(IntPtr pointer) : base(pointer){}
                public EventDoorMoving(bool force) : base("door_moving", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("door_open")]
            public class EventDoorOpen : GameEvent
            {
                public EventDoorOpen(IntPtr pointer) : base(pointer){}
                public EventDoorOpen(bool force) : base("door_open", force){}

                
                
                // Who closed the door
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("drone_above_roof")]
            public class EventDroneAboveRoof : GameEvent
            {
                public EventDroneAboveRoof(IntPtr pointer) : base(pointer){}
                public EventDroneAboveRoof(bool force) : base("drone_above_roof", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Cargo 
                {
                    get => Get<int>("cargo");
                    set => Set<int>("cargo", value);
                }
            }

            [EventName("drone_cargo_detached")]
            public class EventDroneCargoDetached : GameEvent
            {
                public EventDroneCargoDetached(IntPtr pointer) : base(pointer){}
                public EventDroneCargoDetached(bool force) : base("drone_cargo_detached", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Cargo 
                {
                    get => Get<int>("cargo");
                    set => Set<int>("cargo", value);
                }

                
                
                public bool Delivered 
                {
                    get => Get<bool>("delivered");
                    set => Set<bool>("delivered", value);
                }
            }

            [EventName("drone_dispatched")]
            public class EventDroneDispatched : GameEvent
            {
                public EventDroneDispatched(IntPtr pointer) : base(pointer){}
                public EventDroneDispatched(bool force) : base("drone_dispatched", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Priority 
                {
                    get => Get<int>("priority");
                    set => Set<int>("priority", value);
                }

                
                
                public int DroneDispatchedParam 
                {
                    get => Get<int>("drone_dispatched");
                    set => Set<int>("drone_dispatched", value);
                }
            }

            [EventName("dronegun_attack")]
            public class EventDronegunAttack : GameEvent
            {
                public EventDronegunAttack(IntPtr pointer) : base(pointer){}
                public EventDronegunAttack(bool force) : base("dronegun_attack", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("drop_rate_modified")]
            public class EventDropRateModified : GameEvent
            {
                public EventDropRateModified(IntPtr pointer) : base(pointer){}
                public EventDropRateModified(bool force) : base("drop_rate_modified", force){}

                
            }

            [EventName("dynamic_shadow_light_changed")]
            public class EventDynamicShadowLightChanged : GameEvent
            {
                public EventDynamicShadowLightChanged(IntPtr pointer) : base(pointer){}
                public EventDynamicShadowLightChanged(bool force) : base("dynamic_shadow_light_changed", force){}

                
            }

            [EventName("dz_item_interaction")]
            public class EventDzItemInteraction : GameEvent
            {
                public EventDzItemInteraction(IntPtr pointer) : base(pointer){}
                public EventDzItemInteraction(bool force) : base("dz_item_interaction", force){}

                
                
                // player entindex
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // crate entindex
                public int Subject 
                {
                    get => Get<int>("subject");
                    set => Set<int>("subject", value);
                }

                
                // type of crate (metal, wood, or paradrop)
                public string Type 
                {
                    get => Get<string>("type");
                    set => Set<string>("type", value);
                }
            }

            [EventName("enable_restart_voting")]
            public class EventEnableRestartVoting : GameEvent
            {
                public EventEnableRestartVoting(IntPtr pointer) : base(pointer){}
                public EventEnableRestartVoting(bool force) : base("enable_restart_voting", force){}

                
                
                
                public bool Enable 
                {
                    get => Get<bool>("enable");
                    set => Set<bool>("enable", value);
                }
            }

            [EventName("endmatch_cmm_start_reveal_items")]
            public class EventEndmatchCmmStartRevealItems : GameEvent
            {
                public EventEndmatchCmmStartRevealItems(IntPtr pointer) : base(pointer){}
                public EventEndmatchCmmStartRevealItems(bool force) : base("endmatch_cmm_start_reveal_items", force){}

                
            }

            [EventName("endmatch_mapvote_selecting_map")]
            public class EventEndmatchMapvoteSelectingMap : GameEvent
            {
                public EventEndmatchMapvoteSelectingMap(IntPtr pointer) : base(pointer){}
                public EventEndmatchMapvoteSelectingMap(bool force) : base("endmatch_mapvote_selecting_map", force){}

                
                
                // Number of "ties"
                public int Count 
                {
                    get => Get<int>("count");
                    set => Set<int>("count", value);
                }

                
                
                public int Slot1 
                {
                    get => Get<int>("slot1");
                    set => Set<int>("slot1", value);
                }

                
                
                public int Slot2 
                {
                    get => Get<int>("slot2");
                    set => Set<int>("slot2", value);
                }

                
                
                public int Slot3 
                {
                    get => Get<int>("slot3");
                    set => Set<int>("slot3", value);
                }

                
                
                public int Slot4 
                {
                    get => Get<int>("slot4");
                    set => Set<int>("slot4", value);
                }

                
                
                public int Slot5 
                {
                    get => Get<int>("slot5");
                    set => Set<int>("slot5", value);
                }

                
                
                public int Slot6 
                {
                    get => Get<int>("slot6");
                    set => Set<int>("slot6", value);
                }

                
                
                public int Slot7 
                {
                    get => Get<int>("slot7");
                    set => Set<int>("slot7", value);
                }

                
                
                public int Slot8 
                {
                    get => Get<int>("slot8");
                    set => Set<int>("slot8", value);
                }

                
                
                public int Slot9 
                {
                    get => Get<int>("slot9");
                    set => Set<int>("slot9", value);
                }

                
                
                public int Slot10 
                {
                    get => Get<int>("slot10");
                    set => Set<int>("slot10", value);
                }
            }

            [EventName("enter_bombzone")]
            public class EventEnterBombzone : GameEvent
            {
                public EventEnterBombzone(IntPtr pointer) : base(pointer){}
                public EventEnterBombzone(bool force) : base("enter_bombzone", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public bool Hasbomb 
                {
                    get => Get<bool>("hasbomb");
                    set => Set<bool>("hasbomb", value);
                }

                
                
                public bool Isplanted 
                {
                    get => Get<bool>("isplanted");
                    set => Set<bool>("isplanted", value);
                }
            }

            [EventName("enter_buyzone")]
            public class EventEnterBuyzone : GameEvent
            {
                public EventEnterBuyzone(IntPtr pointer) : base(pointer){}
                public EventEnterBuyzone(bool force) : base("enter_buyzone", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public bool Canbuy 
                {
                    get => Get<bool>("canbuy");
                    set => Set<bool>("canbuy", value);
                }
            }

            [EventName("enter_rescue_zone")]
            public class EventEnterRescueZone : GameEvent
            {
                public EventEnterRescueZone(IntPtr pointer) : base(pointer){}
                public EventEnterRescueZone(bool force) : base("enter_rescue_zone", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("entity_killed")]
            public class EventEntityKilled : GameEvent
            {
                public EventEntityKilled(IntPtr pointer) : base(pointer){}
                public EventEntityKilled(bool force) : base("entity_killed", force){}

                
                
                
                public long EntindexKilled 
                {
                    get => Get<long>("entindex_killed");
                    set => Set<long>("entindex_killed", value);
                }

                
                
                public long EntindexAttacker 
                {
                    get => Get<long>("entindex_attacker");
                    set => Set<long>("entindex_attacker", value);
                }

                
                
                public long EntindexInflictor 
                {
                    get => Get<long>("entindex_inflictor");
                    set => Set<long>("entindex_inflictor", value);
                }

                
                
                public long Damagebits 
                {
                    get => Get<long>("damagebits");
                    set => Set<long>("damagebits", value);
                }
            }

            [EventName("entity_visible")]
            public class EventEntityVisible : GameEvent
            {
                public EventEntityVisible(IntPtr pointer) : base(pointer){}
                public EventEntityVisible(bool force) : base("entity_visible", force){}

                
                
                // The player who sees the entity
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // Entindex of the entity they see
                public int Subject 
                {
                    get => Get<int>("subject");
                    set => Set<int>("subject", value);
                }

                
                // Classname of the entity they see
                public string Classname 
                {
                    get => Get<string>("classname");
                    set => Set<string>("classname", value);
                }

                
                // name of the entity they see
                public string Entityname 
                {
                    get => Get<string>("entityname");
                    set => Set<string>("entityname", value);
                }
            }

            [EventName("event_ticket_modified")]
            public class EventEventTicketModified : GameEvent
            {
                public EventEventTicketModified(IntPtr pointer) : base(pointer){}
                public EventEventTicketModified(bool force) : base("event_ticket_modified", force){}

                
            }

            [EventName("exit_bombzone")]
            public class EventExitBombzone : GameEvent
            {
                public EventExitBombzone(IntPtr pointer) : base(pointer){}
                public EventExitBombzone(bool force) : base("exit_bombzone", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public bool Hasbomb 
                {
                    get => Get<bool>("hasbomb");
                    set => Set<bool>("hasbomb", value);
                }

                
                
                public bool Isplanted 
                {
                    get => Get<bool>("isplanted");
                    set => Set<bool>("isplanted", value);
                }
            }

            [EventName("exit_buyzone")]
            public class EventExitBuyzone : GameEvent
            {
                public EventExitBuyzone(IntPtr pointer) : base(pointer){}
                public EventExitBuyzone(bool force) : base("exit_buyzone", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public bool Canbuy 
                {
                    get => Get<bool>("canbuy");
                    set => Set<bool>("canbuy", value);
                }
            }

            [EventName("exit_rescue_zone")]
            public class EventExitRescueZone : GameEvent
            {
                public EventExitRescueZone(IntPtr pointer) : base(pointer){}
                public EventExitRescueZone(bool force) : base("exit_rescue_zone", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("finale_start")]
            public class EventFinaleStart : GameEvent
            {
                public EventFinaleStart(IntPtr pointer) : base(pointer){}
                public EventFinaleStart(bool force) : base("finale_start", force){}

                
                
                
                public int Rushes 
                {
                    get => Get<int>("rushes");
                    set => Set<int>("rushes", value);
                }
            }

            [EventName("firstbombs_incoming_warning")]
            public class EventFirstbombsIncomingWarning : GameEvent
            {
                public EventFirstbombsIncomingWarning(IntPtr pointer) : base(pointer){}
                public EventFirstbombsIncomingWarning(bool force) : base("firstbombs_incoming_warning", force){}

                
                
                
                public bool Global 
                {
                    get => Get<bool>("global");
                    set => Set<bool>("global", value);
                }
            }

            [EventName("flare_ignite_npc")]
            public class EventFlareIgniteNpc : GameEvent
            {
                public EventFlareIgniteNpc(IntPtr pointer) : base(pointer){}
                public EventFlareIgniteNpc(bool force) : base("flare_ignite_npc", force){}

                
                
                // entity ignited
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("flashbang_detonate")]
            public class EventFlashbangDetonate : GameEvent
            {
                public EventFlashbangDetonate(IntPtr pointer) : base(pointer){}
                public EventFlashbangDetonate(bool force) : base("flashbang_detonate", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("game_end")]
            public class EventGameEnd : GameEvent
            {
                public EventGameEnd(IntPtr pointer) : base(pointer){}
                public EventGameEnd(bool force) : base("game_end", force){}

                
                
                // winner team/user id
                public int Winner 
                {
                    get => Get<int>("winner");
                    set => Set<int>("winner", value);
                }
            }

            [EventName("game_init")]
            public class EventGameInit : GameEvent
            {
                public EventGameInit(IntPtr pointer) : base(pointer){}
                public EventGameInit(bool force) : base("game_init", force){}

                
            }

            [EventName("gameinstructor_draw")]
            public class EventGameinstructorDraw : GameEvent
            {
                public EventGameinstructorDraw(IntPtr pointer) : base(pointer){}
                public EventGameinstructorDraw(bool force) : base("gameinstructor_draw", force){}

                
            }

            [EventName("gameinstructor_nodraw")]
            public class EventGameinstructorNodraw : GameEvent
            {
                public EventGameinstructorNodraw(IntPtr pointer) : base(pointer){}
                public EventGameinstructorNodraw(bool force) : base("gameinstructor_nodraw", force){}

                
            }

            [EventName("game_message")]
            public class EventGameMessage : GameEvent
            {
                public EventGameMessage(IntPtr pointer) : base(pointer){}
                public EventGameMessage(bool force) : base("game_message", force){}

                
                
                // 0 = console, 1 = HUD
                public int Target 
                {
                    get => Get<int>("target");
                    set => Set<int>("target", value);
                }

                
                // the message text
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }
            }

            [EventName("game_newmap")]
            public class EventGameNewmap : GameEvent
            {
                public EventGameNewmap(IntPtr pointer) : base(pointer){}
                public EventGameNewmap(bool force) : base("game_newmap", force){}

                
                
                // map name
                public string Mapname 
                {
                    get => Get<string>("mapname");
                    set => Set<string>("mapname", value);
                }
            }

            [EventName("game_phase_changed")]
            public class EventGamePhaseChanged : GameEvent
            {
                public EventGamePhaseChanged(IntPtr pointer) : base(pointer){}
                public EventGamePhaseChanged(bool force) : base("game_phase_changed", force){}

                
                
                
                public int NewPhase 
                {
                    get => Get<int>("new_phase");
                    set => Set<int>("new_phase", value);
                }
            }

            [EventName("game_start")]
            public class EventGameStart : GameEvent
            {
                public EventGameStart(IntPtr pointer) : base(pointer){}
                public EventGameStart(bool force) : base("game_start", force){}

                
                
                // max round
                public long Roundslimit 
                {
                    get => Get<long>("roundslimit");
                    set => Set<long>("roundslimit", value);
                }

                
                // time limit
                public long Timelimit 
                {
                    get => Get<long>("timelimit");
                    set => Set<long>("timelimit", value);
                }

                
                // frag limit
                public long Fraglimit 
                {
                    get => Get<long>("fraglimit");
                    set => Set<long>("fraglimit", value);
                }

                
                // round objective
                public string Objective 
                {
                    get => Get<string>("objective");
                    set => Set<string>("objective", value);
                }
            }

            [EventName("gameui_hidden")]
            public class EventGameuiHidden : GameEvent
            {
                public EventGameuiHidden(IntPtr pointer) : base(pointer){}
                public EventGameuiHidden(bool force) : base("gameui_hidden", force){}

                
            }

            [EventName("gc_connected")]
            public class EventGcConnected : GameEvent
            {
                public EventGcConnected(IntPtr pointer) : base(pointer){}
                public EventGcConnected(bool force) : base("gc_connected", force){}

                
            }

            [EventName("gg_killed_enemy")]
            public class EventGgKilledEnemy : GameEvent
            {
                public EventGgKilledEnemy(IntPtr pointer) : base(pointer){}
                public EventGgKilledEnemy(bool force) : base("gg_killed_enemy", force){}

                
                
                // user ID who died
                public CCSPlayerController Victimid 
                {
                    get => Get<CCSPlayerController>("victimid");
                    set => Set<CCSPlayerController>("victimid", value);
                }

                
                // user ID who killed
                public CCSPlayerController Attackerid 
                {
                    get => Get<CCSPlayerController>("attackerid");
                    set => Set<CCSPlayerController>("attackerid", value);
                }

                
                // did killer dominate victim with this kill
                public int Dominated 
                {
                    get => Get<int>("dominated");
                    set => Set<int>("dominated", value);
                }

                
                // did killer get revenge on victim with this kill
                public int Revenge 
                {
                    get => Get<int>("revenge");
                    set => Set<int>("revenge", value);
                }

                
                // did killer kill with a bonus weapon?
                public bool Bonus 
                {
                    get => Get<bool>("bonus");
                    set => Set<bool>("bonus", value);
                }
            }

            [EventName("grenade_bounce")]
            public class EventGrenadeBounce : GameEvent
            {
                public EventGrenadeBounce(IntPtr pointer) : base(pointer){}
                public EventGrenadeBounce(bool force) : base("grenade_bounce", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("grenade_thrown")]
            public class EventGrenadeThrown : GameEvent
            {
                public EventGrenadeThrown(IntPtr pointer) : base(pointer){}
                public EventGrenadeThrown(bool force) : base("grenade_thrown", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // weapon name used
                public string Weapon 
                {
                    get => Get<string>("weapon");
                    set => Set<string>("weapon", value);
                }
            }

            [EventName("guardian_wave_restart")]
            public class EventGuardianWaveRestart : GameEvent
            {
                public EventGuardianWaveRestart(IntPtr pointer) : base(pointer){}
                public EventGuardianWaveRestart(bool force) : base("guardian_wave_restart", force){}

                
            }

            [EventName("hegrenade_detonate")]
            public class EventHegrenadeDetonate : GameEvent
            {
                public EventHegrenadeDetonate(IntPtr pointer) : base(pointer){}
                public EventHegrenadeDetonate(bool force) : base("hegrenade_detonate", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("helicopter_grenade_punt_miss")]
            public class EventHelicopterGrenadePuntMiss : GameEvent
            {
                public EventHelicopterGrenadePuntMiss(IntPtr pointer) : base(pointer){}
                public EventHelicopterGrenadePuntMiss(bool force) : base("helicopter_grenade_punt_miss", force){}

                
            }

            [EventName("hide_deathpanel")]
            public class EventHideDeathpanel : GameEvent
            {
                public EventHideDeathpanel(IntPtr pointer) : base(pointer){}
                public EventHideDeathpanel(bool force) : base("hide_deathpanel", force){}

                
            }

            [EventName("hltv_cameraman")]
            public class EventHltvCameraman : GameEvent
            {
                public EventHltvCameraman(IntPtr pointer) : base(pointer){}
                public EventHltvCameraman(bool force) : base("hltv_cameraman", force){}

                
                
                // camera man entity index
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("hltv_changed_mode")]
            public class EventHltvChangedMode : GameEvent
            {
                public EventHltvChangedMode(IntPtr pointer) : base(pointer){}
                public EventHltvChangedMode(bool force) : base("hltv_changed_mode", force){}

                
                
                
                public long Oldmode 
                {
                    get => Get<long>("oldmode");
                    set => Set<long>("oldmode", value);
                }

                
                
                public long Newmode 
                {
                    get => Get<long>("newmode");
                    set => Set<long>("newmode", value);
                }

                
                
                public long ObsTarget 
                {
                    get => Get<long>("obs_target");
                    set => Set<long>("obs_target", value);
                }
            }

            [EventName("hltv_chase")]
            public class EventHltvChase : GameEvent
            {
                public EventHltvChase(IntPtr pointer) : base(pointer){}
                public EventHltvChase(bool force) : base("hltv_chase", force){}

                
                
                // primary traget index
                public CCSPlayerController Target1 
                {
                    get => Get<CCSPlayerController>("target1");
                    set => Set<CCSPlayerController>("target1", value);
                }

                
                // secondary traget index or 0
                public CCSPlayerController Target2 
                {
                    get => Get<CCSPlayerController>("target2");
                    set => Set<CCSPlayerController>("target2", value);
                }

                
                // camera distance
                public int Distance 
                {
                    get => Get<int>("distance");
                    set => Set<int>("distance", value);
                }

                
                // view angle horizontal
                public int Theta 
                {
                    get => Get<int>("theta");
                    set => Set<int>("theta", value);
                }

                
                // view angle vertical
                public int Phi 
                {
                    get => Get<int>("phi");
                    set => Set<int>("phi", value);
                }

                
                // camera inertia
                public int Inertia 
                {
                    get => Get<int>("inertia");
                    set => Set<int>("inertia", value);
                }

                
                // diretcor suggests to show ineye
                public int Ineye 
                {
                    get => Get<int>("ineye");
                    set => Set<int>("ineye", value);
                }
            }

            [EventName("hltv_chat")]
            public class EventHltvChat : GameEvent
            {
                public EventHltvChat(IntPtr pointer) : base(pointer){}
                public EventHltvChat(bool force) : base("hltv_chat", force){}

                
                
                
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }

                
                // steam id
                public ulong Steamid 
                {
                    get => Get<ulong>("steamID");
                    set => Set<ulong>("steamID", value);
                }
            }

            [EventName("hltv_fixed")]
            public class EventHltvFixed : GameEvent
            {
                public EventHltvFixed(IntPtr pointer) : base(pointer){}
                public EventHltvFixed(bool force) : base("hltv_fixed", force){}

                
                
                // camera position in world
                public long Posx 
                {
                    get => Get<long>("posx");
                    set => Set<long>("posx", value);
                }

                
                
                public long Posy 
                {
                    get => Get<long>("posy");
                    set => Set<long>("posy", value);
                }

                
                
                public long Posz 
                {
                    get => Get<long>("posz");
                    set => Set<long>("posz", value);
                }

                
                // camera angles
                public int Theta 
                {
                    get => Get<int>("theta");
                    set => Set<int>("theta", value);
                }

                
                
                public int Phi 
                {
                    get => Get<int>("phi");
                    set => Set<int>("phi", value);
                }

                
                
                public int Offset 
                {
                    get => Get<int>("offset");
                    set => Set<int>("offset", value);
                }

                
                
                public float Fov 
                {
                    get => Get<float>("fov");
                    set => Set<float>("fov", value);
                }

                
                // follow this player
                public CCSPlayerController Target 
                {
                    get => Get<CCSPlayerController>("target");
                    set => Set<CCSPlayerController>("target", value);
                }
            }

            [EventName("hltv_message")]
            public class EventHltvMessage : GameEvent
            {
                public EventHltvMessage(IntPtr pointer) : base(pointer){}
                public EventHltvMessage(bool force) : base("hltv_message", force){}

                
                
                
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }
            }

            [EventName("hltv_rank_camera")]
            public class EventHltvRankCamera : GameEvent
            {
                public EventHltvRankCamera(IntPtr pointer) : base(pointer){}
                public EventHltvRankCamera(bool force) : base("hltv_rank_camera", force){}

                
                
                // fixed camera index
                public int Index 
                {
                    get => Get<int>("index");
                    set => Set<int>("index", value);
                }

                
                // ranking, how interesting is this camera view
                public float Rank 
                {
                    get => Get<float>("rank");
                    set => Set<float>("rank", value);
                }

                
                // best/closest target entity
                public CCSPlayerController Target 
                {
                    get => Get<CCSPlayerController>("target");
                    set => Set<CCSPlayerController>("target", value);
                }
            }

            [EventName("hltv_rank_entity")]
            public class EventHltvRankEntity : GameEvent
            {
                public EventHltvRankEntity(IntPtr pointer) : base(pointer){}
                public EventHltvRankEntity(bool force) : base("hltv_rank_entity", force){}

                
                
                // player slot
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // ranking, how interesting is this entity to view
                public float Rank 
                {
                    get => Get<float>("rank");
                    set => Set<float>("rank", value);
                }

                
                // best/closest target entity
                public CCSPlayerController Target 
                {
                    get => Get<CCSPlayerController>("target");
                    set => Set<CCSPlayerController>("target", value);
                }
            }

            [EventName("hltv_replay")]
            public class EventHltvReplay : GameEvent
            {
                public EventHltvReplay(IntPtr pointer) : base(pointer){}
                public EventHltvReplay(bool force) : base("hltv_replay", force){}

                
                
                // number of seconds in killer replay delay
                public long Delay 
                {
                    get => Get<long>("delay");
                    set => Set<long>("delay", value);
                }

                
                // reason for replay	(ReplayEventType_t)
                public long Reason 
                {
                    get => Get<long>("reason");
                    set => Set<long>("reason", value);
                }
            }

            [EventName("hltv_replay_status")]
            public class EventHltvReplayStatus : GameEvent
            {
                public EventHltvReplayStatus(IntPtr pointer) : base(pointer){}
                public EventHltvReplayStatus(bool force) : base("hltv_replay_status", force){}

                
                
                // reason for hltv replay status change ()
                public long Reason 
                {
                    get => Get<long>("reason");
                    set => Set<long>("reason", value);
                }
            }

            [EventName("hltv_status")]
            public class EventHltvStatus : GameEvent
            {
                public EventHltvStatus(IntPtr pointer) : base(pointer){}
                public EventHltvStatus(bool force) : base("hltv_status", force){}

                
                
                // number of HLTV spectators
                public long Clients 
                {
                    get => Get<long>("clients");
                    set => Set<long>("clients", value);
                }

                
                // number of HLTV slots
                public long Slots 
                {
                    get => Get<long>("slots");
                    set => Set<long>("slots", value);
                }

                
                // number of HLTV proxies
                public int Proxies 
                {
                    get => Get<int>("proxies");
                    set => Set<int>("proxies", value);
                }

                
                // disptach master IP:port
                public string Master 
                {
                    get => Get<string>("master");
                    set => Set<string>("master", value);
                }
            }

            [EventName("hltv_title")]
            public class EventHltvTitle : GameEvent
            {
                public EventHltvTitle(IntPtr pointer) : base(pointer){}
                public EventHltvTitle(bool force) : base("hltv_title", force){}

                
                
                
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }
            }

            [EventName("hltv_versioninfo")]
            public class EventHltvVersioninfo : GameEvent
            {
                public EventHltvVersioninfo(IntPtr pointer) : base(pointer){}
                public EventHltvVersioninfo(bool force) : base("hltv_versioninfo", force){}

                
                
                
                public long Version 
                {
                    get => Get<long>("version");
                    set => Set<long>("version", value);
                }
            }

            [EventName("hostage_call_for_help")]
            public class EventHostageCallForHelp : GameEvent
            {
                public EventHostageCallForHelp(IntPtr pointer) : base(pointer){}
                public EventHostageCallForHelp(bool force) : base("hostage_call_for_help", force){}

                
                
                // hostage entity index
                public int Hostage 
                {
                    get => Get<int>("hostage");
                    set => Set<int>("hostage", value);
                }
            }

            [EventName("hostage_follows")]
            public class EventHostageFollows : GameEvent
            {
                public EventHostageFollows(IntPtr pointer) : base(pointer){}
                public EventHostageFollows(bool force) : base("hostage_follows", force){}

                
                
                // player who touched the hostage
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => Get<int>("hostage");
                    set => Set<int>("hostage", value);
                }
            }

            [EventName("hostage_hurt")]
            public class EventHostageHurt : GameEvent
            {
                public EventHostageHurt(IntPtr pointer) : base(pointer){}
                public EventHostageHurt(bool force) : base("hostage_hurt", force){}

                
                
                // player who hurt the hostage
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => Get<int>("hostage");
                    set => Set<int>("hostage", value);
                }
            }

            [EventName("hostage_killed")]
            public class EventHostageKilled : GameEvent
            {
                public EventHostageKilled(IntPtr pointer) : base(pointer){}
                public EventHostageKilled(bool force) : base("hostage_killed", force){}

                
                
                // player who killed the hostage
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => Get<int>("hostage");
                    set => Set<int>("hostage", value);
                }
            }

            [EventName("hostage_rescued")]
            public class EventHostageRescued : GameEvent
            {
                public EventHostageRescued(IntPtr pointer) : base(pointer){}
                public EventHostageRescued(bool force) : base("hostage_rescued", force){}

                
                
                // player who rescued the hostage
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => Get<int>("hostage");
                    set => Set<int>("hostage", value);
                }

                
                // rescue site index
                public int Site 
                {
                    get => Get<int>("site");
                    set => Set<int>("site", value);
                }
            }

            [EventName("hostage_rescued_all")]
            public class EventHostageRescuedAll : GameEvent
            {
                public EventHostageRescuedAll(IntPtr pointer) : base(pointer){}
                public EventHostageRescuedAll(bool force) : base("hostage_rescued_all", force){}

                
            }

            [EventName("hostage_stops_following")]
            public class EventHostageStopsFollowing : GameEvent
            {
                public EventHostageStopsFollowing(IntPtr pointer) : base(pointer){}
                public EventHostageStopsFollowing(bool force) : base("hostage_stops_following", force){}

                
                
                // player who rescued the hostage
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => Get<int>("hostage");
                    set => Set<int>("hostage", value);
                }
            }

            [EventName("hostname_changed")]
            public class EventHostnameChanged : GameEvent
            {
                public EventHostnameChanged(IntPtr pointer) : base(pointer){}
                public EventHostnameChanged(bool force) : base("hostname_changed", force){}

                
                
                
                public string Hostname 
                {
                    get => Get<string>("hostname");
                    set => Set<string>("hostname", value);
                }
            }

            [EventName("inferno_expire")]
            public class EventInfernoExpire : GameEvent
            {
                public EventInfernoExpire(IntPtr pointer) : base(pointer){}
                public EventInfernoExpire(bool force) : base("inferno_expire", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("inferno_extinguish")]
            public class EventInfernoExtinguish : GameEvent
            {
                public EventInfernoExtinguish(IntPtr pointer) : base(pointer){}
                public EventInfernoExtinguish(bool force) : base("inferno_extinguish", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("inferno_startburn")]
            public class EventInfernoStartburn : GameEvent
            {
                public EventInfernoStartburn(IntPtr pointer) : base(pointer){}
                public EventInfernoStartburn(bool force) : base("inferno_startburn", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("inspect_weapon")]
            public class EventInspectWeapon : GameEvent
            {
                public EventInspectWeapon(IntPtr pointer) : base(pointer){}
                public EventInspectWeapon(bool force) : base("inspect_weapon", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("instructor_close_lesson")]
            public class EventInstructorCloseLesson : GameEvent
            {
                public EventInstructorCloseLesson(IntPtr pointer) : base(pointer){}
                public EventInstructorCloseLesson(bool force) : base("instructor_close_lesson", force){}

                
                
                // The player who this lesson is intended for
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // Name of the lesson to start.  Must match instructor_lesson.txt
                public string HintName 
                {
                    get => Get<string>("hint_name");
                    set => Set<string>("hint_name", value);
                }
            }

            [EventName("instructor_server_hint_create")]
            public class EventInstructorServerHintCreate : GameEvent
            {
                public EventInstructorServerHintCreate(IntPtr pointer) : base(pointer){}
                public EventInstructorServerHintCreate(bool force) : base("instructor_server_hint_create", force){}

                
                
                // user ID of the player that triggered the hint
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // what to name the hint. For referencing it again later (e.g. a kill command for the hint instead of a timeout)
                public string HintName 
                {
                    get => Get<string>("hint_name");
                    set => Set<string>("hint_name", value);
                }

                
                // type name so that messages of the same type will replace each other
                public string HintReplaceKey 
                {
                    get => Get<string>("hint_replace_key");
                    set => Set<string>("hint_replace_key", value);
                }

                
                // entity id that the hint should display at
                public long HintTarget 
                {
                    get => Get<long>("hint_target");
                    set => Set<long>("hint_target", value);
                }

                
                // userid id of the activator
                public CCSPlayerController HintActivatorUserid 
                {
                    get => Get<CCSPlayerController>("hint_activator_userid");
                    set => Set<CCSPlayerController>("hint_activator_userid", value);
                }

                
                // how long in seconds until the hint automatically times out, 0 = never
                public int HintTimeout 
                {
                    get => Get<int>("hint_timeout");
                    set => Set<int>("hint_timeout", value);
                }

                
                // the hint icon to use when the hint is onscreen. e.g. "icon_alert_red"
                public string HintIconOnscreen 
                {
                    get => Get<string>("hint_icon_onscreen");
                    set => Set<string>("hint_icon_onscreen", value);
                }

                
                // the hint icon to use when the hint is offscreen. e.g. "icon_alert"
                public string HintIconOffscreen 
                {
                    get => Get<string>("hint_icon_offscreen");
                    set => Set<string>("hint_icon_offscreen", value);
                }

                
                // the hint caption. e.g. "#ThisIsDangerous"
                public string HintCaption 
                {
                    get => Get<string>("hint_caption");
                    set => Set<string>("hint_caption", value);
                }

                
                // the hint caption that only the activator sees e.g. "#YouPushedItGood"
                public string HintActivatorCaption 
                {
                    get => Get<string>("hint_activator_caption");
                    set => Set<string>("hint_activator_caption", value);
                }

                
                // the hint color in "r,g,b" format where each component is 0-255
                public string HintColor 
                {
                    get => Get<string>("hint_color");
                    set => Set<string>("hint_color", value);
                }

                
                // how far on the z axis to offset the hint from entity origin
                public float HintIconOffset 
                {
                    get => Get<float>("hint_icon_offset");
                    set => Set<float>("hint_icon_offset", value);
                }

                
                // range before the hint is culled
                public float HintRange 
                {
                    get => Get<float>("hint_range");
                    set => Set<float>("hint_range", value);
                }

                
                // hint flags
                public long HintFlags 
                {
                    get => Get<long>("hint_flags");
                    set => Set<long>("hint_flags", value);
                }

                
                // bindings to use when use_binding is the onscreen icon
                public string HintBinding 
                {
                    get => Get<string>("hint_binding");
                    set => Set<string>("hint_binding", value);
                }

                
                // gamepad bindings to use when use_binding is the onscreen icon
                public string HintGamepadBinding 
                {
                    get => Get<string>("hint_gamepad_binding");
                    set => Set<string>("hint_gamepad_binding", value);
                }

                
                // if false, the hint will dissappear if the target entity is invisible
                public bool HintAllowNodrawTarget 
                {
                    get => Get<bool>("hint_allow_nodraw_target");
                    set => Set<bool>("hint_allow_nodraw_target", value);
                }

                
                // if true, the hint will not show when outside the player view
                public bool HintNooffscreen 
                {
                    get => Get<bool>("hint_nooffscreen");
                    set => Set<bool>("hint_nooffscreen", value);
                }

                
                // if true, the hint caption will show even if the hint is occluded
                public bool HintForcecaption 
                {
                    get => Get<bool>("hint_forcecaption");
                    set => Set<bool>("hint_forcecaption", value);
                }

                
                // if true, only the local player will see the hint
                public bool HintLocalPlayerOnly 
                {
                    get => Get<bool>("hint_local_player_only");
                    set => Set<bool>("hint_local_player_only", value);
                }
            }

            [EventName("instructor_server_hint_stop")]
            public class EventInstructorServerHintStop : GameEvent
            {
                public EventInstructorServerHintStop(IntPtr pointer) : base(pointer){}
                public EventInstructorServerHintStop(bool force) : base("instructor_server_hint_stop", force){}

                
                
                // The hint to stop. Will stop ALL hints with this name
                public string HintName 
                {
                    get => Get<string>("hint_name");
                    set => Set<string>("hint_name", value);
                }
            }

            [EventName("instructor_start_lesson")]
            public class EventInstructorStartLesson : GameEvent
            {
                public EventInstructorStartLesson(IntPtr pointer) : base(pointer){}
                public EventInstructorStartLesson(bool force) : base("instructor_start_lesson", force){}

                
                
                // The player who this lesson is intended for
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // Name of the lesson to start.  Must match instructor_lesson.txt
                public string HintName 
                {
                    get => Get<string>("hint_name");
                    set => Set<string>("hint_name", value);
                }

                
                // entity id that the hint should display at. Leave empty if controller target
                public long HintTarget 
                {
                    get => Get<long>("hint_target");
                    set => Set<long>("hint_target", value);
                }

                
                
                public int VrMovementType 
                {
                    get => Get<int>("vr_movement_type");
                    set => Set<int>("vr_movement_type", value);
                }

                
                
                public bool VrSingleController 
                {
                    get => Get<bool>("vr_single_controller");
                    set => Set<bool>("vr_single_controller", value);
                }

                
                
                public int VrControllerType 
                {
                    get => Get<int>("vr_controller_type");
                    set => Set<int>("vr_controller_type", value);
                }
            }

            [EventName("inventory_updated")]
            public class EventInventoryUpdated : GameEvent
            {
                public EventInventoryUpdated(IntPtr pointer) : base(pointer){}
                public EventInventoryUpdated(bool force) : base("inventory_updated", force){}

                
            }

            [EventName("item_equip")]
            public class EventItemEquip : GameEvent
            {
                public EventItemEquip(IntPtr pointer) : base(pointer){}
                public EventItemEquip(bool force) : base("item_equip", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item 
                {
                    get => Get<string>("item");
                    set => Set<string>("item", value);
                }

                
                
                public long Defindex 
                {
                    get => Get<long>("defindex");
                    set => Set<long>("defindex", value);
                }

                
                
                public bool Canzoom 
                {
                    get => Get<bool>("canzoom");
                    set => Set<bool>("canzoom", value);
                }

                
                
                public bool Hassilencer 
                {
                    get => Get<bool>("hassilencer");
                    set => Set<bool>("hassilencer", value);
                }

                
                
                public bool Issilenced 
                {
                    get => Get<bool>("issilenced");
                    set => Set<bool>("issilenced", value);
                }

                
                
                public bool Hastracers 
                {
                    get => Get<bool>("hastracers");
                    set => Set<bool>("hastracers", value);
                }

                
                
                public int Weptype 
                {
                    get => Get<int>("weptype");
                    set => Set<int>("weptype", value);
                }

                
                
                public bool Ispainted 
                {
                    get => Get<bool>("ispainted");
                    set => Set<bool>("ispainted", value);
                }
            }

            [EventName("item_pickup")]
            public class EventItemPickup : GameEvent
            {
                public EventItemPickup(IntPtr pointer) : base(pointer){}
                public EventItemPickup(bool force) : base("item_pickup", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item 
                {
                    get => Get<string>("item");
                    set => Set<string>("item", value);
                }

                
                
                public bool Silent 
                {
                    get => Get<bool>("silent");
                    set => Set<bool>("silent", value);
                }

                
                
                public long Defindex 
                {
                    get => Get<long>("defindex");
                    set => Set<long>("defindex", value);
                }
            }

            [EventName("item_pickup_failed")]
            public class EventItemPickupFailed : GameEvent
            {
                public EventItemPickupFailed(IntPtr pointer) : base(pointer){}
                public EventItemPickupFailed(bool force) : base("item_pickup_failed", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public string Item 
                {
                    get => Get<string>("item");
                    set => Set<string>("item", value);
                }

                
                
                public int Reason 
                {
                    get => Get<int>("reason");
                    set => Set<int>("reason", value);
                }

                
                
                public int Limit 
                {
                    get => Get<int>("limit");
                    set => Set<int>("limit", value);
                }
            }

            [EventName("item_pickup_slerp")]
            public class EventItemPickupSlerp : GameEvent
            {
                public EventItemPickupSlerp(IntPtr pointer) : base(pointer){}
                public EventItemPickupSlerp(bool force) : base("item_pickup_slerp", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Index 
                {
                    get => Get<int>("index");
                    set => Set<int>("index", value);
                }

                
                
                public int Behavior 
                {
                    get => Get<int>("behavior");
                    set => Set<int>("behavior", value);
                }
            }

            [EventName("item_purchase")]
            public class EventItemPurchase : GameEvent
            {
                public EventItemPurchase(IntPtr pointer) : base(pointer){}
                public EventItemPurchase(bool force) : base("item_purchase", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                
                public int Loadout 
                {
                    get => Get<int>("loadout");
                    set => Set<int>("loadout", value);
                }

                
                
                public string Weapon 
                {
                    get => Get<string>("weapon");
                    set => Set<string>("weapon", value);
                }
            }

            [EventName("item_remove")]
            public class EventItemRemove : GameEvent
            {
                public EventItemRemove(IntPtr pointer) : base(pointer){}
                public EventItemRemove(bool force) : base("item_remove", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item 
                {
                    get => Get<string>("item");
                    set => Set<string>("item", value);
                }

                
                
                public long Defindex 
                {
                    get => Get<long>("defindex");
                    set => Set<long>("defindex", value);
                }
            }

            [EventName("item_schema_initialized")]
            public class EventItemSchemaInitialized : GameEvent
            {
                public EventItemSchemaInitialized(IntPtr pointer) : base(pointer){}
                public EventItemSchemaInitialized(bool force) : base("item_schema_initialized", force){}

                
            }

            [EventName("items_gifted")]
            public class EventItemsGifted : GameEvent
            {
                public EventItemsGifted(IntPtr pointer) : base(pointer){}
                public EventItemsGifted(bool force) : base("items_gifted", force){}

                
                
                // entity used by player
                public CCSPlayerController Player 
                {
                    get => Get<CCSPlayerController>("player");
                    set => Set<CCSPlayerController>("player", value);
                }

                
                
                public long Itemdef 
                {
                    get => Get<long>("itemdef");
                    set => Set<long>("itemdef", value);
                }

                
                
                public int Numgifts 
                {
                    get => Get<int>("numgifts");
                    set => Set<int>("numgifts", value);
                }

                
                
                public long Giftidx 
                {
                    get => Get<long>("giftidx");
                    set => Set<long>("giftidx", value);
                }

                
                
                public long Accountid 
                {
                    get => Get<long>("accountid");
                    set => Set<long>("accountid", value);
                }
            }

            [EventName("jointeam_failed")]
            public class EventJointeamFailed : GameEvent
            {
                public EventJointeamFailed(IntPtr pointer) : base(pointer){}
                public EventJointeamFailed(bool force) : base("jointeam_failed", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // 0 = team_full
                public int Reason 
                {
                    get => Get<int>("reason");
                    set => Set<int>("reason", value);
                }
            }

            [EventName("local_player_controller_team")]
            public class EventLocalPlayerControllerTeam : GameEvent
            {
                public EventLocalPlayerControllerTeam(IntPtr pointer) : base(pointer){}
                public EventLocalPlayerControllerTeam(bool force) : base("local_player_controller_team", force){}

                
            }

            [EventName("local_player_pawn_changed")]
            public class EventLocalPlayerPawnChanged : GameEvent
            {
                public EventLocalPlayerPawnChanged(IntPtr pointer) : base(pointer){}
                public EventLocalPlayerPawnChanged(bool force) : base("local_player_pawn_changed", force){}

                
            }

            [EventName("local_player_team")]
            public class EventLocalPlayerTeam : GameEvent
            {
                public EventLocalPlayerTeam(IntPtr pointer) : base(pointer){}
                public EventLocalPlayerTeam(bool force) : base("local_player_team", force){}

                
            }

            [EventName("loot_crate_opened")]
            public class EventLootCrateOpened : GameEvent
            {
                public EventLootCrateOpened(IntPtr pointer) : base(pointer){}
                public EventLootCrateOpened(bool force) : base("loot_crate_opened", force){}

                
                
                // player entindex
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // type of crate (metal, wood, or paradrop)
                public string Type 
                {
                    get => Get<string>("type");
                    set => Set<string>("type", value);
                }
            }

            [EventName("loot_crate_visible")]
            public class EventLootCrateVisible : GameEvent
            {
                public EventLootCrateVisible(IntPtr pointer) : base(pointer){}
                public EventLootCrateVisible(bool force) : base("loot_crate_visible", force){}

                
                
                // player entindex
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // crate entindex
                public int Subject 
                {
                    get => Get<int>("subject");
                    set => Set<int>("subject", value);
                }

                
                // type of crate (metal, wood, or paradrop)
                public string Type 
                {
                    get => Get<string>("type");
                    set => Set<string>("type", value);
                }
            }

            [EventName("map_shutdown")]
            public class EventMapShutdown : GameEvent
            {
                public EventMapShutdown(IntPtr pointer) : base(pointer){}
                public EventMapShutdown(bool force) : base("map_shutdown", force){}

                
            }

            [EventName("map_transition")]
            public class EventMapTransition : GameEvent
            {
                public EventMapTransition(IntPtr pointer) : base(pointer){}
                public EventMapTransition(bool force) : base("map_transition", force){}

                
            }

            [EventName("match_end_conditions")]
            public class EventMatchEndConditions : GameEvent
            {
                public EventMatchEndConditions(IntPtr pointer) : base(pointer){}
                public EventMatchEndConditions(bool force) : base("match_end_conditions", force){}

                
                
                
                public long Frags 
                {
                    get => Get<long>("frags");
                    set => Set<long>("frags", value);
                }

                
                
                public long MaxRounds 
                {
                    get => Get<long>("max_rounds");
                    set => Set<long>("max_rounds", value);
                }

                
                
                public long WinRounds 
                {
                    get => Get<long>("win_rounds");
                    set => Set<long>("win_rounds", value);
                }

                
                
                public long Time 
                {
                    get => Get<long>("time");
                    set => Set<long>("time", value);
                }
            }

            [EventName("material_default_complete")]
            public class EventMaterialDefaultComplete : GameEvent
            {
                public EventMaterialDefaultComplete(IntPtr pointer) : base(pointer){}
                public EventMaterialDefaultComplete(bool force) : base("material_default_complete", force){}

                
            }

            [EventName("mb_input_lock_cancel")]
            public class EventMbInputLockCancel : GameEvent
            {
                public EventMbInputLockCancel(IntPtr pointer) : base(pointer){}
                public EventMbInputLockCancel(bool force) : base("mb_input_lock_cancel", force){}

                
            }

            [EventName("mb_input_lock_success")]
            public class EventMbInputLockSuccess : GameEvent
            {
                public EventMbInputLockSuccess(IntPtr pointer) : base(pointer){}
                public EventMbInputLockSuccess(bool force) : base("mb_input_lock_success", force){}

                
            }

            [EventName("molotov_detonate")]
            public class EventMolotovDetonate : GameEvent
            {
                public EventMolotovDetonate(IntPtr pointer) : base(pointer){}
                public EventMolotovDetonate(bool force) : base("molotov_detonate", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("nav_blocked")]
            public class EventNavBlocked : GameEvent
            {
                public EventNavBlocked(IntPtr pointer) : base(pointer){}
                public EventNavBlocked(bool force) : base("nav_blocked", force){}

                
                
                
                public long Area 
                {
                    get => Get<long>("area");
                    set => Set<long>("area", value);
                }

                
                
                public bool Blocked 
                {
                    get => Get<bool>("blocked");
                    set => Set<bool>("blocked", value);
                }
            }

            [EventName("nav_generate")]
            public class EventNavGenerate : GameEvent
            {
                public EventNavGenerate(IntPtr pointer) : base(pointer){}
                public EventNavGenerate(bool force) : base("nav_generate", force){}

                
            }

            [EventName("nextlevel_changed")]
            public class EventNextlevelChanged : GameEvent
            {
                public EventNextlevelChanged(IntPtr pointer) : base(pointer){}
                public EventNextlevelChanged(bool force) : base("nextlevel_changed", force){}

                
                
                
                public string Nextlevel 
                {
                    get => Get<string>("nextlevel");
                    set => Set<string>("nextlevel", value);
                }

                
                
                public string Mapgroup 
                {
                    get => Get<string>("mapgroup");
                    set => Set<string>("mapgroup", value);
                }

                
                
                public string Skirmishmode 
                {
                    get => Get<string>("skirmishmode");
                    set => Set<string>("skirmishmode", value);
                }
            }

            [EventName("open_crate_instr")]
            public class EventOpenCrateInstr : GameEvent
            {
                public EventOpenCrateInstr(IntPtr pointer) : base(pointer){}
                public EventOpenCrateInstr(bool force) : base("open_crate_instr", force){}

                
                
                // player entindex
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // crate entindex
                public int Subject 
                {
                    get => Get<int>("subject");
                    set => Set<int>("subject", value);
                }

                
                // type of crate (metal, wood, or paradrop)
                public string Type 
                {
                    get => Get<string>("type");
                    set => Set<string>("type", value);
                }
            }

            [EventName("other_death")]
            public class EventOtherDeath : GameEvent
            {
                public EventOtherDeath(IntPtr pointer) : base(pointer){}
                public EventOtherDeath(bool force) : base("other_death", force){}

                
                
                // other entity ID who died
                public int Otherid 
                {
                    get => Get<int>("otherid");
                    set => Set<int>("otherid", value);
                }

                
                // other entity type
                public string Othertype 
                {
                    get => Get<string>("othertype");
                    set => Set<string>("othertype", value);
                }

                
                // user ID who killed
                public int Attacker 
                {
                    get => Get<int>("attacker");
                    set => Set<int>("attacker", value);
                }

                
                // weapon name killer used
                public string Weapon 
                {
                    get => Get<string>("weapon");
                    set => Set<string>("weapon", value);
                }

                
                // inventory item id of weapon killer used
                public string WeaponItemid 
                {
                    get => Get<string>("weapon_itemid");
                    set => Set<string>("weapon_itemid", value);
                }

                
                // faux item id of weapon killer used
                public string WeaponFauxitemid 
                {
                    get => Get<string>("weapon_fauxitemid");
                    set => Set<string>("weapon_fauxitemid", value);
                }

                
                
                public string WeaponOriginalownerXuid 
                {
                    get => Get<string>("weapon_originalowner_xuid");
                    set => Set<string>("weapon_originalowner_xuid", value);
                }

                
                // singals a headshot
                public bool Headshot 
                {
                    get => Get<bool>("headshot");
                    set => Set<bool>("headshot", value);
                }

                
                // number of objects shot penetrated before killing target
                public int Penetrated 
                {
                    get => Get<int>("penetrated");
                    set => Set<int>("penetrated", value);
                }

                
                // kill happened without a scope, used for death notice icon
                public bool Noscope 
                {
                    get => Get<bool>("noscope");
                    set => Set<bool>("noscope", value);
                }

                
                // hitscan weapon went through smoke grenade
                public bool Thrusmoke 
                {
                    get => Get<bool>("thrusmoke");
                    set => Set<bool>("thrusmoke", value);
                }

                
                // attacker was blind from flashbang
                public bool Attackerblind 
                {
                    get => Get<bool>("attackerblind");
                    set => Set<bool>("attackerblind", value);
                }
            }

            [EventName("parachute_deploy")]
            public class EventParachuteDeploy : GameEvent
            {
                public EventParachuteDeploy(IntPtr pointer) : base(pointer){}
                public EventParachuteDeploy(bool force) : base("parachute_deploy", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("parachute_pickup")]
            public class EventParachutePickup : GameEvent
            {
                public EventParachutePickup(IntPtr pointer) : base(pointer){}
                public EventParachutePickup(bool force) : base("parachute_pickup", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("physgun_pickup")]
            public class EventPhysgunPickup : GameEvent
            {
                public EventPhysgunPickup(IntPtr pointer) : base(pointer){}
                public EventPhysgunPickup(bool force) : base("physgun_pickup", force){}

                
                
                // entity picked up
                public IntPtr Target 
                {
                    get => Get<IntPtr>("target");
                    set => Set<IntPtr>("target", value);
                }
            }

            [EventName("player_activate")]
            public class EventPlayerActivate : GameEvent
            {
                public EventPlayerActivate(IntPtr pointer) : base(pointer){}
                public EventPlayerActivate(bool force) : base("player_activate", force){}

                
                
                // user ID on server
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("player_avenged_teammate")]
            public class EventPlayerAvengedTeammate : GameEvent
            {
                public EventPlayerAvengedTeammate(IntPtr pointer) : base(pointer){}
                public EventPlayerAvengedTeammate(bool force) : base("player_avenged_teammate", force){}

                
                
                
                public CCSPlayerController AvengerId 
                {
                    get => Get<CCSPlayerController>("avenger_id");
                    set => Set<CCSPlayerController>("avenger_id", value);
                }

                
                
                public CCSPlayerController AvengedPlayerId 
                {
                    get => Get<CCSPlayerController>("avenged_player_id");
                    set => Set<CCSPlayerController>("avenged_player_id", value);
                }
            }

            [EventName("player_blind")]
            public class EventPlayerBlind : GameEvent
            {
                public EventPlayerBlind(IntPtr pointer) : base(pointer){}
                public EventPlayerBlind(bool force) : base("player_blind", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // user ID who threw the flash
                public CCSPlayerController Attacker 
                {
                    get => Get<CCSPlayerController>("attacker");
                    set => Set<CCSPlayerController>("attacker", value);
                }

                
                // the flashbang going off
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float BlindDuration 
                {
                    get => Get<float>("blind_duration");
                    set => Set<float>("blind_duration", value);
                }
            }

            [EventName("player_changename")]
            public class EventPlayerChangename : GameEvent
            {
                public EventPlayerChangename(IntPtr pointer) : base(pointer){}
                public EventPlayerChangename(bool force) : base("player_changename", force){}

                
                
                // user ID on server
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // players old (current) name
                public string Oldname 
                {
                    get => Get<string>("oldname");
                    set => Set<string>("oldname", value);
                }

                
                // players new name
                public string Newname 
                {
                    get => Get<string>("newname");
                    set => Set<string>("newname", value);
                }
            }

            [EventName("player_chat")]
            public class EventPlayerChat : GameEvent
            {
                public EventPlayerChat(IntPtr pointer) : base(pointer){}
                public EventPlayerChat(bool force) : base("player_chat", force){}

                
                
                // true if team only chat
                public bool Teamonly 
                {
                    get => Get<bool>("teamonly");
                    set => Set<bool>("teamonly", value);
                }

                
                // chatting player
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }

                
                // chat text
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }
            }

            [EventName("player_connect")]
            public class EventPlayerConnect : GameEvent
            {
                public EventPlayerConnect(IntPtr pointer) : base(pointer){}
                public EventPlayerConnect(bool force) : base("player_connect", force){}

                
                
                // player name
                public string Name 
                {
                    get => Get<string>("name");
                    set => Set<string>("name", value);
                }

                
                // user ID on server (unique on server)
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // player network (i.e steam) id
                public string Networkid 
                {
                    get => Get<string>("networkid");
                    set => Set<string>("networkid", value);
                }

                
                // steam id
                public ulong Xuid 
                {
                    get => Get<ulong>("xuid");
                    set => Set<ulong>("xuid", value);
                }

                
                // ip:port
                public string Address 
                {
                    get => Get<string>("address");
                    set => Set<string>("address", value);
                }

                
                
                public bool Bot 
                {
                    get => Get<bool>("bot");
                    set => Set<bool>("bot", value);
                }
            }

            [EventName("player_connect_full")]
            public class EventPlayerConnectFull : GameEvent
            {
                public EventPlayerConnectFull(IntPtr pointer) : base(pointer){}
                public EventPlayerConnectFull(bool force) : base("player_connect_full", force){}

                
                
                // user ID on server (unique on server)
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("player_death")]
            public class EventPlayerDeath : GameEvent
            {
                public EventPlayerDeath(IntPtr pointer) : base(pointer){}
                public EventPlayerDeath(bool force) : base("player_death", force){}

                
                
                // user who died
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // player who killed
                public CCSPlayerController Attacker 
                {
                    get => Get<CCSPlayerController>("attacker");
                    set => Set<CCSPlayerController>("attacker", value);
                }

                
                // player who assisted in the kill
                public CCSPlayerController Assister 
                {
                    get => Get<CCSPlayerController>("assister");
                    set => Set<CCSPlayerController>("assister", value);
                }

                
                // assister helped with a flash
                public bool Assistedflash 
                {
                    get => Get<bool>("assistedflash");
                    set => Set<bool>("assistedflash", value);
                }

                
                // weapon name killer used
                public string Weapon 
                {
                    get => Get<string>("weapon");
                    set => Set<string>("weapon", value);
                }

                
                // inventory item id of weapon killer used
                public string WeaponItemid 
                {
                    get => Get<string>("weapon_itemid");
                    set => Set<string>("weapon_itemid", value);
                }

                
                // faux item id of weapon killer used
                public string WeaponFauxitemid 
                {
                    get => Get<string>("weapon_fauxitemid");
                    set => Set<string>("weapon_fauxitemid", value);
                }

                
                
                public string WeaponOriginalownerXuid 
                {
                    get => Get<string>("weapon_originalowner_xuid");
                    set => Set<string>("weapon_originalowner_xuid", value);
                }

                
                // singals a headshot
                public bool Headshot 
                {
                    get => Get<bool>("headshot");
                    set => Set<bool>("headshot", value);
                }

                
                // did killer dominate victim with this kill
                public int Dominated 
                {
                    get => Get<int>("dominated");
                    set => Set<int>("dominated", value);
                }

                
                // did killer get revenge on victim with this kill
                public int Revenge 
                {
                    get => Get<int>("revenge");
                    set => Set<int>("revenge", value);
                }

                
                // is the kill resulting in squad wipe
                public int Wipe 
                {
                    get => Get<int>("wipe");
                    set => Set<int>("wipe", value);
                }

                
                // number of objects shot penetrated before killing target
                public int Penetrated 
                {
                    get => Get<int>("penetrated");
                    set => Set<int>("penetrated", value);
                }

                
                // if replay data is unavailable, this will be present and set to false
                public bool Noreplay 
                {
                    get => Get<bool>("noreplay");
                    set => Set<bool>("noreplay", value);
                }

                
                // kill happened without a scope, used for death notice icon
                public bool Noscope 
                {
                    get => Get<bool>("noscope");
                    set => Set<bool>("noscope", value);
                }

                
                // hitscan weapon went through smoke grenade
                public bool Thrusmoke 
                {
                    get => Get<bool>("thrusmoke");
                    set => Set<bool>("thrusmoke", value);
                }

                
                // attacker was blind from flashbang
                public bool Attackerblind 
                {
                    get => Get<bool>("attackerblind");
                    set => Set<bool>("attackerblind", value);
                }

                
                // distance to victim in meters
                public float Distance 
                {
                    get => Get<float>("distance");
                    set => Set<float>("distance", value);
                }

                
                // damage done to health
                public int DmgHealth 
                {
                    get => Get<int>("dmg_health");
                    set => Set<int>("dmg_health", value);
                }

                
                // damage done to armor
                public int DmgArmor 
                {
                    get => Get<int>("dmg_armor");
                    set => Set<int>("dmg_armor", value);
                }

                
                // hitgroup that was damaged
                public int Hitgroup 
                {
                    get => Get<int>("hitgroup");
                    set => Set<int>("hitgroup", value);
                }
            }

            [EventName("player_decal")]
            public class EventPlayerDecal : GameEvent
            {
                public EventPlayerDecal(IntPtr pointer) : base(pointer){}
                public EventPlayerDecal(bool force) : base("player_decal", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("player_disconnect")]
            public class EventPlayerDisconnect : GameEvent
            {
                public EventPlayerDisconnect(IntPtr pointer) : base(pointer){}
                public EventPlayerDisconnect(bool force) : base("player_disconnect", force){}

                
                
                // user ID on server
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // see networkdisconnect enum protobuf
                public int Reason 
                {
                    get => Get<int>("reason");
                    set => Set<int>("reason", value);
                }

                
                // player name
                public string Name 
                {
                    get => Get<string>("name");
                    set => Set<string>("name", value);
                }

                
                // player network (i.e steam) id
                public string Networkid 
                {
                    get => Get<string>("networkid");
                    set => Set<string>("networkid", value);
                }

                
                // steam id
                public ulong Xuid 
                {
                    get => Get<ulong>("xuid");
                    set => Set<ulong>("xuid", value);
                }

                
                
                public int Playerid 
                {
                    get => Get<int>("PlayerID");
                    set => Set<int>("PlayerID", value);
                }
            }

            [EventName("player_falldamage")]
            public class EventPlayerFalldamage : GameEvent
            {
                public EventPlayerFalldamage(IntPtr pointer) : base(pointer){}
                public EventPlayerFalldamage(bool force) : base("player_falldamage", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public float Damage 
                {
                    get => Get<float>("damage");
                    set => Set<float>("damage", value);
                }
            }

            [EventName("player_footstep")]
            public class EventPlayerFootstep : GameEvent
            {
                public EventPlayerFootstep(IntPtr pointer) : base(pointer){}
                public EventPlayerFootstep(bool force) : base("player_footstep", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("player_full_update")]
            public class EventPlayerFullUpdate : GameEvent
            {
                public EventPlayerFullUpdate(IntPtr pointer) : base(pointer){}
                public EventPlayerFullUpdate(bool force) : base("player_full_update", force){}

                
                
                // user ID on server
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // Number of this full update
                public int Count 
                {
                    get => Get<int>("count");
                    set => Set<int>("count", value);
                }
            }

            [EventName("player_given_c4")]
            public class EventPlayerGivenC4 : GameEvent
            {
                public EventPlayerGivenC4(IntPtr pointer) : base(pointer){}
                public EventPlayerGivenC4(bool force) : base("player_given_c4", force){}

                
                
                // user ID who received the c4
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("player_hintmessage")]
            public class EventPlayerHintmessage : GameEvent
            {
                public EventPlayerHintmessage(IntPtr pointer) : base(pointer){}
                public EventPlayerHintmessage(bool force) : base("player_hintmessage", force){}

                
                
                // localizable string of a hint
                public string Hintmessage 
                {
                    get => Get<string>("hintmessage");
                    set => Set<string>("hintmessage", value);
                }
            }

            [EventName("player_hurt")]
            public class EventPlayerHurt : GameEvent
            {
                public EventPlayerHurt(IntPtr pointer) : base(pointer){}
                public EventPlayerHurt(bool force) : base("player_hurt", force){}

                
                
                // player index who was hurt
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // player index who attacked
                public CCSPlayerController Attacker 
                {
                    get => Get<CCSPlayerController>("attacker");
                    set => Set<CCSPlayerController>("attacker", value);
                }

                
                // remaining health points
                public int Health 
                {
                    get => Get<int>("health");
                    set => Set<int>("health", value);
                }

                
                // remaining armor points
                public int Armor 
                {
                    get => Get<int>("armor");
                    set => Set<int>("armor", value);
                }

                
                // weapon name attacker used, if not the world
                public string Weapon 
                {
                    get => Get<string>("weapon");
                    set => Set<string>("weapon", value);
                }

                
                // damage done to health
                public int DmgHealth 
                {
                    get => Get<int>("dmg_health");
                    set => Set<int>("dmg_health", value);
                }

                
                // damage done to armor
                public int DmgArmor 
                {
                    get => Get<int>("dmg_armor");
                    set => Set<int>("dmg_armor", value);
                }

                
                // hitgroup that was damaged
                public int Hitgroup 
                {
                    get => Get<int>("hitgroup");
                    set => Set<int>("hitgroup", value);
                }
            }

            [EventName("player_info")]
            public class EventPlayerInfo : GameEvent
            {
                public EventPlayerInfo(IntPtr pointer) : base(pointer){}
                public EventPlayerInfo(bool force) : base("player_info", force){}

                
                
                // player name
                public string Name 
                {
                    get => Get<string>("name");
                    set => Set<string>("name", value);
                }

                
                // user ID on server (unique on server)
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // player network (i.e steam) id
                public ulong Steamid 
                {
                    get => Get<ulong>("steamid");
                    set => Set<ulong>("steamid", value);
                }

                
                // true if player is a AI bot
                public bool Bot 
                {
                    get => Get<bool>("bot");
                    set => Set<bool>("bot", value);
                }
            }

            [EventName("player_jump")]
            public class EventPlayerJump : GameEvent
            {
                public EventPlayerJump(IntPtr pointer) : base(pointer){}
                public EventPlayerJump(bool force) : base("player_jump", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("player_ping")]
            public class EventPlayerPing : GameEvent
            {
                public EventPlayerPing(IntPtr pointer) : base(pointer){}
                public EventPlayerPing(bool force) : base("player_ping", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }

                
                
                public bool Urgent 
                {
                    get => Get<bool>("urgent");
                    set => Set<bool>("urgent", value);
                }
            }

            [EventName("player_ping_stop")]
            public class EventPlayerPingStop : GameEvent
            {
                public EventPlayerPingStop(IntPtr pointer) : base(pointer){}
                public EventPlayerPingStop(bool force) : base("player_ping_stop", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }
            }

            [EventName("player_radio")]
            public class EventPlayerRadio : GameEvent
            {
                public EventPlayerRadio(IntPtr pointer) : base(pointer){}
                public EventPlayerRadio(bool force) : base("player_radio", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Slot 
                {
                    get => Get<int>("slot");
                    set => Set<int>("slot", value);
                }
            }

            [EventName("player_reset_vote")]
            public class EventPlayerResetVote : GameEvent
            {
                public EventPlayerResetVote(IntPtr pointer) : base(pointer){}
                public EventPlayerResetVote(bool force) : base("player_reset_vote", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public bool Vote 
                {
                    get => Get<bool>("vote");
                    set => Set<bool>("vote", value);
                }
            }

            [EventName("player_score")]
            public class EventPlayerScore : GameEvent
            {
                public EventPlayerScore(IntPtr pointer) : base(pointer){}
                public EventPlayerScore(bool force) : base("player_score", force){}

                
                
                // user ID on server
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // # of kills
                public int Kills 
                {
                    get => Get<int>("kills");
                    set => Set<int>("kills", value);
                }

                
                // # of deaths
                public int Deaths 
                {
                    get => Get<int>("deaths");
                    set => Set<int>("deaths", value);
                }

                
                // total game score
                public int Score 
                {
                    get => Get<int>("score");
                    set => Set<int>("score", value);
                }
            }

            [EventName("player_shoot")]
            public class EventPlayerShoot : GameEvent
            {
                public EventPlayerShoot(IntPtr pointer) : base(pointer){}
                public EventPlayerShoot(bool force) : base("player_shoot", force){}

                
                
                // user ID on server
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // weapon ID
                public int Weapon 
                {
                    get => Get<int>("weapon");
                    set => Set<int>("weapon", value);
                }

                
                // weapon mode
                public int Mode 
                {
                    get => Get<int>("mode");
                    set => Set<int>("mode", value);
                }
            }

            [EventName("player_sound")]
            public class EventPlayerSound : GameEvent
            {
                public EventPlayerSound(IntPtr pointer) : base(pointer){}
                public EventPlayerSound(bool force) : base("player_sound", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Radius 
                {
                    get => Get<int>("radius");
                    set => Set<int>("radius", value);
                }

                
                
                public float Duration 
                {
                    get => Get<float>("duration");
                    set => Set<float>("duration", value);
                }

                
                
                public bool Step 
                {
                    get => Get<bool>("step");
                    set => Set<bool>("step", value);
                }
            }

            [EventName("player_spawn")]
            public class EventPlayerSpawn : GameEvent
            {
                public EventPlayerSpawn(IntPtr pointer) : base(pointer){}
                public EventPlayerSpawn(bool force) : base("player_spawn", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("player_spawned")]
            public class EventPlayerSpawned : GameEvent
            {
                public EventPlayerSpawned(IntPtr pointer) : base(pointer){}
                public EventPlayerSpawned(bool force) : base("player_spawned", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // true if restart is pending
                public bool Inrestart 
                {
                    get => Get<bool>("inrestart");
                    set => Set<bool>("inrestart", value);
                }
            }

            [EventName("player_stats_updated")]
            public class EventPlayerStatsUpdated : GameEvent
            {
                public EventPlayerStatsUpdated(IntPtr pointer) : base(pointer){}
                public EventPlayerStatsUpdated(bool force) : base("player_stats_updated", force){}

                
                
                
                public bool Forceupload 
                {
                    get => Get<bool>("forceupload");
                    set => Set<bool>("forceupload", value);
                }
            }

            [EventName("player_team")]
            public class EventPlayerTeam : GameEvent
            {
                public EventPlayerTeam(IntPtr pointer) : base(pointer){}
                public EventPlayerTeam(bool force) : base("player_team", force){}

                
                
                // player
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // team id
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                // old team id
                public int Oldteam 
                {
                    get => Get<int>("oldteam");
                    set => Set<int>("oldteam", value);
                }

                
                // team change because player disconnects
                public bool Disconnect 
                {
                    get => Get<bool>("disconnect");
                    set => Set<bool>("disconnect", value);
                }

                
                
                public bool Silent 
                {
                    get => Get<bool>("silent");
                    set => Set<bool>("silent", value);
                }

                
                // true if player is a bot
                public bool Isbot 
                {
                    get => Get<bool>("isbot");
                    set => Set<bool>("isbot", value);
                }
            }

            [EventName("ragdoll_dissolved")]
            public class EventRagdollDissolved : GameEvent
            {
                public EventRagdollDissolved(IntPtr pointer) : base(pointer){}
                public EventRagdollDissolved(bool force) : base("ragdoll_dissolved", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("read_game_titledata")]
            public class EventReadGameTitledata : GameEvent
            {
                public EventReadGameTitledata(IntPtr pointer) : base(pointer){}
                public EventReadGameTitledata(bool force) : base("read_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => Get<int>("controllerId");
                    set => Set<int>("controllerId", value);
                }
            }

            [EventName("repost_xbox_achievements")]
            public class EventRepostXboxAchievements : GameEvent
            {
                public EventRepostXboxAchievements(IntPtr pointer) : base(pointer){}
                public EventRepostXboxAchievements(bool force) : base("repost_xbox_achievements", force){}

                
                
                // splitscreen ID
                public int Splitscreenplayer 
                {
                    get => Get<int>("splitscreenplayer");
                    set => Set<int>("splitscreenplayer", value);
                }
            }

            [EventName("reset_game_titledata")]
            public class EventResetGameTitledata : GameEvent
            {
                public EventResetGameTitledata(IntPtr pointer) : base(pointer){}
                public EventResetGameTitledata(bool force) : base("reset_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => Get<int>("controllerId");
                    set => Set<int>("controllerId", value);
                }
            }

            [EventName("round_announce_final")]
            public class EventRoundAnnounceFinal : GameEvent
            {
                public EventRoundAnnounceFinal(IntPtr pointer) : base(pointer){}
                public EventRoundAnnounceFinal(bool force) : base("round_announce_final", force){}

                
            }

            [EventName("round_announce_last_round_half")]
            public class EventRoundAnnounceLastRoundHalf : GameEvent
            {
                public EventRoundAnnounceLastRoundHalf(IntPtr pointer) : base(pointer){}
                public EventRoundAnnounceLastRoundHalf(bool force) : base("round_announce_last_round_half", force){}

                
            }

            [EventName("round_announce_match_point")]
            public class EventRoundAnnounceMatchPoint : GameEvent
            {
                public EventRoundAnnounceMatchPoint(IntPtr pointer) : base(pointer){}
                public EventRoundAnnounceMatchPoint(bool force) : base("round_announce_match_point", force){}

                
            }

            [EventName("round_announce_match_start")]
            public class EventRoundAnnounceMatchStart : GameEvent
            {
                public EventRoundAnnounceMatchStart(IntPtr pointer) : base(pointer){}
                public EventRoundAnnounceMatchStart(bool force) : base("round_announce_match_start", force){}

                
            }

            [EventName("round_announce_warmup")]
            public class EventRoundAnnounceWarmup : GameEvent
            {
                public EventRoundAnnounceWarmup(IntPtr pointer) : base(pointer){}
                public EventRoundAnnounceWarmup(bool force) : base("round_announce_warmup", force){}

                
            }

            [EventName("round_end")]
            public class EventRoundEnd : GameEvent
            {
                public EventRoundEnd(IntPtr pointer) : base(pointer){}
                public EventRoundEnd(bool force) : base("round_end", force){}

                
                
                // winner team/user i
                public int Winner 
                {
                    get => Get<int>("winner");
                    set => Set<int>("winner", value);
                }

                
                // reson why team won
                public int Reason 
                {
                    get => Get<int>("reason");
                    set => Set<int>("reason", value);
                }

                
                // end round message
                public string Message 
                {
                    get => Get<string>("message");
                    set => Set<string>("message", value);
                }

                
                // server-generated legacy value
                public int Legacy 
                {
                    get => Get<int>("legacy");
                    set => Set<int>("legacy", value);
                }

                
                // total number of players alive at the end of round, used for statistics gathering, computed on the server in the event client is in replay when receiving this message
                public int PlayerCount 
                {
                    get => Get<int>("player_count");
                    set => Set<int>("player_count", value);
                }

                
                // if set, don't play round end music, because action is still on-going
                public int Nomusic 
                {
                    get => Get<int>("nomusic");
                    set => Set<int>("nomusic", value);
                }
            }

            [EventName("round_end_upload_stats")]
            public class EventRoundEndUploadStats : GameEvent
            {
                public EventRoundEndUploadStats(IntPtr pointer) : base(pointer){}
                public EventRoundEndUploadStats(bool force) : base("round_end_upload_stats", force){}

                
            }

            [EventName("round_freeze_end")]
            public class EventRoundFreezeEnd : GameEvent
            {
                public EventRoundFreezeEnd(IntPtr pointer) : base(pointer){}
                public EventRoundFreezeEnd(bool force) : base("round_freeze_end", force){}

                
            }

            [EventName("round_mvp")]
            public class EventRoundMvp : GameEvent
            {
                public EventRoundMvp(IntPtr pointer) : base(pointer){}
                public EventRoundMvp(bool force) : base("round_mvp", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Reason 
                {
                    get => Get<int>("reason");
                    set => Set<int>("reason", value);
                }

                
                
                public long Value 
                {
                    get => Get<long>("value");
                    set => Set<long>("value", value);
                }

                
                
                public long Musickitmvps 
                {
                    get => Get<long>("musickitmvps");
                    set => Set<long>("musickitmvps", value);
                }

                
                
                public int Nomusic 
                {
                    get => Get<int>("nomusic");
                    set => Set<int>("nomusic", value);
                }

                
                
                public long Musickitid 
                {
                    get => Get<long>("musickitid");
                    set => Set<long>("musickitid", value);
                }
            }

            [EventName("round_officially_ended")]
            public class EventRoundOfficiallyEnded : GameEvent
            {
                public EventRoundOfficiallyEnded(IntPtr pointer) : base(pointer){}
                public EventRoundOfficiallyEnded(bool force) : base("round_officially_ended", force){}

                
            }

            [EventName("round_poststart")]
            public class EventRoundPoststart : GameEvent
            {
                public EventRoundPoststart(IntPtr pointer) : base(pointer){}
                public EventRoundPoststart(bool force) : base("round_poststart", force){}

                
            }

            [EventName("round_prestart")]
            public class EventRoundPrestart : GameEvent
            {
                public EventRoundPrestart(IntPtr pointer) : base(pointer){}
                public EventRoundPrestart(bool force) : base("round_prestart", force){}

                
            }

            [EventName("round_start")]
            public class EventRoundStart : GameEvent
            {
                public EventRoundStart(IntPtr pointer) : base(pointer){}
                public EventRoundStart(bool force) : base("round_start", force){}

                
                
                // round time limit in seconds
                public long Timelimit 
                {
                    get => Get<long>("timelimit");
                    set => Set<long>("timelimit", value);
                }

                
                // frag limit in seconds
                public long Fraglimit 
                {
                    get => Get<long>("fraglimit");
                    set => Set<long>("fraglimit", value);
                }

                
                // round objective
                public string Objective 
                {
                    get => Get<string>("objective");
                    set => Set<string>("objective", value);
                }
            }

            [EventName("round_start_post_nav")]
            public class EventRoundStartPostNav : GameEvent
            {
                public EventRoundStartPostNav(IntPtr pointer) : base(pointer){}
                public EventRoundStartPostNav(bool force) : base("round_start_post_nav", force){}

                
            }

            [EventName("round_start_pre_entity")]
            public class EventRoundStartPreEntity : GameEvent
            {
                public EventRoundStartPreEntity(IntPtr pointer) : base(pointer){}
                public EventRoundStartPreEntity(bool force) : base("round_start_pre_entity", force){}

                
            }

            [EventName("round_time_warning")]
            public class EventRoundTimeWarning : GameEvent
            {
                public EventRoundTimeWarning(IntPtr pointer) : base(pointer){}
                public EventRoundTimeWarning(bool force) : base("round_time_warning", force){}

                
            }

            [EventName("seasoncoin_levelup")]
            public class EventSeasoncoinLevelup : GameEvent
            {
                public EventSeasoncoinLevelup(IntPtr pointer) : base(pointer){}
                public EventSeasoncoinLevelup(bool force) : base("seasoncoin_levelup", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Category 
                {
                    get => Get<int>("category");
                    set => Set<int>("category", value);
                }

                
                
                public int Rank 
                {
                    get => Get<int>("rank");
                    set => Set<int>("rank", value);
                }
            }

            [EventName("server_cvar")]
            public class EventServerCvar : GameEvent
            {
                public EventServerCvar(IntPtr pointer) : base(pointer){}
                public EventServerCvar(bool force) : base("server_cvar", force){}

                
                
                // cvar name, eg "mp_roundtime"
                public string Cvarname 
                {
                    get => Get<string>("cvarname");
                    set => Set<string>("cvarname", value);
                }

                
                // new cvar value
                public string Cvarvalue 
                {
                    get => Get<string>("cvarvalue");
                    set => Set<string>("cvarvalue", value);
                }
            }

            [EventName("server_message")]
            public class EventServerMessage : GameEvent
            {
                public EventServerMessage(IntPtr pointer) : base(pointer){}
                public EventServerMessage(bool force) : base("server_message", force){}

                
                
                // the message text
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }
            }

            [EventName("server_pre_shutdown")]
            public class EventServerPreShutdown : GameEvent
            {
                public EventServerPreShutdown(IntPtr pointer) : base(pointer){}
                public EventServerPreShutdown(bool force) : base("server_pre_shutdown", force){}

                
                
                // reason why server is about to be shut down
                public string Reason 
                {
                    get => Get<string>("reason");
                    set => Set<string>("reason", value);
                }
            }

            [EventName("server_shutdown")]
            public class EventServerShutdown : GameEvent
            {
                public EventServerShutdown(IntPtr pointer) : base(pointer){}
                public EventServerShutdown(bool force) : base("server_shutdown", force){}

                
                
                // reason why server was shut down
                public string Reason 
                {
                    get => Get<string>("reason");
                    set => Set<string>("reason", value);
                }
            }

            [EventName("server_spawn")]
            public class EventServerSpawn : GameEvent
            {
                public EventServerSpawn(IntPtr pointer) : base(pointer){}
                public EventServerSpawn(bool force) : base("server_spawn", force){}

                
                
                // public host name
                public string Hostname 
                {
                    get => Get<string>("hostname");
                    set => Set<string>("hostname", value);
                }

                
                // hostame, IP or DNS name
                public string Address 
                {
                    get => Get<string>("address");
                    set => Set<string>("address", value);
                }

                
                // server port
                public int Port 
                {
                    get => Get<int>("port");
                    set => Set<int>("port", value);
                }

                
                // game dir
                public string Game 
                {
                    get => Get<string>("game");
                    set => Set<string>("game", value);
                }

                
                // map name
                public string Mapname 
                {
                    get => Get<string>("mapname");
                    set => Set<string>("mapname", value);
                }

                
                // addon name
                public string Addonname 
                {
                    get => Get<string>("addonname");
                    set => Set<string>("addonname", value);
                }

                
                // max players
                public long Maxplayers 
                {
                    get => Get<long>("maxplayers");
                    set => Set<long>("maxplayers", value);
                }

                
                // WIN32, LINUX
                public string Os 
                {
                    get => Get<string>("os");
                    set => Set<string>("os", value);
                }

                
                // true if dedicated server
                public bool Dedicated 
                {
                    get => Get<bool>("dedicated");
                    set => Set<bool>("dedicated", value);
                }

                
                // true if password protected
                public bool Password 
                {
                    get => Get<bool>("password");
                    set => Set<bool>("password", value);
                }
            }

            [EventName("set_instructor_group_enabled")]
            public class EventSetInstructorGroupEnabled : GameEvent
            {
                public EventSetInstructorGroupEnabled(IntPtr pointer) : base(pointer){}
                public EventSetInstructorGroupEnabled(bool force) : base("set_instructor_group_enabled", force){}

                
                
                
                public string Group 
                {
                    get => Get<string>("group");
                    set => Set<string>("group", value);
                }

                
                
                public int Enabled 
                {
                    get => Get<int>("enabled");
                    set => Set<int>("enabled", value);
                }
            }

            [EventName("sfuievent")]
            public class EventSfuievent : GameEvent
            {
                public EventSfuievent(IntPtr pointer) : base(pointer){}
                public EventSfuievent(bool force) : base("sfuievent", force){}

                
                
                
                public string Action 
                {
                    get => Get<string>("action");
                    set => Set<string>("action", value);
                }

                
                
                public string Data 
                {
                    get => Get<string>("data");
                    set => Set<string>("data", value);
                }

                
                
                public int Slot 
                {
                    get => Get<int>("slot");
                    set => Set<int>("slot", value);
                }
            }

            [EventName("show_deathpanel")]
            public class EventShowDeathpanel : GameEvent
            {
                public EventShowDeathpanel(IntPtr pointer) : base(pointer){}
                public EventShowDeathpanel(bool force) : base("show_deathpanel", force){}

                
                
                // endindex of the one who was killed
                public CCSPlayerController Victim 
                {
                    get => Get<CCSPlayerController>("victim");
                    set => Set<CCSPlayerController>("victim", value);
                }

                
                // entindex of the killer entity
                public IntPtr Killer 
                {
                    get => Get<IntPtr>("killer");
                    set => Set<IntPtr>("killer", value);
                }

                
                
                public CCSPlayerController KillerController 
                {
                    get => Get<CCSPlayerController>("killer_controller");
                    set => Set<CCSPlayerController>("killer_controller", value);
                }

                
                
                public int HitsTaken 
                {
                    get => Get<int>("hits_taken");
                    set => Set<int>("hits_taken", value);
                }

                
                
                public int DamageTaken 
                {
                    get => Get<int>("damage_taken");
                    set => Set<int>("damage_taken", value);
                }

                
                
                public int HitsGiven 
                {
                    get => Get<int>("hits_given");
                    set => Set<int>("hits_given", value);
                }

                
                
                public int DamageGiven 
                {
                    get => Get<int>("damage_given");
                    set => Set<int>("damage_given", value);
                }
            }

            [EventName("show_survival_respawn_status")]
            public class EventShowSurvivalRespawnStatus : GameEvent
            {
                public EventShowSurvivalRespawnStatus(IntPtr pointer) : base(pointer){}
                public EventShowSurvivalRespawnStatus(bool force) : base("show_survival_respawn_status", force){}

                
                
                
                public string LocToken 
                {
                    get => Get<string>("loc_token");
                    set => Set<string>("loc_token", value);
                }

                
                
                public long Duration 
                {
                    get => Get<long>("duration");
                    set => Set<long>("duration", value);
                }

                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("silencer_detach")]
            public class EventSilencerDetach : GameEvent
            {
                public EventSilencerDetach(IntPtr pointer) : base(pointer){}
                public EventSilencerDetach(bool force) : base("silencer_detach", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("silencer_off")]
            public class EventSilencerOff : GameEvent
            {
                public EventSilencerOff(IntPtr pointer) : base(pointer){}
                public EventSilencerOff(bool force) : base("silencer_off", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("silencer_on")]
            public class EventSilencerOn : GameEvent
            {
                public EventSilencerOn(IntPtr pointer) : base(pointer){}
                public EventSilencerOn(bool force) : base("silencer_on", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("smoke_beacon_paradrop")]
            public class EventSmokeBeaconParadrop : GameEvent
            {
                public EventSmokeBeaconParadrop(IntPtr pointer) : base(pointer){}
                public EventSmokeBeaconParadrop(bool force) : base("smoke_beacon_paradrop", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Paradrop 
                {
                    get => Get<int>("paradrop");
                    set => Set<int>("paradrop", value);
                }
            }

            [EventName("smokegrenade_detonate")]
            public class EventSmokegrenadeDetonate : GameEvent
            {
                public EventSmokegrenadeDetonate(IntPtr pointer) : base(pointer){}
                public EventSmokegrenadeDetonate(bool force) : base("smokegrenade_detonate", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("smokegrenade_expired")]
            public class EventSmokegrenadeExpired : GameEvent
            {
                public EventSmokegrenadeExpired(IntPtr pointer) : base(pointer){}
                public EventSmokegrenadeExpired(bool force) : base("smokegrenade_expired", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("spec_mode_updated")]
            public class EventSpecModeUpdated : GameEvent
            {
                public EventSpecModeUpdated(IntPtr pointer) : base(pointer){}
                public EventSpecModeUpdated(bool force) : base("spec_mode_updated", force){}

                
                
                // entindex of the player
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("spec_target_updated")]
            public class EventSpecTargetUpdated : GameEvent
            {
                public EventSpecTargetUpdated(IntPtr pointer) : base(pointer){}
                public EventSpecTargetUpdated(bool force) : base("spec_target_updated", force){}

                
                
                // spectating player
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // ehandle of the target
                public IntPtr Target 
                {
                    get => Get<IntPtr>("target");
                    set => Set<IntPtr>("target", value);
                }
            }

            [EventName("start_halftime")]
            public class EventStartHalftime : GameEvent
            {
                public EventStartHalftime(IntPtr pointer) : base(pointer){}
                public EventStartHalftime(bool force) : base("start_halftime", force){}

                
            }

            [EventName("start_vote")]
            public class EventStartVote : GameEvent
            {
                public EventStartVote(IntPtr pointer) : base(pointer){}
                public EventStartVote(bool force) : base("start_vote", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Type 
                {
                    get => Get<int>("type");
                    set => Set<int>("type", value);
                }

                
                
                public int VoteParameter 
                {
                    get => Get<int>("vote_parameter");
                    set => Set<int>("vote_parameter", value);
                }
            }

            [EventName("store_pricesheet_updated")]
            public class EventStorePricesheetUpdated : GameEvent
            {
                public EventStorePricesheetUpdated(IntPtr pointer) : base(pointer){}
                public EventStorePricesheetUpdated(bool force) : base("store_pricesheet_updated", force){}

                
            }

            [EventName("survival_announce_phase")]
            public class EventSurvivalAnnouncePhase : GameEvent
            {
                public EventSurvivalAnnouncePhase(IntPtr pointer) : base(pointer){}
                public EventSurvivalAnnouncePhase(bool force) : base("survival_announce_phase", force){}

                
                
                // The phase #
                public int Phase 
                {
                    get => Get<int>("phase");
                    set => Set<int>("phase", value);
                }
            }

            [EventName("survival_no_respawns_final")]
            public class EventSurvivalNoRespawnsFinal : GameEvent
            {
                public EventSurvivalNoRespawnsFinal(IntPtr pointer) : base(pointer){}
                public EventSurvivalNoRespawnsFinal(bool force) : base("survival_no_respawns_final", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("survival_no_respawns_warning")]
            public class EventSurvivalNoRespawnsWarning : GameEvent
            {
                public EventSurvivalNoRespawnsWarning(IntPtr pointer) : base(pointer){}
                public EventSurvivalNoRespawnsWarning(bool force) : base("survival_no_respawns_warning", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("survival_paradrop_break")]
            public class EventSurvivalParadropBreak : GameEvent
            {
                public EventSurvivalParadropBreak(IntPtr pointer) : base(pointer){}
                public EventSurvivalParadropBreak(bool force) : base("survival_paradrop_break", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }
            }

            [EventName("survival_paradrop_spawn")]
            public class EventSurvivalParadropSpawn : GameEvent
            {
                public EventSurvivalParadropSpawn(IntPtr pointer) : base(pointer){}
                public EventSurvivalParadropSpawn(bool force) : base("survival_paradrop_spawn", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }
            }

            [EventName("survival_teammate_respawn")]
            public class EventSurvivalTeammateRespawn : GameEvent
            {
                public EventSurvivalTeammateRespawn(IntPtr pointer) : base(pointer){}
                public EventSurvivalTeammateRespawn(bool force) : base("survival_teammate_respawn", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("switch_team")]
            public class EventSwitchTeam : GameEvent
            {
                public EventSwitchTeam(IntPtr pointer) : base(pointer){}
                public EventSwitchTeam(bool force) : base("switch_team", force){}

                
                
                // number of active players on both T and CT
                public int Numplayers 
                {
                    get => Get<int>("numPlayers");
                    set => Set<int>("numPlayers", value);
                }

                
                // number of spectators
                public int Numspectators 
                {
                    get => Get<int>("numSpectators");
                    set => Set<int>("numSpectators", value);
                }

                
                // average rank of human players
                public int AvgRank 
                {
                    get => Get<int>("avg_rank");
                    set => Set<int>("avg_rank", value);
                }

                
                
                public int Numtslotsfree 
                {
                    get => Get<int>("numTSlotsFree");
                    set => Set<int>("numTSlotsFree", value);
                }

                
                
                public int Numctslotsfree 
                {
                    get => Get<int>("numCTSlotsFree");
                    set => Set<int>("numCTSlotsFree", value);
                }
            }

            [EventName("tagrenade_detonate")]
            public class EventTagrenadeDetonate : GameEvent
            {
                public EventTagrenadeDetonate(IntPtr pointer) : base(pointer){}
                public EventTagrenadeDetonate(bool force) : base("tagrenade_detonate", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }

                
                
                public float X 
                {
                    get => Get<float>("x");
                    set => Set<float>("x", value);
                }

                
                
                public float Y 
                {
                    get => Get<float>("y");
                    set => Set<float>("y", value);
                }

                
                
                public float Z 
                {
                    get => Get<float>("z");
                    set => Set<float>("z", value);
                }
            }

            [EventName("teamchange_pending")]
            public class EventTeamchangePending : GameEvent
            {
                public EventTeamchangePending(IntPtr pointer) : base(pointer){}
                public EventTeamchangePending(bool force) : base("teamchange_pending", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                
                public int Toteam 
                {
                    get => Get<int>("toteam");
                    set => Set<int>("toteam", value);
                }
            }

            [EventName("team_info")]
            public class EventTeamInfo : GameEvent
            {
                public EventTeamInfo(IntPtr pointer) : base(pointer){}
                public EventTeamInfo(bool force) : base("team_info", force){}

                
                
                // unique team id
                public int Teamid 
                {
                    get => Get<int>("teamid");
                    set => Set<int>("teamid", value);
                }

                
                // team name eg "Team Blue"
                public string Teamname 
                {
                    get => Get<string>("teamname");
                    set => Set<string>("teamname", value);
                }
            }

            [EventName("team_intro_end")]
            public class EventTeamIntroEnd : GameEvent
            {
                public EventTeamIntroEnd(IntPtr pointer) : base(pointer){}
                public EventTeamIntroEnd(bool force) : base("team_intro_end", force){}

                
            }

            [EventName("team_intro_start")]
            public class EventTeamIntroStart : GameEvent
            {
                public EventTeamIntroStart(IntPtr pointer) : base(pointer){}
                public EventTeamIntroStart(bool force) : base("team_intro_start", force){}

                
            }

            [EventName("teamplay_broadcast_audio")]
            public class EventTeamplayBroadcastAudio : GameEvent
            {
                public EventTeamplayBroadcastAudio(IntPtr pointer) : base(pointer){}
                public EventTeamplayBroadcastAudio(bool force) : base("teamplay_broadcast_audio", force){}

                
                
                // unique team id
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                // name of the sound to emit
                public string Sound 
                {
                    get => Get<string>("sound");
                    set => Set<string>("sound", value);
                }
            }

            [EventName("teamplay_round_start")]
            public class EventTeamplayRoundStart : GameEvent
            {
                public EventTeamplayRoundStart(IntPtr pointer) : base(pointer){}
                public EventTeamplayRoundStart(bool force) : base("teamplay_round_start", force){}

                
                
                // is this a full reset of the map
                public bool FullReset 
                {
                    get => Get<bool>("full_reset");
                    set => Set<bool>("full_reset", value);
                }
            }

            [EventName("team_score")]
            public class EventTeamScore : GameEvent
            {
                public EventTeamScore(IntPtr pointer) : base(pointer){}
                public EventTeamScore(bool force) : base("team_score", force){}

                
                
                // team id
                public int Teamid 
                {
                    get => Get<int>("teamid");
                    set => Set<int>("teamid", value);
                }

                
                // total team score
                public int Score 
                {
                    get => Get<int>("score");
                    set => Set<int>("score", value);
                }
            }

            [EventName("tournament_reward")]
            public class EventTournamentReward : GameEvent
            {
                public EventTournamentReward(IntPtr pointer) : base(pointer){}
                public EventTournamentReward(bool force) : base("tournament_reward", force){}

                
                
                
                public long Defindex 
                {
                    get => Get<long>("defindex");
                    set => Set<long>("defindex", value);
                }

                
                
                public long Totalrewards 
                {
                    get => Get<long>("totalrewards");
                    set => Set<long>("totalrewards", value);
                }

                
                
                public long Accountid 
                {
                    get => Get<long>("accountid");
                    set => Set<long>("accountid", value);
                }
            }

            [EventName("tr_exit_hint_trigger")]
            public class EventTrExitHintTrigger : GameEvent
            {
                public EventTrExitHintTrigger(IntPtr pointer) : base(pointer){}
                public EventTrExitHintTrigger(bool force) : base("tr_exit_hint_trigger", force){}

                
            }

            [EventName("trial_time_expired")]
            public class EventTrialTimeExpired : GameEvent
            {
                public EventTrialTimeExpired(IntPtr pointer) : base(pointer){}
                public EventTrialTimeExpired(bool force) : base("trial_time_expired", force){}

                
                
                // player whose time has expired
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("tr_mark_best_time")]
            public class EventTrMarkBestTime : GameEvent
            {
                public EventTrMarkBestTime(IntPtr pointer) : base(pointer){}
                public EventTrMarkBestTime(bool force) : base("tr_mark_best_time", force){}

                
                
                
                public long Time 
                {
                    get => Get<long>("time");
                    set => Set<long>("time", value);
                }
            }

            [EventName("tr_mark_complete")]
            public class EventTrMarkComplete : GameEvent
            {
                public EventTrMarkComplete(IntPtr pointer) : base(pointer){}
                public EventTrMarkComplete(bool force) : base("tr_mark_complete", force){}

                
                
                
                public int Complete 
                {
                    get => Get<int>("complete");
                    set => Set<int>("complete", value);
                }
            }

            [EventName("tr_player_flashbanged")]
            public class EventTrPlayerFlashbanged : GameEvent
            {
                public EventTrPlayerFlashbanged(IntPtr pointer) : base(pointer){}
                public EventTrPlayerFlashbanged(bool force) : base("tr_player_flashbanged", force){}

                
                
                // user ID of the player banged
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("tr_show_exit_msgbox")]
            public class EventTrShowExitMsgbox : GameEvent
            {
                public EventTrShowExitMsgbox(IntPtr pointer) : base(pointer){}
                public EventTrShowExitMsgbox(bool force) : base("tr_show_exit_msgbox", force){}

                
            }

            [EventName("tr_show_finish_msgbox")]
            public class EventTrShowFinishMsgbox : GameEvent
            {
                public EventTrShowFinishMsgbox(IntPtr pointer) : base(pointer){}
                public EventTrShowFinishMsgbox(bool force) : base("tr_show_finish_msgbox", force){}

                
            }

            [EventName("ugc_file_download_finished")]
            public class EventUgcFileDownloadFinished : GameEvent
            {
                public EventUgcFileDownloadFinished(IntPtr pointer) : base(pointer){}
                public EventUgcFileDownloadFinished(bool force) : base("ugc_file_download_finished", force){}

                
                
                // id of this specific content (may be image or map)
                public ulong Hcontent 
                {
                    get => Get<ulong>("hcontent");
                    set => Set<ulong>("hcontent", value);
                }
            }

            [EventName("ugc_file_download_start")]
            public class EventUgcFileDownloadStart : GameEvent
            {
                public EventUgcFileDownloadStart(IntPtr pointer) : base(pointer){}
                public EventUgcFileDownloadStart(bool force) : base("ugc_file_download_start", force){}

                
                
                // id of this specific content (may be image or map)
                public ulong Hcontent 
                {
                    get => Get<ulong>("hcontent");
                    set => Set<ulong>("hcontent", value);
                }

                
                // id of the associated content package
                public ulong PublishedFileId 
                {
                    get => Get<ulong>("published_file_id");
                    set => Set<ulong>("published_file_id", value);
                }
            }

            [EventName("ugc_map_download_error")]
            public class EventUgcMapDownloadError : GameEvent
            {
                public EventUgcMapDownloadError(IntPtr pointer) : base(pointer){}
                public EventUgcMapDownloadError(bool force) : base("ugc_map_download_error", force){}

                
                
                
                public ulong PublishedFileId 
                {
                    get => Get<ulong>("published_file_id");
                    set => Set<ulong>("published_file_id", value);
                }

                
                
                public long ErrorCode 
                {
                    get => Get<long>("error_code");
                    set => Set<long>("error_code", value);
                }
            }

            [EventName("ugc_map_info_received")]
            public class EventUgcMapInfoReceived : GameEvent
            {
                public EventUgcMapInfoReceived(IntPtr pointer) : base(pointer){}
                public EventUgcMapInfoReceived(bool force) : base("ugc_map_info_received", force){}

                
                
                
                public ulong PublishedFileId 
                {
                    get => Get<ulong>("published_file_id");
                    set => Set<ulong>("published_file_id", value);
                }
            }

            [EventName("ugc_map_unsubscribed")]
            public class EventUgcMapUnsubscribed : GameEvent
            {
                public EventUgcMapUnsubscribed(IntPtr pointer) : base(pointer){}
                public EventUgcMapUnsubscribed(bool force) : base("ugc_map_unsubscribed", force){}

                
                
                
                public ulong PublishedFileId 
                {
                    get => Get<ulong>("published_file_id");
                    set => Set<ulong>("published_file_id", value);
                }
            }

            [EventName("update_matchmaking_stats")]
            public class EventUpdateMatchmakingStats : GameEvent
            {
                public EventUpdateMatchmakingStats(IntPtr pointer) : base(pointer){}
                public EventUpdateMatchmakingStats(bool force) : base("update_matchmaking_stats", force){}

                
            }

            [EventName("user_data_downloaded")]
            public class EventUserDataDownloaded : GameEvent
            {
                public EventUserDataDownloaded(IntPtr pointer) : base(pointer){}
                public EventUserDataDownloaded(bool force) : base("user_data_downloaded", force){}

                
            }

            [EventName("vip_escaped")]
            public class EventVipEscaped : GameEvent
            {
                public EventVipEscaped(IntPtr pointer) : base(pointer){}
                public EventVipEscaped(bool force) : base("vip_escaped", force){}

                
                
                // player who was the VIP
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("vip_killed")]
            public class EventVipKilled : GameEvent
            {
                public EventVipKilled(IntPtr pointer) : base(pointer){}
                public EventVipKilled(bool force) : base("vip_killed", force){}

                
                
                // player who was the VIP
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // user ID who killed the VIP
                public CCSPlayerController Attacker 
                {
                    get => Get<CCSPlayerController>("attacker");
                    set => Set<CCSPlayerController>("attacker", value);
                }
            }

            [EventName("vote_cast")]
            public class EventVoteCast : GameEvent
            {
                public EventVoteCast(IntPtr pointer) : base(pointer){}
                public EventVoteCast(bool force) : base("vote_cast", force){}

                
                
                // which option the player voted on
                public int VoteOption 
                {
                    get => Get<int>("vote_option");
                    set => Set<int>("vote_option", value);
                }

                
                
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                // player who voted
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("vote_cast_no")]
            public class EventVoteCastNo : GameEvent
            {
                public EventVoteCastNo(IntPtr pointer) : base(pointer){}
                public EventVoteCastNo(bool force) : base("vote_cast_no", force){}

                
                
                
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                // entity id of the voter
                public long Entityid 
                {
                    get => Get<long>("entityid");
                    set => Set<long>("entityid", value);
                }
            }

            [EventName("vote_cast_yes")]
            public class EventVoteCastYes : GameEvent
            {
                public EventVoteCastYes(IntPtr pointer) : base(pointer){}
                public EventVoteCastYes(bool force) : base("vote_cast_yes", force){}

                
                
                
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                // entity id of the voter
                public long Entityid 
                {
                    get => Get<long>("entityid");
                    set => Set<long>("entityid", value);
                }
            }

            [EventName("vote_changed")]
            public class EventVoteChanged : GameEvent
            {
                public EventVoteChanged(IntPtr pointer) : base(pointer){}
                public EventVoteChanged(bool force) : base("vote_changed", force){}

                
                
                
                public int VoteOption1 
                {
                    get => Get<int>("vote_option1");
                    set => Set<int>("vote_option1", value);
                }

                
                
                public int VoteOption2 
                {
                    get => Get<int>("vote_option2");
                    set => Set<int>("vote_option2", value);
                }

                
                
                public int VoteOption3 
                {
                    get => Get<int>("vote_option3");
                    set => Set<int>("vote_option3", value);
                }

                
                
                public int VoteOption4 
                {
                    get => Get<int>("vote_option4");
                    set => Set<int>("vote_option4", value);
                }

                
                
                public int VoteOption5 
                {
                    get => Get<int>("vote_option5");
                    set => Set<int>("vote_option5", value);
                }

                
                
                public int Potentialvotes 
                {
                    get => Get<int>("potentialVotes");
                    set => Set<int>("potentialVotes", value);
                }
            }

            [EventName("vote_ended")]
            public class EventVoteEnded : GameEvent
            {
                public EventVoteEnded(IntPtr pointer) : base(pointer){}
                public EventVoteEnded(bool force) : base("vote_ended", force){}

                
            }

            [EventName("vote_failed")]
            public class EventVoteFailed : GameEvent
            {
                public EventVoteFailed(IntPtr pointer) : base(pointer){}
                public EventVoteFailed(bool force) : base("vote_failed", force){}

                
                
                
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                // this event is reliable
                public int Reliable 
                {
                    get => Get<int>("reliable");
                    set => Set<int>("reliable", value);
                }
            }

            [EventName("vote_options")]
            public class EventVoteOptions : GameEvent
            {
                public EventVoteOptions(IntPtr pointer) : base(pointer){}
                public EventVoteOptions(bool force) : base("vote_options", force){}

                
                
                // Number of options - up to MAX_VOTE_OPTIONS
                public int Count 
                {
                    get => Get<int>("count");
                    set => Set<int>("count", value);
                }

                
                
                public string Option1 
                {
                    get => Get<string>("option1");
                    set => Set<string>("option1", value);
                }

                
                
                public string Option2 
                {
                    get => Get<string>("option2");
                    set => Set<string>("option2", value);
                }

                
                
                public string Option3 
                {
                    get => Get<string>("option3");
                    set => Set<string>("option3", value);
                }

                
                
                public string Option4 
                {
                    get => Get<string>("option4");
                    set => Set<string>("option4", value);
                }

                
                
                public string Option5 
                {
                    get => Get<string>("option5");
                    set => Set<string>("option5", value);
                }
            }

            [EventName("vote_passed")]
            public class EventVotePassed : GameEvent
            {
                public EventVotePassed(IntPtr pointer) : base(pointer){}
                public EventVotePassed(bool force) : base("vote_passed", force){}

                
                
                
                public string Details 
                {
                    get => Get<string>("details");
                    set => Set<string>("details", value);
                }

                
                
                public string Param1 
                {
                    get => Get<string>("param1");
                    set => Set<string>("param1", value);
                }

                
                
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                // this event is reliable
                public int Reliable 
                {
                    get => Get<int>("reliable");
                    set => Set<int>("reliable", value);
                }
            }

            [EventName("vote_started")]
            public class EventVoteStarted : GameEvent
            {
                public EventVoteStarted(IntPtr pointer) : base(pointer){}
                public EventVoteStarted(bool force) : base("vote_started", force){}

                
                
                
                public string Issue 
                {
                    get => Get<string>("issue");
                    set => Set<string>("issue", value);
                }

                
                
                public string Param1 
                {
                    get => Get<string>("param1");
                    set => Set<string>("param1", value);
                }

                
                
                public int Team 
                {
                    get => Get<int>("team");
                    set => Set<int>("team", value);
                }

                
                // entity id of the player who initiated the vote
                public long Initiator 
                {
                    get => Get<long>("initiator");
                    set => Set<long>("initiator", value);
                }
            }

            [EventName("warmup_end")]
            public class EventWarmupEnd : GameEvent
            {
                public EventWarmupEnd(IntPtr pointer) : base(pointer){}
                public EventWarmupEnd(bool force) : base("warmup_end", force){}

                
            }

            [EventName("weapon_fire")]
            public class EventWeaponFire : GameEvent
            {
                public EventWeaponFire(IntPtr pointer) : base(pointer){}
                public EventWeaponFire(bool force) : base("weapon_fire", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // weapon name used
                public string Weapon 
                {
                    get => Get<string>("weapon");
                    set => Set<string>("weapon", value);
                }

                
                // is weapon silenced
                public bool Silenced 
                {
                    get => Get<bool>("silenced");
                    set => Set<bool>("silenced", value);
                }
            }

            [EventName("weapon_fire_on_empty")]
            public class EventWeaponFireOnEmpty : GameEvent
            {
                public EventWeaponFireOnEmpty(IntPtr pointer) : base(pointer){}
                public EventWeaponFireOnEmpty(bool force) : base("weapon_fire_on_empty", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // weapon name used
                public string Weapon 
                {
                    get => Get<string>("weapon");
                    set => Set<string>("weapon", value);
                }
            }

            [EventName("weaponhud_selection")]
            public class EventWeaponhudSelection : GameEvent
            {
                public EventWeaponhudSelection(IntPtr pointer) : base(pointer){}
                public EventWeaponhudSelection(bool force) : base("weaponhud_selection", force){}

                
                
                // Player who this event applies to
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }

                
                // EWeaponHudSelectionMode (switch / pickup / drop)
                public int Mode 
                {
                    get => Get<int>("mode");
                    set => Set<int>("mode", value);
                }

                
                // Weapon entity index
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("weapon_outofammo")]
            public class EventWeaponOutofammo : GameEvent
            {
                public EventWeaponOutofammo(IntPtr pointer) : base(pointer){}
                public EventWeaponOutofammo(bool force) : base("weapon_outofammo", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("weapon_reload")]
            public class EventWeaponReload : GameEvent
            {
                public EventWeaponReload(IntPtr pointer) : base(pointer){}
                public EventWeaponReload(bool force) : base("weapon_reload", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("weapon_zoom")]
            public class EventWeaponZoom : GameEvent
            {
                public EventWeaponZoom(IntPtr pointer) : base(pointer){}
                public EventWeaponZoom(bool force) : base("weapon_zoom", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("weapon_zoom_rifle")]
            public class EventWeaponZoomRifle : GameEvent
            {
                public EventWeaponZoomRifle(IntPtr pointer) : base(pointer){}
                public EventWeaponZoomRifle(bool force) : base("weapon_zoom_rifle", force){}

                
                
                
                public CCSPlayerController Userid 
                {
                    get => Get<CCSPlayerController>("userid");
                    set => Set<CCSPlayerController>("userid", value);
                }
            }

            [EventName("write_game_titledata")]
            public class EventWriteGameTitledata : GameEvent
            {
                public EventWriteGameTitledata(IntPtr pointer) : base(pointer){}
                public EventWriteGameTitledata(bool force) : base("write_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => Get<int>("controllerId");
                    set => Set<int>("controllerId", value);
                }
            }

            [EventName("write_profile_data")]
            public class EventWriteProfileData : GameEvent
            {
                public EventWriteProfileData(IntPtr pointer) : base(pointer){}
                public EventWriteProfileData(bool force) : base("write_profile_data", force){}

                
            }
}
