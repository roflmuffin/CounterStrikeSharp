#include "soundflags.h"
#include "vector.h"
#include "entityidentity.h"
#include "utlvector.h"

typedef unsigned int SoundEventGuid_t;
struct SndOpEventGuid_t
{
    SoundEventGuid_t m_nGuid;
    unsigned long m_hStackHash;
};

// used with EmitSound_t
enum gender_t : char
{
    GENDER_NONE = 0x0,
    GENDER_MALE = 0x1,
    GENDER_FEMALE = 0x2,
    GENDER_NAMVET = 0x3,
    GENDER_TEENGIRL = 0x4,
    GENDER_BIKER = 0x5,
    GENDER_MANAGER = 0x6,
    GENDER_GAMBLER = 0x7,
    GENDER_PRODUCER = 0x8,
    GENDER_COACH = 0x9,
    GENDER_MECHANIC = 0xA,
    GENDER_CEDA = 0xB,
    GENDER_CRAWLER = 0xC,
    GENDER_UNDISTRACTABLE = 0xD,
    GENDER_FALLEN = 0xE,
    GENDER_RIOT_CONTROL = 0xF,
    GENDER_CLOWN = 0x10,
    GENDER_JIMMY = 0x11,
    GENDER_HOSPITAL_PATIENT = 0x12,
    GENDER_BRIDE = 0x13,
    GENDER_LAST = 0x14,
};

struct EmitSound_t
{
    EmitSound_t()
        : m_nChannel(0), m_pSoundName(0), m_flVolume(VOL_NORM), m_SoundLevel(SNDLVL_NONE),
          m_nFlags(0), m_nPitch(PITCH_NORM), m_pOrigin(0), m_flSoundTime(0.0f),
          m_pflSoundDuration(0), m_bEmitCloseCaption(true), m_bWarnOnMissingCloseCaption(false),
          m_bWarnOnDirectWaveReference(false), m_nSpeakerEntity(-1), m_UtlVecSoundOrigin(),
          m_nForceGuid(0), m_SpeakerGender(GENDER_NONE)
    {
    }
    int m_nChannel;
    const char* m_pSoundName;
    float m_flVolume;
    soundlevel_t m_SoundLevel;
    int m_nFlags;
    int m_nPitch;
    const Vector* m_pOrigin;
    float m_flSoundTime;
    float* m_pflSoundDuration;
    bool m_bEmitCloseCaption;
    bool m_bWarnOnMissingCloseCaption;
    bool m_bWarnOnDirectWaveReference;
    CEntityIndex m_nSpeakerEntity;
    CUtlVector<Vector, CUtlMemory<Vector, int>> m_UtlVecSoundOrigin;
    SoundEventGuid_t m_nForceGuid;
    gender_t m_SpeakerGender;
};