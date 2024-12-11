using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elders.Cronus;
using Elders.Cronus.EventStore;
using Microsoft.Extensions.Logging;
using PushNotifications.MigrationSignals;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Commands;
using PushNotifications.Subscriptions.Events;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PushNotifications.Service
{
    [DataContract(Name = "9a707b51-a5b2-4e80-b60f-b7cc8b1d49f2", Namespace = "pushnotifications")]
    public class GavrailDeletesUnsubscribeDublicateEvents : ITrigger,
        ISignalHandle<GavrailDeletesUnsubscribeDublicateEventsSignal>
    {
        private readonly IEventStorePlayer _player;
        private readonly IEventStore _eventStore;
        private readonly ISerializer _serializer;
        private readonly ILogger<GavrailDeletesUnsubscribeDublicateEvents> _logger;
        private byte[] deviceSubscriptionRootNameAsBytes = Encoding.UTF8.GetBytes(":subscription:");
        public GavrailDeletesUnsubscribeDublicateEvents(IEventStorePlayer player, IEventStore eventStore, ILogger<GavrailDeletesUnsubscribeDublicateEvents> logger, ISerializer serializer)
        {
            _player = player;
            _eventStore = eventStore;
            _logger = logger;
            _serializer = serializer;
        }

        public async Task<bool> DeleteAsync(GavrailDeletesUnsubscribeDublicateEventsSignal signal)
        {
            var contractIdBytes = Encoding.UTF8.GetBytes(typeof(UnSubscribed).GetContractId());
            var contractSubscribeIdBytes = Encoding.UTF8.GetBytes(typeof(Subscribed).GetContractId());


            _logger.LogInformation("The migration for un/subscibe events is starting...");
            PlayerOperator playerOperator = new PlayerOperator()
            {
                OnAggregateStreamLoadedAsync = async arStream =>
                 {
                     int index = arStream.Commits.First()
                     .Events.First()
                     .AggregateRootId.AsSpan()
                     .IndexOf(deviceSubscriptionRootNameAsBytes.AsSpan());

                     if (index < 0)
                     {
                         //_logger.LogInformation("Skiping unneeded agregate");
                         return;

                     }

                     Queue<EventRecord> eventRecords = new Queue<EventRecord>();

                     Dictionary<DeviceSubscriberId, UnSubscribed> chekPairs = new Dictionary<DeviceSubscriberId, UnSubscribed>();


                     foreach (AggregateCommitRaw @commit in arStream.Commits)
                     {
                         var @event = commit.Events.First();

                         bool isthisEventUnsubscribed = @event.Data.AsSpan().IndexOf(contractIdBytes) > -1;
                         bool isthisEventSubscribed = @event.Data.AsSpan().IndexOf(contractSubscribeIdBytes) > -1;

                         if (isthisEventUnsubscribed)
                         {
                             UnSubscribed data = _serializer.DeserializeFromBytes<UnSubscribed>(@event.Data);

                             if (chekPairs.ContainsKey(data.SubscriberId) && (chekPairs[data.SubscriberId] == null))
                             {
                                 EventRecord eventRecord = new EventRecord(@event, data);
                                 eventRecords.Enqueue(eventRecord);

                                 chekPairs[data.SubscriberId] = data;
                             }
                         }
                         if (isthisEventSubscribed)
                         {
                             Subscribed data = _serializer.DeserializeFromBytes<Subscribed>(@event.Data);

                             if (chekPairs.ContainsKey(data.SubscriberId))
                             {
                                 if (chekPairs[data.SubscriberId] == null)
                                 {
                                     continue;
                                 }
                                 if (chekPairs[data.SubscriberId] != null)
                                 {
                                     chekPairs[data.SubscriberId] = null;
                                     EventRecord eventRecord = new EventRecord(@event, data);
                                     eventRecords.Enqueue(eventRecord);
                                 }
                             }
                             else
                             {
                                 chekPairs.Add(data.SubscriberId, null);
                                 EventRecord eventRecord = new EventRecord(@event, data);
                                 eventRecords.Enqueue(eventRecord);
                             }
                             //_logger.LogInformation($"Subscribe for user:{data.SubscriberId}");
                         }

                     }

                     if (eventRecords.Count == arStream.Commits.Count)
                     {
                         _logger.LogInformation($"Everythig is fine with aggregate {System.Text.Encoding.UTF8.GetString(arStream.Commits.First().Events.First().AggregateRootId)}");
                         return;
                     }

                     foreach (AggregateCommitRaw @commit in arStream.Commits)
                     {
                         if (signal.IsDryRun == false)
                         {
                             await _eventStore.DeleteAsync(commit.Events.First());
                         }
                     }

                     //_logger.LogWarning("The all events are deleted, the store is cleaned");
                     //_logger.LogInformation("Starting to add events");

                     int revision = 0;
                     while (eventRecords.Count > 0)
                     {
                         revision++;
                         EventRecord currentEvent = eventRecords.Dequeue();

                         var newEvent = new AggregateEventRaw(currentEvent.AggregateEvent.AggregateRootId, currentEvent.AggregateEvent.Data, revision, currentEvent.AggregateEvent.Position, currentEvent.AggregateEvent.Timestamp);

                         if (signal.IsDryRun == false)
                         {
                             await _eventStore.AppendAsync(newEvent).ConfigureAwait(false);
                         }
                     }
                     //_logger.LogInformation("Migration for current aggregate completed");
                 }
            };
            await _player.EnumerateEventStore(playerOperator, new PlayerOptions(), CancellationToken.None).ConfigureAwait(false);

            _logger.LogInformation("Completed!");

            return true;
        }

        public async Task HandleAsync(GavrailDeletesUnsubscribeDublicateEventsSignal signal)
        {
            await DeleteAsync(signal);
        }
    }

    class EventRecord
    {
        public EventRecord(AggregateEventRaw aggregateEvent, Subscribed subscribed)
        {
            AggregateEvent = aggregateEvent;
            Subscribed = subscribed;
            Unsubscribed = null;
        }

        public EventRecord(AggregateEventRaw aggregateEvent, UnSubscribed unsubscribed)
        {
            AggregateEvent = aggregateEvent;
            Unsubscribed = unsubscribed;
            Subscribed = null;
        }
        public AggregateEventRaw AggregateEvent { get; set; }
        public UnSubscribed Unsubscribed { get; set; }
        public Subscribed Subscribed { get; set; }
    }
}
