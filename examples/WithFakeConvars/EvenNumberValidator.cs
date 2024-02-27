using CounterStrikeSharp.API.Modules.Cvars.Validators;

namespace WithFakeConvars;

// This is an example of a custom validator that checks if a number is even.
public class EvenNumberValidator : IValidator<int>
{
    public bool Validate(int value, out string? errorMessage)
    {
        if (value % 2 == 0)
        {
            errorMessage = null;
            return true;
        }
        else
        {
            errorMessage = "Value must be an even number";
            return false;
        }
    }
}