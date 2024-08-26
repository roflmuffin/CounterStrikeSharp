using System.Collections.ObjectModel;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.UserMessages;

public class UserMessage : NativeObject, IDisposable
{
    private RecipientFilter _recipients;

    public delegate HookResult UserMessageHandler(UserMessage native);

    public UserMessage(IntPtr pointer) : base(pointer)
    {
        Recipients = new RecipientFilter(NativeAPI.UsermessageGetrecipients(this));
        Recipients.CollectionChanged = () => NativeAPI.UsermessageSetrecipients(this, Recipients.GetRecipientMask());
    }

    /// <summary>
    /// Creates a new user message with a given network message name partial match.
    /// </summary>
    /// <param name="name"></param>
    /// <throws>if the name is not a valid network message name</throws>
    public static UserMessage FromPartialName(string name) => new(NativeAPI.UsermessageCreate(name));

    /// <summary>
    /// Creates a new user message with a given network message ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static UserMessage FromId(int id) => new(NativeAPI.UsermessageCreatebyid(id));

    /// <summary>
    /// Finds a network message ID by name.
    /// <remarks>
    /// Network message system must be loaded before this function can be used.
    /// Avoid calling this method from <see cref="IPlugin.Load"/>
    /// </remarks>
    /// </summary>
    public static int FindIdByName(string name) => NativeAPI.UsermessageFindmessageidbyname(name);

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

    public void SetUInt64(string fieldName, ulong value, int? index = null) =>
        NativeAPI.PbSetint64(this, fieldName, (long)value, index ?? -1);

    public void SetFloat(string fieldName, float value, int? index = null) => NativeAPI.PbSetfloat(this, fieldName, value, index ?? -1);

    public void SetDouble(string fieldName, double value, int? index = null) =>
        NativeAPI.PbSetfloat(this, fieldName, (float)value, index ?? -1);

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

    public void Send() => NativeAPI.UsermessageSend(this);

    public void Send(RecipientFilter recipientFilter)
    {
        Recipients = recipientFilter;
        Send();
    }

    /// <summary>
    /// Returns the network message name of this user message.
    /// <example>CMsgTEFireBullets [452]</example>
    /// </summary>
    public string Name => NativeAPI.UsermessageGetname(this);

    /// <summary>
    /// Returns the network message ID of this user message.
    /// <example>452</example>
    /// </summary>
    public int Id => NativeAPI.UsermessageGetid(this);

    /// <summary>
    /// Returns the protobuf message type of this user message.
    /// <example>CMsgTEFireBullets</example>
    /// </summary>
    public string Type => NativeAPI.UsermessageGettype(this);

    /// <summary>
    /// Returns the debug string of this user message, as defined by protobuf.
    /// <example>
    /// <code>
    /// CMsgTEFireBullets [452], 452, origin {
    ///     x: -1969.24951
    ///     y: 2046.12683
    ///     z: 60.9242516
    /// }
    /// angles {
    /// x: 32.8350143
    /// y: 19.0894909
    /// z: 0
    /// }
    /// weapon_id: 2441476
    /// mode: 1
    /// seed: 991759080
    /// player: 6390016
    /// inaccuracy: 0.00490000239
    /// recoil_index: 0
    /// spread: 0.0015
    /// sound_type: 9
    /// item_def_index: 61
    /// sound_dsp_effect: 2005810340
    /// ent_origin {
    ///     x: -1969.24976
    ///     y: 2046.12671
    ///     z: -1.92980957
    /// }
    /// num_bullets_remaining: 12
    /// attack_type: 0
    /// </code>
    /// </example>
    /// </summary>
    public string DebugString => NativeAPI.PbGetdebugstring(this);

    public RecipientFilter Recipients
    {
        get => _recipients;
        set
        {
            _recipients = value;
            NativeAPI.UsermessageSetrecipients(this, _recipients.GetRecipientMask());
        }
    }

    private void ReleaseUnmanagedResources()
    {
        Server.NextFrame(() => { NativeAPI.UsermessageDelete(this); });
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
