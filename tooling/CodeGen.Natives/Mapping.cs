namespace CodeGen.Natives;

public class Mapping
{
    /**
     * Returns the script context type for the given argument.
     */
    public static string GetPushType(string argType)
    {
        switch (argType)
        {
            case "int":
            case "uint":
            case "float":
            case "pointer":
            case "bool":
            case "double":
                return "Push(";
            case "func":
                return "Push((InputArgument)";
            case "charPtr":
                return "PushString(";
        }

        return "Push(";
    }

    /**
     * Returns the C# type for the given argument.
     */
    public static string GetCSharpType(string type)
    {
        switch (type)
        {
            case "int":
                return "int";
            case "uint":
                return "uint";
            case "bool":
                return "bool";
            case "pointer":
                return "IntPtr";
            case "string":
                return "string";
            case "float":
                return "float";
            case "double":
                return "double";
            case "void":
                return "void";
            case "func":
                return "InputArgument";
            case "object[]":
                return "object[]";
        }

        return "object";
    }
}