using System.Collections.Generic;
using System.ComponentModel;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars.Validators;

namespace CounterStrikeSharp.API.Modules.Cvars;

public class FakeConVar<T> where T : IComparable<T>
{
    private readonly IEnumerable<IValidator<T>>? _customValidators;

    public FakeConVar(string name, string description, T defaultValue = default(T), ConVarFlags flags = ConVarFlags.FCVAR_NONE,
        params IValidator<T>[] customValidators)
    {
        _customValidators = customValidators;
        Name = name;
        Description = description;
        Value = defaultValue;
        Flags = flags;
    }

    public ConVarFlags Flags { get; set; }

    public string Name { get; }
    public string Description { get; }

    public event EventHandler<T> ValueChanged;

    private T _value;

    public T Value
    {
        get => _value;
        set => SetValue(value);
    }

    internal void ExecuteCommand(CCSPlayerController? player, CommandInfo args)
    {
        if (player != null && !Flags.HasFlag(ConVarFlags.FCVAR_REPLICATED))
        {
            return;
        }

        if (args.ArgCount < 2)
        {
            if (Flags.HasFlag(ConVarFlags.FCVAR_PROTECTED) && player != null)
            {
                args.ReplyToCommand($"{args.GetArg(0)} = <protected>");
            }
            else
            {
                args.ReplyToCommand($"{args.GetArg(0)} = {Value.ToString()}");
            }

            return;
        }
        
        if (Flags.HasFlag(ConVarFlags.FCVAR_CHEAT))
        {
            var cheats = ConVar.Find("sv_cheats")!.GetPrimitiveValue<bool>();
            if (!cheats)
            {
                args.ReplyToCommand($"SV: Convar '{Name}' is cheat protected, change ignored");
                return; 
            }
        }
        
        if (player != null)
        {
            return;
        }

        try
        {
            // TODO(dotnet8): Replace with IParsable<T>
            bool success = true;
            T parsedValue = default(T);
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    parsedValue = (T)converter.ConvertFromString(args.ArgString);
                }
                catch
                {
                    success = typeof(T) == typeof(bool) && TryConvertCustomBoolean(args.ArgString, out parsedValue);
                }
            }

            if (!success)
            {
                args.ReplyToCommand($"Error: String '{args.GetArg(1)}' can't be converted to {typeof(T).Name}");
                args.ReplyToCommand($"Failed to parse input ConVar '{Name}' from string '{args.GetArg(1)}'");
                return;
            }

            SetValue(parsedValue);
        }
        catch (Exception ex)
        {
            args.ReplyToCommand($"Error: {ex.Message}");
        }
    }

    private bool TryConvertCustomBoolean(string input, out T result)
    {
        input = input.Trim().ToLowerInvariant();
        if (input == "1" || input == "true")
        {
            result = (T)(object)true;
            return true;
        }
        else if (input == "0" || input == "false")
        {
            result = (T)(object)false;
            return true;
        }

        result = default(T);
        return false;
    }

    private void SetValue(T value)
    {
        if (_customValidators != null)
        {
            foreach (var validator in _customValidators)
            {
                if (!validator.Validate(value, out var error))
                {
                    throw new ArgumentException($"{error ?? "Invalid value provided"}");
                }
            }
        }

        _value = value;
        ValueChanged?.Invoke(this, _value);
    }
}