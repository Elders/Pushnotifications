using Elders.Cronus;
using Elders.Cronus.EventStore;
using Elders.Cronus.IntegrityValidation;
using Microsoft.Extensions.Logging;
using PushNotifications.MigrationSignals;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace PushNotifications.Service
{
    [DataContract(Namespace = "pushnotifications", Name = "3fdc2894-e65b-44bd-b147-f2d13f2e83e6")]
    public sealed class DeleteAggregatesWithIntegrityViolationTrigger : ITrigger,
        ISignalHandle<DeleteAggregatesWithIntegrityViolationSignal>
    {
        private readonly IEventStorePlayer _player;
        private readonly IEventStore _eventStore;
        private readonly ISerializer _serializer;
        private readonly IIntegrityPolicy<EventStream> _integrityPolicy;
        private readonly ILogger<DeleteAggregatesWithIntegrityViolationTrigger> _logger;

        public DeleteAggregatesWithIntegrityViolationTrigger(IEventStorePlayer player, IEventStore eventStore, ISerializer serializer, IIntegrityPolicy<EventStream> integrityPolicy, ILogger<DeleteAggregatesWithIntegrityViolationTrigger> logger)
        {
            _player = player;
            _eventStore = eventStore;
            _serializer = serializer;
            _integrityPolicy = integrityPolicy;
            _logger = logger;
        }

        public async Task HandleAsync(DeleteAggregatesWithIntegrityViolationSignal signal)
        {
            _logger.LogInformation("Starting to delete aggreagtes with violated integrity...");

            var @operator = new PlayerOperator()
            {
                OnAggregateStreamLoadedAsync = async stream =>
                {
                    System.ReadOnlyMemory<byte> aggregateRootId = stream.Commits.First().Events.First().AggregateRootId;
                    string aggregateRootIdAsString = System.Text.Encoding.UTF8.GetString(stream.Commits.First().Events.First().AggregateRootId.Span);

                    List<AggregateCommit> aggregateCommits = new List<AggregateCommit>();

                    foreach (AggregateCommitRaw commitRaw in stream.Commits)
                    {
                        List<IEvent> @events = new List<IEvent>();
                        List<IPublicEvent> publicEvents = new List<IPublicEvent>();

                        var messages = commitRaw.Events.Select(@event => _serializer.DeserializeFromBytes<IMessage>(@event.Data));
                        foreach (IMessage msg in messages)
                        {
                            if (msg is IEvent @event)
                                @events.Add(@event);
                            else if (msg is IPublicEvent publicEvent)
                                publicEvents.Add(publicEvent);
                        }

                        AggregateEventRaw firstEvent = commitRaw.Events.First();
                        int rev = firstEvent.Revision;

                        AggregateCommit sourceCommit = new AggregateCommit(aggregateRootId, rev, @events, publicEvents, commitRaw.Timestamp.ToFileTime());
                        aggregateCommits.Add(sourceCommit);
                    }

                    EventStream eventStream = new EventStream(aggregateCommits);
                    IntegrityResult<EventStream> integrityResult = _integrityPolicy.Apply(eventStream);

                    if (integrityResult.IsIntegrityViolated)
                    {
                        _logger.LogInformation("--------------- Integrity violated for {id}", aggregateRootIdAsString);
                        foreach (AggregateCommitRaw commit in stream.Commits)
                        {
                            foreach (AggregateEventRaw rawEvent in commit.Events)
                            {
                                if (signal.IsDryRun == false)
                                    await _eventStore.DeleteAsync(rawEvent).ConfigureAwait(false);
                            }
                        }
                        _logger.LogInformation("--------------- Finished deleting {id}", aggregateRootIdAsString);
                    }
                }
            };

            await _player.EnumerateEventStore(@operator, new PlayerOptions(), CancellationToken.None);

            _logger.LogInformation("Finished deleting aggreagtes with violated integrity...");
        }
    }
}
