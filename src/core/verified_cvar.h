#pragma once

// The ICvar CounterStrikeSharp calls through: g_pCVar, which the SDK's convar.cpp (ConVarRef, ConCommandRef, ConCommand,
// CConVar<T>, CCommand::Tokenize) uses as well as our own direct calls.
//
// On Linux it is a forwarding ICvar: every ICvar method CounterStrikeSharp uses is a Slot Binding ("ICvar::<Method>",
// CCvar in tier0, ADR-0003) and is called on the engine's CCvar at its verified (or healed) slot index, never at the
// compiled header position. A method whose Slot Binding failed does nothing and returns an empty value (invalid
// reference, nullptr, false); the Engine Features using it are disabled, so plugin-visible APIs refuse before reaching
// it. Methods CounterStrikeSharp never calls have no Slot Binding and are never called at their compiled header
// position: they are refused (logged once, empty value). GetMaxSplitScreenSlots (not virtual) reads a copy taken when
// the forwarding ICvar is created.
//
// On Windows (no Vtable Check) it is the engine's ICvar itself: compiled calls, as before Slot Bindings.

class ICvar;

namespace counterstrikesharp::bindings {

// Points the returned ICvar at engine's methods through the core Resolution Report (each call reads the current slot
// index). Call after ResolveCoreBindings and assign to g_pCVar before anything uses ConVars or commands; calling it
// again rebuilds it (plugin reload).
ICvar* VerifiedCvar(ICvar* engine);

} // namespace counterstrikesharp::bindings
