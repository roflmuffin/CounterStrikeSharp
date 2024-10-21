namespace CounterStrikeSharp.API.Modules.Commands.Targeting;

public enum TargetType
{
    TeamCt, // @ct
    TeamT, // @t
    TeamSpec, // @spec

    GroupAll, // @all
    GroupBots, // @bots
    GroupHumans, // @human
    GroupAlive, // @alive
    GroupDead, // @dead
    GroupNotMe, // @!me

    PlayerMe, // @me
    PlayerAim, // @aim

    IdUserid, // #4
    IdSteamEscaped, // "#STEAM_0:1:8614"
    IdSteam64, // #76561198116940237

    ExplicitName, // #name
    ImplicitName, // name
    Invalid
}
