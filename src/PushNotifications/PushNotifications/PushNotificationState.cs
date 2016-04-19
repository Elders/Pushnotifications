using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;
using System;

namespace PushNotifications.PushNotifications
{
    public class PushNotificationState : AggregateRootState<PushNotification, PushNotificationId>
    {
        public override PushNotificationId Id { get; set; }

        public Payload Payload { get; set; }

        public void When(PushNotificationWasSent e)
        {
            if (ReferenceEquals(null, e)) throw new ArgumentNullException("e");

            Id = e.Id;
            Payload = new Payload(e.Json, e.Text, e.Sound, e.Icon, e.Badge, e.IsSilent);
        }
    }
}