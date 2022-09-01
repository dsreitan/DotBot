using Core.Bot;
using SC2ClientApi;

namespace Core.Game;

public class MultiplayerGame : Game
{
    public MultiplayerGame(GameSettings gameSettings, BaseBot playerOne, BaseBot playerTwo) : base(gameSettings)
    {
        GameSettings = gameSettings;
        PlayerOne = playerOne;
        PlayerTwo = playerTwo;
    }

    private BaseBot PlayerOne { get; }
    private BaseBot PlayerTwo { get; }

    public override async Task Start()
    {
        var p1ConnectionTask = Connect(GameSettings.GamePort, Sc2Process.ScreenPosition.Left);
        var p2ConnectionTask = Connect(GameSettings.StartPort, Sc2Process.ScreenPosition.Right);
        var p1Connection = await p1ConnectionTask;
        var p2Connection = await p2ConnectionTask;

        await p1Connection.CreateGame(PlayerOne.PlayerSetup, PlayerTwo.PlayerSetup, GameSettings.MapName);

        var multiplayerPorts = (
            GameSettings.GamePort + 1,
            GameSettings.GamePort + 2,
            GameSettings.StartPort + 1,
            GameSettings.StartPort + 2);

        await Task.WhenAll(
            p1Connection.JoinGame(PlayerOne.PlayerSetup, multiplayerPorts),
            p2Connection.JoinGame(PlayerTwo.PlayerSetup, multiplayerPorts)
        );

        var p1RunTask = Task.Factory.StartNew(x => PlayerOne.Run(p1Connection), TaskCreationOptions.LongRunning);
        var p2RunTask = Task.Factory.StartNew(x => PlayerTwo.Run(p2Connection), TaskCreationOptions.LongRunning);

        await Task.WhenAll(
            p1RunTask, p2RunTask
        );

        GameProcesses.ForEach(x => x?.Kill());
    }
}