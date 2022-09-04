namespace Core.Extensions;

public static class UnitTypeExtensions
{
    public static bool Is(this uint n, UnitType e) => n == (uint) e;
    public static bool IsWorker(this uint u) => (UnitType)u is UnitType.TERRAN_SCV or UnitType.PROTOSS_PROBE or UnitType.ZERG_DRONE;

    public static bool IsMainBuilding(this uint unitType) =>
        (UnitType) unitType is UnitType.PROTOSS_NEXUS or
        UnitType.ZERG_HATCHERY or UnitType.ZERG_LAIR or UnitType.ZERG_HIVE or
        UnitType.TERRAN_COMMANDCENTER or UnitType.TERRAN_ORBITALCOMMAND or UnitType.TERRAN_PLANETARYFORTRESS or
        UnitType.TERRAN_COMMANDCENTERFLYING or UnitType.TERRAN_ORBITALCOMMANDFLYING;

    /// <summary>
    ///     Minerals have different IDs depending on the game map (due to theme)
    /// </summary>
    /// <param name="id">UnitType ID as defined by Blizzard</param>
    /// <returns>Returns true if the ID is of the type mineral field.</returns>
    public static bool IsMineralField(this uint id) => id is 146 or 147 or 341 or 483 or 885 or 884 or 665 or 666;

    /// <summary>
    ///     All IDs between 362 and 377 is (path) blocking and destructible.
    ///     IDs ranging from 472 - 474 is (placement) blocking and destructible.
    ///     IDs ranging from 623 - 658 is (placement) blocking and destructible.
    /// </summary>
    /// <param name="id">UnitType ID as defined by Blizzard</param>
    /// <returns>Returns true if the ID is of the type destructible and blocks pathing.</returns>
    public static bool IsDestructible(this uint id) => id is > 361 and < 378 or >= 472 and <= 474 or >= 623 and <= 658;
}