using System;

namespace CounterStrikeSharp.API.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class EventNameAttribute : Attribute
{
    public string Name { get; init; }

    public EventNameAttribute(string name)
    {
        Name = name;
    }
}