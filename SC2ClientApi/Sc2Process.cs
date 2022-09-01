using System.Diagnostics;
using System.Runtime.InteropServices;
using SC2ClientApi.Constants;

namespace SC2ClientApi;

public static class Sc2Process
{
    public enum ScreenPosition
    {
        Left,
        Right,
        Center
    }

    private static string? _sc2Directory;

    public static Process? Start(string serverAddress, int port, int screenWidth, int screenHeight, ScreenPosition position)
    {
        var sc2Exe = Sc2Exe();
        var workingDirectory = WorkingDirectory();
        var arguments = Arguments(serverAddress, port, screenWidth, screenHeight, position);

        Log.Info($"Starting sc2 process {arguments}");
        return Process.Start(new ProcessStartInfo(sc2Exe)
        {
            Arguments = arguments, WorkingDirectory = workingDirectory
        });
    }

    public static string MapDirectory(string mapName)
    {
        return @$"{Sc2Directory()}\Maps\{mapName}";
    }

    private static string Arguments(string serverAddress, int port, int screenWidth, int screenHeight, ScreenPosition position)
    {
        var displayMode = position switch
        {
            ScreenPosition.Left =>
                $"0 {ClientConstants.WindowWidth} {screenWidth / 2} {ClientConstants.WindowHeight} {screenHeight / 2} {ClientConstants.WindowY} 0 {ClientConstants.WindowX} 0",
            ScreenPosition.Right =>
                $"0 {ClientConstants.WindowWidth} {screenWidth / 2} {ClientConstants.WindowHeight} {screenHeight / 2} {ClientConstants.WindowY} 0 {ClientConstants.WindowX} {screenWidth / 2}",
            _ =>
                $"0 {ClientConstants.WindowWidth} {screenWidth} {ClientConstants.WindowHeight} {screenHeight} {ClientConstants.WindowY} 0 {ClientConstants.WindowX} 0"
        };

        var arguments = new List<string>
        {
            ClientConstants.Address,
            serverAddress,
            ClientConstants.Port,
            port.ToString(),
            ClientConstants.DisplayMode,
            displayMode
        };

        return string.Join(" ", arguments);
    }


    private static string WorkingDirectory()
    {
        return @$"{Sc2Directory()}\Support64";
    }

    private static string Sc2Exe()
    {
        var versionDirectory = Directory.GetDirectories(@$"{Sc2Directory()}\Versions\", @"Base*")[0];
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? @$"{versionDirectory}\SC2.app"
            : @$"{versionDirectory}\SC2_x64.exe";
    }

    private static string? Sc2Directory()
    {
        if (_sc2Directory != null) return _sc2Directory;

        var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var executeInfo = Path.Combine(myDocuments, "Starcraft II", "ExecuteInfo.txt");
        if (File.Exists(executeInfo))
        {
            var lines = File.ReadAllLines(executeInfo);
            foreach (var line in lines)
            {
                var argument = line[(line.IndexOf('=') + 1)..].Trim();
                if (line.Trim().StartsWith("executable"))
                {
                    _sc2Directory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(argument)));
                    if (_sc2Directory != null) return _sc2Directory;
                }
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            _sc2Directory = Path.Combine(programFiles, "StarCraft II");
            return _sc2Directory;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var applications = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            _sc2Directory = Path.Combine(applications, "StarCraft II");
            return _sc2Directory;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            // implement linux
            return null;

        throw new Exception("Not using a supported OS");
    }
}