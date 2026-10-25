using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.CsScript;

/// <summary>
/// Shared point_script API. Access is restricted to the game thread.
/// </summary>
public sealed class CsScriptRuntime
{
    private static CsScriptRuntime? shared;
    private readonly int threadId = Environment.CurrentManagedThreadId;

    public CsScriptGameRules GameRules { get; }
    public CsScriptConsole Console { get; }
    public CsScriptInstance Instance { get; }
    /// <summary>Gets the current shared host, including after a map change or replacement.</summary>
    public CCSPointScriptEntity Host
    {
        get
        {
            CheckThread();
            return new CHandle<CCSPointScriptEntity>(NativeAPI.CsScriptGetHost()).Value
                ?? throw new InvalidOperationException("The shared cs_script host is unavailable");
        }
    }

    private CsScriptRuntime()
    {
        GameRules = new(this);
        Console = new(this);
        Instance = new(this);
    }

    /// <summary>
    /// Gets the shared runtime. Throws when the host is unavailable or called off-thread.
    /// Cached instances use the current host across map changes.
    /// </summary>
    public static CsScriptRuntime Shared
    {
        get
        {

            NativeAPI.CsScriptGetHost();
            return shared ??= new CsScriptRuntime();
        }
    }

    /// <summary>Checks that the shared host and built-in module are still ready.</summary>
    public bool TryInitialize()
    {
        CheckThread();
        return NativeAPI.CsScriptIsReady();
    }

    internal CsScriptCall BeginInvoke(uint method, uint entity)
    {
        CheckThread();
        return new CsScriptCall(method, entity);
    }

    internal void CheckThread()
    {
        if (Environment.CurrentManagedThreadId != threadId)
            throw new InvalidOperationException("cs_script requires its creating game thread");
    }

}

internal sealed class CsScriptCall : IDisposable
{
    private readonly ScriptContext context = new();

    internal CsScriptCall(uint method, uint entity)
    {
        context.Reset();
        context.PushPrimitive(method);
        context.PushPrimitive(entity);
    }

    internal void Push(double value) => context.PushPrimitive(value);
    internal void Push(bool value) => context.PushPrimitive(value);
    internal void PushEntity(CEntityInstance? value, bool allowAbsent)
    {
        if (value is null)
        {
            if (!allowAbsent) throw new ArgumentNullException(nameof(value));
            context.PushPrimitive(uint.MaxValue);
            return;
        }
        var raw = value.EntityHandle.Raw;

        if (raw == uint.MaxValue) throw new ArgumentException("Entity handle is invalid", nameof(value));
        context.PushPrimitive(raw);
    }

    internal void Push(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Contains('\0')) throw new ArgumentException("cs_script strings cannot contain NUL", nameof(value));
        if (Encoding.UTF8.GetByteCount(value) > 1024 * 1024)
            throw new ArgumentException("cs_script string exceeds 1 MiB", nameof(value));
        context.PushString(value);
    }

    internal void Invoke() => NativeAPI.CsScriptInvoke(context);

    internal T Result<T>() where T : unmanaged => context.GetResultPrimitive<T>();
    internal T EntityResult<T>() where T : CEntityInstance => OptionalEntityResult<T>()
        ?? throw new InvalidOperationException("cs_script returned an absent required entity");

    internal T? OptionalEntityResult<T>() where T : CEntityInstance
    {
        var raw = Result<uint>();
        if (raw == uint.MaxValue) return null;
        return new CHandle<T>(raw).Value
            ?? throw new InvalidOperationException("cs_script returned a deleted entity");
    }

    internal string StringResult() => context.GetResultString();
    public void Dispose() => context.GlobalCleanUp();
}
