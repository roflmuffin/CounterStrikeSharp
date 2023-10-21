namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayerController
{
    public int? UserId
    {
        get
        {
            if (EntityIndex == null) return null;
            return NativeAPI.GetUseridFromIndex((int)this.EntityIndex.Value.Value);
        }
    }
    
    public void PrintToConsole(string message)
    {
        NativeAPI.PrintToConsole((int)EntityIndex.Value.Value, message);
    }
}