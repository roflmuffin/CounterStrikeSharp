#include "core/cs2_sdk/schema.h"
#include "public/variant.h"
#include "entity2/entitysystem.h"
#include "game/shared/ehandle.h"

class CGameSceneNode;
class CCSPlayerPawn;
class CBasePlayerPawn;
class CCollisionProperty;
class CBodyComponent;

class CNetworkTransmitComponent
{
    DECLARE_SCHEMA_CLASS(CNetworkTransmitComponent)

    SCHEMA_FIELD(uint8, m_nTransmitStateOwnedCounter)
};

class GameTick_t
{
    DECLARE_SCHEMA_CLASS(GameTick_t)

    SCHEMA_FIELD(int32, m_Value)
};

enum InputBitMask_t : uint64_t
{
    // MEnumeratorIsNotAFlag
    IN_NONE = 0x0,
    // MEnumeratorIsNotAFlag
    IN_ALL = 0xffffffffffffffff,
    IN_ATTACK = 0x1,
    IN_JUMP = 0x2,
    IN_DUCK = 0x4,
    IN_FORWARD = 0x8,
    IN_BACK = 0x10,
    IN_USE = 0x20,
    IN_TURNLEFT = 0x80,
    IN_TURNRIGHT = 0x100,
    IN_MOVELEFT = 0x200,
    IN_MOVERIGHT = 0x400,
    IN_ATTACK2 = 0x800,
    IN_RELOAD = 0x2000,
    IN_SPEED = 0x10000,
    IN_JOYAUTOSPRINT = 0x20000,
    // MEnumeratorIsNotAFlag
    IN_FIRST_MOD_SPECIFIC_BIT = 0x100000000,
    IN_USEORRELOAD = 0x100000000,
    IN_SCORE = 0x200000000,
    IN_ZOOM = 0x400000000,
    IN_LOOK_AT_WEAPON = 0x800000000,
};

enum class TakeDamageFlags_t : uint64
{
    DFLAG_NONE = 0,
    DFLAG_SUPPRESS_HEALTH_CHANGES = 1, // (1 << 0)
    DFLAG_SUPPRESS_PHYSICS_FORCE = 2, // (1 << 1)
    DFLAG_SUPPRESS_EFFECTS = 4, // (1 << 2)
    DFLAG_PREVENT_DEATH = 8, // (1 << 3)
    DFLAG_FORCE_DEATH = 16, // (1 << 4)
    DFLAG_ALWAYS_GIB = 32, // (1 << 5)
    DFLAG_NEVER_GIB = 64, // (1 << 6)
    DFLAG_REMOVE_NO_RAGDOLL = 128, // (1 << 7)
    DFLAG_SUPPRESS_DAMAGE_MODIFICATION = 256, // (1 << 8)
    DFLAG_ALWAYS_FIRE_DAMAGE_EVENTS = 512, // (1 << 9)
    DFLAG_RADIUS_DMG = 1024, // (1 << 10)
    DFLAG_FORCEREDUCEARMOR_DMG = 2048, // (1 << 11)
    DFLAG_SUPPRESS_INTERRUPT_FLINCH = 4096, // (1 << 12)
    DMG_LASTDFLAG = 4096, // (1 << 12)
    DFLAG_IGNORE_ARMOR = 8192, // (1 << 13)
    DFLAG_SUPPRESS_UTILREMOVE = 16384, // (1 << 14)
};

enum class EntityPlatformTypes_t : uint8
{
    ENTITY_NOT_PLATFORM = 0,
    ENTITY_PLATFORM_PLAYER_FOLLOWS_YAW = 1,
    ENTITY_PLATFORM_PLAYER_IGNORES_YAW = 2,
};

class CEntityIOOutput
{
    DECLARE_SCHEMA_CLASS(CEntityIOOutput)
};

class CNetworkVelocityVector
{
    DECLARE_SCHEMA_CLASS(CNetworkVelocityVector)

    SCHEMA_FIELD(float, m_vecX)
    SCHEMA_FIELD(float, m_vecY)
    SCHEMA_FIELD(float, m_vecZ)
};

enum class BloodType : int32
{
    None = -1,
    ColorRed = 0,
    ColorYellow = 1,
    ColorGreen = 2,
    ColorRedLVL2 = 3,
    ColorRedLVL3 = 4,
    ColorRedLVL4 = 5,
    ColorRedLVL5 = 6,
    ColorRedLVL6 = 7,
};

class CBaseEntity : public CEntityInstance
{
    DECLARE_SCHEMA_CLASS(CBaseEntity)

    SCHEMA_FIELD(CBodyComponent*, m_CBodyComponent)
    SCHEMA_FIELD(CNetworkTransmitComponent, m_NetworkTransmitComponent)
    SCHEMA_FIELD(int32, m_iCurrentThinkContext)
    SCHEMA_FIELD(GameTick_t, m_nLastThinkTick)
    SCHEMA_FIELD(bool, m_bDisabledContextThinks)
    SCHEMA_FIELD(CBitVec<64>, m_isSteadyState)
    SCHEMA_FIELD(float32, m_lastNetworkChange)
    SCHEMA_FIELD(CUtlSymbolLarge, m_iszResponseContext)
    SCHEMA_FIELD(int32, m_iHealth)
    SCHEMA_FIELD(int32, m_iMaxHealth)
    SCHEMA_FIELD(uint8, m_lifeState)
    SCHEMA_FIELD(float32, m_flDamageAccumulator)
    SCHEMA_FIELD(bool, m_bTakesDamage)
    SCHEMA_FIELD(TakeDamageFlags_t, m_nTakeDamageFlags)
    SCHEMA_FIELD(EntityPlatformTypes_t, m_nPlatformType)
    SCHEMA_FIELD(MoveCollide_t, m_MoveCollide)
    SCHEMA_FIELD(MoveType_t, m_MoveType)
    SCHEMA_FIELD(MoveType_t, m_nActualMoveType)
    SCHEMA_FIELD(uint8, m_nWaterTouch)
    SCHEMA_FIELD(uint8, m_nSlimeTouch)
    SCHEMA_FIELD(bool, m_bRestoreInHierarchy)
    SCHEMA_FIELD(CUtlSymbolLarge, m_target)
    SCHEMA_FIELD(CUtlSymbolLarge, m_iszDamageFilterName)
    SCHEMA_FIELD(float32, m_flMoveDoneTime)
    SCHEMA_FIELD(CUtlStringToken, m_nSubclassID)
    SCHEMA_FIELD(float32, m_flAnimTime)
    SCHEMA_FIELD(float32, m_flSimulationTime)
    SCHEMA_FIELD(GameTime_t, m_flCreateTime)
    SCHEMA_FIELD(bool, m_bClientSideRagdoll)
    SCHEMA_FIELD(uint8, m_ubInterpolationFrame)
    SCHEMA_FIELD(Vector, m_vPrevVPhysicsUpdatePos)
    SCHEMA_FIELD(uint8, m_iTeamNum)
    SCHEMA_FIELD(CUtlSymbolLarge, m_iGlobalname)
    SCHEMA_FIELD(int32, m_iSentToClients)
    SCHEMA_FIELD(float32, m_flSpeed)
    SCHEMA_FIELD(CUtlString, m_sUniqueHammerID)
    SCHEMA_FIELD(uint32, m_spawnflags)
    SCHEMA_FIELD(GameTick_t, m_nNextThinkTick)
    SCHEMA_FIELD(int32, m_nSimulationTick)
    SCHEMA_FIELD(CEntityIOOutput, m_OnKilled)
    SCHEMA_FIELD(uint32, m_fFlags)
    SCHEMA_FIELD(Vector, m_vecAbsVelocity)
    SCHEMA_FIELD(CNetworkVelocityVector, m_vecVelocity)
    SCHEMA_FIELD(Vector, m_vecBaseVelocity)
    SCHEMA_FIELD(int32, m_nPushEnumCount)
    SCHEMA_FIELD(CCollisionProperty*, m_pCollision)
    SCHEMA_FIELD(CHandle<CBaseEntity>, m_hEffectEntity)
    SCHEMA_FIELD(CHandle<CBaseEntity>, m_hOwnerEntity)
    SCHEMA_FIELD(uint32, m_fEffects)
    SCHEMA_FIELD(CHandle<CBaseEntity>, m_hGroundEntity)
    SCHEMA_FIELD(int32, m_nGroundBodyIndex)
    SCHEMA_FIELD(float32, m_flFriction)
    SCHEMA_FIELD(float32, m_flElasticity)
    SCHEMA_FIELD(float32, m_flGravityScale)
    SCHEMA_FIELD(float32, m_flTimeScale)
    SCHEMA_FIELD(float32, m_flWaterLevel)
    SCHEMA_FIELD(bool, m_bAnimatedEveryTick)
    SCHEMA_FIELD(bool, m_bDisableLowViolence)
    SCHEMA_FIELD(uint8, m_nWaterType)
    SCHEMA_FIELD(int32, m_iEFlags)
    SCHEMA_FIELD(CEntityIOOutput, m_OnUser1)
    SCHEMA_FIELD(CEntityIOOutput, m_OnUser2)
    SCHEMA_FIELD(CEntityIOOutput, m_OnUser3)
    SCHEMA_FIELD(CEntityIOOutput, m_OnUser4)
    SCHEMA_FIELD(int32, m_iInitialTeamNum)
    SCHEMA_FIELD(GameTime_t, m_flNavIgnoreUntilTime)
    SCHEMA_FIELD(QAngle, m_vecAngVelocity)
    SCHEMA_FIELD(bool, m_bNetworkQuantizeOriginAndAngles)
    SCHEMA_FIELD(bool, m_bLagCompensate)
    SCHEMA_FIELD(float32, m_flOverriddenFriction)
    SCHEMA_FIELD(CHandle<CBaseEntity>, m_pBlocker)
    SCHEMA_FIELD(float32, m_flLocalTime)
    SCHEMA_FIELD(float32, m_flVPhysicsUpdateLocalTime)
    SCHEMA_FIELD(BloodType, m_nBloodType)
};

