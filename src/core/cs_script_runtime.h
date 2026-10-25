#pragma once

#include <cstdint>

namespace counterstrikesharp { class ScriptContext; }

namespace counterstrikesharp::cs_script {
// All calls, including destruction, belong to the game thread. No V8 types
// or handles cross the native dispatcher boundary.
void Dispatch(ScriptContext& context);
void OnMapStart();
void OnGameFrame(bool simulating);
void OnEntityDeleted(uint32_t handle);
void OnMapEnd();
void OnShutdown();
}
