using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Entities
{
    public class SteamID : IEquatable<SteamID>
    {
        const long Base = 76561197960265728;
        public ulong SteamId64 { get; set; }

        public SteamID(ulong id) => SteamId64 = id >= Base ? id : id + Base;
        public SteamID(string id) : this(id.StartsWith('[') ? ParseId3(id) : ParseId(id)) { }

        public static explicit operator SteamID(ulong u) => new(u);
        public static explicit operator SteamID(string s) => new(s);

        static ulong ParseId(string id)
        {
            var parts = id.Split(':');
            if (parts.Length != 3 || !ulong.TryParse(parts[2], out var num)) throw new FormatException();
            return Base + (num * 2) + (parts[1] == "1" ? 1UL : 0);
        }

        static ulong ParseId3(string id)
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
            get => $"[{EnumUtils.GetEnumMemberAttributeValue(AccountType)}:{(int)AccountUniverse}:{SteamId64 - Base}]";
            set => SteamId64 = ParseId3(value);
        }

        public int SteamId32
        {
            get => (int)(SteamId64 - Base);
            set => SteamId64 = (ulong)value + Base;
        }

        public int AccountId => (int)(SteamId64 & 0xFFFFFFFF);

        public SteamAccountInstance AccountInstance =>
            (SteamAccountInstance)((SteamId64 >> 32) & 0xFFFFF);

        public SteamAccountType AccountType =>
            (SteamAccountType)((SteamId64 >> 52) & 0xF);

        public SteamAccountUniverse AccountUniverse =>
            (SteamAccountUniverse)((SteamId64 >> 56) & 0xF);

        public bool IsValid()
        {
            if (AccountUniverse == SteamAccountUniverse.Unspecified
                || AccountType == SteamAccountType.Invalid
                || AccountInstance == SteamAccountInstance.Invalid)
                return false;
            if (AccountType == SteamAccountType.Individual
                && (AccountId == 0 || AccountInstance != SteamAccountInstance.Desktop))
                return false;
            if (AccountType == SteamAccountType.Clan
                && (AccountId == 0 || AccountInstance != SteamAccountInstance.All))
                return false;
            if (AccountType == SteamAccountType.GameServer && AccountId == 0)
                return false;
            return true;
        }

        public override string ToString() => $"[SteamID {SteamId64}, {SteamId2}, {SteamId3}]";

        public Uri ToCommunityUrl()
        {
            return AccountType switch
            {
                SteamAccountType.Individual => new Uri("https://steamcommunity.com/profiles/" + SteamId64),
                SteamAccountType.Clan => new Uri("https://steamcommunity.com/gid/" + SteamId64),
                _ => new Uri(string.Empty),
            };
        }

        public bool Equals(SteamID? other)
        {
            return other != null && SteamId64 == other.SteamId64;
        }

        public override bool Equals(object? obj)
        {
            if (obj?.GetType() != this.GetType()) return false;
            return Equals((SteamID)obj);
        }

        public static bool TryParse(string s, out SteamID? steamId)
        {
            try
            {
                if (ulong.TryParse(s, out var steamid64))
                {
                    steamId = new SteamID(steamid64);
                    return true;
                }

                steamId = new SteamID(s);
                return true;
            }
            catch
            {
                steamId = null;
                return false;
            }
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