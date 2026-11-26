using System.Drawing;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CounterStrikeSharp.API.Tests;

// Schema accessors on a field the running build does not have, against a faked native lookup and a managed buffer
// standing in for an entity. In SchemaMissMode.Warn: one warning per field, reads return the default value, writes are
// dropped, offset 0 is never touched. In Throw: the typed exception on every access. Uses static state.
[Collection(nameof(SchemaMissModeTests))]
public sealed class SchemaMissModeTests : IDisposable
{
    private const int FieldMissing = -2;
    private const int ClassMissing = -1;
    private const int HealthOffset = 16;
    private const byte Sentinel = 0xAB;

    private readonly Func<string, string, int> _originalLookup = Schema.LookupField;
    private readonly CapturingLogger _logger = new();
    private readonly Dictionary<(string, string), int> _lookups = new();
    private readonly IntPtr _entity = Marshal.AllocHGlobal(64);

    public SchemaMissModeTests()
    {
        Schema.ClearCache();
        Schema.Logger = _logger;
        Schema.LookupField = (className, fieldName) =>
            _lookups.TryGetValue((className, fieldName), out var offset) ? offset : FieldMissing;
        for (var i = 0; i < 64; i++) Marshal.WriteByte(_entity, i, Sentinel);
    }

    public void Dispose()
    {
        Schema.LookupField = _originalLookup;
        Schema.Logger = null;
        Schema.ClearCache();
        CoreConfig.ApplyModes(new CoreConfigData(), NullLogger.Instance);
        Marshal.FreeHGlobal(_entity);
    }

    private static void UseMode(SchemaMissMode mode) => CoreConfig.SchemaMissMode = mode;

    private void AssertEntityUntouched()
    {
        for (var i = 0; i < 64; i++) Assert.Equal(Sentinel, Marshal.ReadByte(_entity, i));
    }

    [Fact]
    public void WarnIsTheDefault()
    {
        CoreConfig.ApplyModes(new CoreConfigData(), NullLogger.Instance);

        Assert.Equal(SchemaMissMode.Warn, CoreConfig.SchemaMissMode);
    }

