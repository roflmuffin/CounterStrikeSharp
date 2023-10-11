using System;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public record ValveInterface
{
    enum ValveInterfaceType
    {
        Engine,
        Server,
    }

    private ValveInterface(ValveInterfaceType type, string name)
    {
        this.Type = type;
        this.Name = name;
    }

    private ValveInterfaceType Type { get; }
    public string Name { get; }

    public IntPtr Pointer => NativeAPI.GetValveInterface((int)Type, Name);

    public override string ToString()
    {
        return this.Pointer.ToString();
    }

    public static ValveInterface Engine => new(ValveInterfaceType.Engine, "Source2EngineToServer001");
    public static ValveInterface CVars => new(ValveInterfaceType.Engine, "VEngineCvar007");
    public static ValveInterface Server => new(ValveInterfaceType.Server, "Source2Server001");
    public static ValveInterface ServerGameClients => new(ValveInterfaceType.Server, "Source2GameClients001");
    public static ValveInterface NetworkServerService => new(ValveInterfaceType.Engine, "NetworkServerService_001");
    public static ValveInterface GameEventSystem => new(ValveInterfaceType.Engine, "GameEventSystemServerV001");
}