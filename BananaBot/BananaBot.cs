using Core;
using Core.Bot;
using Core.Extensions;
using SC2APIProtocol;
using SC2ClientApi;

namespace BananaBot;

public class BananaBot : TerranBot
{
    public BananaBot(IServiceProvider services) : base(services)
    {
    }

    private bool hasAttacked;
    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);
        UnitService.Train(UnitType.TERRAN_SCV);
        if (!hasAttacked && Intel.GetWorkers().Count == 14)
        {
            hasAttacked = true;
            
            var enemyBase = Intel.EnemyColonies.First();

            Log.Warning(
                $"Attacking enemy base {enemyBase.Point} with {observation.Observation.RawData.Units.Count(u => u.UnitType.Is(UnitType.TERRAN_SCV))} SCVs");

            var workers = new Squad();
            workers.AddUnits(Intel.GetWorkers());
            MicroService.AttackMove(workers, enemyBase.Point);
        }

        if (observation.Observation.GameLoop % 1000 == 0)
        {
            Log.Info($"Loop {observation.Observation.GameLoop}");
        }
    }
}