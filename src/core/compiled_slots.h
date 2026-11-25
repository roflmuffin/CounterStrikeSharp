#pragma once

// Direct calls that stay compiled method calls (ADR-0003): calls on interfaces that never shifted a slot we use, and on
// those whose modules are not fingerprinted yet. Each called slot is still a Slot Binding; the boot Vtable Check
// compares its verified index with the index the call was compiled with (BindingDeclaration::compiledIndex), because a
// compiled call cannot follow a Heal.

#include <span>
#include <string_view>

namespace counterstrikesharp::bindings {

struct CompiledSlot
{
    std::string_view key; // the Slot Binding's Gamedata key
    int index; // the vtable index the SDK headers compiled the call with
};

// Every compiled direct call on a Slot Binding, by key.
std::span<const CompiledSlot> CompiledSlots();

} // namespace counterstrikesharp::bindings
