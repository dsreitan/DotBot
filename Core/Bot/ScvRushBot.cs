using SC2APIProtocol;
using SC2ClientApi;

namespace Core.Bot;

public class ScvRushBot : TerranBot
{
    public ScvRushBot(IServiceProvider services) : base(services)
    {
    }

    public override void OnFrame(ResponseObservation observation)
    {
        base.OnFrame(observation);

        UnitService.Train(UnitType.TERRAN_SCV);

        if (Intel.GetWorkers().Count < 14) return;

        var enemyBase = Intel.EnemyColonies.First();

        Log.Warning($"Attacking with {Intel.GetWorkers().Count} SCVs");

        var workers = new Squad();
        workers.AddUnits(Intel.GetWorkers());

        MicroService.AttackMove(workers, enemyBase.Point);
    }
}