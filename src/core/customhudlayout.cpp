#include "log.h"
#include "customhudlayout.h"
#include "scripting/callback_manager.h"
#include "cstrike15_usermessages.pb.h"

namespace counterstrikesharp {

static ScriptCallback* on_custom_hud_clicked;

SH_DECL_HOOK4_void(IServerGameClients, ClientSvcUserMessage, SH_NOATTRIB, 0, CPlayerSlot, int, uint32, const void*);

CCSCustomHudLayout::CCSCustomHudLayout() {}

CCSCustomHudLayout::~CCSCustomHudLayout() {}

void CCSCustomHudLayout::OnAllInitialized()
{
    on_custom_hud_clicked = globals::callbackManager.CreateCallback("OnCustomHudClicked");
    SH_ADD_HOOK(IServerGameClients, ClientSvcUserMessage, globals::serverGameClients,
                SH_MEMBER(this, &CCSCustomHudLayout::Hook_ClientSvcUserMessage), true);
}

void CCSCustomHudLayout::OnShutdown()
{
    SH_REMOVE_HOOK(IServerGameClients, ClientSvcUserMessage, globals::serverGameClients,
                   SH_MEMBER(this, &CCSCustomHudLayout::Hook_ClientSvcUserMessage), true);
}

void CCSCustomHudLayout::Hook_ClientSvcUserMessage(CPlayerSlot slot, int um_type, uint32 size, const void* buf)
{
    if (um_type == CS_UM_CustomHudClicked)
    {
        CCSUsrMsg_CustomHudClicked message;
        if (message.ParseFromArray(buf, size))
        {
            CEntityInstance* pCustomLayout = CEntityHandle::FromPackedInt(message.custom_hud_layout()).Get();
            std::string sButtonID = message.button_id();

            on_custom_hud_clicked->ScriptContext().Reset();
            on_custom_hud_clicked->ScriptContext().Push(globals::entitySystem->GetEntityInstance(CEntityIndex(slot.Get() + 1)));
            on_custom_hud_clicked->ScriptContext().Push(pCustomLayout);
            on_custom_hud_clicked->ScriptContext().Push(sButtonID.c_str());
            on_custom_hud_clicked->Execute();
        }
    }

    RETURN_META(MRES_IGNORED);
}

CCSCustomHudLayoutState& CCSCustomHudLayout::GetLayoutState(CCSPlayerController* pController)
{
    if (!pController) return *m_globalLayoutState;

    return *(CCSCustomHudLayoutState*)m_vecPlayerLayoutStates.GetManipulator()(
        SCHEMA_COLLECTION_MANIPULATOR_ACTION_GET_ELEMENT, m_vecPlayerLayoutStates, pController->GetEntityIndex().Get() - 1, 0);
}

void CCSCustomHudLayout::SetHasClass(std::string sPanelId, std::string sClassName, bool bHasClass, CCSPlayerController* pController)
{
    auto panelIndex = m_vecPanelIds()->Find(sPanelId.c_str());
    if (panelIndex == -1) panelIndex = m_vecPanelIds()->AddToTail(sPanelId.c_str());

    auto classIndex = m_vecClassNames()->Find(sClassName.c_str());
    if (classIndex == -1) classIndex = m_vecClassNames()->AddToTail(sClassName.c_str());

    auto& layoutState = GetLayoutState(pController);

    HUDPanelHasClass_t hasClass(panelIndex, classIndex, bHasClass);
    auto hasClassIndex = layoutState.m_vecHasClasses()->Find(hasClass);
    if (hasClassIndex == -1) layoutState.m_vecHasClasses()->AddToTail(hasClass);
    else
        layoutState.m_vecHasClasses()->Element(hasClassIndex).m_eClassStatus = hasClass.m_eClassStatus;
}
void CCSCustomHudLayout::SetDialogVariableString(std::string sPanelId,
                                                 std::string sVariableName,
                                                 std::string sValue,
                                                 CCSPlayerController* pController)
{
    auto panelIndex = m_vecPanelIds()->Find(sPanelId.c_str());
    if (panelIndex == -1) panelIndex = m_vecPanelIds()->AddToTail(sPanelId.c_str());

    auto variableIndex = m_vecDialogVariableNames()->Find(sVariableName.c_str());
    if (variableIndex == -1) variableIndex = m_vecDialogVariableNames()->AddToTail(sVariableName.c_str());

    auto& layoutState = GetLayoutState(pController);

    HUDPanelDialogVariableString_t dialogVariable(panelIndex, variableIndex, sValue.c_str(), true);
    auto dialogVariableIndex = layoutState.m_vecDialogVariableStrings()->Find(dialogVariable);
    if (dialogVariableIndex == -1) layoutState.m_vecDialogVariableStrings()->AddToTail(dialogVariable);
    else
        layoutState.m_vecDialogVariableStrings()->Element(dialogVariableIndex).m_sValue = sValue.c_str();
}
void CCSCustomHudLayout::SetInputCaptureEnabled(bool bEnable, CCSPlayerController* pController)
{
    GetLayoutState(pController).m_bInputCaptureEnabled() = bEnable;
}
bool CCSCustomHudLayout::IsInputCaptureEnabled(CCSPlayerController* pController)
{
    return GetLayoutState(pController).m_bInputCaptureEnabled();
}

} // namespace counterstrikesharp
