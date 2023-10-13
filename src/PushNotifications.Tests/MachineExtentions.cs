using System;
using System.Linq;
using Elders.Cronus;
using Machine.Specifications;

namespace PushNotifications.Tests
{
    public static class MachineExtentions
    {
        public static IAggregateRoot ShouldHaveEvent<TExpectedEvent>(this IAggregateRoot ar, Action<TExpectedEvent> eventAssertions = null) where TExpectedEvent : IEvent
        {
            ar.UncommittedEvents.ShouldContain(@event => @event is TExpectedEvent);
            if (eventAssertions is null == false)
            {
                var gg = ar.UncommittedEvents.Last(x => x is TExpectedEvent);
                eventAssertions((TExpectedEvent)gg);
            }
            return ar;
        }

        public static IAggregateRoot ShouldNotHaveEvent<TExpectedEvent>(this IAggregateRoot ar, Action<TExpectedEvent> eventAssertions = null) where TExpectedEvent : IEvent
        {
            if (eventAssertions is null)
            {
                ar.UncommittedEvents.ShouldNotContain(@event => @event is TExpectedEvent);
                return ar;
            }

            var gg = ar.UncommittedEvents.LastOrDefault(x => x is TExpectedEvent);
            if (gg is null)
                return ar;

            eventAssertions((TExpectedEvent)gg);

            return ar;
        }

        public static void ShouldHaveEventsCount<TExpectedEvent>(this IAggregateRoot ar, int count) where TExpectedEvent : IEvent
        {
            ar.UncommittedEvents.Count(@event => @event is TExpectedEvent).ShouldEqual(count);
        }
    }
}
