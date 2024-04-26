#include "irecipientfilter.h"
#include "core/globals.h"
#include "core/managers/player_manager.h"

class CRecipientFilter : public IRecipientFilter
{
public:
    CRecipientFilter()
    {
        m_NetworkBufType = NetChannelBufType_t::BUF_RELIABLE;
        m_bInitMessage = false;
    }

    CRecipientFilter(IRecipientFilter* source, int iExcept = -1)
    {
        m_NetworkBufType = source->GetNetworkBufType();
        m_bInitMessage = source->IsInitMessage();
        m_Recipients.RemoveAll();

        for (int i = 0; i < source->GetRecipientCount(); ++i)
        {
            if (source->GetRecipientIndex(i).Get() != iExcept)
                m_Recipients.AddToTail(source->GetRecipientIndex(i));
        }
    }

    ~CRecipientFilter() override {}

    NetChannelBufType_t GetNetworkBufType(void) const override { return m_NetworkBufType; }
    bool IsInitMessage(void) const override { return m_bInitMessage; }
    int GetRecipientCount(void) const override { return m_Recipients.Count(); }
    
    CPlayerSlot GetRecipientIndex(int slot) const override
    {
        if (slot < 0 || slot >= GetRecipientCount())
            return CPlayerSlot(-1);

        return m_Recipients[slot];
    }

    void AddAllPlayers(void)
    {
        m_Recipients.RemoveAll();

        for (int i = 0; i < 64; i++) {
            counterstrikesharp::CPlayer* player = counterstrikesharp::globals::playerManager.GetPlayerBySlot(i);
            if (player == nullptr || !player->IsConnected()) // Since the cssharp doesn't remove player from the array
                continue;

            AddRecipient(i);
        }
    }

    void AddRecipient(CPlayerSlot slot)
    {
        if (m_Recipients.Find(slot) != m_Recipients.InvalidIndex())
            return;

        m_Recipients.AddToTail(slot);
    }

private:
    NetChannelBufType_t m_NetworkBufType;
    bool m_bInitMessage;
    CUtlVectorFixed<CPlayerSlot, 64> m_Recipients;
};
