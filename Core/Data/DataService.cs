using SC2APIProtocol;
using Attribute = SC2APIProtocol.Attribute;

namespace Core.Data;

public class DataService : IDataService
{
    private readonly Dictionary<uint, AbilityData> abilityDictionary = new();
    private readonly Dictionary<uint, BuffData> buffDictionary = new();
    private readonly Dictionary<uint, UnitTypeData> unitTypeDictionary = new();
    private readonly Dictionary<uint, UpgradeData> upgradeDictionary = new();

    public void OnStart(ResponseObservation obs, ResponseData? data, ResponseGameInfo? gameInfo)
    {
        if (data == null) throw new Exception("Invalid data");
        
        foreach (var ability in data.Abilities)
            abilityDictionary.Add(ability.AbilityId, ability);
        foreach (var buff in data.Buffs)
            buffDictionary.Add(buff.BuffId, buff);
        foreach (var unitType in data.Units)
            unitTypeDictionary.Add(unitType.UnitId, unitType);
        foreach (var upgrade in data.Upgrades)
            upgradeDictionary.Add(upgrade.UpgradeId, upgrade);
    }
    
    public bool IsStructure(uint unitType)
    {
        return unitTypeDictionary.TryGetValue(unitType, out var value) && value.Attributes.Contains(Attribute.Structure);
    }
}

public interface IDataService
{
    public void OnStart(ResponseObservation obs, ResponseData? data, ResponseGameInfo? gameInfo);
    public bool IsStructure(uint unitType);
}