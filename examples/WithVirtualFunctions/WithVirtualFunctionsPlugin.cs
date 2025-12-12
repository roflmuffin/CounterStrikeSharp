using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

using Microsoft.Extensions.Logging;

namespace WithVirtualFunctions;

public class WithVirtualFunctionsPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Virtual Functions";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that hooks virtual functions by signature and offset";

    // There are 3 ways to access a virtual function:
    // 1 -> By signature
    // 2 -> By the vtable offset (index) and an instance of the class
    // 3 -> By the vtable offset (index) and without an instance of the class, but you need to know the vtable symbol

    // In this example, we are going to cover each method.
    // Note: we won't cover RE here.

    // 1 -> Working with a signature:

    // this example shows that you can just pass the signature of the function
    // Note: CS# assumes the server binary by default. there are overloads that allow you to specify a different binary.
    // this example will continue inside the "Load" method.
    private static VirtualFunctionVoid<CCSPlayerController, CsTeam> CCSPlayerController_SwitchTeam = new(GameData.GetSignature("CCSPlayerController_SwitchTeam"));

    // 2 -> Working with a vtable offset and an instance of the class
    // Since we need an active instance of the class, we can only declare our virtual function here, and the rest of the code will be inside the "Load" method.
    // Declare it as a nullable type, because we only set a value when this is being used.
    private static VirtualFunctionVoid<CCSPlayer_ItemServices, string, CEntityInstance>? CCSPlayer_ItemServices_GiveNamedItem;

    // 3 -> Working with a vtable offset without an instance:

    // this example shows that you can just pass the offset of the function, and CS# will automatically gather the VTable and access the function at the given offset.
    // Note: CS# assumes the server binary by default. there are overloads that allow you to specify a different binary.
    // Note: here, CS# uses the 'CCSPlayerController' (TArg1) argument type name as the "vtable symbol" and you should only rely on this if you know what you are doing.
    // Note: if you need more freedom, there are other variants that supports custom parameters. (Check below)
    private static VirtualFunctionVoid<CCSPlayerController, CsTeam> CCSPlayerController_ChangeTeam = new(GameData.GetOffset("CCSPlayerController_ChangeTeam"));

    // this example is pretty much the same as above, but you should prefer this method if you are not using explicit types, or the vtable symbol is not the same as a predefined class name. (usually when you use 'nint' instead)
    // Note: you can still use the `CCSPlayerController` as TArg1, the main point here is if you explicitly set the vtable symbol name, then that value will be used and CS# will NOT use the TArg1 type name.
    private static VirtualFunctionVoid<nint, CsTeam> CCSPlayerController_Respawn = new("CCSPlayerController", GameData.GetOffset("CCSPlayerController_Respawn"));

    // this is still the same, the main point here is that you can set the vtable symbol, binary path, and the offset.
    // private static VirtualFunctionVoid<nint, int> Random_Function = new("VTABLE_SYMBOL_IN_ENGINE_BINARY", Addresses.EnginePath, 51); // this offset is a random example here

    private static VirtualFunctionWithReturn<CCSGameRules, CBasePlayerController, IntPtr?, double, double, CBaseEntity?>? CCSGameRules_FindPickerEntity;

    // Also there are wrapper classes that you can use:
    // Note that this class actually holds the vtable ptr. (.Handle)
    // VTable CCSGameRules_VTable_Symbol = new VTable("CCSGameRules"); // this will look for "CCSGameRules" vtable symbol, and if found, you can use this class to retrieve the functions.

    // this is the same as above, but CS# here will use the `TClass` type name as the vtable symbol, and when you retrieve functions, you don't have to specify the TArg1 each time, just the parameters.
    VTable<CCSGameRules> CCSGameRules_VTable = new VTable<CCSGameRules>();

    // examples can be found in the load method.

    public override void Load(bool hotReload)
    {
        // 1 -> By signature
        // setup a hook on the virtual function that we got using the signature.
        CCSPlayerController_SwitchTeam.Hook(OnSwitchTeam, HookMode.Pre);

        // 2 -> By the vtable offset (index) and an instance of the class
        // so we need to somehow get an instance of `CCSPlayer_ItemServices`, the following code will be a random example and it depends on context.

        AddCommand("vfunc_2", "Example way of accessing a virtual function", (controller, info) =>
        {
            // we don't want to setup the same thing over and over again
            if (CCSPlayer_ItemServices_GiveNamedItem != null)
                return;

            // sanity checks are up to you
            if (controller == null || !controller.IsValid || controller.IsBot || controller.PlayerPawn.Value == null)
                return;

            if (controller.PlayerPawn.Value.ItemServices == null)
                return;

            // as you can see we used an active instance of the class (controller.PlayerPawn.Value.ItemServices) to access the virtual function.
            // if you hook this function, any and every call to it will be intercepted, regardless of the instance.
            // in this way, the instance is only needed to access the function through the vtable.
            CCSPlayer_ItemServices_GiveNamedItem = new(controller.PlayerPawn.Value.ItemServices, GameData.GetOffset("CCSPlayer_ItemServices_GiveNamedItem"));
            CCSPlayer_ItemServices_GiveNamedItem.Hook(OnGiveNamedItem, HookMode.Pre);
        });

        // 3 -> By the vtable offset (index) and without an instance of the class

        // we have already created our virtual function, so we can just use it here.
        CCSPlayerController_ChangeTeam.Hook(OnChangeTeam, HookMode.Pre);

        // Wrapper examples

        // when using the generic variant of the VTable class, you only need to pass the generic parameters of the function, TArg1 is assumed to be `CCSGameRules`
        CCSGameRules_FindPickerEntity = CCSGameRules_VTable.GetFunctionWithReturn<CBasePlayerController, IntPtr?, double, double, CBaseEntity?>(GameData.GetOffset("CCSGameRules_FindPickerEntity"));
        CCSGameRules_FindPickerEntity.Hook(OnFindPickerEntity, HookMode.Pre);

        // and when you are not using the generic variant, you also need to pass the `CCSGameRules` each time.
        // CCSGameRules_VTable_Symbol.GetFunctionWithReturn<CCSGameRules, CBasePlayerController, CBaseEntity?>(GameData.GetOffset("CCSGameRules_FindPickerEntity")).Hook(OnFindPickerEntity, HookMode.Pre);
    }

    private HookResult OnChangeTeam(DynamicHook hook)
    {
        Logger.LogInformation("ON CHANGE TEAM");
        return HookResult.Continue;
    }

    private HookResult OnGiveNamedItem(DynamicHook hook)
    {
        string itemName = hook.GetParam<string>(1);
        Logger.LogInformation("ON GIVE NAMED ITEM {0}", itemName);
        return HookResult.Continue;
    }

    private HookResult OnSwitchTeam(DynamicHook hook)
    {
        Logger.LogInformation("ON SWITCH TEAM");
        return HookResult.Continue;
    }

    private HookResult OnFindPickerEntity(DynamicHook hook)
    {
        Logger.LogInformation("ON FIND PICKER ENTITY");
        return HookResult.Continue;
    }

    public override void Unload(bool hotReload)
    {
        // Dont forget to release your hooks

        // 1 -> By signature
        CCSPlayerController_SwitchTeam.Unhook(OnSwitchTeam, HookMode.Pre);

        // 2 -> By the vtable offset (index) and an instance of the class
        CCSPlayer_ItemServices_GiveNamedItem?.Unhook(OnGiveNamedItem, HookMode.Pre);

        //3 -> By the vtable offset (index) and without an instance of the class
        CCSPlayerController_ChangeTeam.Unhook(OnChangeTeam, HookMode.Pre);
        CCSGameRules_FindPickerEntity?.Unhook(OnFindPickerEntity, HookMode.Pre);
    }
}