using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;
using System;

namespace PushNotifications.PushNotifications
{
    public class PushNotification : AggregateRoot<PushNotificationState>
    {
        PushNotification() { }

        public PushNotification(PushNotificationId id, string userId, string json, string text, string sound, string icon, string category, int badge, bool isSilent)
        {
            if (ReferenceEquals(null, id)) throw new ArgumentNullException(nameof(id));

            state = new PushNotificationState();

            IEvent evnt = new PushNotificationWasSent(id, userId, json, text, sound, icon, category, badge, isSilent);
            Apply(evnt);
        }
    }
}