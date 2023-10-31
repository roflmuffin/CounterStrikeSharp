
using System;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Core
{
    public class Listeners {
        
        /// <summary>
        /// Called when an entity is spawned.
        /// </summary>
        /// <param name="entity">The spawned entity.</param>
        [ListenerName("OnEntitySpawned")]
        public delegate void OnEntitySpawned(CEntityInstance entity);

        /// <summary>
        /// Called when an entity is created.
        /// </summary>
        /// <param name="entity">The created entity.</param>
        [ListenerName("OnEntityCreated")]
        public delegate void OnEntityCreated(CEntityInstance entity);

        /// <summary>
        /// Called when an entity is deleted.
        /// </summary>
        /// <param name="entity">The deleted entity.</param>
        [ListenerName("OnEntityDeleted")]
        public delegate void OnEntityDeleted(CEntityInstance entity);

        /// <summary>
        /// Called when an entity's parent is changed.
        /// </summary>
        /// <param name="entity">The entity whose parent was changed.</param>
        /// <param name="newParent">The new parent entity.</param>
        [ListenerName("OnEntityParentChanged")]
        public delegate void OnEntityParentChanged(CEntityInstance entity, CEntityInstance newParent);

        /// <summary>
        /// Called on every server tick (64 per second).
        /// This handler should avoid containing expensive operations.
        /// </summary>
        [ListenerName("OnTick")]
        public delegate void OnTick();

        /// <summary>
        /// Called when a new map is loaded.
        /// </summary>
        /// <param name="mapName">The name of the map that was started.</param>
        [ListenerName("OnMapStart")]
        public delegate void OnMapStart(string mapName);

        /// <summary>
        /// Called when the current map is about to end.
        /// </summary>
        [ListenerName("OnMapEnd")]
        public delegate void OnMapEnd();

        /// <summary>
        /// Called when a client connects to the server.
        /// </summary>
        /// <param name="playerSlot">The player slot of the connected client.</param>
        /// <param name="name">The name of the connected client.</param>
        /// <param name="ipAddress">The IP address of the connected client.</param>
        [ListenerName("OnClientConnect")]
        public delegate void OnClientConnect(int playerSlot, string name, string ipAddress);

        /// <summary>
        /// Called when a client connects to the server.
        /// </summary>
        /// <param name="playerSlot">The player slot of the connected client.</param>
        [ListenerName("OnClientConnected")]
        public delegate void OnClientConnected(int playerSlot);

        /// <summary>
        /// Called when a client is entering the game.
        /// </summary>
        /// <param name="playerSlot">The player slot of the client.</param>
        [ListenerName("OnClientPutInServer")]
        public delegate void OnClientPutInServer(int playerSlot);

        /// <summary>
        /// Called when a client disconnects from the server.
        /// </summary>
        /// <param name="playerSlot">The player slot of the disconnected client.</param>
        [ListenerName("OnClientDisconnect")]
        public delegate void OnClientDisconnect(int playerSlot);

        /// <summary>
        /// Called after a client has disconnected from the server.
        /// </summary>
        /// <param name="playerSlot">The player slot of the disconnected client.</param>
        [ListenerName("OnClientDisconnectPost")]
        public delegate void OnClientDisconnectPost(int playerSlot);

        /// <summary>
        /// Called when a client has been authorized by Steam.
        /// </summary>
        /// <param name="playerSlot">The player slot of the authorized client.</param>
        /// <param name="steamId">The Steam ID of the authorized client.</param>
        [ListenerName("OnClientAuthorized")]
        public delegate void OnClientAuthorized(int playerSlot, [CastFrom(typeof(ulong))]SteamID steamId);
    }
}
