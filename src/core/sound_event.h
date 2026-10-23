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

#pragma once

#include <vector>

#include "mathlib/vector.h"
#include "platform.h"

namespace counterstrikesharp {

// Sound event names are hashed with their own seed rather than the string token seed that
// the sound operator parameter names use.
#define SOUNDEVENT_MURMURHASH_SEED 0x53524332

// Builds the parameter blob carried by CMsgSosStartSoundEvent. The blob is a flat sequence of
// parameters, each one a 32 bit hash of the parameter name, a type tag, a 16 bit length and
// then the value.
class SoundEvent
{
  public:
    explicit SoundEvent(const char* pszSoundEventName);

    void SetFloat(const char* pszParameterName, float value);
    void SetInt(const char* pszParameterName, int32 value);
    void SetVector(const char* pszParameterName, const Vector& value);

    // Returns the guid the sound was started with, which Stop takes to end it early.
    int32 Emit(int32 nSourceEntityIndex, uint64 recipientMask);

    static void Stop(int32 nGuid, uint64 recipientMask);

  private:
    enum ParameterType : uint8
    {
        Parameter_Int = 0x02,
        Parameter_Float = 0x08,
        Parameter_Vector = 0x0A,
    };

    void SetParameter(const char* pszParameterName, ParameterType type, const void* pValue, uint16 nSize);

    uint32 m_nSoundEventHash;
    std::vector<uint8> m_packedParameters;
};

} // namespace counterstrikesharp