    [Fact]
    public void WarnReadsReturnTheDefaultValueWithoutTouchingTheEntity()
    {
        UseMode(SchemaMissMode.Warn);

        Assert.Equal(0, Schema.GetRef<int>(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Equal(0, Schema.GetSchemaValue<int>(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Equal(new int[4], Schema.GetFixedArray<int>(_entity, "CBaseEntity", "m_cssharpMissing", 4).ToArray());
        Assert.Equal("", Schema.GetString(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Equal("", Schema.GetUtf8String(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Null(Schema.GetDeclaredClass<CEntityIdentity>(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Null(Schema.GetPointer<CEntityIdentity>(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Equal(default, Schema.GetCustomMarshalledType<Color>(_entity, "CBaseEntity", "m_cssharpMissing"));
        AssertEntityUntouched();
    }

    [Fact]
    public void WarnWritesAreDroppedAndOffsetZeroIsNeverWritten()
    {
        UseMode(SchemaMissMode.Warn);

        Schema.GetRef<int>(_entity, "CBaseEntity", "m_cssharpMissing") = 42;
        Schema.SetSchemaValue(_entity, "CBaseEntity", "m_cssharpMissing", 42);
        Schema.SetString(_entity, "CBaseEntity", "m_cssharpMissing", "x");
        Schema.SetStringBytes(_entity, "CBaseEntity", "m_cssharpMissing", "x", 16);
        Schema.SetCustomMarshalledType(_entity, "CBaseEntity", "m_cssharpMissing", Color.Red);
        Schema.GetFixedArray<int>(_entity, "CBaseEntity", "m_cssharpMissing", 4)[0] = 42;

        AssertEntityUntouched();
        Assert.Equal(0, Schema.GetRef<int>(_entity, "CBaseEntity", "m_cssharpMissing"));
    }

    [Fact]
    public void WarnLogsOneWarningPerFieldSayingItWillThrowLater()
    {
        UseMode(SchemaMissMode.Warn);
        _lookups[("CCssharpMissing", "m_iHealth")] = ClassMissing;

        for (var i = 0; i < 3; i++)
        {
            _ = Schema.GetRef<int>(_entity, "CBaseEntity", "m_cssharpMissing");
            Schema.SetSchemaValue(_entity, "CBaseEntity", "m_cssharpMissing", i);
            _ = Schema.GetRef<int>(_entity, "CCssharpMissing", "m_iHealth");
        }

        Assert.Equal(2, _logger.Entries.Count);
        Assert.All(_logger.Entries, entry => Assert.Equal(LogLevel.Warning, entry.Level));
        Assert.Equal(
            "Schema field CBaseEntity::m_cssharpMissing not found (field missing); reads return the default value and writes are " +
            "dropped. A later release will throw instead (SchemaMissMode: throw)", _logger.Entries[0].Message);
        Assert.Contains("CCssharpMissing::m_iHealth not found (class missing)", _logger.Entries[1].Message);
    }

    [Fact]
    public void GetSchemaOffsetThrowsInEveryModeBecauseTheCallerWouldAddTheOffsetItself()
    {
        UseMode(SchemaMissMode.Warn);

        var exception = Assert.Throws<SchemaFieldNotFoundException>(() => Schema.GetSchemaOffset("CBaseEntity", "m_cssharpMissing"));
        Assert.False(exception.ClassMissing);
    }

    [Fact]
    public void ThrowThrowsTheTypedExceptionOnEveryAccessAndLogsOnce()
    {
        UseMode(SchemaMissMode.Throw);

        Assert.Throws<SchemaFieldNotFoundException>(() => Schema.GetRef<int>(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Throws<SchemaFieldNotFoundException>(() => Schema.GetSchemaValue<int>(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Throws<SchemaFieldNotFoundException>(() => Schema.SetSchemaValue(_entity, "CBaseEntity", "m_cssharpMissing", 1));
        Assert.Throws<SchemaFieldNotFoundException>(() => Schema.GetUtf8String(_entity, "CBaseEntity", "m_cssharpMissing"));
        Assert.Throws<SchemaFieldNotFoundException>(() => Schema.SetStringBytes(_entity, "CBaseEntity", "m_cssharpMissing", "x", 16));

        var error = Assert.Single(_logger.Entries);
        Assert.Equal(LogLevel.Error, error.Level);
        Assert.Equal("Schema field CBaseEntity::m_cssharpMissing not found (field missing); accessing it throws", error.Message);
        AssertEntityUntouched();
    }

    [Fact]
    public void ExistingFieldsReadAndWriteAtTheirOffsetInEitherMode()
    {
        _lookups[("CBaseEntity", "m_iHealth")] = HealthOffset;

        foreach (var mode in new[] { SchemaMissMode.Warn, SchemaMissMode.Throw })
        {
            UseMode(mode);
            Schema.GetRef<int>(_entity, "CBaseEntity", "m_iHealth") = 100 + (int)mode;

            Assert.Equal(100 + (int)mode, Marshal.ReadInt32(_entity, HealthOffset));
            Assert.Equal(HealthOffset, Schema.GetSchemaOffset("CBaseEntity", "m_iHealth"));
        }

        Assert.Empty(_logger.Entries);
    }

    [Fact]
    public void ProbingAFieldOnlySomeClassesHaveDoesNotLog()
    {
        Assert.False(Schema.TryGetSchemaOffset("CCSPlayerController", "__m_pChainEntity", out var offset));
        Assert.Equal(0, offset);
        Assert.Empty(_logger.Entries);
    }

    [Fact]
    public void AFieldFirstProbedStillWarnsWhenAnAccessorReadsIt()
    {
        UseMode(SchemaMissMode.Warn);
        Assert.False(Schema.TryGetSchemaOffset("CBaseEntity", "m_cssharpMissing", out _));

        _ = Schema.GetRef<int>(_entity, "CBaseEntity", "m_cssharpMissing");

        Assert.Equal(LogLevel.Warning, Assert.Single(_logger.Entries).Level);
    }

    private sealed class CapturingLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }
    }
}
