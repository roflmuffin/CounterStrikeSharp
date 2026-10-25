#include "scripting/autonative.h"
#include "core/cs_script_runtime.h"

namespace counterstrikesharp {
template <void (*Handler)(ScriptContext&)> void CheckedCsScriptNative(ScriptContext& context)
{
    try
    {
        Handler(context);
    }
    catch (const std::exception& error)
    {
        context.ThrowNativeError("cs_script: %s", error.what());
    }
}

REGISTER_NATIVES(cs_script, {
    ScriptEngine::RegisterNativeHandler("CS_SCRIPT_GET_HOST", CheckedCsScriptNative<cs_script::GetHost>);
    ScriptEngine::RegisterNativeHandler("CS_SCRIPT_IS_READY", CheckedCsScriptNative<cs_script::IsReady>);
    ScriptEngine::RegisterNativeHandler("CS_SCRIPT_INVOKE", CheckedCsScriptNative<cs_script::Invoke>);
})
}
