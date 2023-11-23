namespace CounterStrikeSharp.API.Modules.Targets;

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

    IdUserid, // #4
    IdSteamEscaped, // #STEAM_0_1_8614
    IdSteam64, // #76561198116940237

    ExplicitName, // #name
    ImplicitName, // name
    Invalid
}