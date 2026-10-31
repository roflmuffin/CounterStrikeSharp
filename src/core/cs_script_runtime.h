#pragma once

#include <cstdint>

namespace counterstrikesharp {
class ScriptContext;
}

namespace counterstrikesharp::cs_script {

void GetHost(ScriptContext& context);
void IsReady(ScriptContext& context);
void Invoke(ScriptContext& context);
void OnMapStart();
void OnGameFrame(bool simulating);
void OnEntityDeleted(uint32_t handle);
void OnMapEnd();
void OnShutdown();
} // namespace counterstrikesharp::cs_script
