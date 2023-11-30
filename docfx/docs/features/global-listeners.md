# Global Listeners

How to subscribe to CounterStrikeSharp global listeners.

## Adding a Listener

Global listeners come in a variety of shapes so there is no automatic registration for these, they must be registered in the `OnLoad` of your plugin (or anywhere you have access to the plugin instance). You can find the full list of event listeners in the `Listeners` class as seen below.

```csharp
public override void Load(bool hotReload)
{
    RegisterListener<Listeners.OnEntitySpawned>(entity =>
    {
        if (entity.DesignerName != "smokegrenade_projectile") return;

        var projectile = new CSmokeGrenadeProjectile(entity.Handle);

        // Changes smoke grenade colour to a random colour each time.
        Server.NextFrame(() =>
        {
            projectile.SmokeColor.X = Random.Shared.NextSingle() * 255.0f;
            projectile.SmokeColor.Y = Random.Shared.NextSingle() * 255.0f;
            projectile.SmokeColor.Z = Random.Shared.NextSingle() * 255.0f;
            Logger.LogInformation("Smoke grenade spawned with color {SmokeColor}", projectile.SmokeColor);
        });
    });
}
```
