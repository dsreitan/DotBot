using System.Net.WebSockets;
using Google.Protobuf;
using SC2APIProtocol;
using SC2ClientApi.Constants;
using Action = SC2APIProtocol.Action;

namespace SC2ClientApi;

public class GameConnection
{
    private const int READ_BUFFER = 1024;
    private const int MAX_CONNECTION_ATTEMPTS = 25;
    private const int TIMEOUT = 2000; //ms
    private const int TIMEOUT_LONG = 5000; //ms
    private readonly ResponseHandler _responseHandler;
    private readonly ClientWebSocket _socket;
    private readonly CancellationToken _token = CancellationToken.None;
    private Status _status;

    public GameConnection()
    {
        _socket = new ClientWebSocket();
        _responseHandler = new ResponseHandler();
    }

    public uint GameLoop { get; set; }
    public uint PlayerId { get; set; }

    private string GameVersion { get; set; } = string.Empty;

    public Status Status
    {
        get => _status;
        private set
        {
            if (_status == value) return;
            Log.Info($"ConnectionStatus changed {_status} -> {value}");
            _status = value;
        }
    }

    public async Task<bool> Connect(string address, int port, int maxAttempts = MAX_CONNECTION_ATTEMPTS)
    {
        var uri = new Uri($"ws://{address}:{port}/sc2api");
        Log.Info($"Connecting to {uri}");

        var connectionAttempt = 1;
        do
        {
            try
            {
                await _socket.ConnectAsync(uri, _token);
            }
            catch (AggregateException ex)
            {
                // handle AggEx differently?
                Log.Info($"Connection to {uri} attempt {connectionAttempt}/{maxAttempts} failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Info($"Connection to {uri} attempt {connectionAttempt}/{maxAttempts} failed: {ex.Message}");
            }

            connectionAttempt++;
            await Task.Delay(TIMEOUT, _token);
        } while (_socket.State != WebSocketState.Open && connectionAttempt <= maxAttempts);

        if (_socket.State != WebSocketState.Open)
            return false;

        Log.Success($"Connected to {uri}. Start receiving responses from client");
        Task.Factory.StartNew(ReceiveForever, TaskCreationOptions.LongRunning);
        await Task.Delay(TIMEOUT);

        var pingResponse = await SendAndReceiveAsync(ClientConstants.RequestPing);
        if (pingResponse == null)
        {
            Log.Error($"Ping to {uri} failed");
            return false;
        }

        Log.Info(
            $"Pinged {uri} - [GameVersion={pingResponse.Ping.GameVersion}] [DataVersion={pingResponse.Ping.DataVersion}] [DataBuild={pingResponse.Ping.DataBuild}] [BaseBuild={pingResponse.Ping.BaseBuild}]");
        GameVersion = pingResponse.Ping.GameVersion;
        return pingResponse.Ping.HasGameVersion;
    }

    public async Task<ResponseCreateGame?> CreateGame(PlayerSetup playerOne, PlayerSetup playerTwo, string mapName)
    {
        var request = new Request
        {
            CreateGame = new RequestCreateGame
            {
                Realtime = false,
                DisableFog = false,
                PlayerSetup = { playerOne, playerTwo }
            }
        };

        if (File.Exists(mapName) || File.Exists(Sc2Process.MapDirectory(mapName)))
            request.CreateGame.LocalMap = new LocalMap { MapPath = mapName };
        else
            request.CreateGame.BattlenetMapName = mapName;

        Log.Info($"{playerOne.PlayerName} Creating game");
        var response = await SendAndReceiveAsync(request);
        if (response?.CreateGame == null || response.CreateGame.HasError)
            Log.Error($"Creating game failed {response?.Error} {response?.CreateGame?.Error}");
        else
            Log.Success($"{playerOne.PlayerName} Created game");

        return response?.CreateGame;
    }

    public async Task<ResponseJoinGame?> JoinGame(PlayerSetup playerSetup, (int, int, int, int)? multiplayerPorts = null)
    {
        var request = new Request
        {
            JoinGame = new RequestJoinGame
            {
                Race = playerSetup.Race,
                PlayerName = playerSetup.PlayerName,
                Options = new InterfaceOptions
                {
                    Raw = true,
                    Score = true,
                    ShowCloaked = true,
                    ShowBurrowedShadows = true,
                    ShowPlaceholders = true,
                    RawAffectsSelection = false,
                    RawCropToPlayableArea = false
                }
            }
        };

        if (multiplayerPorts != null)
        {
            // request.JoinGame.SharedPort // deprecated: https://github.com/Blizzard/s2client-proto/blob/master/s2clientprotocol/sc2api.proto#L220
            request.JoinGame.ServerPorts = new PortSet { GamePort = multiplayerPorts.Value.Item1, BasePort = multiplayerPorts.Value.Item2 };
            request.JoinGame.ClientPorts.Add(new PortSet { GamePort = multiplayerPorts.Value.Item3, BasePort = multiplayerPorts.Value.Item4 });
        }

        Log.Info($"{playerSetup.PlayerName} Joining game");
        var response = await SendAndReceiveAsync(request);
        if (response?.JoinGame == null || response.JoinGame.HasError)
            // getting null response, but the clients are still joining :/
            // Log.Error($"{Player.PlayerName} Joining game failed {response?.JoinGame.Error}");
            return null;

        Log.Success($"{playerSetup.PlayerName} Joined game {response.JoinGame}");
        PlayerId = response.JoinGame.PlayerId;
        return response.JoinGame;
    }

    public async Task<Response?> Step()
    {
        return await SendAndReceiveAsync(ClientConstants.RequestStep);
    }

    public async Task<ResponseObservation?> Observation(uint? gameLoop = null)
    {
        var request = ClientConstants.RequestObservation;

        if (gameLoop != null)
            request.Observation.GameLoop = gameLoop.Value;

        if (Status == Status.Ended) return null;

        var response = await SendAndReceiveAsync(request);
        if (response?.Observation == null)
        {
            Log.Warning("Observation null. Probably timeout.");
            return null;
        }

        GameLoop = response.Observation.Observation.GameLoop;
        return response.Observation;
    }

    public async Task<ResponseAction?> ActionRequest(List<Action> actions)
    {
        if (actions.Count == 0) return null;

        var request = new Request { Action = new RequestAction() };
        request.Action.Actions.AddRange(actions);

        var response = await SendAndReceiveAsync(request);
        return response?.Action;
    }

    public async Task<ResponseDebug?> DebugRequest(List<DebugCommand> debugCommands)
    {
        if (debugCommands.Count == 0) return null;

        var response = await SendAndReceiveAsync(new Request { Debug = new RequestDebug { Debug = { debugCommands } } });

        return response?.Debug;
    }

    public async Task<ResponseGameInfo?> GameInfo()
    {
        var response = await SendAndReceiveAsync(ClientConstants.RequestGameInfo);
        return response?.GameInfo;
    }

    public async Task<ResponseData?> StaticGameData()
    {
        var response = await SendAndReceiveAsync(ClientConstants.RequestData);
        return response?.Data;
    }

    public async Task<Response?> SendAndReceiveAsync(Request req)
    {
        Response? response = null;

        if (req.RequestCase == Request.RequestOneofCase.None)
        {
            Log.Warning("Request case none");
            return response;
        }

        if (_socket.State != WebSocketState.Open)
        {
            Log.Warning($"Can't send request due to socket state {_socket.State}");
            return response;
        }

        var handlerResolve = new Task(() => { });
        var handler = new Action<Response>(r =>
        {
            response = r;
            handlerResolve.RunSynchronously();
        });

        _responseHandler.RegisterHandler(req.RequestCase, handler);

        await SendAsync(req);

        var shouldWaitLonger =
            req.RequestCase is Request.RequestOneofCase.Step or Request.RequestOneofCase.CreateGame;

        if (req.RequestCase != Request.RequestOneofCase.JoinGame && !handlerResolve.Wait(shouldWaitLonger ? TIMEOUT_LONG : TIMEOUT) &&
            Status != Status.Ended)
            Log.Error($"Request timed out \n{req}");

        _responseHandler.DeregisterHandler(req.RequestCase, handler);

        return response;
    }

    private async Task SendAsync(Request req)
    {
        var buffer = new ReadOnlyMemory<byte>(req.ToByteArray());

        await _socket.SendAsync(buffer, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage, _token);
    }

    private async Task ReceiveForever()
    {
        var buffer = new ArraySegment<byte>(new byte[READ_BUFFER]);
        while (_socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result;
            using var ms = new MemoryStream();
            do
            {
                result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            var response = Response.Parser.ParseFrom(ms.GetBuffer(), 0, (int)ms.Position);
            Status = response.Status;
            if (Status != Status.Ended)
                _responseHandler.Handle((Request.RequestOneofCase)response.ResponseCase, response);
        }
    }

    public async Task Disconnect()
    {
        await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _token);
        _socket.Dispose();
    }
}