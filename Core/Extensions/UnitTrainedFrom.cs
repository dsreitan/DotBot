namespace Core.Extensions;

public static class UnitTrainedFrom
{
    public static Dictionary<UnitType, List<(UnitType producerType, Ability ability)>> Producer = new()
    {
        {
            UnitType.TERRAN_SCV, new List<(UnitType, Ability)> { (UnitType.TERRAN_COMMANDCENTER, Ability.TRAIN_SCV) }
        },
        { UnitType.TERRAN_COMMANDCENTER, new List<(UnitType, Ability)> { (UnitType.TERRAN_SCV, Ability.BUILD_COMMANDCENTER) } }
    };
}