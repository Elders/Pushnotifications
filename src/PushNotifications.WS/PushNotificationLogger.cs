using System;
using PushSharp.Core;

namespace PushNotifications.WS
{
    public static class PushNotificationLogger
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(PushNotificationLogger));
        public static string TOKEN;

        public static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            log.Info("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        public static void NotificationSent(object sender, INotification notification)
        {
            log.Info("Sent: " + sender + " -> " + notification);
        }

        public static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            log.Error("Failure: " + sender + " -> " + notification + "TOKEN:" + TOKEN, notificationFailureException);
        }

        public static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            log.Error("Channel Exception: " + sender, exception);
        }

        public static void ServiceException(object sender, Exception exception)
        {
            log.Error("Channel Exception: " + sender, exception);
        }

        public static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            log.Warn("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        public static void ChannelDestroyed(object sender)
        {
            log.Warn("Channel Destroyed for: " + sender);
        }

        public static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            log.Info("Channel Created for: " + sender);
        }
    }
}