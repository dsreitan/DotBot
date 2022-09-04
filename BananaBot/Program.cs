using Core;
using Core.Bot;
using Core.Game;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SC2APIProtocol;
using SC2ClientApi;

var host = Host.CreateDefaultBuilder(args)
    .AddCoreServices()
    .Build();

var gameSettings = new GameSettings(args);
var playerOne = new BananaBot.BananaBot(host.Services.CreateScope().ServiceProvider);

Game game = gameSettings.GameMode switch
{
    GameMode.Singleplayer => new SingleplayerGame(gameSettings, playerOne, Race.Protoss, AIBuild.Air, Difficulty.Easy),
    GameMode.Ladder => new LadderGame(gameSettings, playerOne),
    GameMode.Multiplayer or _ => new MultiplayerGame(gameSettings, playerOne, new ScvRushBot(host.Services.CreateScope().ServiceProvider))
};

Log.Info($"Starting {game} {gameSettings}");

await game.Start();