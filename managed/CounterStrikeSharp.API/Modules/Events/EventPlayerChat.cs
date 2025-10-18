using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Events
{
    [EventName("player_chat")]
    public class EventPlayerChat : GameEvent
    {
        public EventPlayerChat(IntPtr pointer) : base(pointer){}
        public EventPlayerChat(bool force) : base("player_chat", force){}

        /// <summary>
        /// If this chat message was sent to team only
        /// </summary>
        public bool Teamonly
        {
            get => Get<bool>("teamonly");
            set => Set<bool>("teamonly", value);
        }

        /// <summary>
        /// The user ID of the player who sent the chat message
        /// </summary>
        public int Userid
        {
            get => Get<int>("userid");
            set => Set<int>("userid", value);
        }

        /// <summary>
        /// The text content of the chat message
        /// </summary>
        public string Text
        {
            get => Get<string>("text");
            set => Set<string>("text", value);
        }
    }
}