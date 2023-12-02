using System.Runtime.Serialization;

namespace CounterStrikeSharp.API.Modules.Entities.Constants;

/// <summary>
/// Represents the types of Steam accounts.
/// </summary>
public enum SteamAccountType
{
    /// <summary>
    /// Invalid account type.
    /// </summary>
    [EnumMember(Value = "I")]
    Invalid = 0,
    /// <summary>
    /// Individual account type.
    /// </summary>
    [EnumMember(Value = "U")]
    Individual,
    /// <summary>
    /// MultiSeat account type.
    /// </summary>
    [EnumMember(Value = "M")]
    MultiSeat,
    /// <summary>
    /// Game Server account type.
    /// </summary>
    [EnumMember(Value = "G")]
    GameServer,
    /// <summary>
    /// Anonymous Game Server account type.
    /// </summary>
    [EnumMember(Value = "A")]
    AnonGameServer,
    /// <summary>
    /// Pending account type.
    /// </summary>
    [EnumMember(Value = "P")]
    Pending,
    /// <summary>
    /// Content Server account type.
    /// </summary>
    [EnumMember(Value = "C")]
    ContentServer,
    /// <summary>
    /// Clan account type.
    /// </summary>
    [EnumMember(Value = "g")]
    Clan,
    /// <summary>
    /// Chat account type.
    /// </summary>
    [EnumMember(Value = "T")]
    Chat,
    /// <summary>
    /// Console user account type.
    /// </summary>
    [EnumMember(Value = "c")]
    ConsoleUser,
    /// <summary>
    /// P2P Super Seeder account type.
    /// </summary>
    [EnumMember(Value = " ")]
    P2PSuperSeeder,
    /// <summary>
    /// Anonymous user account type.
    /// </summary>
    [EnumMember(Value = "a")]
    AnonUser,
}
