using System.Text.RegularExpressions;

namespace CodeGen.Natives;

public static class Helpers
{
    /**
     * Recurses parent folders until it finds the root git directory.
     */
    public static string GetRootDirectory()
    {
        var path = Environment.CurrentDirectory;
        while (!Directory.Exists(Path.Join(path, "src")))
        {
            path = Path.Join(path, "..");
        }

        return path;
    }
    
    public static string ToPascalCase(this string str)
    {
        return Regex.Replace(str.ToLower(), "(?:^|_| +)(.)", match => match.Groups[1].Value.ToUpper());
    }
    
    public static string ToCamelCase(this string str)
    {
        str = str.ToPascalCase();
        return str[0].ToString().ToLower() + str.Substring(1);
    }
}