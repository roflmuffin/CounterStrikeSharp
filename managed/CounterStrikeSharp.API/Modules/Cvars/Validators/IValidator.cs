namespace CounterStrikeSharp.API.Modules.Cvars.Validators;

public interface IValidator<in T>
{
    bool Validate(T value, out string? errorMessage);
}