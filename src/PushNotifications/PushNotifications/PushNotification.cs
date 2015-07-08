using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.PushNotifications
{
    public class PushNotification : AggregateRoot<PushNotificationState>
    {
        PushNotification() { }

        public PushNotification(PushNotificationId id, string userId, string json, string text, string sound, string icon, string category, int badge)
        {
            if (ReferenceEquals(null, id)) throw new ArgumentNullException("id");

            state = new PushNotificationState();

            IEvent evnt = new PushNotificationWasSent(id, userId, json, text, sound, icon, category, badge);
            Apply(evnt);
        }
    }
}