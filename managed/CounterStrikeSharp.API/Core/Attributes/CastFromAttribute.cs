using System;

namespace CounterStrikeSharp.API.Core.Attributes;

/**
 * Indicates that the parameter should be pulled from the ScriptContext as the passed in type,
 * then cast/converted to the parameter type.
 */
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public class CastFromAttribute : Attribute
{
    public Type Type { get; }

    public CastFromAttribute(Type type)
    {
        Type = type;
    }
}