using System;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Delivery.Pushy
{
    public class PushyDelivery : IPushNotificationDeliver
    {
        public void Send(NotificationDelivery notification)
        {
            throw new NotImplementedException();
        }
    }
}
