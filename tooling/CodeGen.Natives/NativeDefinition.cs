using System.Text.RegularExpressions;

namespace CodeGen.Natives;

class NativeDefinition
{
    public NativeDefinition(string name, Dictionary<string, string> arguments, string returnType)
    {
        Name = name;
        Arguments = arguments;
        ReturnType = returnType;
    }

    public string Name { get; init; }

    public string NameCamelCase => Name.ToPascalCase();

    public Dictionary<string, string> Arguments { get; init; }

    public string ReturnType { get; init; }

    public ulong Hash
    {
        get
        {
            uint result = 5381;

            for (int i = 0; i < Name.Length; i++)
            {
                result = ((result << 5) + result) ^ Name[i];
            }

            return result;
        }
    }
}