/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using System.Text;
using YamlDotNet.Serialization;

namespace CodeGen.Natives
{
    class Program
    {
        static void Main(string[] args)
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

                    natives.Add(new()
                    {
                        Name = nativeName,
                        ReturnType = returnType,
                        Arguments = parameters
                    });
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
}