
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core
{
    
            [EventName("achievement_earned")]
            public class AchievementEarned : GameEvent
            {
                public AchievementEarned() : base(){}
                public AchievementEarned(bool force) : base("achievement_earned", force){}

                
                
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
            public class AchievementEarnedLocal : GameEvent
            {
                public AchievementEarnedLocal() : base(){}
                public AchievementEarnedLocal(bool force) : base("achievement_earned_local", force){}

                
                
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
            public class AchievementEvent : GameEvent
            {
                public AchievementEvent() : base(){}
                public AchievementEvent(bool force) : base("achievement_event", force){}

                
                
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
            public class AchievementInfoLoaded : GameEvent
            {
                public AchievementInfoLoaded() : base(){}
                public AchievementInfoLoaded(bool force) : base("achievement_info_loaded", force){}

                
            }

            [EventName("achievement_write_failed")]
            public class AchievementWriteFailed : GameEvent
            {
                public AchievementWriteFailed() : base(){}
                public AchievementWriteFailed(bool force) : base("achievement_write_failed", force){}

                
            }

            [EventName("add_bullet_hit_marker")]
            public class AddBulletHitMarker : GameEvent
            {
                public AddBulletHitMarker() : base(){}
                public AddBulletHitMarker(bool force) : base("add_bullet_hit_marker", force){}

                
                
                
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
            public class AddPlayerSonarIcon : GameEvent
            {
                public AddPlayerSonarIcon() : base(){}
                public AddPlayerSonarIcon(bool force) : base("add_player_sonar_icon", force){}

                
                
                
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
            public class AmmoPickup : GameEvent
            {
                public AmmoPickup() : base(){}
                public AmmoPickup(bool force) : base("ammo_pickup", force){}

                
                
                
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
            public class AmmoRefill : GameEvent
            {
                public AmmoRefill() : base(){}
                public AmmoRefill(bool force) : base("ammo_refill", force){}

                
                
                
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
            public class AnnouncePhaseEnd : GameEvent
            {
                public AnnouncePhaseEnd() : base(){}
                public AnnouncePhaseEnd(bool force) : base("announce_phase_end", force){}

                
            }

            [EventName("begin_new_match")]
            public class BeginNewMatch : GameEvent
            {
                public BeginNewMatch() : base(){}
                public BeginNewMatch(bool force) : base("begin_new_match", force){}

                
            }

            [EventName("bomb_abortdefuse")]
            public class BombAbortdefuse : GameEvent
            {
                public BombAbortdefuse() : base(){}
                public BombAbortdefuse(bool force) : base("bomb_abortdefuse", force){}

                
                
                // player who was defusing
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("bomb_abortplant")]
            public class BombAbortplant : GameEvent
            {
                public BombAbortplant() : base(){}
                public BombAbortplant(bool force) : base("bomb_abortplant", force){}

                
                
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
            public class BombBeep : GameEvent
            {
                public BombBeep() : base(){}
                public BombBeep(bool force) : base("bomb_beep", force){}

                
                
                // c4 entity
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("bomb_begindefuse")]
            public class BombBegindefuse : GameEvent
            {
                public BombBegindefuse() : base(){}
                public BombBegindefuse(bool force) : base("bomb_begindefuse", force){}

                
                
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
            public class BombBeginplant : GameEvent
            {
                public BombBeginplant() : base(){}
                public BombBeginplant(bool force) : base("bomb_beginplant", force){}

                
                
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
            public class BombDefused : GameEvent
            {
                public BombDefused() : base(){}
                public BombDefused(bool force) : base("bomb_defused", force){}

                
                
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
            public class BombDropped : GameEvent
            {
                public BombDropped() : base(){}
                public BombDropped(bool force) : base("bomb_dropped", force){}

                
                
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
            public class BombExploded : GameEvent
            {
                public BombExploded() : base(){}
                public BombExploded(bool force) : base("bomb_exploded", force){}

                
                
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
            public class BombPickup : GameEvent
            {
                public BombPickup() : base(){}
                public BombPickup(bool force) : base("bomb_pickup", force){}

                
                
                // player pawn who picked up the bomb
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("bomb_planted")]
            public class BombPlanted : GameEvent
            {
                public BombPlanted() : base(){}
                public BombPlanted(bool force) : base("bomb_planted", force){}

                
                
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
            public class BonusUpdated : GameEvent
            {
                public BonusUpdated() : base(){}
                public BonusUpdated(bool force) : base("bonus_updated", force){}

                
                
                
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
            public class BotTakeover : GameEvent
            {
                public BotTakeover() : base(){}
                public BotTakeover(bool force) : base("bot_takeover", force){}

                
                
                
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
            public class BreakBreakable : GameEvent
            {
                public BreakBreakable() : base(){}
                public BreakBreakable(bool force) : base("break_breakable", force){}

                
                
                
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
            public class BreakProp : GameEvent
            {
                public BreakProp() : base(){}
                public BreakProp(bool force) : base("break_prop", force){}

                
                
                
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
            public class BrokenBreakable : GameEvent
            {
                public BrokenBreakable() : base(){}
                public BrokenBreakable(bool force) : base("broken_breakable", force){}

                
                
                
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
            public class BulletFlightResolution : GameEvent
            {
                public BulletFlightResolution() : base(){}
                public BulletFlightResolution(bool force) : base("bullet_flight_resolution", force){}

                
                
                
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
            public class BulletImpact : GameEvent
            {
                public BulletImpact() : base(){}
                public BulletImpact(bool force) : base("bullet_impact", force){}

                
                
                
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
            public class BuymenuClose : GameEvent
            {
                public BuymenuClose() : base(){}
                public BuymenuClose(bool force) : base("buymenu_close", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("buymenu_open")]
            public class BuymenuOpen : GameEvent
            {
                public BuymenuOpen() : base(){}
                public BuymenuOpen(bool force) : base("buymenu_open", force){}

                
            }

            [EventName("buytime_ended")]
            public class BuytimeEnded : GameEvent
            {
                public BuytimeEnded() : base(){}
                public BuytimeEnded(bool force) : base("buytime_ended", force){}

                
            }

            [EventName("cart_updated")]
            public class CartUpdated : GameEvent
            {
                public CartUpdated() : base(){}
                public CartUpdated(bool force) : base("cart_updated", force){}

                
            }

            [EventName("choppers_incoming_warning")]
            public class ChoppersIncomingWarning : GameEvent
            {
                public ChoppersIncomingWarning() : base(){}
                public ChoppersIncomingWarning(bool force) : base("choppers_incoming_warning", force){}

                
                
                
                public bool Global 
                {
                    get => Get<bool>("global");
                    set => Set<bool>("global", value);
                }
            }

            [EventName("client_disconnect")]
            public class ClientDisconnect : GameEvent
            {
                public ClientDisconnect() : base(){}
                public ClientDisconnect(bool force) : base("client_disconnect", force){}

                
            }

            [EventName("client_loadout_changed")]
            public class ClientLoadoutChanged : GameEvent
            {
                public ClientLoadoutChanged() : base(){}
                public ClientLoadoutChanged(bool force) : base("client_loadout_changed", force){}

                
            }

            [EventName("clientside_lesson_closed")]
            public class ClientsideLessonClosed : GameEvent
            {
                public ClientsideLessonClosed() : base(){}
                public ClientsideLessonClosed(bool force) : base("clientside_lesson_closed", force){}

                
                
                
                public string LessonName 
                {
                    get => Get<string>("lesson_name");
                    set => Set<string>("lesson_name", value);
                }
            }

            [EventName("cs_game_disconnected")]
            public class CsGameDisconnected : GameEvent
            {
                public CsGameDisconnected() : base(){}
                public CsGameDisconnected(bool force) : base("cs_game_disconnected", force){}

                
            }

            [EventName("cs_intermission")]
            public class CsIntermission : GameEvent
            {
                public CsIntermission() : base(){}
                public CsIntermission(bool force) : base("cs_intermission", force){}

                
            }

            [EventName("cs_match_end_restart")]
            public class CsMatchEndRestart : GameEvent
            {
                public CsMatchEndRestart() : base(){}
                public CsMatchEndRestart(bool force) : base("cs_match_end_restart", force){}

                
            }

            [EventName("cs_pre_restart")]
            public class CsPreRestart : GameEvent
            {
                public CsPreRestart() : base(){}
                public CsPreRestart(bool force) : base("cs_pre_restart", force){}

                
            }

            [EventName("cs_prev_next_spectator")]
            public class CsPrevNextSpectator : GameEvent
            {
                public CsPrevNextSpectator() : base(){}
                public CsPrevNextSpectator(bool force) : base("cs_prev_next_spectator", force){}

                
                
                
                public bool Next 
                {
                    get => Get<bool>("next");
                    set => Set<bool>("next", value);
                }
            }

            [EventName("cs_round_final_beep")]
            public class CsRoundFinalBeep : GameEvent
            {
                public CsRoundFinalBeep() : base(){}
                public CsRoundFinalBeep(bool force) : base("cs_round_final_beep", force){}

                
            }

            [EventName("cs_round_start_beep")]
            public class CsRoundStartBeep : GameEvent
            {
                public CsRoundStartBeep() : base(){}
                public CsRoundStartBeep(bool force) : base("cs_round_start_beep", force){}

                
            }

            [EventName("cs_win_panel_match")]
            public class CsWinPanelMatch : GameEvent
            {
                public CsWinPanelMatch() : base(){}
                public CsWinPanelMatch(bool force) : base("cs_win_panel_match", force){}

                
            }

            [EventName("cs_win_panel_round")]
            public class CsWinPanelRound : GameEvent
            {
                public CsWinPanelRound() : base(){}
                public CsWinPanelRound(bool force) : base("cs_win_panel_round", force){}

                
                
                
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
            public class DecoyDetonate : GameEvent
            {
                public DecoyDetonate() : base(){}
                public DecoyDetonate(bool force) : base("decoy_detonate", force){}

                
                
                
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
            public class DecoyFiring : GameEvent
            {
                public DecoyFiring() : base(){}
                public DecoyFiring(bool force) : base("decoy_firing", force){}

                
                
                
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
            public class DecoyStarted : GameEvent
            {
                public DecoyStarted() : base(){}
                public DecoyStarted(bool force) : base("decoy_started", force){}

                
                
                
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
            public class DefuserDropped : GameEvent
            {
                public DefuserDropped() : base(){}
                public DefuserDropped(bool force) : base("defuser_dropped", force){}

                
                
                // defuser's entity ID
                public long Entityid 
                {
                    get => Get<long>("entityid");
                    set => Set<long>("entityid", value);
                }
            }

            [EventName("defuser_pickup")]
            public class DefuserPickup : GameEvent
            {
                public DefuserPickup() : base(){}
                public DefuserPickup(bool force) : base("defuser_pickup", force){}

                
                
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
            public class DemoSkip : GameEvent
            {
                public DemoSkip() : base(){}
                public DemoSkip(bool force) : base("demo_skip", force){}

                
                
                
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
            public class DemoStart : GameEvent
            {
                public DemoStart() : base(){}
                public DemoStart(bool force) : base("demo_start", force){}

                
                
                
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
            public class DemoStop : GameEvent
            {
                public DemoStop() : base(){}
                public DemoStop(bool force) : base("demo_stop", force){}

                
            }

            [EventName("difficulty_changed")]
            public class DifficultyChanged : GameEvent
            {
                public DifficultyChanged() : base(){}
                public DifficultyChanged(bool force) : base("difficulty_changed", force){}

                
                
                
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
            public class DmBonusWeaponStart : GameEvent
            {
                public DmBonusWeaponStart() : base(){}
                public DmBonusWeaponStart(bool force) : base("dm_bonus_weapon_start", force){}

                
                
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
            public class DoorBreak : GameEvent
            {
                public DoorBreak() : base(){}
                public DoorBreak(bool force) : base("door_break", force){}

                
                
                
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
            public class DoorClose : GameEvent
            {
                public DoorClose() : base(){}
                public DoorClose(bool force) : base("door_close", force){}

                
                
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
            public class DoorClosed : GameEvent
            {
                public DoorClosed() : base(){}
                public DoorClosed(bool force) : base("door_closed", force){}

                
                
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
            public class DoorMoving : GameEvent
            {
                public DoorMoving() : base(){}
                public DoorMoving(bool force) : base("door_moving", force){}

                
                
                
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
            public class DoorOpen : GameEvent
            {
                public DoorOpen() : base(){}
                public DoorOpen(bool force) : base("door_open", force){}

                
                
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
            public class DroneAboveRoof : GameEvent
            {
                public DroneAboveRoof() : base(){}
                public DroneAboveRoof(bool force) : base("drone_above_roof", force){}

                
                
                
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
            public class DroneCargoDetached : GameEvent
            {
                public DroneCargoDetached() : base(){}
                public DroneCargoDetached(bool force) : base("drone_cargo_detached", force){}

                
                
                
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
            public class DroneDispatched : GameEvent
            {
                public DroneDispatched() : base(){}
                public DroneDispatched(bool force) : base("drone_dispatched", force){}

                
                
                
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
            public class DronegunAttack : GameEvent
            {
                public DronegunAttack() : base(){}
                public DronegunAttack(bool force) : base("dronegun_attack", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("drop_rate_modified")]
            public class DropRateModified : GameEvent
            {
                public DropRateModified() : base(){}
                public DropRateModified(bool force) : base("drop_rate_modified", force){}

                
            }

            [EventName("dynamic_shadow_light_changed")]
            public class DynamicShadowLightChanged : GameEvent
            {
                public DynamicShadowLightChanged() : base(){}
                public DynamicShadowLightChanged(bool force) : base("dynamic_shadow_light_changed", force){}

                
            }

            [EventName("dz_item_interaction")]
            public class DzItemInteraction : GameEvent
            {
                public DzItemInteraction() : base(){}
                public DzItemInteraction(bool force) : base("dz_item_interaction", force){}

                
                
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
            public class EnableRestartVoting : GameEvent
            {
                public EnableRestartVoting() : base(){}
                public EnableRestartVoting(bool force) : base("enable_restart_voting", force){}

                
                
                
                public bool Enable 
                {
                    get => Get<bool>("enable");
                    set => Set<bool>("enable", value);
                }
            }

            [EventName("endmatch_cmm_start_reveal_items")]
            public class EndmatchCmmStartRevealItems : GameEvent
            {
                public EndmatchCmmStartRevealItems() : base(){}
                public EndmatchCmmStartRevealItems(bool force) : base("endmatch_cmm_start_reveal_items", force){}

                
            }

            [EventName("endmatch_mapvote_selecting_map")]
            public class EndmatchMapvoteSelectingMap : GameEvent
            {
                public EndmatchMapvoteSelectingMap() : base(){}
                public EndmatchMapvoteSelectingMap(bool force) : base("endmatch_mapvote_selecting_map", force){}

                
                
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
            public class EnterBombzone : GameEvent
            {
                public EnterBombzone() : base(){}
                public EnterBombzone(bool force) : base("enter_bombzone", force){}

                
                
                
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
            public class EnterBuyzone : GameEvent
            {
                public EnterBuyzone() : base(){}
                public EnterBuyzone(bool force) : base("enter_buyzone", force){}

                
                
                
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
            public class EnterRescueZone : GameEvent
            {
                public EnterRescueZone() : base(){}
                public EnterRescueZone(bool force) : base("enter_rescue_zone", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("entity_killed")]
            public class EntityKilled : GameEvent
            {
                public EntityKilled() : base(){}
                public EntityKilled(bool force) : base("entity_killed", force){}

                
                
                
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
            public class EntityVisible : GameEvent
            {
                public EntityVisible() : base(){}
                public EntityVisible(bool force) : base("entity_visible", force){}

                
                
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
            public class EventTicketModified : GameEvent
            {
                public EventTicketModified() : base(){}
                public EventTicketModified(bool force) : base("event_ticket_modified", force){}

                
            }

            [EventName("exit_bombzone")]
            public class ExitBombzone : GameEvent
            {
                public ExitBombzone() : base(){}
                public ExitBombzone(bool force) : base("exit_bombzone", force){}

                
                
                
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
            public class ExitBuyzone : GameEvent
            {
                public ExitBuyzone() : base(){}
                public ExitBuyzone(bool force) : base("exit_buyzone", force){}

                
                
                
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
            public class ExitRescueZone : GameEvent
            {
                public ExitRescueZone() : base(){}
                public ExitRescueZone(bool force) : base("exit_rescue_zone", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("finale_start")]
            public class FinaleStart : GameEvent
            {
                public FinaleStart() : base(){}
                public FinaleStart(bool force) : base("finale_start", force){}

                
                
                
                public int Rushes 
                {
                    get => Get<int>("rushes");
                    set => Set<int>("rushes", value);
                }
            }

            [EventName("firstbombs_incoming_warning")]
            public class FirstbombsIncomingWarning : GameEvent
            {
                public FirstbombsIncomingWarning() : base(){}
                public FirstbombsIncomingWarning(bool force) : base("firstbombs_incoming_warning", force){}

                
                
                
                public bool Global 
                {
                    get => Get<bool>("global");
                    set => Set<bool>("global", value);
                }
            }

            [EventName("flare_ignite_npc")]
            public class FlareIgniteNpc : GameEvent
            {
                public FlareIgniteNpc() : base(){}
                public FlareIgniteNpc(bool force) : base("flare_ignite_npc", force){}

                
                
                // entity ignited
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("flashbang_detonate")]
            public class FlashbangDetonate : GameEvent
            {
                public FlashbangDetonate() : base(){}
                public FlashbangDetonate(bool force) : base("flashbang_detonate", force){}

                
                
                
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
            public class GameEnd : GameEvent
            {
                public GameEnd() : base(){}
                public GameEnd(bool force) : base("game_end", force){}

                
                
                // winner team/user id
                public int Winner 
                {
                    get => Get<int>("winner");
                    set => Set<int>("winner", value);
                }
            }

            [EventName("game_init")]
            public class GameInit : GameEvent
            {
                public GameInit() : base(){}
                public GameInit(bool force) : base("game_init", force){}

                
            }

            [EventName("gameinstructor_draw")]
            public class GameinstructorDraw : GameEvent
            {
                public GameinstructorDraw() : base(){}
                public GameinstructorDraw(bool force) : base("gameinstructor_draw", force){}

                
            }

            [EventName("gameinstructor_nodraw")]
            public class GameinstructorNodraw : GameEvent
            {
                public GameinstructorNodraw() : base(){}
                public GameinstructorNodraw(bool force) : base("gameinstructor_nodraw", force){}

                
            }

            [EventName("game_message")]
            public class GameMessage : GameEvent
            {
                public GameMessage() : base(){}
                public GameMessage(bool force) : base("game_message", force){}

                
                
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
            public class GameNewmap : GameEvent
            {
                public GameNewmap() : base(){}
                public GameNewmap(bool force) : base("game_newmap", force){}

                
                
                // map name
                public string Mapname 
                {
                    get => Get<string>("mapname");
                    set => Set<string>("mapname", value);
                }
            }

            [EventName("game_phase_changed")]
            public class GamePhaseChanged : GameEvent
            {
                public GamePhaseChanged() : base(){}
                public GamePhaseChanged(bool force) : base("game_phase_changed", force){}

                
                
                
                public int NewPhase 
                {
                    get => Get<int>("new_phase");
                    set => Set<int>("new_phase", value);
                }
            }

            [EventName("game_start")]
            public class GameStart : GameEvent
            {
                public GameStart() : base(){}
                public GameStart(bool force) : base("game_start", force){}

                
                
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
            public class GameuiHidden : GameEvent
            {
                public GameuiHidden() : base(){}
                public GameuiHidden(bool force) : base("gameui_hidden", force){}

                
            }

            [EventName("gc_connected")]
            public class GcConnected : GameEvent
            {
                public GcConnected() : base(){}
                public GcConnected(bool force) : base("gc_connected", force){}

                
            }

            [EventName("gg_killed_enemy")]
            public class GgKilledEnemy : GameEvent
            {
                public GgKilledEnemy() : base(){}
                public GgKilledEnemy(bool force) : base("gg_killed_enemy", force){}

                
                
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
            public class GrenadeBounce : GameEvent
            {
                public GrenadeBounce() : base(){}
                public GrenadeBounce(bool force) : base("grenade_bounce", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("grenade_thrown")]
            public class GrenadeThrown : GameEvent
            {
                public GrenadeThrown() : base(){}
                public GrenadeThrown(bool force) : base("grenade_thrown", force){}

                
                
                
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
            public class GuardianWaveRestart : GameEvent
            {
                public GuardianWaveRestart() : base(){}
                public GuardianWaveRestart(bool force) : base("guardian_wave_restart", force){}

                
            }

            [EventName("hegrenade_detonate")]
            public class HegrenadeDetonate : GameEvent
            {
                public HegrenadeDetonate() : base(){}
                public HegrenadeDetonate(bool force) : base("hegrenade_detonate", force){}

                
                
                
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
            public class HelicopterGrenadePuntMiss : GameEvent
            {
                public HelicopterGrenadePuntMiss() : base(){}
                public HelicopterGrenadePuntMiss(bool force) : base("helicopter_grenade_punt_miss", force){}

                
            }

            [EventName("hide_deathpanel")]
            public class HideDeathpanel : GameEvent
            {
                public HideDeathpanel() : base(){}
                public HideDeathpanel(bool force) : base("hide_deathpanel", force){}

                
            }

            [EventName("hltv_cameraman")]
            public class HltvCameraman : GameEvent
            {
                public HltvCameraman() : base(){}
                public HltvCameraman(bool force) : base("hltv_cameraman", force){}

                
                
                // camera man entity index
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("hltv_changed_mode")]
            public class HltvChangedMode : GameEvent
            {
                public HltvChangedMode() : base(){}
                public HltvChangedMode(bool force) : base("hltv_changed_mode", force){}

                
                
                
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
            public class HltvChase : GameEvent
            {
                public HltvChase() : base(){}
                public HltvChase(bool force) : base("hltv_chase", force){}

                
                
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
            public class HltvChat : GameEvent
            {
                public HltvChat() : base(){}
                public HltvChat(bool force) : base("hltv_chat", force){}

                
                
                
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
            public class HltvFixed : GameEvent
            {
                public HltvFixed() : base(){}
                public HltvFixed(bool force) : base("hltv_fixed", force){}

                
                
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
            public class HltvMessage : GameEvent
            {
                public HltvMessage() : base(){}
                public HltvMessage(bool force) : base("hltv_message", force){}

                
                
                
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }
            }

            [EventName("hltv_rank_camera")]
            public class HltvRankCamera : GameEvent
            {
                public HltvRankCamera() : base(){}
                public HltvRankCamera(bool force) : base("hltv_rank_camera", force){}

                
                
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
            public class HltvRankEntity : GameEvent
            {
                public HltvRankEntity() : base(){}
                public HltvRankEntity(bool force) : base("hltv_rank_entity", force){}

                
                
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
            public class HltvReplay : GameEvent
            {
                public HltvReplay() : base(){}
                public HltvReplay(bool force) : base("hltv_replay", force){}

                
                
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
            public class HltvReplayStatus : GameEvent
            {
                public HltvReplayStatus() : base(){}
                public HltvReplayStatus(bool force) : base("hltv_replay_status", force){}

                
                
                // reason for hltv replay status change ()
                public long Reason 
                {
                    get => Get<long>("reason");
                    set => Set<long>("reason", value);
                }
            }

            [EventName("hltv_status")]
            public class HltvStatus : GameEvent
            {
                public HltvStatus() : base(){}
                public HltvStatus(bool force) : base("hltv_status", force){}

                
                
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
            public class HltvTitle : GameEvent
            {
                public HltvTitle() : base(){}
                public HltvTitle(bool force) : base("hltv_title", force){}

                
                
                
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }
            }

            [EventName("hltv_versioninfo")]
            public class HltvVersioninfo : GameEvent
            {
                public HltvVersioninfo() : base(){}
                public HltvVersioninfo(bool force) : base("hltv_versioninfo", force){}

                
                
                
                public long Version 
                {
                    get => Get<long>("version");
                    set => Set<long>("version", value);
                }
            }

            [EventName("hostage_call_for_help")]
            public class HostageCallForHelp : GameEvent
            {
                public HostageCallForHelp() : base(){}
                public HostageCallForHelp(bool force) : base("hostage_call_for_help", force){}

                
                
                // hostage entity index
                public int Hostage 
                {
                    get => Get<int>("hostage");
                    set => Set<int>("hostage", value);
                }
            }

            [EventName("hostage_follows")]
            public class HostageFollows : GameEvent
            {
                public HostageFollows() : base(){}
                public HostageFollows(bool force) : base("hostage_follows", force){}

                
                
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
            public class HostageHurt : GameEvent
            {
                public HostageHurt() : base(){}
                public HostageHurt(bool force) : base("hostage_hurt", force){}

                
                
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
            public class HostageKilled : GameEvent
            {
                public HostageKilled() : base(){}
                public HostageKilled(bool force) : base("hostage_killed", force){}

                
                
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
            public class HostageRescued : GameEvent
            {
                public HostageRescued() : base(){}
                public HostageRescued(bool force) : base("hostage_rescued", force){}

                
                
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
            public class HostageRescuedAll : GameEvent
            {
                public HostageRescuedAll() : base(){}
                public HostageRescuedAll(bool force) : base("hostage_rescued_all", force){}

                
            }

            [EventName("hostage_stops_following")]
            public class HostageStopsFollowing : GameEvent
            {
                public HostageStopsFollowing() : base(){}
                public HostageStopsFollowing(bool force) : base("hostage_stops_following", force){}

                
                
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
            public class HostnameChanged : GameEvent
            {
                public HostnameChanged() : base(){}
                public HostnameChanged(bool force) : base("hostname_changed", force){}

                
                
                
                public string Hostname 
                {
                    get => Get<string>("hostname");
                    set => Set<string>("hostname", value);
                }
            }

            [EventName("inferno_expire")]
            public class InfernoExpire : GameEvent
            {
                public InfernoExpire() : base(){}
                public InfernoExpire(bool force) : base("inferno_expire", force){}

                
                
                
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
            public class InfernoExtinguish : GameEvent
            {
                public InfernoExtinguish() : base(){}
                public InfernoExtinguish(bool force) : base("inferno_extinguish", force){}

                
                
                
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
            public class InfernoStartburn : GameEvent
            {
                public InfernoStartburn() : base(){}
                public InfernoStartburn(bool force) : base("inferno_startburn", force){}

                
                
                
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
            public class InspectWeapon : GameEvent
            {
                public InspectWeapon() : base(){}
                public InspectWeapon(bool force) : base("inspect_weapon", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("instructor_close_lesson")]
            public class InstructorCloseLesson : GameEvent
            {
                public InstructorCloseLesson() : base(){}
                public InstructorCloseLesson(bool force) : base("instructor_close_lesson", force){}

                
                
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
            public class InstructorServerHintCreate : GameEvent
            {
                public InstructorServerHintCreate() : base(){}
                public InstructorServerHintCreate(bool force) : base("instructor_server_hint_create", force){}

                
                
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
            public class InstructorServerHintStop : GameEvent
            {
                public InstructorServerHintStop() : base(){}
                public InstructorServerHintStop(bool force) : base("instructor_server_hint_stop", force){}

                
                
                // The hint to stop. Will stop ALL hints with this name
                public string HintName 
                {
                    get => Get<string>("hint_name");
                    set => Set<string>("hint_name", value);
                }
            }

            [EventName("instructor_start_lesson")]
            public class InstructorStartLesson : GameEvent
            {
                public InstructorStartLesson() : base(){}
                public InstructorStartLesson(bool force) : base("instructor_start_lesson", force){}

                
                
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
            public class InventoryUpdated : GameEvent
            {
                public InventoryUpdated() : base(){}
                public InventoryUpdated(bool force) : base("inventory_updated", force){}

                
            }

            [EventName("item_equip")]
            public class ItemEquip : GameEvent
            {
                public ItemEquip() : base(){}
                public ItemEquip(bool force) : base("item_equip", force){}

                
                
                
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
            public class ItemPickup : GameEvent
            {
                public ItemPickup() : base(){}
                public ItemPickup(bool force) : base("item_pickup", force){}

                
                
                
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
            public class ItemPickupFailed : GameEvent
            {
                public ItemPickupFailed() : base(){}
                public ItemPickupFailed(bool force) : base("item_pickup_failed", force){}

                
                
                
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
            public class ItemPickupSlerp : GameEvent
            {
                public ItemPickupSlerp() : base(){}
                public ItemPickupSlerp(bool force) : base("item_pickup_slerp", force){}

                
                
                
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
            public class ItemPurchase : GameEvent
            {
                public ItemPurchase() : base(){}
                public ItemPurchase(bool force) : base("item_purchase", force){}

                
                
                
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
            public class ItemRemove : GameEvent
            {
                public ItemRemove() : base(){}
                public ItemRemove(bool force) : base("item_remove", force){}

                
                
                
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
            public class ItemSchemaInitialized : GameEvent
            {
                public ItemSchemaInitialized() : base(){}
                public ItemSchemaInitialized(bool force) : base("item_schema_initialized", force){}

                
            }

            [EventName("items_gifted")]
            public class ItemsGifted : GameEvent
            {
                public ItemsGifted() : base(){}
                public ItemsGifted(bool force) : base("items_gifted", force){}

                
                
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
            public class JointeamFailed : GameEvent
            {
                public JointeamFailed() : base(){}
                public JointeamFailed(bool force) : base("jointeam_failed", force){}

                
                
                
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
            public class LocalPlayerControllerTeam : GameEvent
            {
                public LocalPlayerControllerTeam() : base(){}
                public LocalPlayerControllerTeam(bool force) : base("local_player_controller_team", force){}

                
            }

            [EventName("local_player_pawn_changed")]
            public class LocalPlayerPawnChanged : GameEvent
            {
                public LocalPlayerPawnChanged() : base(){}
                public LocalPlayerPawnChanged(bool force) : base("local_player_pawn_changed", force){}

                
            }

            [EventName("local_player_team")]
            public class LocalPlayerTeam : GameEvent
            {
                public LocalPlayerTeam() : base(){}
                public LocalPlayerTeam(bool force) : base("local_player_team", force){}

                
            }

            [EventName("loot_crate_opened")]
            public class LootCrateOpened : GameEvent
            {
                public LootCrateOpened() : base(){}
                public LootCrateOpened(bool force) : base("loot_crate_opened", force){}

                
                
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
            public class LootCrateVisible : GameEvent
            {
                public LootCrateVisible() : base(){}
                public LootCrateVisible(bool force) : base("loot_crate_visible", force){}

                
                
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
            public class MapShutdown : GameEvent
            {
                public MapShutdown() : base(){}
                public MapShutdown(bool force) : base("map_shutdown", force){}

                
            }

            [EventName("map_transition")]
            public class MapTransition : GameEvent
            {
                public MapTransition() : base(){}
                public MapTransition(bool force) : base("map_transition", force){}

                
            }

            [EventName("match_end_conditions")]
            public class MatchEndConditions : GameEvent
            {
                public MatchEndConditions() : base(){}
                public MatchEndConditions(bool force) : base("match_end_conditions", force){}

                
                
                
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
            public class MaterialDefaultComplete : GameEvent
            {
                public MaterialDefaultComplete() : base(){}
                public MaterialDefaultComplete(bool force) : base("material_default_complete", force){}

                
            }

            [EventName("mb_input_lock_cancel")]
            public class MbInputLockCancel : GameEvent
            {
                public MbInputLockCancel() : base(){}
                public MbInputLockCancel(bool force) : base("mb_input_lock_cancel", force){}

                
            }

            [EventName("mb_input_lock_success")]
            public class MbInputLockSuccess : GameEvent
            {
                public MbInputLockSuccess() : base(){}
                public MbInputLockSuccess(bool force) : base("mb_input_lock_success", force){}

                
            }

            [EventName("molotov_detonate")]
            public class MolotovDetonate : GameEvent
            {
                public MolotovDetonate() : base(){}
                public MolotovDetonate(bool force) : base("molotov_detonate", force){}

                
                
                
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
            public class NavBlocked : GameEvent
            {
                public NavBlocked() : base(){}
                public NavBlocked(bool force) : base("nav_blocked", force){}

                
                
                
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
            public class NavGenerate : GameEvent
            {
                public NavGenerate() : base(){}
                public NavGenerate(bool force) : base("nav_generate", force){}

                
            }

            [EventName("nextlevel_changed")]
            public class NextlevelChanged : GameEvent
            {
                public NextlevelChanged() : base(){}
                public NextlevelChanged(bool force) : base("nextlevel_changed", force){}

                
                
                
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
            public class OpenCrateInstr : GameEvent
            {
                public OpenCrateInstr() : base(){}
                public OpenCrateInstr(bool force) : base("open_crate_instr", force){}

                
                
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
            public class OtherDeath : GameEvent
            {
                public OtherDeath() : base(){}
                public OtherDeath(bool force) : base("other_death", force){}

                
                
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
            public class ParachuteDeploy : GameEvent
            {
                public ParachuteDeploy() : base(){}
                public ParachuteDeploy(bool force) : base("parachute_deploy", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("parachute_pickup")]
            public class ParachutePickup : GameEvent
            {
                public ParachutePickup() : base(){}
                public ParachutePickup(bool force) : base("parachute_pickup", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("physgun_pickup")]
            public class PhysgunPickup : GameEvent
            {
                public PhysgunPickup() : base(){}
                public PhysgunPickup(bool force) : base("physgun_pickup", force){}

                
                
                // entity picked up
                public IntPtr Target 
                {
                    get => Get<IntPtr>("target");
                    set => Set<IntPtr>("target", value);
                }
            }

            [EventName("player_activate")]
            public class PlayerActivate : GameEvent
            {
                public PlayerActivate() : base(){}
                public PlayerActivate(bool force) : base("player_activate", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_avenged_teammate")]
            public class PlayerAvengedTeammate : GameEvent
            {
                public PlayerAvengedTeammate() : base(){}
                public PlayerAvengedTeammate(bool force) : base("player_avenged_teammate", force){}

                
                
                
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
            public class PlayerBlind : GameEvent
            {
                public PlayerBlind() : base(){}
                public PlayerBlind(bool force) : base("player_blind", force){}

                
                
                
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
            public class PlayerChangename : GameEvent
            {
                public PlayerChangename() : base(){}
                public PlayerChangename(bool force) : base("player_changename", force){}

                
                
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
            public class PlayerChat : GameEvent
            {
                public PlayerChat() : base(){}
                public PlayerChat(bool force) : base("player_chat", force){}

                
                
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
            public class PlayerConnect : GameEvent
            {
                public PlayerConnect() : base(){}
                public PlayerConnect(bool force) : base("player_connect", force){}

                
                
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
            public class PlayerConnectFull : GameEvent
            {
                public PlayerConnectFull() : base(){}
                public PlayerConnectFull(bool force) : base("player_connect_full", force){}

                
                
                // user ID on server (unique on server)
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_death")]
            public class PlayerDeath : GameEvent
            {
                public PlayerDeath() : base(){}
                public PlayerDeath(bool force) : base("player_death", force){}

                
                
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
            public class PlayerDecal : GameEvent
            {
                public PlayerDecal() : base(){}
                public PlayerDecal(bool force) : base("player_decal", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_disconnect")]
            public class PlayerDisconnect : GameEvent
            {
                public PlayerDisconnect() : base(){}
                public PlayerDisconnect(bool force) : base("player_disconnect", force){}

                
                
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
            public class PlayerFalldamage : GameEvent
            {
                public PlayerFalldamage() : base(){}
                public PlayerFalldamage(bool force) : base("player_falldamage", force){}

                
                
                
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
            public class PlayerFootstep : GameEvent
            {
                public PlayerFootstep() : base(){}
                public PlayerFootstep(bool force) : base("player_footstep", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_full_update")]
            public class PlayerFullUpdate : GameEvent
            {
                public PlayerFullUpdate() : base(){}
                public PlayerFullUpdate(bool force) : base("player_full_update", force){}

                
                
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
            public class PlayerGivenC4 : GameEvent
            {
                public PlayerGivenC4() : base(){}
                public PlayerGivenC4(bool force) : base("player_given_c4", force){}

                
                
                // user ID who received the c4
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_hintmessage")]
            public class PlayerHintmessage : GameEvent
            {
                public PlayerHintmessage() : base(){}
                public PlayerHintmessage(bool force) : base("player_hintmessage", force){}

                
                
                // localizable string of a hint
                public string Hintmessage 
                {
                    get => Get<string>("hintmessage");
                    set => Set<string>("hintmessage", value);
                }
            }

            [EventName("player_hurt")]
            public class PlayerHurt : GameEvent
            {
                public PlayerHurt() : base(){}
                public PlayerHurt(bool force) : base("player_hurt", force){}

                
                
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
            public class PlayerInfo : GameEvent
            {
                public PlayerInfo() : base(){}
                public PlayerInfo(bool force) : base("player_info", force){}

                
                
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
            public class PlayerJump : GameEvent
            {
                public PlayerJump() : base(){}
                public PlayerJump(bool force) : base("player_jump", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_ping")]
            public class PlayerPing : GameEvent
            {
                public PlayerPing() : base(){}
                public PlayerPing(bool force) : base("player_ping", force){}

                
                
                
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
            public class PlayerPingStop : GameEvent
            {
                public PlayerPingStop() : base(){}
                public PlayerPingStop(bool force) : base("player_ping_stop", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }
            }

            [EventName("player_radio")]
            public class PlayerRadio : GameEvent
            {
                public PlayerRadio() : base(){}
                public PlayerRadio(bool force) : base("player_radio", force){}

                
                
                
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
            public class PlayerResetVote : GameEvent
            {
                public PlayerResetVote() : base(){}
                public PlayerResetVote(bool force) : base("player_reset_vote", force){}

                
                
                
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
            public class PlayerScore : GameEvent
            {
                public PlayerScore() : base(){}
                public PlayerScore(bool force) : base("player_score", force){}

                
                
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
            public class PlayerShoot : GameEvent
            {
                public PlayerShoot() : base(){}
                public PlayerShoot(bool force) : base("player_shoot", force){}

                
                
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
            public class PlayerSound : GameEvent
            {
                public PlayerSound() : base(){}
                public PlayerSound(bool force) : base("player_sound", force){}

                
                
                
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
            public class PlayerSpawn : GameEvent
            {
                public PlayerSpawn() : base(){}
                public PlayerSpawn(bool force) : base("player_spawn", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("player_spawned")]
            public class PlayerSpawned : GameEvent
            {
                public PlayerSpawned() : base(){}
                public PlayerSpawned(bool force) : base("player_spawned", force){}

                
                
                
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
            public class PlayerStatsUpdated : GameEvent
            {
                public PlayerStatsUpdated() : base(){}
                public PlayerStatsUpdated(bool force) : base("player_stats_updated", force){}

                
                
                
                public bool Forceupload 
                {
                    get => Get<bool>("forceupload");
                    set => Set<bool>("forceupload", value);
                }
            }

            [EventName("player_team")]
            public class PlayerTeam : GameEvent
            {
                public PlayerTeam() : base(){}
                public PlayerTeam(bool force) : base("player_team", force){}

                
                
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
            public class RagdollDissolved : GameEvent
            {
                public RagdollDissolved() : base(){}
                public RagdollDissolved(bool force) : base("ragdoll_dissolved", force){}

                
                
                
                public long Entindex 
                {
                    get => Get<long>("entindex");
                    set => Set<long>("entindex", value);
                }
            }

            [EventName("read_game_titledata")]
            public class ReadGameTitledata : GameEvent
            {
                public ReadGameTitledata() : base(){}
                public ReadGameTitledata(bool force) : base("read_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => Get<int>("controllerId");
                    set => Set<int>("controllerId", value);
                }
            }

            [EventName("repost_xbox_achievements")]
            public class RepostXboxAchievements : GameEvent
            {
                public RepostXboxAchievements() : base(){}
                public RepostXboxAchievements(bool force) : base("repost_xbox_achievements", force){}

                
                
                // splitscreen ID
                public int Splitscreenplayer 
                {
                    get => Get<int>("splitscreenplayer");
                    set => Set<int>("splitscreenplayer", value);
                }
            }

            [EventName("reset_game_titledata")]
            public class ResetGameTitledata : GameEvent
            {
                public ResetGameTitledata() : base(){}
                public ResetGameTitledata(bool force) : base("reset_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => Get<int>("controllerId");
                    set => Set<int>("controllerId", value);
                }
            }

            [EventName("round_announce_final")]
            public class RoundAnnounceFinal : GameEvent
            {
                public RoundAnnounceFinal() : base(){}
                public RoundAnnounceFinal(bool force) : base("round_announce_final", force){}

                
            }

            [EventName("round_announce_last_round_half")]
            public class RoundAnnounceLastRoundHalf : GameEvent
            {
                public RoundAnnounceLastRoundHalf() : base(){}
                public RoundAnnounceLastRoundHalf(bool force) : base("round_announce_last_round_half", force){}

                
            }

            [EventName("round_announce_match_point")]
            public class RoundAnnounceMatchPoint : GameEvent
            {
                public RoundAnnounceMatchPoint() : base(){}
                public RoundAnnounceMatchPoint(bool force) : base("round_announce_match_point", force){}

                
            }

            [EventName("round_announce_match_start")]
            public class RoundAnnounceMatchStart : GameEvent
            {
                public RoundAnnounceMatchStart() : base(){}
                public RoundAnnounceMatchStart(bool force) : base("round_announce_match_start", force){}

                
            }

            [EventName("round_announce_warmup")]
            public class RoundAnnounceWarmup : GameEvent
            {
                public RoundAnnounceWarmup() : base(){}
                public RoundAnnounceWarmup(bool force) : base("round_announce_warmup", force){}

                
            }

            [EventName("round_end")]
            public class RoundEnd : GameEvent
            {
                public RoundEnd() : base(){}
                public RoundEnd(bool force) : base("round_end", force){}

                
                
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
            public class RoundEndUploadStats : GameEvent
            {
                public RoundEndUploadStats() : base(){}
                public RoundEndUploadStats(bool force) : base("round_end_upload_stats", force){}

                
            }

            [EventName("round_freeze_end")]
            public class RoundFreezeEnd : GameEvent
            {
                public RoundFreezeEnd() : base(){}
                public RoundFreezeEnd(bool force) : base("round_freeze_end", force){}

                
            }

            [EventName("round_mvp")]
            public class RoundMvp : GameEvent
            {
                public RoundMvp() : base(){}
                public RoundMvp(bool force) : base("round_mvp", force){}

                
                
                
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
            public class RoundOfficiallyEnded : GameEvent
            {
                public RoundOfficiallyEnded() : base(){}
                public RoundOfficiallyEnded(bool force) : base("round_officially_ended", force){}

                
            }

            [EventName("round_poststart")]
            public class RoundPoststart : GameEvent
            {
                public RoundPoststart() : base(){}
                public RoundPoststart(bool force) : base("round_poststart", force){}

                
            }

            [EventName("round_prestart")]
            public class RoundPrestart : GameEvent
            {
                public RoundPrestart() : base(){}
                public RoundPrestart(bool force) : base("round_prestart", force){}

                
            }

            [EventName("round_start")]
            public class RoundStart : GameEvent
            {
                public RoundStart() : base(){}
                public RoundStart(bool force) : base("round_start", force){}

                
                
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
            public class RoundStartPostNav : GameEvent
            {
                public RoundStartPostNav() : base(){}
                public RoundStartPostNav(bool force) : base("round_start_post_nav", force){}

                
            }

            [EventName("round_start_pre_entity")]
            public class RoundStartPreEntity : GameEvent
            {
                public RoundStartPreEntity() : base(){}
                public RoundStartPreEntity(bool force) : base("round_start_pre_entity", force){}

                
            }

            [EventName("round_time_warning")]
            public class RoundTimeWarning : GameEvent
            {
                public RoundTimeWarning() : base(){}
                public RoundTimeWarning(bool force) : base("round_time_warning", force){}

                
            }

            [EventName("seasoncoin_levelup")]
            public class SeasoncoinLevelup : GameEvent
            {
                public SeasoncoinLevelup() : base(){}
                public SeasoncoinLevelup(bool force) : base("seasoncoin_levelup", force){}

                
                
                
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
            public class ServerCvar : GameEvent
            {
                public ServerCvar() : base(){}
                public ServerCvar(bool force) : base("server_cvar", force){}

                
                
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
            public class ServerMessage : GameEvent
            {
                public ServerMessage() : base(){}
                public ServerMessage(bool force) : base("server_message", force){}

                
                
                // the message text
                public string Text 
                {
                    get => Get<string>("text");
                    set => Set<string>("text", value);
                }
            }

            [EventName("server_pre_shutdown")]
            public class ServerPreShutdown : GameEvent
            {
                public ServerPreShutdown() : base(){}
                public ServerPreShutdown(bool force) : base("server_pre_shutdown", force){}

                
                
                // reason why server is about to be shut down
                public string Reason 
                {
                    get => Get<string>("reason");
                    set => Set<string>("reason", value);
                }
            }

            [EventName("server_shutdown")]
            public class ServerShutdown : GameEvent
            {
                public ServerShutdown() : base(){}
                public ServerShutdown(bool force) : base("server_shutdown", force){}

                
                
                // reason why server was shut down
                public string Reason 
                {
                    get => Get<string>("reason");
                    set => Set<string>("reason", value);
                }
            }

            [EventName("server_spawn")]
            public class ServerSpawn : GameEvent
            {
                public ServerSpawn() : base(){}
                public ServerSpawn(bool force) : base("server_spawn", force){}

                
                
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
            public class SetInstructorGroupEnabled : GameEvent
            {
                public SetInstructorGroupEnabled() : base(){}
                public SetInstructorGroupEnabled(bool force) : base("set_instructor_group_enabled", force){}

                
                
                
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
            public class Sfuievent : GameEvent
            {
                public Sfuievent() : base(){}
                public Sfuievent(bool force) : base("sfuievent", force){}

                
                
                
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
            public class ShowDeathpanel : GameEvent
            {
                public ShowDeathpanel() : base(){}
                public ShowDeathpanel(bool force) : base("show_deathpanel", force){}

                
                
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
            public class ShowSurvivalRespawnStatus : GameEvent
            {
                public ShowSurvivalRespawnStatus() : base(){}
                public ShowSurvivalRespawnStatus(bool force) : base("show_survival_respawn_status", force){}

                
                
                
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
            public class SilencerDetach : GameEvent
            {
                public SilencerDetach() : base(){}
                public SilencerDetach(bool force) : base("silencer_detach", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("silencer_off")]
            public class SilencerOff : GameEvent
            {
                public SilencerOff() : base(){}
                public SilencerOff(bool force) : base("silencer_off", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("silencer_on")]
            public class SilencerOn : GameEvent
            {
                public SilencerOn() : base(){}
                public SilencerOn(bool force) : base("silencer_on", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("smoke_beacon_paradrop")]
            public class SmokeBeaconParadrop : GameEvent
            {
                public SmokeBeaconParadrop() : base(){}
                public SmokeBeaconParadrop(bool force) : base("smoke_beacon_paradrop", force){}

                
                
                
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
            public class SmokegrenadeDetonate : GameEvent
            {
                public SmokegrenadeDetonate() : base(){}
                public SmokegrenadeDetonate(bool force) : base("smokegrenade_detonate", force){}

                
                
                
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
            public class SmokegrenadeExpired : GameEvent
            {
                public SmokegrenadeExpired() : base(){}
                public SmokegrenadeExpired(bool force) : base("smokegrenade_expired", force){}

                
                
                
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
            public class SpecModeUpdated : GameEvent
            {
                public SpecModeUpdated() : base(){}
                public SpecModeUpdated(bool force) : base("spec_mode_updated", force){}

                
                
                // entindex of the player
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("spec_target_updated")]
            public class SpecTargetUpdated : GameEvent
            {
                public SpecTargetUpdated() : base(){}
                public SpecTargetUpdated(bool force) : base("spec_target_updated", force){}

                
                
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
            public class StartHalftime : GameEvent
            {
                public StartHalftime() : base(){}
                public StartHalftime(bool force) : base("start_halftime", force){}

                
            }

            [EventName("start_vote")]
            public class StartVote : GameEvent
            {
                public StartVote() : base(){}
                public StartVote(bool force) : base("start_vote", force){}

                
                
                
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
            public class StorePricesheetUpdated : GameEvent
            {
                public StorePricesheetUpdated() : base(){}
                public StorePricesheetUpdated(bool force) : base("store_pricesheet_updated", force){}

                
            }

            [EventName("survival_announce_phase")]
            public class SurvivalAnnouncePhase : GameEvent
            {
                public SurvivalAnnouncePhase() : base(){}
                public SurvivalAnnouncePhase(bool force) : base("survival_announce_phase", force){}

                
                
                // The phase #
                public int Phase 
                {
                    get => Get<int>("phase");
                    set => Set<int>("phase", value);
                }
            }

            [EventName("survival_no_respawns_final")]
            public class SurvivalNoRespawnsFinal : GameEvent
            {
                public SurvivalNoRespawnsFinal() : base(){}
                public SurvivalNoRespawnsFinal(bool force) : base("survival_no_respawns_final", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("survival_no_respawns_warning")]
            public class SurvivalNoRespawnsWarning : GameEvent
            {
                public SurvivalNoRespawnsWarning() : base(){}
                public SurvivalNoRespawnsWarning(bool force) : base("survival_no_respawns_warning", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("survival_paradrop_break")]
            public class SurvivalParadropBreak : GameEvent
            {
                public SurvivalParadropBreak() : base(){}
                public SurvivalParadropBreak(bool force) : base("survival_paradrop_break", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }
            }

            [EventName("survival_paradrop_spawn")]
            public class SurvivalParadropSpawn : GameEvent
            {
                public SurvivalParadropSpawn() : base(){}
                public SurvivalParadropSpawn(bool force) : base("survival_paradrop_spawn", force){}

                
                
                
                public int Entityid 
                {
                    get => Get<int>("entityid");
                    set => Set<int>("entityid", value);
                }
            }

            [EventName("survival_teammate_respawn")]
            public class SurvivalTeammateRespawn : GameEvent
            {
                public SurvivalTeammateRespawn() : base(){}
                public SurvivalTeammateRespawn(bool force) : base("survival_teammate_respawn", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("switch_team")]
            public class SwitchTeam : GameEvent
            {
                public SwitchTeam() : base(){}
                public SwitchTeam(bool force) : base("switch_team", force){}

                
                
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
            public class TagrenadeDetonate : GameEvent
            {
                public TagrenadeDetonate() : base(){}
                public TagrenadeDetonate(bool force) : base("tagrenade_detonate", force){}

                
                
                
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
            public class TeamchangePending : GameEvent
            {
                public TeamchangePending() : base(){}
                public TeamchangePending(bool force) : base("teamchange_pending", force){}

                
                
                
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
            public class TeamInfo : GameEvent
            {
                public TeamInfo() : base(){}
                public TeamInfo(bool force) : base("team_info", force){}

                
                
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
            public class TeamIntroEnd : GameEvent
            {
                public TeamIntroEnd() : base(){}
                public TeamIntroEnd(bool force) : base("team_intro_end", force){}

                
            }

            [EventName("team_intro_start")]
            public class TeamIntroStart : GameEvent
            {
                public TeamIntroStart() : base(){}
                public TeamIntroStart(bool force) : base("team_intro_start", force){}

                
            }

            [EventName("teamplay_broadcast_audio")]
            public class TeamplayBroadcastAudio : GameEvent
            {
                public TeamplayBroadcastAudio() : base(){}
                public TeamplayBroadcastAudio(bool force) : base("teamplay_broadcast_audio", force){}

                
                
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
            public class TeamplayRoundStart : GameEvent
            {
                public TeamplayRoundStart() : base(){}
                public TeamplayRoundStart(bool force) : base("teamplay_round_start", force){}

                
                
                // is this a full reset of the map
                public bool FullReset 
                {
                    get => Get<bool>("full_reset");
                    set => Set<bool>("full_reset", value);
                }
            }

            [EventName("team_score")]
            public class TeamScore : GameEvent
            {
                public TeamScore() : base(){}
                public TeamScore(bool force) : base("team_score", force){}

                
                
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
            public class TournamentReward : GameEvent
            {
                public TournamentReward() : base(){}
                public TournamentReward(bool force) : base("tournament_reward", force){}

                
                
                
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
            public class TrExitHintTrigger : GameEvent
            {
                public TrExitHintTrigger() : base(){}
                public TrExitHintTrigger(bool force) : base("tr_exit_hint_trigger", force){}

                
            }

            [EventName("trial_time_expired")]
            public class TrialTimeExpired : GameEvent
            {
                public TrialTimeExpired() : base(){}
                public TrialTimeExpired(bool force) : base("trial_time_expired", force){}

                
                
                // player whose time has expired
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("tr_mark_best_time")]
            public class TrMarkBestTime : GameEvent
            {
                public TrMarkBestTime() : base(){}
                public TrMarkBestTime(bool force) : base("tr_mark_best_time", force){}

                
                
                
                public long Time 
                {
                    get => Get<long>("time");
                    set => Set<long>("time", value);
                }
            }

            [EventName("tr_mark_complete")]
            public class TrMarkComplete : GameEvent
            {
                public TrMarkComplete() : base(){}
                public TrMarkComplete(bool force) : base("tr_mark_complete", force){}

                
                
                
                public int Complete 
                {
                    get => Get<int>("complete");
                    set => Set<int>("complete", value);
                }
            }

            [EventName("tr_player_flashbanged")]
            public class TrPlayerFlashbanged : GameEvent
            {
                public TrPlayerFlashbanged() : base(){}
                public TrPlayerFlashbanged(bool force) : base("tr_player_flashbanged", force){}

                
                
                // user ID of the player banged
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("tr_show_exit_msgbox")]
            public class TrShowExitMsgbox : GameEvent
            {
                public TrShowExitMsgbox() : base(){}
                public TrShowExitMsgbox(bool force) : base("tr_show_exit_msgbox", force){}

                
            }

            [EventName("tr_show_finish_msgbox")]
            public class TrShowFinishMsgbox : GameEvent
            {
                public TrShowFinishMsgbox() : base(){}
                public TrShowFinishMsgbox(bool force) : base("tr_show_finish_msgbox", force){}

                
            }

            [EventName("ugc_file_download_finished")]
            public class UgcFileDownloadFinished : GameEvent
            {
                public UgcFileDownloadFinished() : base(){}
                public UgcFileDownloadFinished(bool force) : base("ugc_file_download_finished", force){}

                
                
                // id of this specific content (may be image or map)
                public ulong Hcontent 
                {
                    get => Get<ulong>("hcontent");
                    set => Set<ulong>("hcontent", value);
                }
            }

            [EventName("ugc_file_download_start")]
            public class UgcFileDownloadStart : GameEvent
            {
                public UgcFileDownloadStart() : base(){}
                public UgcFileDownloadStart(bool force) : base("ugc_file_download_start", force){}

                
                
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
            public class UgcMapDownloadError : GameEvent
            {
                public UgcMapDownloadError() : base(){}
                public UgcMapDownloadError(bool force) : base("ugc_map_download_error", force){}

                
                
                
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
            public class UgcMapInfoReceived : GameEvent
            {
                public UgcMapInfoReceived() : base(){}
                public UgcMapInfoReceived(bool force) : base("ugc_map_info_received", force){}

                
                
                
                public ulong PublishedFileId 
                {
                    get => Get<ulong>("published_file_id");
                    set => Set<ulong>("published_file_id", value);
                }
            }

            [EventName("ugc_map_unsubscribed")]
            public class UgcMapUnsubscribed : GameEvent
            {
                public UgcMapUnsubscribed() : base(){}
                public UgcMapUnsubscribed(bool force) : base("ugc_map_unsubscribed", force){}

                
                
                
                public ulong PublishedFileId 
                {
                    get => Get<ulong>("published_file_id");
                    set => Set<ulong>("published_file_id", value);
                }
            }

            [EventName("update_matchmaking_stats")]
            public class UpdateMatchmakingStats : GameEvent
            {
                public UpdateMatchmakingStats() : base(){}
                public UpdateMatchmakingStats(bool force) : base("update_matchmaking_stats", force){}

                
            }

            [EventName("user_data_downloaded")]
            public class UserDataDownloaded : GameEvent
            {
                public UserDataDownloaded() : base(){}
                public UserDataDownloaded(bool force) : base("user_data_downloaded", force){}

                
            }

            [EventName("vip_escaped")]
            public class VipEscaped : GameEvent
            {
                public VipEscaped() : base(){}
                public VipEscaped(bool force) : base("vip_escaped", force){}

                
                
                // player who was the VIP
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("vip_killed")]
            public class VipKilled : GameEvent
            {
                public VipKilled() : base(){}
                public VipKilled(bool force) : base("vip_killed", force){}

                
                
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
            public class VoteCast : GameEvent
            {
                public VoteCast() : base(){}
                public VoteCast(bool force) : base("vote_cast", force){}

                
                
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
            public class VoteCastNo : GameEvent
            {
                public VoteCastNo() : base(){}
                public VoteCastNo(bool force) : base("vote_cast_no", force){}

                
                
                
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
            public class VoteCastYes : GameEvent
            {
                public VoteCastYes() : base(){}
                public VoteCastYes(bool force) : base("vote_cast_yes", force){}

                
                
                
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
            public class VoteChanged : GameEvent
            {
                public VoteChanged() : base(){}
                public VoteChanged(bool force) : base("vote_changed", force){}

                
                
                
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
            public class VoteEnded : GameEvent
            {
                public VoteEnded() : base(){}
                public VoteEnded(bool force) : base("vote_ended", force){}

                
            }

            [EventName("vote_failed")]
            public class VoteFailed : GameEvent
            {
                public VoteFailed() : base(){}
                public VoteFailed(bool force) : base("vote_failed", force){}

                
                
                
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
            public class VoteOptions : GameEvent
            {
                public VoteOptions() : base(){}
                public VoteOptions(bool force) : base("vote_options", force){}

                
                
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
            public class VotePassed : GameEvent
            {
                public VotePassed() : base(){}
                public VotePassed(bool force) : base("vote_passed", force){}

                
                
                
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
            public class VoteStarted : GameEvent
            {
                public VoteStarted() : base(){}
                public VoteStarted(bool force) : base("vote_started", force){}

                
                
                
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
            public class WeaponFire : GameEvent
            {
                public WeaponFire() : base(){}
                public WeaponFire(bool force) : base("weapon_fire", force){}

                
                
                
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
            public class WeaponFireOnEmpty : GameEvent
            {
                public WeaponFireOnEmpty() : base(){}
                public WeaponFireOnEmpty(bool force) : base("weapon_fire_on_empty", force){}

                
                
                
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
            public class WeaponhudSelection : GameEvent
            {
                public WeaponhudSelection() : base(){}
                public WeaponhudSelection(bool force) : base("weaponhud_selection", force){}

                
                
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
            public class WeaponOutofammo : GameEvent
            {
                public WeaponOutofammo() : base(){}
                public WeaponOutofammo(bool force) : base("weapon_outofammo", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("weapon_reload")]
            public class WeaponReload : GameEvent
            {
                public WeaponReload() : base(){}
                public WeaponReload(bool force) : base("weapon_reload", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("weapon_zoom")]
            public class WeaponZoom : GameEvent
            {
                public WeaponZoom() : base(){}
                public WeaponZoom(bool force) : base("weapon_zoom", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("weapon_zoom_rifle")]
            public class WeaponZoomRifle : GameEvent
            {
                public WeaponZoomRifle() : base(){}
                public WeaponZoomRifle(bool force) : base("weapon_zoom_rifle", force){}

                
                
                
                public int Userid 
                {
                    get => Get<int>("userid");
                    set => Set<int>("userid", value);
                }
            }

            [EventName("write_game_titledata")]
            public class WriteGameTitledata : GameEvent
            {
                public WriteGameTitledata() : base(){}
                public WriteGameTitledata(bool force) : base("write_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => Get<int>("controllerId");
                    set => Set<int>("controllerId", value);
                }
            }

            [EventName("write_profile_data")]
            public class WriteProfileData : GameEvent
            {
                public WriteProfileData() : base(){}
                public WriteProfileData(bool force) : base("write_profile_data", force){}

                
            }
}
