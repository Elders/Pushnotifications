using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions
{
    [DataContract(Name = "ece0a261-027e-479b-9ce3-530d1acd8dfe")]
    public class SubscriberId : AggregateRootId
    {
        SubscriberId() { }

        //public SubscriberId(AggregateUrn urn) : base("subscriber", urn) { }

        public SubscriberId(string id, string tenant, string application) : base(id, GetAggregateRootName(application), tenant)
        {
            Application = application;
        }

        public static SubscriberId NoUser => new SubscriberId("nouser", "notenant", "noapplication");

        [DataMember(Order = 1)]
        public string Application { get; private set; }

        private static string GetAggregateRootName(string application)
        {
            if (string.IsNullOrEmpty(application))
            {
                return "subscriber";
            }
            else
            {
                return $"subscriber-{application}";
            }
        }
    }
}