enum class PlayerConnectedState : int32
{
    PlayerNeverConnected = -1,
    PlayerConnected = 0,
    PlayerConnecting = 1,
    PlayerReconnecting = 2,
    PlayerDisconnecting = 3,
    PlayerDisconnected = 4,
    PlayerReserved = 5,
};

enum class ChatIgnoreType_t : uint32
{
    CHAT_IGNORE_NONE = 0,
    CHAT_IGNORE_ALL = 1,
    CHAT_IGNORE_TEAM = 2,
};

class CBasePlayerController : public CBaseEntity
{
  public:
    DECLARE_SCHEMA_CLASS(CBasePlayerController)

    SCHEMA_FIELD(uint64, m_nInButtonsWhichAreToggles)
    SCHEMA_FIELD(uint32, m_nTickBase)
    SCHEMA_FIELD(CHandle<CBasePlayerPawn>, m_hPawn)
    SCHEMA_FIELD(bool, m_bKnownTeamMismatch)
    SCHEMA_FIELD(CSplitScreenSlot, m_nSplitScreenSlot)
    SCHEMA_FIELD(CHandle<CBasePlayerController>, m_hSplitOwner)
    SCHEMA_FIELD(bool, m_bIsHLTV)
    SCHEMA_FIELD(PlayerConnectedState, m_iConnected)
    SCHEMA_FIELD_POINTER(char, m_iszPlayerName)
    SCHEMA_FIELD(CUtlString, m_szNetworkIDString)
    SCHEMA_FIELD(float32, m_fLerpTime)
    SCHEMA_FIELD(bool, m_bLagCompensation)
    SCHEMA_FIELD(bool, m_bPredict)
    SCHEMA_FIELD(bool, m_bIsLowViolence)
    SCHEMA_FIELD(bool, m_bGamePaused)
    SCHEMA_FIELD(ChatIgnoreType_t, m_iIgnoreGlobalChat)
    SCHEMA_FIELD(float32, m_flLastPlayerTalkTime)
    SCHEMA_FIELD(float32, m_flLastEntitySteadyState)
    SCHEMA_FIELD(int32, m_nAvailableEntitySteadyState)
    SCHEMA_FIELD(bool, m_bHasAnySteadyStateEnts)
    SCHEMA_FIELD(uint64, m_steamID)
    SCHEMA_FIELD(uint32, m_iDesiredFOV)
};

class QuestProgress
{
    DECLARE_SCHEMA_CLASS(QuestProgress)
};

class IntervalTimer
{
    DECLARE_SCHEMA_CLASS(IntervalTimer)

    SCHEMA_FIELD(GameTime_t, m_timestamp)
    SCHEMA_FIELD(WorldGroupId_t, m_nWorldGroupId)
};

class CCSPlayerController : public CBasePlayerController
{
  public:
    DECLARE_SCHEMA_CLASS(CCSPlayerController)

