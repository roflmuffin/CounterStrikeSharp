using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;

namespace NativeTestsPlugin;

// These tests create only uniquely named test ConVars, never modifying server-owned settings.
public class RealConVarTests : IDisposable
{
    private readonly List<ConVarBase> _created = new();

    private static string NewName() => $"css_itest_cvar_{Guid.NewGuid():N}";

    private ConVar<T> Create<T>(T value, T min, T max)
    {
        var conVar = new ConVar<T>(new ConVarCreationOptions<T>
        {
            Name = NewName(),
            Description = "Native typed ConVar test",
            DefaultValue = value,
            MinValue = min,
            MaxValue = max
        });
        _created.Add(conVar);
        return conVar;
    }

    public void Dispose()
    {
        // xUnit invokes this even when an assertion fails. Deleted/plugin-owned ConVars have index zero.
        foreach (var conVar in _created)
        {
            if (conVar.AccessIndex != ushort.MaxValue)
                conVar.Delete();
        }
    }

    [Theory]
    [InlineData(ConVarType.Bool)]
    [InlineData(ConVarType.Int16)]
    [InlineData(ConVarType.UInt16)]
    [InlineData(ConVarType.Int32)]
    [InlineData(ConVarType.UInt32)]
    [InlineData(ConVarType.Int64)]
    [InlineData(ConVarType.UInt64)]
    [InlineData(ConVarType.Float32)]
    [InlineData(ConVarType.Float64)]
    public void PrimitiveValuesRoundTrip(ConVarType type)
    {
        switch (type)
        {
            case ConVarType.Bool:
                RoundTrip(type, false, true, false, true);
                break;
            case ConVarType.Int16:
                RoundTrip<short>(type, -123, 234, short.MinValue, short.MaxValue);
                break;
            case ConVarType.UInt16:
                RoundTrip<ushort>(type, 40000, 60000, 0, ushort.MaxValue);
                break;
            case ConVarType.Int32:
                RoundTrip(type, -123456, 654321, int.MinValue, int.MaxValue);
                break;
            case ConVarType.UInt32:
                RoundTrip(type, 3000000000u, 4000000000u, 0u, uint.MaxValue);
                break;
            case ConVarType.Int64:
                RoundTrip(type, -5000000000L, 6000000000L, long.MinValue, long.MaxValue);
                break;
            case ConVarType.UInt64:
                RoundTrip(type, 10000000000000000000UL, 11000000000000000000UL, 0UL, ulong.MaxValue);
                break;
            case ConVarType.Float32:
                RoundTrip(type, -1.25f, 9.5f, -100f, 100f);
                break;
            case ConVarType.Float64:
                // This value cannot round-trip through a 32-bit float without losing precision.
                RoundTrip(type, -1.25d, 1.0000000000000002d, -100d, 100d);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }

    private void RoundTrip<T>(ConVarType type, T initial, T updated, T min, T max)
    {
        var conVar = Create(initial, min, max);
        Assert.NotEqual(ushort.MaxValue, conVar.AccessIndex);
        Assert.Equal(type, conVar.Type);
        Assert.Equal("Native typed ConVar test", conVar.Description);
        Assert.Equal(initial, conVar.Value);

        var found = ConVar<T>.Find(conVar.Name);
        Assert.NotNull(found);
        Assert.Equal(conVar.AccessIndex, found.AccessIndex);
        Assert.Equal(conVar.Name, found.Name);
        Assert.Equal(initial, found.Value);

        conVar.Value = updated;
        Assert.Equal(updated, found.Value);
        found.Value = initial;
        Assert.Equal(initial, conVar.Value);
    }

    [Theory]
    [InlineData(ConVarType.Vector2)]
    [InlineData(ConVarType.Vector3)]
    [InlineData(ConVarType.Vector4)]
    [InlineData(ConVarType.Qangle)]
    public void NativeObjectValuesRoundTrip(ConVarType type)
    {
        // Supply explicit vector bounds: native creation currently dereferences all three arguments.
        switch (type)
        {
            case ConVarType.Vector2:
                NativeRoundTrip(type, new Vector2D(1, 2), new Vector2D(-3, 4),
                    new Vector2D(-100, -100), new Vector2D(100, 100), v => new[] { v.X, v.Y });
                break;
            case ConVarType.Vector3:
                NativeRoundTrip(type, new Vector(1, 2, 3), new Vector(-3, 4, 5),
                    new Vector(-100, -100, -100), new Vector(100, 100, 100), v => new[] { v.X, v.Y, v.Z });
                break;
            case ConVarType.Vector4:
                NativeRoundTrip(type, new Vector4D(1, 2, 3, 4), new Vector4D(-3, 4, 5, 6),
                    new Vector4D(-100, -100, -100, -100), new Vector4D(100, 100, 100, 100),
                    v => new[] { v.X, v.Y, v.Z, v.W });
                break;
            case ConVarType.Qangle:
                NativeRoundTrip(type, new QAngle(1, 2, 3), new QAngle(-3, 4, 5),
                    new QAngle(-100, -100, -100), new QAngle(100, 100, 100), v => new[] { v.X, v.Y, v.Z });
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }

    private void NativeRoundTrip<T>(ConVarType type, T initial, T updated, T min, T max,
        Func<T, float[]> components) where T : NativeObject
    {
        var conVar = Create(initial, min, max);
        Assert.Equal(type, conVar.Type);
        Assert.Equal(components(initial), components(conVar.Value));
        conVar.Value = updated;
        var found = ConVar<T>.Find(conVar.Name);
        Assert.NotNull(found);
        Assert.Equal(components(updated), components(found.Value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("spaces and \"quotes\"")]
    [InlineData("Grüße 世界 🎮")]
    public void StringValuesRoundTrip(string value)
    {
        var conVar = new ConVar<string>(NewName(), "String test", "initial");
        _created.Add(conVar);
        Assert.Equal(ConVarType.String, conVar.Type);
        Assert.Equal("initial", conVar.Value);
        conVar.Value = value;
        Assert.Equal(value, conVar.Value);
        Assert.Equal(value, conVar.ValueAsString);
        conVar.ValueAsString = "updated 世界";
        Assert.Equal("updated 世界", conVar.Value);
    }

    [Theory]
    [InlineData(-11, -10)]
    [InlineData(-10, -10)]
    [InlineData(4, 4)]
    [InlineData(10, 10)]
    [InlineData(11, 10)]
    public void IntegerBoundsApplyToTypedAndStringSetters(int value, int expected)
    {
        var conVar = Create(0, -10, 10);
        conVar.Value = value;
        Assert.Equal(expected, conVar.Value);
        conVar.Value = 0;
        conVar.ValueAsString = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(expected, conVar.Value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TryGetBoundsDistinguishesMissingBoundsFromZero(bool hasBounds)
    {
        var options = new ConVarCreationOptions<int> { Name = NewName(), DefaultValue = 0 };
        if (hasBounds)
            options = options with { MinValue = 0, MaxValue = 0 };
        var conVar = new ConVar<int>(options);
        _created.Add(conVar);

        Assert.Equal(hasBounds, conVar.TryGetMinValue(out var minimum));
        Assert.Equal(hasBounds, conVar.TryGetMaxValue(out var maximum));
        Assert.Equal(0, minimum);
        Assert.Equal(0, maximum);
    }

    [Fact]
    public void BoundsCanBeUpdatedAfterCreation()
    {
        var conVar = Create(0, -100, 100);

        conVar.MinValue = -10;
        conVar.MaxValue = 10;
        Assert.Equal(-10, conVar.MinValue);
        Assert.Equal(10, conVar.MaxValue);
        conVar.Value = -50;
        Assert.Equal(-10, conVar.Value);
        conVar.Value = 50;
        Assert.Equal(10, conVar.Value);

        conVar.MinValue = -200;
        conVar.MaxValue = 200;
        Assert.True(conVar.TryGetMinValue(out var minimum));
        Assert.True(conVar.TryGetMaxValue(out var maximum));
        Assert.Equal(-200, minimum);
        Assert.Equal(200, maximum);
        conVar.Value = -150;
        Assert.Equal(-150, conVar.Value);
        conVar.Value = 150;
        Assert.Equal(150, conVar.Value);
    }

    [Fact]
    public void InvalidBoundUpdatesLeaveExistingBoundsUnchanged()
    {
        var conVar = Create(0, -10, 10);
        Assert.Throws<NativeException>(() => conVar.MinValue = 11);
        Assert.Throws<NativeException>(() => conVar.MaxValue = -11);
        Assert.Equal(-10, conVar.MinValue);
        Assert.Equal(10, conVar.MaxValue);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OmittedBoundsDoNotClampValuesToZero(bool useOptions)
    {
        var name = NewName();
        var conVar = useOptions
            ? new ConVar<int>(new ConVarCreationOptions<int> { Name = name, DefaultValue = 42 })
            : new ConVar<int>(name, "Unbounded integer", 42);
        _created.Add(conVar);
        Assert.Equal(42, conVar.Value);
        conVar.Value = -73;
        Assert.Equal(-73, conVar.Value);
        conVar.Value = 99;
        Assert.Equal(99, conVar.Value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OneSidedZeroBoundsLeaveOtherDirectionUnbounded(bool lowerBound)
    {
        var options = new ConVarCreationOptions<int> { Name = NewName(), DefaultValue = 0 };
        options = lowerBound ? options with { MinValue = 0 } : options with { MaxValue = 0 };
        var conVar = new ConVar<int>(options);
        _created.Add(conVar);
        conVar.Value = -73;
        Assert.Equal(lowerBound ? 0 : -73, conVar.Value);
        conVar.Value = 99;
        Assert.Equal(lowerBound ? 99 : 0, conVar.Value);
    }

    [Fact]
    public void ExplicitZeroBoundsClampValuesToZero()
    {
        var conVar = Create(42, 0, 0);
        Assert.Equal(0, conVar.Value);
        conVar.Value = -73;
        Assert.Equal(0, conVar.Value);
        conVar.Value = 99;
        Assert.Equal(0, conVar.Value);
    }

    [Fact]
    public void OmittedVectorBoundsAllowCreationAndUpdates()
    {
        var conVar = new ConVar<Vector>(NewName(), "Unbounded vector", new Vector(1, 2, 3));
        _created.Add(conVar);
        Assert.Equal(1, conVar.Value.X);
        Assert.Equal(2, conVar.Value.Y);
        Assert.Equal(3, conVar.Value.Z);
        conVar.Value = new Vector(-4, 5, -6);
        Assert.Equal(-4, conVar.Value.X);
        Assert.Equal(5, conVar.Value.Y);
        Assert.Equal(-6, conVar.Value.Z);
    }

    [Fact]
    public void FlagsAndPublicPreserveUnrelatedBits()
    {
        var conVar = Create(0, -10, 10);
        var flags = conVar.Flags & ~ConVarFlags.FCVAR_NOTIFY;
        conVar.Flags = flags;
        Assert.Equal(flags, conVar.Flags);
        Assert.False(conVar.Public);
        conVar.Public = true;
        Assert.Equal(flags | ConVarFlags.FCVAR_NOTIFY, conVar.Flags);
        Assert.True(conVar.Public);
        conVar.Public = false;
        Assert.Equal(flags, conVar.Flags);
        Assert.False(conVar.Public);
    }

    [Fact]
    public void MissingConVarReturnsNull()
    {
        Assert.Null(ConVar<int>.Find(NewName()));
    }

    [Fact]
    public void ReadingWithWrongTypeThrowsWithoutChangingValue()
    {
        var conVar = Create(42, 0, 100);
        var wrongType = ConVar<float>.Find(conVar.Name);
        Assert.NotNull(wrongType);
        Assert.Throws<InvalidOperationException>(() => wrongType.Value);
        Assert.Equal(42, conVar.Value);
    }

    [Fact]
    public void UnsupportedTypeIsRejectedBeforeRegistration()
    {
        var name = NewName();
        Assert.Throws<InvalidOperationException>(() => new ConVar<decimal>(name, "Unsupported", 1m));
        Assert.Null(ConVar<int>.Find(name));
    }

    [Fact]
    public void DuplicateNameIsRejectedWithoutChangingOriginal()
    {
        var conVar = Create(42, 0, 100);
        Assert.Throws<NativeException>(() => new ConVar<int>(conVar.Name, "Duplicate", 99));
        Assert.Equal(42, conVar.Value);
        Assert.Equal("Native typed ConVar test", conVar.Description);
    }

    [Fact]
    public void DeleteClearsIndexAndRemovesLookup()
    {
        var conVar = Create(42, 0, 100);
        var name = conVar.Name;
        conVar.Delete();
        Assert.Equal(ushort.MaxValue, conVar.AccessIndex);
        Assert.Null(ConVar<int>.Find(name));
        Assert.Throws<NativeException>(() => conVar.Name);
        Assert.Throws<InvalidOperationException>(() => conVar.Delete());
    }

    [Fact]
    public async Task CreatedConVarIsAccessibleFromServerConsole()
    {
        var conVar = Create(42, 0, 100);
        Server.ExecuteCommand($"{conVar.Name} 73");
        await WaitOneFrame();
        Assert.Equal(73, conVar.Value);
        Assert.Equal("73", conVar.ValueAsString);
    }

    [Fact]
    public void ConVarAreEquatable()
    {
        var conVar1 = ConVar<bool>.Find("sv_cheats");
        var conVar2 = ConVar<bool>.Find("sv_cheats");
        Assert.NotNull(conVar1);
        Assert.NotNull(conVar2);
        Assert.Equal(conVar1, conVar2);
    }

    [Fact]
    public void ConVarsAreNotEquatable()
    {
        var conVar1 = ConVar<bool>.Find("sv_cheats");
        var conVar2 = ConVar<bool>.Find("sv_showimpacts");
        Assert.NotNull(conVar1);
        Assert.NotNull(conVar2);
        Assert.NotEqual(conVar1, conVar2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PluginDisposalDeletesRegisteredInstanceConVars(bool registerAllAttributes)
    {
        var owned = Create(1, 0, 100);
        var unrelated = Create(2, 0, 100);
        var name = owned.Name;
        using var plugin = new ConVarTestPlugin();
        var holder = new InstanceConVars(owned);
        if (registerAllAttributes)
            plugin.RegisterAllAttributes(holder);
        else
            plugin.RegisterConVars(holder);

        Assert.Equal(1, owned.Value);
        plugin.Dispose();
        Assert.Equal(ushort.MaxValue, owned.AccessIndex);
        Assert.Null(ConVar<int>.Find(name));
        Assert.Equal(2, unrelated.Value);
        // Disposal must be idempotent; the using statement also exercises this.
        plugin.Dispose();
    }

    [Fact]
    public void PluginDisposalDeletesRegisteredStaticConVars()
    {
        var owned = Create(1, 0, 100);
        var name = owned.Name;
        using var plugin = new ConVarTestPlugin();
        StaticConVars.Value = owned;
        try
        {
            plugin.RegisterConVars(typeof(StaticConVars));
            plugin.Dispose();
            Assert.Equal(ushort.MaxValue, owned.AccessIndex);
            Assert.Null(ConVar<int>.Find(name));
        }
        finally
        {
            StaticConVars.Value = null!;
        }
    }

    private sealed class InstanceConVars(ConVar<int> value)
    {
        public readonly ConVar<int> Value = value;
        public readonly string Unrelated = "Ignored by ConVar registration";
    }

    private static class StaticConVars
    {
        public static ConVar<int> Value = null!;
    }

    private sealed class ConVarTestPlugin : BasePlugin
    {
        public override string ModuleName => "ConVar ownership test";
        public override string ModuleVersion => "1.0.0";
    }
}
