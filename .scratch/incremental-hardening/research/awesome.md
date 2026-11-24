<div align="center" style="text-align: center">
<h3>Awesome CS2</h3>
<p>
  An awesome list for all available cs2 development information, including modding frameworks, development resources, plugins and etc.
</p>
<p>
  Last Sync: <strong>2026/11/22 16:05:07</strong>
</p>
<p>
  Project Uptime: <strong>1 year</strong>
</p>

</div>

# Contents
- [General](#general)
- [Tools](#tools)
- [Status Trackers](#status-trackers)
- [Modding Frameworks](#modding-frameworks)
- [Development Resources](#development-resources)
- [Communities](#communities)
- [Plugins](#plugins)
  - [Metamod Plugins](#metamod-plugins)
  - [CounterStrikeSharp Plugins](#counterstrikesharp-plugins)
  - [SwiftlyS2 Plugins](#swiftlys2-plugins)
- [Contributors](#contributors)
- [Contributing](#contributing)

# General
- **[Source2 Wiki](https://source2.wiki)**\
  *An actively maintained source2 wiki, where you can learn information about workshop tools and entities.*

- **[cs2browser.net](https://cs2browser.net)**\
  *A clean and live cs2 community server browser without fake servers and advertisements.*

- **[Valve Official Documentation](https://developer.valvesoftware.com/)**\
  *Valve's official documentation for cs2 and other source games.*\
  ***Not recommended for checking cs2 information, most of them are outdated or absent.***

- **[gamebanana.com](https://gamebanana.com/games/18134)**\
  *A workshop resource website that contains cs2 models, maps and etc.*

# Tools
- **[Source2Viewer](https://s2v.app/)**\
  *A powerful tool to unpack and decompile resources from vpk files.*

- **[VPulse Editor](https://github.com/LionDoge/vpulse-editor)**\
  *A visual programming tool to write the vpulse script, which is a new unreleased visualized script system for source2 to replace vscript.*

- **[Schema Dumper](https://github.com/GAMMACASE/Source2SchemaDumper)**\
  *A metamod plugin for dumping schemas from game binary, and also able to generate a header file that can be imported in IDA for reverse engineering.*

- **[DepotDownloader](https://github.com/SteamRE/DepotDownloader)**\
  *A tool to download game files of a specific version.*

- **[PltPatcher](https://github.com/GAMMACASE/PltPatcher)**\
  *A IDA python plugin to patch plt sections when IDA fails to do so automatically, you might encounter such problem when decompiling linux binaries, made by GAMMACASE.*

# Status Trackers

- **[SteamDB](https://steamdb.info/app/730/charts/)**\
  *A website tracking steam games information.*

- **[cs2-signatures](https://github.com/ianlucas/cs2-signatures)**\
  *A github repo for tracking gamedata status after a game update.*

# Modding Frameworks
- **[Metamod](https://github.com/alliedmodders/metamod-source)** [[Website](https://www.sourcemm.net/downloads.php?branch=dev)] [[Discord](https://discord.com/invite/HUc67zN)]\
  *The cobblestone of all of the current open source framework since source1, written in C++, hard to develop but have access to fundamental things.*\
  ***Please notice that it's the dev branch that support cs2, make sure you download the right version.***
  
- **[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)** [[Website](https://docs.cssharp.dev/)] [[Discord](https://discord.com/invite/eAZU3guKWU)]\
  *The most popular modding framework for cs2, the plugins are written in c#.*\
  *Alias: CS#, cssharp*

- **[SwiftlyS2](https://github.com/swiftly-solution/swiftlys2)** [[Website](https://swiftlys2.net)] [[Discord](https://swiftlys2.net/discord)]\
  *A modding framework that support c#, with more features than counterstrikesharp.*

- **[Modsharp](https://github.com/Kxnrl/modsharp-public)** [[Website](https://docs.modsharp.net/)] [[Discord](https://discord.com/invite/wKarAjHm2G)]
  *Modern Source 2 Modding Solution.*

- **[Plugify](https://github.com/untrustedmodders/plugify)** [[Website](https://plugify.net)] [[Discord](https://discord.gg/untrustedmodders)]\
  *A modding framework that support tons of programming languages, more information can be found in there discord channel and website.*

# Development Resources
These are the websites with resources that you might need to check during development.

- **[ConVars](https://github.com/SteamDatabase/GameTracking-CS2/blob/master/DumpSource2/convars.txt)**\
  *A list of all cs2 convars, directly dumped from latest cs2 binary.*

- **[Commands](https://github.com/SteamDatabase/GameTracking-CS2/blob/master/DumpSource2/commands.txt)**\
  *A list of all cs2 commands, directly dumped from latest cs2 binary.*

- **[Game Events](https://cs2.poggu.me/dumped-data/game-events)**\
  *A list of all cs2 game events.*\
  *The data might be outdated, if so, you can find the latest ones here:*\
  *[core.gameevents](https://github.com/SteamDatabase/GameTracking-CS2/blob/master/game/core/pak01_dir/resource/core.gameevents)*
  *[game.gameevents](https://github.com/SteamDatabase/GameTracking-CS2/blob/master/game/csgo/pak01_dir/resource/game.gameevents)*
  *[mod.gameevents](https://github.com/SteamDatabase/GameTracking-CS2/blob/master/game/csgo/pak01_dir/resource/mod.gameevents)*

- **[Schemas](https://github.com/SteamDatabase/GameTracking-CS2/tree/master/DumpSource2/schemas)**\
  *A list of all cs2 schema classes and fields, directly dumped from latest cs2 binary.*

- **[Schema Explorer](https://s2v.app/SchemaExplorer/)**\
  *A website that visualize all schema classes and fields, with advanced query features.

- **[Source2 Dumper](https://boeing666.github.io/source2-dumper)**\
  *A website that visualize all schema classes and fields, convars, commands and etc.

- **[Entities](https://source2.wiki)**\
  *A list of all cs2 entities.*

- **[Protobufs](https://github.com/SteamDatabase/GameTracking-CS2/blob/master/Protobufs)**\
  *A list of all cs2 protobuf definitions.*

- **[Datamaps](https://swiftlys2.net/docs/api/datamaps/)**\
  *A list of all cs2 datamap (entity keyvalues field, input, output and etc.)*
  
- **[gdc.kitsune-lab.com](https://gdc.kitsune-lab.com)**\
  *A online gamedata tracker and validator, maintaining an active list for function signatures and offsets. If there's an game update that break your server, you might find the new gamedata here.*

- **[wiki.alliedmods.net](https://wiki.alliedmods.net/)**\
  *Alliedmodders' wiki. You can find documentation about metamod, ambuild and sourcehook development here.*

- **[HL2SDK](https://github.com/alliedmodders/hl2sdk/tree/cs2)**\
  *Actively maintained SDK for cs2.*

- **[HL2SDK Wend4r's fork](https://github.com/Wend4r/sourcesdk)**\
  *A fork of hl2sdk, if there's a game update, you might find what have changed in here or in pull requests.*

- **[cstrike15_src](https://github.com/perilouswithadollarsign/cstrike15_src)**\
  *CSGO Leaked source code in 2020, cs2 is still using some codes from it.*

- **[cs2.poggu.me](https://cs2.poggu.me/)**\
  *A general information website made by poggu.*

- **[Source2ZE Map Porting Guide](https://docs.google.com/document/d/1buKzjP-2com9GcXVxCfyRBi6sDiKmzMoVy9RNbYQqIo)**\
  *A guide about porting map from csgo, written by s2ze community.*

- **[S2TS](https://s2ts.net/)**\
  *A project that compiles typescript codes into .vts_c files that can run in cs2 scripting engine.*\
  *Also provide documentation for current SourceTS API.*

# Communities
- **[Alliedmodders Discord](https://discord.com/invite/HUc67zN)**\
  *Official discord for alliedmodders, you can find discussions about metamod and sdk here.*

- **[Source2ZE Discord](https://discord.gg/QsSGf9ZEVs)**\
  *Official discord for source2ze community, mainly targeting workshop developers, you can find discussions including maps, models and cs2fixes here.*

- **[CounterStrikeSharp Discord](https://discord.com/invite/eAZU3guKWU)**\
  *Official discord for counterstrikesharp, you can find plugins, code snippets and development discussion here.*

- **[ModSharp Discord](https://discord.gg/wKarAjHm2G)**\
  *Official discord for modsharp, which is a currently unreleased modding framework.*

- **[SwiftlyS2 Discord](https://swiftlys2.net/discord)**\
  *Official discord for swiftlys2, you can find swiftlys2 plugins, code snippets, and discuss about swiftlys2 modding framework here.*

- **[Plugify Discord](https://discord.gg/untrustedmodders)**\
  *Official discord for plugify, you can find plugify plugins, code snippets, and discuss about plugify modding framework here.*

- **[bbs.csgocn.net](https://bbs.csgocn.net/)**\
  *A Chinese forum for cs2 server development.*

- **[hlmod.net](https://hlmod.net/)**\
  *A Russian forum for source games development.*

# Plugins

## Metamod Plugins

- **[Source2ZE/CS2Fixes](https://github.com/Source2ZE/CS2Fixes)**<br>
  ⭐ 364<br>
  ⏱️ updated 12 hours ago<br>
  *A plugin with tons of fixes and features aimed but not limited to zombie escape.*

- **[KZGlobalTeam/cs2kz-metamod](https://github.com/KZGlobalTeam/cs2kz-metamod)**<br>
  ⭐ 212<br>
  ⏱️ updated 3 days ago<br>
  *KZ plugin for cs2. WIP, not ready for release.*

- **[Source2ZE/MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager)**<br>
  ⭐ 138<br>
  ⏱️ updated 1 week ago<br>
  *A plugin that allows you to use multiple workshop addons at once and have clients download them.*

- **[Source2ZE/ServerListPlayersFix](https://github.com/Source2ZE/ServerListPlayersFix)**<br>
  ⭐ 70<br>
  ⏱️ updated 1 month ago<br>
  *Populates the user information inside the steam api, as a result this fixes the players not showing up in the server browser.*

- **[Source2ZE/CleanerCS2](https://github.com/Source2ZE/CleanerCS2)**<br>
  ⭐ 68<br>
  ⏱️ updated 2 months ago<br>
  *A simple plugin that allows you to filter out console prints with regular expressions.*

- **[Source2ZE/CS2ServerGUI](https://github.com/Source2ZE/CS2ServerGUI)**<br>
  ⭐ 67<br>
  ⏱️ updated 4 weeks ago<br>
  *A plugin that shows entities, net messages, events and etc with a GUI.*

- **[Source2ZE/MovementUnlocker](https://github.com/Source2ZE/MovementUnlocker)**<br>
  ⭐ 66<br>
  ⏱️ updated 1 month ago<br>
  *A plugin that removes the max speed limitation from players on the ground, feels like CS:S. Useful for zombie knockback, several movement gamemodes, or just easier bhop.*

- **[Salvatore-Als/cs2-fake-rcon](https://github.com/Salvatore-Als/cs2-fake-rcon)**<br>
  ⭐ 64<br>
  ⏱️ updated 1 day ago<br>
  *Fakercon adds the fake_rcon_password and fake_rcon commands because Valve has not integrated the original command into the game (or it is broken).*

- **[Source2ZE/StripperCS2](https://github.com/Source2ZE/StripperCS2)**<br>
  ⭐ 57<br>
  ⏱️ updated 2 months ago<br>
  *A plugin that allows server operators to manage map lump data similarly to how Stripper:Source worked.*

- **[Cruze03/FakeRanks-RevealAll](https://github.com/Cruze03/FakeRanks-RevealAll)**<br>
  ⭐ 54<br>
  ⏱️ updated 1 week ago<br>
  *A metamod plugin that reveals ranks of all players in server for player who presses tab.*

- **[Interesting-exe/CS2Fixes-RampbugFix](https://github.com/Interesting-exe/CS2Fixes-RampbugFix)**<br>
  ⭐ 35<br>
  ⏱️ updated 1 month ago<br>
  *Minimizes rampbugs. This plugin isn't perfect and rampbugs will continue to occur until Valve decides to finally fix them.*

- **[XBribo/CS2-Bot-Hider](https://github.com/XBribo/CS2-Bot-Hider)**<br>
  ⭐ 34<br>
  ⏱️ updated 3 days ago<br>
  *CS2-Bot-Hider is a plugins that can disguise CS2 bots as real players.*

- **[Source2ZE/AcceleratorCS2](https://github.com/Source2ZE/AcceleratorCS2)**<br>
  ⭐ 31<br>
  ⏱️ updated 2 months ago<br>
  *A plugin that generate a crash dump when there's a crash.*

- **[XBribo/CS2-Bot-Controller](https://github.com/XBribo/CS2-Bot-Controller)**<br>
  ⭐ 30<br>
  ⏱️ updated 1 day ago<br>
  *CS2-Bot-Controller(CS2-Bot-Mimic2) is a plugin that takes control of a bot's behaviour.*

- **[GAMMACASE/Source2SchemaDumper](https://github.com/GAMMACASE/Source2SchemaDumper)**<br>
  ⭐ 28<br>
  ⏱️ updated 4 weeks ago<br>
  *A plugin that can dump the schemas and generate a header file.*

- **[Cruze03/GameBanFix](https://github.com/Cruze03/GameBanFix)**<br>
  ⭐ 24<br>
  ⏱️ updated 1 month ago<br>
  *Fixes issue where if a player with game ban joins, other players even without a ban are then unable to join.*

- **[samyycX/Audio](https://github.com/samyycX/Audio)**<br>
  ⭐ 17<br>
  ⏱️ updated 1 month ago<br>
  *A demo and a metamod lib to provide similar functions to the previous SM-Ext-Audio extension in csgo, that is, sending custom audio streams such as a song through a bot's voice chat.*


## CounterStrikeSharp Plugins

- **[shobhit-pathak/MatchZy](https://github.com/shobhit-pathak/MatchZy)**<br>
  ⭐ 511<br>
  ⏱️ updated 1 week ago<br>
  *MatchZy is a plugin for CS2 (Counter Strike 2) for running and managing practice/pugs/scrims/matches with easy configuration!*

- **[Nereziel/cs2-WeaponPaints](https://github.com/Nereziel/cs2-WeaponPaints)**<br>
  ⭐ 420<br>
  ⏱️ updated 1 hour ago<br>
  *A plugin to change weapon paints, gloves, agents and etc.*

- **[B3none/cs2-retakes](https://github.com/B3none/cs2-retakes)**<br>
  ⭐ 323<br>
  ⏱️ updated 3 days ago<br>
  *CS2 implementation of retakes written in C# for CounterStrikeSharp. Based on the version for CS:GO by Splewis.*

- **[daffyyyy/CS2-SimpleAdmin](https://github.com/daffyyyy/CS2-SimpleAdmin)**<br>
  ⭐ 192<br>
  ⏱️ updated 1 month ago<br>
  *Manage your Counter-Strike 2 server with simple commands!*

- **[counterstrikesharp-panel/css-bans](https://github.com/counterstrikesharp-panel/css-bans)**<br>
  ⭐ 146<br>
  ⏱️ updated 1 month ago<br>
  *CSS-BANS is an admin web panel for Counter-Strike 2, powered by CounterStrikeSharp.*

- **[samyycX/CS2-PlayerModelChanger](https://github.com/samyycX/CS2-PlayerModelChanger)**<br>
  ⭐ 138<br>
  ⏱️ updated 1 month ago<br>
  *A cssharp plugin to change player models.*

- **[NockyCZ/CS2-Deathmatch](https://github.com/NockyCZ/CS2-Deathmatch)**<br>
  ⭐ 135<br>
  ⏱️ updated 1 month ago<br>
  *A plugin to implement deathmatch gamemode.*

- **[NiGHT757/AFKManager](https://github.com/NiGHT757/AFKManager)**<br>
  ⭐ 86<br>
  ⏱️ updated 1 year ago<br>
  *AFK Manager plugin for CS2 based on player-checker by sazonische from CS:GO.*

- **[Cruze03/FortniteEmotesNDances](https://github.com/Cruze03/FortniteEmotesNDances)**<br>
  ⭐ 78<br>
  ⏱️ updated 1 month ago<br>
  *This plugin allows players to use Emotes & Dances just like Fortnite.*

- **[partiusfabaa/cs2-VIPCore](https://github.com/partiusfabaa/cs2-VIPCore)**<br>
  ⭐ 78<br>
  ⏱️ updated 16 minutes ago<br>
  *A vip plugin for cs2.*

- **[oqyh/cs2-Game-Manager-GoldKingZ](https://github.com/oqyh/cs2-Game-Manager-GoldKingZ)**<br>
  ⭐ 73<br>
  ⏱️ updated 4 months ago<br>
  *Block/Hide unnecessaries in game.*

- **[partiusfabaa/cs2-ranks](https://github.com/partiusfabaa/cs2-ranks)**<br>
  ⭐ 69<br>
  ⏱️ updated 1 year ago<br>
  *Each player is assigned a rank based on their accumulated experience points. Ranks range from "None" to the prestigious "The Global Elite."*

- **[B3none/cs2-instadefuse](https://github.com/B3none/cs2-instadefuse)**<br>
  ⭐ 68<br>
  ⏱️ updated 3 weeks ago<br>
  *A plugin that allows players to instantly defuse the bomb.*

- **[Oz-Lin/cs2-rockthevote](https://github.com/Oz-Lin/cs2-rockthevote)**<br>
  ⭐ 66<br>
  ⏱️ updated 1 week ago<br>
  *General purpose map voting plugin, started as a simple RTV and now has more features.*

- **[Lan2Play/PugSharp](https://github.com/Lan2Play/PugSharp)**<br>
  ⭐ 61<br>
  ⏱️ updated 9 months ago<br>
  *Pugsharp is a PUG System Plugin for CS2 based on the awesome CounterStrikeSharp by roflmuffin.*

- **[daffyyyy/CS2-Tags](https://github.com/daffyyyy/CS2-Tags)**<br>
  ⭐ 60<br>
  ⏱️ updated 1 year ago<br>
  *Adds tags to the server that can be easily edited, tags can be assigned via permission or steamid64.*

- **[CounterStrike2-Plugins-Archive/cs2-tags](https://github.com/CounterStrike2-Plugins-Archive/cs2-tags)**<br>
  ⭐ 50<br>
  ⏱️ updated 1 year ago<br>
  *A tag plugin designed to enhance your CS2 experience with a dynamic tagging system. Customise and manage player tags effortlessly for a more interactive and engaging game environment.*

- **[schwarper/CS2MenuManager](https://github.com/schwarper/CS2MenuManager)**<br>
  ⭐ 49<br>
  ⏱️ updated 4 months ago<br>
  *CS2MenuManager is a modern, extensible and easy to use menu system for Counter-Strike 2 based on the CounterStrikeSharp library.*

- **[KillStr3aK/ResourcePrecacher](https://github.com/KillStr3aK/ResourcePrecacher)**<br>
  ⭐ 43<br>
  ⏱️ updated 1 year ago<br>
  *This plugin can precache custom resources.*

- **[HvH-gg/CS2-Essentials](https://github.com/HvH-gg/CS2-Essentials)**<br>
  ⭐ 38<br>
  ⏱️ updated 1 week ago<br>
  *It includes basic features like reset score and rage quit as well as optional restrictions for weapons, friendly fire, rapid fire and other exploit/crash fixes.*

- **[schwarper/CS2TraceRay](https://github.com/schwarper/CS2TraceRay)**<br>
  ⭐ 37<br>
  ⏱️ updated 8 months ago<br>
  *A trace ray library developed for use in Counter Strike 2, in conjunction with the CounterStrikeSharp API. This enables the use of trace ray with TraceMask, Contents and skip enums.*

- **[zwolof/cs2-executes](https://github.com/zwolof/cs2-executes)**<br>
  ⭐ 37<br>
  ⏱️ updated 1 year ago<br>
  *CS2 implementation of executes written in C# for CounterStrikeSharp. Based on the version for CS:GO by Splewis.*

- **[oqyh/cs2-Chat-Logger-GoldKingZ](https://github.com/oqyh/cs2-Chat-Logger-GoldKingZ)**<br>
  ⭐ 33<br>
  ⏱️ updated 1 year ago<br>
  *Log chat to local/discord webhook/mysql/web server.*

- **[darkerz7/EntWatchSharp](https://github.com/darkerz7/EntWatchSharp)**<br>
  ⭐ 32<br>
  ⏱️ updated 4 weeks ago<br>
  *Notify players about entity interactions.*

- **[oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ](https://github.com/oqyh/cs2-Connect-Disconnect-Sound-GoldKingZ)**<br>
  ⭐ 32<br>
  ⏱️ updated 10 months ago<br>
  *Emit a sound when a player connects or disconnects.*

- **[ssypchenko/cs2-gungame](https://github.com/ssypchenko/cs2-gungame)**<br>
  ⭐ 30<br>
  ⏱️ updated 1 month ago<br>
  *GunGame is a gameplay plugin inspired by the SourceMode GunGame plugin.*

- **[R0mz1k/css-C4-Timer](https://github.com/R0mz1k/css-C4-Timer)**<br>
  ⭐ 30<br>
  ⏱️ updated 3 weeks ago<br>
  *This plugin adds countdown to c4 bomb explosion to your server.*

- **[Kandru/cs2-update-manager](https://github.com/Kandru/cs2-update-manager)**<br>
  ⭐ 23<br>
  ⏱️ updated 5 months ago<br>
  *The Plugin Update Manager is a plugin for Counter-Strike 2 designed to automatically update all your other plugins.*

- **[ugurhn/ThirdPerson-WIP](https://github.com/ugurhn/ThirdPerson-WIP)**<br>
  ⭐ 23<br>
  ⏱️ updated 1 year ago<br>
  *WIP ThirdPerson plugin for CS2 expect bugs.*

- **[Cruze03/Clientprefs](https://github.com/Cruze03/Clientprefs)**<br>
  ⭐ 22<br>
  ⏱️ updated 9 months ago<br>
  *This plugin exposes some natives for developers to save player data to SQLite / MySQL without actually adding sql code to your plugin giving developers easy access to save player cookie to database.*

- **[wiruwiru/AutomaticAds-CS2](https://github.com/wiruwiru/AutomaticAds-CS2)**<br>
  ⭐ 22<br>
  ⏱️ updated 3 months ago<br>
  *This plugin allows you to schedule and display announcements in the chat at customizable intervals. Each announcement is accompanied by a brief sound effect to capture players' attention seamlessly.*

- **[qstage/CS2-HidePlayers](https://github.com/qstage/CS2-HidePlayers)**<br>
  ⭐ 21<br>
  ⏱️ updated 1 year ago<br>
  *Allows you to hide player models.*

- **[Kandru/cs2-roll-the-dice](https://github.com/Kandru/cs2-roll-the-dice)**<br>
  ⭐ 21<br>
  ⏱️ updated 3 weeks ago<br>
  *This plugin lets your players roll the dice each round (at any time during an round) to get either a positive or negative effect for the current round.*

- **[Kandru/cs2-quake-sounds](https://github.com/Kandru/cs2-quake-sounds)**<br>
  ⭐ 20<br>
  ⏱️ updated 3 months ago<br>
  *This is a simple Quake Sound plugin for your server. It supports all types of sounds - only a workshop addon is necessesary.*

- **[zakriamansoor47/SLAYER_Duel](https://github.com/zakriamansoor47/SLAYER_Duel)**<br>
  ⭐ 19<br>
  ⏱️ updated 3 months ago<br>
  *This plugin allows players to do 1vs1 duel.*

- **[samyycX/CS2-SkyboxChanger](https://github.com/samyycX/CS2-SkyboxChanger)**<br>
  ⭐ 19<br>
  ⏱️ updated 1 month ago<br>
  *A plugin allow player to change their own skybox material, color and brightness on every map dynamically and seamlessly.*

- **[KillStr3aK/CS2-AntiDLL](https://github.com/KillStr3aK/CS2-AntiDLL)**<br>
  ⭐ 19<br>
  ⏱️ updated 1 year ago<br>
  *This plugin is similar to the CS:GO version.*

- **[qstage/CS2-FixRandomSpawn](https://github.com/qstage/CS2-FixRandomSpawn)**<br>
  ⭐ 19<br>
  ⏱️ updated 1 month ago<br>
  *Fixes convar `mp_randomspawn` for any game mode.*

- **[KKNecmi/ThirdPerson-Revamped](https://github.com/KKNecmi/ThirdPerson-Revamped)**<br>
  ⭐ 19<br>
  ⏱️ updated 3 months ago<br>
  *A modern, improved third-person camera plugin for Counter-Strike 2.*

- **[PhantomYopta/CS2_Speedometer](https://github.com/PhantomYopta/CS2_Speedometer)**<br>
  ⭐ 19<br>
  ⏱️ updated 2 years ago<br>
  *Just a speedometer for CS2.*

- **[samyycX/CSSharpPatcher](https://github.com/samyycX/CSSharpPatcher)**<br>
  ⭐ 18<br>
  ⏱️ updated 1 month ago<br>
  *A plugin with various patches for cs2.*

- **[Letaryat/CS2-Poor-MapPropAds](https://github.com/Letaryat/CS2-Poor-MapPropAds)**<br>
  ⭐ 17<br>
  ⏱️ updated 11 months ago<br>
  *This plugin allows for server owners to create billboard type advertisements that are placed on wall.*

- **[HvH-gg/TeleportFix](https://github.com/HvH-gg/TeleportFix)**<br>
  ⭐ 17<br>
  ⏱️ updated 1 week ago<br>
  *CounterStrikeSharp plugin to fix the teleport/airstuck/crash exploit.*

- **[mister-keno/advanced-ff-cs2](https://github.com/mister-keno/advanced-ff-cs2)**<br>
  ⭐ 16<br>
  ⏱️ updated 7 months ago<br>
  *This plugin implements advanced control of friendly fire.*

- **[Cruze03/cs2_blockradiocommands](https://github.com/Cruze03/cs2_blockradiocommands)**<br>
  ⭐ 16<br>
  ⏱️ updated 3 years ago<br>
  *A CSSharp plugin to block all radio commands.*

- **[Austinbots/CS2-BotAI](https://github.com/Austinbots/CS2-BotAI)**<br>
  ⭐ 16<br>
  ⏱️ updated 1 year ago<br>
  *Improves the built in bots AI.*

- **[schwarper/cs2-store](https://github.com/schwarper/cs2-store)**<br>
  ⭐ 15<br>
  ⏱️ updated 4 months ago<br>
  *A store plugin designed to enhance your gameplay by providing a dynamic credit system that allows players to purchase essential items directly from the store.*

- **[K4ryuu/K4-AlwaysWeaponSkins](https://github.com/K4ryuu/K4-AlwaysWeaponSkins)**<br>
  ⭐ 15<br>
  ⏱️ updated 9 months ago<br>
  *This plugin enhances weapon skin visibility by ensuring your equipped Steam inventory skins are displayed consistently across both teams.*

- **[asapverneri/CS2-Playervotes](https://github.com/asapverneri/CS2-Playervotes)**<br>
  ⭐ 15<br>
  ⏱️ updated 11 months ago<br>
  *Lightweight and efficient voting system for CS2 without anything pointless, allowing players to initiate votes for kicking, banning, and muting players.*

- **[T3Marius/T3Menu-API](https://github.com/T3Marius/T3Menu-API)**<br>
  ⭐ 14<br>
  ⏱️ updated 10 months ago<br>
  *T3Menu-API is a plugin created on counterstrikesharp with purpose of creating a better, refined menu controlled with player buttons.*

- **[Quantor97/CS2-Kill-Plugin](https://github.com/Quantor97/CS2-Kill-Plugin)**<br>
  ⭐ 14<br>
  ⏱️ updated 2 years ago<br>
  *A simple kill plugin that allows players to suicide via chat by entering '!suicide' or '/suicide'.*

- **[Kandru/cs2-challenges](https://github.com/Kandru/cs2-challenges)**<br>
  ⭐ 13<br>
  ⏱️ updated 1 month ago<br>
  *This plugin allows you to create Challenges for players. Challenges are tasks that players need to complete within a certain time frame (e.g., daily, weekly, monthly).*

- **[Kandru/cs2-demo-recorder](https://github.com/Kandru/cs2-demo-recorder)**<br>
  ⭐ 13<br>
  ⏱️ updated 5 months ago<br>
  *This tool automatically start a recording whenever someone is on your server. It makes sure to stop the recording before the level is being changed.*

- **[B3none/cs2-instaplant](https://github.com/B3none/cs2-instaplant)**<br>
  ⭐ 13<br>
  ⏱️ updated 2 years ago<br>
  *Plant the bomb instantly as a terrorist.*

- **[zakriamansoor47/SLAYER_AntiCamp](https://github.com/zakriamansoor47/SLAYER_AntiCamp)**<br>
  ⭐ 11<br>
  ⏱️ updated 3 months ago<br>
  *This plugin detect player who is camping for a specific time.*

- **[HvH-gg/RapidFireFix](https://github.com/HvH-gg/RapidFireFix)**<br>
  ⭐ 11<br>
  ⏱️ updated 1 week ago<br>
  *CounterStrikeSharp plugin to handle rapid fire in CS2.*

- **[zakriamansoor47/SLAYER_UnrestrictedFOV](https://github.com/zakriamansoor47/SLAYER_UnrestrictedFOV)**<br>
  ⭐ 10<br>
  ⏱️ updated 1 year ago<br>
  *This simple plugin allow players to change their FOV.*

- **[T3Marius/MVP-Anthem](https://github.com/T3Marius/MVP-Anthem)**<br>
  ⭐ 10<br>
  ⏱️ updated 1 year ago<br>
  *A plugin that add custom mvp sound to the game.*

- **[darkerz7/CS2-EntityFix](https://github.com/darkerz7/CS2-EntityFix)**<br>
  ⭐ 10<br>
  ⏱️ updated 1 month ago<br>
  *Fixes game_player_equip, game_ui, IgniteLifeTime, point_viewcontrol, trigger_gravity. Idea taken from cs2fixes.*

- **[NeuTroNBZh/CS2-AdminTools](https://github.com/NeuTroNBZh/CS2-AdminTools)**<br>
  ⭐ 9<br>
  ⏱️ updated 3 months ago<br>
  *Admin toolkit for private/custom servers — wallhack, invisibility, god mode, speed boost and more, with CS2-SimpleAdmin menu integration.*

- **[zakriamansoor47/SLAYER_HeadshotOnly](https://github.com/zakriamansoor47/SLAYER_HeadshotOnly)**<br>
  ⭐ 8<br>
  ⏱️ updated 1 year ago<br>
  *This plugin enables headshot only to all players or enables headshot only on players themselves by using public command !hs.*

- **[TICHOJEBEC-SK/cs2-WeaponRestrict](https://github.com/TICHOJEBEC-SK/cs2-WeaponRestrict)**<br>
  ⭐ 8<br>
  ⏱️ updated 1 year ago<br>
  *A plugin that lets you restrict weapons by rules.*

- **[darkerz7/CS2-HideTeammates](https://github.com/darkerz7/CS2-HideTeammates)**<br>
  ⭐ 8<br>
  ⏱️ updated 4 months ago<br>
  *Hides Teammates on the entire map or distance.*

- **[darkerz7/CSSharp-Fixes](https://github.com/darkerz7/CSSharp-Fixes)**<br>
  ⭐ 7<br>
  ⏱️ updated 2 months ago<br>
  *CS#Fixes is a CounterStrikeSharp plugin that fixes some bugs in Counter-Strike 2 and adds some commonly requested features.*

- **[asapverneri/CS2-ChatRelay](https://github.com/asapverneri/CS2-ChatRelay)**<br>
  ⭐ 7<br>
  ⏱️ updated 1 year ago<br>
  *Lightweight plugin to send your cs2 server chat messages to discord channel using webhook.*

- **[abnerfs/cs2-killfeed-filter](https://github.com/abnerfs/cs2-killfeed-filter)**<br>
  ⭐ 7<br>
  ⏱️ updated 3 years ago<br>
  *Show only your kills/assists/deaths in the killfeed, useful for DM servers where the killfeed has too much kills going on.*

- **[nvmxre/cs2-ui-kit](https://github.com/nvmxre/cs2-ui-kit)**<br>
  ⭐ 7<br>
  ⏱️ updated 1 month ago<br>
  *Clickable, flicker-free HUD panels on the custom_hud_layout entity — one entity with per-player state, Show/Hide/SetText/SetClass and a Clicked event, plus a worked example and a list of the engine gotchas.*

- **[zakriamansoor47/SLAYER_Noscope](https://github.com/zakriamansoor47/SLAYER_Noscope)**<br>
  ⭐ 6<br>
  ⏱️ updated 8 months ago<br>
  *This plugin disables scope of scope weapons like AWP, scout, etc.*

- **[Interesting-exe/QuickDefuse](https://github.com/Interesting-exe/QuickDefuse)**<br>
  ⭐ 6<br>
  ⏱️ updated 2 years ago<br>
  *Guess a wire for a chance to instantly defuse the bomb.*

- **[Ferks-FK/CS2-TeamLimiter](https://github.com/Ferks-FK/CS2-TeamLimiter)**<br>
  ⭐ 6<br>
  ⏱️ updated 1 year ago<br>
  *This plugin allows you to set a maximum number of players in both teams. Useful for PUG/MIX or Retake mode servers.*

- **[asapverneri/CS2-BotQuotaManager](https://github.com/asapverneri/CS2-BotQuotaManager)**<br>
  ⭐ 6<br>
  ⏱️ updated 2 years ago<br>
  *This plugin handles bot_quota depending on playercount on the server.*

- **[NeuTroNBZh/CS2-RETAKE](https://github.com/NeuTroNBZh/CS2-RETAKE)**<br>
  ⭐ 5<br>
  ⏱️ updated 1 month ago<br>
  *Feature-rich CS2 retake plugin with weapon selection menu, AWP restrictions, built-in InstaDefuse, CT kit distribution, and team scramble/queue management.*

- **[zakriamansoor47/SLAYER_Revive](https://github.com/zakriamansoor47/SLAYER_Revive)**<br>
  ⭐ 5<br>
  ⏱️ updated 1 year ago<br>
  *Revive teammates with 'E' (+use button).*

- **[M-archand/CS2FlashingHtmlHudFix](https://github.com/M-archand/CS2FlashingHtmlHudFix)**<br>
  ⭐ 5<br>
  ⏱️ updated 2 years ago<br>
  *A CS2 CS# Plugin that uses a workaround found by Poggu to make .PrintToCenterHTML not flash every second by setting.*

- **[R0mz1k/CSS-Knockback](https://github.com/R0mz1k/CSS-Knockback)**<br>
  ⭐ 5<br>
  ⏱️ updated 2 years ago<br>
  *This plugin, when hit, repels the recipient of the damage.*

- **[LeqenddC/CS2-BhopCommand](https://github.com/LeqenddC/CS2-BhopCommand)**<br>
  ⭐ 5<br>
  ⏱️ updated 2 months ago<br>
  *Toggles bunnyhop with !bhop and puts the server back to its previous movement settings when turned off.*

- **[NeuTroNBZh/CS2-BreakerAndOpenDoor](https://github.com/NeuTroNBZh/CS2-BreakerAndOpenDoor)**<br>
  ⭐ 4<br>
  ⏱️ updated 1 month ago<br>
  *Automatic round-start map cleanup for retake servers — opens doors and breaks windows/vents/breakables with a robust multi-pass, fallback-safe execution.*

- **[asapverneri/CS2-Gunsmenu](https://github.com/asapverneri/CS2-Gunsmenu)**<br>
  ⭐ 4<br>
  ⏱️ updated 8 months ago<br>
  *Up to date and lightweight gunmenu for CS2, allowing players to choose guns at any time of the round.*

- **[asapverneri/CS2-Baninfo](https://github.com/asapverneri/CS2-Baninfo)**<br>
  ⭐ 4<br>
  ⏱️ updated 2 years ago<br>
  *Useful plugin that shows connected players ban & mute history in console.*

- **[asapverneri/CS2-Legs](https://github.com/asapverneri/CS2-Legs)**<br>
  ⭐ 4<br>
  ⏱️ updated 2 years ago<br>
  *Simple plugin to disable lower body.*

- **[asapverneri/CS2-Clantags](https://github.com/asapverneri/CS2-Clantags)**<br>
  ⭐ 4<br>
  ⏱️ updated 1 year ago<br>
  *Extremely simple plugin to set admin/vip tag to scoreboard.*

- **[zakriamansoor47/SLAYER_1HitKill](https://github.com/zakriamansoor47/SLAYER_1HitKill)**<br>
  ⭐ 4<br>
  ⏱️ updated 1 year ago<br>
  *This simple plugin allows players to kill others in 1 Hit.*

- **[NeuTroNBZh/CS2-AntiSlow](https://github.com/NeuTroNBZh/CS2-AntiSlow)**<br>
  ⭐ 3<br>
  ⏱️ updated 1 month ago<br>
  *Blocks Shift slow-walk for targeted players, with temporary or permanent blocks and full localization support.*

- **[Dliix66/CS2-BotSlay](https://github.com/Dliix66/CS2-BotSlay)**<br>
  ⭐ 3<br>
  ⏱️ updated 2 years ago<br>
  *CSSharp plugin to slay all bots when the last player dies.*

- **[Austinbots/cs2-Change-Map-Rotation-Using-Text-File](https://github.com/Austinbots/cs2-Change-Map-Rotation-Using-Text-File)**<br>
  ⭐ 3<br>
  ⏱️ updated 1 year ago<br>
  *It allows you to change the map rotation using a text file.*

- **[LeqenddC/CS2-OnlyHS](https://github.com/LeqenddC/CS2-OnlyHS)**<br>
  ⭐ 3<br>
  ⏱️ updated 2 months ago<br>
  *Toggles headshot-only damage with !onlyhs, restored automatically when turned off or on map change.*

- **[NeuTroNBZh/CS2-SpawnEditor](https://github.com/NeuTroNBZh/CS2-SpawnEditor)**<br>
  ⭐ 2<br>
  ⏱️ updated 1 month ago<br>
  *In-game visual spawn editor companion for CS2-RETAKE — add, edit, delete and teleport to retake spawn points with live glow-pillar visualization.*

- **[NeuTroNBZh/CS2-STATPLAY](https://github.com/NeuTroNBZh/CS2-STATPLAY)**<br>
  ⭐ 2<br>
  ⏱️ updated 1 month ago<br>
  *Production-ready stats plugin — captures kills, rounds, objectives and grenades in real time, persists to MySQL, with !stats/!rank/!top commands and milestone webhooks.*

- **[NeuTroNBZh/CS2-Antibait](https://github.com/NeuTroNBZh/CS2-Antibait)**<br>
  ⭐ 2<br>
  ⏱️ updated 1 month ago<br>
  *Through-wall glow highlight tool for retake servers — permanent admin-toggled glow plus automatic last-CT-alive highlighting.*

- **[TICHOJEBEC-SK/cs2-WarnSystem](https://github.com/TICHOJEBEC-SK/cs2-WarnSystem)**<br>
  ⭐ 2<br>
  ⏱️ updated 1 year ago<br>
  *A plugin that allows admins to warn players.*

- **[asapverneri/CS2-ScoutzKnivez](https://github.com/asapverneri/CS2-ScoutzKnivez)**<br>
  ⭐ 2<br>
  ⏱️ updated 1 year ago<br>
  *This is a legendary gamemode made for CS2. It's in early state, but it should be pretty stable.*

- **[asapverneri/CS2-ChatLogs](https://github.com/asapverneri/CS2-ChatLogs)**<br>
  ⭐ 2<br>
  ⏱️ updated 1 year ago<br>
  *Plugin that store chat messages to MySQL database.*

- **[asapverneri/CS2-StaffList](https://github.com/asapverneri/CS2-StaffList)**<br>
  ⭐ 2<br>
  ⏱️ updated 2 years ago<br>
  *Simple plugin which show online admins in the server.*

- **[Austinbots/cs2-BotsNoKnife](https://github.com/Austinbots/cs2-BotsNoKnife)**<br>
  ⭐ 2<br>
  ⏱️ updated 1 year ago<br>
  *Strips knife from bots.*

- **[schwarper/cs2-anticheat](https://github.com/schwarper/cs2-anticheat)**<br>
  ⭐ 0<br>
  ⏱️ updated 0 seconds ago<br>
  *cs2-anticheat is an anti-cheat system in development for CounterStrike 2.*

- **[sink3x/cs2-server-admin-plugin](https://github.com/sink3x/cs2-server-admin-plugin)**<br>
  ⭐ 0<br>
  ⏱️ updated 1 week ago<br>
  *Companion plugin for a CS2 web admin panel: in-game !menu, sounds, team names and logos, clan tags and map cleanup.*


## SwiftlyS2 Plugins

Check [Forum](https://forum.swiftlys2.net/) for all swiftlys2 plugins.

- **[samyycX/WeaponSkins](https://github.com/samyycX/WeaponSkins)**<br>
  ⭐ 36<br>
  ⏱️ updated 1 day ago<br>
  *A powerful swiftlys2 plugin to change player's weapon skins, knifes and gloves.*

- **[SwiftlyS2-Plugins/AddonsManager](https://github.com/SwiftlyS2-Plugins/AddonsManager)**<br>
  ⭐ 20<br>
  ⏱️ updated 1 week ago<br>
  *This plugin is a port of Source2ZE's MultiAddonManager for SwiftlyS2, allowing you to use multiple workshop addons at once and have clients download them.*

- **[SwiftlyS2-Plugins/MapChooser](https://github.com/SwiftlyS2-Plugins/MapChooser)**<br>
  ⭐ 14<br>
  ⏱️ updated 3 days ago<br>
  *A powerful and customizable map voting system for SwiftlyS2.*

- **[SwiftlyS2-Plugins/Admins](https://github.com/SwiftlyS2-Plugins/Admins)**<br>
  ⭐ 13<br>
  ⏱️ updated 3 days ago<br>
  *Base admin system for your server.*

- **[SwiftlyS2-Plugins/Audio](https://github.com/SwiftlyS2-Plugins/Audio)**<br>
  ⭐ 8<br>
  ⏱️ updated 4 months ago<br>
  *A high performance VoIP audio lib for swiftlys2.*

- **[SwiftlyS2-Plugins/VIPCore](https://github.com/SwiftlyS2-Plugins/VIPCore)**<br>
  ⭐ 8<br>
  ⏱️ updated 1 week ago<br>
  *A comprehensive VIP management system for Counter-Strike 2 servers running SwiftlyS2.*

- **[SwiftlyS2-Plugins/Economy](https://github.com/SwiftlyS2-Plugins/Economy)**<br>
  ⭐ 6<br>
  ⏱️ updated 8 months ago<br>
  *The base economy plugin for your server.*

- **[SwiftlyS2-Plugins/Cookies](https://github.com/SwiftlyS2-Plugins/Cookies)**<br>
  ⭐ 5<br>
  ⏱️ updated 1 week ago<br>
  *Server/Client persistent variables.*

- **[SwiftlyS2-Plugins/MenuFlickeringFix](https://github.com/SwiftlyS2-Plugins/MenuFlickeringFix)**<br>
  ⭐ 4<br>
  ⏱️ updated 1 year ago<br>
  *Fix the flickering menu. Won't have effect during warmup for game reason.*


# Contributors

<table>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/123suarez96"><img src="https://avatars.githubusercontent.com/u/232894409?v=4" width="64" height="64" alt="123suarez96" /></a><br/>
      <a href="https://github.com/123suarez96">@123suarez96</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/1Mack"><img src="https://avatars.githubusercontent.com/u/60943107?v=4" width="64" height="64" alt="1Mack" /></a><br/>
      <a href="https://github.com/1Mack">@1Mack</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/2vg"><img src="https://avatars.githubusercontent.com/u/17700125?v=4" width="64" height="64" alt="2vg" /></a><br/>
      <a href="https://github.com/2vg">@2vg</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/8tnc"><img src="https://avatars.githubusercontent.com/u/78674116?v=4" width="64" height="64" alt="8tnc" /></a><br/>
      <a href="https://github.com/8tnc">@8tnc</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/abnerfs"><img src="https://avatars.githubusercontent.com/u/14078661?v=4" width="64" height="64" alt="abnerfs" /></a><br/>
      <a href="https://github.com/abnerfs">@abnerfs</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/AcaroMan"><img src="https://avatars.githubusercontent.com/u/82720436?v=4" width="64" height="64" alt="AcaroMan" /></a><br/>
      <a href="https://github.com/AcaroMan">@AcaroMan</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/adammyllykoski"><img src="https://avatars.githubusercontent.com/u/150072141?v=4" width="64" height="64" alt="adammyllykoski" /></a><br/>
      <a href="https://github.com/adammyllykoski">@adammyllykoski</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/afikpr123"><img src="https://avatars.githubusercontent.com/u/46192403?v=4" width="64" height="64" alt="afikpr123" /></a><br/>
      <a href="https://github.com/afikpr123">@afikpr123</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/agasking1337"><img src="https://avatars.githubusercontent.com/u/29358982?v=4" width="64" height="64" alt="agasking1337" /></a><br/>
      <a href="https://github.com/agasking1337">@agasking1337</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/akappakappa"><img src="https://avatars.githubusercontent.com/u/20224688?v=4" width="64" height="64" alt="akappakappa" /></a><br/>
      <a href="https://github.com/akappakappa">@akappakappa</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/akxcv"><img src="https://avatars.githubusercontent.com/u/17192674?v=4" width="64" height="64" alt="akxcv" /></a><br/>
      <a href="https://github.com/akxcv">@akxcv</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/algorithm-developer"><img src="https://avatars.githubusercontent.com/u/17625859?v=4" width="64" height="64" alt="algorithm-developer" /></a><br/>
      <a href="https://github.com/algorithm-developer">@algorithm-developer</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/AloneElxy"><img src="https://avatars.githubusercontent.com/u/74292375?v=4" width="64" height="64" alt="AloneElxy" /></a><br/>
      <a href="https://github.com/AloneElxy">@AloneElxy</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/AlphaKeks"><img src="https://avatars.githubusercontent.com/u/85143381?v=4" width="64" height="64" alt="AlphaKeks" /></a><br/>
      <a href="https://github.com/AlphaKeks">@AlphaKeks</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/amitelka"><img src="https://avatars.githubusercontent.com/u/12958411?v=4" width="64" height="64" alt="amitelka" /></a><br/>
      <a href="https://github.com/amitelka">@amitelka</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/AmnesiaLemon"><img src="https://avatars.githubusercontent.com/u/103835174?v=4" width="64" height="64" alt="AmnesiaLemon" /></a><br/>
      <a href="https://github.com/AmnesiaLemon">@AmnesiaLemon</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/AnshumanRohella"><img src="https://avatars.githubusercontent.com/u/6627611?v=4" width="64" height="64" alt="AnshumanRohella" /></a><br/>
      <a href="https://github.com/AnshumanRohella">@AnshumanRohella</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/AntPGN"><img src="https://avatars.githubusercontent.com/u/3760018?v=4" width="64" height="64" alt="AntPGN" /></a><br/>
      <a href="https://github.com/AntPGN">@AntPGN</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Apfelwurm"><img src="https://avatars.githubusercontent.com/u/23552885?v=4" width="64" height="64" alt="Apfelwurm" /></a><br/>
      <a href="https://github.com/Apfelwurm">@Apfelwurm</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ar1a"><img src="https://avatars.githubusercontent.com/u/8436007?v=4" width="64" height="64" alt="ar1a" /></a><br/>
      <a href="https://github.com/ar1a">@ar1a</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/asapverneri"><img src="https://avatars.githubusercontent.com/u/142944488?v=4" width="64" height="64" alt="asapverneri" /></a><br/>
      <a href="https://github.com/asapverneri">@asapverneri</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Austinbots"><img src="https://avatars.githubusercontent.com/u/182579764?v=4" width="64" height="64" alt="Austinbots" /></a><br/>
      <a href="https://github.com/Austinbots">@Austinbots</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ay-gitgud"><img src="https://avatars.githubusercontent.com/u/241843953?v=4" width="64" height="64" alt="ay-gitgud" /></a><br/>
      <a href="https://github.com/ay-gitgud">@ay-gitgud</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/B3none"><img src="https://avatars.githubusercontent.com/u/24966460?v=4" width="64" height="64" alt="B3none" /></a><br/>
      <a href="https://github.com/B3none">@B3none</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/BMathers35"><img src="https://avatars.githubusercontent.com/u/34012215?v=4" width="64" height="64" alt="BMathers35" /></a><br/>
      <a href="https://github.com/BMathers35">@BMathers35</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/BombFoolGranny"><img src="https://avatars.githubusercontent.com/u/32615723?v=4" width="64" height="64" alt="BombFoolGranny" /></a><br/>
      <a href="https://github.com/BombFoolGranny">@BombFoolGranny</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/borzaka"><img src="https://avatars.githubusercontent.com/u/1230402?v=4" width="64" height="64" alt="borzaka" /></a><br/>
      <a href="https://github.com/borzaka">@borzaka</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/brkvlr"><img src="https://avatars.githubusercontent.com/u/50466021?v=4" width="64" height="64" alt="brkvlr" /></a><br/>
      <a href="https://github.com/brkvlr">@brkvlr</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/btnrv"><img src="https://avatars.githubusercontent.com/u/83274417?v=4" width="64" height="64" alt="btnrv" /></a><br/>
      <a href="https://github.com/btnrv">@btnrv</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/busheezy"><img src="https://avatars.githubusercontent.com/u/6915170?v=4" width="64" height="64" alt="busheezy" /></a><br/>
      <a href="https://github.com/busheezy">@busheezy</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/CasaRica"><img src="https://avatars.githubusercontent.com/u/156474752?v=4" width="64" height="64" alt="CasaRica" /></a><br/>
      <a href="https://github.com/CasaRica">@CasaRica</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/CeLicat"><img src="https://avatars.githubusercontent.com/u/19570348?v=4" width="64" height="64" alt="CeLicat" /></a><br/>
      <a href="https://github.com/CeLicat">@CeLicat</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/characharm"><img src="https://avatars.githubusercontent.com/u/123120856?v=4" width="64" height="64" alt="characharm" /></a><br/>
      <a href="https://github.com/characharm">@characharm</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/CharlesBarone"><img src="https://avatars.githubusercontent.com/u/16052438?v=4" width="64" height="64" alt="CharlesBarone" /></a><br/>
      <a href="https://github.com/CharlesBarone">@CharlesBarone</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Cherno-Beliy"><img src="https://avatars.githubusercontent.com/u/143458283?v=4" width="64" height="64" alt="Cherno-Beliy" /></a><br/>
      <a href="https://github.com/Cherno-Beliy">@Cherno-Beliy</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/chycken"><img src="https://avatars.githubusercontent.com/u/25958225?v=4" width="64" height="64" alt="chycken" /></a><br/>
      <a href="https://github.com/chycken">@chycken</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/claude"><img src="https://avatars.githubusercontent.com/u/81847?v=4" width="64" height="64" alt="claude" /></a><br/>
      <a href="https://github.com/claude">@claude</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/CounterStrike2-Plugins-Archive"><img src="https://avatars.githubusercontent.com/u/268485493?v=4" width="64" height="64" alt="CounterStrike2-Plugins-Archive" /></a><br/>
      <a href="https://github.com/CounterStrike2-Plugins-Archive">@CounterStrike2-Plugins-Archive</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/counterstrikesharp-panel"><img src="https://avatars.githubusercontent.com/u/167018976?v=4" width="64" height="64" alt="counterstrikesharp-panel" /></a><br/>
      <a href="https://github.com/counterstrikesharp-panel">@counterstrikesharp-panel</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/crashzk"><img src="https://avatars.githubusercontent.com/u/32937653?v=4" width="64" height="64" alt="crashzk" /></a><br/>
      <a href="https://github.com/crashzk">@crashzk</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/criskkky"><img src="https://avatars.githubusercontent.com/u/36176343?v=4" width="64" height="64" alt="criskkky" /></a><br/>
      <a href="https://github.com/criskkky">@criskkky</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Cruze03"><img src="https://avatars.githubusercontent.com/u/31343375?v=4" width="64" height="64" alt="Cruze03" /></a><br/>
      <a href="https://github.com/Cruze03">@Cruze03</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/daffyyyy"><img src="https://avatars.githubusercontent.com/u/41084667?v=4" width="64" height="64" alt="daffyyyy" /></a><br/>
      <a href="https://github.com/daffyyyy">@daffyyyy</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/darichey"><img src="https://avatars.githubusercontent.com/u/3052455?v=4" width="64" height="64" alt="darichey" /></a><br/>
      <a href="https://github.com/darichey">@darichey</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/darkerz7"><img src="https://avatars.githubusercontent.com/u/25752428?v=4" width="64" height="64" alt="darkerz7" /></a><br/>
      <a href="https://github.com/darkerz7">@darkerz7</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/DEADLYDEVIL-YT"><img src="https://avatars.githubusercontent.com/u/64157024?v=4" width="64" height="64" alt="DEADLYDEVIL-YT" /></a><br/>
      <a href="https://github.com/DEADLYDEVIL-YT">@DEADLYDEVIL-YT</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/DeadSwimek"><img src="https://avatars.githubusercontent.com/u/66752697?v=4" width="64" height="64" alt="DeadSwimek" /></a><br/>
      <a href="https://github.com/DeadSwimek">@DeadSwimek</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/deafps"><img src="https://avatars.githubusercontent.com/u/170218118?v=4" width="64" height="64" alt="deafps" /></a><br/>
      <a href="https://github.com/deafps">@deafps</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/DearCrazyLeaf"><img src="https://avatars.githubusercontent.com/u/124550277?v=4" width="64" height="64" alt="DearCrazyLeaf" /></a><br/>
      <a href="https://github.com/DearCrazyLeaf">@DearCrazyLeaf</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/DeonduPreez"><img src="https://avatars.githubusercontent.com/u/23405975?v=4" width="64" height="64" alt="DeonduPreez" /></a><br/>
      <a href="https://github.com/DeonduPreez">@DeonduPreez</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/apps/dependabot"><img src="https://avatars.githubusercontent.com/in/29110?v=4" width="64" height="64" alt="dependabot[bot]" /></a><br/>
      <a href="https://github.com/apps/dependabot">@dependabot[bot]</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/derkalle4"><img src="https://avatars.githubusercontent.com/u/15657782?v=4" width="64" height="64" alt="derkalle4" /></a><br/>
      <a href="https://github.com/derkalle4">@derkalle4</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/DizzyThermal"><img src="https://avatars.githubusercontent.com/u/730767?v=4" width="64" height="64" alt="DizzyThermal" /></a><br/>
      <a href="https://github.com/DizzyThermal">@DizzyThermal</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Dliix66"><img src="https://avatars.githubusercontent.com/u/2865871?v=4" width="64" height="64" alt="Dliix66" /></a><br/>
      <a href="https://github.com/Dliix66">@Dliix66</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/dollannn"><img src="https://avatars.githubusercontent.com/u/50216897?v=4" width="64" height="64" alt="dollannn" /></a><br/>
      <a href="https://github.com/dollannn">@dollannn</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/dom1torii"><img src="https://avatars.githubusercontent.com/u/84600518?v=4" width="64" height="64" alt="dom1torii" /></a><br/>
      <a href="https://github.com/dom1torii">@dom1torii</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/dr3fty"><img src="https://avatars.githubusercontent.com/u/12001194?v=4" width="64" height="64" alt="dr3fty" /></a><br/>
      <a href="https://github.com/dr3fty">@dr3fty</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Dying-Ducks"><img src="https://avatars.githubusercontent.com/u/109040754?v=4" width="64" height="64" alt="Dying-Ducks" /></a><br/>
      <a href="https://github.com/Dying-Ducks">@Dying-Ducks</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/e-n-v-i"><img src="https://avatars.githubusercontent.com/u/112311421?v=4" width="64" height="64" alt="e-n-v-i" /></a><br/>
      <a href="https://github.com/e-n-v-i">@e-n-v-i</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/e54385991"><img src="https://avatars.githubusercontent.com/u/19951346?v=4" width="64" height="64" alt="e54385991" /></a><br/>
      <a href="https://github.com/e54385991">@e54385991</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/EasterLee"><img src="https://avatars.githubusercontent.com/u/34824423?v=4" width="64" height="64" alt="EasterLee" /></a><br/>
      <a href="https://github.com/EasterLee">@EasterLee</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ehwhattaugonnado"><img src="https://avatars.githubusercontent.com/u/54551926?v=4" width="64" height="64" alt="ehwhattaugonnado" /></a><br/>
      <a href="https://github.com/ehwhattaugonnado">@ehwhattaugonnado</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ELDment"><img src="https://avatars.githubusercontent.com/u/79680886?v=4" width="64" height="64" alt="ELDment" /></a><br/>
      <a href="https://github.com/ELDment">@ELDment</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/emilhaapalainen"><img src="https://avatars.githubusercontent.com/u/47125778?v=4" width="64" height="64" alt="emilhaapalainen" /></a><br/>
      <a href="https://github.com/emilhaapalainen">@emilhaapalainen</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/erikpeik"><img src="https://avatars.githubusercontent.com/u/52178013?v=4" width="64" height="64" alt="erikpeik" /></a><br/>
      <a href="https://github.com/erikpeik">@erikpeik</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/eSTuuu"><img src="https://avatars.githubusercontent.com/u/58341152?v=4" width="64" height="64" alt="eSTuuu" /></a><br/>
      <a href="https://github.com/eSTuuu">@eSTuuu</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/exababy"><img src="https://avatars.githubusercontent.com/u/44870116?v=4" width="64" height="64" alt="exababy" /></a><br/>
      <a href="https://github.com/exababy">@exababy</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/exd02"><img src="https://avatars.githubusercontent.com/u/62575526?v=4" width="64" height="64" alt="exd02" /></a><br/>
      <a href="https://github.com/exd02">@exd02</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/execut1ve"><img src="https://avatars.githubusercontent.com/u/37169808?v=4" width="64" height="64" alt="execut1ve" /></a><br/>
      <a href="https://github.com/execut1ve">@execut1ve</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/exkludera"><img src="https://avatars.githubusercontent.com/u/51145038?v=4" width="64" height="64" alt="exkludera" /></a><br/>
      <a href="https://github.com/exkludera">@exkludera</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/fabiosantoscode"><img src="https://avatars.githubusercontent.com/u/1611595?v=4" width="64" height="64" alt="fabiosantoscode" /></a><br/>
      <a href="https://github.com/fabiosantoscode">@fabiosantoscode</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/FadelAlwan"><img src="https://avatars.githubusercontent.com/u/117265682?v=4" width="64" height="64" alt="FadelAlwan" /></a><br/>
      <a href="https://github.com/FadelAlwan">@FadelAlwan</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Faramour"><img src="https://avatars.githubusercontent.com/u/44729057?v=4" width="64" height="64" alt="Faramour" /></a><br/>
      <a href="https://github.com/Faramour">@Faramour</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Ferks-FK"><img src="https://avatars.githubusercontent.com/u/69549678?v=4" width="64" height="64" alt="Ferks-FK" /></a><br/>
      <a href="https://github.com/Ferks-FK">@Ferks-FK</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/FlowingSPDG"><img src="https://avatars.githubusercontent.com/u/30292185?v=4" width="64" height="64" alt="FlowingSPDG" /></a><br/>
      <a href="https://github.com/FlowingSPDG">@FlowingSPDG</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/fltuna"><img src="https://avatars.githubusercontent.com/u/47306029?v=4" width="64" height="64" alt="fltuna" /></a><br/>
      <a href="https://github.com/fltuna">@fltuna</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/freakexeuLow"><img src="https://avatars.githubusercontent.com/u/32603929?v=4" width="64" height="64" alt="freakexeuLow" /></a><br/>
      <a href="https://github.com/freakexeuLow">@freakexeuLow</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Frozen-H2O"><img src="https://avatars.githubusercontent.com/u/43626458?v=4" width="64" height="64" alt="Frozen-H2O" /></a><br/>
      <a href="https://github.com/Frozen-H2O">@Frozen-H2O</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/fsynr"><img src="https://avatars.githubusercontent.com/u/257867946?v=4" width="64" height="64" alt="fsynr" /></a><br/>
      <a href="https://github.com/fsynr">@fsynr</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/GabrielBarcelos1"><img src="https://avatars.githubusercontent.com/u/57267134?v=4" width="64" height="64" alt="GabrielBarcelos1" /></a><br/>
      <a href="https://github.com/GabrielBarcelos1">@GabrielBarcelos1</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/GabrielBigardi"><img src="https://avatars.githubusercontent.com/u/42253181?v=4" width="64" height="64" alt="GabrielBigardi" /></a><br/>
      <a href="https://github.com/GabrielBigardi">@GabrielBigardi</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/GameChaos"><img src="https://avatars.githubusercontent.com/u/25118806?v=4" width="64" height="64" alt="GameChaos" /></a><br/>
      <a href="https://github.com/GameChaos">@GameChaos</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/GAMMACASE"><img src="https://avatars.githubusercontent.com/u/31375974?v=4" width="64" height="64" alt="GAMMACASE" /></a><br/>
      <a href="https://github.com/GAMMACASE">@GAMMACASE</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Geekvcn"><img src="https://avatars.githubusercontent.com/u/20044214?v=4" width="64" height="64" alt="Geekvcn" /></a><br/>
      <a href="https://github.com/Geekvcn">@Geekvcn</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/geison66"><img src="https://avatars.githubusercontent.com/u/17097504?v=4" width="64" height="64" alt="geison66" /></a><br/>
      <a href="https://github.com/geison66">@geison66</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/GekasD"><img src="https://avatars.githubusercontent.com/u/67982775?v=4" width="64" height="64" alt="GekasD" /></a><br/>
      <a href="https://github.com/GekasD">@GekasD</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/girlglock"><img src="https://avatars.githubusercontent.com/u/178796756?v=4" width="64" height="64" alt="girlglock" /></a><br/>
      <a href="https://github.com/girlglock">@girlglock</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/apps/github-actions"><img src="https://avatars.githubusercontent.com/in/15368?v=4" width="64" height="64" alt="github-actions[bot]" /></a><br/>
      <a href="https://github.com/apps/github-actions">@github-actions[bot]</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Gorakel"><img src="https://avatars.githubusercontent.com/u/29128011?v=4" width="64" height="64" alt="Gorakel" /></a><br/>
      <a href="https://github.com/Gorakel">@Gorakel</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/hardixnph"><img src="https://avatars.githubusercontent.com/u/48999517?v=4" width="64" height="64" alt="hardixnph" /></a><br/>
      <a href="https://github.com/hardixnph">@hardixnph</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/hazmat321"><img src="https://avatars.githubusercontent.com/u/132187457?v=4" width="64" height="64" alt="hazmat321" /></a><br/>
      <a href="https://github.com/hazmat321">@hazmat321</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/HerrMagiic"><img src="https://avatars.githubusercontent.com/u/70164539?v=4" width="64" height="64" alt="HerrMagiic" /></a><br/>
      <a href="https://github.com/HerrMagiic">@HerrMagiic</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Hichatu"><img src="https://avatars.githubusercontent.com/u/47224085?v=4" width="64" height="64" alt="Hichatu" /></a><br/>
      <a href="https://github.com/Hichatu">@Hichatu</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/hlyl"><img src="https://avatars.githubusercontent.com/u/1834987?v=4" width="64" height="64" alt="hlyl" /></a><br/>
      <a href="https://github.com/hlyl">@hlyl</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/hmerrors"><img src="https://avatars.githubusercontent.com/u/97862393?v=4" width="64" height="64" alt="hmerrors" /></a><br/>
      <a href="https://github.com/hmerrors">@hmerrors</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/hobsRKM"><img src="https://avatars.githubusercontent.com/u/11420858?v=4" width="64" height="64" alt="hobsRKM" /></a><br/>
      <a href="https://github.com/hobsRKM">@hobsRKM</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/human416c6578"><img src="https://avatars.githubusercontent.com/u/34635940?v=4" width="64" height="64" alt="human416c6578" /></a><br/>
      <a href="https://github.com/human416c6578">@human416c6578</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/HvH-gg"><img src="https://avatars.githubusercontent.com/u/112752845?v=4" width="64" height="64" alt="HvH-gg" /></a><br/>
      <a href="https://github.com/HvH-gg">@HvH-gg</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/hzqst"><img src="https://avatars.githubusercontent.com/u/12287588?v=4" width="64" height="64" alt="hzqst" /></a><br/>
      <a href="https://github.com/hzqst">@hzqst</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/icyteriyaki"><img src="https://avatars.githubusercontent.com/u/174741064?v=4" width="64" height="64" alt="icyteriyaki" /></a><br/>
      <a href="https://github.com/icyteriyaki">@icyteriyaki</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/idk1703"><img src="https://avatars.githubusercontent.com/u/64554241?v=4" width="64" height="64" alt="idk1703" /></a><br/>
      <a href="https://github.com/idk1703">@idk1703</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Iksix"><img src="https://avatars.githubusercontent.com/u/109164274?v=4" width="64" height="64" alt="Iksix" /></a><br/>
      <a href="https://github.com/Iksix">@Iksix</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ilyhalight"><img src="https://avatars.githubusercontent.com/u/62353659?v=4" width="64" height="64" alt="ilyhalight" /></a><br/>
      <a href="https://github.com/ilyhalight">@ilyhalight</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/im6705"><img src="https://avatars.githubusercontent.com/u/10539689?v=4" width="64" height="64" alt="im6705" /></a><br/>
      <a href="https://github.com/im6705">@im6705</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/imi-tat0r"><img src="https://avatars.githubusercontent.com/u/17706178?v=4" width="64" height="64" alt="imi-tat0r" /></a><br/>
      <a href="https://github.com/imi-tat0r">@imi-tat0r</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/imkaiwhyask"><img src="https://avatars.githubusercontent.com/u/110795442?v=4" width="64" height="64" alt="imkaiwhyask" /></a><br/>
      <a href="https://github.com/imkaiwhyask">@imkaiwhyask</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Interesting-exe"><img src="https://avatars.githubusercontent.com/u/52731127?v=4" width="64" height="64" alt="Interesting-exe" /></a><br/>
      <a href="https://github.com/Interesting-exe">@Interesting-exe</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/invoker420"><img src="https://avatars.githubusercontent.com/u/187507316?v=4" width="64" height="64" alt="invoker420" /></a><br/>
      <a href="https://github.com/invoker420">@invoker420</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ipsvn"><img src="https://avatars.githubusercontent.com/u/80017804?v=4" width="64" height="64" alt="ipsvn" /></a><br/>
      <a href="https://github.com/ipsvn">@ipsvn</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/iWanheda"><img src="https://avatars.githubusercontent.com/u/34320875?v=4" width="64" height="64" alt="iWanheda" /></a><br/>
      <a href="https://github.com/iWanheda">@iWanheda</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Iwhite67"><img src="https://avatars.githubusercontent.com/u/9698964?v=4" width="64" height="64" alt="Iwhite67" /></a><br/>
      <a href="https://github.com/Iwhite67">@Iwhite67</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/iWYO"><img src="https://avatars.githubusercontent.com/u/48887967?v=4" width="64" height="64" alt="iWYO" /></a><br/>
      <a href="https://github.com/iWYO">@iWYO</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/JackJack3231"><img src="https://avatars.githubusercontent.com/u/26525530?v=4" width="64" height="64" alt="JackJack3231" /></a><br/>
      <a href="https://github.com/JackJack3231">@JackJack3231</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/jakkekz"><img src="https://avatars.githubusercontent.com/u/120755650?v=4" width="64" height="64" alt="jakkekz" /></a><br/>
      <a href="https://github.com/jakkekz">@jakkekz</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/JayCroghan"><img src="https://avatars.githubusercontent.com/u/1171148?v=4" width="64" height="64" alt="JayCroghan" /></a><br/>
      <a href="https://github.com/JayCroghan">@JayCroghan</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/JeBenzon"><img src="https://avatars.githubusercontent.com/u/15323233?v=4" width="64" height="64" alt="JeBenzon" /></a><br/>
      <a href="https://github.com/JeBenzon">@JeBenzon</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/jmgraeffe"><img src="https://avatars.githubusercontent.com/u/1965594?v=4" width="64" height="64" alt="jmgraeffe" /></a><br/>
      <a href="https://github.com/jmgraeffe">@jmgraeffe</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/jonahbearde"><img src="https://avatars.githubusercontent.com/u/48353815?v=4" width="64" height="64" alt="jonahbearde" /></a><br/>
      <a href="https://github.com/jonahbearde">@jonahbearde</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/jonassjoh"><img src="https://avatars.githubusercontent.com/u/17067347?v=4" width="64" height="64" alt="jonassjoh" /></a><br/>
      <a href="https://github.com/jonassjoh">@jonassjoh</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Julienhqr"><img src="https://avatars.githubusercontent.com/u/43007197?v=4" width="64" height="64" alt="Julienhqr" /></a><br/>
      <a href="https://github.com/Julienhqr">@Julienhqr</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/jvnipers"><img src="https://avatars.githubusercontent.com/u/96595352?v=4" width="64" height="64" alt="jvnipers" /></a><br/>
      <a href="https://github.com/jvnipers">@jvnipers</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/jwsharpe"><img src="https://avatars.githubusercontent.com/u/23224124?v=4" width="64" height="64" alt="jwsharpe" /></a><br/>
      <a href="https://github.com/jwsharpe">@jwsharpe</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/k0shir0"><img src="https://avatars.githubusercontent.com/u/163620373?v=4" width="64" height="64" alt="k0shir0" /></a><br/>
      <a href="https://github.com/k0shir0">@k0shir0</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/K4ryuu"><img src="https://avatars.githubusercontent.com/u/104531589?v=4" width="64" height="64" alt="K4ryuu" /></a><br/>
      <a href="https://github.com/K4ryuu">@K4ryuu</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Kandru"><img src="https://avatars.githubusercontent.com/u/1965552?v=4" width="64" height="64" alt="Kandru" /></a><br/>
      <a href="https://github.com/Kandru">@Kandru</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Katarina-E"><img src="https://avatars.githubusercontent.com/u/153746513?v=4" width="64" height="64" alt="Katarina-E" /></a><br/>
      <a href="https://github.com/Katarina-E">@Katarina-E</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/KennyZorro"><img src="https://avatars.githubusercontent.com/u/90090231?v=4" width="64" height="64" alt="KennyZorro" /></a><br/>
      <a href="https://github.com/KennyZorro">@KennyZorro</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/KillerRoi"><img src="https://avatars.githubusercontent.com/u/54289708?v=4" width="64" height="64" alt="KillerRoi" /></a><br/>
      <a href="https://github.com/KillerRoi">@KillerRoi</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/KillStr3aK"><img src="https://avatars.githubusercontent.com/u/19509520?v=4" width="64" height="64" alt="KillStr3aK" /></a><br/>
      <a href="https://github.com/KillStr3aK">@KillStr3aK</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/kkeegzz"><img src="https://avatars.githubusercontent.com/u/54126889?v=4" width="64" height="64" alt="kkeegzz" /></a><br/>
      <a href="https://github.com/kkeegzz">@kkeegzz</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/KKNecmi"><img src="https://avatars.githubusercontent.com/u/174268271?v=4" width="64" height="64" alt="KKNecmi" /></a><br/>
      <a href="https://github.com/KKNecmi">@KKNecmi</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/koiie111"><img src="https://avatars.githubusercontent.com/u/20115518?v=4" width="64" height="64" alt="koiie111" /></a><br/>
      <a href="https://github.com/koiie111">@koiie111</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/komashchenko"><img src="https://avatars.githubusercontent.com/u/22940384?v=4" width="64" height="64" alt="komashchenko" /></a><br/>
      <a href="https://github.com/komashchenko">@komashchenko</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/kraigher"><img src="https://avatars.githubusercontent.com/u/4018223?v=4" width="64" height="64" alt="kraigher" /></a><br/>
      <a href="https://github.com/kraigher">@kraigher</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/kralgorkem"><img src="https://avatars.githubusercontent.com/u/61798602?v=4" width="64" height="64" alt="kralgorkem" /></a><br/>
      <a href="https://github.com/kralgorkem">@kralgorkem</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Kroytz"><img src="https://avatars.githubusercontent.com/u/33564475?v=4" width="64" height="64" alt="Kroytz" /></a><br/>
      <a href="https://github.com/Kroytz">@Kroytz</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/kus"><img src="https://avatars.githubusercontent.com/u/571523?v=4" width="64" height="64" alt="kus" /></a><br/>
      <a href="https://github.com/kus">@kus</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Kxnrl"><img src="https://avatars.githubusercontent.com/u/3905845?v=4" width="64" height="64" alt="Kxnrl" /></a><br/>
      <a href="https://github.com/Kxnrl">@Kxnrl</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/KZGlobalTeam"><img src="https://avatars.githubusercontent.com/u/71567872?v=4" width="64" height="64" alt="KZGlobalTeam" /></a><br/>
      <a href="https://github.com/KZGlobalTeam">@KZGlobalTeam</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/laazzee"><img src="https://avatars.githubusercontent.com/u/19431478?v=4" width="64" height="64" alt="laazzee" /></a><br/>
      <a href="https://github.com/laazzee">@laazzee</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Lan2Play"><img src="https://avatars.githubusercontent.com/u/73311398?v=4" width="64" height="64" alt="Lan2Play" /></a><br/>
      <a href="https://github.com/Lan2Play">@Lan2Play</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/lanslide-team"><img src="https://avatars.githubusercontent.com/u/122711296?v=4" width="64" height="64" alt="lanslide-team" /></a><br/>
      <a href="https://github.com/lanslide-team">@lanslide-team</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/LaplaceTor"><img src="https://avatars.githubusercontent.com/u/28674126?v=4" width="64" height="64" alt="LaplaceTor" /></a><br/>
      <a href="https://github.com/LaplaceTor">@LaplaceTor</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/LeqenddC"><img src="https://avatars.githubusercontent.com/u/68579920?v=4" width="64" height="64" alt="LeqenddC" /></a><br/>
      <a href="https://github.com/LeqenddC">@LeqenddC</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Letaryat"><img src="https://avatars.githubusercontent.com/u/69041891?v=4" width="64" height="64" alt="Letaryat" /></a><br/>
      <a href="https://github.com/Letaryat">@Letaryat</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Lukseh"><img src="https://avatars.githubusercontent.com/u/78541969?v=4" width="64" height="64" alt="Lukseh" /></a><br/>
      <a href="https://github.com/Lukseh">@Lukseh</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/M-archand"><img src="https://avatars.githubusercontent.com/u/13469995?v=4" width="64" height="64" alt="M-archand" /></a><br/>
      <a href="https://github.com/M-archand">@M-archand</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/m-arcuri"><img src="https://avatars.githubusercontent.com/u/124469923?v=4" width="64" height="64" alt="m-arcuri" /></a><br/>
      <a href="https://github.com/m-arcuri">@m-arcuri</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/mAa4a97"><img src="https://avatars.githubusercontent.com/u/34139457?v=4" width="64" height="64" alt="mAa4a97" /></a><br/>
      <a href="https://github.com/mAa4a97">@mAa4a97</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/marqdevx"><img src="https://avatars.githubusercontent.com/u/11246294?v=4" width="64" height="64" alt="marqdevx" /></a><br/>
      <a href="https://github.com/marqdevx">@marqdevx</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/MatthewNeko"><img src="https://avatars.githubusercontent.com/u/9393126?v=4" width="64" height="64" alt="MatthewNeko" /></a><br/>
      <a href="https://github.com/MatthewNeko">@MatthewNeko</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/mavproductions"><img src="https://avatars.githubusercontent.com/u/6368850?v=4" width="64" height="64" alt="mavproductions" /></a><br/>
      <a href="https://github.com/mavproductions">@mavproductions</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/maximee-leroyy"><img src="https://avatars.githubusercontent.com/u/19607336?v=4" width="64" height="64" alt="maximee-leroyy" /></a><br/>
      <a href="https://github.com/maximee-leroyy">@maximee-leroyy</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/MD-V"><img src="https://avatars.githubusercontent.com/u/436613?v=4" width="64" height="64" alt="MD-V" /></a><br/>
      <a href="https://github.com/MD-V">@MD-V</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/MerakW"><img src="https://avatars.githubusercontent.com/u/34398344?v=4" width="64" height="64" alt="MerakW" /></a><br/>
      <a href="https://github.com/MerakW">@MerakW</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Mesharsky"><img src="https://avatars.githubusercontent.com/u/30102036?v=4" width="64" height="64" alt="Mesharsky" /></a><br/>
      <a href="https://github.com/Mesharsky">@Mesharsky</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/mfgredo"><img src="https://avatars.githubusercontent.com/u/43547101?v=4" width="64" height="64" alt="mfgredo" /></a><br/>
      <a href="https://github.com/mfgredo">@mfgredo</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/michihupf"><img src="https://avatars.githubusercontent.com/u/26794144?v=4" width="64" height="64" alt="michihupf" /></a><br/>
      <a href="https://github.com/michihupf">@michihupf</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Micka2302"><img src="https://avatars.githubusercontent.com/u/20491694?v=4" width="64" height="64" alt="Micka2302" /></a><br/>
      <a href="https://github.com/Micka2302">@Micka2302</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/mihaigsm2003"><img src="https://avatars.githubusercontent.com/u/51678688?v=4" width="64" height="64" alt="mihaigsm2003" /></a><br/>
      <a href="https://github.com/mihaigsm2003">@mihaigsm2003</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/milkywayfarer"><img src="https://avatars.githubusercontent.com/u/17110335?v=4" width="64" height="64" alt="milkywayfarer" /></a><br/>
      <a href="https://github.com/milkywayfarer">@milkywayfarer</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Misaka17032"><img src="https://avatars.githubusercontent.com/u/40137262?v=4" width="64" height="64" alt="Misaka17032" /></a><br/>
      <a href="https://github.com/Misaka17032">@Misaka17032</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/mister-keno"><img src="https://avatars.githubusercontent.com/u/129111509?v=4" width="64" height="64" alt="mister-keno" /></a><br/>
      <a href="https://github.com/mister-keno">@mister-keno</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/mrc4tt"><img src="https://avatars.githubusercontent.com/u/5301047?v=4" width="64" height="64" alt="mrc4tt" /></a><br/>
      <a href="https://github.com/mrc4tt">@mrc4tt</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/MSWS"><img src="https://avatars.githubusercontent.com/u/14448584?v=4" width="64" height="64" alt="MSWS" /></a><br/>
      <a href="https://github.com/MSWS">@MSWS</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/NaathySz"><img src="https://avatars.githubusercontent.com/u/97997774?v=4" width="64" height="64" alt="NaathySz" /></a><br/>
      <a href="https://github.com/NaathySz">@NaathySz</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Nereziel"><img src="https://avatars.githubusercontent.com/u/8618825?v=4" width="64" height="64" alt="Nereziel" /></a><br/>
      <a href="https://github.com/Nereziel">@Nereziel</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/NeuTroNBZh"><img src="https://avatars.githubusercontent.com/u/135857979?v=4" width="64" height="64" alt="NeuTroNBZh" /></a><br/>
      <a href="https://github.com/NeuTroNBZh">@NeuTroNBZh</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/nicedayzhu"><img src="https://avatars.githubusercontent.com/u/5286862?v=4" width="64" height="64" alt="nicedayzhu" /></a><br/>
      <a href="https://github.com/nicedayzhu">@nicedayzhu</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/NiGHT757"><img src="https://avatars.githubusercontent.com/u/86895149?v=4" width="64" height="64" alt="NiGHT757" /></a><br/>
      <a href="https://github.com/NiGHT757">@NiGHT757</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/NikolaJyun"><img src="https://avatars.githubusercontent.com/u/22559124?v=4" width="64" height="64" alt="NikolaJyun" /></a><br/>
      <a href="https://github.com/NikolaJyun">@NikolaJyun</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/njyeung"><img src="https://avatars.githubusercontent.com/u/125100451?v=4" width="64" height="64" alt="njyeung" /></a><br/>
      <a href="https://github.com/njyeung">@njyeung</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/NockyCZ"><img src="https://avatars.githubusercontent.com/u/63038995?v=4" width="64" height="64" alt="NockyCZ" /></a><br/>
      <a href="https://github.com/NockyCZ">@NockyCZ</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/nokkvireyr"><img src="https://avatars.githubusercontent.com/u/60022455?v=4" width="64" height="64" alt="nokkvireyr" /></a><br/>
      <a href="https://github.com/nokkvireyr">@nokkvireyr</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/notkoen"><img src="https://avatars.githubusercontent.com/u/45914779?v=4" width="64" height="64" alt="notkoen" /></a><br/>
      <a href="https://github.com/notkoen">@notkoen</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/nry1337"><img src="https://avatars.githubusercontent.com/u/40671068?v=4" width="64" height="64" alt="nry1337" /></a><br/>
      <a href="https://github.com/nry1337">@nry1337</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Nubston"><img src="https://avatars.githubusercontent.com/u/15358552?v=4" width="64" height="64" alt="Nubston" /></a><br/>
      <a href="https://github.com/Nubston">@Nubston</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/nuclearsilo583"><img src="https://avatars.githubusercontent.com/u/58926275?v=4" width="64" height="64" alt="nuclearsilo583" /></a><br/>
      <a href="https://github.com/nuclearsilo583">@nuclearsilo583</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/NullVerdict"><img src="https://avatars.githubusercontent.com/u/24498484?v=4" width="64" height="64" alt="NullVerdict" /></a><br/>
      <a href="https://github.com/NullVerdict">@NullVerdict</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/NVedsted"><img src="https://avatars.githubusercontent.com/u/1313288?v=4" width="64" height="64" alt="NVedsted" /></a><br/>
      <a href="https://github.com/NVedsted">@NVedsted</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/nvmxre"><img src="https://avatars.githubusercontent.com/u/44291653?v=4" width="64" height="64" alt="nvmxre" /></a><br/>
      <a href="https://github.com/nvmxre">@nvmxre</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Nyano1337"><img src="https://avatars.githubusercontent.com/u/62145918?v=4" width="64" height="64" alt="Nyano1337" /></a><br/>
      <a href="https://github.com/Nyano1337">@Nyano1337</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/NYSHUN"><img src="https://avatars.githubusercontent.com/u/175985294?v=4" width="64" height="64" alt="NYSHUN" /></a><br/>
      <a href="https://github.com/NYSHUN">@NYSHUN</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/o3LL"><img src="https://avatars.githubusercontent.com/u/16344976?v=4" width="64" height="64" alt="o3LL" /></a><br/>
      <a href="https://github.com/o3LL">@o3LL</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/OniquirAK"><img src="https://avatars.githubusercontent.com/u/84151839?v=4" width="64" height="64" alt="OniquirAK" /></a><br/>
      <a href="https://github.com/OniquirAK">@OniquirAK</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/oqyh"><img src="https://avatars.githubusercontent.com/u/48490385?v=4" width="64" height="64" alt="oqyh" /></a><br/>
      <a href="https://github.com/oqyh">@oqyh</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/originalaidn"><img src="https://avatars.githubusercontent.com/u/45371311?v=4" width="64" height="64" alt="originalaidn" /></a><br/>
      <a href="https://github.com/originalaidn">@originalaidn</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/oscar-wos"><img src="https://avatars.githubusercontent.com/u/29248751?v=4" width="64" height="64" alt="oscar-wos" /></a><br/>
      <a href="https://github.com/oscar-wos">@oscar-wos</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/oylsister"><img src="https://avatars.githubusercontent.com/u/57207701?v=4" width="64" height="64" alt="oylsister" /></a><br/>
      <a href="https://github.com/oylsister">@oylsister</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Oz-Lin"><img src="https://avatars.githubusercontent.com/u/36224575?v=4" width="64" height="64" alt="Oz-Lin" /></a><br/>
      <a href="https://github.com/Oz-Lin">@Oz-Lin</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/pandathebeasty"><img src="https://avatars.githubusercontent.com/u/64710238?v=4" width="64" height="64" alt="pandathebeasty" /></a><br/>
      <a href="https://github.com/pandathebeasty">@pandathebeasty</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/panikajo"><img src="https://avatars.githubusercontent.com/u/4964185?v=4" width="64" height="64" alt="panikajo" /></a><br/>
      <a href="https://github.com/panikajo">@panikajo</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/partiusfabaa"><img src="https://avatars.githubusercontent.com/u/96542489?v=4" width="64" height="64" alt="partiusfabaa" /></a><br/>
      <a href="https://github.com/partiusfabaa">@partiusfabaa</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/pc-dongles"><img src="https://avatars.githubusercontent.com/u/79170834?v=4" width="64" height="64" alt="pc-dongles" /></a><br/>
      <a href="https://github.com/pc-dongles">@pc-dongles</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/PhantomYopta"><img src="https://avatars.githubusercontent.com/u/129750207?v=4" width="64" height="64" alt="PhantomYopta" /></a><br/>
      <a href="https://github.com/PhantomYopta">@PhantomYopta</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Poggicek"><img src="https://avatars.githubusercontent.com/u/45881722?v=4" width="64" height="64" alt="Poggicek" /></a><br/>
      <a href="https://github.com/Poggicek">@Poggicek</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/poggicek"><img src="https://github.com/poggicek.png" width="64" height="64" alt="poggicek" /></a><br/>
      <a href="https://github.com/poggicek">@poggicek</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/prahzera"><img src="https://avatars.githubusercontent.com/u/110359720?v=4" width="64" height="64" alt="prahzera" /></a><br/>
      <a href="https://github.com/prahzera">@prahzera</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Prefix"><img src="https://avatars.githubusercontent.com/u/1775218?v=4" width="64" height="64" alt="Prefix" /></a><br/>
      <a href="https://github.com/Prefix">@Prefix</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/qq410525209"><img src="https://avatars.githubusercontent.com/u/47157419?v=4" width="64" height="64" alt="qq410525209" /></a><br/>
      <a href="https://github.com/qq410525209">@qq410525209</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/qstage"><img src="https://avatars.githubusercontent.com/u/166386817?v=4" width="64" height="64" alt="qstage" /></a><br/>
      <a href="https://github.com/qstage">@qstage</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/quachuoi2"><img src="https://avatars.githubusercontent.com/u/78841468?v=4" width="64" height="64" alt="quachuoi2" /></a><br/>
      <a href="https://github.com/quachuoi2">@quachuoi2</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Quantor97"><img src="https://avatars.githubusercontent.com/u/45876155?v=4" width="64" height="64" alt="Quantor97" /></a><br/>
      <a href="https://github.com/Quantor97">@Quantor97</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/R0mz1k"><img src="https://avatars.githubusercontent.com/u/81678188?v=4" width="64" height="64" alt="R0mz1k" /></a><br/>
      <a href="https://github.com/R0mz1k">@R0mz1k</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/rainstorm-9"><img src="https://avatars.githubusercontent.com/u/181768486?v=4" width="64" height="64" alt="rainstorm-9" /></a><br/>
      <a href="https://github.com/rainstorm-9">@rainstorm-9</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Ravid-A"><img src="https://avatars.githubusercontent.com/u/72127473?v=4" width="64" height="64" alt="Ravid-A" /></a><br/>
      <a href="https://github.com/Ravid-A">@Ravid-A</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/razpbrry"><img src="https://avatars.githubusercontent.com/u/9876747?v=4" width="64" height="64" alt="razpbrry" /></a><br/>
      <a href="https://github.com/razpbrry">@razpbrry</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/rcnoob"><img src="https://avatars.githubusercontent.com/u/139053812?v=4" width="64" height="64" alt="rcnoob" /></a><br/>
      <a href="https://github.com/rcnoob">@rcnoob</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/rcon420"><img src="https://avatars.githubusercontent.com/u/47292145?v=4" width="64" height="64" alt="rcon420" /></a><br/>
      <a href="https://github.com/rcon420">@rcon420</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/rdfortega"><img src="https://avatars.githubusercontent.com/u/19917918?v=4" width="64" height="64" alt="rdfortega" /></a><br/>
      <a href="https://github.com/rdfortega">@rdfortega</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ReneRebsdorf"><img src="https://avatars.githubusercontent.com/u/44376705?v=4" width="64" height="64" alt="ReneRebsdorf" /></a><br/>
      <a href="https://github.com/ReneRebsdorf">@ReneRebsdorf</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Riicesp"><img src="https://avatars.githubusercontent.com/u/171302297?v=4" width="64" height="64" alt="Riicesp" /></a><br/>
      <a href="https://github.com/Riicesp">@Riicesp</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/rislah"><img src="https://avatars.githubusercontent.com/u/12495774?v=4" width="64" height="64" alt="rislah" /></a><br/>
      <a href="https://github.com/rislah">@rislah</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/rodopoulos1"><img src="https://avatars.githubusercontent.com/u/101236856?v=4" width="64" height="64" alt="rodopoulos1" /></a><br/>
      <a href="https://github.com/rodopoulos1">@rodopoulos1</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/roflmuffin"><img src="https://github.com/roflmuffin.png" width="64" height="64" alt="roflmuffin" /></a><br/>
      <a href="https://github.com/roflmuffin">@roflmuffin</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/rogeraabbccdd"><img src="https://avatars.githubusercontent.com/u/15815422?v=4" width="64" height="64" alt="rogeraabbccdd" /></a><br/>
      <a href="https://github.com/rogeraabbccdd">@rogeraabbccdd</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/roxyrekt"><img src="https://avatars.githubusercontent.com/u/87309991?v=4" width="64" height="64" alt="roxyrekt" /></a><br/>
      <a href="https://github.com/roxyrekt">@roxyrekt</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/RoyZ-iwnl"><img src="https://avatars.githubusercontent.com/u/24809366?v=4" width="64" height="64" alt="RoyZ-iwnl" /></a><br/>
      <a href="https://github.com/RoyZ-iwnl">@RoyZ-iwnl</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/rsKliPPy"><img src="https://avatars.githubusercontent.com/u/9073673?v=4" width="64" height="64" alt="rsKliPPy" /></a><br/>
      <a href="https://github.com/rsKliPPy">@rsKliPPy</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Salvatore-Als"><img src="https://avatars.githubusercontent.com/u/58212852?v=4" width="64" height="64" alt="Salvatore-Als" /></a><br/>
      <a href="https://github.com/Salvatore-Als">@Salvatore-Als</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/samayala22"><img src="https://avatars.githubusercontent.com/u/72941105?v=4" width="64" height="64" alt="samayala22" /></a><br/>
      <a href="https://github.com/samayala22">@samayala22</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/samyycX"><img src="https://avatars.githubusercontent.com/u/60744529?v=4" width="64" height="64" alt="samyycX" /></a><br/>
      <a href="https://github.com/samyycX">@samyycX</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/sapsanDev"><img src="https://avatars.githubusercontent.com/u/42625140?v=4" width="64" height="64" alt="sapsanDev" /></a><br/>
      <a href="https://github.com/sapsanDev">@sapsanDev</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/sarim-hk"><img src="https://avatars.githubusercontent.com/u/58489447?v=4" width="64" height="64" alt="sarim-hk" /></a><br/>
      <a href="https://github.com/sarim-hk">@sarim-hk</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Satttoshi"><img src="https://avatars.githubusercontent.com/u/109807794?v=4" width="64" height="64" alt="Satttoshi" /></a><br/>
      <a href="https://github.com/Satttoshi">@Satttoshi</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ScchutzZ"><img src="https://avatars.githubusercontent.com/u/10107035?v=4" width="64" height="64" alt="ScchutzZ" /></a><br/>
      <a href="https://github.com/ScchutzZ">@ScchutzZ</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/schwarper"><img src="https://avatars.githubusercontent.com/u/75811921?v=4" width="64" height="64" alt="schwarper" /></a><br/>
      <a href="https://github.com/schwarper">@schwarper</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/shmitzas"><img src="https://avatars.githubusercontent.com/u/46751666?v=4" width="64" height="64" alt="shmitzas" /></a><br/>
      <a href="https://github.com/shmitzas">@shmitzas</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/shobhit-pathak"><img src="https://avatars.githubusercontent.com/u/140690706?v=4" width="64" height="64" alt="shobhit-pathak" /></a><br/>
      <a href="https://github.com/shobhit-pathak">@shobhit-pathak</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/shuni3360"><img src="https://avatars.githubusercontent.com/u/328813756?v=4" width="64" height="64" alt="shuni3360" /></a><br/>
      <a href="https://github.com/shuni3360">@shuni3360</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/sink3x"><img src="https://avatars.githubusercontent.com/u/69904090?v=4" width="64" height="64" alt="sink3x" /></a><br/>
      <a href="https://github.com/sink3x">@sink3x</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/sirNugg3ts"><img src="https://avatars.githubusercontent.com/u/28486181?v=4" width="64" height="64" alt="sirNugg3ts" /></a><br/>
      <a href="https://github.com/sirNugg3ts">@sirNugg3ts</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/skayee"><img src="https://avatars.githubusercontent.com/u/75901057?v=4" width="64" height="64" alt="skayee" /></a><br/>
      <a href="https://github.com/skayee">@skayee</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/skuzzis"><img src="https://avatars.githubusercontent.com/u/61626661?v=4" width="64" height="64" alt="skuzzis" /></a><br/>
      <a href="https://github.com/skuzzis">@skuzzis</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/SlynxCZ"><img src="https://avatars.githubusercontent.com/u/88426022?v=4" width="64" height="64" alt="SlynxCZ" /></a><br/>
      <a href="https://github.com/SlynxCZ">@SlynxCZ</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/sniperpl"><img src="https://avatars.githubusercontent.com/u/87908771?v=4" width="64" height="64" alt="sniperpl" /></a><br/>
      <a href="https://github.com/sniperpl">@sniperpl</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/snorux"><img src="https://avatars.githubusercontent.com/u/34697265?v=4" width="64" height="64" alt="snorux" /></a><br/>
      <a href="https://github.com/snorux">@snorux</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/snowhp"><img src="https://avatars.githubusercontent.com/u/13867481?v=4" width="64" height="64" alt="snowhp" /></a><br/>
      <a href="https://github.com/snowhp">@snowhp</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Souplax1"><img src="https://avatars.githubusercontent.com/u/54031758?v=4" width="64" height="64" alt="Souplax1" /></a><br/>
      <a href="https://github.com/Souplax1">@Souplax1</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Source2ZE"><img src="https://avatars.githubusercontent.com/u/129127192?v=4" width="64" height="64" alt="Source2ZE" /></a><br/>
      <a href="https://github.com/Source2ZE">@Source2ZE</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/SpawnTerror"><img src="https://avatars.githubusercontent.com/u/39906114?v=4" width="64" height="64" alt="SpawnTerror" /></a><br/>
      <a href="https://github.com/SpawnTerror">@SpawnTerror</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/sphaxa"><img src="https://avatars.githubusercontent.com/u/25424125?v=4" width="64" height="64" alt="sphaxa" /></a><br/>
      <a href="https://github.com/sphaxa">@sphaxa</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ssypchenko"><img src="https://avatars.githubusercontent.com/u/30598453?v=4" width="64" height="64" alt="ssypchenko" /></a><br/>
      <a href="https://github.com/ssypchenko">@ssypchenko</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/st1ng2"><img src="https://avatars.githubusercontent.com/u/41505081?v=4" width="64" height="64" alt="st1ng2" /></a><br/>
      <a href="https://github.com/st1ng2">@st1ng2</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Staaar0"><img src="https://avatars.githubusercontent.com/u/45707960?v=4" width="64" height="64" alt="Staaar0" /></a><br/>
      <a href="https://github.com/Staaar0">@Staaar0</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/stefanx111"><img src="https://avatars.githubusercontent.com/u/60297289?v=4" width="64" height="64" alt="stefanx111" /></a><br/>
      <a href="https://github.com/stefanx111">@stefanx111</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/stevefan1999-personal"><img src="https://avatars.githubusercontent.com/u/29133953?v=4" width="64" height="64" alt="stevefan1999-personal" /></a><br/>
      <a href="https://github.com/stevefan1999-personal">@stevefan1999-personal</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Stimayk"><img src="https://avatars.githubusercontent.com/u/51941742?v=4" width="64" height="64" alt="Stimayk" /></a><br/>
      <a href="https://github.com/Stimayk">@Stimayk</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/STONE-GPY"><img src="https://avatars.githubusercontent.com/u/131870243?v=4" width="64" height="64" alt="STONE-GPY" /></a><br/>
      <a href="https://github.com/STONE-GPY">@STONE-GPY</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/SwiftlyS2-Plugins"><img src="https://avatars.githubusercontent.com/u/237630240?v=4" width="64" height="64" alt="SwiftlyS2-Plugins" /></a><br/>
      <a href="https://github.com/SwiftlyS2-Plugins">@SwiftlyS2-Plugins</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/szogun1910"><img src="https://avatars.githubusercontent.com/u/69729783?v=4" width="64" height="64" alt="szogun1910" /></a><br/>
      <a href="https://github.com/szogun1910">@szogun1910</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/T3Marius"><img src="https://avatars.githubusercontent.com/u/168598779?v=4" width="64" height="64" alt="T3Marius" /></a><br/>
      <a href="https://github.com/T3Marius">@T3Marius</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/T3Marius-hub"><img src="https://avatars.githubusercontent.com/u/173165797?v=4" width="64" height="64" alt="T3Marius-hub" /></a><br/>
      <a href="https://github.com/T3Marius-hub">@T3Marius-hub</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/tatuaua"><img src="https://avatars.githubusercontent.com/u/116430797?v=4" width="64" height="64" alt="tatuaua" /></a><br/>
      <a href="https://github.com/tatuaua">@tatuaua</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/The0mikkel"><img src="https://avatars.githubusercontent.com/u/28625667?v=4" width="64" height="64" alt="The0mikkel" /></a><br/>
      <a href="https://github.com/The0mikkel">@The0mikkel</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/thechrisnixon"><img src="https://avatars.githubusercontent.com/u/672057?v=4" width="64" height="64" alt="thechrisnixon" /></a><br/>
      <a href="https://github.com/thechrisnixon">@thechrisnixon</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/TheR00st3r"><img src="https://avatars.githubusercontent.com/u/1764864?v=4" width="64" height="64" alt="TheR00st3r" /></a><br/>
      <a href="https://github.com/TheR00st3r">@TheR00st3r</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ThunderClapZ"><img src="https://avatars.githubusercontent.com/u/25366481?v=4" width="64" height="64" alt="ThunderClapZ" /></a><br/>
      <a href="https://github.com/ThunderClapZ">@ThunderClapZ</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/TICHOJEBEC-SK"><img src="https://avatars.githubusercontent.com/u/150498032?v=4" width="64" height="64" alt="TICHOJEBEC-SK" /></a><br/>
      <a href="https://github.com/TICHOJEBEC-SK">@TICHOJEBEC-SK</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/tilgep"><img src="https://avatars.githubusercontent.com/u/66904238?v=4" width="64" height="64" alt="tilgep" /></a><br/>
      <a href="https://github.com/tilgep">@tilgep</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/tiltysola"><img src="https://avatars.githubusercontent.com/u/19757919?v=4" width="64" height="64" alt="tiltysola" /></a><br/>
      <a href="https://github.com/tiltysola">@tiltysola</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Truba55"><img src="https://avatars.githubusercontent.com/u/28046241?v=4" width="64" height="64" alt="Truba55" /></a><br/>
      <a href="https://github.com/Truba55">@Truba55</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Tsukasa-Nefren"><img src="https://avatars.githubusercontent.com/u/89155368?v=4" width="64" height="64" alt="Tsukasa-Nefren" /></a><br/>
      <a href="https://github.com/Tsukasa-Nefren">@Tsukasa-Nefren</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ugurhn"><img src="https://avatars.githubusercontent.com/u/105857708?v=4" width="64" height="64" alt="ugurhn" /></a><br/>
      <a href="https://github.com/ugurhn">@ugurhn</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/umbr1X"><img src="https://avatars.githubusercontent.com/u/123817480?v=4" width="64" height="64" alt="umbr1X" /></a><br/>
      <a href="https://github.com/umbr1X">@umbr1X</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/unicbm"><img src="https://avatars.githubusercontent.com/u/136828391?v=4" width="64" height="64" alt="unicbm" /></a><br/>
      <a href="https://github.com/unicbm">@unicbm</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/UpkkXnet"><img src="https://avatars.githubusercontent.com/u/96552017?v=4" width="64" height="64" alt="UpkkXnet" /></a><br/>
      <a href="https://github.com/UpkkXnet">@UpkkXnet</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ValMadBox"><img src="https://avatars.githubusercontent.com/u/195292199?v=4" width="64" height="64" alt="ValMadBox" /></a><br/>
      <a href="https://github.com/ValMadBox">@ValMadBox</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Vauff"><img src="https://avatars.githubusercontent.com/u/6075172?v=4" width="64" height="64" alt="Vauff" /></a><br/>
      <a href="https://github.com/Vauff">@Vauff</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/vulikit"><img src="https://avatars.githubusercontent.com/u/155004361?v=4" width="64" height="64" alt="vulikit" /></a><br/>
      <a href="https://github.com/vulikit">@vulikit</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/weblate"><img src="https://avatars.githubusercontent.com/u/1607653?v=4" width="64" height="64" alt="weblate" /></a><br/>
      <a href="https://github.com/weblate">@weblate</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/webpashtet"><img src="https://avatars.githubusercontent.com/u/44953225?v=4" width="64" height="64" alt="webpashtet" /></a><br/>
      <a href="https://github.com/webpashtet">@webpashtet</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Wend4r"><img src="https://avatars.githubusercontent.com/u/47463683?v=4" width="64" height="64" alt="Wend4r" /></a><br/>
      <a href="https://github.com/Wend4r">@Wend4r</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/wiruwiru"><img src="https://avatars.githubusercontent.com/u/61034981?v=4" width="64" height="64" alt="wiruwiru" /></a><br/>
      <a href="https://github.com/wiruwiru">@wiruwiru</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/wjsrnrgus33"><img src="https://avatars.githubusercontent.com/u/112553297?v=4" width="64" height="64" alt="wjsrnrgus33" /></a><br/>
      <a href="https://github.com/wjsrnrgus33">@wjsrnrgus33</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Wngui"><img src="https://avatars.githubusercontent.com/u/12588316?v=4" width="64" height="64" alt="Wngui" /></a><br/>
      <a href="https://github.com/Wngui">@Wngui</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/XBribo"><img src="https://avatars.githubusercontent.com/u/63445088?v=4" width="64" height="64" alt="XBribo" /></a><br/>
      <a href="https://github.com/XBribo">@XBribo</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/xen-000"><img src="https://avatars.githubusercontent.com/u/24222257?v=4" width="64" height="64" alt="xen-000" /></a><br/>
      <a href="https://github.com/xen-000">@xen-000</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/xiaoyueyoqwq"><img src="https://avatars.githubusercontent.com/u/71379165?v=4" width="64" height="64" alt="xiaoyueyoqwq" /></a><br/>
      <a href="https://github.com/xiaoyueyoqwq">@xiaoyueyoqwq</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/XIU-kr"><img src="https://avatars.githubusercontent.com/u/24391844?v=4" width="64" height="64" alt="XIU-kr" /></a><br/>
      <a href="https://github.com/XIU-kr">@XIU-kr</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/xLeviNx"><img src="https://avatars.githubusercontent.com/u/48465241?v=4" width="64" height="64" alt="xLeviNx" /></a><br/>
      <a href="https://github.com/xLeviNx">@xLeviNx</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/y0ungsm"><img src="https://avatars.githubusercontent.com/u/84058740?v=4" width="64" height="64" alt="y0ungsm" /></a><br/>
      <a href="https://github.com/y0ungsm">@y0ungsm</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Yarukon"><img src="https://avatars.githubusercontent.com/u/61296195?v=4" width="64" height="64" alt="Yarukon" /></a><br/>
      <a href="https://github.com/Yarukon">@Yarukon</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Yeagorn"><img src="https://avatars.githubusercontent.com/u/37038659?v=4" width="64" height="64" alt="Yeagorn" /></a><br/>
      <a href="https://github.com/Yeagorn">@Yeagorn</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ymenn"><img src="https://avatars.githubusercontent.com/u/135514683?v=4" width="64" height="64" alt="ymenn" /></a><br/>
      <a href="https://github.com/ymenn">@ymenn</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/yonilerner"><img src="https://avatars.githubusercontent.com/u/278810?v=4" width="64" height="64" alt="yonilerner" /></a><br/>
      <a href="https://github.com/yonilerner">@yonilerner</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/zakriamansoor47"><img src="https://avatars.githubusercontent.com/u/56433510?v=4" width="64" height="64" alt="zakriamansoor47" /></a><br/>
      <a href="https://github.com/zakriamansoor47">@zakriamansoor47</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/Zeisenx"><img src="https://avatars.githubusercontent.com/u/19303143?v=4" width="64" height="64" alt="Zeisenx" /></a><br/>
      <a href="https://github.com/Zeisenx">@Zeisenx</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/zer0k-z"><img src="https://avatars.githubusercontent.com/u/61156310?v=4" width="64" height="64" alt="zer0k-z" /></a><br/>
      <a href="https://github.com/zer0k-z">@zer0k-z</a>
    </td>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/ZeroCPU"><img src="https://avatars.githubusercontent.com/u/53643730?v=4" width="64" height="64" alt="ZeroCPU" /></a><br/>
      <a href="https://github.com/ZeroCPU">@ZeroCPU</a>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top" width="80">
      <a href="https://github.com/zwolof"><img src="https://avatars.githubusercontent.com/u/21288834?v=4" width="64" height="64" alt="zwolof" /></a><br/>
      <a href="https://github.com/zwolof">@zwolof</a>
    </td>
  </tr>
</table>


# Contributing

## How to add plugins
You only need to add an entry in `manifests/xxx.json` and follow the same format as the other entries.

## Rules
- Plugin must be open source
- Plugin must be functioning
- Website must be non-profit

This project is a free and open-source project. We welcome any community resource websites/repositories/plugins as long as they follow the rules above, regardless of their popularity.\
Please feel free to submit pull requests or issues, we need your contributions!
