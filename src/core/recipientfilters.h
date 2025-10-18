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

#pragma once
#include "irecipientfilter.h"

class CRecipientFilter : public IRecipientFilter
{
  public:
    CRecipientFilter(NetChannelBufType_t nBufType = BUF_RELIABLE, bool bInitMessage = false)
        : m_nBufType(nBufType), m_bInitMessage(bInitMessage)
    {
    }

    ~CRecipientFilter() override {}

    NetChannelBufType_t GetNetworkBufType(void) const override { return m_nBufType; }
    bool IsInitMessage(void) const override { return m_bInitMessage; }
    const CPlayerBitVec& GetRecipients(void) const override { return m_Recipients; }

    void AddRecipient(CPlayerSlot slot)
    {
        if (slot.Get() >= 0 && slot.Get() < ABSOLUTE_PLAYER_LIMIT) m_Recipients.Set(slot.Get());
    }

    void AddRecipientsFromMask(uint64 mask)
    {
        for (int i = 0; i < 64; ++i)
        {
            if (mask & (uint64)1 << i)
            {
                AddRecipient(CPlayerSlot(i));
            }
        }
    }

  protected:
    NetChannelBufType_t m_nBufType;
    bool m_bInitMessage;
    CPlayerBitVec m_Recipients;
};

class CSingleRecipientFilter : public CRecipientFilter
{
  public:
    CSingleRecipientFilter(CPlayerSlot nRecipientSlot, NetChannelBufType_t nBufType = BUF_RELIABLE, bool bInitMessage = false)
        : CRecipientFilter(nBufType, bInitMessage)
    {
        if (nRecipientSlot.Get() >= 0 && nRecipientSlot.Get() < ABSOLUTE_PLAYER_LIMIT) m_Recipients.Set(nRecipientSlot.Get());
    }
};
