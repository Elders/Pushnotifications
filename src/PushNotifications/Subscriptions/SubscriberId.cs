using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions
{
    [DataContract(Name = "ece0a261-027e-479b-9ce3-530d1acd8dfe")]
    public class SubscriberId : AggregateRootId
    {
        SubscriberId() { }

        public SubscriberId(AggregateUrn urn) : base("subscriber", urn) { }

        public SubscriberId(string id, string tenant) : base(id, "subscriber", tenant) { }

        public static SubscriberId NoUser => new SubscriberId("nouser", "notenant");
    }
}
