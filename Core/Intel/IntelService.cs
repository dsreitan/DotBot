using SC2APIProtocol;
using SC2ClientApi;
using Attribute = SC2APIProtocol.Attribute;

namespace Core.Intel;

public class IntelService : IIntelService
{
    private readonly Dictionary<uint, AbilityData> abilityDictionary = new();
    private readonly Dictionary<uint, BuffData> buffDictionary = new();
    private readonly Dictionary<uint, UnitTypeData> unitTypeDictionary = new();
    private readonly Dictionary<uint, UpgradeData> upgradeDictionary = new();

    public Dictionary<ulong, IntelUnit> Workers = new(), Structures = new(), Units = new();

    public List<IntelColony> Colonies { get; set; } = new();

    //TODO: remove or internal
    public Observation Observation { get; set; } = new();

    public List<IntelUnit> GetStructures(UnitType unitType)
    {
        return Structures.Values.Where(s => s.UnitType.Is(unitType)).ToList();
    }

    public List<IntelUnit> GetWorkers()
    {
        return Workers.Values.ToList();
    }

    public List<IntelColony> EnemyColonies { get; set; } = new();

    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null)
    {
        foreach (var ability in responseData.Abilities)
            abilityDictionary.Add(ability.AbilityId, ability);
        foreach (var buff in responseData.Buffs)
            buffDictionary.Add(buff.BuffId, buff);
        foreach (var unitType in responseData.Units)
            unitTypeDictionary.Add(unitType.UnitId, unitType);
        foreach (var upgrade in responseData.Upgrades)
            upgradeDictionary.Add(upgrade.UpgradeId, upgrade);

        if (gameInfo != null)
            EnemyColonies.Add(new IntelColony { Point = gameInfo.StartRaw.StartLocations.Last() });

        OnFrame(firstObservation);
    }

    public virtual void OnFrame(ResponseObservation observation)
    {
        Observation = observation.Observation;

        foreach (var unit in observation.Observation.RawData.Units)
            switch (unit.Alliance)
            {
                case Alliance.Self:
                    AddUnit(unit);
                    break;
                case Alliance.Enemy:
                    break;
                case Alliance.Neutral:
                    break;
                case Alliance.Ally:
                default:
                    throw new NotImplementedException();
            }
    }

    private void AddUnit(Unit unit)
    {
        if (unit.UnitType.IsWorker())
        {
            if (Workers.ContainsKey(unit.Tag))
                Workers[unit.Tag].Data = unit;
            else
                Workers.Add(unit.Tag, new IntelUnit(unit));
        }
        else if (IsStructure(unit.UnitType))
        {
            if (Structures.ContainsKey(unit.Tag))
                Structures[unit.Tag].Data = unit;
            else
                Structures.Add(unit.Tag, new IntelUnit(unit));
        }
    }

    private bool IsStructure(uint unitType)
    {
        return unitTypeDictionary.TryGetValue(unitType, out var value) && value.Attributes.Contains(Attribute.Structure);
    }
}

public interface IIntelService
{
    public Observation Observation { get; set; }
    // Structures method take type, return list unit

    public List<IntelColony> EnemyColonies { get; set; }

    public List<IntelUnit> GetStructures(UnitType unitType);
    public List<IntelUnit> GetWorkers();
    public void OnStart(ResponseObservation firstObservation, ResponseData? responseData = null, ResponseGameInfo? gameInfo = null);
    public void OnFrame(ResponseObservation observation);
}