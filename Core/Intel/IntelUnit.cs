using Core.Extensions;
using Core.Model;
using SC2APIProtocol;

namespace Core.Intel;

public class IntelUnit : IUnit
{
    public Unit Data;

    public IntelUnit(Unit unit)
    {
        Data = unit;
    }

    public ulong AddOnTag => Data.AddOnTag;

    public Alliance Alliance => Data.Alliance;

    public int AssignedHarvesters
    {
        get => Data.AssignedHarvesters;
        set => Data.AssignedHarvesters = value;
    }

    public ICollection<uint> BuffIds => Data.BuffIds;

    public float BuildProgress => Data.BuildProgress;

    public int CargoSpaceMax => Data.CargoSpaceMax;

    public int CargoSpaceTaken => Data.CargoSpaceTaken;

    public CloakState Cloak => Data.Cloak;

    public float DetectRange => Data.DetectRange;

    public float Energy => Data.Energy;

    public ulong EngagedTargetTag => Data.EngagedTargetTag;

    public float Facing => Data.Facing;

    public float Health => Data.Health;

    public float HealthMax => Data.HealthMax;

    public int IdealHarvesters => Data.IdealHarvesters;

    public bool IsBlip => Data.IsBlip;

    public bool IsBurrowed => Data.IsBurrowed;

    public bool IsFlying => Data.IsFlying;

    public bool IsOnScreen => Data.IsOnScreen;

    public bool IsPowered => Data.IsPowered;

    public bool IsSelected => Data.IsSelected;

    public int MineralContents => Data.MineralContents;

    public ICollection<UnitOrder> Orders => Data.Orders;

    public int Owner => Data.Owner;
    public ICollection<PassengerUnit> Passengers => Data.Passengers;

    public Point Pos => Data.Pos;

    public float RadarRange => Data.RadarRange;

    public float Radius => Data.Radius;

    public float Shield => Data.Shield;

    public ulong Tag => Data.Tag;

    public uint UnitType => Data.UnitType;

    public int VespeneContents => Data.VespeneContents;

    public float WeaponCooldown => Data.WeaponCooldown;

    public Point2D Point => Pos.ConvertTo2D();

    public override bool Equals(object obj)
    {
        var unit = obj as IUnit;
        if (unit == null) return false;
        return unit.Tag == Tag;
    }

    public override int GetHashCode()
    {
        return (int)Tag;
    }
}