    SCHEMA_FIELD(uint32, m_iPing)
    SCHEMA_FIELD(bool, m_bHasCommunicationAbuseMute)
    SCHEMA_FIELD(uint32, m_uiCommunicationMuteFlags)
    SCHEMA_FIELD(CUtlSymbolLarge, m_szCrosshairCodes)
    SCHEMA_FIELD(uint8, m_iPendingTeamNum)
    SCHEMA_FIELD(GameTime_t, m_flForceTeamTime)
    SCHEMA_FIELD(int32, m_iCompTeammateColor)
    SCHEMA_FIELD(bool, m_bEverPlayedOnTeam)
    SCHEMA_FIELD(bool, m_bAttemptedToGetColor)
    SCHEMA_FIELD(int32, m_iTeammatePreferredColor)
    SCHEMA_FIELD(bool, m_bTeamChanged)
    SCHEMA_FIELD(bool, m_bInSwitchTeam)
    SCHEMA_FIELD(bool, m_bHasSeenJoinGame)
    SCHEMA_FIELD(bool, m_bJustBecameSpectator)
    SCHEMA_FIELD(bool, m_bSwitchTeamsOnNextRoundReset)
    SCHEMA_FIELD(bool, m_bRemoveAllItemsOnNextRoundReset)
    SCHEMA_FIELD(GameTime_t, m_flLastJoinTeamTime)
    SCHEMA_FIELD(CUtlSymbolLarge, m_szClan)
    SCHEMA_FIELD_POINTER(char, m_szClanName)
    SCHEMA_FIELD(int32, m_iCoachingTeam)
    SCHEMA_FIELD(uint64, m_nPlayerDominated)
    SCHEMA_FIELD(uint64, m_nPlayerDominatingMe)
    SCHEMA_FIELD(int32, m_iCompetitiveRanking)
    SCHEMA_FIELD(int32, m_iCompetitiveWins)
    SCHEMA_FIELD(int8, m_iCompetitiveRankType)
    SCHEMA_FIELD(int32, m_iCompetitiveRankingPredicted_Win)
    SCHEMA_FIELD(int32, m_iCompetitiveRankingPredicted_Loss)
    SCHEMA_FIELD(int32, m_iCompetitiveRankingPredicted_Tie)
    SCHEMA_FIELD(int32, m_nEndMatchNextMapVote)
    SCHEMA_FIELD(uint16, m_unActiveQuestId)
    SCHEMA_FIELD(uint32, m_rtActiveMissionPeriod)
    SCHEMA_FIELD(uint32, m_unPlayerTvControlFlags)
    SCHEMA_FIELD(int32, m_iDraftIndex)
    SCHEMA_FIELD(uint32, m_msQueuedModeDisconnectionTimestamp)
    SCHEMA_FIELD(uint32, m_uiAbandonRecordedReason)
    SCHEMA_FIELD(uint32, m_eNetworkDisconnectionReason)
    SCHEMA_FIELD(bool, m_bCannotBeKicked)
    SCHEMA_FIELD(bool, m_bEverFullyConnected)
    SCHEMA_FIELD(bool, m_bAbandonAllowsSurrender)
    SCHEMA_FIELD(bool, m_bAbandonOffersInstantSurrender)
    SCHEMA_FIELD(bool, m_bDisconnection1MinWarningPrinted)
    SCHEMA_FIELD(bool, m_bScoreReported)
    SCHEMA_FIELD(int32, m_nDisconnectionTick)
    SCHEMA_FIELD(bool, m_bControllingBot)
    SCHEMA_FIELD(bool, m_bHasControlledBotThisRound)
    SCHEMA_FIELD(bool, m_bHasBeenControlledByPlayerThisRound)
    SCHEMA_FIELD(int32, m_nBotsControlledThisRound)
    SCHEMA_FIELD(bool, m_bCanControlObservedBot)
    SCHEMA_FIELD(CHandle<CCSPlayerPawn>, m_hPlayerPawn)
    SCHEMA_FIELD(int32, m_DesiredObserverMode)
    SCHEMA_FIELD(CEntityHandle, m_hDesiredObserverTarget)
    SCHEMA_FIELD(bool, m_bPawnIsAlive)
    SCHEMA_FIELD(uint32, m_iPawnHealth)
    SCHEMA_FIELD(int32, m_iPawnArmor)
    SCHEMA_FIELD(bool, m_bPawnHasDefuser)
    SCHEMA_FIELD(bool, m_bPawnHasHelmet)
    SCHEMA_FIELD(uint16, m_nPawnCharacterDefIndex)
    SCHEMA_FIELD(int32, m_iPawnLifetimeStart)
    SCHEMA_FIELD(int32, m_iPawnLifetimeEnd)
    SCHEMA_FIELD(int32, m_iPawnBotDifficulty)
    SCHEMA_FIELD(CHandle<CCSPlayerController>, m_hOriginalControllerOfCurrentPawn)
    SCHEMA_FIELD(int32, m_iScore)
    SCHEMA_FIELD(int32, m_iRoundScore)
    SCHEMA_FIELD(int32, m_iRoundsWon)
    SCHEMA_FIELD_POINTER(uint8, m_recentKillQueue)
    SCHEMA_FIELD(uint8, m_nFirstKill)
    SCHEMA_FIELD(uint8, m_nKillCount)
    SCHEMA_FIELD(bool, m_bMvpNoMusic)
    SCHEMA_FIELD(int32, m_eMvpReason)
    SCHEMA_FIELD(int32, m_iMusicKitID)
    SCHEMA_FIELD(int32, m_iMusicKitMVPs)
    SCHEMA_FIELD(int32, m_iMVPs)
    SCHEMA_FIELD(int32, m_nUpdateCounter)
    SCHEMA_FIELD(float32, m_flSmoothedPing)
    SCHEMA_FIELD(IntervalTimer, m_lastHeldVoteTimer)
    SCHEMA_FIELD(bool, m_bShowHints)
    SCHEMA_FIELD(int32, m_iNextTimeCheck)
    SCHEMA_FIELD(bool, m_bJustDidTeamKill)
    SCHEMA_FIELD(bool, m_bPunishForTeamKill)
    SCHEMA_FIELD(bool, m_bGaveTeamDamageWarning)
    SCHEMA_FIELD(bool, m_bGaveTeamDamageWarningThisRound)
    SCHEMA_FIELD(float64, m_dblLastReceivedPacketPlatFloatTime)
    SCHEMA_FIELD(GameTime_t, m_LastTeamDamageWarningTime)
    SCHEMA_FIELD(GameTime_t, m_LastTimePlayerWasDisconnectedForPawnsRemove)
    SCHEMA_FIELD(uint32, m_nSuspiciousHitCount)
    SCHEMA_FIELD(uint32, m_nNonSuspiciousHitStreak)
    SCHEMA_FIELD(bool, m_bFireBulletsSeedSynchronized)
};

class CNetworkVarChainer
{
    DECLARE_SCHEMA_CLASS(CNetworkVarChainer)

    SCHEMA_FIELD(ChangeAccessorFieldPathIndex_t, m_PathIndex)
};

class CBodyComponent : public CEntityComponent
{
    DECLARE_SCHEMA_CLASS(CBodyComponent)

    SCHEMA_FIELD(CGameSceneNode*, m_pSceneNode)
    SCHEMA_FIELD(CNetworkVarChainer, __m_pChainEntity)
};

class CHitboxComponent : public CEntityComponent
{
    DECLARE_SCHEMA_CLASS(CHitboxComponent)

    SCHEMA_FIELD_POINTER(uint32, m_bvDisabledHitGroups)
};

class VPhysicsCollisionAttribute_t
{
    DECLARE_SCHEMA_CLASS(VPhysicsCollisionAttribute_t)

    SCHEMA_FIELD(uint64, m_nInteractsAs)
    SCHEMA_FIELD(uint64, m_nInteractsWith)
    SCHEMA_FIELD(uint64, m_nInteractsExclude)
    SCHEMA_FIELD(uint32, m_nEntityId)
    SCHEMA_FIELD(uint32, m_nOwnerId)
    SCHEMA_FIELD(uint16, m_nHierarchyId)
    SCHEMA_FIELD(uint8, m_nCollisionGroup)
    SCHEMA_FIELD(uint8, m_nCollisionFunctionMask)
};

enum class SurroundingBoundsType_t : uint8
{
    USE_OBB_COLLISION_BOUNDS = 0,
    USE_BEST_COLLISION_BOUNDS = 1,
    USE_HITBOXES = 2,
    USE_SPECIFIED_BOUNDS = 3,
    USE_GAME_CODE = 4,
    USE_ROTATION_EXPANDED_BOUNDS = 5,
    USE_ROTATION_EXPANDED_ORIENTED_BOUNDS = 6,
    USE_COLLISION_BOUNDS_NEVER_VPHYSICS = 7,
    USE_ROTATION_EXPANDED_SEQUENCE_BOUNDS = 8,
    SURROUNDING_TYPE_BIT_COUNT = 3,
};

class CCollisionProperty
{
    DECLARE_SCHEMA_CLASS(CCollisionProperty)

