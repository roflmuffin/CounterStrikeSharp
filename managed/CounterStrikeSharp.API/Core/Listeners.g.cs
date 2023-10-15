
using System;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Core
{
    public partial class Listeners {
        
        [ListenerName("OnTick")]
        public delegate void OnTick();

        [ListenerName("OnMapStart")]
        public delegate void OnMapStart(string mapName);

        [ListenerName("OnMapEnd")]
        public delegate void OnMapEnd();

        [ListenerName("OnEntitySpawned")]
        public delegate void OnEntitySpawned(IntPtr entity);

        [ListenerName("OnClientConnect")]
        public delegate void OnClientConnect(int index, string name, string ipAddress);

        [ListenerName("OnClientConnected")]
        public delegate void OnClientConnected(int index);

        [ListenerName("OnClientPutInServer")]
        public delegate void OnClientPutInServer(int index);

        [ListenerName("OnClientDisconnect")]
        public delegate void OnClientDisconnect(int index);

        [ListenerName("OnClientDisconnectPost")]
        public delegate void OnClientDisconnectPost(int index);

        [ListenerName("OnClientAuthorized")]
        public delegate void OnClientAuthorized(int index, [CastFrom(typeof(ulong))]SteamID steamId);
    }
}
