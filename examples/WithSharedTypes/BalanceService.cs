using MySharedTypes.Contracts;

namespace WithSharedTypes;

public class BalanceService : IBalanceService
{
    public void ClearAllBalances()
    {
        BalanceHandler.Balances.Clear();
    }
}