    SCHEMA_FIELD(VPhysicsCollisionAttribute_t, m_collisionAttribute)
    SCHEMA_FIELD(Vector, m_vecMins)
    SCHEMA_FIELD(Vector, m_vecMaxs)
    SCHEMA_FIELD(uint8, m_usSolidFlags)
    SCHEMA_FIELD(SolidType_t, m_nSolidType)
    SCHEMA_FIELD(uint8, m_triggerBloat)
    SCHEMA_FIELD(SurroundingBoundsType_t, m_nSurroundType)
    SCHEMA_FIELD(uint8, m_CollisionGroup)
    SCHEMA_FIELD(uint8, m_nEnablePhysics)
    SCHEMA_FIELD(float32, m_flBoundingRadius)
    SCHEMA_FIELD(Vector, m_vecSpecifiedSurroundingMins)
    SCHEMA_FIELD(Vector, m_vecSpecifiedSurroundingMaxs)
    SCHEMA_FIELD(Vector, m_vecSurroundingMaxs)
    SCHEMA_FIELD(Vector, m_vecSurroundingMins)
    SCHEMA_FIELD(Vector, m_vCapsuleCenter1)
    SCHEMA_FIELD(Vector, m_vCapsuleCenter2)
    SCHEMA_FIELD(float32, m_flCapsuleRadius)
};

class CGlowProperty
{
    DECLARE_SCHEMA_CLASS(CGlowProperty)

    SCHEMA_FIELD(Vector, m_fGlowColor)
    SCHEMA_FIELD(int32, m_iGlowType)
    SCHEMA_FIELD(int32, m_iGlowTeam)
    SCHEMA_FIELD(int32, m_nGlowRange)
    SCHEMA_FIELD(int32, m_nGlowRangeMin)
    SCHEMA_FIELD(Color, m_glowColorOverride)
    SCHEMA_FIELD(bool, m_bFlashing)
    SCHEMA_FIELD(float32, m_flGlowTime)
    SCHEMA_FIELD(float32, m_flGlowStartTime)
    SCHEMA_FIELD(bool, m_bEligibleForScreenHighlight)
    SCHEMA_FIELD(bool, m_bGlowing)
};

class CNetworkViewOffsetVector
{
    DECLARE_SCHEMA_CLASS(CNetworkViewOffsetVector)

    SCHEMA_FIELD(float, m_vecX)
    SCHEMA_FIELD(float, m_vecY)
    SCHEMA_FIELD(float, m_vecZ)
};

class CBaseModelEntity : public CBaseEntity
{
    DECLARE_SCHEMA_CLASS(CBaseModelEntity)

    SCHEMA_FIELD(CHitboxComponent, m_CHitboxComponent)
    SCHEMA_FIELD(HitGroup_t, m_nDestructiblePartInitialStateDestructed0)
    SCHEMA_FIELD(HitGroup_t, m_nDestructiblePartInitialStateDestructed1)
    SCHEMA_FIELD(HitGroup_t, m_nDestructiblePartInitialStateDestructed2)
    SCHEMA_FIELD(HitGroup_t, m_nDestructiblePartInitialStateDestructed3)
    SCHEMA_FIELD(HitGroup_t, m_nDestructiblePartInitialStateDestructed4)
    SCHEMA_FIELD(int32, m_nLastHitDestructiblePartIndex)
    SCHEMA_FIELD(HitGroup_t, m_LastHitGroup)
    SCHEMA_FIELD(GameTime_t, m_flDissolveStartTime)
    SCHEMA_FIELD(CEntityIOOutput, m_OnIgnite)
    SCHEMA_FIELD(RenderMode_t, m_nRenderMode)
    SCHEMA_FIELD(RenderFx_t, m_nRenderFX)
    SCHEMA_FIELD(bool, m_bAllowFadeInView)
    SCHEMA_FIELD(Color, m_clrRender)
    SCHEMA_FIELD(bool, m_bRenderToCubemaps)
    SCHEMA_FIELD(bool, m_bNoInterpolate)
    SCHEMA_FIELD(CCollisionProperty, m_Collision)
    SCHEMA_FIELD(CGlowProperty, m_Glow)
    SCHEMA_FIELD(float32, m_flGlowBackfaceMult)
    SCHEMA_FIELD(float32, m_fadeMinDist)
    SCHEMA_FIELD(float32, m_fadeMaxDist)
    SCHEMA_FIELD(float32, m_flFadeScale)
    SCHEMA_FIELD(float32, m_flShadowStrength)
    SCHEMA_FIELD(uint8, m_nObjectCulling)
    SCHEMA_FIELD(int32, m_nAddDecal)
    SCHEMA_FIELD(Vector, m_vDecalPosition)
    SCHEMA_FIELD(Vector, m_vDecalForwardAxis)
    SCHEMA_FIELD(float32, m_flDecalHealBloodRate)
    SCHEMA_FIELD(float32, m_flDecalHealHeightRate)
    SCHEMA_FIELD(CNetworkViewOffsetVector, m_vecViewOffset)
};

class PhysicsRagdollPose_t
{
    DECLARE_SCHEMA_CLASS(PhysicsRagdollPose_t)

    SCHEMA_FIELD(CHandle<CBaseEntity>, m_hOwner)
};

class CBaseAnimGraph : public CBaseModelEntity
{
    DECLARE_SCHEMA_CLASS(CBaseAnimGraph)

    SCHEMA_FIELD(bool, m_bInitiallyPopulateInterpHistory)
    SCHEMA_FIELD(bool, m_bAnimGraphUpdateEnabled)
    SCHEMA_FIELD(float32, m_flMaxSlopeDistance)
    SCHEMA_FIELD(Vector, m_vLastSlopeCheckPos)
    SCHEMA_FIELD(bool, m_bAnimationUpdateScheduled)
    SCHEMA_FIELD(Vector, m_vecForce)
    SCHEMA_FIELD(int32, m_nForceBone)
    SCHEMA_FIELD(PhysicsRagdollPose_t, m_RagdollPose)
    SCHEMA_FIELD(bool, m_bRagdollClientSide)
    SCHEMA_FIELD(int32, m_nLastDestructiblePartDestroyedAnimgraphSetTick)
};

class SceneEventId_t
{
    DECLARE_SCHEMA_CLASS(SceneEventId_t)

    SCHEMA_FIELD(uint32, m_Value)
};

class CBaseFlex : public CBaseAnimGraph
{
    DECLARE_SCHEMA_CLASS(CBaseFlex)

    SCHEMA_FIELD(Vector, m_vLookTargetPosition)
    SCHEMA_FIELD(bool, m_blinktoggle)
    SCHEMA_FIELD(GameTime_t, m_flAllowResponsesEndTime)
    SCHEMA_FIELD(GameTime_t, m_flLastFlexAnimationTime)
    SCHEMA_FIELD(SceneEventId_t, m_nNextSceneEventId)
    SCHEMA_FIELD(bool, m_bUpdateLayerPriorities)
};

enum class Hull_t : uint32
{
    HULL_HUMAN = 0,
    HULL_SMALL_CENTERED = 1,
    HULL_WIDE_HUMAN = 2,
    HULL_TINY = 3,
    HULL_MEDIUM = 4,
    HULL_TINY_CENTERED = 5,
    HULL_LARGE = 6,
    HULL_LARGE_CENTERED = 7,
    HULL_MEDIUM_TALL = 8,
    HULL_SMALL = 9,
    NUM_HULLS = 10,
    HULL_NONE = 11,
};

class CBaseCombatCharacter : public CBaseFlex
{
    DECLARE_SCHEMA_CLASS(CBaseCombatCharacter)

