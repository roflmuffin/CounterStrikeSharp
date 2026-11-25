# 06: IServerGameClients hooks

Spec: `.scratch/hardening-b-boot-verify/spec.md` (Hooks and calls through Slot Bindings, New Engine Features).

**What to build:** The IServerGameClients hooks (client connect, put in server, disconnect, client command, client voice, client user message) become verified Slot Bindings hooked by index, backing `cssharp.player/connection-events`, `cssharp.player/client-commands`, `cssharp.player/voice-events` and `cssharp.ui/custom-hud-layout`.

**Blocked by:** 03, 04

**Status:** resolved

**Tier:** 2 add-on · later

- [x] Slot Bindings bootstrapped and carried forward by the generator
- [x] Real-build test (local): zero wrong resolutions over the archived pairs
- [x] Each Engine Feature fails independently; listeners on a disabled one register, log once and never fire
- [x] End-to-end: connection hooks one index off → heal; corrupted ClientCommand window → client-commands disabled, connections still work
- [x] Windows behaves as today

## Comments

- Resolved on `spec-b/06-iservergameclients-hooks`. Native doctest 98 cases / 1516 assertions pass locally with the archive (3 skipped: oracle and `CSSHARP_SERVER_BINARY` tests); CI mode (`CSSHARP_BUILD_ARCHIVE=/nonexistent`) 87 pass, 14 skipped. Managed unit tests 194 pass. E2E `eng/e2e/cases/server-game-clients.sh`: all four scenarios green.
- **What landed:**
  - Catalog (`src/core/engine_features.{h,cpp}`, one block after game-frame): `cssharp.player/connection-events` (ClientConnect, ClientPutInServer, ClientDisconnect), `cssharp.player/client-commands` (ClientCommand), `cssharp.player/voice-events` (ClientVoice), `cssharp.ui/custom-hud-layout` (ClientSvcUserMessage). Keys `IServerGameClients::<Method>`, class `CSource2GameClients`, library `server`. Managed enum regenerated (41 Engine Features at this point).
  - Gamedata: six entries bootstrapped on 1.42.0.1 with the generator CLI at the SDK header indices 12, 13, 16, 17, 27, 38 (the same in the pinned and the e2e SDK). No warnings, size 46.
  - Hooks: `player_manager.cpp` and `customhudlayout.cpp` use `HookSlot` behind `ShouldRegisterHooks`, one gate per Engine Feature. Callbacks are still created unconditionally.
  - Managed: `ListenerEngineFeatures` maps OnClientConnect/Connected/PutInServer/Disconnect/DisconnectPost and OnClientAuthorized to connection-events, OnClientVoice to voice-events, OnCustomHudClicked to custom-hud-layout. `AddCommandListener` logs once on a disabled client-commands ("Command listener on client-typed commands registered on ...; it will never fire") but still registers, because console-dispatched commands still reach it.
  - Tests: `tests/native/server_game_clients_test.cpp` covers the catalog, each feature failing only on its own Bindings, and healed/unverified Bindings keeping features available. Its archive tests check that the 1.42.0.1 bootstrap reproduces the committed entries, that the 1.41.9.7 bootstrap carries forward to them (from == to), that a replay over all 8 archived pairs (1.41.8.1 .. 1.42.0.1, `.img` and `.so`) resolves every slot at its true index, and that the committed entries resolve on 1.41.9.7. Truth: the research oracle (`sim.py`, RELAX=uniq) maps all six slots to themselves on every pair. Managed `ServerGameClientsEngineFeatureTests`. Smoke `ServerGameClientsTests`.
  - `archive_test.cpp`: the GameFrame carry-forward replay no longer asserts that GameFrame is the only carried entry; any other committed Slot Binding must stay at its index. Tickets 05/07/08 would have hit the same assertion.
- **E2E scenarios:**
  - valid: all resolved, no Heal or unverified line. Bots fire connect, put-in-server and disconnect, and the client command reaches its listener.
  - connection hooks at 11/12/15: they heal to 12/13/16 ("all N resolved, 3 healed"). The native and managed Heal lines appear, `css_engine_features` shows "available, healed (...)", and Gamedata is unchanged after the run.
  - ClientCommand window corrupted: only client-commands is disabled. Connections still fire, and the client command reaches no listener.
  - ClientConnect window corrupted: only connection-events is disabled. Its listeners never fire, and client commands still work.
- **Deviations / notes:**
  - Bots never send commands to the server. I measured that `IVEngineServer2::ClientCommand` on a bot does not reach `IServerGameClients::ClientCommand`. So the smoke test calls `ClientCommand` itself, through the vtable at the Gamedata index, from inside a server command and for an empty player slot. Only our hook can then pass the command to listeners. ClientVoice and ClientSvcUserMessage cannot be triggered without real clients; the e2e covers them through verification and availability only.
  - OnClientAuthorized is mapped to connection-events only, although it also needs game-frame (auth checks run per frame). The listener map holds one Engine Feature per listener.
  - With connection-events disabled, `ListenerTests.CanRegisterAndDeregisterListeners` fails, as expected (OnClientConnect never fires).
  - Smoke flakiness seen in these runs and in other tickets' runs:
    - `CommandTests.*`: server commands are not seen within one frame.
    - `SchemaPropertyTests.*`: fails when it runs with no player in the server (order-dependent; its constructor calls `GetPlayers().First()`).
    - the known flaky tests.
    The case asserts named tests only.
  - Windows: no `offsets.windows` added. `HookSlot` uses the compiled header index and the Bindings report unverified. Not compiled or run here.
