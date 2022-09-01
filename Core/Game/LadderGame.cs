using Core.Bot;

namespace Core.Game;

public class LadderGame : Game
{
    public LadderGame(GameSettings gameSettings, BaseBot playerOne) : base(gameSettings)
    {
        GameSettings = gameSettings;
        PlayerOne = playerOne;
    }

    private BaseBot PlayerOne { get; }

    public override async Task Start()
    {
        var p1Connection = await Connect(GameSettings.GamePort);
        await p1Connection.JoinGame(PlayerOne.PlayerSetup, (
            GameSettings.GamePort + 1,
            GameSettings.GamePort + 2,
            GameSettings.StartPort + 1,
            GameSettings.StartPort + 2));
        await PlayerOne.Run(p1Connection);
    }
}