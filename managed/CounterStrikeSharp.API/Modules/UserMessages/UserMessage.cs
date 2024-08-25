namespace CounterStrikeSharp.API.Modules.UserMessages;

public class UserMessage : NativeObject, IDisposable
{
    public delegate HookResult UserMessageHandler(UserMessage native);

    public UserMessage(IntPtr pointer) : base(pointer)
    {
    }

    public bool HasField(string fieldName) => NativeAPI.PbHasfield(this, fieldName);

    public int ReadInt(string fieldName, int? index = null) => NativeAPI.PbReadint(this, fieldName, index ?? -1);
    public uint ReadUInt(string fieldName, int? index = null) => (uint)NativeAPI.PbReadint(this, fieldName, index ?? -1);
    public long ReadInt64(string fieldName, int? index = null) => NativeAPI.PbReadint64(this, fieldName, index ?? -1);
    public ulong ReadUInt64(string fieldName, int? index = null) => (ulong)NativeAPI.PbReadint64(this, fieldName, index ?? -1);
    public float ReadFloat(string fieldName, int? index = null) => NativeAPI.PbReadfloat(this, fieldName, index ?? -1);
    public double ReadDouble(string fieldName, int? index = null) => NativeAPI.PbReadfloat(this, fieldName, index ?? -1);
    public string ReadString(string fieldName, int? index = null) => NativeAPI.PbReadstring(this, fieldName, index ?? -1);
    public bool ReadBool(string fieldName, int? index = null) => NativeAPI.PbReadbool(this, fieldName, index ?? -1);

    public void SetInt(string fieldName, int value, int? index = null) => NativeAPI.PbSetint(this, fieldName, value, index ?? -1);
    public void SetUInt(string fieldName, uint value, int? index = null) => NativeAPI.PbSetint(this, fieldName, (int)value, index ?? -1);
    public void SetInt64(string fieldName, long value, int? index = null) => NativeAPI.PbSetint64(this, fieldName, value, index ?? -1);
    public void SetUInt64(string fieldName, ulong value, int? index = null) => NativeAPI.PbSetint64(this, fieldName, (long)value, index ?? -1);
    public void SetFloat(string fieldName, float value, int? index = null) => NativeAPI.PbSetfloat(this, fieldName, value, index ?? -1);
    public void SetDouble(string fieldName, double value, int? index = null) => NativeAPI.PbSetfloat(this, fieldName, (float)value, index ?? -1);
    public void SetString(string fieldName, string value, int? index = null) => NativeAPI.PbSetstring(this, fieldName, value, index ?? -1);
    public void SetBool(string fieldName, bool value, int? index = null) => NativeAPI.PbSetbool(this, fieldName, value, index ?? -1);

    public int GetRepeatedFieldCount(string fieldName) => NativeAPI.PbGetrepeatedfieldcount(this, fieldName);

    public void RemoveRepeatedField(string fieldName, int index) => NativeAPI.PbRemoverepeatedfieldvalue(this, fieldName, index);

    public void AddInt(string fieldName, int value) => NativeAPI.PbAddint(this, fieldName, value);
    public void AddUInt(string fieldName, uint value) => NativeAPI.PbAddint(this, fieldName, (int)value);
    public void AddInt64(string fieldName, long value) => NativeAPI.PbAddint64(this, fieldName, value);
    public void AddUInt64(string fieldName, ulong value) => NativeAPI.PbAddint64(this, fieldName, (long)value);
    public void AddFloat(string fieldName, float value) => NativeAPI.PbAddfloat(this, fieldName, value);
    public void AddDouble(string fieldName, double value) => NativeAPI.PbAddfloat(this, fieldName, (float)value);
    public void AddString(string fieldName, string value) => NativeAPI.PbAddstring(this, fieldName, value);
    public void AddBool(string fieldName, bool value) => NativeAPI.PbAddbool(this, fieldName, value);
    // public UserMessage ReadMessage(string fieldName) => NativeAPI.PbReadmessage(this, fieldName);
    // public UserMessage ReadRepeatedMessage(string fieldName, int index ) => NativeAPI.PbReadrepeatedmessage(this, fieldName, index);
    // public UserMessage AddMessage(string fieldName) => NativeAPI.PbAddmessage(this, fieldName);

    public string DebugString => NativeAPI.PbGetdebugstring(this);

    private void ReleaseUnmanagedResources()
    {
        Server.NextFrame(() =>
        {
            NativeAPI.UsermessageDelete(this);
        });
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~UserMessage()
    {
        ReleaseUnmanagedResources();
    }
}
