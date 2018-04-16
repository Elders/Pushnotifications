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
            if (ReferenceEquals(null, eventAssertions) == false)
            {
                var gg = ar.UncommittedEvents.Last(x => x is TExpectedEvent);
                eventAssertions((TExpectedEvent)gg);
            }
            return ar;
        }

        public static IAggregateRoot ShouldNotHaveEvent<TExpectedEvent>(this IAggregateRoot ar, Action<TExpectedEvent> eventAssertions = null) where TExpectedEvent : IEvent
        {
            if (ReferenceEquals(null, eventAssertions))
            {
                ar.UncommittedEvents.ShouldNotContain(@event => @event is TExpectedEvent);
                return ar;
            }

            var gg = ar.UncommittedEvents.LastOrDefault(x => x is TExpectedEvent);
            if (ReferenceEquals(null, gg))
                return ar;

            eventAssertions((TExpectedEvent)gg);

            return ar;
        }
    }
}
