using Elders.Cronus;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts.PushNotifications;
using System;
using System.Collections.Generic;
using PushNotifications.Subscriptions;
using PushNotifications.Contracts.PushNotifications.Delivery;
using System.Linq;

namespace PushNotifications.Api.Controllers.PushNotifications.Models
{
    public class SendPushNotificationModel
    {
        public SendPushNotificationModel()
        {
            ExpiresAt = Timestamp.JudgementDay();
            NotificationData = new Dictionary<string, string>();
        }

        /// <summary>
        /// URN of who should PN be send to. This must be string tenant urn
        /// </summary>
        [Required]
        public string SubscriberUrn { get; set; }

        /// <summary>
        /// The notification's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The notification's body text
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The sound to play when the device receives the notification
        /// </summary>
        public string Sound { get; set; }

        /// <summary>
        /// The notification's icon. For iOS use Badge
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The value of the badge on the home screen app icon. If not specified, the badge is not changed. If set to 0, the badge is removed. Works for iOS
        /// </summary>
        public int Badge { get; set; }

        /// <summary>
        /// This parameter specifies at what time the message should be discarded if not send. By default this is set too current time plus 100 years
        /// </summary>
        public Timestamp ExpiresAt { get; set; }

        /// <summary>
        /// On iOS, use this field to represent content-available in the APNs payload. When a notification or message is sent and this is set to true, an inactive client app is awoken, and the message is sent through APNs as a silent notification and not through the FCM connection server. Note that silent notifications in APNs are not guaranteed to be delivered, and can depend on factors such as the user turning on Low Power Mode, force quitting the app, etc. On Android, data messages wake the app by default. On Chrome, currently not supported
        /// </summary>
        public bool ContentAvailable { get; set; }

        public PushNotificationId MessageId { get; set; }

        public string Application { get; set; }

        /// <summary>
        /// The payload data
        /// </summary>
        public Dictionary<string, string> NotificationData { get; set; }

        public NotificationMessageSignal AsSignal()
        {
            var subscriber = AggregateUrn.Parse(SubscriberUrn, Urn.Uber);
            var subscriberId = new DeviceSubscriberId(subscriber.Id, subscriber.Tenant, Application);
            var notificationPayload = new NotificationPayload(Title, Body, Sound, Icon, Badge);
            var target = new NotificationTarget(subscriber.Tenant, Application);

            return new NotificationMessageSignal(subscriberId, notificationPayload, NotificationData.ToDictionary(x => x.Key, y => y.Value as object), DateTimeOffset.FromFileTime(ExpiresAt.FileTimeUtc), ContentAvailable, target);
        }
    }

    public class Timestamp
    {
        private Timestamp() { }

        public Timestamp(DateTime dateTime)
        {
            if (ReferenceEquals(null, dateTime) == true) throw new ArgumentNullException(nameof(dateTime));
            if (dateTime.Kind != DateTimeKind.Utc) throw new ArgumentException("All timestamps should be utc!");

            FileTimeUtc = dateTime.ToFileTimeUtc();
        }

        public long FileTimeUtc { get; set; }

        public DateTime DateTime { get { return DateTime.FromFileTimeUtc(FileTimeUtc); } }

        public static Timestamp UtcNow()
        {
            return new Timestamp(DateTime.UtcNow);
        }

        public static Timestamp JudgementDay()
        {
            return new Timestamp(DateTime.UtcNow.AddYears(100));
        }
    }
}
