#include "scripting/autonative.h"
#include "core/cs_script_runtime.h"

namespace counterstrikesharp {
static void CsScript(ScriptContext& context)
{
    try
    {
        cs_script::Dispatch(context);
    }
    catch (const std::exception& error)
    {
        context.ThrowNativeError("cs_script: %s", error.what());
    }
}

REGISTER_NATIVES(cs_script, { ScriptEngine::RegisterNativeHandler("CS_SCRIPT", CsScript); })
}
