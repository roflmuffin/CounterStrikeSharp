using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.CsScript;

/// <summary>
/// The shared, CS#-owned point_script context for the current map. Access on
/// the game thread. CS# creates and releases the context with the map.
/// </summary>
public sealed class CsScriptRuntime
{
    private static CsScriptRuntime? shared;
    private readonly int threadId = Environment.CurrentManagedThreadId;
    private readonly CCSPointScriptEntity host;
    private readonly ulong session;

    public CsScriptGameRules GameRules { get; }
    public CsScriptConsole Console { get; }
    public CsScriptInstance Instance { get; }
    public CCSPointScriptEntity Host { get { CheckThread(); return host; } }

    private CsScriptRuntime(ulong session)
    {
        this.session = session;
        using var call = new CsScriptCall(CsScriptOperation.Host, session);
        call.Invoke();
        host = new CHandle<CCSPointScriptEntity>(call.Result<uint>()).Value
            ?? throw new InvalidOperationException("The shared cs_script host is unavailable");
        GameRules = new(this);
        Console = new(this);
        Instance = new(this);
    }

    /// <summary>
    /// Gets the context initialized by CS# for this map. All plugins share the
    /// same instance. Throws before initialization, between maps, or off-thread.
    /// Reacquire this property after a map change; old facades stay invalid.
    /// </summary>
    public static CsScriptRuntime Shared
    {
        get
        {
            // Native validation happens before touching the shared cache, so a
            // worker cannot create a facade or race the game-thread owner.
            using var call = new CsScriptCall(CsScriptOperation.Current);
            call.Invoke();
            var id = call.Result<ulong>();
            if (shared?.session != id) shared = new CsScriptRuntime(id);
            return shared;
        }
    }

    /// <summary>Checks that this session's host and built-in module are still ready.</summary>
    public bool TryInitialize() => Command(CsScriptOperation.Ready) != 0;

    internal CsScriptCall BeginInvoke(uint method, uint entity)
    {
        CheckThread();
        return new CsScriptCall(CsScriptOperation.Invoke, session, method, entity);
    }

    internal ulong Command(CsScriptOperation command)
    {
        CheckThread();
        using var call = new CsScriptCall(command, session);
        call.Invoke();
        return call.Result<ulong>();
    }

    internal void CheckThread()
    {
        if (Environment.CurrentManagedThreadId != threadId)
            throw new InvalidOperationException("cs_script requires its creating game thread");
    }

}

// Each call owns its native slots and UTF-8 allocations, including during nested
// calls. Generated wrappers write scalars directly, without boxing or JSON.
internal sealed class CsScriptCall : IDisposable
{
    private readonly ScriptContext context = new();

    internal CsScriptCall(CsScriptOperation operation, ulong session = 0, uint method = 0, uint entity = uint.MaxValue)
    {
        context.Reset();
        context.SetIdentifier(CsScriptSchema.Native);
        context.PushPrimitive(CsScriptSchema.Word0);
        context.PushPrimitive(CsScriptSchema.Word1);
        context.PushPrimitive(CsScriptSchema.Word2);
        context.PushPrimitive(CsScriptSchema.Word3);
        context.PushPrimitive((uint)operation);
        context.PushPrimitive(session);
        context.PushPrimitive(method);
        context.PushPrimitive(entity);
    }

    internal void Push(double value) => context.PushPrimitive(value);
    internal void Push(bool value) => context.PushPrimitive(value);
    internal void Push(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        // ScriptContext's existing string ABI is NUL-terminated UTF-8.
        if (value.Contains('\0')) throw new ArgumentException("cs_script strings cannot contain NUL", nameof(value));
        if (Encoding.UTF8.GetByteCount(value) > 1024 * 1024)
            throw new ArgumentException("cs_script string exceeds 1 MiB", nameof(value));
        context.PushString(value);
    }

    internal void Invoke()
    {
        context.Invoke();
        context.CheckErrors();
    }

    internal T Result<T>() where T : unmanaged => context.GetResultPrimitive<T>();
    internal string StringResult() => context.GetResultString();
    public void Dispose() => context.GlobalCleanUp();
}
