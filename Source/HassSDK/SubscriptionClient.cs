using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HassSDK.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassSDK
{
    public class SubscriptionClient
    {
        private HassClient Client { get; }

        private readonly string _hassWebsocketUri;

        private bool _started;

        private ClientWebSocket _ws;

        private CancellationTokenSource cancellationTokenSource;

        private readonly Dictionary<string, List<Action<EventData>>> subscriptions = new Dictionary<string, List<Action<EventData>>>();

        public SubscriptionClient(HassClient client)
        {
            Client = client;

            _hassWebsocketUri = client.BaseUri + "/websocket";
        }

        public async Task StartAsync()
        {
            if (_started)
            {
                return;
            }

            _started = true;
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            _ws = new ClientWebSocket();
            await _ws.ConnectAsync(new Uri(_hassWebsocketUri.Replace("http://", "ws://") + $"?api_password={Client.Password}"), default);

            Task.Run(() => ProcessAsync(cancellationTokenSource.Token));

            _started = true;
        }

        public async Task StopAsync()
        {
            if (_ws == null)
                return;

            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", default);
            _ws.Dispose();

            _ws = null;
            _started = false;
        }

        public void Subscribe(string entityId, Action<EventData> action)
        {
            lock (subscriptions)
            {
                if (subscriptions.TryGetValue(entityId, out var existing))
                {
                    existing.Add(action);
                }
                else
                {
                    subscriptions.Add(entityId, new List<Action<EventData>> { action });
                }
            }
        }

        private async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var buffer = new ArraySegment<Byte>(new Byte[2048]);

            while (_ws.State == WebSocketState.Open)
            {
                var result = await _ws.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", default);
                }
                else
                {
                    byte[] payload = buffer.Array;
                    var msg = Encoding.UTF8.GetString(payload, 0, result.Count);
                    while (!result.EndOfMessage)
                    {
                        result = await _ws.ReceiveAsync(buffer, cancellationToken);
                        payload = buffer.Array;
                        msg += Encoding.UTF8.GetString(payload, 0, result.Count);
                    }

                    var events = GetEventsToRaise(msg, out var eventData);
                    if (events != null)
                    {
                        foreach (var ev in events)
                        {
                            ev(eventData);
                        }
                    }
                }
            }
        }

        private List<Action<EventData>> GetEventsToRaise(string msg, out EventData eventData)
        {
            var json = JToken.Parse(msg);
            if (json.IsAuthMessage())
            {
                var newMsg = JsonConvert.SerializeObject(new SubscribeToEventsCommand(EventType.state_changed, 1));
                var bytes = Encoding.UTF8.GetBytes(newMsg);
                _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, default);
                eventData = null;
                return null;
            }

            if (json.IsResult())
            {
                eventData = null;
                return null;
            }

            if (!json.IsEvent())
            {
                eventData = null;
                return null;
            }

            var entityId = json.ExtractEntityId().ToLowerInvariant();
            if (!subscriptions.TryGetValue(entityId, out var actions))
            {
                eventData = null;
                return null;
            }

            if (json.IsClickEvent())
            {
                //eventData.ClickData = new Click { ClickType = (string)json["event"]["data"]["click_type"] };
            }

            if (!json.IsStateChangeEvent())
            {
                eventData = null;
                return null;
            }

            //entity_boolean doesn't have a "last_triggered" attribute.
            if (!entityId.Contains("input_boolean."))
            {
                if (!json.HasNewStateWithLastTriggered())
                {
                    eventData = null;
                    return null; // Irrelevant event, we need new states that has "last time triggered" otherwise it might be an event provoked by reloading Hass. Unsure about this.
                }

            }
            if (!json.IsTheMostRelevantStateChangeMessage())
            {
                eventData = null;
                return null; // Is most probably a 'duped' event, throw it away ..
            }
            if (!json.HasNewState())
            {
                eventData = null;
                return null; // Irrelevant event, we need new states only ..
            }

            eventData = EventData.FromJson(json);
            return actions;
        }
    }
}
