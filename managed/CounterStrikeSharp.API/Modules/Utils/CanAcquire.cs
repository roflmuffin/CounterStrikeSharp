namespace CounterStrikeSharp.API.Modules.Utils;

public enum AcquireMethod : int
{
    PickUp = 0,
    Buy,
};

public enum AcquireResult : int
{
    Allowed = 0,
    InvalidItem,
    AlreadyOwned,
    AlreadyPurchased,
    ReachedGrenadeTypeLimit,
    ReachedGrenadeTotalLimit,
    NotAllowedByTeam,
    NotAllowedByMap,
    NotAllowedByMode,
    NotAllowedForPurchase,
    NotAllowedByProhibition,
};
