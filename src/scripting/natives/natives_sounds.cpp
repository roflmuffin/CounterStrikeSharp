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
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>.
 */

#include "core/log.h"
#include "core/sound_event.h"
#include "scripting/autonative.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp {

static SoundEvent* GetSoundEvent(ScriptContext& script_context)
{
    auto soundEvent = script_context.GetArgument<SoundEvent*>(0);
    if (!soundEvent)
    {
        script_context.ThrowNativeError("Invalid sound event pointer");
        return nullptr;
    }

    return soundEvent;
}

static void SoundEventCreate(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    if (!name || name[0] == '\0')
    {
        script_context.ThrowNativeError("Sound event name cannot be empty");
        return;
    }

    script_context.SetResult(new SoundEvent(name));
}

static void SoundEventRelease(ScriptContext& script_context)
{
    auto soundEvent = GetSoundEvent(script_context);
    if (!soundEvent) return;

    delete soundEvent;
}

static void SoundEventSetFloat(ScriptContext& script_context)
{
    auto soundEvent = GetSoundEvent(script_context);
    if (!soundEvent) return;

    soundEvent->SetFloat(script_context.GetArgument<const char*>(1), script_context.GetArgument<float>(2));
}

static void SoundEventSetInt(ScriptContext& script_context)
{
    auto soundEvent = GetSoundEvent(script_context);
    if (!soundEvent) return;

    soundEvent->SetInt(script_context.GetArgument<const char*>(1), script_context.GetArgument<int32>(2));
}

static void SoundEventSetVector(ScriptContext& script_context)
{
    auto soundEvent = GetSoundEvent(script_context);
    if (!soundEvent) return;

    auto value = script_context.GetArgument<Vector*>(2);
    if (!value)
    {
        script_context.ThrowNativeError("Invalid vector pointer");
        return;
    }

    soundEvent->SetVector(script_context.GetArgument<const char*>(1), *value);
}

static int32 SoundEventEmit(ScriptContext& script_context)
{
    auto soundEvent = GetSoundEvent(script_context);
    if (!soundEvent) return 0;

    auto sourceEntityIndex = script_context.GetArgument<int32>(1);
    auto recipientMask = script_context.GetArgument<uint64>(2);

    return soundEvent->Emit(sourceEntityIndex, recipientMask);
}

static void SoundEventStop(ScriptContext& script_context)
{
    auto guid = script_context.GetArgument<int32>(0);
    auto recipientMask = script_context.GetArgument<uint64>(1);

    SoundEvent::Stop(guid, recipientMask);
}

REGISTER_NATIVES(sounds, {
    ScriptEngine::RegisterNativeHandler("SOUND_EVENT_CREATE", SoundEventCreate);
    ScriptEngine::RegisterNativeHandler("SOUND_EVENT_RELEASE", SoundEventRelease);
    ScriptEngine::RegisterNativeHandler("SOUND_EVENT_SET_FLOAT", SoundEventSetFloat);
    ScriptEngine::RegisterNativeHandler("SOUND_EVENT_SET_INT", SoundEventSetInt);
    ScriptEngine::RegisterNativeHandler("SOUND_EVENT_SET_VECTOR", SoundEventSetVector);
    ScriptEngine::RegisterNativeHandler("SOUND_EVENT_EMIT", SoundEventEmit);
    ScriptEngine::RegisterNativeHandler("SOUND_EVENT_STOP", SoundEventStop);
})
} // namespace counterstrikesharp
