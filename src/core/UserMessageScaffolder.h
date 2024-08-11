/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */
#include "clientmessages.pb.h"
#include "cs_gameevents.pb.h"
#include "cs_usercmd.pb.h"
#include "cstrike15_usermessages.pb.h"
#include "gameevents.pb.h"
#include "google/protobuf/message.h"
#include "netmessages.pb.h"
#include "networkbasetypes.pb.h"
#include "te.pb.h"
#include "usermessages.pb.h"

// static CNetMessage* CreateMessagePrototype(int messageId, INetworkMessageInternal* messageInternal)
//{
//
//     auto x = messageInternal->AllocateMessage()
//
//     switch (messageId)
//     {
//         case 0:
//             message = new CNETMsg_NOP;
//             break;
//         case 1:
//             message = new CNETMsg_Disconnect;
//             break;
//         case 3:
//             message = new CNETMsg_SplitScreenUser;
//             break;
//         case 4:
//             message = new CNETMsg_Tick;
//             break;
//         case 5:
//             message = new CNETMsg_StringCmd;
//             break;
//         case 6:
//             message = new CNETMsg_SetConVar;
//             break;
//         case 7:
//             message = new CNETMsg_SignonState;
//             break;
//         case 8:
//             message = new CNETMsg_SpawnGroup_Load;
//             break;
//         case 9:
//             message = new CNETMsg_SpawnGroup_ManifestUpdate;
//             break;
//         case 11:
//             message = new CNETMsg_SpawnGroup_SetCreationTick;
//             break;
//         case 12:
//             message = new CNETMsg_SpawnGroup_Unload;
//             break;
//         case 13:
//             message = new CNETMsg_SpawnGroup_LoadCompleted;
//             break;
//         case 15:
//             message = new CNETMsg_DebugOverlay;
//             break;
//         case 16:
//             message = new CBidirMsg_RebroadcastGameEvent;
//             break;
//         case 17:
//             message = new CBidirMsg_RebroadcastSource;
//             break;
//         case 20:
//             message = new CCLCMsg_ClientInfo;
//             break;
//         case 21:
//             message = new CCLCMsg_Move;
//             break;
//         case 22:
//             message = new CCLCMsg_VoiceData;
//             break;
//         case 23:
//             message = new CCLCMsg_BaselineAck;
//             break;
//         case 25:
//             message = new CCLCMsg_RespondCvarValue;
//             break;
//         case 26:
//             message = new CCLCMsg_FileCRCCheck;
//             break;
//         case 27:
//             message = new CCLCMsg_LoadingProgress;
//             break;
//         case 28:
//             message = new CCLCMsg_SplitPlayerConnect;
//             break;
//         case 30:
//             message = new CCLCMsg_SplitPlayerDisconnect;
//             break;
//         case 31:
//             message = new CCLCMsg_ServerStatus;
//             break;
//         case 33:
//             message = new CCLCMsg_RequestPause;
//             break;
//         case 34:
//             message = new CCLCMsg_CmdKeyValues;
//             break;
//         case 35:
//             message = new CCLCMsg_RconServerDetails;
//             break;
//         case 36:
//             message = new CCLCMsg_HltvReplay;
//             break;
//         case 40:
//             message = new CSVCMsg_ServerInfo;
//             break;
//         case 41:
//             message = new CSVCMsg_FlattenedSerializer;
//             break;
//         case 42:
//             message = new CSVCMsg_ClassInfo;
//             break;
//         case 43:
//             message = new CSVCMsg_SetPause;
//             break;
//         case 44:
//             message = new CSVCMsg_CreateStringTable;
//             break;
//         case 45:
//             message = new CSVCMsg_UpdateStringTable;
//             break;
//         case 46:
//             message = new CSVCMsg_VoiceInit;
//             break;
//         case 47:
//             message = new CSVCMsg_VoiceData;
//             break;
//         case 48:
//             message = new CSVCMsg_Print;
//             break;
//         case 49:
//             message = new CSVCMsg_Sounds;
//             break;
//         case 50:
//             message = new CSVCMsg_SetView;
//             break;
//         case 51:
//             message = new CSVCMsg_ClearAllStringTables;
//             break;
//         case 52:
//             message = new CSVCMsg_CmdKeyValues;
//             break;
//         case 54:
//             message = new CSVCMsg_SplitScreen;
//             break;
//         case 55:
//             message = new CSVCMsg_PacketEntities;
//             break;
//         case 56:
//             message = new CSVCMsg_Prefetch;
//             break;
//         case 57:
//             message = new CSVCMsg_Menu;
//             break;
//         case 58:
//             message = new CSVCMsg_GetCvarValue;
//             break;
//         case 59:
//             message = new CSVCMsg_StopSound;
//             break;
//         case 60:
//             message = new CSVCMsg_PeerList;
//             break;
//         case 61:
//             message = new CSVCMsg_PacketReliable;
//             break;
//         case 62:
//             message = new CSVCMsg_HLTVStatus;
//             break;
//         case 63:
//             message = new CSVCMsg_ServerSteamID;
//             break;
//         case 70:
//             message = new CSVCMsg_FullFrameSplit;
//             break;
//         case 71:
//             message = new CSVCMsg_RconServerDetails;
//             break;
//         case 72:
//             message = new CSVCMsg_UserMessage;
//             break;
//         case 74:
//             message = new CSVCMsg_HltvReplay;
//             break;
//         case 75:
//             message = new CCLCMsg_HltvFixupOperatorTick;
//             break;
//         case 101:
//             message = new CCSUsrMsg_AchievementEvent;
//             break;
//         case 102:
//             message = new CUserMessageCloseCaption;
//             break;
//         case 103:
//             message = new CCSUsrMsg_CloseCaptionDirect;
//             break;
//         case 104:
//             message = new CCSUsrMsg_CurrentTimescale;
//             break;
//         case 105:
//             message = new CUserMessageDesiredTimescale;
//             break;
//         case 106:
//             message = new CUserMessageFade;
//             break;
//         case 110:
//             message = new CUserMessageHudMsg;
//             break;
//         case 111:
//             message = new CUserMessageHudText;
//             break;
//         case 113:
//             message = new CUserMessageColoredText;
//             break;
//         case 114:
//             message = new CUserMessageRequestState;
//             break;
//         case 115:
//             message = new CUserMessageResetHUD;
//             break;
//         case 116:
//             message = new CUserMessageRumble;
//             break;
//         case 117:
//             message = new CUserMessageSayText;
//             break;
//         case 118:
//             message = new CUserMessageSayText2;
//             break;
//         case 119:
//             message = new CUserMessageSayTextChannel;
//             break;
//         case 120:
//             message = new CUserMessageShake;
//             break;
//         case 121:
//             message = new CUserMessageShakeDir;
//             break;
//         case 122:
//             message = new CUserMessageWaterShake;
//             break;
//         case 124:
//             message = new CUserMessageTextMsg;
//             break;
//         case 125:
//             message = new CUserMessageScreenTilt;
//             break;
//         case 128:
//             message = new CUserMessageVoiceMask;
//             break;
//         case 130:
//             message = new CUserMessageSendAudio;
//             break;
//         case 131:
//             message = new CUserMessageItemPickup;
//             break;
//         case 132:
//             message = new CUserMessageAmmoDenied;
//             break;
//         case 134:
//             message = new CUserMessageShowMenu;
//             break;
//         case 135:
//             message = new CUserMessageCreditsMsg;
//             break;
//         case 137:
//             message = new CEntityMessageScreenOverlay;
//             break;
//         case 138:
//             message = new CEntityMessageRemoveAllDecals;
//             break;
//         case 139:
//             message = new CEntityMessagePropagateForce;
//             break;
//         case 140:
//             message = new CEntityMessageDoSpark;
//             break;
//         case 142:
//             message = new CUserMessageCloseCaptionPlaceholder;
//             break;
//         case 143:
//             message = new CUserMessageCameraTransition;
//             break;
//         case 144:
//             message = new CUserMessageAudioParameter;
//             break;
//         case 145:
//             message = new CUserMsg_ParticleManager;
//             break;
//         case 146:
//             message = new CUserMsg_HudError;
//             break;
//         case 148:
//             message = new CUserMsg_CustomGameEvent;
//             break;
//         case 150:
//             message = new CUserMessageHapticsManagerPulse;
//             break;
//         case 151:
//             message = new CUserMessageHapticsManagerEffect;
//             break;
//         case 152:
//             message = new CUserMessageCommandQueueState;
//             break;
//         case 153:
//             message = new CUserMessageUpdateCssClasses;
//             break;
//         case 154:
//             message = new CUserMessageServerFrameTime;
//             break;
//         case 155:
//             message = new CUserMessageLagCompensationError;
//             break;
//         case 156:
//             message = new CUserMessageRequestDllStatus;
//             break;
//         case 157:
//             message = new CUserMessageRequestUtilAction;
//             break;
//         case 160:
//             message = new CUserMessageRequestInventory;
//             break;
//         case 162:
//             message = new CUserMessageRequestDiagnostic;
//             break;
//         case 205:
//             message = new CMsgSource1LegacyGameEventList;
//             break;
//         case 206:
//             message = new CMsgSource1LegacyListenEvents;
//             break;
//         case 207:
//             message = new CMsgSource1LegacyGameEvent;
//             break;
//         case 208:
//             message = new CMsgSosStartSoundEvent;
//             break;
//         case 209:
//             message = new CMsgSosStopSoundEvent;
//             break;
//         case 210:
//             message = new CMsgSosSetSoundEventParams;
//             break;
//         case 211:
//             message = new CMsgSosSetLibraryStackFields;
//             break;
//         case 212:
//             message = new CMsgSosStopSoundEventHash;
//             break;
//         case 280:
//             message = new CClientMsg_CustomGameEvent;
//             break;
//         case 281:
//             message = new CClientMsg_CustomGameEventBounce;
//             break;
//         case 282:
//             message = new CClientMsg_ClientUIEvent;
//             break;
//         case 301:
//             message = new CCSUsrMsg_VGUIMenu;
//             break;
//         case 312:
//             message = new CCSUsrMsg_Shake;
//             break;
//         case 317:
//             message = new CCSUsrMsg_SendAudio;
//             break;
//         case 318:
//             message = new CCSUsrMsg_RawAudio;
//             break;
//         case 321:
//             message = new CCSUsrMsg_Damage;
//             break;
//         case 322:
//             message = new CCSUsrMsg_RadioText;
//             break;
//         case 323:
//             message = new CCSUsrMsg_HintText;
//             break;
//         case 324:
//             message = new CCSUsrMsg_KeyHintText;
//             break;
//         case 325:
//             message = new CCSUsrMsg_ProcessSpottedEntityUpdate;
//             break;
//         case 327:
//             message = new CCSUsrMsg_AdjustMoney;
//             break;
//         case 330:
//             message = new CCSUsrMsg_KillCam;
//             break;
//         case 334:
//             message = new CCSUsrMsg_MatchEndConditions;
//             break;
//         case 335:
//             message = new CCSUsrMsg_DisconnectToLobby;
//             break;
//         case 336:
//             message = new CCSUsrMsg_PlayerStatsUpdate;
//             break;
//         case 338:
//             message = new CCSUsrMsg_WarmupHasEnded;
//             break;
//         case 345:
//             message = new CCSUsrMsg_CallVoteFailed;
//             break;
//         case 346:
//             message = new CCSUsrMsg_VoteStart;
//             break;
//         case 347:
//             message = new CCSUsrMsg_VotePass;
//             break;
//         case 348:
//             message = new CCSUsrMsg_VoteFailed;
//             break;
//         case 349:
//             message = new CCSUsrMsg_VoteSetup;
//             break;
//         case 350:
//             message = new CCSUsrMsg_ServerRankRevealAll;
//             break;
//         case 351:
//             message = new CCSUsrMsg_SendLastKillerDamageToClient;
//             break;
//         case 352:
//             message = new CCSUsrMsg_ServerRankUpdate;
//             break;
//         case 360:
//             message = new CCSUsrMsg_GlowPropTurnOff;
//             break;
//         case 361:
//             message = new CCSUsrMsg_SendPlayerItemDrops;
//             break;
//         case 362:
//             message = new CCSUsrMsg_RoundBackupFilenames;
//             break;
//         case 363:
//             message = new CCSUsrMsg_SendPlayerItemFound;
//             break;
//         case 364:
//             message = new CCSUsrMsg_ReportHit;
//             break;
//         case 365:
//             message = new CCSUsrMsg_XpUpdate;
//             break;
//         case 366:
//             message = new CCSUsrMsg_QuestProgress;
//             break;
//         case 367:
//             message = new CCSUsrMsg_ScoreLeaderboardData;
//             break;
//         case 368:
//             message = new CCSUsrMsg_PlayerDecalDigitalSignature;
//             break;
//         case 369:
//             message = new CCSUsrMsg_WeaponSound;
//             break;
//         case 370:
//             message = new CCSUsrMsg_UpdateScreenHealthBar;
//             break;
//         case 371:
//             message = new CCSUsrMsg_EntityOutlineHighlight;
//             break;
//         case 372:
//             message = new CCSUsrMsg_SSUI;
//             break;
//         case 373:
//             message = new CCSUsrMsg_SurvivalStats;
//             break;
//         case 374:
//             message = new CCSUsrMsg_DisconnectToLobby;
//             break;
//         case 375:
//             message = new CCSUsrMsg_EndOfMatchAllPlayersData;
//             break;
//         case 376:
//             message = new CCSUsrMsg_PostRoundDamageReport;
//             break;
//         case 379:
//             message = new CCSUsrMsg_RoundEndReportData;
//             break;
//         case 380:
//             message = new CCSUsrMsg_CurrentRoundOdds;
//             break;
//         case 381:
//             message = new CCSUsrMsg_DeepStats;
//             break;
//         case 383:
//             message = new CCSUsrMsg_ShootInfo;
//             break;
//         case 400:
//             message = new CMsgTEEffectDispatch;
//             break;
//         case 401:
//             message = new CMsgTEArmorRicochet;
//             break;
//         case 402:
//             message = new CMsgTEBeamEntPoint;
//             break;
//         case 403:
//             message = new CMsgTEBeamEnts;
//             break;
//         case 404:
//             message = new CMsgTEBeamPoints;
//             break;
//         case 405:
//             message = new CMsgTEBeamRing;
//             break;
//         case 407:
//             message = new CMsgTEBSPDecal;
//             break;
//         case 408:
//             message = new CMsgTEBubbles;
//             break;
//         case 409:
//             message = new CMsgTEBubbleTrail;
//             break;
//         case 410:
//             message = new CMsgTEDecal;
//             break;
//         case 411:
//             message = new CMsgTEWorldDecal;
//             break;
//         case 412:
//             message = new CMsgTEEnergySplash;
//             break;
//         case 413:
//             message = new CMsgTEFizz;
//             break;
//         case 415:
//             message = new CMsgTEGlowSprite;
//             break;
//         case 416:
//             message = new CMsgTEImpact;
//             break;
//         case 417:
//             message = new CMsgTEMuzzleFlash;
//             break;
//         case 418:
//             message = new CMsgTEBloodStream;
//             break;
//         case 419:
//             message = new CMsgTEExplosion;
//             break;
//         case 420:
//             message = new CMsgTEDust;
//             break;
//         case 421:
//             message = new CMsgTELargeFunnel;
//             break;
//         case 422:
//             message = new CMsgTESparks;
//             break;
//         case 423:
//             message = new CMsgTEPhysicsProp;
//             break;
//         case 424:
//             message = new CMsgTEPlayerDecal;
//             break;
//         case 425:
//             message = new CMsgTEProjectedDecal;
//             break;
//         case 426:
//             message = new CMsgTESmoke;
//             break;
//         case 452:
//             message = new CMsgTEFireBullets;
//             break;
//     }
//
//     return message;
// }
