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
*  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
*/

#pragma once

#include <map>
#include <vector>

#include "core/globals.h"
#include "core/global_listener.h"
#include "scripting/script_engine.h"
#include "entitysystem.h"

namespace counterstrikesharp {
class ScriptCallback;

class CEntityListener : public IEntityListener {
    void OnEntitySpawned(CEntityInstance *pEntity) override;
    void OnEntityCreated(CEntityInstance *pEntity) override;
    void OnEntityDeleted(CEntityInstance *pEntity) override;
    void OnEntityParentChanged(CEntityInstance *pEntity, CEntityInstance *pNewParent) override;
};

class EntityManager : public GlobalClass {
    friend CEntityListener;
public:
    EntityManager();
    ~EntityManager();
    void OnAllInitialized() override;
    void OnShutdown() override;
    CEntityListener entityListener;
private:
    ScriptCallback *on_entity_spawned_callback;
    ScriptCallback *on_entity_created_callback;
    ScriptCallback *on_entity_deleted_callback;
    ScriptCallback *on_entity_parent_changed_callback;
};

}  // namespace counterstrikesharp