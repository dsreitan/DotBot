using Core.Intel;
using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Bot;

public abstract class BaseBot
{
    protected readonly IIntelService Intel;
    protected readonly IMicroService MicroService;
    protected readonly IRequestService Requester;

    public BaseBot(IServiceProvider services, Race race)
    {
        Intel = services.GetRequiredService<IIntelService>();
        Requester = services.GetRequiredService<IRequestService>();
        MicroService = services.GetRequiredService<IMicroService>();
        PlayerSetup = new PlayerSetup
        {
            PlayerName = GetType().Name,
            Race = race,
            Type = PlayerType.Participant
        };
    }

    internal PlayerSetup PlayerSetup { get; init; }

    public virtual void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo)
    {
        Log.Info("Start");
        Intel.OnStart(firstObservation, responseData, gameInfo);
    }

    public virtual void OnFrame(ResponseObservation observation)
    {
        if (observation?.Observation == null)
        {
            Log.Error("Obs null");
        }
        Intel.OnFrame(observation);
        Requester.OnFrame();
    }

    public virtual void OnEnd()
    {
        Log.Info($"End");
    }

    internal async Task Run(GameConnection gameConnection)
    {
        while (gameConnection.Status != Status.InGame)
        {
            // loading screen
        }

        Requester.SetConnection(gameConnection);

        OnStart(
            await gameConnection.Observation(),
            await gameConnection.StaticGameData(),
            await gameConnection.GameInfo()
        );

        while (gameConnection.Status == Status.InGame)
        {
            await gameConnection.Step();

            var obs = await gameConnection.Observation();
            if (obs == null && gameConnection.Status == Status.Ended) break;
            
            OnFrame(obs);
        }

        OnEnd();
    }
}