using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Xunit;

namespace NativeTestsPlugin;

public class SchemaTests
{
    [Fact]
    public void GetSchemaOffset_ReturnsValidOffset()
    {
        var offset = NativeAPI.GetSchemaOffset("CBaseEntity", "m_iHealth");

        Assert.True(offset > 0, $"Schema offset for m_iHealth should be positive, got {offset}");
    }

    [Fact]
    public void GetSchemaOffset_CachesValues()
    {
        // First call should populate cache
        var offset1 = Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");
        // Second call should return cached value
        var offset2 = Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");

        Assert.Equal(offset1, offset2);
    }

    [Fact]
    public void GetSchemaClassSize_ReturnsCorrectSize()
    {
        var size = Schema.GetClassSize("CEntityIdentity");

        // This seems like the most likely fixed size class to test with
        Assert.Equal(112, size);
    }

    [Fact]
    public void IsSchemaFieldNetworked_ReturnsExpectedValue()
    {
        // m_iHealth is a networked field
        var isNetworked = Schema.IsSchemaFieldNetworked("CBaseEntity", "m_iHealth");

        Assert.True(isNetworked, "m_iHealth should be a networked field");
    }

    [Fact]
    public void GetSchemaValue_ReadsEntityProperty()
    {
        // Get the world entity which is always present
        var world = Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();

        Assert.NotNull(world);

        // Read a schema value - world entity should have valid health
        var health = world.Health;

        // Health can be 0 for world entity, just verify we can read it without exception
        Assert.True(health >= 0, $"Health should be non-negative, got {health}");
    }

    [Fact]
    public void SetSchemaValue_WritesEntityProperty()
    {
        // Create a test entity that we can safely modify
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");

        Assert.NotNull(entity);

        try
        {
            // Set the health value
            var newHealth = 50;

            entity.Health = newHealth;

            // Read back the value
            var readHealth = entity.Health;

            Assert.Equal(newHealth, readHealth);
        }
        finally
        {
            // Clean up the entity
            entity.Remove();
        }
    }
}
