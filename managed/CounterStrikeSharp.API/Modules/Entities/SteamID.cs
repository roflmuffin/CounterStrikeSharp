using System;

namespace CounterStrikeSharp.API.Modules.Entities
{
    public class SteamID : IEquatable<SteamID>
    {
        const long Base = 76561197960265728;
        public ulong SteamId64 { get; set; }

        public SteamID(ulong id) => SteamId64 = id;
        public SteamID(string id) => SteamId64 = id.StartsWith("[") ? ParseId3(id) : ParseId(id);

        public static explicit operator SteamID(ulong u) => new(u);
        public static explicit operator SteamID(string s) => new(s);

        ulong ParseId(string id)
        {
            var parts = id.Split(':');
            if (parts.Length != 3 || !ulong.TryParse(parts[2], out var num)) throw new FormatException();
            return Base + num * 2 + (parts[1] == "1" ? 1UL : 0);
        }

        ulong ParseId3(string id)
        {
            var parts = id.Replace("[", "").Replace("]", "").Split(':');
            if (parts.Length != 3 || !ulong.TryParse(parts[2], out var num)) throw new FormatException();
            return Base + num;
        }

        public string SteamId2
        {
            get => $"STEAM_0:{(SteamId64 - Base) % 2}:{(SteamId64 - Base) / 2}";
            set => SteamId64 = ParseId(value);
        }

        public string SteamId3
        {
            get => $"[U:1:{SteamId64 - Base}]";
            set => SteamId64 = ParseId3(value);
        }
        
        public int SteamId32
        {
            get => (int)(SteamId64 - Base);
            set => SteamId64 = (ulong)value + Base;
        }

        public override string ToString() => $"[SteamID {SteamId64}, {SteamId2}, {SteamId3}]";

        public bool Equals(SteamID? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return SteamId64 == other.SteamId64;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SteamID)obj);
        }

        public override int GetHashCode()
        {
            return SteamId64.GetHashCode();
        }

        public static bool operator ==(SteamID? left, SteamID? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SteamID? left, SteamID? right)
        {
            return !Equals(left, right);
        }
    }
}