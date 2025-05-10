namespace CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

public abstract class BaseMemoryFunction : NativeObject
{
    private static Dictionary<string, IntPtr> _createdFunctions = new();

    private static Dictionary<string, IntPtr> _createdOffsetFunctions = new();

    private static IntPtr CreateValveFunctionBySignature(string signature, DataType returnType,
        DataType[] argumentTypes)
    {
        if (!_createdFunctions.TryGetValue(signature, out var function))
        {
            try
            {
                function = NativeAPI.CreateVirtualFunctionBySignature(IntPtr.Zero, Addresses.ServerPath, signature,
                    argumentTypes.Length, (int)returnType, argumentTypes.Cast<object>().ToArray());
                _createdFunctions[signature] = function;
            }
            catch (Exception)
            {
            }
        }

        return function;
    }

    private static IntPtr CreateValveFunctionBySignature(string signature, string binarypath, DataType returnType,
        DataType[] argumentTypes)
    {
        if (!_createdFunctions.TryGetValue(signature, out var function))
        {
            try
            {
                function = NativeAPI.CreateVirtualFunctionBySignature(IntPtr.Zero, binarypath, signature,
                    argumentTypes.Length, (int)returnType, argumentTypes.Cast<object>().ToArray());
                _createdFunctions[signature] = function;
            }
            catch (Exception)
            {
            }
        }

        return function;
    }

    private static IntPtr CreateValveFunctionByOffset(IntPtr objectPtr, Type type, int offset, DataType returnType,
        DataType[] argumentTypes)
    {
        string constructKey = $"{type.Name}_{offset}";

        if (!_createdOffsetFunctions.TryGetValue(constructKey, out var function))
        {
            try
            {
                function = NativeAPI.CreateVirtualFunction(objectPtr, offset,
                    argumentTypes.Length, (int)returnType, argumentTypes.Cast<object>().ToArray());
                _createdOffsetFunctions[constructKey] = function;
            } catch (Exception)
            {
            }
        }

        return function;
    }

    public BaseMemoryFunction(string signature, DataType returnType, DataType[] parameters) : base(
        CreateValveFunctionBySignature(signature, returnType, parameters))
    {
    }

    public BaseMemoryFunction(string signature, string binarypath, DataType returnType, DataType[] parameters) : base(
        CreateValveFunctionBySignature(signature, binarypath, returnType, parameters))
    {
    }

    /// <summary>
    /// <b>WARNING:</b> this is only supposed to be used with <see cref="VirtualFunctionVoid"/> and <see cref="VirtualFunctionWithReturn{TResult}"/>
    /// </summary>
    internal BaseMemoryFunction(IntPtr objectPtr, Type type, int offset, DataType returnType, DataType[] parameters) : base(
        CreateValveFunctionByOffset(objectPtr, type, offset, returnType, parameters))
    {
    }

    public void Hook(Func<DynamicHook, HookResult> handler, HookMode mode)
    {
        NativeAPI.HookFunction(Handle, handler, mode == HookMode.Post);
    }

    public void Unhook(Func<DynamicHook, HookResult> handler, HookMode mode)
    {
        NativeAPI.UnhookFunction(Handle, handler, mode == HookMode.Post);
    }

    protected T InvokeInternal<T>(params object[] args)
    {
        return NativeAPI.ExecuteVirtualFunction<T>(Handle, args);
    }

    protected void InvokeInternalVoid(params object[] args)
    {
        NativeAPI.ExecuteVirtualFunction<object>(Handle, args);
    }
}
