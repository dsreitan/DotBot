using SC2APIProtocol;
using SC2ClientApi;
using Action = SC2APIProtocol.Action;

namespace Core;

internal class RequestService : IRequestService
{
    public GameConnection Connection { get; set; } = new();
    public List<Action> Actions { get; } = new();
    public List<DebugCommand> Debugs { get; } = new();


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

public interface IRequestService
{
    public List<Action> Actions { get; }
    public List<DebugCommand> Debugs { get; }
    public void SetConnection(GameConnection connection);
    public Task OnFrame();
}