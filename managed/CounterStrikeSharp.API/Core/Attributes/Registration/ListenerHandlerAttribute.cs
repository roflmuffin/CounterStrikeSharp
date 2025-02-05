using System;

namespace CounterStrikeSharp.API.Core.Attributes.Registration;

[AttributeUsage(AttributeTargets.Method)]
public class ListenerHandlerAttribute<T> : Attribute
    where T: Delegate
{
}