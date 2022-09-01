namespace SC2ClientApi;

public static class Log
{
    public static void Info(string text) => WriteLine(text);
    public static void Success(string text) => WriteLineColored(text, ConsoleColor.Green);
    public static void Warning(string text) => WriteLineColored(text, ConsoleColor.Yellow);
    public static void Error(string text) => WriteLineColored(text, ConsoleColor.Red);

    private static void WriteLine(string text) => Console.WriteLine($"[{DateTime.Now:T}] {text}");

    private static void WriteLineColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        WriteLine(text);
        Console.ResetColor();
    }
}