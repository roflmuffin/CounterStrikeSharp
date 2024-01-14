namespace CounterStrikeSharp.API.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SchemaMemberAttribute : Attribute
{
    public string ClassName { get; }
    public string MemberName { get; }

    public SchemaMemberAttribute(string className, string memberName)
    {
        ClassName = className;
        MemberName = memberName;
    }
}