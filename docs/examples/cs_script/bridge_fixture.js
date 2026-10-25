// Test asset source. Compile/mount using the game's .vjs resource pipeline.
// The native fixture must associate readiness with this host's context.
import { Instance, Console, GameRules } from "cs_script/point_script";

Console.Log("[css bridge fixture] module evaluated");

Instance.OnActivate(() => {
    Console.Log("[css bridge fixture] activated");
});

// Exercises the engine's normal context/scope entry without changing the match.
Instance.OnScriptInput("__css_bridge_probe", () => {
    const warmup = GameRules.IsWarmupPeriod();
    Console.Log(`[css bridge fixture] input probe warmup=${warmup}`);
});
