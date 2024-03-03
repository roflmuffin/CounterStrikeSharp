using CounterStrikeSharp.API.Core;
using MySharedTypes.Contracts;

namespace WithSharedTypes;

public class BalanceHandler : IBalanceHandler
{
    private readonly CCSPlayerController _player;

    // This could be a database, a file, or a dictionary like this.
    internal static readonly Dictionary<CCSPlayerController, decimal> Balances = new();

    public BalanceHandler(CCSPlayerController player)
    {
        _player = player;
    }

    public decimal Balance
    {
        get => Balances.TryGetValue(_player, out var balance) ? balance : 0;
        set => Balances[_player] = value;
    }

    public decimal Add(decimal amount)
    {
        return Balance += amount;
    }

    public decimal Subtract(decimal amount)
    {
        return Balance -= amount;
    }
}