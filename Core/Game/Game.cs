using System.Diagnostics;
using SC2ClientApi;

namespace Core.Game;

public abstract class Game
{
    protected Game(GameSettings gameSettings)
    {
        GameSettings = gameSettings;
    }

    protected GameSettings GameSettings { get; init; }
    protected List<Process?> GameProcesses { get; set; } = new();
    public abstract Task Start();

    public async Task<GameConnection> Connect(int port, Sc2Process.ScreenPosition position = Sc2Process.ScreenPosition.Center)
    {
        var connection = new GameConnection();
        if (await connection.Connect(GameSettings.ServerAddress, port, 1))
            return connection;

        GameProcesses.Add(Sc2Process.Start(GameSettings.ServerAddress, port, GameSettings.ScreenWidth, GameSettings.ScreenHeight, position)); 
        Log.Info("Waiting for sc2 to start");
        await Task.Delay(5000);

        await connection.Connect(GameSettings.ServerAddress, port, 10);

        return connection;
    }
}