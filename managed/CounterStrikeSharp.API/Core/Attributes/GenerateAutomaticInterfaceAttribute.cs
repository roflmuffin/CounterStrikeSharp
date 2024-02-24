namespace CounterStrikeSharp.API.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateAutomaticInterfaceAttribute : Attribute
{
    public GenerateAutomaticInterfaceAttribute(string namespaceName = "") { }
}