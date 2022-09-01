using Core.Bot;
using SC2APIProtocol;

namespace Core.Game;

public class SingleplayerGame : Game
{
    public SingleplayerGame(GameSettings gameSettings, BaseBot playerOne, Race race, AIBuild build, Difficulty difficulty) : base(gameSettings)
    {
        GameSettings = gameSettings;
        PlayerOne = playerOne;
        ComputerSetup = new PlayerSetup
        {
            Race = race,
            AiBuild = build,
            Difficulty = difficulty,
            Type = PlayerType.Computer
        };
    }

    private BaseBot PlayerOne { get; }
    private PlayerSetup ComputerSetup { get; }

    public override async Task Start()
    {
        var connection = await Connect(GameSettings.GamePort);
        await connection.CreateGame(PlayerOne.PlayerSetup, ComputerSetup, GameSettings.MapName);
        await connection.JoinGame(PlayerOne.PlayerSetup);
        await PlayerOne.Run(connection);
    }
}