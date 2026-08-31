/**
 * =============================================================================
 * CS2Fixes
 * Copyright (C) 2023-2026 Source2ZE
 * =============================================================================
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License, version 3.0, as published by the
 * Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
 * details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#pragma once

#include "utlstring.h"
#include "globals.h"
#include "global_listener.h"
#include "core/cs2_sdk/entity/dump.h"

namespace counterstrikesharp {

class CCSCustomHudLayout;

enum EHudPanelClassStatus_t : int
{
    k_eHudPanelClassStatus_Undefined = -1,
    k_eHudPanelClassStatus_DoesNotHaveClass = 0,
    k_eHudPanelClassStatus_HasClass = 1,
};

struct HUDPanelDialogVariableString_t
{
  private:
    virtual void unk00() {}

  public:
    HUDPanelDialogVariableString_t(uint16 nPanelIdIndex, uint16 nDialogVariableIndex, CUtlString sValue, bool bIsSet)
        : m_nPanelIdIndex(nPanelIdIndex), m_nDialogVariableIndex(nDialogVariableIndex), m_sValue(sValue), m_bIsSet(bIsSet)
    {
        // Since we're constructing a new object, the vtable pointer will be incorrect so fix it
        static const auto pVTable = counterstrikesharp::modules::server->FindVirtualTable("HUDPanelDialogVariableString_t");
        ((void**)this)[0] = pVTable;
    }

    bool operator==(const HUDPanelDialogVariableString_t& other) const
    {
        return m_nPanelIdIndex == other.m_nPanelIdIndex && m_nDialogVariableIndex == other.m_nDialogVariableIndex;
    }

    uint16 m_nPanelIdIndex;
    uint16 m_nDialogVariableIndex;
    CUtlString m_sValue;
    bool m_bIsSet;
};

struct HUDPanelHasClass_t
{
  public:
    HUDPanelHasClass_t(uint16 nPanelIdIndex, uint16 nClassNameIndex, bool bHasClass)
        : m_nPanelIdIndex(nPanelIdIndex), m_nClassNameIndex(nClassNameIndex), m_eClassStatus((EHudPanelClassStatus_t)bHasClass)
    {
    }

    bool operator==(const HUDPanelHasClass_t& other) const
    {
        return m_nPanelIdIndex == other.m_nPanelIdIndex && m_nClassNameIndex == other.m_nClassNameIndex;
    }

    uint16 m_nPanelIdIndex;
    uint16 m_nClassNameIndex;
    EHudPanelClassStatus_t m_eClassStatus;
};

class CCSCustomHudLayoutState
{
  public:
    DECLARE_SCHEMA_CLASS_INLINE(CCSCustomHudLayoutState)

    SCHEMA_FIELD(int, m_playerSlot)
    SCHEMA_FIELD(bool, m_bInputCaptureEnabled)
    SCHEMA_FIELD_POINTER(CUtlVector<HUDPanelHasClass_t>, m_vecHasClasses)
    SCHEMA_FIELD_POINTER(CUtlVector<HUDPanelDialogVariableString_t>, m_vecDialogVariableStrings)
};

class CCSCustomHudLayout : public GlobalClass
{
  public:
    DECLARE_SCHEMA_CLASS(CCSCustomHudLayout)

    SCHEMA_FIELD(CUtlSymbolLarge, m_strLayout);
    SCHEMA_FIELD_POINTER(CUtlVector<CCSCustomHudLayoutState>, m_vecPlayerLayoutStates);
    SCHEMA_FIELD_POINTER(CCSCustomHudLayoutState, m_globalLayoutState);
    SCHEMA_FIELD_POINTER(CUtlVector<CUtlString>, m_vecPanelIds);
    SCHEMA_FIELD_POINTER(CUtlVector<CUtlString>, m_vecClassNames);
    SCHEMA_FIELD_POINTER(CUtlVector<CUtlString>, m_vecDialogVariableNames);

    CCSCustomHudLayout();
    ~CCSCustomHudLayout();

    void OnAllInitialized() override;
    void OnShutdown() override;

    CCSCustomHudLayoutState& GetLayoutState(CCSPlayerController* pController = nullptr);
    void SetHasClass(std::string sPanelId, std::string sClassName, bool bHasClass, CCSPlayerController* pController = nullptr);
    void SetDialogVariableString(std::string sPanelId,
                                 std::string sVariableName,
                                 std::string sValue,
                                 CCSPlayerController* pController = nullptr);
    void SetInputCaptureEnabled(bool bEnable, CCSPlayerController* pController);
    bool IsInputCaptureEnabled(CCSPlayerController* pController);

  private:
    void Hook_ClientSvcUserMessage(CPlayerSlot slot, int um_type, uint32 size, const void* buf);
};

} // namespace counterstrikesharp
