namespace CounterStrikeSharp.API.Modules.UserMessages;

public class UserMessage : NativeObject
{
    public delegate HookResult UserMessageHandler(UserMessage native);
    
    public UserMessage(IntPtr pointer) : base(pointer)
    {
    }

    public bool HasField(string fieldName) => NativeAPI.UsermessageHasfield(this, fieldName);
    
    public int ReadInt(string fieldName) => NativeAPI.UsermessageReadint(this, fieldName);
    
    public void SetInt(string fieldName, int value) => NativeAPI.UsermessageSetint(this, fieldName, value);
}