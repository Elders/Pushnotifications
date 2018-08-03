//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using Elders.Cronus;
//using Elders.Cronus.EventStore;
//using Elders.Cronus.Migration.Middleware;
//using log4net;
//using Vapt.EventStore.Migrations.Extensions;

//namespace Vapt.EventStore.Migrations
//{
//    public class TimestampMigration : IMigration<AggregateCommit, AggregateCommit>
//    {
//        static ILog log = LogManager.GetLogger(typeof(TimestampMigration));

//        static List<string> LogPropertyFilters = new List<string> { "id", "timestamp" };

//        public AggregateCommit Apply(AggregateCommit current)
//        {
//            long arCommitTimestamp = current.Timestamp;
//            var newAggregateCommit = new AggregateCommit(current.AggregateRootId, current.BoundedContext, current.Revision, new List<IEvent>());

//            foreach (var @event in current.Events.Normalize())
//            {
//                var normalizedEvent = @event.GetNormalizedVersion();
//                var feedIdProp = normalizedEvent.GetType().GetProperty("Feed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
//                if (ReferenceEquals(null, feedIdProp) == true)
//                    throw new DataMisalignedException($"Missing feed id property for event: {normalizedEvent.GetPropertiesAsString()}");

//                var feedIdPropValue = feedIdProp.GetValue(normalizedEvent) as FeedId;
//                if (ReferenceEquals(null, feedIdPropValue) == true)
//                    throw new DataMisalignedException($"Missing feed id value for event: {normalizedEvent.GetPropertiesAsString()}");

//                var timestampProp = normalizedEvent.GetType().GetProperty("Timestamp", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
//                if (ReferenceEquals(null, timestampProp) == true)
//                    throw new DataMisalignedException($"Missing timestamp property for event: {normalizedEvent.GetPropertiesAsString()}");

//                var timestampPropValue = timestampProp.GetValue(normalizedEvent) as Timestamp;


//                if (ReferenceEquals(null, timestampPropValue) == true)
//                    @event.ApplyMigration(() => FixTimestamp(normalizedEvent, arCommitTimestamp, timestampProp));

//                var forStorage = @event.GetEventStoreVersion();
//                newAggregateCommit.Events.Add(forStorage);
//            }

//            return newAggregateCommit;
//        }

//        IEvent FixTimestamp(IEvent @event, long arCommitTimestamp, PropertyInfo timestampProperty)
//        {
//            var dt = DateTime.FromFileTimeUtc(arCommitTimestamp);
//            log.Info($"Applying timestamp = {dt} {@event.GetPropertiesAsString(LogPropertyFilters)}");
//            timestampProperty.SetValue(@event, new Chat.Timestamp(dt));

//            return @event;
//        }

//        public bool ShouldApply(AggregateCommit current)
//        {
//            foreach (var @event in current.Events)
//            {
//                try
//                {
//                    var Event = @event.Unwrap();

//                    var timestampProp = Event.GetType().GetProperty("Timestamp", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
//                    if (ReferenceEquals(null, timestampProp) == false)
//                    {
//                        var timestampPropValue = timestampProp.GetValue(Event) as Timestamp;
//                        // if timestamp has no value then we should migrate
//                        if (ReferenceEquals(null, timestampPropValue) == true)
//                            return true;
//                    }
//                }
//                catch (Exception ex)
//                {

//                    log.Error(ex);
//                }
//            }
//            return false;
//        }
//    }
//}