    SCHEMA_FIELD(bool, m_bForceServerRagdoll)
    SCHEMA_FIELD(float32, m_impactEnergyScale)
    SCHEMA_FIELD(bool, m_bApplyStressDamage)
    SCHEMA_FIELD(int32, m_iDamageCount)
    SCHEMA_FIELD(CUtlSymbolLarge, m_strRelationships)
    SCHEMA_FIELD(Hull_t, m_eHull)
    SCHEMA_FIELD(uint32, m_nNavHullIdx)
};

class fogparams_t
{
    DECLARE_SCHEMA_CLASS(fogparams_t)

    SCHEMA_FIELD(Vector, dirPrimary)
    SCHEMA_FIELD(Color, colorPrimary)
    SCHEMA_FIELD(Color, colorSecondary)
    SCHEMA_FIELD(Color, colorPrimaryLerpTo)
    SCHEMA_FIELD(Color, colorSecondaryLerpTo)
    SCHEMA_FIELD(float32, start)
    SCHEMA_FIELD(float32, end)
    SCHEMA_FIELD(float32, farz)
    SCHEMA_FIELD(float32, maxdensity)
    SCHEMA_FIELD(float32, exponent)
    SCHEMA_FIELD(float32, HDRColorScale)
    SCHEMA_FIELD(float32, skyboxFogFactor)
    SCHEMA_FIELD(float32, skyboxFogFactorLerpTo)
    SCHEMA_FIELD(float32, startLerpTo)
    SCHEMA_FIELD(float32, endLerpTo)
    SCHEMA_FIELD(float32, maxdensityLerpTo)
    SCHEMA_FIELD(GameTime_t, lerptime)
    SCHEMA_FIELD(float32, duration)
    SCHEMA_FIELD(float32, blendtobackground)
    SCHEMA_FIELD(float32, scattering)
    SCHEMA_FIELD(float32, locallightscale)
    SCHEMA_FIELD(bool, enable)
    SCHEMA_FIELD(bool, blend)
    SCHEMA_FIELD(bool, m_bNoReflectionFog)
    SCHEMA_FIELD(bool, m_bPadding)
};

class sky3dparams_t
{
    DECLARE_SCHEMA_CLASS(sky3dparams_t)

    SCHEMA_FIELD(int16, scale)
    SCHEMA_FIELD(Vector, origin)
    SCHEMA_FIELD(bool, bClip3DSkyBoxNearToWorldFar)
    SCHEMA_FIELD(float32, flClip3DSkyBoxNearToWorldFarOffset)
    SCHEMA_FIELD(fogparams_t, fog)
    SCHEMA_FIELD(WorldGroupId_t, m_nWorldGroupID)
};

class CInButtonState
{
  public:
    DECLARE_SCHEMA_CLASS(CInButtonState)

    SCHEMA_FIELD_POINTER(uint64, m_pButtonStates)
};

class CPlayerPawnComponent
{
    DECLARE_SCHEMA_CLASS(CPlayerPawnComponent)

    SCHEMA_FIELD(CNetworkVarChainer, __m_pChainEntity)
};

class CPlayer_MovementServices : public CPlayerPawnComponent
{
  public:
    DECLARE_SCHEMA_CLASS(CPlayer_MovementServices)

    SCHEMA_FIELD(int32, m_nImpulse)
    SCHEMA_FIELD(CInButtonState, m_nButtons)
    SCHEMA_FIELD(uint64, m_nQueuedButtonDownMask)
    SCHEMA_FIELD(uint64, m_nQueuedButtonChangeMask)
    SCHEMA_FIELD(uint64, m_nButtonDoublePressed)
    SCHEMA_FIELD_POINTER(uint32, m_pButtonPressedCmdNumber)
    SCHEMA_FIELD(uint32, m_nLastCommandNumberProcessed)
    SCHEMA_FIELD(uint64, m_nToggleButtonDownMask)
    SCHEMA_FIELD(float32, m_flMaxspeed)
    SCHEMA_FIELD_POINTER(float32, m_arrForceSubtickMoveWhen)
    SCHEMA_FIELD(float32, m_flForwardMove)
    SCHEMA_FIELD(float32, m_flLeftMove)
    SCHEMA_FIELD(float32, m_flUpMove)
    SCHEMA_FIELD(Vector, m_vecLastMovementImpulses)
    SCHEMA_FIELD(QAngle, m_vecOldViewAngles)
};

class CPlayer_MovementServices_Humanoid : public CPlayer_MovementServices
{
    DECLARE_SCHEMA_CLASS(CPlayer_MovementServices_Humanoid)

    SCHEMA_FIELD(float32, m_flStepSoundTime)
    SCHEMA_FIELD(float32, m_flFallVelocity)
    SCHEMA_FIELD(bool, m_bInCrouch)
    SCHEMA_FIELD(uint32, m_nCrouchState)
    SCHEMA_FIELD(GameTime_t, m_flCrouchTransitionStartTime)
    SCHEMA_FIELD(bool, m_bDucked)
    SCHEMA_FIELD(bool, m_bDucking)
    SCHEMA_FIELD(bool, m_bInDuckJump)
    SCHEMA_FIELD(Vector, m_groundNormal)
    SCHEMA_FIELD(float32, m_flSurfaceFriction)
    SCHEMA_FIELD(CUtlStringToken, m_surfaceProps)
    SCHEMA_FIELD(int32, m_nStepside)
    SCHEMA_FIELD(int32, m_iTargetVolume)
    SCHEMA_FIELD(Vector, m_vecSmoothedVelocity)
};

class CCSPlayer_MovementServices : public CPlayer_MovementServices_Humanoid
{
    DECLARE_SCHEMA_CLASS(CCSPlayer_MovementServices)

    SCHEMA_FIELD(Vector, m_vecLadderNormal)
    SCHEMA_FIELD(int32, m_nLadderSurfacePropIndex)
    SCHEMA_FIELD(float32, m_flDuckAmount)
    SCHEMA_FIELD(float32, m_flDuckSpeed)
    SCHEMA_FIELD(bool, m_bDuckOverride)
    SCHEMA_FIELD(bool, m_bDesiresDuck)
    SCHEMA_FIELD(float32, m_flDuckOffset)
    SCHEMA_FIELD(uint32, m_nDuckTimeMsecs)
    SCHEMA_FIELD(uint32, m_nDuckJumpTimeMsecs)
    SCHEMA_FIELD(uint32, m_nJumpTimeMsecs)
    SCHEMA_FIELD(float32, m_flLastDuckTime)
    SCHEMA_FIELD(Vector2D, m_vecLastPositionAtFullCrouchSpeed)
    SCHEMA_FIELD(bool, m_duckUntilOnGround)
    SCHEMA_FIELD(bool, m_bHasWalkMovedSinceLastJump)
    SCHEMA_FIELD(bool, m_bInStuckTest)
    SCHEMA_FIELD_POINTER(float32, m_flStuckCheckTime)
    SCHEMA_FIELD(int32, m_nTraceCount)
    SCHEMA_FIELD(int32, m_StuckLast)
    SCHEMA_FIELD(bool, m_bSpeedCropped)
    SCHEMA_FIELD(float32, m_flGroundMoveEfficiency)
    SCHEMA_FIELD(int32, m_nOldWaterLevel)
    SCHEMA_FIELD(float32, m_flWaterEntryTime)
    SCHEMA_FIELD(Vector, m_vecForward)
    SCHEMA_FIELD(Vector, m_vecLeft)
    SCHEMA_FIELD(Vector, m_vecUp)
    SCHEMA_FIELD(int32, m_nGameCodeHasMovedPlayerAfterCommand)
    SCHEMA_FIELD(bool, m_bMadeFootstepNoise)
    SCHEMA_FIELD(int32, m_iFootsteps)
    SCHEMA_FIELD(bool, m_bOldJumpPressed)
    SCHEMA_FIELD(float32, m_flJumpPressedTime)
    SCHEMA_FIELD(GameTime_t, m_fStashGrenadeParameterWhen)
    SCHEMA_FIELD(uint64, m_nButtonDownMaskPrev)
    SCHEMA_FIELD(float32, m_flOffsetTickCompleteTime)
    SCHEMA_FIELD(float32, m_flOffsetTickStashedSpeed)
    SCHEMA_FIELD(float32, m_flStamina)
    SCHEMA_FIELD(float32, m_flHeightAtJumpStart)
    SCHEMA_FIELD(float32, m_flMaxJumpHeightThisJump)
    SCHEMA_FIELD(float32, m_flMaxJumpHeightLastJump)
    SCHEMA_FIELD(float32, m_flStaminaAtJumpStart)
    SCHEMA_FIELD(float32, m_flAccumulatedJumpError)
};

