using Core;
using Core.Bot;
using SC2APIProtocol;

namespace AppleBot;

public class ApplePlayerBot : TerranBot
{
    public ApplePlayerBot(IServiceProvider services) : base(services)
    {
    }

    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);
        UnitService.Train(UnitType.TERRAN_SCV);
        if (Intel.Observation.PlayerCommon.FoodWorkers == 13)
        {
            var enemyBase = Intel.EnemyColonies.First();

            var workers = new Squad();
            workers.AddUnits(Intel.GetWorkers());
            MicroService.AttackMove(workers, enemyBase.Point);
        }
    }
}