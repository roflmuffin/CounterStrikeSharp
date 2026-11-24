namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

public abstract class BaseMemoryFunction : NativeObject
{
    internal static Dictionary<string, IntPtr> _createdOffsetFunctions = new();

    internal static string FunctionCacheKey(string function, DataType returnType, IEnumerable<DataType> argumentTypes)
    {
        return $"{function}:{returnType}:{string.Join(",", argumentTypes)}";
    }

    private static IntPtr CreateValveFunctionByOffset(string symbolName, int offset, DataType returnType,
        DataType[] argumentTypes, Func<nint> nativeCaller)
    {
        string constructKey = FunctionCacheKey($"{symbolName}_{offset}", returnType, argumentTypes);

        if (!_createdOffsetFunctions.TryGetValue(constructKey, out var function))
        {
            try
            {
                function = nativeCaller();
                _createdOffsetFunctions[constructKey] = function;
            }
            catch (Exception)
            {
            }
        }

        return function;
    }

    private static IntPtr CreateValveFunctionByOffset(IntPtr objectPtr, string symbolName, int offset, DataType returnType,
        DataType[] argumentTypes)
    {
        return CreateValveFunctionByOffset(symbolName, offset, returnType, argumentTypes, () =>
        {
            return NativeAPI.CreateVirtualFunction(objectPtr, offset, argumentTypes.Length,
                (int)returnType, argumentTypes.Cast<object>().ToArray());
        });
    }

    private static IntPtr CreateValveFunctionBySymbol(string symbolName, string binaryPath, int offset, DataType returnType,
        DataType[] argumentTypes)
    {
        return CreateValveFunctionByOffset(symbolName, offset, returnType, argumentTypes, () =>
        {
            return NativeAPI.CreateVirtualFunctionBySymbol(binaryPath, symbolName, offset, argumentTypes.Length,
                (int)returnType, argumentTypes.Cast<object>().ToArray());
        });
    }

    private static IntPtr CreateValveFunctionFromVTable(string symbolName, IntPtr vtable, int offset, DataType returnType,
        DataType[] argumentTypes)
    {
        return CreateValveFunctionByOffset(symbolName, offset, returnType, argumentTypes, () =>
        {
            return NativeAPI.CreateVirtualFunctionFromVTable(vtable, offset, argumentTypes.Length,
                (int)returnType, argumentTypes.Cast<object>().ToArray());
        });
    }

    // Set for functions created from a signature, so an unbound one can say why when called.
    private readonly string? _signature;
    private readonly string? _binaryPath;

    public BaseMemoryFunction(string signature, DataType returnType, DataType[] parameters) : this(signature, Addresses.ServerPath,
        false, returnType, parameters)
    {
    }

    public BaseMemoryFunction(string signature, string binarypath, DataType returnType, DataType[] parameters) : this(signature,
        binarypath, false, returnType, parameters)
    {
    }

    /// <summary>
    /// Creates a function from a raw signature. The signature binds to its first match. If it matches more than once, a
    /// warning with the match count is logged once; with <paramref name="strict"/> set, an ambiguous signature is
    /// rejected instead. A signature that cannot be bound is logged once, and calling or hooking the function throws a
    /// <see cref="NativeException"/> naming the signature and the reason.
    /// </summary>
    /// <param name="signature">Signature bytes, space-separated hex ("55 48 ? E5") or code style ("\x55\x48\x2A\xE5").</param>
    /// <param name="binarypath">Path of the binary to scan, e.g. <see cref="Addresses.ServerPath"/>.</param>
    /// <param name="strict">If true, a signature that matches more than once fails as ambiguous instead of binding to the first match.</param>
    /// <param name="returnType">Return type.</param>
    /// <param name="parameters">Parameter types.</param>
    public BaseMemoryFunction(string signature, string binarypath, bool strict, DataType returnType, DataType[] parameters) : base(
        SignatureFunctions.Create(signature, binarypath, returnType, parameters, strict))
    {
        _signature = signature;
        _binaryPath = binarypath;
    }

    /// <summary>
    /// <b>WARNING:</b> this is only supposed to be used with <see cref="VirtualFunctionVoid{TArg1}"/> and <see cref="VirtualFunctionWithReturn{TArg1, TResult}"/> variants.
    /// </summary>
    internal BaseMemoryFunction(IntPtr objectPtr, string symbolName, int offset, DataType returnType, DataType[] parameters) : base(
        CreateValveFunctionByOffset(objectPtr, symbolName, offset, returnType, parameters))
    {
    }

    /// <summary>
    /// <b>WARNING:</b> this is only supposed to be used with <see cref="VirtualFunctionVoid{TArg1}"/> and <see cref="VirtualFunctionWithReturn{TArg1, TResult}"/> variants.
    /// </summary>
    internal BaseMemoryFunction(string symbolName, string binaryPath, int offset, DataType returnType, DataType[] parameters) : base(
        CreateValveFunctionBySymbol(symbolName, binaryPath, offset, returnType, parameters))
    {
    }

    /// <summary>
    /// <b>WARNING:</b> this is only supposed to be used with <see cref="VirtualFunctionVoid{TArg1}"/> and <see cref="VirtualFunctionWithReturn{TArg1, TResult}"/> variants.
    /// </summary>
    internal BaseMemoryFunction(string symbolName, int offset, DataType returnType, DataType[] parameters) : base(
        CreateValveFunctionBySymbol(symbolName, Addresses.ServerPath, offset, returnType, parameters))
    {
    }

    /// <summary>
    /// <b>WARNING:</b> this is only supposed to be used with <see cref="VirtualFunctionVoid{TArg1}"/> and <see cref="VirtualFunctionWithReturn{TArg1, TResult}"/> variants.
    /// </summary>
    internal BaseMemoryFunction(string symbolName, IntPtr vtable, int offset, DataType returnType, DataType[] parameters) : base(
        CreateValveFunctionFromVTable(symbolName, vtable, offset, returnType, parameters))
    {
    }

    // Unbound signature functions throw a clear error here instead of the native "Invalid function pointer".
    private void EnsureBound()
    {
        if (_signature == null || RawHandle != IntPtr.Zero) return;

        var reason = SignatureFunctions.FailureReason(_binaryPath!, _signature) ?? "not found";
        throw new NativeException(
            $"Memory function with signature \"{_signature}\" in {Path.GetFileName(_binaryPath)} is not available: {reason}");
    }

    public void Hook(Func<DynamicHook, HookResult> handler, HookMode mode)
    {
        EnsureBound();
        NativeAPI.HookFunction(Handle, handler, mode == HookMode.Post);
    }

    public void Unhook(Func<DynamicHook, HookResult> handler, HookMode mode)
    {
        EnsureBound();
        NativeAPI.UnhookFunction(Handle, handler, mode == HookMode.Post);
    }

    protected T InvokeInternal<T>(bool bypass, params object[] args)
    {
        EnsureBound();
        return NativeAPI.ExecuteVirtualFunction<T>(Handle, bypass, args);
    }

    protected void InvokeInternalVoid(bool bypass, params object[] args)
    {
        EnsureBound();
        NativeAPI.ExecuteVirtualFunction<object>(Handle, bypass, args);
    }
}