class CBasePlayerPawn : public CBaseCombatCharacter
{
  public:
    DECLARE_SCHEMA_CLASS(CBasePlayerPawn)

    SCHEMA_FIELD(CPlayer_MovementServices*, m_pMovementServices)
    SCHEMA_FIELD(uint32, m_nHighestGeneratedServerViewAngleChangeIndex)
    SCHEMA_FIELD(QAngle, v_angle)
    SCHEMA_FIELD(QAngle, v_anglePrevious)
    SCHEMA_FIELD(uint32, m_iHideHUD)
    SCHEMA_FIELD(sky3dparams_t, m_skybox3d)
    SCHEMA_FIELD(GameTime_t, m_fTimeLastHurt)
    SCHEMA_FIELD(GameTime_t, m_flDeathTime)
    SCHEMA_FIELD(GameTime_t, m_fNextSuicideTime)
    SCHEMA_FIELD(bool, m_fInitHUD)
    SCHEMA_FIELD(CHandle<CBasePlayerController>, m_hController)
    SCHEMA_FIELD(float32, m_fHltvReplayDelay)
    SCHEMA_FIELD(float32, m_fHltvReplayEnd)
    SCHEMA_FIELD(CEntityIndex, m_iHltvReplayEntity)
};

class CTouchExpansionComponent : public CEntityComponent
{
    DECLARE_SCHEMA_CLASS(CTouchExpansionComponent)
};

enum class CSPlayerState : uint32
{
    STATE_ACTIVE = 0,
    STATE_WELCOME = 1,
    STATE_PICKINGTEAM = 2,
    STATE_PICKINGCLASS = 3,
    STATE_DEATH_ANIM = 4,
    STATE_DEATH_WAIT_FOR_KEY = 5,
    STATE_OBSERVER_MODE = 6,
    STATE_GUNGAME_RESPAWN = 7,
    STATE_DORMANT = 8,
    NUM_PLAYER_STATES = 9,
};

class CCSPlayerPawnBase : public CBasePlayerPawn
{
    DECLARE_SCHEMA_CLASS(CCSPlayerPawnBase)

    SCHEMA_FIELD(CTouchExpansionComponent, m_CTouchExpansionComponent)
    SCHEMA_FIELD(GameTime_t, m_blindUntilTime)
    SCHEMA_FIELD(GameTime_t, m_blindStartTime)
    SCHEMA_FIELD(CSPlayerState, m_iPlayerState)
    SCHEMA_FIELD(bool, m_bRespawning)
    SCHEMA_FIELD(GameTime_t, m_fImmuneToGunGameDamageTime)
    SCHEMA_FIELD(bool, m_bGunGameImmunity)
    SCHEMA_FIELD(float32, m_fMolotovDamageTime)
    SCHEMA_FIELD(bool, m_bHasMovedSinceSpawn)
    SCHEMA_FIELD(int32, m_iNumSpawns)
    SCHEMA_FIELD(float32, m_flIdleTimeSinceLastAction)
    SCHEMA_FIELD(float32, m_fNextRadarUpdateTime)
    SCHEMA_FIELD(float32, m_flFlashDuration)
    SCHEMA_FIELD(float32, m_flFlashMaxAlpha)
    SCHEMA_FIELD(float32, m_flProgressBarStartTime)
    SCHEMA_FIELD(int32, m_iProgressBarDuration)
    SCHEMA_FIELD(QAngle, m_angEyeAngles)
    SCHEMA_FIELD(bool, m_wasNotKilledNaturally)
    SCHEMA_FIELD(bool, m_bCommittingSuicideOnTeamChange)
    SCHEMA_FIELD(CHandle<CCSPlayerController>, m_hOriginalController)
};

enum class loadout_slot_t : int32
{
    LOADOUT_SLOT_PROMOTED = -2,
    LOADOUT_SLOT_INVALID = -1,
    LOADOUT_SLOT_MELEE = 0,
    LOADOUT_SLOT_C4 = 1,
    LOADOUT_SLOT_FIRST_AUTO_BUY_WEAPON = 0,
    LOADOUT_SLOT_LAST_AUTO_BUY_WEAPON = 1,
    LOADOUT_SLOT_SECONDARY0 = 2,
    LOADOUT_SLOT_SECONDARY1 = 3,
    LOADOUT_SLOT_SECONDARY2 = 4,
    LOADOUT_SLOT_SECONDARY3 = 5,
    LOADOUT_SLOT_SECONDARY4 = 6,
    LOADOUT_SLOT_SECONDARY5 = 7,
    LOADOUT_SLOT_SMG0 = 8,
    LOADOUT_SLOT_SMG1 = 9,
    LOADOUT_SLOT_SMG2 = 10,
    LOADOUT_SLOT_SMG3 = 11,
    LOADOUT_SLOT_SMG4 = 12,
    LOADOUT_SLOT_SMG5 = 13,
    LOADOUT_SLOT_RIFLE0 = 14,
    LOADOUT_SLOT_RIFLE1 = 15,
    LOADOUT_SLOT_RIFLE2 = 16,
    LOADOUT_SLOT_RIFLE3 = 17,
    LOADOUT_SLOT_RIFLE4 = 18,
    LOADOUT_SLOT_RIFLE5 = 19,
    LOADOUT_SLOT_HEAVY0 = 20,
    LOADOUT_SLOT_HEAVY1 = 21,
    LOADOUT_SLOT_HEAVY2 = 22,
    LOADOUT_SLOT_HEAVY3 = 23,
    LOADOUT_SLOT_HEAVY4 = 24,
    LOADOUT_SLOT_HEAVY5 = 25,
    LOADOUT_SLOT_FIRST_WHEEL_WEAPON = 2,
    LOADOUT_SLOT_LAST_WHEEL_WEAPON = 25,
    LOADOUT_SLOT_FIRST_PRIMARY_WEAPON = 8,
    LOADOUT_SLOT_LAST_PRIMARY_WEAPON = 25,
    LOADOUT_SLOT_FIRST_WHEEL_GRENADE = 26,
    LOADOUT_SLOT_GRENADE0 = 26,
    LOADOUT_SLOT_GRENADE1 = 27,
    LOADOUT_SLOT_GRENADE2 = 28,
    LOADOUT_SLOT_GRENADE3 = 29,
    LOADOUT_SLOT_GRENADE4 = 30,
    LOADOUT_SLOT_GRENADE5 = 31,
    LOADOUT_SLOT_LAST_WHEEL_GRENADE = 31,
    LOADOUT_SLOT_EQUIPMENT0 = 32,
    LOADOUT_SLOT_EQUIPMENT1 = 33,
    LOADOUT_SLOT_EQUIPMENT2 = 34,
    LOADOUT_SLOT_EQUIPMENT3 = 35,
    LOADOUT_SLOT_EQUIPMENT4 = 36,
    LOADOUT_SLOT_EQUIPMENT5 = 37,
    LOADOUT_SLOT_FIRST_WHEEL_EQUIPMENT = 32,
    LOADOUT_SLOT_LAST_WHEEL_EQUIPMENT = 37,
    LOADOUT_SLOT_CLOTHING_CUSTOMPLAYER = 38,
    LOADOUT_SLOT_CLOTHING_CUSTOMHEAD = 39,
    LOADOUT_SLOT_CLOTHING_FACEMASK = 40,
    LOADOUT_SLOT_CLOTHING_HANDS = 41,
    LOADOUT_SLOT_FIRST_COSMETIC = 41,
    LOADOUT_SLOT_LAST_COSMETIC = 41,
    LOADOUT_SLOT_CLOTHING_EYEWEAR = 42,
    LOADOUT_SLOT_CLOTHING_HAT = 43,
    LOADOUT_SLOT_CLOTHING_LOWERBODY = 44,
    LOADOUT_SLOT_CLOTHING_TORSO = 45,
    LOADOUT_SLOT_CLOTHING_APPEARANCE = 46,
    LOADOUT_SLOT_MISC0 = 47,
    LOADOUT_SLOT_MISC1 = 48,
    LOADOUT_SLOT_MISC2 = 49,
    LOADOUT_SLOT_MISC3 = 50,
    LOADOUT_SLOT_MISC4 = 51,
    LOADOUT_SLOT_MISC5 = 52,
    LOADOUT_SLOT_MISC6 = 53,
    LOADOUT_SLOT_MUSICKIT = 54,
    LOADOUT_SLOT_FLAIR0 = 55,
    LOADOUT_SLOT_SPRAY0 = 56,
    LOADOUT_SLOT_FIRST_ALL_CHARACTER = 54,
    LOADOUT_SLOT_LAST_ALL_CHARACTER = 56,
    LOADOUT_SLOT_COUNT = 57,
};

