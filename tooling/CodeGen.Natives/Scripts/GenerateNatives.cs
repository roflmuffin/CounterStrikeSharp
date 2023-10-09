using System.Text;
using YamlDotNet.Serialization;

namespace CodeGen.Natives.Scripts;

public partial class Generators
{
    public static void GenerateNatives()
    {
        var pathToSearch = Path.Join(Helpers.GetRootDirectory(), "src/scripting/natives");

        var deserializer = new DeserializerBuilder()
            .Build();

        var natives = new List<NativeDefinition>();
        foreach (string file in Directory.EnumerateFiles(pathToSearch, "natives*yaml", SearchOption.AllDirectories))
        {
            var deserialized = deserializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file));

            foreach (var nativeName in deserialized.Keys)
            {
                var parts = deserialized[nativeName].Split(new string[] { "->" }, StringSplitOptions.None);
                var returnType = parts[1].Trim();
                var parameterString = parts[0].Trim();

                var parameters = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(parameterString))
                {
                    // Get each parameter, then map its type to the dictionary
                    // i.e. callback: pointer, name: string
                    parameters = parameterString
                        .Split(',')
                        .Select(part => part.Split(':'))
                        .ToDictionary(
                            pair => pair[0].Trim().ToCamelCase(),
                            pair => pair[1].Trim()
                        );
                }

                natives.Add(new(nativeName, parameters, returnType));
            }
        }

        var nativesString = string.Join("\n", natives.Select(native =>
        {
            var arguments = string.Join(", ",
                native.Arguments.Select(pair => $"{Mapping.GetCSharpType(pair.Value)} {pair.Key}"));

            var returnStr = new StringBuilder($@"
        public static {Mapping.GetCSharpType(native.ReturnType)} {native.NameCamelCase}({arguments}){{{Environment.NewLine}");

            returnStr.Append("\t\t\tlock (ScriptContext.GlobalScriptContext.Lock) {\n");
            returnStr.Append("\t\t\tScriptContext.GlobalScriptContext.Reset();\n");
            foreach (var kv in native.Arguments)
            {
                if (kv.Value == "object[]")
                {
                    returnStr.Append($"\t\t\tforeach (var obj in {kv.Key})\n");
                    returnStr.Append($"\t\t\t{{\n");
                    returnStr.Append($"\t\t\t\tScriptContext.GlobalScriptContext.Push(obj);\n");
                    returnStr.Append($"\t\t\t}}\n");
                }
                else
                {
                    returnStr.Append(
                        $"\t\t\tScriptContext.GlobalScriptContext.{Mapping.GetPushType(kv.Value)}{kv.Key});\n");
                }
            }

            returnStr.Append(
                $"\t\t\tScriptContext.GlobalScriptContext.SetIdentifier({$"0x{native.Hash:X}"});\n");
            returnStr.Append("\t\t\tScriptContext.GlobalScriptContext.Invoke();\n");
            returnStr.Append("\t\t\tScriptContext.GlobalScriptContext.CheckErrors();\n");

            if (native.ReturnType != "void")
            {
                returnStr.Append(
                    $"\t\t\treturn ({Mapping.GetCSharpType(native.ReturnType)})ScriptContext.GlobalScriptContext.GetResult(typeof({Mapping.GetCSharpType(native.ReturnType)}));\n");
            }

            returnStr.Append("\t\t\t}\n");
            returnStr.Append("\t\t}");

            return returnStr.ToString();
        }));

        var result = $@"
using System;

namespace CounterStrikeSharp.API.Core
{{
    public class NativeAPI {{
        {nativesString}
    }}
}}
";

        Console.WriteLine($"Generated C# bindings for {natives.Count} methods successfully.");

        File.WriteAllText(Path.Join(Helpers.GetRootDirectory(), "managed/CounterStrikeSharp.API/Core/API.cs"),
            result);
    }
}