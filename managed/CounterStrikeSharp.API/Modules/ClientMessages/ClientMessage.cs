using System.Collections.ObjectModel;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.ClientMessages;

public class ClientMessage : NativeObject, IDisposable
{
    private RecipientFilter _recipients;
	
    public delegate HookResult ClientMessageHandler(ClientMessage native);

    public ClientMessage(IntPtr pointer) : base(pointer)
    {
        Recipients = new RecipientFilter(NativeAPI.ClientMessageGetrecipients(this));
        Recipients.CollectionChanged = () => NativeAPI.ClientMessageSetrecipients(this, Recipients.GetRecipientMask());
    }

    /// <summary>
    /// Creates a new client message with a given network message name partial match.
    /// </summary>
    /// <param name="name"></param>
    /// <throws>if the name is not a valid network message name</throws>
    public static ClientMessage FromPartialName(string name) => new(NativeAPI.ClientMessageCreate(name));

    /// <summary>
    /// Creates a new client message with a given network message ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ClientMessage FromId(int id) => new(NativeAPI.ClientMessageCreatebyid(id));

    /// <summary>
    /// Finds a network message ID by name.
    /// <remarks>
    /// Network message system must be loaded before this function can be used.
    /// Avoid calling this method from <see cref="IPlugin.Load"/>
    /// </remarks>
    /// </summary>
    public static int FindIdByName(string name) => NativeAPI.ClientMessageFindmessageidbyname(name);

    public bool HasField(string fieldName) => NativeAPI.PbHasfield(this, fieldName);

    public int ReadInt(string fieldName, int? index = null) => NativeAPI.PbReadint(this, fieldName, index ?? -1);
    public uint ReadUInt(string fieldName, int? index = null) => (uint)NativeAPI.PbReadint(this, fieldName, index ?? -1);
    public long ReadInt64(string fieldName, int? index = null) => NativeAPI.PbReadint64(this, fieldName, index ?? -1);
    public ulong ReadUInt64(string fieldName, int? index = null) => (ulong)NativeAPI.PbReadint64(this, fieldName, index ?? -1);
    public float ReadFloat(string fieldName, int? index = null) => NativeAPI.PbReadfloat(this, fieldName, index ?? -1);
    public double ReadDouble(string fieldName, int? index = null) => NativeAPI.PbReadfloat(this, fieldName, index ?? -1);
    public string ReadString(string fieldName, int? index = null) => NativeAPI.PbReadstring(this, fieldName, index ?? -1);
    public byte[] ReadBytes(string fieldName, int? index = null) => NativeAPI.PbReadbytes(this, fieldName, index ?? -1);
    public bool ReadBool(string fieldName, int? index = null) => NativeAPI.PbReadbool(this, fieldName, index ?? -1);
    public void SetInt(string fieldName, int value, int? index = null) => NativeAPI.PbSetint(this, fieldName, value, index ?? -1);
    public void SetUInt(string fieldName, uint value, int? index = null) => NativeAPI.PbSetint(this, fieldName, (int)value, index ?? -1);
    public void SetInt64(string fieldName, long value, int? index = null) => NativeAPI.PbSetint64(this, fieldName, value, index ?? -1);

    public void SetUInt64(string fieldName, ulong value, int? index = null) =>
        NativeAPI.PbSetint64(this, fieldName, (long)value, index ?? -1);

    public void SetFloat(string fieldName, float value, int? index = null) => NativeAPI.PbSetfloat(this, fieldName, value, index ?? -1);

    public void SetDouble(string fieldName, double value, int? index = null) =>
        NativeAPI.PbSetfloat(this, fieldName, (float)value, index ?? -1);

    public void SetString(string fieldName, string value, int? index = null) => NativeAPI.PbSetstring(this, fieldName, value, index ?? -1);
    public void SetBytes(string fieldName, byte[] value, int? index = null) => NativeAPI.PbSetbytes(this, fieldName, value, index ?? -1);
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

    // public ClientMessage ReadMessage(string fieldName) => NativeAPI.PbReadmessage(this, fieldName);
    // public ClientMessage ReadRepeatedMessage(string fieldName, int index ) => NativeAPI.PbReadrepeatedmessage(this, fieldName, index);
    // public ClientMessage AddMessage(string fieldName) => NativeAPI.PbAddmessage(this, fieldName);

    public void Send() => NativeAPI.ClientMessageSend(this);

    public void Send(RecipientFilter recipientFilter)
    {
        Recipients = recipientFilter;
        Send();
    }

    /// <summary>
    /// Returns the network message name of this client message.
    /// <example>CMsgTEFireBullets [452]</example>
    /// </summary>
    public string Name => NativeAPI.ClientMessageGetname(this);

    /// <summary>
    /// Returns the network message ID of this client message.
    /// <example>452</example>
    /// </summary>
    public int Id => NativeAPI.ClientMessageGetid(this);

    /// <summary>
    /// Returns the network message sender of this client message.
    /// <example>0</example>
    /// </summary>
    public int Sender => NativeAPI.ClientMessageGetsender(this);

    /// <summary>
    /// Returns the protobuf message type of this client message.
    /// <example>CMsgTEFireBullets</example>
    /// </summary>
    public string Type => NativeAPI.ClientMessageGettype(this);

    /// <summary>
    /// Returns the debug string of this client message, as defined by protobuf.  
    /// </summary>
    public string DebugString => NativeAPI.PbGetdebugstring(this);

    public RecipientFilter Recipients
    {
        get => _recipients;
        set
        {
            _recipients = value;
            NativeAPI.ClientMessageSetrecipients(this, _recipients.GetRecipientMask());
        }
    }
	
    private void ReleaseUnmanagedResources()
    {
        Server.NextFrame(() => { NativeAPI.ClientMessageDelete(this); });
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~ClientMessage()
    {
        ReleaseUnmanagedResources();
    }
}
