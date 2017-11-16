using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Client.Models
{
    public class PushNotificationSendModel
    {
        public PushNotificationSendModel(StringTenantUrn subscriberUrn, string title, string body, string sound, string icon, int badge, Timestamp expiresAt, bool contentAvailable)
        {
            Tenant = subscriberUrn.Tenant;
            SubscriberUrn = subscriberUrn;
            Title = title;
            Body = body;
            Sound = sound;
            Icon = icon;
            Badge = badge;
            ExpiresAt = expiresAt;
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
