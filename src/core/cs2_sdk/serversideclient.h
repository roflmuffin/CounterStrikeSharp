#pragma once

#include "inetchannel.h"
#include "networkbasetypes.pb.h"
#include "playerslot.h"
#include "steam/steamclientpublic.h"
#include "utlstring.h"

class CServerSideClient : public INetworkMessageProcessingPreFilter
{
  public:
    virtual ~CServerSideClient() = 0;

    INetChannel* GetNetChannel() const { return m_NetChannel; };
    CPlayerSlot GetPlayerSlot() const { return m_nClientSlot; };
    CEntityIndex GetEntityIndex() const { return m_nEntityIndex; };
    CPlayerUserId GetUserID() const { return m_UserID; };
    int GetSignonState() const { return m_nSignonState; }
    CSteamID GetClientSteamID() const { return m_SteamID; }
    const char* GetClientName() const { return m_Name; };
    bool IsConnected() const { return m_nSignonState >= SIGNONSTATE_CONNECTED; };
    bool IsSpawned() const { return m_nSignonState >= SIGNONSTATE_NEW; };
    bool IsActive() const { return m_nSignonState == SIGNONSTATE_FULL; };
    bool IsFakeClient() const { return m_bFakePlayer; };
    bool IsHLTV() const { return m_bIsHLTV; }
    bool IsFullyAuthenticated() { return m_bFullyAuthenticated; }
    const netadr_t* GetRemoteAddress() const { return &m_NetAdr0; }

  private:
    [[maybe_unused]] void* m_pVT1; // INetworkMessageProcessingPreFilter
    [[maybe_unused]] char pad1[0x30];
#ifdef __linux__
    [[maybe_unused]] char pad2[0x10];
#endif
    CUtlString m_Name; // 64 | 80
    CPlayerSlot m_nClientSlot; // 72 | 88
    CEntityIndex m_nEntityIndex; // 76 | 92
    [[maybe_unused]] char pad3[0x8];
    INetChannel* m_NetChannel; // 88 | 104
    [[maybe_unused]] char pad4[0x4];
    int32 m_nSignonState; // 100 | 116
    [[maybe_unused]] char pad5[0x38];
    bool m_bFakePlayer; // 160 | 176
    [[maybe_unused]] char pad6[0x7];
    CPlayerUserId m_UserID; // 168 | 184
    [[maybe_unused]] char pad7[0x1];
    CSteamID m_SteamID; // 171 | 187
    [[maybe_unused]] char pad8[0x19];
    netadr_t m_NetAdr0; // 204 | 220
    [[maybe_unused]] char pad9[0x14];
    netadr_t m_NetAdr1; // 236 | 252
    [[maybe_unused]] char pad10[0x22];
    bool m_bIsHLTV; // 282 | 298
    [[maybe_unused]] char pad11[0x19];
    int m_nDeltaTick; // 308 | 324
    [[maybe_unused]] char pad12[0x882];
    bool m_bFullyAuthenticated; // 2490 | 2506
};
