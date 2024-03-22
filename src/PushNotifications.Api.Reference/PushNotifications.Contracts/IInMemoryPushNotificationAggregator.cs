using System;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Contracts
{
    public interface IPushNotificationAggregator : IDisposable
    {
        SendTokensResult Queue(SubscriptionToken token, NotificationForDelivery notification);
    }
}
