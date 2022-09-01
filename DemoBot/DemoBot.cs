using Core;
using Core.Bot;
using Microsoft.Extensions.DependencyInjection;
using SC2APIProtocol;
using SC2ClientApi;

namespace Demo;

public class DemoBot : TerranBot
{
    public DemoBot(IServiceProvider services):base(services)
    {
    }
    
    public override void OnFrame(ResponseObservation observation)
    {
        if (observation.Observation.GameLoop % 100 == 0)
        {
        }
    }
}