using System;

namespace CounterStrikeSharp.API.Modules.Memory;

/// <summary>
/// Thrown when a schema field is accessed that does not exist in the running game build, either because its class
/// or the field itself is missing from the schema. Nothing is read or written in that case.
/// </summary>
public class SchemaFieldNotFoundException : Exception
{
    public SchemaFieldNotFoundException(string className, string fieldName, bool classMissing)
        : base($"Schema field '{className}::{fieldName}' not found ({(classMissing ? "class missing" : "field missing")})")
    {
        ClassName = className;
        FieldName = fieldName;
        ClassMissing = classMissing;
    }

    /// <summary>The schema class that was looked up.</summary>
    public string ClassName { get; }

    /// <summary>The field that was looked up on <see cref="ClassName"/>.</summary>
    public string FieldName { get; }

    /// <summary>True if the whole class is missing from the schema; false if only the field is.</summary>
    public bool ClassMissing { get; }
}
