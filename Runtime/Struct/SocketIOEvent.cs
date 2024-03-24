using SocketIOClient;
using UnityEngine.Events;

namespace Ellyality.SocketIO
{
    [System.Serializable]
    public struct SocketIOEvent
    {
        public string EventName;
        public UnityEvent<SocketIOResponse> Callback;
    }
}
