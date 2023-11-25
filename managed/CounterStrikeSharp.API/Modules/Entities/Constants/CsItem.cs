using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CounterStrikeSharp.API.Modules.Entities.Constants
{
    public enum CsItem
    {
        //-----------------------------------------
        //EQUIPMENT
        //-----------------------------------------
        [EnumMember(Value = "item_kevlar")]
        Kevlar = 000,

        [EnumMember(Value = "item_assaultsuit")]
        AssaultSuit = 001,
        KevlarHelmet = AssaultSuit,

        [EnumMember(Value = "weapon_taser")]
        Taser = 002,
        Zeus = Taser,

        [EnumMember(Value = "weapon_snowball")]
        Snowball = 003,

        [EnumMember(Value = "weapon_shield")]
        Shield = 004,

        [EnumMember(Value = "weapon_c4")]
        C4 = 005,
        Bomb = C4,

        [EnumMember(Value = "weapon_healthshot")]
        Healthshot = 006,

        [EnumMember(Value = "weapon_breachcharge")]
        BreachCharge = 007,

        [EnumMember(Value = "weapon_tablet")]
        Tablet = 008,

        [EnumMember(Value = "weapon_bumpmine")]
        Bumpmine = 009,

        //-----------------------------------------
        //GRENADES
        //-----------------------------------------
        [EnumMember(Value = "weapon_smokegrenade")]
        Smoke = 100,
        SmokeGrenade = Smoke,

        [EnumMember(Value = "weapon_flashbang")]
        Flashbang = 101,
        FlashbangGrenade = Flashbang,

        [EnumMember(Value = "weapon_hegrenade")]
        HighExplosive = 102,
        HE = HighExplosive,
        HighExplosiveGrenade = HighExplosive,
        HEGrenade = HighExplosive,

        [EnumMember(Value = "weapon_molotov")]
        Molotov = 103,

        [EnumMember(Value = "weapon_incgrenade")]
        Incendiary = 104,
        IncGrenade = Incendiary,
        IncendiaryGrenade = Incendiary,

        [EnumMember(Value = "weapon_decoy")]
        Decoy = 105,
        DecoyGrenade = Decoy,

        //XRay-Grenade
        [EnumMember(Value = "weapon_tagrenade")]
        TacticalAwareness = 106,
        TAGrenade = TacticalAwareness,
        XRayGrenade = TacticalAwareness,

        //Dangerzone: Better HighExplosive
        [EnumMember(Value = "weapon_frag")]
        Frag = 107,
        FragGrenade = Frag,

        //Dangerzone: Better Molotov
        [EnumMember(Value = "weapon_firebomb")]
        Firebomb = 108,

        //Dangerzone: Decoy but Footsteps instead of gun sounds 
        [EnumMember(Value = "weapon_diversion")]
        Diversion = 109,

        //-----------------------------------------
        //PISTOLS
        //-----------------------------------------
        [EnumMember(Value = "weapon_deagle")]
        Deagle = 200,
        DesertEagle = Deagle,

        [EnumMember(Value = "weapon_glock")]
        Glock = 201,
        Glock18 = Glock,

        [EnumMember(Value = "weapon_usp_silencer")]
        USPS = 202,
        USP = USPS,

        [EnumMember(Value = "weapon_hkp2000")]
        HKP2000 = 203,
        P2000 = HKP2000,
        P2K = HKP2000,

        [EnumMember(Value = "weapon_elite")]
        Elite = 204,
        DualBerettas = Elite,
        Dualies = Elite,

        [EnumMember(Value = "weapon_tec9")]
        Tec9 = 205,

        [EnumMember(Value = "weapon_p250")]
        P250 = 206,

        [EnumMember(Value = "weapon_cz75a")]
        CZ = 207,
        CZ75 = CZ,

        [EnumMember(Value = "weapon_fiveseven")]
        FiveSeven = 208,

        [EnumMember(Value = "weapon_revolver")]
        Revolver = 209,
        R8 = Revolver,

        //-----------------------------------------
        //MID-TIER
        //-----------------------------------------
        [EnumMember(Value = "weapon_mac10")]
        Mac10 = 300,

        [EnumMember(Value = "weapon_mp9")]
        MP9 = 301,

        [EnumMember(Value = "weapon_mp7")]
        MP7 = 302,

        [EnumMember(Value = "weapon_p90")]
        P90 = 303,

        [EnumMember(Value = "weapon_mp5sd")]
        MP5SD = 304,
        MP5 = MP5SD,

        [EnumMember(Value = "weapon_bizon")]
        Bizon = 305,
        PPBizon = Bizon,

        [EnumMember(Value = "weapon_ump45")]
        UMP45 = 306,
        UMP = UMP45,

        [EnumMember(Value = "weapon_xm1014")]
        XM1014 = 307,

        [EnumMember(Value = "weapon_nova")]
        Nova = 308,

        [EnumMember(Value = "weapon_mag7")]
        MAG7 = 309,

        [EnumMember(Value = "weapon_sawedoff")]
        SawedOff = 310,

        [EnumMember(Value = "weapon_m249")]
        M249 = 311,

        [EnumMember(Value = "weapon_negev")]
        Negev = 312,

        //-----------------------------------------
        //RIFLES
        //-----------------------------------------
        [EnumMember(Value = "weapon_ak47")]
        AK47 = 400,

        [EnumMember(Value = "weapon_m4a1_silencer")]
        M4A1S = 401,
        SilencedM4 = M4A1S,

        [EnumMember(Value = "weapon_m4a1")]
        M4A1 = 402,
        M4A4 = M4A1,

        [EnumMember(Value = "weapon_galilar")]
        GalilAR = 403,
        Galil = GalilAR,

        [EnumMember(Value = "weapon_famas")]
        Famas = 404,

        [EnumMember(Value = "weapon_sg556")]
        SG556 = 405,
        SG553 = SG556,
        Krieg = SG556,

        [EnumMember(Value = "weapon_awp")]
        AWP = 406,

        [EnumMember(Value = "weapon_aug")]
        AUG = 407,

        [EnumMember(Value = "weapon_ssg08")]
        SSG08 = 408,
        Scout = SSG08,

        [EnumMember(Value = "weapon_scar20")]
        SCAR20 = 409,
        AutoSniperCT = SCAR20,

        [EnumMember(Value = "weapon_g3sg1")]
        G3SG1 = 410,
        AutoSniperT = G3SG1,

        //-----------------------------------------
        //KNIFE
        //-----------------------------------------
        [EnumMember(Value = "weapon_knife_t")]
        DefaultKnifeT = 500,
        KnifeT = DefaultKnifeT,

        [EnumMember(Value = "weapon_knife")]
        DefaultKnifeCT = 501,
        KnifeCT = DefaultKnifeCT,
        Knife = DefaultKnifeCT,


    }
}
