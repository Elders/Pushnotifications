using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using System.ComponentModel.DataAnnotations;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Commands;
using PushNotifications.Contracts.PushNotifications;
using System;

namespace PushNotifications.Api.Controllers.Subscriptions.Commands
{
    public class PushNotificationSendModel
    {
        public PushNotificationSendModel()
        {
            ExpiresAt = Timestamp.JudgementDay();
        }

        [AuthorizeClaim(AuthorizeClaimType.Tenant, AuthorizeClaimType.TenantClient)]
        public string Tenant { get; set; }

        [Required]
        public StringTenantUrn SubscriberUrn { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public string Sound { get; set; }

        public string Icon { get; set; }

        public int Badge { get; set; }

        public Timestamp ExpiresAt { get; set; }

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
