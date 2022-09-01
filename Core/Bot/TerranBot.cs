using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;

namespace Core.Bot;

public abstract class TerranBot : BaseBot
{
    protected readonly IUnitService UnitService;

    protected TerranBot(IServiceProvider services) : base(services, Race.Terran)
    {
        UnitService = services.GetRequiredService<IUnitService>();
    }

    public override void OnStart(ResponseObservation firstObservation, ResponseData responseData, ResponseGameInfo gameInfo)
    {
        base.OnStart(firstObservation, responseData,gameInfo);
    }
    
    public override void OnFrame(ResponseObservation observation){base.OnFrame(observation);}
    public override void OnEnd(ResponseObservation observation){}

}