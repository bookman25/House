using System;
using Newtonsoft.Json.Linq;

namespace HassSDK.Models
{
    public class EventData
    {
        [NotNull]
        public string EntityId { get; }

        public string Origin { get; }

        [NotNull]
        public string Type { get; }

        [NotNull]
        public StateChange<string> OldState { get; }

        [NotNull]
        public StateChange<string> NewState { get; }

        protected EventData(
            [NotNull] string entityId,
            [NotNull] string type,
            string origin,
            [NotNull] StateChange<string> oldState,
            [NotNull] StateChange<string> newState)
        {
            EntityId = entityId;
            OldState = oldState;
            NewState = newState;
        }

        [NotNull]
        public static EventData FromJson(JToken json)
        {
            var entityId = json["event"]["data"]["entity_id"].ToString();
            var type = json["type"].Value<string>();
            var origin = json["event"]["origin"].ToString();
            var oldState = json["event"]["data"]["old_state"];
            var newState = json["event"]["data"]["new_state"];

            return new EventData(
                Constraint.NotNull(entityId, "entityId"),
                Constraint.NotNull(type, "type"),
                origin,
                FromStateJson(oldState),
                FromStateJson(newState));
        }


        [NotNull]
        private static StateChange<string> FromStateJson(JToken json)
        {
            var state = json["state"].Value<string>();
            var date = json["last_changed"].Value<DateTime>();
            return new StateChange<string>(Constraint.NotNull(state, "state"), date);
        }
    }

    public class StateChange<T>
    {
        [NotNull]
        public T State { get; }

        public DateTime Date { get; }

        public StateChange([NotNull] T state, DateTime date)
        {
            State = state;
            Date = date;
        }

        [NotNull]
        public StateChange<int> AsInt()
        {
            return new StateChange<int>(int.Parse(State.ToString()), Date);
        }
    }


    internal class SubscribeToEventsCommand
    {
        public int id { get; }
        public string type { get; } = "subscribe_events";
        public string event_type { get; }

        public SubscribeToEventsCommand(EventType eventType, int id)
        {
            this.event_type = Enum.GetName(typeof(EventType), eventType);
            this.id = id;
        }
    }

    internal enum EventType
    {
        state_changed,
        click
    }
}
