namespace CounterStrikeSharp.API.Modules.Entities.Constants;

/// <summary>
/// Represents the universe of a Steam account.
/// </summary>
public enum SteamAccountUniverse
{
    /// <summary>
    /// Invalid universe.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// Public universe.
    /// </summary>
    Public = 1,

    /// <summary>
    /// Beta universe.
    /// </summary>
    Beta = 2,

    /// <summary>
    /// Internal universe.
    /// </summary>
    Internal = 3,

    /// <summary>
    /// Development universe.
    /// </summary>
    Dev = 4,
}
