namespace Core.Game;

public class GameSettings
{
    public GameSettings(string[] args)
    {
        UseArgs(args);
    }

    public GameMode GameMode { get; set; } = GameMode.Singleplayer;
    public string ServerAddress { get; set; } = "127.0.0.1";
    public int GamePort { get; set; } = 8765;
    public int StartPort { get; set; } = 8775;
    public string OpponentId { get; set; } = "";

    public int ScreenWidth { get; set; } = 2560;
    public int ScreenHeight { get; set; } = 1440;

    public string MapName { get; set; } = "2000AtmospheresAIE.SC2Map";

    private void UseArgs(string[] args)
    {
        if (args.Length == 0) return;
        GameMode = GameMode.Ladder;
        for (var i = 0; i < args.Length; i += 2)
            switch (args[i])
            {
                case "-g":
                case "--GamePort":
                    GamePort = int.Parse(args[i + 1]);
                    break;
                case "-o":
                case "--StartPort":
                    StartPort = int.Parse(args[i + 1]);
                    break;
                case "-l":
                case "--LadderServer":
                    ServerAddress = args[i + 1];
                    break;
                case "--OpponentId":
                    OpponentId = args[i + 1];
                    break;
            }
    }
}

public enum GameMode
{
    Singleplayer,
    Multiplayer,
    Ladder
}