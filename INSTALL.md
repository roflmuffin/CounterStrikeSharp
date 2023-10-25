# Installation

1. Download and install Metamod:Source for CS2. Detailed instructions can be found [here](https://cs2.poggu.me/metamod/installation/).
2. Download the latest release of CounterStrikeSharp from [here](https://github.com/roflmuffin/CounterStrikeSharp/actions/workflows/cmake-single-platform.yml).
   - If this is your first time installing, you will need to download the `with-runtime` version. This includes the .NET runtime, which is required to run the plugin.
   - Subsequent upgrades will not require the runtime, unless a version bump of the .NET runtime is required (i.e. from 7.0.x to 8.0.x).
   - Depending on the os you might also need to install `libicu` / `icu-libs` / `libicu-dev` using your package manager for .NET to run.
3. Extract the `addons` folder to the `/csgo/` directory of the dedicated server.
4. Start the server. If everything is working correctly, you should see a message in the console that says `CounterStrikeSharp.API Loaded Successfully.`