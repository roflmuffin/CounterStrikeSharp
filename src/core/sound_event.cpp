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
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>.
 */

#include "core/sound_event.h"

#include <cstring>

#include "core/globals.h"
#include "core/log.h"
#include "core/recipientfilters.h"
#include "gameevents.pb.h"
#include "igameeventsystem.h"
#include "networksystem/inetworkmessages.h"
#include "tier1/generichash.h"
#include "tier1/utlstringtoken.h"

namespace counterstrikesharp {

namespace {
// The game numbers its own sound events up from zero, so start well clear of them. Sharing a
// guid with a game sound would let one Stop call silence the other.
int32 g_nNextSoundEventGuid = 1 << 24;
} // namespace

SoundEvent::SoundEvent(const char* pszSoundEventName)
    : m_nSoundEventHash(MurmurHash2LowerCase(pszSoundEventName, SOUNDEVENT_MURMURHASH_SEED))
{
}

void SoundEvent::SetParameter(const char* pszParameterName, ParameterType type, const void* pValue, uint16 nSize)
{
    // A parameter is a 32 bit name hash, a type tag, a 16 bit length, then the value.
    static constexpr size_t kHeaderSize = sizeof(uint32) + sizeof(uint8) + sizeof(uint16);

    const uint32 nParameterHash = MurmurHash2LowerCase(pszParameterName, STRINGTOKEN_MURMURHASH_SEED);
    const uint8 nType = static_cast<uint8>(type);

    // Drop any earlier value for this parameter, so that setting one twice sends it once.
    for (size_t at = 0; at + kHeaderSize <= m_packedParameters.size();)
    {
        uint32 nExistingHash;
        uint16 nExistingSize;
        memcpy(&nExistingHash, &m_packedParameters[at], sizeof(nExistingHash));
        memcpy(&nExistingSize, &m_packedParameters[at + sizeof(uint32) + sizeof(uint8)], sizeof(nExistingSize));

        const size_t next = at + kHeaderSize + nExistingSize;
        if (next > m_packedParameters.size()) break;

        if (nExistingHash == nParameterHash)
        {
            m_packedParameters.erase(m_packedParameters.begin() + at, m_packedParameters.begin() + next);
            break;
        }

        at = next;
    }

    // Values go out in the byte order they are already in, which is what the client reads.
    const size_t at = m_packedParameters.size();
    m_packedParameters.resize(at + kHeaderSize + nSize);

    uint8* pOut = m_packedParameters.data() + at;
    memcpy(pOut, &nParameterHash, sizeof(nParameterHash));
    pOut += sizeof(nParameterHash);
    memcpy(pOut, &nType, sizeof(nType));
    pOut += sizeof(nType);
    memcpy(pOut, &nSize, sizeof(nSize));
    pOut += sizeof(nSize);
    memcpy(pOut, pValue, nSize);
}

void SoundEvent::SetFloat(const char* pszParameterName, float value)
{
    SetParameter(pszParameterName, Parameter_Float, &value, sizeof(value));
}

void SoundEvent::SetInt(const char* pszParameterName, int32 value) { SetParameter(pszParameterName, Parameter_Int, &value, sizeof(value)); }

void SoundEvent::SetVector(const char* pszParameterName, const Vector& value)
{
    const float coordinates[3] = { value.x, value.y, value.z };
    SetParameter(pszParameterName, Parameter_Vector, coordinates, sizeof(coordinates));
}

int32 SoundEvent::Emit(int32 nSourceEntityIndex, uint64 recipientMask)
{
    INetworkMessageInternal* pNetMsg = globals::networkMessages->FindNetworkMessageById(GE_SosStartSoundEvent);
    if (!pNetMsg)
    {
        CSSHARP_CORE_ERROR("[SoundEvent] Failed to find the SosStartSoundEvent network message");
        return 0;
    }

    const int32 nGuid = g_nNextSoundEventGuid++;

    auto msg = pNetMsg->AllocateMessage()->ToPB<CMsgSosStartSoundEvent>();
    msg->set_soundevent_guid(nGuid);
    msg->set_soundevent_hash(m_nSoundEventHash);
    msg->set_source_entity_index(nSourceEntityIndex);
    msg->set_seed(nGuid);

    if (!m_packedParameters.empty())
    {
        msg->set_packed_params(m_packedParameters.data(), m_packedParameters.size());
    }

    CRecipientFilter filter{};
    filter.AddRecipientsFromMask(recipientMask);

    globals::gameEventSystem->PostEventAbstract(-1, false, &filter, pNetMsg, msg, 0);

    delete msg;
    return nGuid;
}

void SoundEvent::Stop(int32 nGuid, uint64 recipientMask)
{
    INetworkMessageInternal* pNetMsg = globals::networkMessages->FindNetworkMessageById(GE_SosStopSoundEvent);
    if (!pNetMsg)
    {
        CSSHARP_CORE_ERROR("[SoundEvent] Failed to find the SosStopSoundEvent network message");
        return;
    }

    auto msg = pNetMsg->AllocateMessage()->ToPB<CMsgSosStopSoundEvent>();
    msg->set_soundevent_guid(nGuid);

    CRecipientFilter filter{};
    filter.AddRecipientsFromMask(recipientMask);

    globals::gameEventSystem->PostEventAbstract(-1, false, &filter, pNetMsg, msg, 0);

    delete msg;
}

} // namespace counterstrikesharp