class IEconItemInterface
{
    DECLARE_SCHEMA_CLASS(IEconItemInterface)
};

class CAttributeList
{
    DECLARE_SCHEMA_CLASS(CAttributeList)
};

class CEconItemView : public IEconItemInterface
{
    DECLARE_SCHEMA_CLASS(CEconItemView)

    SCHEMA_FIELD(uint16, m_iItemDefinitionIndex)
    SCHEMA_FIELD(int32, m_iEntityQuality)
    SCHEMA_FIELD(uint32, m_iEntityLevel)
    SCHEMA_FIELD(uint64, m_iItemID)
    SCHEMA_FIELD(uint32, m_iItemIDHigh)
    SCHEMA_FIELD(uint32, m_iItemIDLow)
    SCHEMA_FIELD(uint32, m_iAccountID)
    SCHEMA_FIELD(uint32, m_iInventoryPosition)
    SCHEMA_FIELD(bool, m_bInitialized)
    SCHEMA_FIELD(CAttributeList, m_AttributeList)
    SCHEMA_FIELD(CAttributeList, m_NetworkedDynamicAttributes)
    SCHEMA_FIELD_POINTER(char, m_szCustomName)
    SCHEMA_FIELD_POINTER(char, m_szCustomNameOverride)
};

class EntitySpottedState_t
{
    DECLARE_SCHEMA_CLASS(EntitySpottedState_t)

    SCHEMA_FIELD(bool, m_bSpotted)
    SCHEMA_FIELD_POINTER(uint32, m_bSpottedByMask)
};

enum class CSPlayerBlockingUseAction_t : uint32
{
    k_CSPlayerBlockingUseAction_None = 0,
    k_CSPlayerBlockingUseAction_DefusingDefault = 1,
    k_CSPlayerBlockingUseAction_DefusingWithKit = 2,
    k_CSPlayerBlockingUseAction_HostageGrabbing = 3,
    k_CSPlayerBlockingUseAction_HostageDropping = 4,
    k_CSPlayerBlockingUseAction_MapLongUseEntity_Pickup = 5,
    k_CSPlayerBlockingUseAction_MapLongUseEntity_Place = 6,
    k_CSPlayerBlockingUseAction_MaxCount = 7,
};

class CCSPlayerPawn : public CCSPlayerPawnBase
{
    DECLARE_SCHEMA_CLASS(CCSPlayerPawn)

