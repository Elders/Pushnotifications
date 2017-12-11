using System;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Contracts
{
    public interface IPushNotificationAggregator : IDisposable
    {
        bool Queue(SubscriptionToken token, NotificationForDelivery notification);
    }
}
