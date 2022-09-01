using Core;
using Core.Bot;
using SC2APIProtocol;
using SC2ClientApi;

namespace Demo;

public class ScvRushBot : TerranBot
{
    public ScvRushBot(IServiceProvider services) : base(services)
    {
    }

    private bool hasAttacked;
    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);
        UnitService.Train(UnitType.TERRAN_SCV);
        if (!hasAttacked && Intel.Observation.PlayerCommon.FoodWorkers == 14)
        {
            hasAttacked = true;
            
            var enemyBase = Intel.EnemyColonies.First();

            Log.Warning(
                $"Attacking enemy base {enemyBase.Point} with {observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.TERRAN_SCV))} SCVs");

            var workers = new Squad();
            workers.AddUnits(Intel.GetWorkers());
            MicroService.AttackMove(workers, enemyBase.Point);
        }
    }
}