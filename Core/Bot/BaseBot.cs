using Core.Data;
using Core.Intel;
using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Bot;

public abstract class BaseBot
{
    protected readonly IDataService Data;
    protected readonly IIntelService Intel;
    protected readonly IMessageService MessageService;
    protected readonly IMicroService MicroService;

    public BaseBot(IServiceProvider services, Race race)
    {
        Data = services.GetRequiredService<IDataService>();
        Intel = services.GetRequiredService<IIntelService>();
        MessageService = services.GetRequiredService<IMessageService>();
        MicroService = services.GetRequiredService<IMicroService>();
        PlayerSetup = new PlayerSetup { PlayerName = GetType().Name, Race = race, Type = PlayerType.Participant };
    }

    internal PlayerSetup PlayerSetup { get; init; }

    public virtual void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo)
    {
        Data.OnStart(firstObservation, responseData, gameInfo);
        Intel.OnStart(firstObservation, responseData, gameInfo);
    }

    public virtual void OnFrame(ResponseObservation observation)
    {
        if (observation?.Observation == null) Log.Error("Obs null");
        Intel.OnFrame(observation);
        MessageService.OnFrame();
    }

    public virtual void OnEnd()
    {
        Log.Info("End");
    }

    internal async Task Run(GameConnection gameConnection)
    {
        while (gameConnection.Status != Status.InGame)
        {
            // loading screen
        }

        MessageService.SetConnection(gameConnection);

        OnStart(
            await gameConnection.Observation(),
            await gameConnection.StaticGameData(),
            await gameConnection.GameInfo()
        );

        while (gameConnection.Status == Status.InGame)
        {
            await gameConnection.Step();

            var obs = await gameConnection.Observation();
            if (obs == null || gameConnection.Status == Status.Ended) break;

            OnFrame(obs);
        }

        OnEnd();
    }
}