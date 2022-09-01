using SC2APIProtocol;
using RequestType = SC2APIProtocol.Request.RequestOneofCase;

namespace SC2ClientApi;

/// <summary>
///     Handles responses from websockets sequentially by type
///     ResponseOneofCase and RequestOneofCase are equal
/// </summary>
public class ResponseHandler
{
    private readonly IDictionary<RequestType, List<Action<Response>>> _handlers;

    public ResponseHandler()
    {
        _handlers = new Dictionary<RequestType, List<Action<Response>>>();
    }

    public void RegisterHandler(RequestType key, Action<Response> handler)
    {
        lock (_handlers)
        {
            if (_handlers.ContainsKey(key))
                _handlers[key].Add(handler);
            else
                _handlers[key] = new() {handler};
        }
    }

    public void DeregisterHandler(RequestType key, Action<Response> handler)
    {
        lock (_handlers)
        {
            if (_handlers.ContainsKey(key))
                _handlers[key].Remove(handler);
        }
    }

    public void Handle(RequestType key, Response response)
    {
        if (key == RequestType.None)
        {
            Log.Error($"ResponseHandler error: {response}");
            return;
        }

        lock (_handlers)
        {
            // returns the same first response of a request type, to all other waiting requests for that type as well :|
            if (_handlers.ContainsKey(key))
            {
                if (_handlers[key].Count > 1) Log.Warning($"{key} > 1 handlers: {_handlers[key].First()} {_handlers[key].Last()}");
                _handlers[key].ForEach(handler => handler(response));
            }
        }
    }
}