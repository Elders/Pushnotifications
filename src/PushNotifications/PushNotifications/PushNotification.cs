﻿using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications
{
    public class PushNotification : AggregateRoot<PushNotificationState>
    {
        PushNotification() { }

        public PushNotification(PushNotificationId id, SubscriberId subscriberId, NotificationPayload notificationPayload)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, notificationPayload) == true) throw new ArgumentException(nameof(notificationPayload));

            state = new PushNotificationState();

            IEvent @event = new PushNotificationSent(id, subscriberId, notificationPayload);
            Apply(@event);
        }
    }
}
