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
}