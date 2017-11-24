using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Client.Models
{
    public class SendPushNotificationModel
    {
        public SendPushNotificationModel(StringTenantUrn subscriberUrn, string title, string body, string sound, string icon, int badge, DateTime expiresAtUtc, bool contentAvailable)
        {
            if (ReferenceEquals(subscriberUrn, null) == true) throw new ArgumentNullException(nameof(subscriberUrn));
            if (ReferenceEquals(expiresAtUtc, null) == true) throw new ArgumentNullException(nameof(expiresAtUtc));
            if (string.IsNullOrEmpty(title) == true) throw new ArgumentNullException(nameof(title));
            if (string.IsNullOrEmpty(body) == true) throw new ArgumentNullException(nameof(body));

            Tenant = subscriberUrn.Tenant;
            SubscriberUrn = subscriberUrn;
            Title = title;
            Body = body;
            Sound = sound;
            Icon = icon;
            Badge = badge;
            ExpiresAt = new Timestamp(expiresAtUtc);
            ContentAvailable = contentAvailable;
        }

        public string Tenant { get; private set; }

        public StringTenantUrn SubscriberUrn { get; private set; }

        public string Title { get; private set; }

        public string Body { get; private set; }

        public string Sound { get; private set; }

        public string Icon { get; private set; }

        public int Badge { get; private set; }

        public Timestamp ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }
    }
}
