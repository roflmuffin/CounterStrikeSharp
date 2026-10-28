using System;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using Xunit;

namespace NativeTestsPlugin;

public class EntitySystemTests
{
    private static bool _lookedUpOnLoad;
    private static bool _lookupFailedOnLoad;

    internal static void LookUpEntitiesOnLoad()
    {
        // On a cold server start, Load runs before the entity system exists and this lookup fails. The failure must not
        // be remembered, or every entity lookup afterwards fails the same way. On a hot reload the map is already up and
        // the lookup succeeds, so the run log says which of the two happened.
        try
        {
            _ = Utilities.GetAllEntities().Count();
        }
        catch (NativeException e)
        {
            _lookupFailedOnLoad = true;
            Console.WriteLine($"[EntitySystemTests] entity lookup during Load failed, as it does on a cold start: {e.Message}");
        }

        if (!_lookupFailedOnLoad)
        {
            Console.WriteLine("[EntitySystemTests] entity lookup during Load succeeded: the map was already up, the cold start case was not exercised");
        }

        _lookedUpOnLoad = true;
    }

    [Fact]
    public void EntityLookups_AfterTheLookupDuringLoad_Work()
    {
        Assert.True(_lookedUpOnLoad);
        Assert.NotEmpty(Utilities.GetAllEntities());
        Assert.NotNull(EntitySystem.GetEntityByIndex(0));
    }
}
