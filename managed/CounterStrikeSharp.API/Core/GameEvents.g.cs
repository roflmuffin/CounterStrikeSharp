
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core
{
    
            [EventName("achievement_earned")]
            public class EventAchievementEarned : GameEvent
            {
                public EventAchievementEarned() : base(){}
                public EventAchievementEarned(bool force) : base("achievement_earned", force){}

                
                
                // entindex of the player
                public int Player 
                {
                    get => Get<int>("player");
                    set => Set<int>("player", value);
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
                public EventAchievementEarnedLocal() : base(){}
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
                public EventAchievementEvent() : base(){}
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
                public EventAchievementInfoLoaded() : base(){}
                public EventAchievementInfoLoaded(bool force) : base("achievement_info_loaded", force){}

                
            }

            [EventName("achievement_write_failed")]
            public class EventAchievementWriteFailed : GameEvent
            {
                public EventAchievementWriteFailed() : base(){}
                public EventAchievementWriteFailed(bool force) : base("achievement_write_failed", force){}

                
            }

            [EventName("add_bullet_hit_marker")]
            public class EventAddBulletHitMarker : GameEvent
            {
                public EventAddBulletHitMarker() : base(){}
                public EventAddBulletHitMarker(bool force) : base("add_bullet_hit_marker", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventAddPlayerSonarIcon() : base(){}
                public EventAddPlayerSonarIcon(bool force) : base("add_player_sonar_icon", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventAmmoPickup() : base(){}
                public EventAmmoPickup(bool force) : base("ammo_pickup", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventAmmoRefill() : base(){}
                public EventAmmoRefill(bool force) : base("ammo_refill", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventAnnouncePhaseEnd() : base(){}
                public EventAnnouncePhaseEnd(bool force) : base("announce_phase_end", force){}

                
            }

            [EventName("begin_new_match")]
            public class EventBeginNewMatch : GameEvent
            {
                public EventBeginNewMatch() : base(){}
                public EventBeginNewMatch(bool force) : base("begin_new_match", force){}

                
            }

            [EventName("bomb_abortdefuse")]
            public class EventBombAbortdefuse : GameEvent
            {
                public EventBombAbortdefuse() : base(){}
                public EventBombAbortdefuse(bool force) : base("bomb_abortdefuse", force){}

                
                
                // player who was defusing
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("bomb_abortplant")]
            public class EventBombAbortplant : GameEvent
            {
                public EventBombAbortplant() : base(){}
                public EventBombAbortplant(bool force) : base("bomb_abortplant", force){}

                
                
                // player who is planting the bomb
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBombBeep() : base(){}
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
                public EventBombBegindefuse() : base(){}
                public EventBombBegindefuse(bool force) : base("bomb_begindefuse", force){}

                
                
                // player who is defusing
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBombBeginplant() : base(){}
                public EventBombBeginplant(bool force) : base("bomb_beginplant", force){}

                
                
                // player who is planting the bomb
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBombDefused() : base(){}
                public EventBombDefused(bool force) : base("bomb_defused", force){}

                
                
                // player who defused the bomb
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBombDropped() : base(){}
                public EventBombDropped(bool force) : base("bomb_dropped", force){}

                
                
                // player who dropped the bomb
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBombExploded() : base(){}
                public EventBombExploded(bool force) : base("bomb_exploded", force){}

                
                
                // player who planted the bomb
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBombPickup() : base(){}
                public EventBombPickup(bool force) : base("bomb_pickup", force){}

                
                
                // player pawn who picked up the bomb
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("bomb_planted")]
            public class EventBombPlanted : GameEvent
            {
                public EventBombPlanted() : base(){}
                public EventBombPlanted(bool force) : base("bomb_planted", force){}

                
                
                // player who planted the bomb
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBonusUpdated() : base(){}
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
                public EventBotTakeover() : base(){}
                public EventBotTakeover(bool force) : base("bot_takeover", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }

                
                
                public int Botid 
                {
                    get => Get<int>("botid");
                    set => Set<int>("botid", value);
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
                public EventBreakBreakable() : base(){}
                public EventBreakBreakable(bool force) : base("break_breakable", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }

                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBreakProp() : base(){}
                public EventBreakProp(bool force) : base("break_prop", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }

                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("broken_breakable")]
            public class EventBrokenBreakable : GameEvent
            {
                public EventBrokenBreakable() : base(){}
                public EventBrokenBreakable(bool force) : base("broken_breakable", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }

                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBulletFlightResolution() : base(){}
                public EventBulletFlightResolution(bool force) : base("bullet_flight_resolution", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBulletImpact() : base(){}
                public EventBulletImpact(bool force) : base("bullet_impact", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventBuymenuClose() : base(){}
                public EventBuymenuClose(bool force) : base("buymenu_close", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("buymenu_open")]
            public class EventBuymenuOpen : GameEvent
            {
                public EventBuymenuOpen() : base(){}
                public EventBuymenuOpen(bool force) : base("buymenu_open", force){}

                
            }

            [EventName("buytime_ended")]
            public class EventBuytimeEnded : GameEvent
            {
                public EventBuytimeEnded() : base(){}
                public EventBuytimeEnded(bool force) : base("buytime_ended", force){}

                
            }

            [EventName("cart_updated")]
            public class EventCartUpdated : GameEvent
            {
                public EventCartUpdated() : base(){}
                public EventCartUpdated(bool force) : base("cart_updated", force){}

                
            }

            [EventName("choppers_incoming_warning")]
            public class EventChoppersIncomingWarning : GameEvent
            {
                public EventChoppersIncomingWarning() : base(){}
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
                public EventClientDisconnect() : base(){}
                public EventClientDisconnect(bool force) : base("client_disconnect", force){}

                
            }

            [EventName("client_loadout_changed")]
            public class EventClientLoadoutChanged : GameEvent
            {
                public EventClientLoadoutChanged() : base(){}
                public EventClientLoadoutChanged(bool force) : base("client_loadout_changed", force){}

                
            }

            [EventName("clientside_lesson_closed")]
            public class EventClientsideLessonClosed : GameEvent
            {
                public EventClientsideLessonClosed() : base(){}
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
                public EventCsGameDisconnected() : base(){}
                public EventCsGameDisconnected(bool force) : base("cs_game_disconnected", force){}

                
            }

            [EventName("cs_intermission")]
            public class EventCsIntermission : GameEvent
            {
                public EventCsIntermission() : base(){}
                public EventCsIntermission(bool force) : base("cs_intermission", force){}

                
            }

            [EventName("cs_match_end_restart")]
            public class EventCsMatchEndRestart : GameEvent
            {
                public EventCsMatchEndRestart() : base(){}
                public EventCsMatchEndRestart(bool force) : base("cs_match_end_restart", force){}

                
            }

            [EventName("cs_pre_restart")]
            public class EventCsPreRestart : GameEvent
            {
                public EventCsPreRestart() : base(){}
                public EventCsPreRestart(bool force) : base("cs_pre_restart", force){}

                
            }

            [EventName("cs_prev_next_spectator")]
            public class EventCsPrevNextSpectator : GameEvent
            {
                public EventCsPrevNextSpectator() : base(){}
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
                public EventCsRoundFinalBeep() : base(){}
                public EventCsRoundFinalBeep(bool force) : base("cs_round_final_beep", force){}

                
            }

            [EventName("cs_round_start_beep")]
            public class EventCsRoundStartBeep : GameEvent
            {
                public EventCsRoundStartBeep() : base(){}
                public EventCsRoundStartBeep(bool force) : base("cs_round_start_beep", force){}

                
            }

            [EventName("cs_win_panel_match")]
            public class EventCsWinPanelMatch : GameEvent
            {
                public EventCsWinPanelMatch() : base(){}
                public EventCsWinPanelMatch(bool force) : base("cs_win_panel_match", force){}

                
            }

            [EventName("cs_win_panel_round")]
            public class EventCsWinPanelRound : GameEvent
            {
                public EventCsWinPanelRound() : base(){}
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

                
                
                public int FunfactPlayer 
                {
                    get => Get<int>("funfact_player");
                    set => Set<int>("funfact_player", value);
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
                public EventDecoyDetonate() : base(){}
                public EventDecoyDetonate(bool force) : base("decoy_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDecoyFiring() : base(){}
                public EventDecoyFiring(bool force) : base("decoy_firing", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDecoyStarted() : base(){}
                public EventDecoyStarted(bool force) : base("decoy_started", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDefuserDropped() : base(){}
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
                public EventDefuserPickup() : base(){}
                public EventDefuserPickup(bool force) : base("defuser_pickup", force){}

                
                
                // defuser's entity ID
                public long Entityid 
                {
                    get => Get<long>("entityid");
                    set => Set<long>("entityid", value);
                }

                
                // player who picked up the defuser
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("demo_skip")]
            public class EventDemoSkip : GameEvent
            {
                public EventDemoSkip() : base(){}
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
                public EventDemoStart() : base(){}
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
                public EventDemoStop() : base(){}
                public EventDemoStop(bool force) : base("demo_stop", force){}

                
            }

            [EventName("difficulty_changed")]
            public class EventDifficultyChanged : GameEvent
            {
                public EventDifficultyChanged() : base(){}
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
                public EventDmBonusWeaponStart() : base(){}
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
                public EventDoorBreak() : base(){}
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
                public EventDoorClose() : base(){}
                public EventDoorClose(bool force) : base("door_close", force){}

                
                
                // Who closed the door
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDoorClosed() : base(){}
                public EventDoorClosed(bool force) : base("door_closed", force){}

                
                
                // Who closed the door
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDoorMoving() : base(){}
                public EventDoorMoving(bool force) : base("door_moving", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDoorOpen() : base(){}
                public EventDoorOpen(bool force) : base("door_open", force){}

                
                
                // Who closed the door
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDroneAboveRoof() : base(){}
                public EventDroneAboveRoof(bool force) : base("drone_above_roof", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDroneCargoDetached() : base(){}
                public EventDroneCargoDetached(bool force) : base("drone_cargo_detached", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDroneDispatched() : base(){}
                public EventDroneDispatched(bool force) : base("drone_dispatched", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventDronegunAttack() : base(){}
                public EventDronegunAttack(bool force) : base("dronegun_attack", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("drop_rate_modified")]
            public class EventDropRateModified : GameEvent
            {
                public EventDropRateModified() : base(){}
                public EventDropRateModified(bool force) : base("drop_rate_modified", force){}

                
            }

            [EventName("dynamic_shadow_light_changed")]
            public class EventDynamicShadowLightChanged : GameEvent
            {
                public EventDynamicShadowLightChanged() : base(){}
                public EventDynamicShadowLightChanged(bool force) : base("dynamic_shadow_light_changed", force){}

                
            }

            [EventName("dz_item_interaction")]
            public class EventDzItemInteraction : GameEvent
            {
                public EventDzItemInteraction() : base(){}
                public EventDzItemInteraction(bool force) : base("dz_item_interaction", force){}

                
                
                // player entindex
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventEnableRestartVoting() : base(){}
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
                public EventEndmatchCmmStartRevealItems() : base(){}
                public EventEndmatchCmmStartRevealItems(bool force) : base("endmatch_cmm_start_reveal_items", force){}

                
            }

            [EventName("endmatch_mapvote_selecting_map")]
            public class EventEndmatchMapvoteSelectingMap : GameEvent
            {
                public EventEndmatchMapvoteSelectingMap() : base(){}
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
                public EventEnterBombzone() : base(){}
                public EventEnterBombzone(bool force) : base("enter_bombzone", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventEnterBuyzone() : base(){}
                public EventEnterBuyzone(bool force) : base("enter_buyzone", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventEnterRescueZone() : base(){}
                public EventEnterRescueZone(bool force) : base("enter_rescue_zone", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("entity_killed")]
            public class EventEntityKilled : GameEvent
            {
                public EventEntityKilled() : base(){}
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
                public EventEntityVisible() : base(){}
                public EventEntityVisible(bool force) : base("entity_visible", force){}

                
                
                // The player who sees the entity
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventEventTicketModified() : base(){}
                public EventEventTicketModified(bool force) : base("event_ticket_modified", force){}

                
            }

            [EventName("exit_bombzone")]
            public class EventExitBombzone : GameEvent
            {
                public EventExitBombzone() : base(){}
                public EventExitBombzone(bool force) : base("exit_bombzone", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventExitBuyzone() : base(){}
                public EventExitBuyzone(bool force) : base("exit_buyzone", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventExitRescueZone() : base(){}
                public EventExitRescueZone(bool force) : base("exit_rescue_zone", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("finale_start")]
            public class EventFinaleStart : GameEvent
            {
                public EventFinaleStart() : base(){}
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
                public EventFirstbombsIncomingWarning() : base(){}
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
                public EventFlareIgniteNpc() : base(){}
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
                public EventFlashbangDetonate() : base(){}
                public EventFlashbangDetonate(bool force) : base("flashbang_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventGameEnd() : base(){}
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
                public EventGameInit() : base(){}
                public EventGameInit(bool force) : base("game_init", force){}

                
            }

            [EventName("gameinstructor_draw")]
            public class EventGameinstructorDraw : GameEvent
            {
                public EventGameinstructorDraw() : base(){}
                public EventGameinstructorDraw(bool force) : base("gameinstructor_draw", force){}

                
            }

            [EventName("gameinstructor_nodraw")]
            public class EventGameinstructorNodraw : GameEvent
            {
                public EventGameinstructorNodraw() : base(){}
                public EventGameinstructorNodraw(bool force) : base("gameinstructor_nodraw", force){}

                
            }

            [EventName("game_message")]
            public class EventGameMessage : GameEvent
            {
                public EventGameMessage() : base(){}
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
                public EventGameNewmap() : base(){}
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
                public EventGamePhaseChanged() : base(){}
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
                public EventGameStart() : base(){}
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
                public EventGameuiHidden() : base(){}
                public EventGameuiHidden(bool force) : base("gameui_hidden", force){}

                
            }

            [EventName("gc_connected")]
            public class EventGcConnected : GameEvent
            {
                public EventGcConnected() : base(){}
                public EventGcConnected(bool force) : base("gc_connected", force){}

                
            }

            [EventName("gg_killed_enemy")]
            public class EventGgKilledEnemy : GameEvent
            {
                public EventGgKilledEnemy() : base(){}
                public EventGgKilledEnemy(bool force) : base("gg_killed_enemy", force){}

                
                
                // user ID who died
                public int Victimid 
                {
                    get => Get<int>("victimid");
                    set => Set<int>("victimid", value);
                }

                
                // user ID who killed
                public int Attackerid 
                {
                    get => Get<int>("attackerid");
                    set => Set<int>("attackerid", value);
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
                public EventGrenadeBounce() : base(){}
                public EventGrenadeBounce(bool force) : base("grenade_bounce", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("grenade_thrown")]
            public class EventGrenadeThrown : GameEvent
            {
                public EventGrenadeThrown() : base(){}
                public EventGrenadeThrown(bool force) : base("grenade_thrown", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventGuardianWaveRestart() : base(){}
                public EventGuardianWaveRestart(bool force) : base("guardian_wave_restart", force){}

                
            }

            [EventName("hegrenade_detonate")]
            public class EventHegrenadeDetonate : GameEvent
            {
                public EventHegrenadeDetonate() : base(){}
                public EventHegrenadeDetonate(bool force) : base("hegrenade_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventHelicopterGrenadePuntMiss() : base(){}
                public EventHelicopterGrenadePuntMiss(bool force) : base("helicopter_grenade_punt_miss", force){}

                
            }

            [EventName("hide_deathpanel")]
            public class EventHideDeathpanel : GameEvent
            {
                public EventHideDeathpanel() : base(){}
                public EventHideDeathpanel(bool force) : base("hide_deathpanel", force){}

                
            }

            [EventName("hltv_cameraman")]
            public class EventHltvCameraman : GameEvent
            {
                public EventHltvCameraman() : base(){}
                public EventHltvCameraman(bool force) : base("hltv_cameraman", force){}

                
                
                // camera man entity index
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("hltv_changed_mode")]
            public class EventHltvChangedMode : GameEvent
            {
                public EventHltvChangedMode() : base(){}
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
                public EventHltvChase() : base(){}
                public EventHltvChase(bool force) : base("hltv_chase", force){}

                
                
                // primary traget index
                public int Target1 
                {
                    get => Get<int>("target1");
                    set => Set<int>("target1", value);
                }

                
                // secondary traget index or 0
                public int Target2 
                {
                    get => Get<int>("target2");
                    set => Set<int>("target2", value);
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
                public EventHltvChat() : base(){}
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
                public EventHltvFixed() : base(){}
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
                public int Target 
                {
                    get => Get<int>("target");
                    set => Set<int>("target", value);
                }
            }

            [EventName("hltv_message")]
            public class EventHltvMessage : GameEvent
            {
                public EventHltvMessage() : base(){}
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
                public EventHltvRankCamera() : base(){}
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
                public int Target 
                {
                    get => Get<int>("target");
                    set => Set<int>("target", value);
                }
            }

            [EventName("hltv_rank_entity")]
            public class EventHltvRankEntity : GameEvent
            {
                public EventHltvRankEntity() : base(){}
                public EventHltvRankEntity(bool force) : base("hltv_rank_entity", force){}

                
                
                // player slot
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }

                
                // ranking, how interesting is this entity to view
                public float Rank 
                {
                    get => Get<float>("rank");
                    set => Set<float>("rank", value);
                }

                
                // best/closest target entity
                public int Target 
                {
                    get => Get<int>("target");
                    set => Set<int>("target", value);
                }
            }

            [EventName("hltv_replay")]
            public class EventHltvReplay : GameEvent
            {
                public EventHltvReplay() : base(){}
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
                public EventHltvReplayStatus() : base(){}
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
                public EventHltvStatus() : base(){}
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
                public EventHltvTitle() : base(){}
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
                public EventHltvVersioninfo() : base(){}
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
                public EventHostageCallForHelp() : base(){}
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
                public EventHostageFollows() : base(){}
                public EventHostageFollows(bool force) : base("hostage_follows", force){}

                
                
                // player who touched the hostage
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventHostageHurt() : base(){}
                public EventHostageHurt(bool force) : base("hostage_hurt", force){}

                
                
                // player who hurt the hostage
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventHostageKilled() : base(){}
                public EventHostageKilled(bool force) : base("hostage_killed", force){}

                
                
                // player who killed the hostage
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventHostageRescued() : base(){}
                public EventHostageRescued(bool force) : base("hostage_rescued", force){}

                
                
                // player who rescued the hostage
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventHostageRescuedAll() : base(){}
                public EventHostageRescuedAll(bool force) : base("hostage_rescued_all", force){}

                
            }

            [EventName("hostage_stops_following")]
            public class EventHostageStopsFollowing : GameEvent
            {
                public EventHostageStopsFollowing() : base(){}
                public EventHostageStopsFollowing(bool force) : base("hostage_stops_following", force){}

                
                
                // player who rescued the hostage
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventHostnameChanged() : base(){}
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
                public EventInfernoExpire() : base(){}
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
                public EventInfernoExtinguish() : base(){}
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
                public EventInfernoStartburn() : base(){}
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
                public EventInspectWeapon() : base(){}
                public EventInspectWeapon(bool force) : base("inspect_weapon", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("instructor_close_lesson")]
            public class EventInstructorCloseLesson : GameEvent
            {
                public EventInstructorCloseLesson() : base(){}
                public EventInstructorCloseLesson(bool force) : base("instructor_close_lesson", force){}

                
                
                // The player who this lesson is intended for
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventInstructorServerHintCreate() : base(){}
                public EventInstructorServerHintCreate(bool force) : base("instructor_server_hint_create", force){}

                
                
                // user ID of the player that triggered the hint
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public int HintActivatorUserid 
                {
                    get => Get<int>("hint_activator_userid");
                    set => Set<int>("hint_activator_userid", value);
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
                public EventInstructorServerHintStop() : base(){}
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
                public EventInstructorStartLesson() : base(){}
                public EventInstructorStartLesson(bool force) : base("instructor_start_lesson", force){}

                
                
                // The player who this lesson is intended for
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventInventoryUpdated() : base(){}
                public EventInventoryUpdated(bool force) : base("inventory_updated", force){}

                
            }

            [EventName("item_equip")]
            public class EventItemEquip : GameEvent
            {
                public EventItemEquip() : base(){}
                public EventItemEquip(bool force) : base("item_equip", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventItemPickup() : base(){}
                public EventItemPickup(bool force) : base("item_pickup", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventItemPickupFailed() : base(){}
                public EventItemPickupFailed(bool force) : base("item_pickup_failed", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventItemPickupSlerp() : base(){}
                public EventItemPickupSlerp(bool force) : base("item_pickup_slerp", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventItemPurchase() : base(){}
                public EventItemPurchase(bool force) : base("item_purchase", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventItemRemove() : base(){}
                public EventItemRemove(bool force) : base("item_remove", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventItemSchemaInitialized() : base(){}
                public EventItemSchemaInitialized(bool force) : base("item_schema_initialized", force){}

                
            }

            [EventName("items_gifted")]
            public class EventItemsGifted : GameEvent
            {
                public EventItemsGifted() : base(){}
                public EventItemsGifted(bool force) : base("items_gifted", force){}

                
                
                // entity used by player
                public int Player 
                {
                    get => Get<int>("player");
                    set => Set<int>("player", value);
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
                public EventJointeamFailed() : base(){}
                public EventJointeamFailed(bool force) : base("jointeam_failed", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventLocalPlayerControllerTeam() : base(){}
                public EventLocalPlayerControllerTeam(bool force) : base("local_player_controller_team", force){}

                
            }

            [EventName("local_player_pawn_changed")]
            public class EventLocalPlayerPawnChanged : GameEvent
            {
                public EventLocalPlayerPawnChanged() : base(){}
                public EventLocalPlayerPawnChanged(bool force) : base("local_player_pawn_changed", force){}

                
            }

            [EventName("local_player_team")]
            public class EventLocalPlayerTeam : GameEvent
            {
                public EventLocalPlayerTeam() : base(){}
                public EventLocalPlayerTeam(bool force) : base("local_player_team", force){}

                
            }

            [EventName("loot_crate_opened")]
            public class EventLootCrateOpened : GameEvent
            {
                public EventLootCrateOpened() : base(){}
                public EventLootCrateOpened(bool force) : base("loot_crate_opened", force){}

                
                
                // player entindex
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventLootCrateVisible() : base(){}
                public EventLootCrateVisible(bool force) : base("loot_crate_visible", force){}

                
                
                // player entindex
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventMapShutdown() : base(){}
                public EventMapShutdown(bool force) : base("map_shutdown", force){}

                
            }

            [EventName("map_transition")]
            public class EventMapTransition : GameEvent
            {
                public EventMapTransition() : base(){}
                public EventMapTransition(bool force) : base("map_transition", force){}

                
            }

            [EventName("match_end_conditions")]
            public class EventMatchEndConditions : GameEvent
            {
                public EventMatchEndConditions() : base(){}
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
                public EventMaterialDefaultComplete() : base(){}
                public EventMaterialDefaultComplete(bool force) : base("material_default_complete", force){}

                
            }

            [EventName("mb_input_lock_cancel")]
            public class EventMbInputLockCancel : GameEvent
            {
                public EventMbInputLockCancel() : base(){}
                public EventMbInputLockCancel(bool force) : base("mb_input_lock_cancel", force){}

                
            }

            [EventName("mb_input_lock_success")]
            public class EventMbInputLockSuccess : GameEvent
            {
                public EventMbInputLockSuccess() : base(){}
                public EventMbInputLockSuccess(bool force) : base("mb_input_lock_success", force){}

                
            }

            [EventName("molotov_detonate")]
            public class EventMolotovDetonate : GameEvent
            {
                public EventMolotovDetonate() : base(){}
                public EventMolotovDetonate(bool force) : base("molotov_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventNavBlocked() : base(){}
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
                public EventNavGenerate() : base(){}
                public EventNavGenerate(bool force) : base("nav_generate", force){}

                
            }

            [EventName("nextlevel_changed")]
            public class EventNextlevelChanged : GameEvent
            {
                public EventNextlevelChanged() : base(){}
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
                public EventOpenCrateInstr() : base(){}
                public EventOpenCrateInstr(bool force) : base("open_crate_instr", force){}

                
                
                // player entindex
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventOtherDeath() : base(){}
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
                public EventParachuteDeploy() : base(){}
                public EventParachuteDeploy(bool force) : base("parachute_deploy", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("parachute_pickup")]
            public class EventParachutePickup : GameEvent
            {
                public EventParachutePickup() : base(){}
                public EventParachutePickup(bool force) : base("parachute_pickup", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("physgun_pickup")]
            public class EventPhysgunPickup : GameEvent
            {
                public EventPhysgunPickup() : base(){}
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
                public EventPlayerActivate() : base(){}
                public EventPlayerActivate(bool force) : base("player_activate", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_avenged_teammate")]
            public class EventPlayerAvengedTeammate : GameEvent
            {
                public EventPlayerAvengedTeammate() : base(){}
                public EventPlayerAvengedTeammate(bool force) : base("player_avenged_teammate", force){}

                
                
                
                public int AvengerId 
                {
                    get => Get<int>("avenger_id");
                    set => Set<int>("avenger_id", value);
                }

                
                
                public int AvengedPlayerId 
                {
                    get => Get<int>("avenged_player_id");
                    set => Set<int>("avenged_player_id", value);
                }
            }

            [EventName("player_blind")]
            public class EventPlayerBlind : GameEvent
            {
                public EventPlayerBlind() : base(){}
                public EventPlayerBlind(bool force) : base("player_blind", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }

                
                // user ID who threw the flash
                public int Attacker 
                {
                    get => Get<int>("attacker");
                    set => Set<int>("attacker", value);
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
                public EventPlayerChangename() : base(){}
                public EventPlayerChangename(bool force) : base("player_changename", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerChat() : base(){}
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
                public EventPlayerConnect() : base(){}
                public EventPlayerConnect(bool force) : base("player_connect", force){}

                
                
                // player name
                public string Name 
                {
                    get => Get<string>("name");
                    set => Set<string>("name", value);
                }

                
                // user ID on server (unique on server)
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerConnectFull() : base(){}
                public EventPlayerConnectFull(bool force) : base("player_connect_full", force){}

                
                
                // user ID on server (unique on server)
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_death")]
            public class EventPlayerDeath : GameEvent
            {
                public EventPlayerDeath() : base(){}
                public EventPlayerDeath(bool force) : base("player_death", force){}

                
                
                // user who died
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }

                
                // player who killed
                public int Attacker 
                {
                    get => Get<int>("attacker");
                    set => Set<int>("attacker", value);
                }

                
                // player who assisted in the kill
                public int Assister 
                {
                    get => Get<int>("assister");
                    set => Set<int>("assister", value);
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
                public EventPlayerDecal() : base(){}
                public EventPlayerDecal(bool force) : base("player_decal", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_disconnect")]
            public class EventPlayerDisconnect : GameEvent
            {
                public EventPlayerDisconnect() : base(){}
                public EventPlayerDisconnect(bool force) : base("player_disconnect", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerFalldamage() : base(){}
                public EventPlayerFalldamage(bool force) : base("player_falldamage", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerFootstep() : base(){}
                public EventPlayerFootstep(bool force) : base("player_footstep", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_full_update")]
            public class EventPlayerFullUpdate : GameEvent
            {
                public EventPlayerFullUpdate() : base(){}
                public EventPlayerFullUpdate(bool force) : base("player_full_update", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerGivenC4() : base(){}
                public EventPlayerGivenC4(bool force) : base("player_given_c4", force){}

                
                
                // user ID who received the c4
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_hintmessage")]
            public class EventPlayerHintmessage : GameEvent
            {
                public EventPlayerHintmessage() : base(){}
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
                public EventPlayerHurt() : base(){}
                public EventPlayerHurt(bool force) : base("player_hurt", force){}

                
                
                // player index who was hurt
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }

                
                // player index who attacked
                public int Attacker 
                {
                    get => Get<int>("attacker");
                    set => Set<int>("attacker", value);
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
                public EventPlayerInfo() : base(){}
                public EventPlayerInfo(bool force) : base("player_info", force){}

                
                
                // player name
                public string Name 
                {
                    get => Get<string>("name");
                    set => Set<string>("name", value);
                }

                
                // user ID on server (unique on server)
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerJump() : base(){}
                public EventPlayerJump(bool force) : base("player_jump", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_ping")]
            public class EventPlayerPing : GameEvent
            {
                public EventPlayerPing() : base(){}
                public EventPlayerPing(bool force) : base("player_ping", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerPingStop() : base(){}
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
                public EventPlayerRadio() : base(){}
                public EventPlayerRadio(bool force) : base("player_radio", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerResetVote() : base(){}
                public EventPlayerResetVote(bool force) : base("player_reset_vote", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerScore() : base(){}
                public EventPlayerScore(bool force) : base("player_score", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerShoot() : base(){}
                public EventPlayerShoot(bool force) : base("player_shoot", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerSound() : base(){}
                public EventPlayerSound(bool force) : base("player_sound", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerSpawn() : base(){}
                public EventPlayerSpawn(bool force) : base("player_spawn", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_spawned")]
            public class EventPlayerSpawned : GameEvent
            {
                public EventPlayerSpawned() : base(){}
                public EventPlayerSpawned(bool force) : base("player_spawned", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventPlayerStatsUpdated() : base(){}
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
                public EventPlayerTeam() : base(){}
                public EventPlayerTeam(bool force) : base("player_team", force){}

                
                
                // player
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventRagdollDissolved() : base(){}
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
                public EventReadGameTitledata() : base(){}
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
                public EventRepostXboxAchievements() : base(){}
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
                public EventResetGameTitledata() : base(){}
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
                public EventRoundAnnounceFinal() : base(){}
                public EventRoundAnnounceFinal(bool force) : base("round_announce_final", force){}

                
            }

            [EventName("round_announce_last_round_half")]
            public class EventRoundAnnounceLastRoundHalf : GameEvent
            {
                public EventRoundAnnounceLastRoundHalf() : base(){}
                public EventRoundAnnounceLastRoundHalf(bool force) : base("round_announce_last_round_half", force){}

                
            }

            [EventName("round_announce_match_point")]
            public class EventRoundAnnounceMatchPoint : GameEvent
            {
                public EventRoundAnnounceMatchPoint() : base(){}
                public EventRoundAnnounceMatchPoint(bool force) : base("round_announce_match_point", force){}

                
            }

            [EventName("round_announce_match_start")]
            public class EventRoundAnnounceMatchStart : GameEvent
            {
                public EventRoundAnnounceMatchStart() : base(){}
                public EventRoundAnnounceMatchStart(bool force) : base("round_announce_match_start", force){}

                
            }

            [EventName("round_announce_warmup")]
            public class EventRoundAnnounceWarmup : GameEvent
            {
                public EventRoundAnnounceWarmup() : base(){}
                public EventRoundAnnounceWarmup(bool force) : base("round_announce_warmup", force){}

                
            }

            [EventName("round_end")]
            public class EventRoundEnd : GameEvent
            {
                public EventRoundEnd() : base(){}
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
                public EventRoundEndUploadStats() : base(){}
                public EventRoundEndUploadStats(bool force) : base("round_end_upload_stats", force){}

                
            }

            [EventName("round_freeze_end")]
            public class EventRoundFreezeEnd : GameEvent
            {
                public EventRoundFreezeEnd() : base(){}
                public EventRoundFreezeEnd(bool force) : base("round_freeze_end", force){}

                
            }

            [EventName("round_mvp")]
            public class EventRoundMvp : GameEvent
            {
                public EventRoundMvp() : base(){}
                public EventRoundMvp(bool force) : base("round_mvp", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventRoundOfficiallyEnded() : base(){}
                public EventRoundOfficiallyEnded(bool force) : base("round_officially_ended", force){}

                
            }

            [EventName("round_poststart")]
            public class EventRoundPoststart : GameEvent
            {
                public EventRoundPoststart() : base(){}
                public EventRoundPoststart(bool force) : base("round_poststart", force){}

                
            }

            [EventName("round_prestart")]
            public class EventRoundPrestart : GameEvent
            {
                public EventRoundPrestart() : base(){}
                public EventRoundPrestart(bool force) : base("round_prestart", force){}

                
            }

            [EventName("round_start")]
            public class EventRoundStart : GameEvent
            {
                public EventRoundStart() : base(){}
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
                public EventRoundStartPostNav() : base(){}
                public EventRoundStartPostNav(bool force) : base("round_start_post_nav", force){}

                
            }

            [EventName("round_start_pre_entity")]
            public class EventRoundStartPreEntity : GameEvent
            {
                public EventRoundStartPreEntity() : base(){}
                public EventRoundStartPreEntity(bool force) : base("round_start_pre_entity", force){}

                
            }

            [EventName("round_time_warning")]
            public class EventRoundTimeWarning : GameEvent
            {
                public EventRoundTimeWarning() : base(){}
                public EventRoundTimeWarning(bool force) : base("round_time_warning", force){}

                
            }

            [EventName("seasoncoin_levelup")]
            public class EventSeasoncoinLevelup : GameEvent
            {
                public EventSeasoncoinLevelup() : base(){}
                public EventSeasoncoinLevelup(bool force) : base("seasoncoin_levelup", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventServerCvar() : base(){}
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
                public EventServerMessage() : base(){}
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
                public EventServerPreShutdown() : base(){}
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
                public EventServerShutdown() : base(){}
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
                public EventServerSpawn() : base(){}
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
                public EventSetInstructorGroupEnabled() : base(){}
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
                public EventSfuievent() : base(){}
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
                public EventShowDeathpanel() : base(){}
                public EventShowDeathpanel(bool force) : base("show_deathpanel", force){}

                
                
                // endindex of the one who was killed
                public int Victim 
                {
                    get => Get<int>("victim");
                    set => Set<int>("victim", value);
                }

                
                // entindex of the killer entity
                public IntPtr Killer 
                {
                    get => Get<IntPtr>("killer");
                    set => Set<IntPtr>("killer", value);
                }

                
                
                public int KillerController 
                {
                    get => Get<int>("killer_controller");
                    set => Set<int>("killer_controller", value);
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
                public EventShowSurvivalRespawnStatus() : base(){}
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

                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("silencer_detach")]
            public class EventSilencerDetach : GameEvent
            {
                public EventSilencerDetach() : base(){}
                public EventSilencerDetach(bool force) : base("silencer_detach", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("silencer_off")]
            public class EventSilencerOff : GameEvent
            {
                public EventSilencerOff() : base(){}
                public EventSilencerOff(bool force) : base("silencer_off", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("silencer_on")]
            public class EventSilencerOn : GameEvent
            {
                public EventSilencerOn() : base(){}
                public EventSilencerOn(bool force) : base("silencer_on", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("smoke_beacon_paradrop")]
            public class EventSmokeBeaconParadrop : GameEvent
            {
                public EventSmokeBeaconParadrop() : base(){}
                public EventSmokeBeaconParadrop(bool force) : base("smoke_beacon_paradrop", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventSmokegrenadeDetonate() : base(){}
                public EventSmokegrenadeDetonate(bool force) : base("smokegrenade_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventSmokegrenadeExpired() : base(){}
                public EventSmokegrenadeExpired(bool force) : base("smokegrenade_expired", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventSpecModeUpdated() : base(){}
                public EventSpecModeUpdated(bool force) : base("spec_mode_updated", force){}

                
                
                // entindex of the player
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("spec_target_updated")]
            public class EventSpecTargetUpdated : GameEvent
            {
                public EventSpecTargetUpdated() : base(){}
                public EventSpecTargetUpdated(bool force) : base("spec_target_updated", force){}

                
                
                // spectating player
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventStartHalftime() : base(){}
                public EventStartHalftime(bool force) : base("start_halftime", force){}

                
            }

            [EventName("start_vote")]
            public class EventStartVote : GameEvent
            {
                public EventStartVote() : base(){}
                public EventStartVote(bool force) : base("start_vote", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventStorePricesheetUpdated() : base(){}
                public EventStorePricesheetUpdated(bool force) : base("store_pricesheet_updated", force){}

                
            }

            [EventName("survival_announce_phase")]
            public class EventSurvivalAnnouncePhase : GameEvent
            {
                public EventSurvivalAnnouncePhase() : base(){}
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
                public EventSurvivalNoRespawnsFinal() : base(){}
                public EventSurvivalNoRespawnsFinal(bool force) : base("survival_no_respawns_final", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("survival_no_respawns_warning")]
            public class EventSurvivalNoRespawnsWarning : GameEvent
            {
                public EventSurvivalNoRespawnsWarning() : base(){}
                public EventSurvivalNoRespawnsWarning(bool force) : base("survival_no_respawns_warning", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("survival_paradrop_break")]
            public class EventSurvivalParadropBreak : GameEvent
            {
                public EventSurvivalParadropBreak() : base(){}
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
                public EventSurvivalParadropSpawn() : base(){}
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
                public EventSurvivalTeammateRespawn() : base(){}
                public EventSurvivalTeammateRespawn(bool force) : base("survival_teammate_respawn", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("switch_team")]
            public class EventSwitchTeam : GameEvent
            {
                public EventSwitchTeam() : base(){}
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
                public EventTagrenadeDetonate() : base(){}
                public EventTagrenadeDetonate(bool force) : base("tagrenade_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventTeamchangePending() : base(){}
                public EventTeamchangePending(bool force) : base("teamchange_pending", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventTeamInfo() : base(){}
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
                public EventTeamIntroEnd() : base(){}
                public EventTeamIntroEnd(bool force) : base("team_intro_end", force){}

                
            }

            [EventName("team_intro_start")]
            public class EventTeamIntroStart : GameEvent
            {
                public EventTeamIntroStart() : base(){}
                public EventTeamIntroStart(bool force) : base("team_intro_start", force){}

                
            }

            [EventName("teamplay_broadcast_audio")]
            public class EventTeamplayBroadcastAudio : GameEvent
            {
                public EventTeamplayBroadcastAudio() : base(){}
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
                public EventTeamplayRoundStart() : base(){}
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
                public EventTeamScore() : base(){}
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
                public EventTournamentReward() : base(){}
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
                public EventTrExitHintTrigger() : base(){}
                public EventTrExitHintTrigger(bool force) : base("tr_exit_hint_trigger", force){}

                
            }

            [EventName("trial_time_expired")]
            public class EventTrialTimeExpired : GameEvent
            {
                public EventTrialTimeExpired() : base(){}
                public EventTrialTimeExpired(bool force) : base("trial_time_expired", force){}

                
                
                // player whose time has expired
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("tr_mark_best_time")]
            public class EventTrMarkBestTime : GameEvent
            {
                public EventTrMarkBestTime() : base(){}
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
                public EventTrMarkComplete() : base(){}
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
                public EventTrPlayerFlashbanged() : base(){}
                public EventTrPlayerFlashbanged(bool force) : base("tr_player_flashbanged", force){}

                
                
                // user ID of the player banged
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("tr_show_exit_msgbox")]
            public class EventTrShowExitMsgbox : GameEvent
            {
                public EventTrShowExitMsgbox() : base(){}
                public EventTrShowExitMsgbox(bool force) : base("tr_show_exit_msgbox", force){}

                
            }

            [EventName("tr_show_finish_msgbox")]
            public class EventTrShowFinishMsgbox : GameEvent
            {
                public EventTrShowFinishMsgbox() : base(){}
                public EventTrShowFinishMsgbox(bool force) : base("tr_show_finish_msgbox", force){}

                
            }

            [EventName("ugc_file_download_finished")]
            public class EventUgcFileDownloadFinished : GameEvent
            {
                public EventUgcFileDownloadFinished() : base(){}
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
                public EventUgcFileDownloadStart() : base(){}
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
                public EventUgcMapDownloadError() : base(){}
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
                public EventUgcMapInfoReceived() : base(){}
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
                public EventUgcMapUnsubscribed() : base(){}
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
                public EventUpdateMatchmakingStats() : base(){}
                public EventUpdateMatchmakingStats(bool force) : base("update_matchmaking_stats", force){}

                
            }

            [EventName("user_data_downloaded")]
            public class EventUserDataDownloaded : GameEvent
            {
                public EventUserDataDownloaded() : base(){}
                public EventUserDataDownloaded(bool force) : base("user_data_downloaded", force){}

                
            }

            [EventName("vip_escaped")]
            public class EventVipEscaped : GameEvent
            {
                public EventVipEscaped() : base(){}
                public EventVipEscaped(bool force) : base("vip_escaped", force){}

                
                
                // player who was the VIP
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("vip_killed")]
            public class EventVipKilled : GameEvent
            {
                public EventVipKilled() : base(){}
                public EventVipKilled(bool force) : base("vip_killed", force){}

                
                
                // player who was the VIP
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }

                
                // user ID who killed the VIP
                public int Attacker 
                {
                    get => Get<int>("attacker");
                    set => Set<int>("attacker", value);
                }
            }

            [EventName("vote_cast")]
            public class EventVoteCast : GameEvent
            {
                public EventVoteCast() : base(){}
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
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("vote_cast_no")]
            public class EventVoteCastNo : GameEvent
            {
                public EventVoteCastNo() : base(){}
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
                public EventVoteCastYes() : base(){}
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
                public EventVoteChanged() : base(){}
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
                public EventVoteEnded() : base(){}
                public EventVoteEnded(bool force) : base("vote_ended", force){}

                
            }

            [EventName("vote_failed")]
            public class EventVoteFailed : GameEvent
            {
                public EventVoteFailed() : base(){}
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
                public EventVoteOptions() : base(){}
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
                public EventVotePassed() : base(){}
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
                public EventVoteStarted() : base(){}
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

            [EventName("weapon_fire")]
            public class EventWeaponFire : GameEvent
            {
                public EventWeaponFire() : base(){}
                public EventWeaponFire(bool force) : base("weapon_fire", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventWeaponFireOnEmpty() : base(){}
                public EventWeaponFireOnEmpty(bool force) : base("weapon_fire_on_empty", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventWeaponhudSelection() : base(){}
                public EventWeaponhudSelection(bool force) : base("weaponhud_selection", force){}

                
                
                // Player who this event applies to
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
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
                public EventWeaponOutofammo() : base(){}
                public EventWeaponOutofammo(bool force) : base("weapon_outofammo", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("weapon_reload")]
            public class EventWeaponReload : GameEvent
            {
                public EventWeaponReload() : base(){}
                public EventWeaponReload(bool force) : base("weapon_reload", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("weapon_zoom")]
            public class EventWeaponZoom : GameEvent
            {
                public EventWeaponZoom() : base(){}
                public EventWeaponZoom(bool force) : base("weapon_zoom", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("weapon_zoom_rifle")]
            public class EventWeaponZoomRifle : GameEvent
            {
                public EventWeaponZoomRifle() : base(){}
                public EventWeaponZoomRifle(bool force) : base("weapon_zoom_rifle", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("write_game_titledata")]
            public class EventWriteGameTitledata : GameEvent
            {
                public EventWriteGameTitledata() : base(){}
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
                public EventWriteProfileData() : base(){}
                public EventWriteProfileData(bool force) : base("write_profile_data", force){}

                
            }
}
