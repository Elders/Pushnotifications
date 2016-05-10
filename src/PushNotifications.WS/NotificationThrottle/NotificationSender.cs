using System;
using System.Collections.Generic;
using System.Linq;
using PushSharp.Core;
using PushSharp.Apple;
using PushSharp.Android;
using PushNotifications.Ports.Parse;

namespace PushNotifications.WS.NotificationThrottle
{
    public class NotificationSender : IDisposable
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(NotificationSender));

        IPushBroker broker;
        Dictionary<Type, IPushChannel> channels;

        public NotificationSender(IPushBroker broker)
        {
            this.broker = broker;
            this.channels = new Dictionary<Type, IPushChannel>();
        }

        public void SendNotification(INotification notification)
        {
            var notificationType = notification.GetType();

            if (!channels.ContainsKey(notificationType))
            {
                if (notification is AppleNotification)
                {
                    broker.QueueNotification<AppleNotification>(notification as AppleNotification);
                }
                else if (notification is GcmNotification)
                {
                    broker.QueueNotification<GcmNotification>(notification as GcmNotification);
                }
                else if (notification is ParseAndroidNotifcation)
                {
                    broker.QueueNotification<ParseAndroidNotifcation>(notification as ParseAndroidNotifcation);
                }
                else
                    throw new NotSupportedException(notification.GetType().ToString());
            }

        }

        private void CreateChannel(IPushService registration, Type t)
        {
            if (channels.ContainsKey(t) == false)
            {
                var settings = registration.ChannelSettings;

                var channel = registration.PushChannelFactory.CreateChannel(settings);

                channels.Add(t, channel);
            }
        }

        private void SendNotificationCallbackDelegate(object sender, SendNotificationResult result)
        {
            if (result.IsSuccess)
            {
                log.Info("Success sending notification for token: " + GetNotificationToken(result) + "Notification " + result.Notification.ToString(), result.Error);
            }
            else
            {
                log.Error("Fail to send notification  for token:" + GetNotificationToken(result) + "Error " + result.Error.ToString() + "Notification " + result.Notification.ToString(), result.Error);
            }
        }

        private string GetNotificationToken(SendNotificationResult result)
        {
            var token = string.Empty;
            var notification = result.Notification;

            if (notification is AppleNotification)
                token = (notification as AppleNotification).DeviceToken;
            else if (notification is GcmNotification)
                token = (notification as GcmNotification).RegistrationIds.Single();
            else if (notification is ParseAndroidNotifcation)
                token = (notification as ParseAndroidNotifcation).Token;

            return token;
        }

        public void Dispose()
        {
            if (channels != null)
            {
                foreach (var item in channels)
                {
                    item.Value.Dispose();
                }
            }
            channels = null;
        }
    }
}
