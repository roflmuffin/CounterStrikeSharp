using CounterStrikeSharp.API.Modules.Commands;

namespace CounterStrikeSharp.API.Core.Commands;

public interface ICommandManager
{
    void RegisterCommand(CommandDefinition definition);
    
    void RemoveCommand(CommandDefinition definition);
}