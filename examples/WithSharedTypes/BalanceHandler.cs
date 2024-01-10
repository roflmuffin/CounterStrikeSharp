using CounterStrikeSharp.API.Core;
using MySharedTypes.Contracts;

namespace WithSharedTypes;

public class BalanceHandler : IBalanceHandler
{
    private readonly CCSPlayerController _player;

    private static readonly Dictionary<CCSPlayerController, decimal> Balances = new();

    public BalanceHandler(CCSPlayerController player)
    {
        _player = player;
    }

    public decimal Balance
    {
        get => Balances.TryGetValue(_player, out var balance) ? balance : 0;
        set => Balances[_player] = value;
    }
}