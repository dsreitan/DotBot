using Core.Extensions;
using Core.Intel;
using SC2ClientApi;

namespace Core;

public class UnitService : IUnitService
{
    private readonly IIntelService _intelService;
    private readonly IMessageService _messageService;

    public UnitService(IMessageService messageService, IIntelService intelService)
    {
        _messageService = messageService;
        _intelService = intelService;
    }

    public void Train(UnitType unitType)
    {
        if (!UnitTrainedFrom.Producer.TryGetValue(unitType, out var producers))
            throw new NotImplementedException();

        var producer = producers.Single();

        _messageService.Action(producer.ability, 0, _intelService.GetStructures(producer.producerType).Select(x => x.Tag));
    }
}

public interface IUnitService
{
    void Train(UnitType unitType);
}