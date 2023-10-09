
using System;
using CounterStrikeSharp.API.Modules.Events;

namespace CounterStrikeSharp.API.Core
{
    
            public class AchievementEarned : GameEvent
            {
                public AchievementEarned() : base(){}
                public AchievementEarned(bool force) : base("achievement_earned", force){}

                
                
                // entindex of the player
                public int Player 
                {
                    get => GetInt("player");
                    set => SetInt("player", value);
                }

                
                // achievement ID
                public int Achievement 
                {
                    get => GetInt("achievement");
                    set => SetInt("achievement", value);
                }
            }

            public class AchievementEarnedLocal : GameEvent
            {
                public AchievementEarnedLocal() : base(){}
                public AchievementEarnedLocal(bool force) : base("achievement_earned_local", force){}

                
                
                // achievement ID
                public int Achievement 
                {
                    get => GetInt("achievement");
                    set => SetInt("achievement", value);
                }

                
                // splitscreen ID
                public int Splitscreenplayer 
                {
                    get => GetInt("splitscreenplayer");
                    set => SetInt("splitscreenplayer", value);
                }
            }

            public class AchievementEvent : GameEvent
            {
                public AchievementEvent() : base(){}
                public AchievementEvent(bool force) : base("achievement_event", force){}

                
                
                // non-localized name of achievement
                public string AchievementName 
                {
                    get => GetString("achievement_name");
                    set => SetString("achievement_name", value);
                }

                
                // # of steps toward achievement
                public int CurVal 
                {
                    get => GetInt("cur_val");
                    set => SetInt("cur_val", value);
                }

                
                // total # of steps in achievement
                public int MaxVal 
                {
                    get => GetInt("max_val");
                    set => SetInt("max_val", value);
                }
            }

            public class AchievementInfoLoaded : GameEvent
            {
                public AchievementInfoLoaded() : base(){}
                public AchievementInfoLoaded(bool force) : base("achievement_info_loaded", force){}

                
            }

            public class AchievementWriteFailed : GameEvent
            {
                public AchievementWriteFailed() : base(){}
                public AchievementWriteFailed(bool force) : base("achievement_write_failed", force){}

                
            }

            public class AddBulletHitMarker : GameEvent
            {
                public AddBulletHitMarker() : base(){}
                public AddBulletHitMarker(bool force) : base("add_bullet_hit_marker", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Bone 
                {
                    get => GetInt("bone");
                    set => SetInt("bone", value);
                }

                
                
                public int PosX 
                {
                    get => GetInt("pos_x");
                    set => SetInt("pos_x", value);
                }

                
                
                public int PosY 
                {
                    get => GetInt("pos_y");
                    set => SetInt("pos_y", value);
                }

                
                
                public int PosZ 
                {
                    get => GetInt("pos_z");
                    set => SetInt("pos_z", value);
                }

                
                
                public int AngX 
                {
                    get => GetInt("ang_x");
                    set => SetInt("ang_x", value);
                }

                
                
                public int AngY 
                {
                    get => GetInt("ang_y");
                    set => SetInt("ang_y", value);
                }

                
                
                public int AngZ 
                {
                    get => GetInt("ang_z");
                    set => SetInt("ang_z", value);
                }

                
                
                public int StartX 
                {
                    get => GetInt("start_x");
                    set => SetInt("start_x", value);
                }

                
                
                public int StartY 
                {
                    get => GetInt("start_y");
                    set => SetInt("start_y", value);
                }

                
                
                public int StartZ 
                {
                    get => GetInt("start_z");
                    set => SetInt("start_z", value);
                }

                
                
                public bool Hit 
                {
                    get => GetBool("hit");
                    set => SetBool("hit", value);
                }
            }

            public class AddPlayerSonarIcon : GameEvent
            {
                public AddPlayerSonarIcon() : base(){}
                public AddPlayerSonarIcon(bool force) : base("add_player_sonar_icon", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public float PosX 
                {
                    get => GetFloat("pos_x");
                    set => SetFloat("pos_x", value);
                }

                
                
                public float PosY 
                {
                    get => GetFloat("pos_y");
                    set => SetFloat("pos_y", value);
                }

                
                
                public float PosZ 
                {
                    get => GetFloat("pos_z");
                    set => SetFloat("pos_z", value);
                }
            }

            public class AmmoPickup : GameEvent
            {
                public AmmoPickup() : base(){}
                public AmmoPickup(bool force) : base("ammo_pickup", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item 
                {
                    get => GetString("item");
                    set => SetString("item", value);
                }

                
                // the weapon entindex
                public long Index 
                {
                    get => GetInt("index");
                    set => SetInt("index", value);
                }
            }

            public class AmmoRefill : GameEvent
            {
                public AmmoRefill() : base(){}
                public AmmoRefill(bool force) : base("ammo_refill", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public bool Success 
                {
                    get => GetBool("success");
                    set => SetBool("success", value);
                }
            }

            public class AnnouncePhaseEnd : GameEvent
            {
                public AnnouncePhaseEnd() : base(){}
                public AnnouncePhaseEnd(bool force) : base("announce_phase_end", force){}

                
            }

            public class BeginNewMatch : GameEvent
            {
                public BeginNewMatch() : base(){}
                public BeginNewMatch(bool force) : base("begin_new_match", force){}

                
            }

            public class BombAbortdefuse : GameEvent
            {
                public BombAbortdefuse() : base(){}
                public BombAbortdefuse(bool force) : base("bomb_abortdefuse", force){}

                
                
                // player who was defusing
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class BombAbortplant : GameEvent
            {
                public BombAbortplant() : base(){}
                public BombAbortplant(bool force) : base("bomb_abortplant", force){}

                
                
                // player who is planting the bomb
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => GetInt("site");
                    set => SetInt("site", value);
                }
            }

            public class BombBeep : GameEvent
            {
                public BombBeep() : base(){}
                public BombBeep(bool force) : base("bomb_beep", force){}

                
                
                // c4 entity
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }
            }

            public class BombBegindefuse : GameEvent
            {
                public BombBegindefuse() : base(){}
                public BombBegindefuse(bool force) : base("bomb_begindefuse", force){}

                
                
                // player who is defusing
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public bool Haskit 
                {
                    get => GetBool("haskit");
                    set => SetBool("haskit", value);
                }
            }

            public class BombBeginplant : GameEvent
            {
                public BombBeginplant() : base(){}
                public BombBeginplant(bool force) : base("bomb_beginplant", force){}

                
                
                // player who is planting the bomb
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => GetInt("site");
                    set => SetInt("site", value);
                }
            }

            public class BombDefused : GameEvent
            {
                public BombDefused() : base(){}
                public BombDefused(bool force) : base("bomb_defused", force){}

                
                
                // player who defused the bomb
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => GetInt("site");
                    set => SetInt("site", value);
                }
            }

            public class BombDropped : GameEvent
            {
                public BombDropped() : base(){}
                public BombDropped(bool force) : base("bomb_dropped", force){}

                
                
                // player who dropped the bomb
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }
            }

            public class BombExploded : GameEvent
            {
                public BombExploded() : base(){}
                public BombExploded(bool force) : base("bomb_exploded", force){}

                
                
