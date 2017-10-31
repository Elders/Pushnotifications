using System;
using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications.Projections
{
    [DataContract(Name = "dfe9296a-e9b5-4000-938e-e097e0eec8de")]
    public class TestProjection : ProjectionDefinition<object, SubscriberId>, IProjection,
        IEventHandler<PushNotificationWasSent>
    {
        public TestProjection()
        {
            Subscribe<PushNotificationWasSent>(x => x.SubscriberId);

        }

        public void Handle(PushNotificationWasSent @event)
        {
            throw new NotImplementedException();
        }
    }
}
