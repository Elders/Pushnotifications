using System;
using Elders.Cronus;

namespace Vapt.EventStore.Migrations
{
    public class MigrationEvent
    {
        public MigrationEvent(IEvent raw)
        {
            if (ReferenceEquals(null, raw)) throw new ArgumentNullException(nameof(raw));
            Raw = raw;
        }

        public IEvent Raw { get; set; }

        /// <summary>
        /// Normalized
        /// </summary>
        IEvent MigratedEvent { get; set; }

        bool NeedsWrapping { get; set; }

        public bool IsMigrated { get; set; }

        public IEvent GetNormalizedVersion()
        {
            return Normalize(Raw);
        }

        public void ApplyMigration(Func<IEvent> migration)
        {
            MigratedEvent = migration();
            IsMigrated = true;
        }

        public IEvent GetEventStoreVersion()
        {
            if (NeedsWrapping)
                return Wrap(MigratedEvent, Raw);

            return Raw;
        }

        IEvent Wrap(IEvent normalizedEvent, IEvent rawEvent)
        {
            var entityId = ((EntityEvent)rawEvent).EntityId;
            var Event = new EntityEvent(entityId, normalizedEvent);
            return Event;
        }

        IEvent Normalize(IEvent @event)
        {
            var entityEventContractId = typeof(EntityEvent).GetContractId();
            var currentEventContractId = @event.GetType().GetContractId();
            var shouldUnwrap = currentEventContractId.Equals(entityEventContractId);
            var Event = @event;

            if (shouldUnwrap)
            {
                Event = ((EntityEvent)@event).Event;
                NeedsWrapping = true;
            }

            return Event;
        }
    }
}