    SCHEMA_FIELD(uint16, m_nCharacterDefIndex)
    SCHEMA_FIELD(bool, m_bHasFemaleVoice)
    SCHEMA_FIELD(CUtlString, m_strVOPrefix)
    SCHEMA_FIELD_POINTER(char, m_szLastPlaceName)
    SCHEMA_FIELD(bool, m_bInHostageResetZone)
    SCHEMA_FIELD(bool, m_bInBuyZone)
    SCHEMA_FIELD(bool, m_bWasInBuyZone)
    SCHEMA_FIELD(bool, m_bInHostageRescueZone)
    SCHEMA_FIELD(bool, m_bInBombZone)
    SCHEMA_FIELD(bool, m_bWasInHostageRescueZone)
    SCHEMA_FIELD(int32, m_iRetakesOffering)
    SCHEMA_FIELD(int32, m_iRetakesOfferingCard)
    SCHEMA_FIELD(bool, m_bRetakesHasDefuseKit)
    SCHEMA_FIELD(bool, m_bRetakesMVPLastRound)
    SCHEMA_FIELD(int32, m_iRetakesMVPBoostItem)
    SCHEMA_FIELD(loadout_slot_t, m_RetakesMVPBoostExtraUtility)
    SCHEMA_FIELD(GameTime_t, m_flHealthShotBoostExpirationTime)
    SCHEMA_FIELD(float32, m_flLandingTimeSeconds)
    SCHEMA_FIELD(QAngle, m_aimPunchAngle)
    SCHEMA_FIELD(QAngle, m_aimPunchAngleVel)
    SCHEMA_FIELD(int32, m_aimPunchTickBase)
    SCHEMA_FIELD(float32, m_aimPunchTickFraction)
    SCHEMA_FIELD(bool, m_bIsBuyMenuOpen)
    SCHEMA_FIELD(CTransform, m_xLastHeadBoneTransform)
    SCHEMA_FIELD(bool, m_bLastHeadBoneTransformIsValid)
    SCHEMA_FIELD(GameTime_t, m_lastLandTime)
    SCHEMA_FIELD(bool, m_bOnGroundLastTick)
    SCHEMA_FIELD(int32, m_iPlayerLocked)
    SCHEMA_FIELD(GameTime_t, m_flTimeOfLastInjury)
    SCHEMA_FIELD(GameTime_t, m_flNextSprayDecalTime)
    SCHEMA_FIELD(bool, m_bNextSprayDecalTimeExpedited)
    SCHEMA_FIELD(int32, m_nRagdollDamageBone)
    SCHEMA_FIELD(Vector, m_vRagdollDamageForce)
    SCHEMA_FIELD(Vector, m_vRagdollDamagePosition)
    SCHEMA_FIELD_POINTER(char, m_szRagdollDamageWeaponName)
    SCHEMA_FIELD(bool, m_bRagdollDamageHeadshot)
    SCHEMA_FIELD(Vector, m_vRagdollServerOrigin)
    SCHEMA_FIELD(CEconItemView, m_EconGloves)
    SCHEMA_FIELD(uint8, m_nEconGlovesChanged)
    SCHEMA_FIELD(QAngle, m_qDeathEyeAngles)
    SCHEMA_FIELD(bool, m_bSkipOneHeadConstraintUpdate)
    SCHEMA_FIELD(bool, m_bLeftHanded)
    SCHEMA_FIELD(GameTime_t, m_fSwitchedHandednessTime)
    SCHEMA_FIELD(float32, m_flViewmodelOffsetX)
    SCHEMA_FIELD(float32, m_flViewmodelOffsetY)
    SCHEMA_FIELD(float32, m_flViewmodelOffsetZ)
    SCHEMA_FIELD(float32, m_flViewmodelFOV)
    SCHEMA_FIELD(bool, m_bIsWalking)
    SCHEMA_FIELD(float32, m_fLastGivenDefuserTime)
    SCHEMA_FIELD(float32, m_fLastGivenBombTime)
    SCHEMA_FIELD(float32, m_flDealtDamageToEnemyMostRecentTimestamp)
    SCHEMA_FIELD(uint32, m_iDisplayHistoryBits)
    SCHEMA_FIELD(float32, m_flLastAttackedTeammate)
    SCHEMA_FIELD(GameTime_t, m_allowAutoFollowTime)
    SCHEMA_FIELD(bool, m_bResetArmorNextSpawn)
    SCHEMA_FIELD(CEntityIndex, m_nLastKillerIndex)
    SCHEMA_FIELD(EntitySpottedState_t, m_entitySpottedState)
    SCHEMA_FIELD(int32, m_nSpotRules)
    SCHEMA_FIELD(bool, m_bIsScoped)
    SCHEMA_FIELD(bool, m_bResumeZoom)
    SCHEMA_FIELD(bool, m_bIsDefusing)
    SCHEMA_FIELD(bool, m_bIsGrabbingHostage)
    SCHEMA_FIELD(CSPlayerBlockingUseAction_t, m_iBlockingUseActionInProgress)
    SCHEMA_FIELD(GameTime_t, m_flEmitSoundTime)
    SCHEMA_FIELD(bool, m_bInNoDefuseArea)
    SCHEMA_FIELD(CEntityIndex, m_iBombSiteIndex)
    SCHEMA_FIELD(int32, m_nWhichBombZone)
    SCHEMA_FIELD(bool, m_bInBombZoneTrigger)
    SCHEMA_FIELD(bool, m_bWasInBombZoneTrigger)
    SCHEMA_FIELD(int32, m_iShotsFired)
    SCHEMA_FIELD(float32, m_flFlinchStack)
    SCHEMA_FIELD(float32, m_flVelocityModifier)
    SCHEMA_FIELD(float32, m_flHitHeading)
    SCHEMA_FIELD(int32, m_nHitBodyPart)
    SCHEMA_FIELD(Vector, m_vecTotalBulletForce)
    SCHEMA_FIELD(bool, m_bWaitForNoAttack)
    SCHEMA_FIELD(float32, m_ignoreLadderJumpTime)
    SCHEMA_FIELD(bool, m_bKilledByHeadshot)
    SCHEMA_FIELD(int32, m_LastHitBox)
    SCHEMA_FIELD(int32, m_LastHealth)
    SCHEMA_FIELD(bool, m_bBotAllowActive)
    SCHEMA_FIELD(QAngle, m_thirdPersonHeading)
    SCHEMA_FIELD(float32, m_flSlopeDropOffset)
    SCHEMA_FIELD(float32, m_flSlopeDropHeight)
    SCHEMA_FIELD(Vector, m_vHeadConstraintOffset)
    SCHEMA_FIELD(int32, m_nLastPickupPriority)
    SCHEMA_FIELD(float32, m_flLastPickupPriorityTime)
    SCHEMA_FIELD(int32, m_ArmorValue)
    SCHEMA_FIELD(uint16, m_unCurrentEquipmentValue)
    SCHEMA_FIELD(uint16, m_unRoundStartEquipmentValue)
    SCHEMA_FIELD(uint16, m_unFreezetimeEndEquipmentValue)
    SCHEMA_FIELD(int32, m_iLastWeaponFireUsercmd)
    SCHEMA_FIELD(bool, m_bIsSpawning)
    SCHEMA_FIELD(int32, m_iDeathFlags)
    SCHEMA_FIELD(bool, m_bHasDeathInfo)
    SCHEMA_FIELD(float32, m_flDeathInfoTime)
    SCHEMA_FIELD(Vector, m_vecDeathInfoOrigin)
    SCHEMA_FIELD_POINTER(uint32, m_vecPlayerPatchEconIndices)
    SCHEMA_FIELD(Color, m_GunGameImmunityColor)
    SCHEMA_FIELD(GameTime_t, m_grenadeParameterStashTime)
    SCHEMA_FIELD(bool, m_bGrenadeParametersStashed)
    SCHEMA_FIELD(QAngle, m_angStashedShootAngles)
    SCHEMA_FIELD(Vector, m_vecStashedGrenadeThrowPosition)
    SCHEMA_FIELD(Vector, m_vecStashedVelocity)
    SCHEMA_FIELD_POINTER(QAngle, m_angShootAngleHistory)
    SCHEMA_FIELD_POINTER(Vector, m_vecThrowPositionHistory)
    SCHEMA_FIELD_POINTER(Vector, m_vecVelocityHistory)
    SCHEMA_FIELD(int32, m_nHighestAppliedDamageTagTick)
};

class CGameSceneNode
{
    DECLARE_SCHEMA_CLASS(CGameSceneNode)

    SCHEMA_FIELD(CTransform, m_nodeToWorld)
    SCHEMA_FIELD(CEntityInstance*, m_pOwner)
    SCHEMA_FIELD(CGameSceneNode*, m_pParent)
    SCHEMA_FIELD(CGameSceneNode*, m_pChild)
    SCHEMA_FIELD(CGameSceneNode*, m_pNextSibling)
    SCHEMA_FIELD(QAngle, m_angRotation)
    SCHEMA_FIELD(float32, m_flScale)
    SCHEMA_FIELD(Vector, m_vecAbsOrigin)
    SCHEMA_FIELD(QAngle, m_angAbsRotation)
    SCHEMA_FIELD(float32, m_flAbsScale)
    SCHEMA_FIELD(int16, m_nParentAttachmentOrBone)
    SCHEMA_FIELD(bool, m_bDebugAbsOriginChanges)
    SCHEMA_FIELD(bool, m_bDormant)
    SCHEMA_FIELD(bool, m_bForceParentToBeNetworked)

    SCHEMA_FIELD(uint8, m_nHierarchicalDepth)
    SCHEMA_FIELD(uint8, m_nHierarchyType)
    SCHEMA_FIELD(uint8, m_nDoNotSetAnimTimeInInvalidatePhysicsCount)
    SCHEMA_FIELD(CUtlStringToken, m_name)
    SCHEMA_FIELD(CUtlStringToken, m_hierarchyAttachName)
    SCHEMA_FIELD(float32, m_flZOffset)
    SCHEMA_FIELD(float32, m_flClientLocalScale)
    SCHEMA_FIELD(Vector, m_vRenderOrigin)
};
