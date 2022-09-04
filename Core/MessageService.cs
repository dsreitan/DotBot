using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace Core;

internal class MessageService : IMessageService
{
    public GameConnection Connection { get; set; } = new();
    private List<Action> Actions { get; } = new();
    public List<DebugCommand> Debugs { get; } = new();


    public void Action(Ability ability, Point2D target, IEnumerable<ulong> unitTags, bool queue = false)
    {
        var command = new ActionRawUnitCommand
        {
            AbilityId = (int)Ability.ATTACK,
            TargetWorldSpacePos = target,
            QueueCommand = queue,
            UnitTags = { unitTags }
        };

        Actions.Add(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
    }

    public void Action(Ability ability, ulong target, IEnumerable<ulong> unitTags, bool queue = false)
    {
        var command = new ActionRawUnitCommand
        {
            AbilityId = (int)Ability.ATTACK,
            TargetUnitTag = target,
            QueueCommand = queue,
            UnitTags = { unitTags }
        };

        Actions.Add(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
    }

    public void SetConnection(GameConnection connection)
    {
        Connection = connection;
    }

    public async Task OnFrame()
    {
        await Connection.ActionRequest(Actions);
        await Connection.DebugRequest(Debugs);
        Actions.Clear();
        Debugs.Clear();
    }
}

public interface IMessageService
{
    public void Action(Ability ability, Point2D target, IEnumerable<ulong> unitTags, bool queue = false);
    public void Action(Ability ability, ulong target, IEnumerable<ulong> unitTags, bool queue = false);
    public void SetConnection(GameConnection connection);
    public Task OnFrame();
}