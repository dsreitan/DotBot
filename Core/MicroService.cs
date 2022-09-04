using SC2APIProtocol;

namespace Core;

public class MicroService : IMicroService
{
    private readonly IMessageService _messageService;

    public MicroService(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public void AttackMove(Squad squad, Point2D target, bool queue = false)
    {
        _messageService.Action(Ability.ATTACK, target, squad.Units.Select(x => x.Tag), queue);
    }
}

public interface IMicroService
{
    public void AttackMove(Squad squad, Point2D target, bool queue = false);
}