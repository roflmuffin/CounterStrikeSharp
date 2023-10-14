using System;

namespace CounterStrikeSharp.API.Core.Attributes;

[AttributeUsage(AttributeTargets.Delegate, Inherited = false)]
public class ListenerNameAttribute : Attribute
{
    public string Name { get; init; }

    public ListenerNameAttribute(string name)
    {
        Name = name;
    }
}