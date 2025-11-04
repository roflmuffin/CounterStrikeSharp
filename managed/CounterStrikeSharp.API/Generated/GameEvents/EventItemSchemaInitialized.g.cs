#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("item_schema_initialized")]
public class EventItemSchemaInitialized : GameEvent
{
    public EventItemSchemaInitialized(IntPtr pointer) : base(pointer){}
    public EventItemSchemaInitialized(bool force) : base("item_schema_initialized", force){}
    
}
#nullable restore
