
using System;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Core
{
    public partial class Listeners {
        
        [ListenerName("OnEntitySpawned")]
        public delegate void OnEntitySpawned(IntPtr entity);

        [ListenerName("OnEntityCreated")]
        public delegate void OnEntityCreated(IntPtr entity);

        [ListenerName("OnEntityDeleted")]
        public delegate void OnEntityDeleted(IntPtr entity);

        [ListenerName("OnEntityParentChanged")]
        public delegate void OnEntityParentChanged(IntPtr entity, IntPtr newParent);

        [ListenerName("OnTick")]
        public delegate void OnTick();

        [ListenerName("OnMapStart")]
        public delegate void OnMapStart(string mapName);

        [ListenerName("OnMapEnd")]
        public delegate void OnMapEnd();

        [ListenerName("OnClientConnect")]
        public delegate void OnClientConnect(int playerSlot, string name, string ipAddress);

        [ListenerName("OnClientConnected")]
        public delegate void OnClientConnected(int playerSlot);

        [ListenerName("OnClientPutInServer")]
        public delegate void OnClientPutInServer(int playerSlot);

        [ListenerName("OnClientDisconnect")]
        public delegate void OnClientDisconnect(int playerSlot);

        [ListenerName("OnClientDisconnectPost")]
        public delegate void OnClientDisconnectPost(int playerSlot);

        [ListenerName("OnClientAuthorized")]
        public delegate void OnClientAuthorized(int playerSlot, [CastFrom(typeof(ulong))]SteamID steamId);
    }
}
