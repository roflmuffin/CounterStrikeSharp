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
            case "short":
                return "Push(";
            case "func":
            case "callback":
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
            case "short":
                return "short";
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
            case "uint64":
                return "ulong";
            case "func":
            case "callback":
                return "InputArgument";
            case "object[]":
                return "object[]";
            case "SteamID":
                return "[CastFrom(typeof(ulong))]SteamID";
            case "HookMode":
                return "HookMode";
            case "ListenOverride":
                return "ListenOverride";
            case "DataType_t":
                return "DataType";
            case "any":
                return "T";
        }

        return "object";
    }

    public static string GetCSharpTypeFromGameEventType(string type)
    {
        switch (type)
        {
            case "1":
            case "short":
            case "byte":
            case "local": // unknown
                return "int";
            case "player_controller":
            case "player_pawn":
            case "player_controller_and_pawn":
                return "CCSPlayerController";
            case "ehandle":
                return "IntPtr";
            case "uint64":
                return "ulong";
            default:
                return type;
        }
    }
}