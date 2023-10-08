using System.Text.RegularExpressions;

namespace CodeGen.Natives;

class NativeDefinition
{
    public string Name { get; set; }

    public string NameCamelCase => Name.ToPascalCase();

    public Dictionary<string, string> Arguments { get; set; }

    public string ReturnType { get; set; }

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