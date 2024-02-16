namespace CounterStrikeSharp.API.Modules.Cvars.Validators;

public class RangeValidator<T> : IValidator<T> where T : IComparable<T>
{
    private readonly T _min;
    private readonly T _max;

    public RangeValidator(T min, T max)
    {
        _min = min;
        _max = max;
    }

    public bool Validate(T value, out string? errorMessage)
    {
        if (value.CompareTo(_min) >= 0 && value.CompareTo(_max) <= 0)
        {
            errorMessage = null;
            return true;
        }
        else
        {
            errorMessage = $"Value must be between {_min} and {_max}";
            return false;
        }
    }
}