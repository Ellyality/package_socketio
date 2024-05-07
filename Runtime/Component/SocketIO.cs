using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using SocketIOClient.Transport;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ellyality.SocketIO
{
    [AddComponentMenu("Ellyality/Network/SocketIO")]
    public class SocketIO : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField] string uri;
        [SerializeField] bool StartAtBegining = true;
        [Header("Config")]
        [SerializeField] double RandomizationFactor = 0.5;
        [SerializeField] double ReconnectionDelay = 1000;
        [SerializeField] int ReconnectionDelayMax = 5000;
        [SerializeField] int ReconnectionAttempts = int.MaxValue;
        [SerializeField] string Path = "/socket.io";
        [SerializeField] double ConnectionTimeout = 20;
        [SerializeField] bool Reconnection = true;
        [SerializeField] TransportProtocol Transport = TransportProtocol.WebSocket;
        [SerializeField] int EIO = 4;
        [SerializeField] bool AutoUpgrade = true;
        [Header("Register")]
        [SerializeField] SocketIOUnity.UnityThreadScope ThreadScope = SocketIOUnity.UnityThreadScope.Update;
        [SerializeField] SocketIOEvent[] Events = new SocketIOEvent[0];
        [SerializeField] SocketIOEvent[] UnityEvents = new SocketIOEvent[0];

        public SocketIOUnity socket { protected set; get; } = null;

        protected virtual void Start()
        {
            if (StartAtBegining) StartSocketIO();
        }

        protected virtual void OnDestroy()
        {
            if (socket != null)
            {
                try
                {
                    socket.Dispose();
                }
                catch (Exception) 
                {
                    Debug.LogWarning("[SocketIO] Dispose a socket without connection");
                }
            }
        }

        public void StartSocketIO()
        {
            if (socket != null) return;
            Debug.Log($"Socket IO Start!\nPath: {Path}\nURL: {uri}");
            socket = new SocketIOUnity(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                },
                RandomizationFactor = RandomizationFactor,
                ReconnectionDelay = ReconnectionDelay,
                ReconnectionDelayMax = ReconnectionDelayMax,
                ReconnectionAttempts = ReconnectionAttempts,
                Path = Path,
                ConnectionTimeout = TimeSpan.FromSeconds(ConnectionTimeout),
                Reconnection = Reconnection,
                Transport = Transport,
                EIO = EIO,
                AutoUpgrade = AutoUpgrade,
            });
            socket.JsonSerializer = new NewtonsoftJsonSerializer();
            socket.unityThreadScope = ThreadScope;
            for (int i = 0; i < Events.Length; i++)
            {
                socket.On(Events[i].EventName, Events[i].Callback.Invoke);
            }
            for (int i = 0; i < UnityEvents.Length; i++)
            {
                socket.OnUnityThread(UnityEvents[i].EventName, UnityEvents[i].Callback.Invoke);
            }
            Debug.Log("Trying connect");
            socket.Connect();
        }

        public void Emit(string eventName, params object[] data) => socket.Emit(eventName, data);
        public void EmitStringAsJSON(string eventName, string data) => socket.EmitStringAsJSON(eventName, data);
        public void Connect() => socket.Connect();
        public void Disconnect() => socket.Disconnect();
        public void RegisterEvent(string eventName, Action<SocketIOResponse> callback) => socket.On(eventName, callback);
        public void UnRegisterEvent(string eventName) => socket.Off(eventName);
        public void RegisterUnityEvent(string eventName, Action<SocketIOResponse> callback) => socket.OnUnityThread(eventName, callback);
    }
}
