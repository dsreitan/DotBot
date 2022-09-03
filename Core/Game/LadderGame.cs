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
            GameSettings.GamePort + 2,
            GameSettings.GamePort + 3,
            GameSettings.GamePort + 4,
            GameSettings.GamePort + 5));

        await PlayerOne.Run(p1Connection);
    }
}