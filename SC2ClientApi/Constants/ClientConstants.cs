using SC2APIProtocol;

namespace SC2ClientApi.Constants;

public static class ClientConstants
{
    public const string Address = "-listen";
    public const string Port = "-port";
    public const string DisplayMode = "-displaymode";
    public const string WindowWidth = "-windowwidth";
    public const string WindowHeight = "-windowheight";
    public const string WindowX = "-windowx";
    public const string WindowY = "-windowy";
    public static readonly Request RequestRestartGame = new() {RestartGame = new()};
    public static readonly Request RequestLeaveGame = new() {LeaveGame = new()};
    public static readonly Request RequestQuickSave = new() {QuickSave = new()};
    public static readonly Request RequestQuickLoad = new() {QuickLoad = new()};
    public static readonly Request RequestQuit = new() {Quit = new()};
    public static readonly Request RequestGameInfo = new() {GameInfo = new()};
    public static readonly Request RequestSaveReplay = new() {SaveReplay = new()};
    public static readonly Request RequestAvailableMaps = new() {AvailableMaps = new()};
    public static readonly Request RequestPing = new() {Ping = new()};
    public static readonly Request RequestObservation = new() {Observation = new()};
    public static readonly Request RequestStep = new() {Step = new() {Count = 1}};
    public static readonly Request RequestData = new() {Data = new() {AbilityId = true, BuffId = true, UnitTypeId = true, UpgradeId = true}};
}