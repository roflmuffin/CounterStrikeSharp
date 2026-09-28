#include "core/log.h"
#include "scripting/script_engine.h"
#include "scripting/autonative.h"

#include "core/customhudlayout.h"

namespace counterstrikesharp {

void SetHasClass(ScriptContext& scriptContext)
{
    auto pCustomHudLayout = scriptContext.GetArgument<CCSCustomHudLayout*>(0);

    if (!pCustomHudLayout)
    {
        scriptContext.ThrowNativeError("CCSCustomHudLayout is null");
        return;
    }

    auto szPanelId = scriptContext.GetArgument<const char*>(1);
    auto szClassName = scriptContext.GetArgument<const char*>(2);
    auto bHasClass = scriptContext.GetArgument<bool>(3);

    pCustomHudLayout->SetHasClass(szPanelId, szClassName, bHasClass);
}

void SetHasClassForPlayer(ScriptContext& scriptContext)
{
    auto pCustomHudLayout = scriptContext.GetArgument<CCSCustomHudLayout*>(0);

    if (!pCustomHudLayout)
    {
        scriptContext.ThrowNativeError("CCSCustomHudLayout is null");
        return;
    }

    auto pController = scriptContext.GetArgument<CCSPlayerController*>(1);
    auto szPanelId = scriptContext.GetArgument<const char*>(2);
    auto szClassName = scriptContext.GetArgument<const char*>(3);
    auto bHasClass = scriptContext.GetArgument<bool>(4);

    pCustomHudLayout->SetHasClass(szPanelId, szClassName, bHasClass, pController);
}

void SetDialogVariableString(ScriptContext& scriptContext)
{
    auto pCustomHudLayout = scriptContext.GetArgument<CCSCustomHudLayout*>(0);

    if (!pCustomHudLayout)
    {
        scriptContext.ThrowNativeError("CCSCustomHudLayout is null");
        return;
    }

    auto szPanelId = scriptContext.GetArgument<const char*>(1);
    auto szVariableName = scriptContext.GetArgument<const char*>(2);
    auto szValue = scriptContext.GetArgument<const char*>(3);

    pCustomHudLayout->SetDialogVariableString(szPanelId, szVariableName, szValue);
}

void SetDialogVariableStringForPlayer(ScriptContext& scriptContext)
{
    auto pCustomHudLayout = scriptContext.GetArgument<CCSCustomHudLayout*>(0);

    if (!pCustomHudLayout)
    {
        scriptContext.ThrowNativeError("CCSCustomHudLayout is null");
        return;
    }

    auto pController = scriptContext.GetArgument<CCSPlayerController*>(1);
    auto szPanelId = scriptContext.GetArgument<const char*>(2);
    auto szVariableName = scriptContext.GetArgument<const char*>(3);
    auto szValue = scriptContext.GetArgument<const char*>(4);

    pCustomHudLayout->SetDialogVariableString(szPanelId, szVariableName, szValue, pController);
}

void SetInputCaptureEnabled(ScriptContext& scriptContext)
{
    auto pCustomHudLayout = scriptContext.GetArgument<CCSCustomHudLayout*>(0);

    if (!pCustomHudLayout)
    {
        scriptContext.ThrowNativeError("CCSCustomHudLayout is null");
        return;
    }

    auto pController = scriptContext.GetArgument<CCSPlayerController*>(1);
    auto bEnable = scriptContext.GetArgument<bool>(2);

    pCustomHudLayout->SetInputCaptureEnabled(bEnable, pController);
}

bool IsInputCaptureEnabled(ScriptContext& scriptContext)
{
    auto pCustomHudLayout = scriptContext.GetArgument<CCSCustomHudLayout*>(0);

    if (!pCustomHudLayout)
    {
        scriptContext.ThrowNativeError("CCSCustomHudLayout is null");
        return false;
    }

    auto pController = scriptContext.GetArgument<CCSPlayerController*>(1);

    return pCustomHudLayout->IsInputCaptureEnabled(pController);
}

REGISTER_NATIVES(customhud, {
    ScriptEngine::RegisterNativeHandler("SET_HAS_CLASS", SetHasClass);
    ScriptEngine::RegisterNativeHandler("SET_HAS_CLASS_FOR_PLAYER", SetHasClassForPlayer);

    ScriptEngine::RegisterNativeHandler("SET_DIALOG_VARIABLE_STRING", SetDialogVariableString);
    ScriptEngine::RegisterNativeHandler("SET_DIALOG_VARIABLE_STRING_FOR_PLAYER", SetDialogVariableStringForPlayer);

    ScriptEngine::RegisterNativeHandler("SET_INPUT_CAPTURE_ENABLED", SetInputCaptureEnabled);
    ScriptEngine::RegisterNativeHandler("IS_INPUT_CAPTURE_ENABLED", IsInputCaptureEnabled);
})
} // namespace counterstrikesharp