                // player who planted the bomb
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => GetInt("site");
                    set => SetInt("site", value);
                }
            }

            public class BombPickup : GameEvent
            {
                public BombPickup() : base(){}
                public BombPickup(bool force) : base("bomb_pickup", force){}

                
                
                // player pawn who picked up the bomb
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class BombPlanted : GameEvent
            {
                public BombPlanted() : base(){}
                public BombPlanted(bool force) : base("bomb_planted", force){}

                
                
                // player who planted the bomb
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // bombsite index
                public int Site 
                {
                    get => GetInt("site");
                    set => SetInt("site", value);
                }
            }

            public class BonusUpdated : GameEvent
            {
                public BonusUpdated() : base(){}
                public BonusUpdated(bool force) : base("bonus_updated", force){}

                
                
                
                public int Numadvanced 
                {
                    get => GetInt("numadvanced");
                    set => SetInt("numadvanced", value);
                }

                
                
                public int Numbronze 
                {
                    get => GetInt("numbronze");
                    set => SetInt("numbronze", value);
                }

                
                
                public int Numsilver 
                {
                    get => GetInt("numsilver");
                    set => SetInt("numsilver", value);
                }

                
                
                public int Numgold 
                {
                    get => GetInt("numgold");
                    set => SetInt("numgold", value);
                }
            }

            public class BotTakeover : GameEvent
            {
                public BotTakeover() : base(){}
                public BotTakeover(bool force) : base("bot_takeover", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Botid 
                {
                    get => GetInt("botid");
                    set => SetInt("botid", value);
                }

                
                
                public float P 
                {
                    get => GetFloat("p");
                    set => SetFloat("p", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float R 
                {
                    get => GetFloat("r");
                    set => SetFloat("r", value);
                }
            }

            public class BreakBreakable : GameEvent
            {
                public BreakBreakable() : base(){}
                public BreakBreakable(bool force) : base("break_breakable", force){}

                
                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }

                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // BREAK_GLASS, BREAK_WOOD, etc
                public int Material 
                {
                    get => GetInt("material");
                    set => SetInt("material", value);
                }
            }

            public class BreakProp : GameEvent
            {
                public BreakProp() : base(){}
                public BreakProp(bool force) : base("break_prop", force){}

                
                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }

                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class BrokenBreakable : GameEvent
            {
                public BrokenBreakable() : base(){}
                public BrokenBreakable(bool force) : base("broken_breakable", force){}

                
                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }

                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // BREAK_GLASS, BREAK_WOOD, etc
                public int Material 
                {
                    get => GetInt("material");
                    set => SetInt("material", value);
                }
            }

            public class BulletFlightResolution : GameEvent
            {
                public BulletFlightResolution() : base(){}
                public BulletFlightResolution(bool force) : base("bullet_flight_resolution", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int PosX 
                {
                    get => GetInt("pos_x");
                    set => SetInt("pos_x", value);
                }

                
                
                public int PosY 
                {
                    get => GetInt("pos_y");
                    set => SetInt("pos_y", value);
                }

                
                
                public int PosZ 
                {
                    get => GetInt("pos_z");
                    set => SetInt("pos_z", value);
                }

                
                
                public int AngX 
                {
                    get => GetInt("ang_x");
                    set => SetInt("ang_x", value);
                }

                
                
                public int AngY 
                {
                    get => GetInt("ang_y");
                    set => SetInt("ang_y", value);
                }

                
                
                public int AngZ 
                {
                    get => GetInt("ang_z");
                    set => SetInt("ang_z", value);
                }

                
                
                public int StartX 
                {
                    get => GetInt("start_x");
                    set => SetInt("start_x", value);
                }

                
                
                public int StartY 
                {
                    get => GetInt("start_y");
                    set => SetInt("start_y", value);
                }

                
                
                public int StartZ 
                {
                    get => GetInt("start_z");
                    set => SetInt("start_z", value);
                }
            }

            public class BulletImpact : GameEvent
            {
                public BulletImpact() : base(){}
                public BulletImpact(bool force) : base("bullet_impact", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class BuymenuClose : GameEvent
            {
                public BuymenuClose() : base(){}
                public BuymenuClose(bool force) : base("buymenu_close", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class BuymenuOpen : GameEvent
            {
                public BuymenuOpen() : base(){}
                public BuymenuOpen(bool force) : base("buymenu_open", force){}

                
            }

            public class BuytimeEnded : GameEvent
            {
                public BuytimeEnded() : base(){}
                public BuytimeEnded(bool force) : base("buytime_ended", force){}

                
            }

            public class CartUpdated : GameEvent
            {
                public CartUpdated() : base(){}
                public CartUpdated(bool force) : base("cart_updated", force){}

                
            }

            public class ChoppersIncomingWarning : GameEvent
            {
                public ChoppersIncomingWarning() : base(){}
                public ChoppersIncomingWarning(bool force) : base("choppers_incoming_warning", force){}

                
                
                
                public bool Global 
                {
                    get => GetBool("global");
                    set => SetBool("global", value);
                }
            }

            public class ClientDisconnect : GameEvent
            {
                public ClientDisconnect() : base(){}
                public ClientDisconnect(bool force) : base("client_disconnect", force){}

                
            }

            public class ClientLoadoutChanged : GameEvent
            {
                public ClientLoadoutChanged() : base(){}
                public ClientLoadoutChanged(bool force) : base("client_loadout_changed", force){}

                
            }

            public class ClientsideLessonClosed : GameEvent
            {
                public ClientsideLessonClosed() : base(){}
                public ClientsideLessonClosed(bool force) : base("clientside_lesson_closed", force){}

                
                
                
                public string LessonName 
                {
                    get => GetString("lesson_name");
                    set => SetString("lesson_name", value);
                }
            }

            public class CsGameDisconnected : GameEvent
            {
                public CsGameDisconnected() : base(){}
                public CsGameDisconnected(bool force) : base("cs_game_disconnected", force){}

                
            }

            public class CsIntermission : GameEvent
            {
                public CsIntermission() : base(){}
                public CsIntermission(bool force) : base("cs_intermission", force){}

                
            }

            public class CsMatchEndRestart : GameEvent
            {
                public CsMatchEndRestart() : base(){}
                public CsMatchEndRestart(bool force) : base("cs_match_end_restart", force){}

                
            }

            public class CsPreRestart : GameEvent
            {
                public CsPreRestart() : base(){}
                public CsPreRestart(bool force) : base("cs_pre_restart", force){}

                
            }

            public class CsPrevNextSpectator : GameEvent
            {
                public CsPrevNextSpectator() : base(){}
                public CsPrevNextSpectator(bool force) : base("cs_prev_next_spectator", force){}

                
                
                
                public bool Next 
                {
                    get => GetBool("next");
                    set => SetBool("next", value);
                }
            }

            public class CsRoundFinalBeep : GameEvent
            {
                public CsRoundFinalBeep() : base(){}
                public CsRoundFinalBeep(bool force) : base("cs_round_final_beep", force){}

                
            }

            public class CsRoundStartBeep : GameEvent
            {
                public CsRoundStartBeep() : base(){}
                public CsRoundStartBeep(bool force) : base("cs_round_start_beep", force){}

                
            }

            public class CsWinPanelMatch : GameEvent
            {
                public CsWinPanelMatch() : base(){}
                public CsWinPanelMatch(bool force) : base("cs_win_panel_match", force){}

                
            }

            public class CsWinPanelRound : GameEvent
            {
                public CsWinPanelRound() : base(){}
                public CsWinPanelRound(bool force) : base("cs_win_panel_round", force){}

                
                
                
                public bool ShowTimerDefend 
                {
                    get => GetBool("show_timer_defend");
                    set => SetBool("show_timer_defend", value);
                }

                
                
                public bool ShowTimerAttack 
                {
                    get => GetBool("show_timer_attack");
                    set => SetBool("show_timer_attack", value);
                }

                
                
                public int TimerTime 
                {
                    get => GetInt("timer_time");
                    set => SetInt("timer_time", value);
                }

                
                // define in cs_gamerules.h
                public int FinalEvent 
                {
                    get => GetInt("final_event");
                    set => SetInt("final_event", value);
                }

                
                
                public string FunfactToken 
                {
                    get => GetString("funfact_token");
                    set => SetString("funfact_token", value);
                }

                
                
                public int FunfactPlayer 
                {
                    get => GetInt("funfact_player");
                    set => SetInt("funfact_player", value);
                }

                
                
                public long FunfactData1 
                {
                    get => GetInt("funfact_data1");
                    set => SetInt("funfact_data1", value);
                }

                
                
                public long FunfactData2 
                {
                    get => GetInt("funfact_data2");
                    set => SetInt("funfact_data2", value);
                }

                
                
                public long FunfactData3 
                {
                    get => GetInt("funfact_data3");
                    set => SetInt("funfact_data3", value);
                }
            }

            public class DecoyDetonate : GameEvent
            {
                public DecoyDetonate() : base(){}
                public DecoyDetonate(bool force) : base("decoy_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class DecoyFiring : GameEvent
            {
                public DecoyFiring() : base(){}
                public DecoyFiring(bool force) : base("decoy_firing", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class DecoyStarted : GameEvent
            {
                public DecoyStarted() : base(){}
                public DecoyStarted(bool force) : base("decoy_started", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class DefuserDropped : GameEvent
            {
                public DefuserDropped() : base(){}
                public DefuserDropped(bool force) : base("defuser_dropped", force){}

                
                
                // defuser's entity ID
                public long Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }
            }

            public class DefuserPickup : GameEvent
            {
                public DefuserPickup() : base(){}
                public DefuserPickup(bool force) : base("defuser_pickup", force){}

                
                
                // defuser's entity ID
                public long Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                // player who picked up the defuser
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class DemoSkip : GameEvent
            {
                public DemoSkip() : base(){}
                public DemoSkip(bool force) : base("demo_skip", force){}

                
                
                
                public int Local 
                {
                    get => GetInt("local");
                    set => SetInt("local", value);
                }

                
                // current playback tick
                public long PlaybackTick 
                {
                    get => GetInt("playback_tick");
                    set => SetInt("playback_tick", value);
                }

                
                // tick we're going to
                public long SkiptoTick 
                {
                    get => GetInt("skipto_tick");
                    set => SetInt("skipto_tick", value);
                }

                
                // CSVCMsgList_UserMessages
                public int UserMessageList 
                {
                    get => GetInt("user_message_list");
                    set => SetInt("user_message_list", value);
                }

                
                // CSVCMsgList_GameEvents
                public int DotaHeroChaseList 
                {
                    get => GetInt("dota_hero_chase_list");
                    set => SetInt("dota_hero_chase_list", value);
                }
            }

            public class DemoStart : GameEvent
            {
                public DemoStart() : base(){}
                public DemoStart(bool force) : base("demo_start", force){}

                
                
                
                public int Local 
                {
                    get => GetInt("local");
                    set => SetInt("local", value);
                }

                
                // CSVCMsgList_GameEvents that are combat log events
                public int DotaCombatlogList 
                {
                    get => GetInt("dota_combatlog_list");
                    set => SetInt("dota_combatlog_list", value);
                }

                
                // CSVCMsgList_GameEvents
                public int DotaHeroChaseList 
                {
                    get => GetInt("dota_hero_chase_list");
                    set => SetInt("dota_hero_chase_list", value);
                }

                
                // CSVCMsgList_GameEvents
                public int DotaPickHeroList 
                {
                    get => GetInt("dota_pick_hero_list");
                    set => SetInt("dota_pick_hero_list", value);
                }
            }

            public class DemoStop : GameEvent
            {
                public DemoStop() : base(){}
                public DemoStop(bool force) : base("demo_stop", force){}

                
            }

            public class DifficultyChanged : GameEvent
            {
                public DifficultyChanged() : base(){}
                public DifficultyChanged(bool force) : base("difficulty_changed", force){}

                
                
                
                public int Newdifficulty 
                {
                    get => GetInt("newDifficulty");
                    set => SetInt("newDifficulty", value);
                }

                
                
                public int Olddifficulty 
                {
                    get => GetInt("oldDifficulty");
                    set => SetInt("oldDifficulty", value);
                }

                
                // new difficulty as string
                public string Strdifficulty 
                {
                    get => GetString("strDifficulty");
                    set => SetString("strDifficulty", value);
                }
            }

            public class DmBonusWeaponStart : GameEvent
            {
                public DmBonusWeaponStart() : base(){}
                public DmBonusWeaponStart(bool force) : base("dm_bonus_weapon_start", force){}

                
                
                // The length of time that this bonus lasts
                public int Time 
                {
                    get => GetInt("time");
                    set => SetInt("time", value);
                }

                
                // Loadout position of the bonus weapon
                public int Pos 
                {
                    get => GetInt("Pos");
                    set => SetInt("Pos", value);
                }
            }

            public class DoorBreak : GameEvent
            {
                public DoorBreak() : base(){}
                public DoorBreak(bool force) : base("door_break", force){}

                
                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }

                
                
                public long Dmgstate 
                {
                    get => GetInt("dmgstate");
                    set => SetInt("dmgstate", value);
                }
            }

            public class DoorClose : GameEvent
            {
                public DoorClose() : base(){}
                public DoorClose(bool force) : base("door_close", force){}

                
                
                // Who closed the door
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // Is the door a checkpoint door
                public bool Checkpoint 
                {
                    get => GetBool("checkpoint");
                    set => SetBool("checkpoint", value);
                }
            }

            public class DoorClosed : GameEvent
            {
                public DoorClosed() : base(){}
                public DoorClosed(bool force) : base("door_closed", force){}

                
                
                // Who closed the door
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }
            }

            public class DoorMoving : GameEvent
            {
                public DoorMoving() : base(){}
                public DoorMoving(bool force) : base("door_moving", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }
            }

            public class DoorOpen : GameEvent
            {
                public DoorOpen() : base(){}
                public DoorOpen(bool force) : base("door_open", force){}

                
                
                // Who closed the door
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }
            }

            public class DroneAboveRoof : GameEvent
            {
                public DroneAboveRoof() : base(){}
                public DroneAboveRoof(bool force) : base("drone_above_roof", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Cargo 
                {
                    get => GetInt("cargo");
                    set => SetInt("cargo", value);
                }
            }

            public class DroneCargoDetached : GameEvent
            {
                public DroneCargoDetached() : base(){}
                public DroneCargoDetached(bool force) : base("drone_cargo_detached", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Cargo 
                {
                    get => GetInt("cargo");
                    set => SetInt("cargo", value);
                }

                
                
                public bool Delivered 
                {
                    get => GetBool("delivered");
                    set => SetBool("delivered", value);
                }
            }

            public class DroneDispatched : GameEvent
            {
                public DroneDispatched() : base(){}
                public DroneDispatched(bool force) : base("drone_dispatched", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Priority 
                {
                    get => GetInt("priority");
                    set => SetInt("priority", value);
                }

                
                
                public int DroneDispatchedParam 
                {
                    get => GetInt("drone_dispatched");
                    set => SetInt("drone_dispatched", value);
                }
            }

            public class DronegunAttack : GameEvent
            {
                public DronegunAttack() : base(){}
                public DronegunAttack(bool force) : base("dronegun_attack", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class DropRateModified : GameEvent
            {
                public DropRateModified() : base(){}
                public DropRateModified(bool force) : base("drop_rate_modified", force){}

                
            }

            public class DynamicShadowLightChanged : GameEvent
            {
                public DynamicShadowLightChanged() : base(){}
                public DynamicShadowLightChanged(bool force) : base("dynamic_shadow_light_changed", force){}

                
            }

            public class DzItemInteraction : GameEvent
            {
                public DzItemInteraction() : base(){}
                public DzItemInteraction(bool force) : base("dz_item_interaction", force){}

                
                
                // player entindex
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // crate entindex
                public int Subject 
                {
                    get => GetInt("subject");
                    set => SetInt("subject", value);
                }

                
                // type of crate (metal, wood, or paradrop)
                public string Type 
                {
                    get => GetString("type");
                    set => SetString("type", value);
                }
            }

            public class EnableRestartVoting : GameEvent
            {
                public EnableRestartVoting() : base(){}
                public EnableRestartVoting(bool force) : base("enable_restart_voting", force){}

                
                
                
                public bool Enable 
                {
                    get => GetBool("enable");
                    set => SetBool("enable", value);
                }
            }

            public class EndmatchCmmStartRevealItems : GameEvent
            {
                public EndmatchCmmStartRevealItems() : base(){}
                public EndmatchCmmStartRevealItems(bool force) : base("endmatch_cmm_start_reveal_items", force){}

                
            }

            public class EndmatchMapvoteSelectingMap : GameEvent
            {
                public EndmatchMapvoteSelectingMap() : base(){}
                public EndmatchMapvoteSelectingMap(bool force) : base("endmatch_mapvote_selecting_map", force){}

                
                
                // Number of "ties"
                public int Count 
                {
                    get => GetInt("count");
                    set => SetInt("count", value);
                }

                
                
                public int Slot1 
                {
                    get => GetInt("slot1");
                    set => SetInt("slot1", value);
                }

                
                
                public int Slot2 
                {
                    get => GetInt("slot2");
                    set => SetInt("slot2", value);
                }

                
                
                public int Slot3 
                {
                    get => GetInt("slot3");
                    set => SetInt("slot3", value);
                }

                
                
                public int Slot4 
                {
                    get => GetInt("slot4");
                    set => SetInt("slot4", value);
                }

                
                
                public int Slot5 
                {
                    get => GetInt("slot5");
                    set => SetInt("slot5", value);
                }

                
                
                public int Slot6 
                {
                    get => GetInt("slot6");
                    set => SetInt("slot6", value);
                }

                
                
                public int Slot7 
                {
                    get => GetInt("slot7");
                    set => SetInt("slot7", value);
                }

                
                
                public int Slot8 
                {
                    get => GetInt("slot8");
                    set => SetInt("slot8", value);
                }

                
                
                public int Slot9 
                {
                    get => GetInt("slot9");
                    set => SetInt("slot9", value);
                }

                
                
                public int Slot10 
                {
                    get => GetInt("slot10");
                    set => SetInt("slot10", value);
                }
            }

            public class EnterBombzone : GameEvent
            {
                public EnterBombzone() : base(){}
                public EnterBombzone(bool force) : base("enter_bombzone", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public bool Hasbomb 
                {
                    get => GetBool("hasbomb");
                    set => SetBool("hasbomb", value);
                }

                
                
                public bool Isplanted 
                {
                    get => GetBool("isplanted");
                    set => SetBool("isplanted", value);
                }
            }

            public class EnterBuyzone : GameEvent
            {
                public EnterBuyzone() : base(){}
                public EnterBuyzone(bool force) : base("enter_buyzone", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public bool Canbuy 
                {
                    get => GetBool("canbuy");
                    set => SetBool("canbuy", value);
                }
            }

            public class EnterRescueZone : GameEvent
            {
                public EnterRescueZone() : base(){}
                public EnterRescueZone(bool force) : base("enter_rescue_zone", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class EntityKilled : GameEvent
            {
                public EntityKilled() : base(){}
                public EntityKilled(bool force) : base("entity_killed", force){}

                
                
                
                public long EntindexKilled 
                {
                    get => GetInt("entindex_killed");
                    set => SetInt("entindex_killed", value);
                }

                
                
                public long EntindexAttacker 
                {
                    get => GetInt("entindex_attacker");
                    set => SetInt("entindex_attacker", value);
                }

                
                
                public long EntindexInflictor 
                {
                    get => GetInt("entindex_inflictor");
                    set => SetInt("entindex_inflictor", value);
                }

                
                
                public long Damagebits 
                {
                    get => GetInt("damagebits");
                    set => SetInt("damagebits", value);
                }
            }

            public class EntityVisible : GameEvent
            {
                public EntityVisible() : base(){}
                public EntityVisible(bool force) : base("entity_visible", force){}

                
                
                // The player who sees the entity
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // Entindex of the entity they see
                public int Subject 
                {
                    get => GetInt("subject");
                    set => SetInt("subject", value);
                }

                
                // Classname of the entity they see
                public string Classname 
                {
                    get => GetString("classname");
                    set => SetString("classname", value);
                }

                
                // name of the entity they see
                public string Entityname 
                {
                    get => GetString("entityname");
                    set => SetString("entityname", value);
                }
            }

            public class EventTicketModified : GameEvent
            {
                public EventTicketModified() : base(){}
                public EventTicketModified(bool force) : base("event_ticket_modified", force){}

                
            }

            public class ExitBombzone : GameEvent
            {
                public ExitBombzone() : base(){}
                public ExitBombzone(bool force) : base("exit_bombzone", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public bool Hasbomb 
                {
                    get => GetBool("hasbomb");
                    set => SetBool("hasbomb", value);
                }

                
                
                public bool Isplanted 
                {
                    get => GetBool("isplanted");
                    set => SetBool("isplanted", value);
                }
            }

            public class ExitBuyzone : GameEvent
            {
                public ExitBuyzone() : base(){}
                public ExitBuyzone(bool force) : base("exit_buyzone", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public bool Canbuy 
                {
                    get => GetBool("canbuy");
                    set => SetBool("canbuy", value);
                }
            }

            public class ExitRescueZone : GameEvent
            {
                public ExitRescueZone() : base(){}
                public ExitRescueZone(bool force) : base("exit_rescue_zone", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class FinaleStart : GameEvent
            {
                public FinaleStart() : base(){}
                public FinaleStart(bool force) : base("finale_start", force){}

                
                
                
                public int Rushes 
                {
                    get => GetInt("rushes");
                    set => SetInt("rushes", value);
                }
            }

            public class FirstbombsIncomingWarning : GameEvent
            {
                public FirstbombsIncomingWarning() : base(){}
                public FirstbombsIncomingWarning(bool force) : base("firstbombs_incoming_warning", force){}

                
                
                
                public bool Global 
                {
                    get => GetBool("global");
                    set => SetBool("global", value);
                }
            }

            public class FlareIgniteNpc : GameEvent
            {
                public FlareIgniteNpc() : base(){}
                public FlareIgniteNpc(bool force) : base("flare_ignite_npc", force){}

                
                
                // entity ignited
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }
            }

            public class FlashbangDetonate : GameEvent
            {
                public FlashbangDetonate() : base(){}
                public FlashbangDetonate(bool force) : base("flashbang_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class GameEnd : GameEvent
            {
                public GameEnd() : base(){}
                public GameEnd(bool force) : base("game_end", force){}

                
                
                // winner team/user id
                public int Winner 
                {
                    get => GetInt("winner");
                    set => SetInt("winner", value);
                }
            }

            public class GameInit : GameEvent
            {
                public GameInit() : base(){}
                public GameInit(bool force) : base("game_init", force){}

                
            }

            public class GameinstructorDraw : GameEvent
            {
                public GameinstructorDraw() : base(){}
                public GameinstructorDraw(bool force) : base("gameinstructor_draw", force){}

                
            }

            public class GameinstructorNodraw : GameEvent
            {
                public GameinstructorNodraw() : base(){}
                public GameinstructorNodraw(bool force) : base("gameinstructor_nodraw", force){}

                
            }

            public class GameMessage : GameEvent
            {
                public GameMessage() : base(){}
                public GameMessage(bool force) : base("game_message", force){}

                
                
                // 0 = console, 1 = HUD
                public int Target 
                {
                    get => GetInt("target");
                    set => SetInt("target", value);
                }

                
                // the message text
                public string Text 
                {
                    get => GetString("text");
                    set => SetString("text", value);
                }
            }

            public class GameNewmap : GameEvent
            {
                public GameNewmap() : base(){}
                public GameNewmap(bool force) : base("game_newmap", force){}

                
                
                // map name
                public string Mapname 
                {
                    get => GetString("mapname");
                    set => SetString("mapname", value);
                }
            }

            public class GamePhaseChanged : GameEvent
            {
                public GamePhaseChanged() : base(){}
                public GamePhaseChanged(bool force) : base("game_phase_changed", force){}

                
                
                
                public int NewPhase 
                {
                    get => GetInt("new_phase");
                    set => SetInt("new_phase", value);
                }
            }

            public class GameStart : GameEvent
            {
                public GameStart() : base(){}
                public GameStart(bool force) : base("game_start", force){}

                
                
                // max round
                public long Roundslimit 
                {
                    get => GetInt("roundslimit");
                    set => SetInt("roundslimit", value);
                }

                
                // time limit
                public long Timelimit 
                {
                    get => GetInt("timelimit");
                    set => SetInt("timelimit", value);
                }

                
                // frag limit
                public long Fraglimit 
                {
                    get => GetInt("fraglimit");
                    set => SetInt("fraglimit", value);
                }

                
                // round objective
                public string Objective 
                {
                    get => GetString("objective");
                    set => SetString("objective", value);
                }
            }

            public class GameuiHidden : GameEvent
            {
                public GameuiHidden() : base(){}
                public GameuiHidden(bool force) : base("gameui_hidden", force){}

                
            }

            public class GcConnected : GameEvent
            {
                public GcConnected() : base(){}
                public GcConnected(bool force) : base("gc_connected", force){}

                
            }

            public class GgKilledEnemy : GameEvent
            {
                public GgKilledEnemy() : base(){}
                public GgKilledEnemy(bool force) : base("gg_killed_enemy", force){}

                
                
                // user ID who died
                public int Victimid 
                {
                    get => GetInt("victimid");
                    set => SetInt("victimid", value);
                }

                
                // user ID who killed
                public int Attackerid 
                {
                    get => GetInt("attackerid");
                    set => SetInt("attackerid", value);
                }

                
                // did killer dominate victim with this kill
                public int Dominated 
                {
                    get => GetInt("dominated");
                    set => SetInt("dominated", value);
                }

                
                // did killer get revenge on victim with this kill
                public int Revenge 
                {
                    get => GetInt("revenge");
                    set => SetInt("revenge", value);
                }

                
                // did killer kill with a bonus weapon?
                public bool Bonus 
                {
                    get => GetBool("bonus");
                    set => SetBool("bonus", value);
                }
            }

            public class GrenadeBounce : GameEvent
            {
                public GrenadeBounce() : base(){}
                public GrenadeBounce(bool force) : base("grenade_bounce", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class GrenadeThrown : GameEvent
            {
                public GrenadeThrown() : base(){}
                public GrenadeThrown(bool force) : base("grenade_thrown", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // weapon name used
                public string Weapon 
                {
                    get => GetString("weapon");
                    set => SetString("weapon", value);
                }
            }

            public class GuardianWaveRestart : GameEvent
            {
                public GuardianWaveRestart() : base(){}
                public GuardianWaveRestart(bool force) : base("guardian_wave_restart", force){}

                
            }

            public class HegrenadeDetonate : GameEvent
            {
                public HegrenadeDetonate() : base(){}
                public HegrenadeDetonate(bool force) : base("hegrenade_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class HelicopterGrenadePuntMiss : GameEvent
            {
                public HelicopterGrenadePuntMiss() : base(){}
                public HelicopterGrenadePuntMiss(bool force) : base("helicopter_grenade_punt_miss", force){}

                
            }

            public class HideDeathpanel : GameEvent
            {
                public HideDeathpanel() : base(){}
                public HideDeathpanel(bool force) : base("hide_deathpanel", force){}

                
            }

            public class HltvCameraman : GameEvent
            {
                public HltvCameraman() : base(){}
                public HltvCameraman(bool force) : base("hltv_cameraman", force){}

                
                
                // camera man entity index
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class HltvChangedMode : GameEvent
            {
                public HltvChangedMode() : base(){}
                public HltvChangedMode(bool force) : base("hltv_changed_mode", force){}

                
                
                
                public long Oldmode 
                {
                    get => GetInt("oldmode");
                    set => SetInt("oldmode", value);
                }

                
                
                public long Newmode 
                {
                    get => GetInt("newmode");
                    set => SetInt("newmode", value);
                }

                
                
                public long ObsTarget 
                {
                    get => GetInt("obs_target");
                    set => SetInt("obs_target", value);
                }
            }

            public class HltvChase : GameEvent
            {
                public HltvChase() : base(){}
                public HltvChase(bool force) : base("hltv_chase", force){}

                
                
                // primary traget index
                public int Target1 
                {
                    get => GetInt("target1");
                    set => SetInt("target1", value);
                }

                
                // secondary traget index or 0
                public int Target2 
                {
                    get => GetInt("target2");
                    set => SetInt("target2", value);
                }

                
                // camera distance
                public int Distance 
                {
                    get => GetInt("distance");
                    set => SetInt("distance", value);
                }

                
                // view angle horizontal
                public int Theta 
                {
                    get => GetInt("theta");
                    set => SetInt("theta", value);
                }

                
                // view angle vertical
                public int Phi 
                {
                    get => GetInt("phi");
                    set => SetInt("phi", value);
                }

                
                // camera inertia
                public int Inertia 
                {
                    get => GetInt("inertia");
                    set => SetInt("inertia", value);
                }

                
                // diretcor suggests to show ineye
                public int Ineye 
                {
                    get => GetInt("ineye");
                    set => SetInt("ineye", value);
                }
            }

            public class HltvChat : GameEvent
            {
                public HltvChat() : base(){}
                public HltvChat(bool force) : base("hltv_chat", force){}

                
                
                
                public string Text 
                {
                    get => GetString("text");
                    set => SetString("text", value);
                }

                
                // steam id
                public ulong Steamid 
                {
                    get => GetUint64("steamID");
                    set => SetUint64("steamID", value);
                }
            }

            public class HltvFixed : GameEvent
            {
                public HltvFixed() : base(){}
                public HltvFixed(bool force) : base("hltv_fixed", force){}

                
                
                // camera position in world
                public long Posx 
                {
                    get => GetInt("posx");
                    set => SetInt("posx", value);
                }

                
                
                public long Posy 
                {
                    get => GetInt("posy");
                    set => SetInt("posy", value);
                }

                
                
                public long Posz 
                {
                    get => GetInt("posz");
                    set => SetInt("posz", value);
                }

                
                // camera angles
                public int Theta 
                {
                    get => GetInt("theta");
                    set => SetInt("theta", value);
                }

                
                
                public int Phi 
                {
                    get => GetInt("phi");
                    set => SetInt("phi", value);
                }

                
                
                public int Offset 
                {
                    get => GetInt("offset");
                    set => SetInt("offset", value);
                }

                
                
                public float Fov 
                {
                    get => GetFloat("fov");
                    set => SetFloat("fov", value);
                }

                
                // follow this player
                public int Target 
                {
                    get => GetInt("target");
                    set => SetInt("target", value);
                }
            }

            public class HltvMessage : GameEvent
            {
                public HltvMessage() : base(){}
                public HltvMessage(bool force) : base("hltv_message", force){}

                
                
                
                public string Text 
                {
                    get => GetString("text");
                    set => SetString("text", value);
                }
            }

            public class HltvRankCamera : GameEvent
            {
                public HltvRankCamera() : base(){}
                public HltvRankCamera(bool force) : base("hltv_rank_camera", force){}

                
                
                // fixed camera index
                public int Index 
                {
                    get => GetInt("index");
                    set => SetInt("index", value);
                }

                
                // ranking, how interesting is this camera view
                public float Rank 
                {
                    get => GetFloat("rank");
                    set => SetFloat("rank", value);
                }

                
                // best/closest target entity
                public int Target 
                {
                    get => GetInt("target");
                    set => SetInt("target", value);
                }
            }

            public class HltvRankEntity : GameEvent
            {
                public HltvRankEntity() : base(){}
                public HltvRankEntity(bool force) : base("hltv_rank_entity", force){}

                
                
                // player slot
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // ranking, how interesting is this entity to view
                public float Rank 
                {
                    get => GetFloat("rank");
                    set => SetFloat("rank", value);
                }

                
                // best/closest target entity
                public int Target 
                {
                    get => GetInt("target");
                    set => SetInt("target", value);
                }
            }

            public class HltvReplay : GameEvent
            {
                public HltvReplay() : base(){}
                public HltvReplay(bool force) : base("hltv_replay", force){}

                
                
                // number of seconds in killer replay delay
                public long Delay 
                {
                    get => GetInt("delay");
                    set => SetInt("delay", value);
                }

                
                // reason for replay	(ReplayEventType_t)
                public long Reason 
                {
                    get => GetInt("reason");
                    set => SetInt("reason", value);
                }
            }

            public class HltvReplayStatus : GameEvent
            {
                public HltvReplayStatus() : base(){}
                public HltvReplayStatus(bool force) : base("hltv_replay_status", force){}

                
                
                // reason for hltv replay status change ()
                public long Reason 
                {
                    get => GetInt("reason");
                    set => SetInt("reason", value);
                }
            }

            public class HltvStatus : GameEvent
            {
                public HltvStatus() : base(){}
                public HltvStatus(bool force) : base("hltv_status", force){}

                
                
                // number of HLTV spectators
                public long Clients 
                {
                    get => GetInt("clients");
                    set => SetInt("clients", value);
                }

                
                // number of HLTV slots
                public long Slots 
                {
                    get => GetInt("slots");
                    set => SetInt("slots", value);
                }

                
                // number of HLTV proxies
                public int Proxies 
                {
                    get => GetInt("proxies");
                    set => SetInt("proxies", value);
                }

                
                // disptach master IP:port
                public string Master 
                {
                    get => GetString("master");
                    set => SetString("master", value);
                }
            }

            public class HltvTitle : GameEvent
            {
                public HltvTitle() : base(){}
                public HltvTitle(bool force) : base("hltv_title", force){}

                
                
                
                public string Text 
                {
                    get => GetString("text");
                    set => SetString("text", value);
                }
            }

            public class HltvVersioninfo : GameEvent
            {
                public HltvVersioninfo() : base(){}
                public HltvVersioninfo(bool force) : base("hltv_versioninfo", force){}

                
                
                
                public long Version 
                {
                    get => GetInt("version");
                    set => SetInt("version", value);
                }
            }

            public class HostageCallForHelp : GameEvent
            {
                public HostageCallForHelp() : base(){}
                public HostageCallForHelp(bool force) : base("hostage_call_for_help", force){}

                
                
                // hostage entity index
                public int Hostage 
                {
                    get => GetInt("hostage");
                    set => SetInt("hostage", value);
                }
            }

            public class HostageFollows : GameEvent
            {
                public HostageFollows() : base(){}
                public HostageFollows(bool force) : base("hostage_follows", force){}

                
                
                // player who touched the hostage
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => GetInt("hostage");
                    set => SetInt("hostage", value);
                }
            }

            public class HostageHurt : GameEvent
            {
                public HostageHurt() : base(){}
                public HostageHurt(bool force) : base("hostage_hurt", force){}

                
                
                // player who hurt the hostage
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => GetInt("hostage");
                    set => SetInt("hostage", value);
                }
            }

            public class HostageKilled : GameEvent
            {
                public HostageKilled() : base(){}
                public HostageKilled(bool force) : base("hostage_killed", force){}

                
                
                // player who killed the hostage
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => GetInt("hostage");
                    set => SetInt("hostage", value);
                }
            }

            public class HostageRescued : GameEvent
            {
                public HostageRescued() : base(){}
                public HostageRescued(bool force) : base("hostage_rescued", force){}

                
                
                // player who rescued the hostage
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => GetInt("hostage");
                    set => SetInt("hostage", value);
                }

                
                // rescue site index
                public int Site 
                {
                    get => GetInt("site");
                    set => SetInt("site", value);
                }
            }

            public class HostageRescuedAll : GameEvent
            {
                public HostageRescuedAll() : base(){}
                public HostageRescuedAll(bool force) : base("hostage_rescued_all", force){}

                
            }

            public class HostageStopsFollowing : GameEvent
            {
                public HostageStopsFollowing() : base(){}
                public HostageStopsFollowing(bool force) : base("hostage_stops_following", force){}

                
                
                // player who rescued the hostage
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // hostage entity index
                public int Hostage 
                {
                    get => GetInt("hostage");
                    set => SetInt("hostage", value);
                }
            }

            public class HostnameChanged : GameEvent
            {
                public HostnameChanged() : base(){}
                public HostnameChanged(bool force) : base("hostname_changed", force){}

                
                
                
                public string Hostname 
                {
                    get => GetString("hostname");
                    set => SetString("hostname", value);
                }
            }

            public class InfernoExpire : GameEvent
            {
                public InfernoExpire() : base(){}
                public InfernoExpire(bool force) : base("inferno_expire", force){}

                
                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class InfernoExtinguish : GameEvent
            {
                public InfernoExtinguish() : base(){}
                public InfernoExtinguish(bool force) : base("inferno_extinguish", force){}

                
                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class InfernoStartburn : GameEvent
            {
                public InfernoStartburn() : base(){}
                public InfernoStartburn(bool force) : base("inferno_startburn", force){}

                
                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class InspectWeapon : GameEvent
            {
                public InspectWeapon() : base(){}
                public InspectWeapon(bool force) : base("inspect_weapon", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class InstructorCloseLesson : GameEvent
            {
                public InstructorCloseLesson() : base(){}
                public InstructorCloseLesson(bool force) : base("instructor_close_lesson", force){}

                
                
                // The player who this lesson is intended for
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // Name of the lesson to start.  Must match instructor_lesson.txt
                public string HintName 
                {
                    get => GetString("hint_name");
                    set => SetString("hint_name", value);
                }
            }

            public class InstructorServerHintCreate : GameEvent
            {
                public InstructorServerHintCreate() : base(){}
                public InstructorServerHintCreate(bool force) : base("instructor_server_hint_create", force){}

                
                
                // user ID of the player that triggered the hint
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // what to name the hint. For referencing it again later (e.g. a kill command for the hint instead of a timeout)
                public string HintName 
                {
                    get => GetString("hint_name");
                    set => SetString("hint_name", value);
                }

                
                // type name so that messages of the same type will replace each other
                public string HintReplaceKey 
                {
                    get => GetString("hint_replace_key");
                    set => SetString("hint_replace_key", value);
                }

                
                // entity id that the hint should display at
                public long HintTarget 
                {
                    get => GetInt("hint_target");
                    set => SetInt("hint_target", value);
                }

                
                // userid id of the activator
                public int HintActivatorUserid 
                {
                    get => GetInt("hint_activator_userid");
                    set => SetInt("hint_activator_userid", value);
                }

                
                // how long in seconds until the hint automatically times out, 0 = never
                public int HintTimeout 
                {
                    get => GetInt("hint_timeout");
                    set => SetInt("hint_timeout", value);
                }

                
                // the hint icon to use when the hint is onscreen. e.g. "icon_alert_red"
                public string HintIconOnscreen 
                {
                    get => GetString("hint_icon_onscreen");
                    set => SetString("hint_icon_onscreen", value);
                }

                
                // the hint icon to use when the hint is offscreen. e.g. "icon_alert"
                public string HintIconOffscreen 
                {
                    get => GetString("hint_icon_offscreen");
                    set => SetString("hint_icon_offscreen", value);
                }

                
                // the hint caption. e.g. "#ThisIsDangerous"
                public string HintCaption 
                {
                    get => GetString("hint_caption");
                    set => SetString("hint_caption", value);
                }

                
                // the hint caption that only the activator sees e.g. "#YouPushedItGood"
                public string HintActivatorCaption 
                {
                    get => GetString("hint_activator_caption");
                    set => SetString("hint_activator_caption", value);
                }

                
                // the hint color in "r,g,b" format where each component is 0-255
                public string HintColor 
                {
                    get => GetString("hint_color");
                    set => SetString("hint_color", value);
                }

                
                // how far on the z axis to offset the hint from entity origin
                public float HintIconOffset 
                {
                    get => GetFloat("hint_icon_offset");
                    set => SetFloat("hint_icon_offset", value);
                }

                
                // range before the hint is culled
                public float HintRange 
                {
                    get => GetFloat("hint_range");
                    set => SetFloat("hint_range", value);
                }

                
                // hint flags
                public long HintFlags 
                {
                    get => GetInt("hint_flags");
                    set => SetInt("hint_flags", value);
                }

                
                // bindings to use when use_binding is the onscreen icon
                public string HintBinding 
                {
                    get => GetString("hint_binding");
                    set => SetString("hint_binding", value);
                }

                
                // gamepad bindings to use when use_binding is the onscreen icon
                public string HintGamepadBinding 
                {
                    get => GetString("hint_gamepad_binding");
                    set => SetString("hint_gamepad_binding", value);
                }

                
                // if false, the hint will dissappear if the target entity is invisible
                public bool HintAllowNodrawTarget 
                {
                    get => GetBool("hint_allow_nodraw_target");
                    set => SetBool("hint_allow_nodraw_target", value);
                }

                
                // if true, the hint will not show when outside the player view
                public bool HintNooffscreen 
                {
                    get => GetBool("hint_nooffscreen");
                    set => SetBool("hint_nooffscreen", value);
                }

                
                // if true, the hint caption will show even if the hint is occluded
                public bool HintForcecaption 
                {
                    get => GetBool("hint_forcecaption");
                    set => SetBool("hint_forcecaption", value);
                }

                
                // if true, only the local player will see the hint
                public bool HintLocalPlayerOnly 
                {
                    get => GetBool("hint_local_player_only");
                    set => SetBool("hint_local_player_only", value);
                }
            }

            public class InstructorServerHintStop : GameEvent
            {
                public InstructorServerHintStop() : base(){}
                public InstructorServerHintStop(bool force) : base("instructor_server_hint_stop", force){}

                
                
                // The hint to stop. Will stop ALL hints with this name
                public string HintName 
                {
                    get => GetString("hint_name");
                    set => SetString("hint_name", value);
                }
            }

            public class InstructorStartLesson : GameEvent
            {
                public InstructorStartLesson() : base(){}
                public InstructorStartLesson(bool force) : base("instructor_start_lesson", force){}

                
                
                // The player who this lesson is intended for
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // Name of the lesson to start.  Must match instructor_lesson.txt
                public string HintName 
                {
                    get => GetString("hint_name");
                    set => SetString("hint_name", value);
                }

                
                // entity id that the hint should display at. Leave empty if controller target
                public long HintTarget 
                {
                    get => GetInt("hint_target");
                    set => SetInt("hint_target", value);
                }

                
                
                public int VrMovementType 
                {
                    get => GetInt("vr_movement_type");
                    set => SetInt("vr_movement_type", value);
                }

                
                
                public bool VrSingleController 
                {
                    get => GetBool("vr_single_controller");
                    set => SetBool("vr_single_controller", value);
                }

                
                
                public int VrControllerType 
                {
                    get => GetInt("vr_controller_type");
                    set => SetInt("vr_controller_type", value);
                }
            }

            public class InventoryUpdated : GameEvent
            {
                public InventoryUpdated() : base(){}
                public InventoryUpdated(bool force) : base("inventory_updated", force){}

                
            }

            public class ItemEquip : GameEvent
            {
                public ItemEquip() : base(){}
                public ItemEquip(bool force) : base("item_equip", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item 
                {
                    get => GetString("item");
                    set => SetString("item", value);
                }

                
                
                public long Defindex 
                {
                    get => GetInt("defindex");
                    set => SetInt("defindex", value);
                }

                
                
                public bool Canzoom 
                {
                    get => GetBool("canzoom");
                    set => SetBool("canzoom", value);
                }

                
                
                public bool Hassilencer 
                {
                    get => GetBool("hassilencer");
                    set => SetBool("hassilencer", value);
                }

                
                
                public bool Issilenced 
                {
                    get => GetBool("issilenced");
                    set => SetBool("issilenced", value);
                }

                
                
                public bool Hastracers 
                {
                    get => GetBool("hastracers");
                    set => SetBool("hastracers", value);
                }

                
                
                public int Weptype 
                {
                    get => GetInt("weptype");
                    set => SetInt("weptype", value);
                }

                
                
                public bool Ispainted 
                {
                    get => GetBool("ispainted");
                    set => SetBool("ispainted", value);
                }
            }

            public class ItemPickup : GameEvent
            {
                public ItemPickup() : base(){}
                public ItemPickup(bool force) : base("item_pickup", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item 
                {
                    get => GetString("item");
                    set => SetString("item", value);
                }

                
                
                public bool Silent 
                {
                    get => GetBool("silent");
                    set => SetBool("silent", value);
                }

                
                
                public long Defindex 
                {
                    get => GetInt("defindex");
                    set => SetInt("defindex", value);
                }
            }

            public class ItemPickupFailed : GameEvent
            {
                public ItemPickupFailed() : base(){}
                public ItemPickupFailed(bool force) : base("item_pickup_failed", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public string Item 
                {
                    get => GetString("item");
                    set => SetString("item", value);
                }

                
                
                public int Reason 
                {
                    get => GetInt("reason");
                    set => SetInt("reason", value);
                }

                
                
                public int Limit 
                {
                    get => GetInt("limit");
                    set => SetInt("limit", value);
                }
            }

            public class ItemPickupSlerp : GameEvent
            {
                public ItemPickupSlerp() : base(){}
                public ItemPickupSlerp(bool force) : base("item_pickup_slerp", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Index 
                {
                    get => GetInt("index");
                    set => SetInt("index", value);
                }

                
                
                public int Behavior 
                {
                    get => GetInt("behavior");
                    set => SetInt("behavior", value);
                }
            }

            public class ItemPurchase : GameEvent
            {
                public ItemPurchase() : base(){}
                public ItemPurchase(bool force) : base("item_purchase", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                
                public int Loadout 
                {
                    get => GetInt("loadout");
                    set => SetInt("loadout", value);
                }

                
                
                public string Weapon 
                {
                    get => GetString("weapon");
                    set => SetString("weapon", value);
                }
            }

            public class ItemRemove : GameEvent
            {
                public ItemRemove() : base(){}
                public ItemRemove(bool force) : base("item_remove", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item 
                {
                    get => GetString("item");
                    set => SetString("item", value);
                }

                
                
                public long Defindex 
                {
                    get => GetInt("defindex");
                    set => SetInt("defindex", value);
                }
            }

            public class ItemSchemaInitialized : GameEvent
            {
                public ItemSchemaInitialized() : base(){}
                public ItemSchemaInitialized(bool force) : base("item_schema_initialized", force){}

                
            }

            public class ItemsGifted : GameEvent
            {
                public ItemsGifted() : base(){}
                public ItemsGifted(bool force) : base("items_gifted", force){}

                
                
                // entity used by player
                public int Player 
                {
                    get => GetInt("player");
                    set => SetInt("player", value);
                }

                
                
                public long Itemdef 
                {
                    get => GetInt("itemdef");
                    set => SetInt("itemdef", value);
                }

                
                
                public int Numgifts 
                {
                    get => GetInt("numgifts");
                    set => SetInt("numgifts", value);
                }

                
                
                public long Giftidx 
                {
                    get => GetInt("giftidx");
                    set => SetInt("giftidx", value);
                }

                
                
                public long Accountid 
                {
                    get => GetInt("accountid");
                    set => SetInt("accountid", value);
                }
            }

            public class JointeamFailed : GameEvent
            {
                public JointeamFailed() : base(){}
                public JointeamFailed(bool force) : base("jointeam_failed", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // 0 = team_full
                public int Reason 
                {
                    get => GetInt("reason");
                    set => SetInt("reason", value);
                }
            }

            public class LocalPlayerControllerTeam : GameEvent
            {
                public LocalPlayerControllerTeam() : base(){}
                public LocalPlayerControllerTeam(bool force) : base("local_player_controller_team", force){}

                
            }

            public class LocalPlayerPawnChanged : GameEvent
            {
                public LocalPlayerPawnChanged() : base(){}
                public LocalPlayerPawnChanged(bool force) : base("local_player_pawn_changed", force){}

                
            }

            public class LocalPlayerTeam : GameEvent
            {
                public LocalPlayerTeam() : base(){}
                public LocalPlayerTeam(bool force) : base("local_player_team", force){}

                
            }

            public class LootCrateOpened : GameEvent
            {
                public LootCrateOpened() : base(){}
                public LootCrateOpened(bool force) : base("loot_crate_opened", force){}

                
                
                // player entindex
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // type of crate (metal, wood, or paradrop)
                public string Type 
                {
                    get => GetString("type");
                    set => SetString("type", value);
                }
            }

            public class LootCrateVisible : GameEvent
            {
                public LootCrateVisible() : base(){}
                public LootCrateVisible(bool force) : base("loot_crate_visible", force){}

                
                
                // player entindex
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // crate entindex
                public int Subject 
                {
                    get => GetInt("subject");
                    set => SetInt("subject", value);
                }

                
                // type of crate (metal, wood, or paradrop)
                public string Type 
                {
                    get => GetString("type");
                    set => SetString("type", value);
                }
            }

            public class MapShutdown : GameEvent
            {
                public MapShutdown() : base(){}
                public MapShutdown(bool force) : base("map_shutdown", force){}

                
            }

            public class MapTransition : GameEvent
            {
                public MapTransition() : base(){}
                public MapTransition(bool force) : base("map_transition", force){}

                
            }

            public class MatchEndConditions : GameEvent
            {
                public MatchEndConditions() : base(){}
                public MatchEndConditions(bool force) : base("match_end_conditions", force){}

                
                
                
                public long Frags 
                {
                    get => GetInt("frags");
                    set => SetInt("frags", value);
                }

                
                
                public long MaxRounds 
                {
                    get => GetInt("max_rounds");
                    set => SetInt("max_rounds", value);
                }

                
                
                public long WinRounds 
                {
                    get => GetInt("win_rounds");
                    set => SetInt("win_rounds", value);
                }

                
                
                public long Time 
                {
                    get => GetInt("time");
                    set => SetInt("time", value);
                }
            }

            public class MaterialDefaultComplete : GameEvent
            {
                public MaterialDefaultComplete() : base(){}
                public MaterialDefaultComplete(bool force) : base("material_default_complete", force){}

                
            }

            public class MbInputLockCancel : GameEvent
            {
                public MbInputLockCancel() : base(){}
                public MbInputLockCancel(bool force) : base("mb_input_lock_cancel", force){}

                
            }

            public class MbInputLockSuccess : GameEvent
            {
                public MbInputLockSuccess() : base(){}
                public MbInputLockSuccess(bool force) : base("mb_input_lock_success", force){}

                
            }

            public class MolotovDetonate : GameEvent
            {
                public MolotovDetonate() : base(){}
                public MolotovDetonate(bool force) : base("molotov_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class NavBlocked : GameEvent
            {
                public NavBlocked() : base(){}
                public NavBlocked(bool force) : base("nav_blocked", force){}

                
                
                
                public long Area 
                {
                    get => GetInt("area");
                    set => SetInt("area", value);
                }

                
                
                public bool Blocked 
                {
                    get => GetBool("blocked");
                    set => SetBool("blocked", value);
                }
            }

            public class NavGenerate : GameEvent
            {
                public NavGenerate() : base(){}
                public NavGenerate(bool force) : base("nav_generate", force){}

                
            }

            public class NextlevelChanged : GameEvent
            {
                public NextlevelChanged() : base(){}
                public NextlevelChanged(bool force) : base("nextlevel_changed", force){}

                
                
                
                public string Nextlevel 
                {
                    get => GetString("nextlevel");
                    set => SetString("nextlevel", value);
                }

                
                
                public string Mapgroup 
                {
                    get => GetString("mapgroup");
                    set => SetString("mapgroup", value);
                }

                
                
                public string Skirmishmode 
                {
                    get => GetString("skirmishmode");
                    set => SetString("skirmishmode", value);
                }
            }

            public class OpenCrateInstr : GameEvent
            {
                public OpenCrateInstr() : base(){}
                public OpenCrateInstr(bool force) : base("open_crate_instr", force){}

                
                
                // player entindex
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // crate entindex
                public int Subject 
                {
                    get => GetInt("subject");
                    set => SetInt("subject", value);
                }

                
                // type of crate (metal, wood, or paradrop)
                public string Type 
                {
                    get => GetString("type");
                    set => SetString("type", value);
                }
            }

            public class OtherDeath : GameEvent
            {
                public OtherDeath() : base(){}
                public OtherDeath(bool force) : base("other_death", force){}

                
                
                // other entity ID who died
                public int Otherid 
                {
                    get => GetInt("otherid");
                    set => SetInt("otherid", value);
                }

                
                // other entity type
                public string Othertype 
                {
                    get => GetString("othertype");
                    set => SetString("othertype", value);
                }

                
                // user ID who killed
                public int Attacker 
                {
                    get => GetInt("attacker");
                    set => SetInt("attacker", value);
                }

                
                // weapon name killer used
                public string Weapon 
                {
                    get => GetString("weapon");
                    set => SetString("weapon", value);
                }

                
                // inventory item id of weapon killer used
                public string WeaponItemid 
                {
                    get => GetString("weapon_itemid");
                    set => SetString("weapon_itemid", value);
                }

                
                // faux item id of weapon killer used
                public string WeaponFauxitemid 
                {
                    get => GetString("weapon_fauxitemid");
                    set => SetString("weapon_fauxitemid", value);
                }

                
                
                public string WeaponOriginalownerXuid 
                {
                    get => GetString("weapon_originalowner_xuid");
                    set => SetString("weapon_originalowner_xuid", value);
                }

                
                // singals a headshot
                public bool Headshot 
                {
                    get => GetBool("headshot");
                    set => SetBool("headshot", value);
                }

                
                // number of objects shot penetrated before killing target
                public int Penetrated 
                {
                    get => GetInt("penetrated");
                    set => SetInt("penetrated", value);
                }

                
                // kill happened without a scope, used for death notice icon
                public bool Noscope 
                {
                    get => GetBool("noscope");
                    set => SetBool("noscope", value);
                }

                
                // hitscan weapon went through smoke grenade
                public bool Thrusmoke 
                {
                    get => GetBool("thrusmoke");
                    set => SetBool("thrusmoke", value);
                }

                
                // attacker was blind from flashbang
                public bool Attackerblind 
                {
                    get => GetBool("attackerblind");
                    set => SetBool("attackerblind", value);
                }
            }

            public class ParachuteDeploy : GameEvent
            {
                public ParachuteDeploy() : base(){}
                public ParachuteDeploy(bool force) : base("parachute_deploy", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class ParachutePickup : GameEvent
            {
                public ParachutePickup() : base(){}
                public ParachutePickup(bool force) : base("parachute_pickup", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class PhysgunPickup : GameEvent
            {
                public PhysgunPickup() : base(){}
                public PhysgunPickup(bool force) : base("physgun_pickup", force){}

                
                
                // entity picked up
                public IntPtr Target 
                {
                    get => GetInt("target");
                    set => SetInt("target", value);
                }
            }

            public class PlayerActivate : GameEvent
            {
                public PlayerActivate() : base(){}
                public PlayerActivate(bool force) : base("player_activate", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class PlayerAvengedTeammate : GameEvent
            {
                public PlayerAvengedTeammate() : base(){}
                public PlayerAvengedTeammate(bool force) : base("player_avenged_teammate", force){}

                
                
                
                public int AvengerId 
                {
                    get => GetInt("avenger_id");
                    set => SetInt("avenger_id", value);
                }

                
                
                public int AvengedPlayerId 
                {
                    get => GetInt("avenged_player_id");
                    set => SetInt("avenged_player_id", value);
                }
            }

            public class PlayerBlind : GameEvent
            {
                public PlayerBlind() : base(){}
                public PlayerBlind(bool force) : base("player_blind", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // user ID who threw the flash
                public int Attacker 
                {
                    get => GetInt("attacker");
                    set => SetInt("attacker", value);
                }

                
                // the flashbang going off
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float BlindDuration 
                {
                    get => GetFloat("blind_duration");
                    set => SetFloat("blind_duration", value);
                }
            }

            public class PlayerChangename : GameEvent
            {
                public PlayerChangename() : base(){}
                public PlayerChangename(bool force) : base("player_changename", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // players old (current) name
                public string Oldname 
                {
                    get => GetString("oldname");
                    set => SetString("oldname", value);
                }

                
                // players new name
                public string Newname 
                {
                    get => GetString("newname");
                    set => SetString("newname", value);
                }
            }

            public class PlayerChat : GameEvent
            {
                public PlayerChat() : base(){}
                public PlayerChat(bool force) : base("player_chat", force){}

                
                
                // true if team only chat
                public bool Teamonly 
                {
                    get => GetBool("teamonly");
                    set => SetBool("teamonly", value);
                }

                
                // chatting player
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // chat text
                public string Text 
                {
                    get => GetString("text");
                    set => SetString("text", value);
                }
            }

            public class PlayerConnect : GameEvent
            {
                public PlayerConnect() : base(){}
                public PlayerConnect(bool force) : base("player_connect", force){}

                
                
                // player name
                public string Name 
                {
                    get => GetString("name");
                    set => SetString("name", value);
                }

                
                // user ID on server (unique on server)
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // player network (i.e steam) id
                public string Networkid 
                {
                    get => GetString("networkid");
                    set => SetString("networkid", value);
                }

                
                // steam id
                public ulong Xuid 
                {
                    get => GetUint64("xuid");
                    set => SetUint64("xuid", value);
                }

                
                // ip:port
                public string Address 
                {
                    get => GetString("address");
                    set => SetString("address", value);
                }

                
                
                public bool Bot 
                {
                    get => GetBool("bot");
                    set => SetBool("bot", value);
                }
            }

            public class PlayerConnectFull : GameEvent
            {
                public PlayerConnectFull() : base(){}
                public PlayerConnectFull(bool force) : base("player_connect_full", force){}

                
                
                // user ID on server (unique on server)
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class PlayerDeath : GameEvent
            {
                public PlayerDeath() : base(){}
                public PlayerDeath(bool force) : base("player_death", force){}

                
                
                // user who died
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // player who killed
                public int Attacker 
                {
                    get => GetInt("attacker");
                    set => SetInt("attacker", value);
                }

                
                // player who assisted in the kill
                public int Assister 
                {
                    get => GetInt("assister");
                    set => SetInt("assister", value);
                }

                
                // assister helped with a flash
                public bool Assistedflash 
                {
                    get => GetBool("assistedflash");
                    set => SetBool("assistedflash", value);
                }

                
                // weapon name killer used
                public string Weapon 
                {
                    get => GetString("weapon");
                    set => SetString("weapon", value);
                }

                
                // inventory item id of weapon killer used
                public string WeaponItemid 
                {
                    get => GetString("weapon_itemid");
                    set => SetString("weapon_itemid", value);
                }

                
                // faux item id of weapon killer used
                public string WeaponFauxitemid 
                {
                    get => GetString("weapon_fauxitemid");
                    set => SetString("weapon_fauxitemid", value);
                }

                
                
                public string WeaponOriginalownerXuid 
                {
                    get => GetString("weapon_originalowner_xuid");
                    set => SetString("weapon_originalowner_xuid", value);
                }

                
                // singals a headshot
                public bool Headshot 
                {
                    get => GetBool("headshot");
                    set => SetBool("headshot", value);
                }

                
                // did killer dominate victim with this kill
                public int Dominated 
                {
                    get => GetInt("dominated");
                    set => SetInt("dominated", value);
                }

                
                // did killer get revenge on victim with this kill
                public int Revenge 
                {
                    get => GetInt("revenge");
                    set => SetInt("revenge", value);
                }

                
                // is the kill resulting in squad wipe
                public int Wipe 
                {
                    get => GetInt("wipe");
                    set => SetInt("wipe", value);
                }

                
                // number of objects shot penetrated before killing target
                public int Penetrated 
                {
                    get => GetInt("penetrated");
                    set => SetInt("penetrated", value);
                }

                
                // if replay data is unavailable, this will be present and set to false
                public bool Noreplay 
                {
                    get => GetBool("noreplay");
                    set => SetBool("noreplay", value);
                }

                
                // kill happened without a scope, used for death notice icon
                public bool Noscope 
                {
                    get => GetBool("noscope");
                    set => SetBool("noscope", value);
                }

                
                // hitscan weapon went through smoke grenade
                public bool Thrusmoke 
                {
                    get => GetBool("thrusmoke");
                    set => SetBool("thrusmoke", value);
                }

                
                // attacker was blind from flashbang
                public bool Attackerblind 
                {
                    get => GetBool("attackerblind");
                    set => SetBool("attackerblind", value);
                }

                
                // distance to victim in meters
                public float Distance 
                {
                    get => GetFloat("distance");
                    set => SetFloat("distance", value);
                }

                
                // damage done to health
                public int DmgHealth 
                {
                    get => GetInt("dmg_health");
                    set => SetInt("dmg_health", value);
                }

                
                // damage done to armor
                public int DmgArmor 
                {
                    get => GetInt("dmg_armor");
                    set => SetInt("dmg_armor", value);
                }

                
                // hitgroup that was damaged
                public int Hitgroup 
                {
                    get => GetInt("hitgroup");
                    set => SetInt("hitgroup", value);
                }
            }

            public class PlayerDecal : GameEvent
            {
                public PlayerDecal() : base(){}
                public PlayerDecal(bool force) : base("player_decal", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class PlayerDisconnect : GameEvent
            {
                public PlayerDisconnect() : base(){}
                public PlayerDisconnect(bool force) : base("player_disconnect", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // see networkdisconnect enum protobuf
                public int Reason 
                {
                    get => GetInt("reason");
                    set => SetInt("reason", value);
                }

                
                // player name
                public string Name 
                {
                    get => GetString("name");
                    set => SetString("name", value);
                }

                
                // player network (i.e steam) id
                public string Networkid 
                {
                    get => GetString("networkid");
                    set => SetString("networkid", value);
                }

                
                // steam id
                public ulong Xuid 
                {
                    get => GetUint64("xuid");
                    set => SetUint64("xuid", value);
                }

                
                
                public int Playerid 
                {
                    get => GetInt("PlayerID");
                    set => SetInt("PlayerID", value);
                }
            }

            public class PlayerFalldamage : GameEvent
            {
                public PlayerFalldamage() : base(){}
                public PlayerFalldamage(bool force) : base("player_falldamage", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public float Damage 
                {
                    get => GetFloat("damage");
                    set => SetFloat("damage", value);
                }
            }

            public class PlayerFootstep : GameEvent
            {
                public PlayerFootstep() : base(){}
                public PlayerFootstep(bool force) : base("player_footstep", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class PlayerFullUpdate : GameEvent
            {
                public PlayerFullUpdate() : base(){}
                public PlayerFullUpdate(bool force) : base("player_full_update", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // Number of this full update
                public int Count 
                {
                    get => GetInt("count");
                    set => SetInt("count", value);
                }
            }

            public class PlayerGivenC4 : GameEvent
            {
                public PlayerGivenC4() : base(){}
                public PlayerGivenC4(bool force) : base("player_given_c4", force){}

                
                
                // user ID who received the c4
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class PlayerHintmessage : GameEvent
            {
                public PlayerHintmessage() : base(){}
                public PlayerHintmessage(bool force) : base("player_hintmessage", force){}

                
                
                // localizable string of a hint
                public string Hintmessage 
                {
                    get => GetString("hintmessage");
                    set => SetString("hintmessage", value);
                }
            }

            public class PlayerHurt : GameEvent
            {
                public PlayerHurt() : base(){}
                public PlayerHurt(bool force) : base("player_hurt", force){}

                
                
                // player index who was hurt
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // player index who attacked
                public int Attacker 
                {
                    get => GetInt("attacker");
                    set => SetInt("attacker", value);
                }

                
                // remaining health points
                public int Health 
                {
                    get => GetInt("health");
                    set => SetInt("health", value);
                }

                
                // remaining armor points
                public int Armor 
                {
                    get => GetInt("armor");
                    set => SetInt("armor", value);
                }

                
                // weapon name attacker used, if not the world
                public string Weapon 
                {
                    get => GetString("weapon");
                    set => SetString("weapon", value);
                }

                
                // damage done to health
                public int DmgHealth 
                {
                    get => GetInt("dmg_health");
                    set => SetInt("dmg_health", value);
                }

                
                // damage done to armor
                public int DmgArmor 
                {
                    get => GetInt("dmg_armor");
                    set => SetInt("dmg_armor", value);
                }

                
                // hitgroup that was damaged
                public int Hitgroup 
                {
                    get => GetInt("hitgroup");
                    set => SetInt("hitgroup", value);
                }
            }

            public class PlayerInfo : GameEvent
            {
                public PlayerInfo() : base(){}
                public PlayerInfo(bool force) : base("player_info", force){}

                
                
                // player name
                public string Name 
                {
                    get => GetString("name");
                    set => SetString("name", value);
                }

                
                // user ID on server (unique on server)
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // player network (i.e steam) id
                public ulong Steamid 
                {
                    get => GetUint64("steamid");
                    set => SetUint64("steamid", value);
                }

                
                // true if player is a AI bot
                public bool Bot 
                {
                    get => GetBool("bot");
                    set => SetBool("bot", value);
                }
            }

            public class PlayerJump : GameEvent
            {
                public PlayerJump() : base(){}
                public PlayerJump(bool force) : base("player_jump", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class PlayerPing : GameEvent
            {
                public PlayerPing() : base(){}
                public PlayerPing(bool force) : base("player_ping", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }

                
                
                public bool Urgent 
                {
                    get => GetBool("urgent");
                    set => SetBool("urgent", value);
                }
            }

            public class PlayerPingStop : GameEvent
            {
                public PlayerPingStop() : base(){}
                public PlayerPingStop(bool force) : base("player_ping_stop", force){}

                
                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }
            }

            public class PlayerRadio : GameEvent
            {
                public PlayerRadio() : base(){}
                public PlayerRadio(bool force) : base("player_radio", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Slot 
                {
                    get => GetInt("slot");
                    set => SetInt("slot", value);
                }
            }

            public class PlayerResetVote : GameEvent
            {
                public PlayerResetVote() : base(){}
                public PlayerResetVote(bool force) : base("player_reset_vote", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public bool Vote 
                {
                    get => GetBool("vote");
                    set => SetBool("vote", value);
                }
            }

            public class PlayerScore : GameEvent
            {
                public PlayerScore() : base(){}
                public PlayerScore(bool force) : base("player_score", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // # of kills
                public int Kills 
                {
                    get => GetInt("kills");
                    set => SetInt("kills", value);
                }

                
                // # of deaths
                public int Deaths 
                {
                    get => GetInt("deaths");
                    set => SetInt("deaths", value);
                }

                
                // total game score
                public int Score 
                {
                    get => GetInt("score");
                    set => SetInt("score", value);
                }
            }

            public class PlayerShoot : GameEvent
            {
                public PlayerShoot() : base(){}
                public PlayerShoot(bool force) : base("player_shoot", force){}

                
                
                // user ID on server
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // weapon ID
                public int Weapon 
                {
                    get => GetInt("weapon");
                    set => SetInt("weapon", value);
                }

                
                // weapon mode
                public int Mode 
                {
                    get => GetInt("mode");
                    set => SetInt("mode", value);
                }
            }

            public class PlayerSound : GameEvent
            {
                public PlayerSound() : base(){}
                public PlayerSound(bool force) : base("player_sound", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Radius 
                {
                    get => GetInt("radius");
                    set => SetInt("radius", value);
                }

                
                
                public float Duration 
                {
                    get => GetFloat("duration");
                    set => SetFloat("duration", value);
                }

                
                
                public bool Step 
                {
                    get => GetBool("step");
                    set => SetBool("step", value);
                }
            }

            public class PlayerSpawn : GameEvent
            {
                public PlayerSpawn() : base(){}
                public PlayerSpawn(bool force) : base("player_spawn", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class PlayerSpawned : GameEvent
            {
                public PlayerSpawned() : base(){}
                public PlayerSpawned(bool force) : base("player_spawned", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // true if restart is pending
                public bool Inrestart 
                {
                    get => GetBool("inrestart");
                    set => SetBool("inrestart", value);
                }
            }

            public class PlayerStatsUpdated : GameEvent
            {
                public PlayerStatsUpdated() : base(){}
                public PlayerStatsUpdated(bool force) : base("player_stats_updated", force){}

                
                
                
                public bool Forceupload 
                {
                    get => GetBool("forceupload");
                    set => SetBool("forceupload", value);
                }
            }

            public class PlayerTeam : GameEvent
            {
                public PlayerTeam() : base(){}
                public PlayerTeam(bool force) : base("player_team", force){}

                
                
                // player
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // team id
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                // old team id
                public int Oldteam 
                {
                    get => GetInt("oldteam");
                    set => SetInt("oldteam", value);
                }

                
                // team change because player disconnects
                public bool Disconnect 
                {
                    get => GetBool("disconnect");
                    set => SetBool("disconnect", value);
                }

                
                
                public bool Silent 
                {
                    get => GetBool("silent");
                    set => SetBool("silent", value);
                }

                
                // true if player is a bot
                public bool Isbot 
                {
                    get => GetBool("isbot");
                    set => SetBool("isbot", value);
                }
            }

            public class RagdollDissolved : GameEvent
            {
                public RagdollDissolved() : base(){}
                public RagdollDissolved(bool force) : base("ragdoll_dissolved", force){}

                
                
                
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }
            }

            public class ReadGameTitledata : GameEvent
            {
                public ReadGameTitledata() : base(){}
                public ReadGameTitledata(bool force) : base("read_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => GetInt("controllerId");
                    set => SetInt("controllerId", value);
                }
            }

            public class RepostXboxAchievements : GameEvent
            {
                public RepostXboxAchievements() : base(){}
                public RepostXboxAchievements(bool force) : base("repost_xbox_achievements", force){}

                
                
                // splitscreen ID
                public int Splitscreenplayer 
                {
                    get => GetInt("splitscreenplayer");
                    set => SetInt("splitscreenplayer", value);
                }
            }

            public class ResetGameTitledata : GameEvent
            {
                public ResetGameTitledata() : base(){}
                public ResetGameTitledata(bool force) : base("reset_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => GetInt("controllerId");
                    set => SetInt("controllerId", value);
                }
            }

            public class RoundAnnounceFinal : GameEvent
            {
                public RoundAnnounceFinal() : base(){}
                public RoundAnnounceFinal(bool force) : base("round_announce_final", force){}

                
            }

            public class RoundAnnounceLastRoundHalf : GameEvent
            {
                public RoundAnnounceLastRoundHalf() : base(){}
                public RoundAnnounceLastRoundHalf(bool force) : base("round_announce_last_round_half", force){}

                
            }

            public class RoundAnnounceMatchPoint : GameEvent
            {
                public RoundAnnounceMatchPoint() : base(){}
                public RoundAnnounceMatchPoint(bool force) : base("round_announce_match_point", force){}

                
            }

            public class RoundAnnounceMatchStart : GameEvent
            {
                public RoundAnnounceMatchStart() : base(){}
                public RoundAnnounceMatchStart(bool force) : base("round_announce_match_start", force){}

                
            }

            public class RoundAnnounceWarmup : GameEvent
            {
                public RoundAnnounceWarmup() : base(){}
                public RoundAnnounceWarmup(bool force) : base("round_announce_warmup", force){}

                
            }

            public class RoundEnd : GameEvent
            {
                public RoundEnd() : base(){}
                public RoundEnd(bool force) : base("round_end", force){}

                
                
                // winner team/user i
                public int Winner 
                {
                    get => GetInt("winner");
                    set => SetInt("winner", value);
                }

                
                // reson why team won
                public int Reason 
                {
                    get => GetInt("reason");
                    set => SetInt("reason", value);
                }

                
                // end round message
                public string Message 
                {
                    get => GetString("message");
                    set => SetString("message", value);
                }

                
                // server-generated legacy value
                public int Legacy 
                {
                    get => GetInt("legacy");
                    set => SetInt("legacy", value);
                }

                
                // total number of players alive at the end of round, used for statistics gathering, computed on the server in the event client is in replay when receiving this message
                public int PlayerCount 
                {
                    get => GetInt("player_count");
                    set => SetInt("player_count", value);
                }

                
                // if set, don't play round end music, because action is still on-going
                public int Nomusic 
                {
                    get => GetInt("nomusic");
                    set => SetInt("nomusic", value);
                }
            }

            public class RoundEndUploadStats : GameEvent
            {
                public RoundEndUploadStats() : base(){}
                public RoundEndUploadStats(bool force) : base("round_end_upload_stats", force){}

                
            }

            public class RoundFreezeEnd : GameEvent
            {
                public RoundFreezeEnd() : base(){}
                public RoundFreezeEnd(bool force) : base("round_freeze_end", force){}

                
            }

            public class RoundMvp : GameEvent
            {
                public RoundMvp() : base(){}
                public RoundMvp(bool force) : base("round_mvp", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Reason 
                {
                    get => GetInt("reason");
                    set => SetInt("reason", value);
                }

                
                
                public long Value 
                {
                    get => GetInt("value");
                    set => SetInt("value", value);
                }

                
                
                public long Musickitmvps 
                {
                    get => GetInt("musickitmvps");
                    set => SetInt("musickitmvps", value);
                }

                
                
                public int Nomusic 
                {
                    get => GetInt("nomusic");
                    set => SetInt("nomusic", value);
                }

                
                
                public long Musickitid 
                {
                    get => GetInt("musickitid");
                    set => SetInt("musickitid", value);
                }
            }

            public class RoundOfficiallyEnded : GameEvent
            {
                public RoundOfficiallyEnded() : base(){}
                public RoundOfficiallyEnded(bool force) : base("round_officially_ended", force){}

                
            }

            public class RoundPoststart : GameEvent
            {
                public RoundPoststart() : base(){}
                public RoundPoststart(bool force) : base("round_poststart", force){}

                
            }

            public class RoundPrestart : GameEvent
            {
                public RoundPrestart() : base(){}
                public RoundPrestart(bool force) : base("round_prestart", force){}

                
            }

            public class RoundStart : GameEvent
            {
                public RoundStart() : base(){}
                public RoundStart(bool force) : base("round_start", force){}

                
                
                // round time limit in seconds
                public long Timelimit 
                {
                    get => GetInt("timelimit");
                    set => SetInt("timelimit", value);
                }

                
                // frag limit in seconds
                public long Fraglimit 
                {
                    get => GetInt("fraglimit");
                    set => SetInt("fraglimit", value);
                }

                
                // round objective
                public string Objective 
                {
                    get => GetString("objective");
                    set => SetString("objective", value);
                }
            }

            public class RoundStartPostNav : GameEvent
            {
                public RoundStartPostNav() : base(){}
                public RoundStartPostNav(bool force) : base("round_start_post_nav", force){}

                
            }

            public class RoundStartPreEntity : GameEvent
            {
                public RoundStartPreEntity() : base(){}
                public RoundStartPreEntity(bool force) : base("round_start_pre_entity", force){}

                
            }

            public class RoundTimeWarning : GameEvent
            {
                public RoundTimeWarning() : base(){}
                public RoundTimeWarning(bool force) : base("round_time_warning", force){}

                
            }

            public class SeasoncoinLevelup : GameEvent
            {
                public SeasoncoinLevelup() : base(){}
                public SeasoncoinLevelup(bool force) : base("seasoncoin_levelup", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Category 
                {
                    get => GetInt("category");
                    set => SetInt("category", value);
                }

                
                
                public int Rank 
                {
                    get => GetInt("rank");
                    set => SetInt("rank", value);
                }
            }

            public class ServerCvar : GameEvent
            {
                public ServerCvar() : base(){}
                public ServerCvar(bool force) : base("server_cvar", force){}

                
                
                // cvar name, eg "mp_roundtime"
                public string Cvarname 
                {
                    get => GetString("cvarname");
                    set => SetString("cvarname", value);
                }

                
                // new cvar value
                public string Cvarvalue 
                {
                    get => GetString("cvarvalue");
                    set => SetString("cvarvalue", value);
                }
            }

            public class ServerMessage : GameEvent
            {
                public ServerMessage() : base(){}
                public ServerMessage(bool force) : base("server_message", force){}

                
                
                // the message text
                public string Text 
                {
                    get => GetString("text");
                    set => SetString("text", value);
                }
            }

            public class ServerPreShutdown : GameEvent
            {
                public ServerPreShutdown() : base(){}
                public ServerPreShutdown(bool force) : base("server_pre_shutdown", force){}

                
                
                // reason why server is about to be shut down
                public string Reason 
                {
                    get => GetString("reason");
                    set => SetString("reason", value);
                }
            }

            public class ServerShutdown : GameEvent
            {
                public ServerShutdown() : base(){}
                public ServerShutdown(bool force) : base("server_shutdown", force){}

                
                
                // reason why server was shut down
                public string Reason 
                {
                    get => GetString("reason");
                    set => SetString("reason", value);
                }
            }

            public class ServerSpawn : GameEvent
            {
                public ServerSpawn() : base(){}
                public ServerSpawn(bool force) : base("server_spawn", force){}

                
                
                // public host name
                public string Hostname 
                {
                    get => GetString("hostname");
                    set => SetString("hostname", value);
                }

                
                // hostame, IP or DNS name
                public string Address 
                {
                    get => GetString("address");
                    set => SetString("address", value);
                }

                
                // server port
                public int Port 
                {
                    get => GetInt("port");
                    set => SetInt("port", value);
                }

                
                // game dir
                public string Game 
                {
                    get => GetString("game");
                    set => SetString("game", value);
                }

                
                // map name
                public string Mapname 
                {
                    get => GetString("mapname");
                    set => SetString("mapname", value);
                }

                
                // addon name
                public string Addonname 
                {
                    get => GetString("addonname");
                    set => SetString("addonname", value);
                }

                
                // max players
                public long Maxplayers 
                {
                    get => GetInt("maxplayers");
                    set => SetInt("maxplayers", value);
                }

                
                // WIN32, LINUX
                public string Os 
                {
                    get => GetString("os");
                    set => SetString("os", value);
                }

                
                // true if dedicated server
                public bool Dedicated 
                {
                    get => GetBool("dedicated");
                    set => SetBool("dedicated", value);
                }

                
                // true if password protected
                public bool Password 
                {
                    get => GetBool("password");
                    set => SetBool("password", value);
                }
            }

            public class SetInstructorGroupEnabled : GameEvent
            {
                public SetInstructorGroupEnabled() : base(){}
                public SetInstructorGroupEnabled(bool force) : base("set_instructor_group_enabled", force){}

                
                
                
                public string Group 
                {
                    get => GetString("group");
                    set => SetString("group", value);
                }

                
                
                public int Enabled 
                {
                    get => GetInt("enabled");
                    set => SetInt("enabled", value);
                }
            }

            public class Sfuievent : GameEvent
            {
                public Sfuievent() : base(){}
                public Sfuievent(bool force) : base("sfuievent", force){}

                
                
                
                public string Action 
                {
                    get => GetString("action");
                    set => SetString("action", value);
                }

                
                
                public string Data 
                {
                    get => GetString("data");
                    set => SetString("data", value);
                }

                
                
                public int Slot 
                {
                    get => GetInt("slot");
                    set => SetInt("slot", value);
                }
            }

            public class ShowDeathpanel : GameEvent
            {
                public ShowDeathpanel() : base(){}
                public ShowDeathpanel(bool force) : base("show_deathpanel", force){}

                
                
                // endindex of the one who was killed
                public int Victim 
                {
                    get => GetInt("victim");
                    set => SetInt("victim", value);
                }

                
                // entindex of the killer entity
                public IntPtr Killer 
                {
                    get => GetInt("killer");
                    set => SetInt("killer", value);
                }

                
                
                public int KillerController 
                {
                    get => GetInt("killer_controller");
                    set => SetInt("killer_controller", value);
                }

                
                
                public int HitsTaken 
                {
                    get => GetInt("hits_taken");
                    set => SetInt("hits_taken", value);
                }

                
                
                public int DamageTaken 
                {
                    get => GetInt("damage_taken");
                    set => SetInt("damage_taken", value);
                }

                
                
                public int HitsGiven 
                {
                    get => GetInt("hits_given");
                    set => SetInt("hits_given", value);
                }

                
                
                public int DamageGiven 
                {
                    get => GetInt("damage_given");
                    set => SetInt("damage_given", value);
                }
            }

            public class ShowSurvivalRespawnStatus : GameEvent
            {
                public ShowSurvivalRespawnStatus() : base(){}
                public ShowSurvivalRespawnStatus(bool force) : base("show_survival_respawn_status", force){}

                
                
                
                public string LocToken 
                {
                    get => GetString("loc_token");
                    set => SetString("loc_token", value);
                }

                
                
                public long Duration 
                {
                    get => GetInt("duration");
                    set => SetInt("duration", value);
                }

                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class SilencerDetach : GameEvent
            {
                public SilencerDetach() : base(){}
                public SilencerDetach(bool force) : base("silencer_detach", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class SilencerOff : GameEvent
            {
                public SilencerOff() : base(){}
                public SilencerOff(bool force) : base("silencer_off", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class SilencerOn : GameEvent
            {
                public SilencerOn() : base(){}
                public SilencerOn(bool force) : base("silencer_on", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class SmokeBeaconParadrop : GameEvent
            {
                public SmokeBeaconParadrop() : base(){}
                public SmokeBeaconParadrop(bool force) : base("smoke_beacon_paradrop", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Paradrop 
                {
                    get => GetInt("paradrop");
                    set => SetInt("paradrop", value);
                }
            }

            public class SmokegrenadeDetonate : GameEvent
            {
                public SmokegrenadeDetonate() : base(){}
                public SmokegrenadeDetonate(bool force) : base("smokegrenade_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class SmokegrenadeExpired : GameEvent
            {
                public SmokegrenadeExpired() : base(){}
                public SmokegrenadeExpired(bool force) : base("smokegrenade_expired", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class SpecModeUpdated : GameEvent
            {
                public SpecModeUpdated() : base(){}
                public SpecModeUpdated(bool force) : base("spec_mode_updated", force){}

                
                
                // entindex of the player
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class SpecTargetUpdated : GameEvent
            {
                public SpecTargetUpdated() : base(){}
                public SpecTargetUpdated(bool force) : base("spec_target_updated", force){}

                
                
                // spectating player
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // ehandle of the target
                public IntPtr Target 
                {
                    get => GetInt("target");
                    set => SetInt("target", value);
                }
            }

            public class StartHalftime : GameEvent
            {
                public StartHalftime() : base(){}
                public StartHalftime(bool force) : base("start_halftime", force){}

                
            }

            public class StartVote : GameEvent
            {
                public StartVote() : base(){}
                public StartVote(bool force) : base("start_vote", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Type 
                {
                    get => GetInt("type");
                    set => SetInt("type", value);
                }

                
                
                public int VoteParameter 
                {
                    get => GetInt("vote_parameter");
                    set => SetInt("vote_parameter", value);
                }
            }

            public class StorePricesheetUpdated : GameEvent
            {
                public StorePricesheetUpdated() : base(){}
                public StorePricesheetUpdated(bool force) : base("store_pricesheet_updated", force){}

                
            }

            public class SurvivalAnnouncePhase : GameEvent
            {
                public SurvivalAnnouncePhase() : base(){}
                public SurvivalAnnouncePhase(bool force) : base("survival_announce_phase", force){}

                
                
                // The phase #
                public int Phase 
                {
                    get => GetInt("phase");
                    set => SetInt("phase", value);
                }
            }

            public class SurvivalNoRespawnsFinal : GameEvent
            {
                public SurvivalNoRespawnsFinal() : base(){}
                public SurvivalNoRespawnsFinal(bool force) : base("survival_no_respawns_final", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class SurvivalNoRespawnsWarning : GameEvent
            {
                public SurvivalNoRespawnsWarning() : base(){}
                public SurvivalNoRespawnsWarning(bool force) : base("survival_no_respawns_warning", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class SurvivalParadropBreak : GameEvent
            {
                public SurvivalParadropBreak() : base(){}
                public SurvivalParadropBreak(bool force) : base("survival_paradrop_break", force){}

                
                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }
            }

            public class SurvivalParadropSpawn : GameEvent
            {
                public SurvivalParadropSpawn() : base(){}
                public SurvivalParadropSpawn(bool force) : base("survival_paradrop_spawn", force){}

                
                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }
            }

            public class SurvivalTeammateRespawn : GameEvent
            {
                public SurvivalTeammateRespawn() : base(){}
                public SurvivalTeammateRespawn(bool force) : base("survival_teammate_respawn", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class SwitchTeam : GameEvent
            {
                public SwitchTeam() : base(){}
                public SwitchTeam(bool force) : base("switch_team", force){}

                
                
                // number of active players on both T and CT
                public int Numplayers 
                {
                    get => GetInt("numPlayers");
                    set => SetInt("numPlayers", value);
                }

                
                // number of spectators
                public int Numspectators 
                {
                    get => GetInt("numSpectators");
                    set => SetInt("numSpectators", value);
                }

                
                // average rank of human players
                public int AvgRank 
                {
                    get => GetInt("avg_rank");
                    set => SetInt("avg_rank", value);
                }

                
                
                public int Numtslotsfree 
                {
                    get => GetInt("numTSlotsFree");
                    set => SetInt("numTSlotsFree", value);
                }

                
                
                public int Numctslotsfree 
                {
                    get => GetInt("numCTSlotsFree");
                    set => SetInt("numCTSlotsFree", value);
                }
            }

            public class TagrenadeDetonate : GameEvent
            {
                public TagrenadeDetonate() : base(){}
                public TagrenadeDetonate(bool force) : base("tagrenade_detonate", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }

                
                
                public float X 
                {
                    get => GetFloat("x");
                    set => SetFloat("x", value);
                }

                
                
                public float Y 
                {
                    get => GetFloat("y");
                    set => SetFloat("y", value);
                }

                
                
                public float Z 
                {
                    get => GetFloat("z");
                    set => SetFloat("z", value);
                }
            }

            public class TeamchangePending : GameEvent
            {
                public TeamchangePending() : base(){}
                public TeamchangePending(bool force) : base("teamchange_pending", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                
                public int Toteam 
                {
                    get => GetInt("toteam");
                    set => SetInt("toteam", value);
                }
            }

            public class TeamInfo : GameEvent
            {
                public TeamInfo() : base(){}
                public TeamInfo(bool force) : base("team_info", force){}

                
                
                // unique team id
                public int Teamid 
                {
                    get => GetInt("teamid");
                    set => SetInt("teamid", value);
                }

                
                // team name eg "Team Blue"
                public string Teamname 
                {
                    get => GetString("teamname");
                    set => SetString("teamname", value);
                }
            }

            public class TeamIntroEnd : GameEvent
            {
                public TeamIntroEnd() : base(){}
                public TeamIntroEnd(bool force) : base("team_intro_end", force){}

                
            }

            public class TeamIntroStart : GameEvent
            {
                public TeamIntroStart() : base(){}
                public TeamIntroStart(bool force) : base("team_intro_start", force){}

                
            }

            public class TeamplayBroadcastAudio : GameEvent
            {
                public TeamplayBroadcastAudio() : base(){}
                public TeamplayBroadcastAudio(bool force) : base("teamplay_broadcast_audio", force){}

                
                
                // unique team id
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                // name of the sound to emit
                public string Sound 
                {
                    get => GetString("sound");
                    set => SetString("sound", value);
                }
            }

            public class TeamplayRoundStart : GameEvent
            {
                public TeamplayRoundStart() : base(){}
                public TeamplayRoundStart(bool force) : base("teamplay_round_start", force){}

                
                
                // is this a full reset of the map
                public bool FullReset 
                {
                    get => GetBool("full_reset");
                    set => SetBool("full_reset", value);
                }
            }

            public class TeamScore : GameEvent
            {
                public TeamScore() : base(){}
                public TeamScore(bool force) : base("team_score", force){}

                
                
                // team id
                public int Teamid 
                {
                    get => GetInt("teamid");
                    set => SetInt("teamid", value);
                }

                
                // total team score
                public int Score 
                {
                    get => GetInt("score");
                    set => SetInt("score", value);
                }
            }

            public class TournamentReward : GameEvent
            {
                public TournamentReward() : base(){}
                public TournamentReward(bool force) : base("tournament_reward", force){}

                
                
                
                public long Defindex 
                {
                    get => GetInt("defindex");
                    set => SetInt("defindex", value);
                }

                
                
                public long Totalrewards 
                {
                    get => GetInt("totalrewards");
                    set => SetInt("totalrewards", value);
                }

                
                
                public long Accountid 
                {
                    get => GetInt("accountid");
                    set => SetInt("accountid", value);
                }
            }

            public class TrExitHintTrigger : GameEvent
            {
                public TrExitHintTrigger() : base(){}
                public TrExitHintTrigger(bool force) : base("tr_exit_hint_trigger", force){}

                
            }

            public class TrialTimeExpired : GameEvent
            {
                public TrialTimeExpired() : base(){}
                public TrialTimeExpired(bool force) : base("trial_time_expired", force){}

                
                
                // player whose time has expired
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class TrMarkBestTime : GameEvent
            {
                public TrMarkBestTime() : base(){}
                public TrMarkBestTime(bool force) : base("tr_mark_best_time", force){}

                
                
                
                public long Time 
                {
                    get => GetInt("time");
                    set => SetInt("time", value);
                }
            }

            public class TrMarkComplete : GameEvent
            {
                public TrMarkComplete() : base(){}
                public TrMarkComplete(bool force) : base("tr_mark_complete", force){}

                
                
                
                public int Complete 
                {
                    get => GetInt("complete");
                    set => SetInt("complete", value);
                }
            }

            public class TrPlayerFlashbanged : GameEvent
            {
                public TrPlayerFlashbanged() : base(){}
                public TrPlayerFlashbanged(bool force) : base("tr_player_flashbanged", force){}

                
                
                // user ID of the player banged
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class TrShowExitMsgbox : GameEvent
            {
                public TrShowExitMsgbox() : base(){}
                public TrShowExitMsgbox(bool force) : base("tr_show_exit_msgbox", force){}

                
            }

            public class TrShowFinishMsgbox : GameEvent
            {
                public TrShowFinishMsgbox() : base(){}
                public TrShowFinishMsgbox(bool force) : base("tr_show_finish_msgbox", force){}

                
            }

            public class UgcFileDownloadFinished : GameEvent
            {
                public UgcFileDownloadFinished() : base(){}
                public UgcFileDownloadFinished(bool force) : base("ugc_file_download_finished", force){}

                
                
                // id of this specific content (may be image or map)
                public ulong Hcontent 
                {
                    get => GetUint64("hcontent");
                    set => SetUint64("hcontent", value);
                }
            }

            public class UgcFileDownloadStart : GameEvent
            {
                public UgcFileDownloadStart() : base(){}
                public UgcFileDownloadStart(bool force) : base("ugc_file_download_start", force){}

                
                
                // id of this specific content (may be image or map)
                public ulong Hcontent 
                {
                    get => GetUint64("hcontent");
                    set => SetUint64("hcontent", value);
                }

                
                // id of the associated content package
                public ulong PublishedFileId 
                {
                    get => GetUint64("published_file_id");
                    set => SetUint64("published_file_id", value);
                }
            }

            public class UgcMapDownloadError : GameEvent
            {
                public UgcMapDownloadError() : base(){}
                public UgcMapDownloadError(bool force) : base("ugc_map_download_error", force){}

                
                
                
                public ulong PublishedFileId 
                {
                    get => GetUint64("published_file_id");
                    set => SetUint64("published_file_id", value);
                }

                
                
                public long ErrorCode 
                {
                    get => GetInt("error_code");
                    set => SetInt("error_code", value);
                }
            }

            public class UgcMapInfoReceived : GameEvent
            {
                public UgcMapInfoReceived() : base(){}
                public UgcMapInfoReceived(bool force) : base("ugc_map_info_received", force){}

                
                
                
                public ulong PublishedFileId 
                {
                    get => GetUint64("published_file_id");
                    set => SetUint64("published_file_id", value);
                }
            }

            public class UgcMapUnsubscribed : GameEvent
            {
                public UgcMapUnsubscribed() : base(){}
                public UgcMapUnsubscribed(bool force) : base("ugc_map_unsubscribed", force){}

                
                
                
                public ulong PublishedFileId 
                {
                    get => GetUint64("published_file_id");
                    set => SetUint64("published_file_id", value);
                }
            }

            public class UpdateMatchmakingStats : GameEvent
            {
                public UpdateMatchmakingStats() : base(){}
                public UpdateMatchmakingStats(bool force) : base("update_matchmaking_stats", force){}

                
            }

            public class UserDataDownloaded : GameEvent
            {
                public UserDataDownloaded() : base(){}
                public UserDataDownloaded(bool force) : base("user_data_downloaded", force){}

                
            }

            public class VipEscaped : GameEvent
            {
                public VipEscaped() : base(){}
                public VipEscaped(bool force) : base("vip_escaped", force){}

                
                
                // player who was the VIP
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class VipKilled : GameEvent
            {
                public VipKilled() : base(){}
                public VipKilled(bool force) : base("vip_killed", force){}

                
                
                // player who was the VIP
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // user ID who killed the VIP
                public int Attacker 
                {
                    get => GetInt("attacker");
                    set => SetInt("attacker", value);
                }
            }

            public class VoteCast : GameEvent
            {
                public VoteCast() : base(){}
                public VoteCast(bool force) : base("vote_cast", force){}

                
                
                // which option the player voted on
                public int VoteOption 
                {
                    get => GetInt("vote_option");
                    set => SetInt("vote_option", value);
                }

                
                
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                // player who voted
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class VoteCastNo : GameEvent
            {
                public VoteCastNo() : base(){}
                public VoteCastNo(bool force) : base("vote_cast_no", force){}

                
                
                
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                // entity id of the voter
                public long Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }
            }

            public class VoteCastYes : GameEvent
            {
                public VoteCastYes() : base(){}
                public VoteCastYes(bool force) : base("vote_cast_yes", force){}

                
                
                
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                // entity id of the voter
                public long Entityid 
                {
                    get => GetInt("entityid");
                    set => SetInt("entityid", value);
                }
            }

            public class VoteChanged : GameEvent
            {
                public VoteChanged() : base(){}
                public VoteChanged(bool force) : base("vote_changed", force){}

                
                
                
                public int VoteOption1 
                {
                    get => GetInt("vote_option1");
                    set => SetInt("vote_option1", value);
                }

                
                
                public int VoteOption2 
                {
                    get => GetInt("vote_option2");
                    set => SetInt("vote_option2", value);
                }

                
                
                public int VoteOption3 
                {
                    get => GetInt("vote_option3");
                    set => SetInt("vote_option3", value);
                }

                
                
                public int VoteOption4 
                {
                    get => GetInt("vote_option4");
                    set => SetInt("vote_option4", value);
                }

                
                
                public int VoteOption5 
                {
                    get => GetInt("vote_option5");
                    set => SetInt("vote_option5", value);
                }

                
                
                public int Potentialvotes 
                {
                    get => GetInt("potentialVotes");
                    set => SetInt("potentialVotes", value);
                }
            }

            public class VoteEnded : GameEvent
            {
                public VoteEnded() : base(){}
                public VoteEnded(bool force) : base("vote_ended", force){}

                
            }

            public class VoteFailed : GameEvent
            {
                public VoteFailed() : base(){}
                public VoteFailed(bool force) : base("vote_failed", force){}

                
                
                
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                // this event is reliable
                public int Reliable 
                {
                    get => GetInt("reliable");
                    set => SetInt("reliable", value);
                }
            }

            public class VoteOptions : GameEvent
            {
                public VoteOptions() : base(){}
                public VoteOptions(bool force) : base("vote_options", force){}

                
                
                // Number of options - up to MAX_VOTE_OPTIONS
                public int Count 
                {
                    get => GetInt("count");
                    set => SetInt("count", value);
                }

                
                
                public string Option1 
                {
                    get => GetString("option1");
                    set => SetString("option1", value);
                }

                
                
                public string Option2 
                {
                    get => GetString("option2");
                    set => SetString("option2", value);
                }

                
                
                public string Option3 
                {
                    get => GetString("option3");
                    set => SetString("option3", value);
                }

                
                
                public string Option4 
                {
                    get => GetString("option4");
                    set => SetString("option4", value);
                }

                
                
                public string Option5 
                {
                    get => GetString("option5");
                    set => SetString("option5", value);
                }
            }

            public class VotePassed : GameEvent
            {
                public VotePassed() : base(){}
                public VotePassed(bool force) : base("vote_passed", force){}

                
                
                
                public string Details 
                {
                    get => GetString("details");
                    set => SetString("details", value);
                }

                
                
                public string Param1 
                {
                    get => GetString("param1");
                    set => SetString("param1", value);
                }

                
                
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                // this event is reliable
                public int Reliable 
                {
                    get => GetInt("reliable");
                    set => SetInt("reliable", value);
                }
            }

            public class VoteStarted : GameEvent
            {
                public VoteStarted() : base(){}
                public VoteStarted(bool force) : base("vote_started", force){}

                
                
                
                public string Issue 
                {
                    get => GetString("issue");
                    set => SetString("issue", value);
                }

                
                
                public string Param1 
                {
                    get => GetString("param1");
                    set => SetString("param1", value);
                }

                
                
                public int Team 
                {
                    get => GetInt("team");
                    set => SetInt("team", value);
                }

                
                // entity id of the player who initiated the vote
                public long Initiator 
                {
                    get => GetInt("initiator");
                    set => SetInt("initiator", value);
                }
            }

            public class WeaponFire : GameEvent
            {
                public WeaponFire() : base(){}
                public WeaponFire(bool force) : base("weapon_fire", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // weapon name used
                public string Weapon 
                {
                    get => GetString("weapon");
                    set => SetString("weapon", value);
                }

                
                // is weapon silenced
                public bool Silenced 
                {
                    get => GetBool("silenced");
                    set => SetBool("silenced", value);
                }
            }

            public class WeaponFireOnEmpty : GameEvent
            {
                public WeaponFireOnEmpty() : base(){}
                public WeaponFireOnEmpty(bool force) : base("weapon_fire_on_empty", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // weapon name used
                public string Weapon 
                {
                    get => GetString("weapon");
                    set => SetString("weapon", value);
                }
            }

            public class WeaponhudSelection : GameEvent
            {
                public WeaponhudSelection() : base(){}
                public WeaponhudSelection(bool force) : base("weaponhud_selection", force){}

                
                
                // Player who this event applies to
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }

                
                // EWeaponHudSelectionMode (switch / pickup / drop)
                public int Mode 
                {
                    get => GetInt("mode");
                    set => SetInt("mode", value);
                }

                
                // Weapon entity index
                public long Entindex 
                {
                    get => GetInt("entindex");
                    set => SetInt("entindex", value);
                }
            }

            public class WeaponOutofammo : GameEvent
            {
                public WeaponOutofammo() : base(){}
                public WeaponOutofammo(bool force) : base("weapon_outofammo", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class WeaponReload : GameEvent
            {
                public WeaponReload() : base(){}
                public WeaponReload(bool force) : base("weapon_reload", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class WeaponZoom : GameEvent
            {
                public WeaponZoom() : base(){}
                public WeaponZoom(bool force) : base("weapon_zoom", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class WeaponZoomRifle : GameEvent
            {
                public WeaponZoomRifle() : base(){}
                public WeaponZoomRifle(bool force) : base("weapon_zoom_rifle", force){}

                
                
                
                public int Userid 
                {
                    get => GetInt("userid");
                    set => SetInt("userid", value);
                }
            }

            public class WriteGameTitledata : GameEvent
            {
                public WriteGameTitledata() : base(){}
                public WriteGameTitledata(bool force) : base("write_game_titledata", force){}

                
                
                // Controller id of user
                public int Controllerid 
                {
                    get => GetInt("controllerId");
                    set => SetInt("controllerId", value);
                }
            }

            public class WriteProfileData : GameEvent
            {
                public WriteProfileData() : base(){}
                public WriteProfileData(bool force) : base("write_profile_data", force){}

                
            }
}
