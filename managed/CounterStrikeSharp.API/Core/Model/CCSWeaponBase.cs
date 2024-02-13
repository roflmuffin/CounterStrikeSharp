namespace CounterStrikeSharp.API.Core;

public partial class CCSWeaponBase
{
    public new CCSWeaponBaseVData? VData => GetVData<CCSWeaponBaseVData>();
}