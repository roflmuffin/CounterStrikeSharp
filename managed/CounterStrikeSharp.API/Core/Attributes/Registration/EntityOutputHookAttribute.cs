using System;

namespace CounterStrikeSharp.API.Core.Attributes.Registration;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class EntityOutputHookAttribute : Attribute
{
    public string Classname { get; }
    public string OutputName { get; }

    public EntityOutputHookAttribute(string classname, string outputName)
    {
        Classname = classname;
        OutputName = outputName;
    }
}