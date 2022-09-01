using SC2APIProtocol;
using Action = SC2APIProtocol.Action;

namespace Core;

public class MicroService : IMicroService
{
    private readonly IRequestService _requestService;

    public MicroService(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public void AttackMove(Squad squad, Point2D? target, bool queue = false)
    {
        foreach (var unit in squad.Units)
        {
            var command = new ActionRawUnitCommand
            {
                AbilityId = (int)Ability.ATTACK,
                TargetWorldSpacePos = target,
                QueueCommand = queue,
                UnitTags = { unit.Tag }
            };

            _requestService.Actions.Add(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }
    }
}

public interface IMicroService
{
    public void AttackMove(Squad squad, Point2D? target, bool queue = false);
}