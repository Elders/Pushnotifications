using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions
{
    [DataContract(Name = "ece0a261-027e-479b-9ce3-530d1acd8dfe")]
    public class DeviceSubscriberId : AggregateRootId
    {
        DeviceSubscriberId() { }

        //public SubscriberId(AggregateUrn urn) : base("subscriber", urn) { }

        public DeviceSubscriberId(string id, string tenant, string application) : base(id, GetAggregateRootName(application), tenant)
        {
            Application = application;
        }

        public static DeviceSubscriberId NoUser => new DeviceSubscriberId("nouser", "notenant", "noapplication");

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
