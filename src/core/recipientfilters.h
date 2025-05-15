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

#include "irecipientfilter.h"

class CSingleRecipientFilter : public IRecipientFilter
{
  public:
    CSingleRecipientFilter(CPlayerSlot iRecipient, NetChannelBufType_t nBufType = BUF_RELIABLE, bool bInitMessage = false)
        : m_iRecipient(iRecipient), m_nBufType(nBufType), m_bInitMessage(bInitMessage)
    {
    }

    ~CSingleRecipientFilter() override {}

    NetChannelBufType_t GetNetworkBufType(void) const override { return m_nBufType; }
    bool IsInitMessage(void) const override { return m_bInitMessage; }
    int GetRecipientCount(void) const override { return 1; }
    CPlayerSlot GetRecipientIndex(int slot) const override { return m_iRecipient; }

  private:
    CPlayerSlot m_iRecipient;
    NetChannelBufType_t m_nBufType;
    bool m_bInitMessage;
};

class CRecipientFilter : public IRecipientFilter
{
  public:
    CRecipientFilter()
    {
        m_nBufType = BUF_RELIABLE;
        m_bInitMessage = false;
    }

    ~CRecipientFilter() override {}

    NetChannelBufType_t GetNetworkBufType(void) const override { return m_nBufType; }
    bool IsInitMessage(void) const override { return m_bInitMessage; }
    int GetRecipientCount(void) const override { return m_Recipients.Count(); }

    CPlayerSlot GetRecipientIndex(int slot) const override
    {
        if (slot < 0 || slot >= GetRecipientCount()) return CPlayerSlot(-1);

        return m_Recipients[slot];
    }

    void AddRecipient(CPlayerSlot slot)
    {
        if (m_Recipients.Find(slot) != m_Recipients.InvalidIndex()) return;

        m_Recipients.AddToTail(slot);
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

  private:
    NetChannelBufType_t m_nBufType;
    bool m_bInitMessage;
    CUtlVectorFixed<CPlayerSlot, 64> m_Recipients;
};
