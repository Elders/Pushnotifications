using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Commands;
using PushNotifications.Contracts.PushNotifications;
using System;

namespace PushNotifications.Api.Controllers.PushNotifications.Models
{
    public class SendPushNotificationModel
    {
        public SendPushNotificationModel()
        {
            ExpiresAt = Timestamp.JudgementDay();
        }

        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        /// <summary>
        /// URN of who should PN be send to. This must be string tenant urn
        /// </summary>
        [Required]
        public StringTenantUrn SubscriberUrn { get; set; }

        /// <summary>
        /// The notification's title.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The notification's body text
        /// </summary>
        [Required]
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

        public SendPushNotification AsCommand()
        {
            var id = new PushNotificationId(Guid.NewGuid().ToString(), Tenant);
            var subscriberId = new SubscriberId(SubscriberUrn);
            var notificationPayload = new NotificationPayload(Title, Body, Sound, Icon, Badge);
            return new SendPushNotification(id, subscriberId, notificationPayload, ExpiresAt, ContentAvailable);
        }
    }
}
