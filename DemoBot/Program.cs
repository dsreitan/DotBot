using Core;
using Core.Game;
using Demo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SC2APIProtocol;

Console.WriteLine("Hello, DemoBot!");

var host = Host.CreateDefaultBuilder(args)
    .AddCoreServices()
    //.ConfigureServices((_, s) => s.AddScoped<IUnitService, MyCustomUnitService>())
    .Build();

var gameSettings = new GameSettings(args);
var playerOne = new ScvRushBot(host.Services.CreateScope().ServiceProvider);

Game game = gameSettings.GameMode switch
{
    GameMode.Singleplayer => new SingleplayerGame(gameSettings, playerOne, Race.Protoss, AIBuild.Air, Difficulty.Easy),
    GameMode.Ladder => new LadderGame(gameSettings, playerOne),
    GameMode.Multiplayer or _ =>
        new MultiplayerGame(gameSettings, playerOne, new ScvRushBot(host.Services.CreateScope().ServiceProvider))
};

await game.Start();