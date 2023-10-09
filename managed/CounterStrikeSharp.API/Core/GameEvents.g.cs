
using System;
using CounterStrikeSharp.API.Modules.Events;

namespace CounterStrikeSharp.API.Core
{
    
            public class AchievementEarned : GameEvent
            {
                // entindex of the player
                public int Player => GetInt("player");

                // achievement ID
                public int Achievement => GetInt("achievement");
            }

            public class AchievementEarnedLocal : GameEvent
            {
                // achievement ID
                public int Achievement => GetInt("achievement");

                // splitscreen ID
                public int Splitscreenplayer => GetInt("splitscreenplayer");
            }

            public class AchievementEvent : GameEvent
            {
                // non-localized name of achievement
                public string AchievementName => GetString("achievement_name");

                // # of steps toward achievement
                public int CurVal => GetInt("cur_val");

                // total # of steps in achievement
                public int MaxVal => GetInt("max_val");
            }

            public class AchievementInfoLoaded : GameEvent
            {
            }

            public class AchievementWriteFailed : GameEvent
            {
            }

            public class AddBulletHitMarker : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Bone => GetInt("bone");

                
                public int PosX => GetInt("pos_x");

                
                public int PosY => GetInt("pos_y");

                
                public int PosZ => GetInt("pos_z");

                
                public int AngX => GetInt("ang_x");

                
                public int AngY => GetInt("ang_y");

                
                public int AngZ => GetInt("ang_z");

                
                public int StartX => GetInt("start_x");

                
                public int StartY => GetInt("start_y");

                
                public int StartZ => GetInt("start_z");

                
                public bool Hit => GetBool("hit");
            }

            public class AddPlayerSonarIcon : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public float PosX => GetFloat("pos_x");

                
                public float PosY => GetFloat("pos_y");

                
                public float PosZ => GetFloat("pos_z");
            }

            public class AmmoPickup : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item => GetString("item");

                // the weapon entindex
                public long Index => GetInt("index");
            }

            public class AmmoRefill : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public bool Success => GetBool("success");
            }

            public class AnnouncePhaseEnd : GameEvent
            {
            }

            public class BeginNewMatch : GameEvent
            {
            }

            public class BombAbortdefuse : GameEvent
            {
                // player who was defusing
                public int Userid => GetInt("userid");
            }

            public class BombAbortplant : GameEvent
            {
                // player who is planting the bomb
                public int Userid => GetInt("userid");

                // bombsite index
                public int Site => GetInt("site");
            }

            public class BombBeep : GameEvent
            {
                // c4 entity
                public long Entindex => GetInt("entindex");
            }

            public class BombBegindefuse : GameEvent
            {
                // player who is defusing
                public int Userid => GetInt("userid");

                
                public bool Haskit => GetBool("haskit");
            }

            public class BombBeginplant : GameEvent
            {
                // player who is planting the bomb
                public int Userid => GetInt("userid");

                // bombsite index
                public int Site => GetInt("site");
            }

            public class BombDefused : GameEvent
            {
                // player who defused the bomb
                public int Userid => GetInt("userid");

                // bombsite index
                public int Site => GetInt("site");
            }

            public class BombDropped : GameEvent
            {
                // player who dropped the bomb
                public int Userid => GetInt("userid");

                
                public long Entindex => GetInt("entindex");
            }

            public class BombExploded : GameEvent
            {
                // player who planted the bomb
                public int Userid => GetInt("userid");

                // bombsite index
                public int Site => GetInt("site");
            }

            public class BombPickup : GameEvent
            {
                // player pawn who picked up the bomb
                public int Userid => GetInt("userid");
            }

            public class BombPlanted : GameEvent
            {
                // player who planted the bomb
                public int Userid => GetInt("userid");

                // bombsite index
                public int Site => GetInt("site");
            }

            public class BonusUpdated : GameEvent
            {
                
                public int Numadvanced => GetInt("numadvanced");

                
                public int Numbronze => GetInt("numbronze");

                
                public int Numsilver => GetInt("numsilver");

                
                public int Numgold => GetInt("numgold");
            }

            public class BotTakeover : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Botid => GetInt("botid");

                
                public float P => GetFloat("p");

                
                public float Y => GetFloat("y");

                
                public float R => GetFloat("r");
            }

            public class BreakBreakable : GameEvent
            {
                
                public long Entindex => GetInt("entindex");

                
                public int Userid => GetInt("userid");

                // BREAK_GLASS, BREAK_WOOD, etc
                public int Material => GetInt("material");
            }

            public class BreakProp : GameEvent
            {
                
                public long Entindex => GetInt("entindex");

                
                public int Userid => GetInt("userid");
            }

            public class BrokenBreakable : GameEvent
            {
                
                public long Entindex => GetInt("entindex");

                
                public int Userid => GetInt("userid");

                // BREAK_GLASS, BREAK_WOOD, etc
                public int Material => GetInt("material");
            }

            public class BulletFlightResolution : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int PosX => GetInt("pos_x");

                
                public int PosY => GetInt("pos_y");

                
                public int PosZ => GetInt("pos_z");

                
                public int AngX => GetInt("ang_x");

                
                public int AngY => GetInt("ang_y");

                
                public int AngZ => GetInt("ang_z");

                
                public int StartX => GetInt("start_x");

                
                public int StartY => GetInt("start_y");

                
                public int StartZ => GetInt("start_z");
            }

            public class BulletImpact : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class BuymenuClose : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class BuymenuOpen : GameEvent
            {
            }

            public class BuytimeEnded : GameEvent
            {
            }

            public class CartUpdated : GameEvent
            {
            }

            public class ChoppersIncomingWarning : GameEvent
            {
                
                public bool Global => GetBool("global");
            }

            public class ClientDisconnect : GameEvent
            {
            }

            public class ClientLoadoutChanged : GameEvent
            {
            }

            public class ClientsideLessonClosed : GameEvent
            {
                
                public string LessonName => GetString("lesson_name");
            }

            public class CsGameDisconnected : GameEvent
            {
            }

            public class CsIntermission : GameEvent
            {
            }

            public class CsMatchEndRestart : GameEvent
            {
            }

            public class CsPreRestart : GameEvent
            {
            }

            public class CsPrevNextSpectator : GameEvent
            {
                
                public bool Next => GetBool("next");
            }

            public class CsRoundFinalBeep : GameEvent
            {
            }

            public class CsRoundStartBeep : GameEvent
            {
            }

            public class CsWinPanelMatch : GameEvent
            {
            }

            public class CsWinPanelRound : GameEvent
            {
                
                public bool ShowTimerDefend => GetBool("show_timer_defend");

                
                public bool ShowTimerAttack => GetBool("show_timer_attack");

                
                public int TimerTime => GetInt("timer_time");

                // define in cs_gamerules.h
                public int FinalEvent => GetInt("final_event");

                
                public string FunfactToken => GetString("funfact_token");

                
                public int FunfactPlayer => GetInt("funfact_player");

                
                public long FunfactData1 => GetInt("funfact_data1");

                
                public long FunfactData2 => GetInt("funfact_data2");

                
                public long FunfactData3 => GetInt("funfact_data3");
            }

            public class DecoyDetonate : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class DecoyFiring : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class DecoyStarted : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class DefuserDropped : GameEvent
            {
                // defuser's entity ID
                public long Entityid => GetInt("entityid");
            }

            public class DefuserPickup : GameEvent
            {
                // defuser's entity ID
                public long Entityid => GetInt("entityid");

                // player who picked up the defuser
                public int Userid => GetInt("userid");
            }

            public class DemoSkip : GameEvent
            {
                
                public int Local => GetInt("local");

                // current playback tick
                public long PlaybackTick => GetInt("playback_tick");

                // tick we're going to
                public long SkiptoTick => GetInt("skipto_tick");

                // CSVCMsgList_UserMessages
                public int UserMessageList => GetInt("user_message_list");

                // CSVCMsgList_GameEvents
                public int DotaHeroChaseList => GetInt("dota_hero_chase_list");
            }

            public class DemoStart : GameEvent
            {
                
                public int Local => GetInt("local");

                // CSVCMsgList_GameEvents that are combat log events
                public int DotaCombatlogList => GetInt("dota_combatlog_list");

                // CSVCMsgList_GameEvents
                public int DotaHeroChaseList => GetInt("dota_hero_chase_list");

                // CSVCMsgList_GameEvents
                public int DotaPickHeroList => GetInt("dota_pick_hero_list");
            }

            public class DemoStop : GameEvent
            {
            }

            public class DifficultyChanged : GameEvent
            {
                
                public int Newdifficulty => GetInt("newDifficulty");

                
                public int Olddifficulty => GetInt("oldDifficulty");

                // new difficulty as string
                public string Strdifficulty => GetString("strDifficulty");
            }

            public class DmBonusWeaponStart : GameEvent
            {
                // The length of time that this bonus lasts
                public int Time => GetInt("time");

                // Loadout position of the bonus weapon
                public int Pos => GetInt("Pos");
            }

            public class DoorBreak : GameEvent
            {
                
                public long Entindex => GetInt("entindex");

                
                public long Dmgstate => GetInt("dmgstate");
            }

            public class DoorClose : GameEvent
            {
                // Who closed the door
                public int Userid => GetInt("userid");

                // Is the door a checkpoint door
                public bool Checkpoint => GetBool("checkpoint");
            }

            public class DoorClosed : GameEvent
            {
                // Who closed the door
                public int Userid => GetInt("userid");

                
                public long Entindex => GetInt("entindex");
            }

            public class DoorMoving : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public long Entindex => GetInt("entindex");
            }

            public class DoorOpen : GameEvent
            {
                // Who closed the door
                public int Userid => GetInt("userid");

                
                public long Entindex => GetInt("entindex");
            }

            public class DroneAboveRoof : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Cargo => GetInt("cargo");
            }

            public class DroneCargoDetached : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Cargo => GetInt("cargo");

                
                public bool Delivered => GetBool("delivered");
            }

            public class DroneDispatched : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Priority => GetInt("priority");

                
                public int DroneDispatchedParam => GetInt("drone_dispatched");
            }

            public class DronegunAttack : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class DropRateModified : GameEvent
            {
            }

            public class DynamicShadowLightChanged : GameEvent
            {
            }

            public class DzItemInteraction : GameEvent
            {
                // player entindex
                public int Userid => GetInt("userid");

                // crate entindex
                public int Subject => GetInt("subject");

                // type of crate (metal, wood, or paradrop)
                public string Type => GetString("type");
            }

            public class EnableRestartVoting : GameEvent
            {
                
                public bool Enable => GetBool("enable");
            }

            public class EndmatchCmmStartRevealItems : GameEvent
            {
            }

            public class EndmatchMapvoteSelectingMap : GameEvent
            {
                // Number of "ties"
                public int Count => GetInt("count");

                
                public int Slot1 => GetInt("slot1");

                
                public int Slot2 => GetInt("slot2");

                
                public int Slot3 => GetInt("slot3");

                
                public int Slot4 => GetInt("slot4");

                
                public int Slot5 => GetInt("slot5");

                
                public int Slot6 => GetInt("slot6");

                
                public int Slot7 => GetInt("slot7");

                
                public int Slot8 => GetInt("slot8");

                
                public int Slot9 => GetInt("slot9");

                
                public int Slot10 => GetInt("slot10");
            }

            public class EnterBombzone : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public bool Hasbomb => GetBool("hasbomb");

                
                public bool Isplanted => GetBool("isplanted");
            }

            public class EnterBuyzone : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public bool Canbuy => GetBool("canbuy");
            }

            public class EnterRescueZone : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class EntityKilled : GameEvent
            {
                
                public long EntindexKilled => GetInt("entindex_killed");

                
                public long EntindexAttacker => GetInt("entindex_attacker");

                
                public long EntindexInflictor => GetInt("entindex_inflictor");

                
                public long Damagebits => GetInt("damagebits");
            }

            public class EntityVisible : GameEvent
            {
                // The player who sees the entity
                public int Userid => GetInt("userid");

                // Entindex of the entity they see
                public int Subject => GetInt("subject");

                // Classname of the entity they see
                public string Classname => GetString("classname");

                // name of the entity they see
                public string Entityname => GetString("entityname");
            }

            public class EventTicketModified : GameEvent
            {
            }

            public class ExitBombzone : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public bool Hasbomb => GetBool("hasbomb");

                
                public bool Isplanted => GetBool("isplanted");
            }

            public class ExitBuyzone : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public bool Canbuy => GetBool("canbuy");
            }

            public class ExitRescueZone : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class FinaleStart : GameEvent
            {
                
                public int Rushes => GetInt("rushes");
            }

            public class FirstbombsIncomingWarning : GameEvent
            {
                
                public bool Global => GetBool("global");
            }

            public class FlareIgniteNpc : GameEvent
            {
                // entity ignited
                public long Entindex => GetInt("entindex");
            }

            public class FlashbangDetonate : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class GameEnd : GameEvent
            {
                // winner team/user id
                public int Winner => GetInt("winner");
            }

            public class GameInit : GameEvent
            {
            }

            public class GameinstructorDraw : GameEvent
            {
            }

            public class GameinstructorNodraw : GameEvent
            {
            }

            public class GameMessage : GameEvent
            {
                // 0 = console, 1 = HUD
                public int Target => GetInt("target");

                // the message text
                public string Text => GetString("text");
            }

            public class GameNewmap : GameEvent
            {
                // map name
                public string Mapname => GetString("mapname");
            }

            public class GamePhaseChanged : GameEvent
            {
                
                public int NewPhase => GetInt("new_phase");
            }

            public class GameStart : GameEvent
            {
                // max round
                public long Roundslimit => GetInt("roundslimit");

                // time limit
                public long Timelimit => GetInt("timelimit");

                // frag limit
                public long Fraglimit => GetInt("fraglimit");

                // round objective
                public string Objective => GetString("objective");
            }

            public class GameuiHidden : GameEvent
            {
            }

            public class GcConnected : GameEvent
            {
            }

            public class GgKilledEnemy : GameEvent
            {
                // user ID who died
                public int Victimid => GetInt("victimid");

                // user ID who killed
                public int Attackerid => GetInt("attackerid");

                // did killer dominate victim with this kill
                public int Dominated => GetInt("dominated");

                // did killer get revenge on victim with this kill
                public int Revenge => GetInt("revenge");

                // did killer kill with a bonus weapon?
                public bool Bonus => GetBool("bonus");
            }

            public class GrenadeBounce : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class GrenadeThrown : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // weapon name used
                public string Weapon => GetString("weapon");
            }

            public class GuardianWaveRestart : GameEvent
            {
            }

            public class HegrenadeDetonate : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class HelicopterGrenadePuntMiss : GameEvent
            {
            }

            public class HideDeathpanel : GameEvent
            {
            }

            public class HltvCameraman : GameEvent
            {
                // camera man entity index
                public int Userid => GetInt("userid");
            }

            public class HltvChangedMode : GameEvent
            {
                
                public long Oldmode => GetInt("oldmode");

                
                public long Newmode => GetInt("newmode");

                
                public long ObsTarget => GetInt("obs_target");
            }

            public class HltvChase : GameEvent
            {
                // primary traget index
                public int Target1 => GetInt("target1");

                // secondary traget index or 0
                public int Target2 => GetInt("target2");

                // camera distance
                public int Distance => GetInt("distance");

                // view angle horizontal
                public int Theta => GetInt("theta");

                // view angle vertical
                public int Phi => GetInt("phi");

                // camera inertia
                public int Inertia => GetInt("inertia");

                // diretcor suggests to show ineye
                public int Ineye => GetInt("ineye");
            }

            public class HltvChat : GameEvent
            {
                
                public string Text => GetString("text");

                // steam id
                public ulong Steamid => GetUint64("steamID");
            }

            public class HltvFixed : GameEvent
            {
                // camera position in world
                public long Posx => GetInt("posx");

                
                public long Posy => GetInt("posy");

                
                public long Posz => GetInt("posz");

                // camera angles
                public int Theta => GetInt("theta");

                
                public int Phi => GetInt("phi");

                
                public int Offset => GetInt("offset");

                
                public float Fov => GetFloat("fov");

                // follow this player
                public int Target => GetInt("target");
            }

            public class HltvMessage : GameEvent
            {
                
                public string Text => GetString("text");
            }

            public class HltvRankCamera : GameEvent
            {
                // fixed camera index
                public int Index => GetInt("index");

                // ranking, how interesting is this camera view
                public float Rank => GetFloat("rank");

                // best/closest target entity
                public int Target => GetInt("target");
            }

            public class HltvRankEntity : GameEvent
            {
                // player slot
                public int Userid => GetInt("userid");

                // ranking, how interesting is this entity to view
                public float Rank => GetFloat("rank");

                // best/closest target entity
                public int Target => GetInt("target");
            }

            public class HltvReplay : GameEvent
            {
                // number of seconds in killer replay delay
                public long Delay => GetInt("delay");

                // reason for replay	(ReplayEventType_t)
                public long Reason => GetInt("reason");
            }

            public class HltvReplayStatus : GameEvent
            {
                // reason for hltv replay status change ()
                public long Reason => GetInt("reason");
            }

            public class HltvStatus : GameEvent
            {
                // number of HLTV spectators
                public long Clients => GetInt("clients");

                // number of HLTV slots
                public long Slots => GetInt("slots");

                // number of HLTV proxies
                public int Proxies => GetInt("proxies");

                // disptach master IP:port
                public string Master => GetString("master");
            }

            public class HltvTitle : GameEvent
            {
                
                public string Text => GetString("text");
            }

            public class HltvVersioninfo : GameEvent
            {
                
                public long Version => GetInt("version");
            }

            public class HostageCallForHelp : GameEvent
            {
                // hostage entity index
                public int Hostage => GetInt("hostage");
            }

            public class HostageFollows : GameEvent
            {
                // player who touched the hostage
                public int Userid => GetInt("userid");

                // hostage entity index
                public int Hostage => GetInt("hostage");
            }

            public class HostageHurt : GameEvent
            {
                // player who hurt the hostage
                public int Userid => GetInt("userid");

                // hostage entity index
                public int Hostage => GetInt("hostage");
            }

            public class HostageKilled : GameEvent
            {
                // player who killed the hostage
                public int Userid => GetInt("userid");

                // hostage entity index
                public int Hostage => GetInt("hostage");
            }

            public class HostageRescued : GameEvent
            {
                // player who rescued the hostage
                public int Userid => GetInt("userid");

                // hostage entity index
                public int Hostage => GetInt("hostage");

                // rescue site index
                public int Site => GetInt("site");
            }

            public class HostageRescuedAll : GameEvent
            {
            }

            public class HostageStopsFollowing : GameEvent
            {
                // player who rescued the hostage
                public int Userid => GetInt("userid");

                // hostage entity index
                public int Hostage => GetInt("hostage");
            }

            public class HostnameChanged : GameEvent
            {
                
                public string Hostname => GetString("hostname");
            }

            public class InfernoExpire : GameEvent
            {
                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class InfernoExtinguish : GameEvent
            {
                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class InfernoStartburn : GameEvent
            {
                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class InspectWeapon : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class InstructorCloseLesson : GameEvent
            {
                // The player who this lesson is intended for
                public int Userid => GetInt("userid");

                // Name of the lesson to start.  Must match instructor_lesson.txt
                public string HintName => GetString("hint_name");
            }

            public class InstructorServerHintCreate : GameEvent
            {
                // user ID of the player that triggered the hint
                public int Userid => GetInt("userid");

                // what to name the hint. For referencing it again later (e.g. a kill command for the hint instead of a timeout)
                public string HintName => GetString("hint_name");

                // type name so that messages of the same type will replace each other
                public string HintReplaceKey => GetString("hint_replace_key");

                // entity id that the hint should display at
                public long HintTarget => GetInt("hint_target");

                // userid id of the activator
                public int HintActivatorUserid => GetInt("hint_activator_userid");

                // how long in seconds until the hint automatically times out, 0 = never
                public int HintTimeout => GetInt("hint_timeout");

                // the hint icon to use when the hint is onscreen. e.g. "icon_alert_red"
                public string HintIconOnscreen => GetString("hint_icon_onscreen");

                // the hint icon to use when the hint is offscreen. e.g. "icon_alert"
                public string HintIconOffscreen => GetString("hint_icon_offscreen");

                // the hint caption. e.g. "#ThisIsDangerous"
                public string HintCaption => GetString("hint_caption");

                // the hint caption that only the activator sees e.g. "#YouPushedItGood"
                public string HintActivatorCaption => GetString("hint_activator_caption");

                // the hint color in "r,g,b" format where each component is 0-255
                public string HintColor => GetString("hint_color");

                // how far on the z axis to offset the hint from entity origin
                public float HintIconOffset => GetFloat("hint_icon_offset");

                // range before the hint is culled
                public float HintRange => GetFloat("hint_range");

                // hint flags
                public long HintFlags => GetInt("hint_flags");

                // bindings to use when use_binding is the onscreen icon
                public string HintBinding => GetString("hint_binding");

                // gamepad bindings to use when use_binding is the onscreen icon
                public string HintGamepadBinding => GetString("hint_gamepad_binding");

                // if false, the hint will dissappear if the target entity is invisible
                public bool HintAllowNodrawTarget => GetBool("hint_allow_nodraw_target");

                // if true, the hint will not show when outside the player view
                public bool HintNooffscreen => GetBool("hint_nooffscreen");

                // if true, the hint caption will show even if the hint is occluded
                public bool HintForcecaption => GetBool("hint_forcecaption");

                // if true, only the local player will see the hint
                public bool HintLocalPlayerOnly => GetBool("hint_local_player_only");
            }

            public class InstructorServerHintStop : GameEvent
            {
                // The hint to stop. Will stop ALL hints with this name
                public string HintName => GetString("hint_name");
            }

            public class InstructorStartLesson : GameEvent
            {
                // The player who this lesson is intended for
                public int Userid => GetInt("userid");

                // Name of the lesson to start.  Must match instructor_lesson.txt
                public string HintName => GetString("hint_name");

                // entity id that the hint should display at. Leave empty if controller target
                public long HintTarget => GetInt("hint_target");

                
                public int VrMovementType => GetInt("vr_movement_type");

                
                public bool VrSingleController => GetBool("vr_single_controller");

                
                public int VrControllerType => GetInt("vr_controller_type");
            }

            public class InventoryUpdated : GameEvent
            {
            }

            public class ItemEquip : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item => GetString("item");

                
                public long Defindex => GetInt("defindex");

                
                public bool Canzoom => GetBool("canzoom");

                
                public bool Hassilencer => GetBool("hassilencer");

                
                public bool Issilenced => GetBool("issilenced");

                
                public bool Hastracers => GetBool("hastracers");

                
                public int Weptype => GetInt("weptype");

                
                public bool Ispainted => GetBool("ispainted");
            }

            public class ItemPickup : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item => GetString("item");

                
                public bool Silent => GetBool("silent");

                
                public long Defindex => GetInt("defindex");
            }

            public class ItemPickupFailed : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public string Item => GetString("item");

                
                public int Reason => GetInt("reason");

                
                public int Limit => GetInt("limit");
            }

            public class ItemPickupSlerp : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Index => GetInt("index");

                
                public int Behavior => GetInt("behavior");
            }

            public class ItemPurchase : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Team => GetInt("team");

                
                public int Loadout => GetInt("loadout");

                
                public string Weapon => GetString("weapon");
            }

            public class ItemRemove : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // either a weapon such as 'tmp' or 'hegrenade', or an item such as 'nvgs'
                public string Item => GetString("item");

                
                public long Defindex => GetInt("defindex");
            }

            public class ItemSchemaInitialized : GameEvent
            {
            }

            public class ItemsGifted : GameEvent
            {
                // entity used by player
                public int Player => GetInt("player");

                
                public long Itemdef => GetInt("itemdef");

                
                public int Numgifts => GetInt("numgifts");

                
                public long Giftidx => GetInt("giftidx");

                
                public long Accountid => GetInt("accountid");
            }

            public class JointeamFailed : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // 0 = team_full
                public int Reason => GetInt("reason");
            }

            public class LocalPlayerControllerTeam : GameEvent
            {
            }

            public class LocalPlayerPawnChanged : GameEvent
            {
            }

            public class LocalPlayerTeam : GameEvent
            {
            }

            public class LootCrateOpened : GameEvent
            {
                // player entindex
                public int Userid => GetInt("userid");

                // type of crate (metal, wood, or paradrop)
                public string Type => GetString("type");
            }

            public class LootCrateVisible : GameEvent
            {
                // player entindex
                public int Userid => GetInt("userid");

                // crate entindex
                public int Subject => GetInt("subject");

                // type of crate (metal, wood, or paradrop)
                public string Type => GetString("type");
            }

            public class MapShutdown : GameEvent
            {
            }

            public class MapTransition : GameEvent
            {
            }

            public class MatchEndConditions : GameEvent
            {
                
                public long Frags => GetInt("frags");

                
                public long MaxRounds => GetInt("max_rounds");

                
                public long WinRounds => GetInt("win_rounds");

                
                public long Time => GetInt("time");
            }

            public class MaterialDefaultComplete : GameEvent
            {
            }

            public class MbInputLockCancel : GameEvent
            {
            }

            public class MbInputLockSuccess : GameEvent
            {
            }

            public class MolotovDetonate : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class NavBlocked : GameEvent
            {
                
                public long Area => GetInt("area");

                
                public bool Blocked => GetBool("blocked");
            }

            public class NavGenerate : GameEvent
            {
            }

            public class NextlevelChanged : GameEvent
            {
                
                public string Nextlevel => GetString("nextlevel");

                
                public string Mapgroup => GetString("mapgroup");

                
                public string Skirmishmode => GetString("skirmishmode");
            }

            public class OpenCrateInstr : GameEvent
            {
                // player entindex
                public int Userid => GetInt("userid");

                // crate entindex
                public int Subject => GetInt("subject");

                // type of crate (metal, wood, or paradrop)
                public string Type => GetString("type");
            }

            public class OtherDeath : GameEvent
            {
                // other entity ID who died
                public int Otherid => GetInt("otherid");

                // other entity type
                public string Othertype => GetString("othertype");

                // user ID who killed
                public int Attacker => GetInt("attacker");

                // weapon name killer used
                public string Weapon => GetString("weapon");

                // inventory item id of weapon killer used
                public string WeaponItemid => GetString("weapon_itemid");

                // faux item id of weapon killer used
                public string WeaponFauxitemid => GetString("weapon_fauxitemid");

                
                public string WeaponOriginalownerXuid => GetString("weapon_originalowner_xuid");

                // singals a headshot
                public bool Headshot => GetBool("headshot");

                // number of objects shot penetrated before killing target
                public int Penetrated => GetInt("penetrated");

                // kill happened without a scope, used for death notice icon
                public bool Noscope => GetBool("noscope");

                // hitscan weapon went through smoke grenade
                public bool Thrusmoke => GetBool("thrusmoke");

                // attacker was blind from flashbang
                public bool Attackerblind => GetBool("attackerblind");
            }

            public class ParachuteDeploy : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class ParachutePickup : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class PhysgunPickup : GameEvent
            {
                // entity picked up
                public IntPtr Target => GetInt("target");
            }

            public class PlayerActivate : GameEvent
            {
                // user ID on server
                public int Userid => GetInt("userid");
            }

            public class PlayerAvengedTeammate : GameEvent
            {
                
                public int AvengerId => GetInt("avenger_id");

                
                public int AvengedPlayerId => GetInt("avenged_player_id");
            }

            public class PlayerBlind : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // user ID who threw the flash
                public int Attacker => GetInt("attacker");

                // the flashbang going off
                public int Entityid => GetInt("entityid");

                
                public float BlindDuration => GetFloat("blind_duration");
            }

            public class PlayerChangename : GameEvent
            {
                // user ID on server
                public int Userid => GetInt("userid");

                // players old (current) name
                public string Oldname => GetString("oldname");

                // players new name
                public string Newname => GetString("newname");
            }

            public class PlayerChat : GameEvent
            {
                // true if team only chat
                public bool Teamonly => GetBool("teamonly");

                // chatting player
                public int Userid => GetInt("userid");

                // chat text
                public string Text => GetString("text");
            }

            public class PlayerConnect : GameEvent
            {
                // player name
                public string Name => GetString("name");

                // user ID on server (unique on server)
                public int Userid => GetInt("userid");

                // player network (i.e steam) id
                public string Networkid => GetString("networkid");

                // steam id
                public ulong Xuid => GetUint64("xuid");

                // ip:port
                public string Address => GetString("address");

                
                public bool Bot => GetBool("bot");
            }

            public class PlayerConnectFull : GameEvent
            {
                // user ID on server (unique on server)
                public int Userid => GetInt("userid");
            }

            public class PlayerDeath : GameEvent
            {
                // user who died
                public int Userid => GetInt("userid");

                // player who killed
                public int Attacker => GetInt("attacker");

                // player who assisted in the kill
                public int Assister => GetInt("assister");

                // assister helped with a flash
                public bool Assistedflash => GetBool("assistedflash");

                // weapon name killer used
                public string Weapon => GetString("weapon");

                // inventory item id of weapon killer used
                public string WeaponItemid => GetString("weapon_itemid");

                // faux item id of weapon killer used
                public string WeaponFauxitemid => GetString("weapon_fauxitemid");

                
                public string WeaponOriginalownerXuid => GetString("weapon_originalowner_xuid");

                // singals a headshot
                public bool Headshot => GetBool("headshot");

                // did killer dominate victim with this kill
                public int Dominated => GetInt("dominated");

                // did killer get revenge on victim with this kill
                public int Revenge => GetInt("revenge");

                // is the kill resulting in squad wipe
                public int Wipe => GetInt("wipe");

                // number of objects shot penetrated before killing target
                public int Penetrated => GetInt("penetrated");

                // if replay data is unavailable, this will be present and set to false
                public bool Noreplay => GetBool("noreplay");

                // kill happened without a scope, used for death notice icon
                public bool Noscope => GetBool("noscope");

                // hitscan weapon went through smoke grenade
                public bool Thrusmoke => GetBool("thrusmoke");

                // attacker was blind from flashbang
                public bool Attackerblind => GetBool("attackerblind");

                // distance to victim in meters
                public float Distance => GetFloat("distance");

                // damage done to health
                public int DmgHealth => GetInt("dmg_health");

                // damage done to armor
                public int DmgArmor => GetInt("dmg_armor");

                // hitgroup that was damaged
                public int Hitgroup => GetInt("hitgroup");
            }

            public class PlayerDecal : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class PlayerDisconnect : GameEvent
            {
                // user ID on server
                public int Userid => GetInt("userid");

                // see networkdisconnect enum protobuf
                public int Reason => GetInt("reason");

                // player name
                public string Name => GetString("name");

                // player network (i.e steam) id
                public string Networkid => GetString("networkid");

                // steam id
                public ulong Xuid => GetUint64("xuid");

                
                public int Playerid => GetInt("PlayerID");
            }

            public class PlayerFalldamage : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public float Damage => GetFloat("damage");
            }

            public class PlayerFootstep : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class PlayerFullUpdate : GameEvent
            {
                // user ID on server
                public int Userid => GetInt("userid");

                // Number of this full update
                public int Count => GetInt("count");
            }

            public class PlayerGivenC4 : GameEvent
            {
                // user ID who received the c4
                public int Userid => GetInt("userid");
            }

            public class PlayerHintmessage : GameEvent
            {
                // localizable string of a hint
                public string Hintmessage => GetString("hintmessage");
            }

            public class PlayerHurt : GameEvent
            {
                // player index who was hurt
                public int Userid => GetInt("userid");

                // player index who attacked
                public int Attacker => GetInt("attacker");

                // remaining health points
                public int Health => GetInt("health");

                // remaining armor points
                public int Armor => GetInt("armor");

                // weapon name attacker used, if not the world
                public string Weapon => GetString("weapon");

                // damage done to health
                public int DmgHealth => GetInt("dmg_health");

                // damage done to armor
                public int DmgArmor => GetInt("dmg_armor");

                // hitgroup that was damaged
                public int Hitgroup => GetInt("hitgroup");
            }

            public class PlayerInfo : GameEvent
            {
                // player name
                public string Name => GetString("name");

                // user ID on server (unique on server)
                public int Userid => GetInt("userid");

                // player network (i.e steam) id
                public ulong Steamid => GetUint64("steamid");

                // true if player is a AI bot
                public bool Bot => GetBool("bot");
            }

            public class PlayerJump : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class PlayerPing : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");

                
                public bool Urgent => GetBool("urgent");
            }

            public class PlayerPingStop : GameEvent
            {
                
                public int Entityid => GetInt("entityid");
            }

            public class PlayerRadio : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Slot => GetInt("slot");
            }

            public class PlayerResetVote : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public bool Vote => GetBool("vote");
            }

            public class PlayerScore : GameEvent
            {
                // user ID on server
                public int Userid => GetInt("userid");

                // # of kills
                public int Kills => GetInt("kills");

                // # of deaths
                public int Deaths => GetInt("deaths");

                // total game score
                public int Score => GetInt("score");
            }

            public class PlayerShoot : GameEvent
            {
                // user ID on server
                public int Userid => GetInt("userid");

                // weapon ID
                public int Weapon => GetInt("weapon");

                // weapon mode
                public int Mode => GetInt("mode");
            }

            public class PlayerSound : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Radius => GetInt("radius");

                
                public float Duration => GetFloat("duration");

                
                public bool Step => GetBool("step");
            }

            public class PlayerSpawn : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class PlayerSpawned : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // true if restart is pending
                public bool Inrestart => GetBool("inrestart");
            }

            public class PlayerStatsUpdated : GameEvent
            {
                
                public bool Forceupload => GetBool("forceupload");
            }

            public class PlayerTeam : GameEvent
            {
                // player
                public int Userid => GetInt("userid");

                // team id
                public int Team => GetInt("team");

                // old team id
                public int Oldteam => GetInt("oldteam");

                // team change because player disconnects
                public bool Disconnect => GetBool("disconnect");

                
                public bool Silent => GetBool("silent");

                // true if player is a bot
                public bool Isbot => GetBool("isbot");
            }

            public class RagdollDissolved : GameEvent
            {
                
                public long Entindex => GetInt("entindex");
            }

            public class ReadGameTitledata : GameEvent
            {
                // Controller id of user
                public int Controllerid => GetInt("controllerId");
            }

            public class RepostXboxAchievements : GameEvent
            {
                // splitscreen ID
                public int Splitscreenplayer => GetInt("splitscreenplayer");
            }

            public class ResetGameTitledata : GameEvent
            {
                // Controller id of user
                public int Controllerid => GetInt("controllerId");
            }

            public class RoundAnnounceFinal : GameEvent
            {
            }

            public class RoundAnnounceLastRoundHalf : GameEvent
            {
            }

            public class RoundAnnounceMatchPoint : GameEvent
            {
            }

            public class RoundAnnounceMatchStart : GameEvent
            {
            }

            public class RoundAnnounceWarmup : GameEvent
            {
            }

            public class RoundEnd : GameEvent
            {
                // winner team/user i
                public int Winner => GetInt("winner");

                // reson why team won
                public int Reason => GetInt("reason");

                // end round message
                public string Message => GetString("message");

                // server-generated legacy value
                public int Legacy => GetInt("legacy");

                // total number of players alive at the end of round, used for statistics gathering, computed on the server in the event client is in replay when receiving this message
                public int PlayerCount => GetInt("player_count");

                // if set, don't play round end music, because action is still on-going
                public int Nomusic => GetInt("nomusic");
            }

            public class RoundEndUploadStats : GameEvent
            {
            }

            public class RoundFreezeEnd : GameEvent
            {
            }

            public class RoundMvp : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Reason => GetInt("reason");

                
                public long Value => GetInt("value");

                
                public long Musickitmvps => GetInt("musickitmvps");

                
                public int Nomusic => GetInt("nomusic");

                
                public long Musickitid => GetInt("musickitid");
            }

            public class RoundOfficiallyEnded : GameEvent
            {
            }

            public class RoundPoststart : GameEvent
            {
            }

            public class RoundPrestart : GameEvent
            {
            }

            public class RoundStart : GameEvent
            {
                // round time limit in seconds
                public long Timelimit => GetInt("timelimit");

                // frag limit in seconds
                public long Fraglimit => GetInt("fraglimit");

                // round objective
                public string Objective => GetString("objective");
            }

            public class RoundStartPostNav : GameEvent
            {
            }

            public class RoundStartPreEntity : GameEvent
            {
            }

            public class RoundTimeWarning : GameEvent
            {
            }

            public class SeasoncoinLevelup : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Category => GetInt("category");

                
                public int Rank => GetInt("rank");
            }

            public class ServerCvar : GameEvent
            {
                // cvar name, eg "mp_roundtime"
                public string Cvarname => GetString("cvarname");

                // new cvar value
                public string Cvarvalue => GetString("cvarvalue");
            }

            public class ServerMessage : GameEvent
            {
                // the message text
                public string Text => GetString("text");
            }

            public class ServerPreShutdown : GameEvent
            {
                // reason why server is about to be shut down
                public string Reason => GetString("reason");
            }

            public class ServerShutdown : GameEvent
            {
                // reason why server was shut down
                public string Reason => GetString("reason");
            }

            public class ServerSpawn : GameEvent
            {
                // public host name
                public string Hostname => GetString("hostname");

                // hostame, IP or DNS name
                public string Address => GetString("address");

                // server port
                public int Port => GetInt("port");

                // game dir
                public string Game => GetString("game");

                // map name
                public string Mapname => GetString("mapname");

                // addon name
                public string Addonname => GetString("addonname");

                // max players
                public long Maxplayers => GetInt("maxplayers");

                // WIN32, LINUX
                public string Os => GetString("os");

                // true if dedicated server
                public bool Dedicated => GetBool("dedicated");

                // true if password protected
                public bool Password => GetBool("password");
            }

            public class SetInstructorGroupEnabled : GameEvent
            {
                
                public string Group => GetString("group");

                
                public int Enabled => GetInt("enabled");
            }

            public class Sfuievent : GameEvent
            {
                
                public string Action => GetString("action");

                
                public string Data => GetString("data");

                
                public int Slot => GetInt("slot");
            }

            public class ShowDeathpanel : GameEvent
            {
                // endindex of the one who was killed
                public int Victim => GetInt("victim");

                // entindex of the killer entity
                public IntPtr Killer => GetInt("killer");

                
                public int KillerController => GetInt("killer_controller");

                
                public int HitsTaken => GetInt("hits_taken");

                
                public int DamageTaken => GetInt("damage_taken");

                
                public int HitsGiven => GetInt("hits_given");

                
                public int DamageGiven => GetInt("damage_given");
            }

            public class ShowSurvivalRespawnStatus : GameEvent
            {
                
                public string LocToken => GetString("loc_token");

                
                public long Duration => GetInt("duration");

                
                public int Userid => GetInt("userid");
            }

            public class SilencerDetach : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class SilencerOff : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class SilencerOn : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class SmokeBeaconParadrop : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Paradrop => GetInt("paradrop");
            }

            public class SmokegrenadeDetonate : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class SmokegrenadeExpired : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class SpecModeUpdated : GameEvent
            {
                // entindex of the player
                public int Userid => GetInt("userid");
            }

            public class SpecTargetUpdated : GameEvent
            {
                // spectating player
                public int Userid => GetInt("userid");

                // ehandle of the target
                public IntPtr Target => GetInt("target");
            }

            public class StartHalftime : GameEvent
            {
            }

            public class StartVote : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Type => GetInt("type");

                
                public int VoteParameter => GetInt("vote_parameter");
            }

            public class StorePricesheetUpdated : GameEvent
            {
            }

            public class SurvivalAnnouncePhase : GameEvent
            {
                // The phase #
                public int Phase => GetInt("phase");
            }

            public class SurvivalNoRespawnsFinal : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class SurvivalNoRespawnsWarning : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class SurvivalParadropBreak : GameEvent
            {
                
                public int Entityid => GetInt("entityid");
            }

            public class SurvivalParadropSpawn : GameEvent
            {
                
                public int Entityid => GetInt("entityid");
            }

            public class SurvivalTeammateRespawn : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class SwitchTeam : GameEvent
            {
                // number of active players on both T and CT
                public int Numplayers => GetInt("numPlayers");

                // number of spectators
                public int Numspectators => GetInt("numSpectators");

                // average rank of human players
                public int AvgRank => GetInt("avg_rank");

                
                public int Numtslotsfree => GetInt("numTSlotsFree");

                
                public int Numctslotsfree => GetInt("numCTSlotsFree");
            }

            public class TagrenadeDetonate : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Entityid => GetInt("entityid");

                
                public float X => GetFloat("x");

                
                public float Y => GetFloat("y");

                
                public float Z => GetFloat("z");
            }

            public class TeamchangePending : GameEvent
            {
                
                public int Userid => GetInt("userid");

                
                public int Toteam => GetInt("toteam");
            }

            public class TeamInfo : GameEvent
            {
                // unique team id
                public int Teamid => GetInt("teamid");

                // team name eg "Team Blue"
                public string Teamname => GetString("teamname");
            }

            public class TeamIntroEnd : GameEvent
            {
            }

            public class TeamIntroStart : GameEvent
            {
            }

            public class TeamplayBroadcastAudio : GameEvent
            {
                // unique team id
                public int Team => GetInt("team");

                // name of the sound to emit
                public string Sound => GetString("sound");
            }

            public class TeamplayRoundStart : GameEvent
            {
                // is this a full reset of the map
                public bool FullReset => GetBool("full_reset");
            }

            public class TeamScore : GameEvent
            {
                // team id
                public int Teamid => GetInt("teamid");

                // total team score
                public int Score => GetInt("score");
            }

            public class TournamentReward : GameEvent
            {
                
                public long Defindex => GetInt("defindex");

                
                public long Totalrewards => GetInt("totalrewards");

                
                public long Accountid => GetInt("accountid");
            }

            public class TrExitHintTrigger : GameEvent
            {
            }

            public class TrialTimeExpired : GameEvent
            {
                // player whose time has expired
                public int Userid => GetInt("userid");
            }

            public class TrMarkBestTime : GameEvent
            {
                
                public long Time => GetInt("time");
            }

            public class TrMarkComplete : GameEvent
            {
                
                public int Complete => GetInt("complete");
            }

            public class TrPlayerFlashbanged : GameEvent
            {
                // user ID of the player banged
                public int Userid => GetInt("userid");
            }

            public class TrShowExitMsgbox : GameEvent
            {
            }

            public class TrShowFinishMsgbox : GameEvent
            {
            }

            public class UgcFileDownloadFinished : GameEvent
            {
                // id of this specific content (may be image or map)
                public ulong Hcontent => GetUint64("hcontent");
            }

            public class UgcFileDownloadStart : GameEvent
            {
                // id of this specific content (may be image or map)
                public ulong Hcontent => GetUint64("hcontent");

                // id of the associated content package
                public ulong PublishedFileId => GetUint64("published_file_id");
            }

            public class UgcMapDownloadError : GameEvent
            {
                
                public ulong PublishedFileId => GetUint64("published_file_id");

                
                public long ErrorCode => GetInt("error_code");
            }

            public class UgcMapInfoReceived : GameEvent
            {
                
                public ulong PublishedFileId => GetUint64("published_file_id");
            }

            public class UgcMapUnsubscribed : GameEvent
            {
                
                public ulong PublishedFileId => GetUint64("published_file_id");
            }

            public class UpdateMatchmakingStats : GameEvent
            {
            }

            public class UserDataDownloaded : GameEvent
            {
            }

            public class VipEscaped : GameEvent
            {
                // player who was the VIP
                public int Userid => GetInt("userid");
            }

            public class VipKilled : GameEvent
            {
                // player who was the VIP
                public int Userid => GetInt("userid");

                // user ID who killed the VIP
                public int Attacker => GetInt("attacker");
            }

            public class VoteCast : GameEvent
            {
                // which option the player voted on
                public int VoteOption => GetInt("vote_option");

                
                public int Team => GetInt("team");

                // player who voted
                public int Userid => GetInt("userid");
            }

            public class VoteCastNo : GameEvent
            {
                
                public int Team => GetInt("team");

                // entity id of the voter
                public long Entityid => GetInt("entityid");
            }

            public class VoteCastYes : GameEvent
            {
                
                public int Team => GetInt("team");

                // entity id of the voter
                public long Entityid => GetInt("entityid");
            }

            public class VoteChanged : GameEvent
            {
                
                public int VoteOption1 => GetInt("vote_option1");

                
                public int VoteOption2 => GetInt("vote_option2");

                
                public int VoteOption3 => GetInt("vote_option3");

                
                public int VoteOption4 => GetInt("vote_option4");

                
                public int VoteOption5 => GetInt("vote_option5");

                
                public int Potentialvotes => GetInt("potentialVotes");
            }

            public class VoteEnded : GameEvent
            {
            }

            public class VoteFailed : GameEvent
            {
                
                public int Team => GetInt("team");

                // this event is reliable
                public int Reliable => GetInt("reliable");
            }

            public class VoteOptions : GameEvent
            {
                // Number of options - up to MAX_VOTE_OPTIONS
                public int Count => GetInt("count");

                
                public string Option1 => GetString("option1");

                
                public string Option2 => GetString("option2");

                
                public string Option3 => GetString("option3");

                
                public string Option4 => GetString("option4");

                
                public string Option5 => GetString("option5");
            }

            public class VotePassed : GameEvent
            {
                
                public string Details => GetString("details");

                
                public string Param1 => GetString("param1");

                
                public int Team => GetInt("team");

                // this event is reliable
                public int Reliable => GetInt("reliable");
            }

            public class VoteStarted : GameEvent
            {
                
                public string Issue => GetString("issue");

                
                public string Param1 => GetString("param1");

                
                public int Team => GetInt("team");

                // entity id of the player who initiated the vote
                public long Initiator => GetInt("initiator");
            }

            public class WeaponFire : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // weapon name used
                public string Weapon => GetString("weapon");

                // is weapon silenced
                public bool Silenced => GetBool("silenced");
            }

            public class WeaponFireOnEmpty : GameEvent
            {
                
                public int Userid => GetInt("userid");

                // weapon name used
                public string Weapon => GetString("weapon");
            }

            public class WeaponhudSelection : GameEvent
            {
                // Player who this event applies to
                public int Userid => GetInt("userid");

                // EWeaponHudSelectionMode (switch / pickup / drop)
                public int Mode => GetInt("mode");

                // Weapon entity index
                public long Entindex => GetInt("entindex");
            }

            public class WeaponOutofammo : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class WeaponReload : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class WeaponZoom : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class WeaponZoomRifle : GameEvent
            {
                
                public int Userid => GetInt("userid");
            }

            public class WriteGameTitledata : GameEvent
            {
                // Controller id of user
                public int Controllerid => GetInt("controllerId");
            }

            public class WriteProfileData : GameEvent
            {
            }
}
