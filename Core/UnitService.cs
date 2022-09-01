using Core.Intel;
using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace Core;

public class UnitService : IUnitService
{
    private readonly IIntelService _intelService;
    private readonly IRequestService _requestService;

    public UnitService(IRequestService requestService, IIntelService intelService)
    {
        _requestService = requestService;
        _intelService = intelService;
    }

    public void Train(UnitType unitType)
    {
        if (!UnitTrainedFrom.Producer.TryGetValue(unitType, out var producers))
            throw new NotImplementedException();

        var producer = producers.Single();

        foreach (var unit in _intelService.GetStructures(producer.producerType))
        {
            var command = new ActionRawUnitCommand();
            command.UnitTags.Add(unit.Tag);
            command.AbilityId = (int)producer.ability;

            _requestService.Actions.Add(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }
    }
}

public interface IUnitService
{
    void Train(UnitType unitType);
}