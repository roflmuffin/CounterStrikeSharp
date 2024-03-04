namespace MySharedTypes.Contracts;

public interface IBalanceHandler
{
    decimal Balance { get; }

    // These are just here to show that you can have methods on your shared types.
    // You could also add a Setter to the Balance property.
    public decimal Add(decimal amount);
    public decimal Subtract(decimal amount);
}